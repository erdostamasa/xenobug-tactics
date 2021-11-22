using System;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyAI {
    public List<Unit> units;
    public abstract IEnumerator MakeMove();


    public void SetUnits(List<Unit> units) {
        this.units = units;
    }

    public void RemoveUnit(Unit unit) {
        if (units.Contains(unit)) {
            units.Remove(unit);
        }
    }

    protected EnemyAI() {
        units = new List<Unit>();
    }
}