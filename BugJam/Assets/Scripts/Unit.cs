using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Tile currentTile;
    public Owner owner;
    public int[,] attackPattern;
    public bool available;
    [SerializeField] Material availableMaterial;
    [SerializeField] Material unavailableMaterial;
    [SerializeField] TextMeshProUGUI healthDisplay;
    [SerializeField] TextMeshProUGUI attackDisplay;

    public int moveRange = 1;
    public int health;
    public int damage;

    void Start() {
        available = true;
        SetAvailable();
        healthDisplay.text = health.ToString();
        attackDisplay.text = damage.ToString();
    }


    public void SetAttackPattern(int[,] pattern) {
        attackPattern = pattern;
        attackPattern = Utils.RotateMatrix90Degrees(attackPattern);
    }

    public void MoveTo(int x, int y) {
        currentTile.unit = null;
        currentTile = Grid.instance.grid[x, y];
        currentTile.unit = this;
        transform.position = currentTile.unitPosition.position;
        SetUnavailable();
    }

    public void Attack(Unit target) {
        target.TakeDamage(damage);
    }

    public void TakeDamage(int dmg) {
        health -= dmg;
        if (health <= 0) {
            DestroySelf();
        }

        healthDisplay.text = health.ToString();
    }

    public void SetAvailable() {
        available = true;
        GetComponentInChildren<Renderer>().material = availableMaterial;
    }

    public void SetUnavailable() {
        available = false;
        GetComponentInChildren<Renderer>().material = unavailableMaterial;
    }

    public List<Tile> GetAttackableTiles() {
        List<Tile> attackable = new List<Tile>();

        List<(int, int)> targetCoordinates = Utils.MatrixMask(Grid.instance.grid, (currentTile.x, currentTile.y), attackPattern);

        foreach ((int, int) coordinate in targetCoordinates) {
            Unit u = Grid.instance.grid[coordinate.Item1, coordinate.Item2].unit;
            if ((u != null && u.owner != owner) || u == null) {
                // check if target tile is visible
                Vector3 start = currentTile.unitPosition.position;
                Vector3 end = Grid.instance.grid[coordinate.Item1, coordinate.Item2].unitPosition.position;
                float distance = (end - start).magnitude;
                if (Physics.Raycast(start, (end - start).normalized, out var hit, distance, ~LayerMask.NameToLayer("Obstacle"))) {
                    //obstacle detected, attack not possible
                    //Debug.DrawLine(start, hit.point, Color.red);    
                }
                else {
                    attackable.Add(Grid.instance.grid[coordinate.Item1, coordinate.Item2]);
                    //Debug.DrawLine(start, end, Color.green);   
                }
            }
        }

        return attackable;
    }

    public List<Tile> GetMovableTiles() {
        List<Tile> reachable = Grid.instance.GetReachableInRange(currentTile, moveRange);

        List<Tile> toRemove = new List<Tile>();
        foreach (Tile tile in reachable) {
            if (tile.unit != null) {
                toRemove.Add(tile);
            }
        }

        reachable = reachable.Except(toRemove).ToList();

        return reachable;
    }

    public List<AttackCommand> GetAvailableAttacks() {
        List<AttackCommand> attacks = new List<AttackCommand>();

        foreach (Tile attackableTile in GetAttackableTiles()) {
            if (attackableTile.unit != null && attackableTile.unit.owner != owner) {
                attacks.Add(new AttackCommand(this, attackableTile.unit));
            }
        }

        return attacks;
    }

    public List<MoveUnitCommand> GetAvailableMoves() {
        List<MoveUnitCommand> moves = new List<MoveUnitCommand>();

        List<Tile> reachable = Grid.instance.GetReachableInRange(currentTile, moveRange);


        foreach (Tile tile in reachable) {
            //Debug.Log(tile);
            // possible moves
            if (tile.unit == null) {
                moves.Add(new MoveUnitCommand(this, tile.x, tile.y));
            }
        }

        moves.Add(new MoveUnitCommand(this, currentTile.x, currentTile.y));

        return moves;
    }

    public void DestroySelf() {
        EventManager.instance.UnitDestroyed(this);
        Destroy(gameObject);
    }

    public enum Owner {
        PLAYER,
        ENEMY
    }
}