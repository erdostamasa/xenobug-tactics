using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RandomEnemyAI : EnemyAI {
    public override void MakeMove() {
        foreach (Unit unit in units) {
            List<Command> possibleMoves = unit.GetAvailableMoves();
            int chosen = Random.Range(0, possibleMoves.Count);
            possibleMoves[chosen].Execute();
        }
    }
}