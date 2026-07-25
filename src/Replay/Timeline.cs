using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;

namespace HalfNibbleGame.Replay;

public class Timeline(SceneTree tree, int totalRoundCount) {

  public delegate void CountdownChangedEventHandler(int currentRound, int totalRoundCount);
  public event CountdownChangedEventHandler CountdownChanged = delegate { };

  public int CurrentRound { get; private set; }
  public int TotalRoundCount => totalRoundCount;

  public void Advance() {
    var roundContext = new RoundContext(CurrentRound++);
    CountdownChanged(CurrentRound, totalRoundCount);
    simulatedObjects().ForEach(obj => obj.Advance(roundContext));
    var hazardList = hazards();
    mortals().ForEach(mortal => mortal.CheckAgainstHazards(hazardList, roundContext));
    roundContext.Finish();
  }

  public void Rollback() {
    if (CurrentRound <= 0) return;
    ResetToRound(CurrentRound - 1);
  }

  public void Reset() {
    ResetToRound(0);
  }

  public void ResetToRound(int round) {
    CountdownChanged(round, totalRoundCount);
    simulatedObjects().ForEach(obj => obj.ResetToRound(round));
    CurrentRound = round;
  }

  private List<ISimulated> simulatedObjects() {
    return tree.GetNodesInGroup(Groups.Simulated).OfType<ISimulated>().ToList();
  }

  private List<IHazard> hazards() {
    return tree.GetNodesInGroup(Groups.Hazard).OfType<IHazard>().ToList();
  }

  private List<IMortal> mortals() {
    return tree.GetNodesInGroup(Groups.Mortal).OfType<IMortal>().ToList();
  }
}
