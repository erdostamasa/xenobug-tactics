using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    public Tile currentTile;
    public Owner owner;
    public int[,] attackPattern;
    public bool available;
    [SerializeField] Material availableMaterial;
    [SerializeField] Material unavailableMaterial;

    public int range = 2;
    public int health;
    public int damage;

    void Start() {
        attackPattern = new int[,] {
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
            { 1, 1, 0, 1, 1 },
            { 0, 0, 1, 0, 0 },
            { 0, 0, 1, 0, 0 },
        };
        attackPattern = Utils.RotateMatrix90Degrees(attackPattern);
        available = true;
    }

    public void MoveTo(int x, int y) {
        currentTile.unit = null;
        currentTile = Grid.instance.grid[x, y];
        currentTile.unit = this;
        transform.position = currentTile.unitPosition.position;
    }

    public void Attack(Unit target) {
        target.Destroy();
    }

    public void SetAvailable() {
        available = true;
        GetComponent<Renderer>().material = availableMaterial;
    }
    
    public void SetUnavailable() {
        available = false;
        GetComponent<Renderer>().material = unavailableMaterial;
    }

    public List<Tile> GetAttackableTiles() {
        List<Tile> attackable = new List<Tile>();

        List<(int, int)> targetCoordinates = Utils.MatrixMask(Grid.instance.grid, (currentTile.x, currentTile.y), attackPattern);

        foreach ((int, int) coordinate in targetCoordinates) {
            attackable.Add(Grid.instance.grid[coordinate.Item1, coordinate.Item2]);
        }

        return attackable;
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

        List<Tile> reachable = Grid.instance.GetReachableInRange(currentTile, range);


        foreach (Tile tile in reachable) {
            //Debug.Log(tile);
            // possible moves
            if (tile.unit == null) {
                moves.Add(new MoveUnitCommand(this, tile.x, tile.y));
            }
            else {
                //possible attacks
                if (tile.unit.owner != owner) {
                    //      moves.Add(new AttackCommand(this, tile.unit));
                }
            }
        }


        return moves;
    }

    public void Destroy() {
        EventManager.instance.UnitDestroyed(this);
        Destroy(gameObject);
    }

    public enum Owner {
        PLAYER,
        ENEMY
    }
}