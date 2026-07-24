using System;
using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public static class PlannedActions {
  private static IPlannedAction moveLeft { get; } = new MoveAction(Vector2I.Left, InputActions.Left);
  private static IPlannedAction moveRight { get; } = new MoveAction(Vector2I.Right, InputActions.Right);
  private static IPlannedAction moveUp { get; } = new MoveAction(Vector2I.Up, InputActions.Up);
  private static IPlannedAction moveDown { get; } = new MoveAction(Vector2I.Down, InputActions.Down);
  private static IPlannedAction dash { get; } = new ForwardAction(2, InputActions.Dash);

  public static IReadOnlyList<IPlannedAction> All { get; } = [moveLeft, moveRight, moveUp, moveDown, dash];

  private abstract class ActionBase(StringName? shortcut) : IPlannedAction {
    public StringName? Shortcut => shortcut;

    public abstract void Do(RoundContext context, SimulatedGridObject target);
  }

  private class MoveAction(Vector2I diff, StringName? shortcut) : ActionBase(shortcut) {
    public override void Do(RoundContext context, SimulatedGridObject target) {
      var result = target.TryMove(diff);
      handleMoveResult(result, context, target);
    }
  }

  private class ForwardAction(int amount, StringName? shortcut) : ActionBase(shortcut) {
    public override void Do(RoundContext context, SimulatedGridObject target) {
      var result = target.TryMove(target.Forward * amount);
      handleMoveResult(result, context, target);
    }
  }

  private static void handleMoveResult(
    MovingGridObject.MoveResult result, RoundContext context, SimulatedGridObject target) {
    switch (result.Outcome) {
      case MovingGridObject.MoveOutcome.Moved:
      case MovingGridObject.MoveOutcome.Prevented:
        break;
      case MovingGridObject.MoveOutcome.Collided:
        // Stun for 1 turn + 1 turn for every tile moved
        context.RegisterOutcome(() =>
          target.Stun(1 + Math.Max(Math.Abs(result.ActuallyMoved.X), Math.Abs(result.ActuallyMoved.Y))));
        break;
      case MovingGridObject.MoveOutcome.FellDown:
        context.RegisterOutcome(target.Die);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(result), result, null);
    }
  }
}
