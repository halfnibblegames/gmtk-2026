namespace HalfNibbleGame.Grid;

public abstract partial class StaticGridObject : GridObject {
  public override void _Ready() {
    Level = GetParent<Level>();
    Coords = Level.TileFromPosition(Position).Coords;
  }
}
