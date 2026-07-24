using System;
using Godot;

namespace HalfNibbleGame.Grid;

public abstract partial class MovingGridObject : GridObject {

  [Signal]
  public delegate void MovedEventHandler(Vector2I newCoords);

  [Export] public Orchestrator Orchestrator = null!;

  public Vector2I Forward { get; protected set; }

  public override void _Process(double delta) {
    if (Level != Orchestrator.CurrentLevel) {
      Level = Orchestrator.CurrentLevel;
      SnapToTile();
    }
  }

  public MoveResult TryMove(Vector2I diff) {
    if (Level is null) {
      throw new Exception($"Attempting to move {this} without a level");
    }

    if (IsMovementPrevented()) {
      GD.Print("Not moving, movement was prevented");
      return new MoveResult(MoveOutcome.Prevented, Vector2I.Zero);
    }

    validateValidMoveDiff(diff);
    // We move one step at a time
    var dir = toDirection(diff);
    var accumulatedMovement = Vector2I.Zero;

    while (accumulatedMovement != diff) {
      var targetPos = Coords + accumulatedMovement + dir;
      var targetTile = Level.GetTile(targetPos);
      if (targetTile.Collides) {
        Move(accumulatedMovement);
        return new MoveResult(MoveOutcome.Collided, accumulatedMovement);
      }
      if (targetTile.Pit) {
        accumulatedMovement += dir;
        Move(accumulatedMovement);
        return new MoveResult(MoveOutcome.FellDown, accumulatedMovement);
      }
      accumulatedMovement += dir;
    }

    Move(accumulatedMovement);
    return new MoveResult(MoveOutcome.Moved, accumulatedMovement);
  }

  protected virtual bool IsMovementPrevented() => false;

  private static void validateValidMoveDiff(Vector2I diff) {
    // Pure horizontal or vertical moves are fine
    if (diff.X == 0 || diff.Y == 0) {
      return;
    }

    // For diagonal moves, we only allow 45 degrees, so the magnitude of the X and Y diff should be equal.
    if (Math.Abs(diff.X) != Math.Abs(diff.Y)) {
      throw new Exception($"Invalid move vector: {diff}");
    }
  }

  private static Vector2I toDirection(Vector2I diff) {
    return new Vector2I(Math.Sign(diff.X), Math.Sign(diff.Y));
  }

  public void Move(Vector2I diff) {
    // TODO: make move look different than teleport
    TeleportTo(Coords + diff);
    Forward = new Vector2I(Math.Sign(diff.X), Math.Sign(diff.Y));
  }

  public void TeleportTo(Vector2I coords) {
    Coords = coords;
    SnapToTile();
    EmitSignalMoved(Coords);
  }

  public readonly record struct MoveResult(MoveOutcome Outcome, Vector2I ActuallyMoved);

  public enum MoveOutcome {
    Moved,
    Prevented,
    Collided,
    FellDown
  }
}
