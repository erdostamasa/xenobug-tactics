using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command {
    Unit unit;
    public Unit target;

    public override void Execute() {
        unit.Attack(target);
        unit.SetUnavailable();
    }


    public override void ExecuteAnimate() {
        throw new System.NotImplementedException();
    }

    public AttackCommand(Unit unit, Unit target) {
        this.unit = unit;
        this.target = target;
    }

    public override void DisplayCommand() {
        Grid.instance.grid[target.currentTile.x, target.currentTile.y].DisplayMove();
    }
}