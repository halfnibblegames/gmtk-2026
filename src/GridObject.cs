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

  protected bool TryMove(Vector2I diff) {
    if (Level is null) {
      GD.PushError($"Attempting to move {this} without a level");
      return false;
    }

    if (diff.LengthSquared() != 1) {
      GD.PushWarning($"Attempting to move {this} by more than one step: {diff}");
    }

    var targetPos = Coords + diff;
    var targetTile = Level.GetTile(targetPos);
    if (targetTile.Collides) {
      return false;
    }

    Move(diff);
    return true;
  }

  protected virtual void Move(Vector2I diff) {
    // TODO: make move different than teleport
    TeleportTo(Coords + diff);
  }
}
