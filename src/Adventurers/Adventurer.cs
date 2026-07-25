using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Planning;

namespace HalfNibbleGame.Adventurers;

public partial class Adventurer : SimulatedGridObject {
  [Export] private AdventurerClass adventurerClass;
  [Export] public Texture2D? Portrait { get; private set; }

  public IReadOnlyList<IPlannedAction> AvailableActions => adventurerClass.AvailableActions;
}
