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

            List<Command> possibleCommands = new List<Command>();
            possibleCommands = possibleCommands.Concat(possibleMoves).ToList();
            possibleCommands = possibleCommands.Concat(possibleAttacks).ToList();

            if (possibleCommands.Count > 0) {
                int rand = Random.Range(0, possibleCommands.Count);
                possibleCommands[rand].Execute();
                yield return new WaitForSeconds(0.25f);
            }
            else {
                unit.SetUnavailable();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}