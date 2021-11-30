using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartAI : EnemyAI {
    int value;

    public override IEnumerator MakeMove() {
        foreach (Unit unit in units) {
            yield return new WaitUntil(() => !GameManager.instance.moveInProgress);
            // 1. MOVE UNIT
            List<MoveUnitCommand> possibleMoves = unit.GetAvailableMoves().OrderBy(a => Guid.NewGuid()).ToList(); //randomize order

            MoveUnitCommand bestMove = possibleMoves[0];
            int bestValue = 0;
            foreach (MoveUnitCommand moveUnitCommand in possibleMoves) {
                //int value = CalculateMoveValue(moveUnitCommand);
                yield return CalculateMoveValue(moveUnitCommand);
                if (value > bestValue) {
                    bestMove = moveUnitCommand;
                    bestValue = value;
                }
            }


            Tile t = Grid.instance.grid[bestMove.x, bestMove.y];
            /*GameManager.instance.DisplayLine(unit.currentTile, t, Color.blue);
            GameManager.instance.ResetLine();*/
            bestMove.ExecuteAnimate();


            yield return new WaitUntil(() => !GameManager.instance.moveInProgress);
            //yield return new WaitForSeconds(0.2f);


            // 2. MOVE OR ATTACK
            possibleMoves = unit.GetAvailableMoves().OrderBy(a => Guid.NewGuid()).ToList(); //randomize order
            List<AttackCommand> possibleAttacks = unit.GetAvailableAttacks();

            if (possibleAttacks.Count > 0) {
                GameManager.instance.StraightLine(unit.currentTile, possibleAttacks[0].target.currentTile, Color.red);
                //yield return new WaitForSeconds(0.3f);
                GameManager.instance.ResetLine();
                possibleAttacks[0].ExecuteAnimate();
            }
            else {
                foreach (MoveUnitCommand moveUnitCommand in possibleMoves) {
                    //int value = CalculateMoveValue(moveUnitCommand);
                    yield return CalculateMoveValue(moveUnitCommand);
                    if (value > bestValue) {
                        bestMove = moveUnitCommand;
                        bestValue = value;
                    }
                }

                bestMove.ExecuteAnimate();
            }


            /*
            foreach (MoveUnitCommand command in possibleMoves) {
                command.DisplayCommand();
            }

            foreach (Tile tile in unit.GetAttackableTiles()) {
                tile.DisplayAttack();
            }
            */

            //GameManager.instance.ResetLine();


            foreach (Tile tile in Grid.instance.grid) {
                if (tile != null && tile.walkable) {
                    tile.ClearDisplays();
                }
            }

            //yield return new WaitForSeconds(0.5f);
            //GameManager.instance.ResetLine();
            yield return new WaitForEndOfFrame();
        }

        GameManager.instance.wantsToEndTurn = true;
    }

    //TODO: make number of simulation turns a parameter
    IEnumerator CalculateMoveValue(MoveUnitCommand command) {
        //Time.timeScale = 0;
        int calculatedValue = 0;

        // execute command
        command.Execute();

        // check potential player moves

        bool canHitEnemy = false;
        bool canBeHit = false;
        
        foreach (Unit playerUnit in GameManager.instance.player.units) {
            List<MoveUnitCommand> playerMoves = playerUnit.GetAvailableMoves();
            foreach (MoveUnitCommand playerMove in playerMoves) {
                playerMove.Execute();

                List<MoveUnitCommand> playerSecondMoves = playerUnit.GetAvailableMoves();
                foreach (MoveUnitCommand secondMove in playerSecondMoves) {
                    secondMove.Execute();
                    // add value if unit can hit player
                    if (command.unit.GetAvailableAttacks().Count > 0) {
                        //calculatedValue += command.unit.attackAiValue;
                        canHitEnemy = true;
                        break;
                    }

                    // subtract value if unit can be hit by player
                    if (playerUnit.GetAttackableTiles().Contains(command.unit.currentTile)) {
                        calculatedValue += command.unit.dangerAiValue;
                    }

                    secondMove.Undo();
                }

                playerMove.Undo();
            }

            yield return null;
        }

        if (canHitEnemy) {
            calculatedValue += command.unit.attackAiValue;
        }
        
        if (Grid.instance.grid[command.x, command.y] == command.unit.currentTile) {
            calculatedValue -= 10;
        }

        command.Undo();

        //Time.timeScale = 1f;
        //Debug.Log("VALUE: " + calculatedValue);
        value = calculatedValue;
    }
}