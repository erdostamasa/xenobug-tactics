using System;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyAI {
    public List<Unit> units;
    public abstract IEnumerator MakeMove();
    

    public void RemoveUnit(Unit unit) {
        if (units.Contains(unit)) {
            units.Remove(unit);
        }
    }

    protected EnemyAI() {
        units = new List<Unit>();
    }
}