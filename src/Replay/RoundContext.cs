using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Grid;

namespace HalfNibbleGame.Replay;

public class RoundContext(int roundNumber, double roundDuration) {
  private readonly List<Action> outcomes = [];
  private readonly List<EnteredTile> enteredTiles = [];

  public int RoundNumber => roundNumber;
  public double RoundDuration => roundDuration;

  public void RegisterOutcome(Action action) {
    outcomes.Add(action);
  }

  public void RegisterTileEntered(SimulatedGridObject obj, Vector2I coords) {
    enteredTiles.Add(new EnteredTile(obj, coords));
  }

  public IEnumerable<SimulatedGridObject> ObjectsInTile(Vector2I coords) {
    return enteredTiles.Where(t => t.Coords == coords).Select(t => t.Obj);
  }

  public void Finish() {
    foreach (var action in outcomes) {
      action();
    }
  }

  private readonly record struct EnteredTile(SimulatedGridObject Obj, Vector2I Coords);
}
