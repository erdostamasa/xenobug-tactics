using UnityEngine;

public class MoveUnitCommand : Command {
    Unit unit;
    public int x;
    public int y;

    public MoveUnitCommand(Unit unit, int x, int y) {
        this.unit = unit;
        this.x = x;
        this.y = y;
    }

    public override void Execute() {
        unit.MoveTo(x, y);
        unit.SetUnavailable();
    }


    public override string ToString() {
        return unit + " -> " + x + "," + y;
    }

    public override void DisplayCommand() {
        Grid.instance.grid[x,y].DisplayMove();
        //Grid.instance.grid[x,y].DisplayAttack();
    }
}
