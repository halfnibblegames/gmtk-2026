using System;
using Godot;

namespace HalfNibbleGame.Grid;

public abstract partial class MovingGridObject : GridObject {

  [Signal]
  public delegate void MovedEventHandler(Vector2I newCoords);

  public delegate MoveAnimation MoveAnimationFactory(MovingGridObject target, Vector2 start, Vector2 end);

  private MoveAnimation? moveAnimation;

  public Vector2I Forward { get; protected set; }
  protected bool Flipped { get; set; }

  public override void _Process(double delta) {
    if (moveAnimation is not null) {
      moveAnimation.Update(delta);
      EmitSignalMoved(Coords);
    }

    Scale = new Vector2(Flipped ? -1 : 1, 1);
  }

  public MoveResult PreviewMove(Vector2I diff) {
    if (Level is null) {
      throw new Exception($"Attempting to move {this} without a level");
    }

    validateValidMoveDiff(diff);
    // We move one step at a time
    var dir = toDirection(diff);
    var accumulatedMovement = Vector2I.Zero;

    while (accumulatedMovement != diff) {
      var targetPos = Coords + accumulatedMovement + dir;
      var targetTile = Level.GetTile(targetPos);
      if (targetTile.Collides) {
        return new MoveResult(MoveOutcome.Collided, accumulatedMovement);
      }
      if (targetTile.Pit) {
        accumulatedMovement += dir;
        return new MoveResult(MoveOutcome.FellDown, accumulatedMovement);
      }
      accumulatedMovement += dir;
    }

    return new MoveResult(MoveOutcome.Moved, accumulatedMovement);
  }

  public void DoMove(MoveResult moveResult) {
    MoveAnimationFactory animation = moveResult.Outcome switch {
      MoveOutcome.Moved => MoveAnimation.Move,
      MoveOutcome.Collided => throw new InvalidOperationException($"Attempting to move {this} with an invalid move"),
      MoveOutcome.FellDown => MoveAnimation.Fall,
      _ => throw new ArgumentOutOfRangeException()
    };

    Move(moveResult.ActuallyMoved, animation);
  }

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

  public void Move(Vector2I diff, MoveAnimationFactory animationFactory) {
    if (moveAnimation is not null) {
      SnapToTile();
      moveAnimation = null;
    }

    var start = ToTilePosition(Coords);
    Coords += diff;
    if (diff.LengthSquared() > 0) {
      Forward = new Vector2I(Math.Sign(diff.X), Math.Sign(diff.Y));
      if (Forward == Vector2I.Right && Flipped) {
        Flipped = false;
      } else if (Forward == Vector2I.Left && !Flipped) {
        Flipped = true;
      }
    }

    var end = ToTilePosition(Coords);

    moveAnimation = animationFactory(this, start, end);

    EmitSignalMoved(Coords);
  }

  public void TeleportTo(Vector2I coords) {
    moveAnimation?.CompleteInstantly();
    moveAnimation = null;
    Coords = coords;
    SnapToTile();
    EmitSignalMoved(Coords);
  }

  public readonly record struct MoveResult(MoveOutcome Outcome, Vector2I ActuallyMoved) {
    public bool Valid => Outcome is MoveOutcome.Moved or MoveOutcome.FellDown;
  }

  public enum MoveOutcome {
    Moved,
    Collided,
    FellDown
  }
}
