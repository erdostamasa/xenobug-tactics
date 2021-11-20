public class MoveUnitCommand : Command {
    Unit unit;
    int x;
    int y;

    public MoveUnitCommand(Unit unit, int x, int y) {
        this.unit = unit;
        this.x = x;
        this.y = y;
    }

    public override void Execute() {
        unit.MoveTo(x, y);
    }

    public override string ToString() {
        return unit + " -> " + x + "," + y;
    }
}