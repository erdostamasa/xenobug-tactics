using System;
using System.Collections;
using System.Collections.Generic;

public abstract class EnemyAI {
    public List<Unit> units;
    public abstract void MakeMove();

    public void SetUnits(List<Unit> units) {
        this.units = units;
    }

    protected EnemyAI() {
        units = new List<Unit>();
    }
}