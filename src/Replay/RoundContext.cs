using System;
using System.Collections.Generic;

namespace HalfNibbleGame.Replay;

public class RoundContext(int roundNumber) {
  private readonly List<Action> outcomes = [];

  public int RoundNumber => roundNumber;

  public void RegisterOutcome(Action action) {
    outcomes.Add(action);
  }

  public void Finalize() {
    foreach (var action in outcomes) {
      action();
    }
  }
}
