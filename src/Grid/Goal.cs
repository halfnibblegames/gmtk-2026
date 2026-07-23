using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public partial class Goal : StaticGridObject, IReplayable {
  [Export] private int turnsLeft;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Replayable);
  }

  public void Advance() {
    turnsLeft--;
  }

  public void Rollback() {
    turnsLeft++;
  }
}
