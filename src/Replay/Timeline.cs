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

  public void Advance(double roundDuration) {
    var roundContext = new RoundContext(CurrentRound++, roundDuration);
    simulatedObjects().ForEach(obj => obj.Advance(roundContext));
    var hazardList = hazards();
    mortals().ForEach(mortal => mortal.CheckAgainstHazards(hazardList, roundContext));
    roundContext.Finish();
    CountdownChanged(CurrentRound, totalRoundCount);
  }

  public void Rollback() {
    if (CurrentRound <= 0) return;
    ResetToRound(CurrentRound - 1);
  }

  public void Reset() {
    ResetToRound(0);
  }

  public void ResetToRound(int round) {
    if (CurrentRound == round) return;

    CurrentRound = round;
    var roundContext = new RoundContext(round, 0);
    simulatedObjects().ForEach(obj => obj.ResetToRound(roundContext));
    roundContext.Finish();
    CountdownChanged(round, totalRoundCount);
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
