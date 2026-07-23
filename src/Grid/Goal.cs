using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public partial class Goal : StaticGridObject, ISimulated {
  [Export] private int turnCount;
  private int turnsLeft;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    Reset();
  }

  public void Advance(RoundContext context) {
    turnsLeft = turnCount - context.RoundNumber - 1;
  }

  public void Reset() {
    turnsLeft = turnCount;
  }
}
