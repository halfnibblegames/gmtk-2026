using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame.Replay;

public class Timeline(SceneTree tree) {

  private int roundNumber;

  public void Advance() {
    var roundInfo = new RoundInfo(roundNumber++);
    simulatedObjects().ForEach(obj => obj.Advance(roundInfo));
  }

  public void Reset() {
    simulatedObjects().ForEach(obj => obj.Reset());
  }

  private List<ISimulated> simulatedObjects() {
    return tree.GetNodesInGroup(Groups.Simulated).OfType<ISimulated>().ToList();
  }
}
