using Godot;

namespace HalfNibbleGame.Grid;

public abstract partial class MovingGridObject : GridObject {

  [Export] public Orchestrator Orchestrator = null!;

  [Signal]
  public delegate void MovedEventHandler(Vector2I newCoords);

  public override void _Process(double delta) {
    if (Level != Orchestrator.CurrentLevel) {
      Level = Orchestrator.CurrentLevel;
      SnapToTile();
    }
  }

  public void Move(Vector2I diff) {
    // TODO: make move different than teleport
    TeleportTo(Coords + diff);
  }

  public void TeleportTo(Vector2I coords) {
    Coords = coords;
    SnapToTile();
    EmitSignalMoved(Coords);
  }

  protected bool CanMove(Vector2I diff) {
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

    return true;
  }
}
