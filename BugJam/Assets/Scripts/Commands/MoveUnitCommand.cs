using UnityEngine;

public class MoveUnitCommand : Command {
    public Unit unit;
    public int xBefore;
    public int yBefore;
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

        unit.MoveTo(x, y);
    }

    public void Undo() {
        unit.MoveTo(xBefore, yBefore);
    }

    public override string ToString() {
        return unit + " -> " + x + "," + y;
    }

    public override void DisplayCommand() {
        Grid.instance.grid[x, y].DisplayMove();
    }
}