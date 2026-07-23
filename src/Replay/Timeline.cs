using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame.Replay;

public class Timeline(SceneTree tree) {

  private int currentRound;

  public void Advance() {
    replayableObjects().ForEach(obj => obj.Advance());
    currentRound++;
  }

  public void Rollback() {
    if (currentRound == 0) {
      return;
    }

    replayableObjects().ForEach(obj => obj.Rollback());
    currentRound--;
  }

  private List<IReplayable> replayableObjects() {
    return tree.GetNodesInGroup(Groups.Replayable).OfType<IReplayable>().ToList();
  }
}
