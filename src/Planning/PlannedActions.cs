using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public static class PlannedActions {
  public static IPlannedAction MoveLeft { get; } = new MoveAction(Vector2I.Left, InputActions.Left);
  public static IPlannedAction MoveRight { get; } = new MoveAction(Vector2I.Right, InputActions.Right);
  public static IPlannedAction MoveUp { get; } = new MoveAction(Vector2I.Up, InputActions.Up);
  public static IPlannedAction MoveDown { get; } = new MoveAction(Vector2I.Down, InputActions.Down);
  public static IPlannedAction Dash { get; } = new ForwardAction(2, InputActions.Dash);

  private abstract class ActionBase(StringName? shortcut) : IPlannedAction {
    public StringName? Shortcut => shortcut;

    public abstract bool CheckValid(SimulatedGridObject target);
    public abstract void Do(RoundContext context, SimulatedGridObject target);
  }

  private class MoveAction(Vector2I diff, StringName? shortcut) : ActionBase(shortcut) {
    public override bool CheckValid(SimulatedGridObject target) {
      var result = target.PreviewMove(diff);
      return result.Valid;
    }

    public override void Do(RoundContext context, SimulatedGridObject target) {
      var result = target.PreviewMove(diff);
      target.DoMove(result);
      handleMoveResult(result, context, target);
    }
  }

  private class ForwardAction(int amount, StringName? shortcut) : ActionBase(shortcut) {
    public override bool CheckValid(SimulatedGridObject target) {
      if (target.Forward.LengthSquared() < 0) {
        return false;
      }
      var result = target.PreviewMove(target.Forward * amount);
      return result.Valid;
    }

    public override void Do(RoundContext context, SimulatedGridObject target) {
      var result = target.PreviewMove(target.Forward * amount);
      target.DoMove(result);
      handleMoveResult(result, context, target);
    }
  }

  private static void handleMoveResult(
    MovingGridObject.MoveResult result, RoundContext context, SimulatedGridObject target) {
    foreach (var tile in result.TilesVisited) {
      context.RegisterTileEntered(target, tile);
    }

    if (result.Outcome == MovingGridObject.MoveOutcome.FellDown) {
      context.RegisterOutcome(target.Die);
    }
  }
}
