using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour {
    [SerializeField] LayerMask tileLayer;
    Tile tileUnderMouse;
    public List<Unit> units;


    Unit selected;
    List<MoveUnitCommand> selectedCommands;

    void Start() {
        EventManager.instance.onUnitDestroyed += RemoveUnit;
    }

    public void EndTurn() {
        foreach (Unit unit in units) {
            unit.SetUnavailable();
        }
        GameManager.instance.state = GameManager.GameState.ENEMY_TURN;
    }

    void Update() {
        if (GameManager.instance.state == GameManager.GameState.ENEMY_TURN) return;
        if (!HasAvailableUnit()) {
            GameManager.instance.state = GameManager.GameState.ENEMY_TURN;
            return;
        }

        UpdateMouseInput();


        //click
        if (Input.GetMouseButtonDown(0)) {
            //clicked on a valid tile
            if (tileUnderMouse != null && tileUnderMouse.walkable) {
                //clicked on a unit
                if (tileUnderMouse.unit != null) {
                    //clicked on own unit
                    if (units.Contains(tileUnderMouse.unit)) {
                        //select unit
                        selected = tileUnderMouse.unit;
                        //display range
                        selectedCommands = selected.GetAvailableMoves();
                        foreach (MoveUnitCommand command in selectedCommands) {
                            command.DisplayCommand();
                        }

                        foreach (Tile tile in selected.GetAttackableTiles()) {
                            tile.DisplayAttack();
                        }
                    }
                    //clicked on enemy unit
                    else {
                        if (selected != null) {
                            //check if attackable
                            var possibleAttacks = selected.GetAvailableAttacks();
                            //Debug.Log(possibleAttacks.Count);
                            foreach (AttackCommand possibleAttack in possibleAttacks) {
                                if (possibleAttack.target == tileUnderMouse.unit) {
                                    possibleAttack.Execute();

                                    selected = null;
                                    selectedCommands = null;
                                    ResetGridDisplay();
                                }
                            }
                        }
                    }
                } //clicked on empty tile
                else {
                    //selected unit
                    if (selected != null) {
                        foreach (MoveUnitCommand command in selectedCommands) {
                            //found clicked tile command
                            if (command.x == tileUnderMouse.x && command.y == tileUnderMouse.y) {
                                command.Execute();

                                selected = null;
                                selectedCommands = null;
                                ResetGridDisplay();
                            }
                        }
                    }

                    selected = null;
                    selectedCommands = null;
                    ResetGridDisplay();
                }
            }
            else {
                selected = null;
                selectedCommands = null;
                ResetGridDisplay();
            }
        }


        //move player unit to random position
        // if (Input.GetMouseButtonDown(0)) {
        //     if (tileUnderMouse != null && tileUnderMouse.unit != null && units.Contains(tileUnderMouse.unit)) {
        //         var moves = tileUnderMouse.unit.GetAvailableMoves();
        //         int i = Random.Range(0, moves.Count);
        //         moves[i].Execute();
        //         GameManager.instance.state = GameManager.GameState.ENEMY_TURN;
        //         tileUnderMouse.Select(Random.ColorHSV());
        //     }
        // }
    }

    void RemoveUnit(Unit unit) {
        if (units.Contains(unit)) {
            units.Remove(unit);
        }
    }

    bool HasAvailableUnit() {
        return units.Select(unit => unit.available).Contains(true);
    }

    void ResetGridDisplay() {
        foreach (Tile tile in Grid.instance.grid) {
            if (tile != null && tile.walkable) {
                tile.ClearDisplays();
            }
        }
    }


    void UpdateMouseInput() {
        Vector3 mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        //Debug.DrawLine(mouseRay.origin, mouseRay.origin + mouseRay.direction * 10f, Color.red);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, 50f, tileLayer)) {
            Tile mouseOverTile = hit.collider.GetComponent<Tile>();
            if (mouseOverTile.selectable) {
                tileUnderMouse = mouseOverTile;
            }
        }
        else {
            tileUnderMouse = null;
        }
    }
}