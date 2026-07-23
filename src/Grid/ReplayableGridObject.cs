using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public abstract partial class ReplayableGridObject : MovingGridObject, IReplayable {
  private readonly ActionStack actionStack = new();

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Replayable);
  }

  protected bool TryQueueMove(Vector2I diff) {
    if (!CanMove(diff)) {
      return false;
    }
    actionStack.QueueAction(Actions.Move(this, diff));
    return true;
  }

  public void Advance() {
    actionStack.Advance();
  }

  public void Rollback() {
    actionStack.Rollback();
  }

  // <== Hack
  public void QueueActions(IEnumerable<IReplayableAction> action) {
    actionStack.QueueActions(action);
  }
  // ==>
}
