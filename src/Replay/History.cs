using System;
using System.Collections.Generic;

namespace HalfNibbleGame.Replay;

public class History<T> {
  // The entry at index i is the state at the START of round i
  private readonly List<T> roundStates = [];

  public int Length => roundStates.Count;

  public T this[int index] => roundStates[index];

  public T this[Index index] => roundStates[index];

  public T LastKnownStateInRound(int round) {
    return roundStates[Math.Min(round, roundStates.Count - 1)];
  }

  public void Push(T value) {
    roundStates.Add(value);
  }

  public void InvalidateFrom(int round) {
    if (round < 0 || round >= roundStates.Count) return;
    roundStates.RemoveRange(round, roundStates.Count - round);
  }
}
