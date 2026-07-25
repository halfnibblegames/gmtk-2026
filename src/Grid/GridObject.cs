using System;
using Godot;

namespace HalfNibbleGame.Grid;

public abstract partial class GridObject : Node2D {
  protected Levels.Level? Level;
  public Vector2I Coords { get; protected set; }

  public override void _Ready() {
    Level = GetParent<Levels.Level>();
    Coords = Level.TileFromPosition(Position).Coords;
  }

  protected void SnapToTile() {
    if (Level is null) return;
    Position = ToTilePosition(Coords);
  }

  protected Vector2 ToTilePosition(Vector2I coords) {
    if (Level is null) throw new Exception("Cannot handle grid tile positions without level");

    var tile = Level.GetTile(coords);
    return tile.Position;
  }
}
