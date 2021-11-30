using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
        ResetSelection();
        GameManager.instance.ResetLine();
        
        foreach (Unit unit in units) {
            unit.SetUnavailable();
        }

        GameManager.instance.wantsToEndTurn = true;
        GameManager.instance.state = GameManager.GameState.ENEMY_TURN;
    }


    void Update() {
        if (GameManager.instance.state == GameManager.GameState.ENEMY_TURN) return;

        if (!HasAvailableUnit()) {
            EndTurn();
            return;
        }

        UpdateMouseInput();

        if (Input.GetKeyDown(KeyCode.Space)) {
            EndTurn();
        }


        //click
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
            //clicked on a valid tile
            if (tileUnderMouse != null && tileUnderMouse.walkable) {
                //clicked on a unit
                if (tileUnderMouse.Unit != null) {
                    //clicked on own unit
                    
                    if (units.Contains(tileUnderMouse.Unit)) {
                        if (selected != null) {
                            ResetSelection();
                        }
                        else if (tileUnderMouse.Unit.available) {
                            ResetSelection();
                            //select unit
                            selected = tileUnderMouse.Unit;
                            
                            //if (tileUnderMouse.Unit.canMove) {
                                //display move range
                                selectedCommands = selected.GetAvailableMoves();
                                foreach (MoveUnitCommand command in selectedCommands) {
                                    command.DisplayCommand();
                                }    
                            //}
                            
                            // display attack range
                            foreach (Tile tile in selected.GetAttackableTiles()) {
                                tile.DisplayAttack();
                            }
                        }
                    }
                    //clicked on enemy unit
                    else {
                        ResetGridDisplay();
                        if (selected != null) {
                            //check if attackable
                            var possibleAttacks = selected.GetAvailableAttacks();
                            //Debug.Log(possibleAttacks.Count);
                            foreach (AttackCommand possibleAttack in possibleAttacks) {
                                if (possibleAttack.target == tileUnderMouse.Unit) {
                                    possibleAttack.ExecuteAnimate();

                                    ResetSelection();
                                }
                            }
                        }
                        else {
                            ResetSelection();
                            //display possible enemy moves
                            foreach (MoveUnitCommand command in tileUnderMouse.Unit.GetAvailableMoves()) {
                                command.DisplayCommand();
                            }

                            foreach (Tile tile in tileUnderMouse.Unit.GetAttackableTiles()) {
                                tile.DisplayAttack();
                            }
                        }
                    }
                } //clicked on empty tile
                else {
                    //selected unit
                    if (selected != null && selectedCommands != null) {
                        foreach (MoveUnitCommand command in selectedCommands) {
                            //found clicked tile command
                            if (command.x == tileUnderMouse.x && command.y == tileUnderMouse.y) {
                                command.ExecuteAnimate();

                                ResetSelection();
                            }
                        }
                    }

                    ResetSelection();
                }
            }
            else {
                ResetSelection();
            }
        }


        GameManager.instance.ResetLine();
        if (selected != null && tileUnderMouse != null && tileUnderMouse.Unit != selected) {
            //check if tile is in movement range
            if (selected.GetMovableTiles().Contains(tileUnderMouse)) {
                //check if selected can reach tile
                List<Tile> path = Grid.instance.GetPath(selected.currentTile, tileUnderMouse);
                if (path != null) {
                    // draw path to tileundermouse
                    GameManager.instance.DisplayLine(selected.currentTile, tileUnderMouse, Color.blue);
                }
            }
        }
    }

    void ResetSelection() {
        selected = null;
        selectedCommands = null;
        ResetGridDisplay();
    }

    void RemoveUnit(Unit unit) {
        if (units.Contains(unit)) {
            units.Remove(unit);
        }
    }

    bool HasAvailableUnit() {
        return units.Select(unit => unit.available).Contains(true);
    }

    public void ResetGridDisplay() {
        foreach (Tile tile in Grid.instance.grid) {
            if (tile != null) {
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