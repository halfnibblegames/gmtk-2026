using Godot;
using HalfNibbleGame.Grid;

namespace HalfNibbleGame.Replay;

public static class Actions {
  public static IReplayableAction DoNothing() => new NoopAction();

  private class NoopAction : IReplayableAction {
    public void Do() {}
    public void Undo() {}
  }

  public static IReplayableAction Move(MovingGridObject obj, Vector2I diff) => new MoveAction(obj, diff);

  private class MoveAction(MovingGridObject obj, Vector2I diff) : IReplayableAction {
    public void Do() {
      obj.Move(diff);
    }

    public void Undo() {
      obj.Move(-diff);
    }
  }
}
