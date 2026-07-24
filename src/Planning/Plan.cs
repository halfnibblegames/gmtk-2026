using System;
using System.Collections.Generic;

namespace HalfNibbleGame.Planning;

public class Plan {
  private readonly List<IPlannedAction> actions = [];

  public int PlannedRoundCount => actions.Count;

  public void SetActionForRound(int round, IPlannedAction action) {
    if (round > actions.Count) {
      throw new Exception("Cannot skip rounds");
    }

    ClearActionForRound(round);
    actions.Add(action);
  }

  public void ClearActionForRound(int round) {
    if (actions.Count >= round) {
      actions.RemoveRange(round, actions.Count - round);
    }
  }

  public IPlannedAction? GetActionForRound(int round) {
    return round >= actions.Count ? null : actions[round];
  }
}
