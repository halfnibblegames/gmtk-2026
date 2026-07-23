using Godot;

namespace HalfNibbleGame;

public abstract partial class GridObject : Node2D {
  [Export] public Orchestrator Orchestrator = null!;

  [Export] public Level? Level;
  [Export] public Vector2I Coords { get; private set; }

  public override void _Process(double delta) {
    Level = Orchestrator.CurrentLevel;

    if (Level is null) return;

    var tile = Level.GetTile(Coords);
    Position = tile.Position;
  }

  protected void TeleportTo(Vector2I coords) {
    Coords = coords;
  }
}
