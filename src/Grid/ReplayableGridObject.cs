using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Extensions;

namespace HalfNibbleGame.Grid;

public abstract partial class ReplayableGridObject : MovingGridObject {
  private readonly List<Vector2I> moveStack = [];

  protected override void Move(Vector2I diff) {
    base.Move(diff);
    moveStack.Push(diff);
  }

  protected bool TryRevertLastMove() {
    if (moveStack.Count == 0) {
      return false;
    }

    var move = moveStack.Pop();
    base.Move(-move);
    return true;
  }
}
