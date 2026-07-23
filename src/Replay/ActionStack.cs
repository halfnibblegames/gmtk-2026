using System.Collections.Generic;
using HalfNibbleGame.Extensions;

namespace HalfNibbleGame.Replay;

public class ActionStack {
  private readonly List<IReplayableAction> actionStack = [];
  private int currentRound;

  public void QueueAction(IReplayableAction action) {
    // Anything from the rounds after the current round is overwritten.
    if (actionStack.Count >= currentRound) {
      actionStack.RemoveRange(currentRound, actionStack.Count - currentRound);
    }
    actionStack.Push(action);
  }

  public void Advance() {
    while (actionStack.Count < currentRound - 1) {
      actionStack.Push(MakeActionForRound(actionStack.Count - 1));
    }
    actionStack[currentRound].Do();
    currentRound++;
  }

  protected virtual IReplayableAction MakeActionForRound(int round) {
    return Actions.DoNothing();
  }

  public void Rollback() {
    currentRound--;
    actionStack[currentRound].Undo();
  }
}
