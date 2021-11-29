using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmartAI : EnemyAI {
    public override IEnumerator MakeMove() {
        
        foreach (Unit unit in units) {
            List<MoveUnitCommand> possibleMoves = unit.GetAvailableMoves().OrderBy(a => Guid.NewGuid()).ToList(); //randomize order
            List<AttackCommand> possibleAttacks = unit.GetAvailableAttacks();

            /*
            foreach (MoveUnitCommand command in possibleMoves) {
                command.DisplayCommand();
            }

            foreach (Tile tile in unit.GetAttackableTiles()) {
                tile.DisplayAttack();
            }
            */

            GameManager.instance.ResetLine();
            if (possibleAttacks.Count > 0) {
                GameManager.instance.StraightLine(unit.currentTile, possibleAttacks[0].target.currentTile, Color.red);
                yield return new WaitForSeconds(0.3f);
                GameManager.instance.ResetLine();
                possibleAttacks[0].ExecuteAnimate();
            }
            else {
                MoveUnitCommand bestMove = possibleMoves[0];
                int bestValue = 0;
                foreach (MoveUnitCommand moveUnitCommand in possibleMoves) {
                    int value = CalculateMoveValue(moveUnitCommand);
                    if (value > bestValue) {
                        bestMove = moveUnitCommand;
                        bestValue = value;
                    }
                }

                //Debug.Log($"Best value:{bestValue}");
                Tile t = Grid.instance.grid[bestMove.x, bestMove.y];
                GameManager.instance.DisplayLine(unit.currentTile, t, Color.blue);

                yield return new WaitForSeconds(0.3f);
                GameManager.instance.ResetLine();
                bestMove.ExecuteAnimate();
            }

            foreach (Tile tile in Grid.instance.grid) {
                if (tile != null && tile.walkable) {
                    tile.ClearDisplays();
                }
            }

            yield return new WaitForSeconds(0.5f);
            GameManager.instance.ResetLine();
            yield return new WaitForEndOfFrame();
        }

        
    }

    //TODO: make number of simulation turns a parameter
    int CalculateMoveValue(MoveUnitCommand command) {
        Time.timeScale = 0;
        int value = 0;

        command.Execute();
        foreach (Unit playerUnit in GameManager.instance.player.units) {
            List<MoveUnitCommand> playerMoves = playerUnit.GetAvailableMoves();
            foreach (MoveUnitCommand playerMove in playerMoves) {
                playerMove.Execute();
                value += command.unit.GetAvailableAttacks().Count;
                playerMove.Undo();
            }
        }

        command.Undo();

        Time.timeScale = 1f;
        return value;
    }
}