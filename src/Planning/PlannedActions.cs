using System;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public static class PlannedActions {

  public static IPlannedAction? FromEnum(PlayerAction action) => action switch {
    PlayerAction.None => null,
    PlayerAction.MoveLeft => new MoveAction(action, Vector2I.Left, "<", InputActions.Left),
    PlayerAction.MoveRight => new MoveAction(action, Vector2I.Right, ">", InputActions.Right),
    PlayerAction.MoveUp => new MoveAction(action, Vector2I.Up, "^", InputActions.Up),
    PlayerAction.MoveDown => new MoveAction(action, Vector2I.Down, "V", InputActions.Down),
    PlayerAction.Dash => new ForwardAction(action, 2, ">>", InputActions.Dash),
    _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
  };

  private abstract class ActionBase(PlayerAction asEnum, string name, StringName? shortcut) : IPlannedAction {
    public string Name => name;
    public StringName? Shortcut => shortcut;
    public PlayerAction AsEnum() => asEnum;

    public abstract void Do(RoundContext context, SimulatedGridObject target);
  }

  private class MoveAction(PlayerAction asEnum, Vector2I diff, string name, StringName? shortcut)
    : ActionBase(asEnum, name, shortcut) {
    public override void Do(RoundContext context, SimulatedGridObject target) {
      var result = target.TryMove(diff);
      handleMoveResult(result, context, target);
    }
  }

  private class ForwardAction(PlayerAction asEnum, int amount, string name, StringName? shortcut)
    : ActionBase(asEnum, name, shortcut) {
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
