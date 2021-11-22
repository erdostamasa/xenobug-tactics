using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEnemyAI : EnemyAI {
    public override IEnumerator MakeMove() {
        foreach (Unit unit in units) {
            List<MoveUnitCommand> possibleMoves = unit.GetAvailableMoves();
            List<AttackCommand> possibleAttacks = unit.GetAvailableAttacks();

            foreach (MoveUnitCommand command in possibleMoves) {
                command.DisplayCommand();
            }

            foreach (Tile tile in unit.GetAttackableTiles()) {
                tile.DisplayAttack();
            }


            // List<Command> possibleCommands = new List<Command>();
            // possibleCommands = possibleCommands.Concat(possibleMoves).ToList();
            // possibleCommands = possibleCommands.Concat(possibleAttacks).ToList();

            GameManager.instance.ResetLine();
            if (possibleAttacks.Count > 0) {
                GameManager.instance.StraightLine(unit.currentTile, possibleAttacks[0].target.currentTile, Color.red);
                yield return new WaitForSeconds(0.7f);
                possibleAttacks[0].Execute();
            }
            else {
                if (possibleMoves.Count > 0) {
                    int rand = Random.Range(0, possibleMoves.Count);
                    Tile t = Grid.instance.grid[possibleMoves[rand].x, possibleMoves[rand].y];

                    GameManager.instance.DisplayLine(unit.currentTile, t, Color.gray);
                    yield return new WaitForSeconds(0.7f);
                    possibleMoves[rand].Execute();
                }
                else {
                    unit.SetUnavailable();
                }
            }

            foreach (Tile tile in Grid.instance.grid) {
                if (tile != null && tile.walkable) {
                    tile.ClearDisplays();
                }
            }

            GameManager.instance.ResetLine();
            yield return new WaitForSeconds(0.1f);
        }
    }
}