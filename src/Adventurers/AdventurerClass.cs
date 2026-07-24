using System;
using System.Collections.Generic;
using HalfNibbleGame.Planning;

namespace HalfNibbleGame.Adventurers;

public enum AdventurerClass {
  Rogue,
  Wizard
}

public static class AdventurerClassExtensions {
  private static readonly IReadOnlyList<IPlannedAction> sharedActions = [
    PlannedActions.MoveLeft, PlannedActions.MoveRight, PlannedActions.MoveUp, PlannedActions.MoveDown
  ];

  extension(AdventurerClass clazz) {
    public IReadOnlyList<IPlannedAction> AvailableActions => clazz switch {
      AdventurerClass.Rogue => [.. sharedActions, PlannedActions.Dash],
      AdventurerClass.Wizard => [.. sharedActions],
      _ => throw new ArgumentOutOfRangeException(nameof(clazz), clazz, null)
    };
  }
}
