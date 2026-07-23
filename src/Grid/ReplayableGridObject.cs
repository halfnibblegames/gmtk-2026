using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Extensions;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public abstract partial class ReplayableGridObject : MovingGridObject, IReplayable {
  private readonly ActionStack moveStack = new();

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Replayable);
  }

  protected bool TryQueueMove(Vector2I diff) {
    if (!CanMove(diff)) {
      return false;
    }
    moveStack.QueueAction(Actions.Move(this, diff));
    return true;
  }

  public void Advance() {
    moveStack.Advance();
  }

  public void Rollback() {
    moveStack.Rollback();
  }
}
