using Godot;

namespace HalfNibbleGame.Grid;

public abstract partial class GridObject : Node2D {
  protected Level? Level;
  public Vector2I Coords { get; protected set; }

  protected void SnapToTile() {
    if (Level is null) return;

    var tile = Level.GetTile(Coords);
    Position = tile.Position;
  }
}
