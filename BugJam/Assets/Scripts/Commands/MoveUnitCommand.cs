using UnityEngine;

public class MoveUnitCommand : Command {
    public Unit unit;
    public int xBefore;
    public int yBefore;
    Quaternion rotBefore;
    public int x;
    public int y;

    public MoveUnitCommand(Unit unit, int x, int y) {
        this.unit = unit;
        this.x = x;
        this.y = y;
    }


    public override void Execute() {
        xBefore = unit.currentTile.x;
        yBefore = unit.currentTile.y;
        rotBefore = unit.transform.rotation;

        unit.MoveTo(x, y);
    }

    public override void ExecuteAnimate() {
        unit.MoveAnimate(x, y);
    }

    public void Undo() {
        unit.MoveTo(xBefore, yBefore);
        unit.transform.rotation = rotBefore;
    }

    public override string ToString() {
        return unit + " -> " + x + "," + y;
    }

    public override void DisplayCommand() {
        Grid.instance.grid[x, y].DisplayMove();
    }
}