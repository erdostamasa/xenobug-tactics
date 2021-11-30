using UnityEngine;

public class MoveUnitCommand : Command {
    public Unit unit;
    public int xBefore;
    public int yBefore;
    bool movedBefore;
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
        movedBefore = unit.movedThisTurn;

        unit.MoveTo(x, y);

        if (unit.movedThisTurn) {
            unit.SetUnavailable();
        }
        else {
            unit.movedThisTurn = true;
            unit.UnitActionPointsChanged();
        }
    }

    public override void ExecuteAnimate() {
        unit.MoveAnimate(x, y);

        if (unit.movedThisTurn) {
            unit.SetUnavailable();
        }
        else {
            unit.movedThisTurn = true;
            unit.UnitActionPointsChanged();
        }
    }

    public void Undo() {
        unit.MoveTo(xBefore, yBefore);
        unit.transform.rotation = rotBefore;
        unit.movedThisTurn = movedBefore;
    }

    public override string ToString() {
        return unit + " -> " + x + "," + y;
    }

    public override void DisplayCommand() {
        Grid.instance.grid[x, y].DisplayMove();
    }
}