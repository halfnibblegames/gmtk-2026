using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame.Replay;

public class Timeline(SceneTree tree) {

  private int roundNumber;

  public void Advance() {
    var roundContext = new RoundContext(roundNumber++);
    simulatedObjects().ForEach(obj => obj.Advance(roundContext));
    roundContext.Finalize();
  }

  public void Reset() {
    simulatedObjects().ForEach(obj => obj.Reset());
    roundNumber = 0;
  }

  private List<ISimulated> simulatedObjects() {
    return tree.GetNodesInGroup(Groups.Simulated).OfType<ISimulated>().ToList();
  }
}
