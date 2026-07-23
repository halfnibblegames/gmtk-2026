using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public partial class Adventurer : ReplayableGridObject {
  // <== Hack
  public override void _Ready() {
    Global.Services.Get<Planner>().SetAdventurer(this);
    GetNode<AnimatedSprite2D>("Sprite").Play();
    base._Ready();
  }
  // ==>

  private bool tryMove(Vector2I diff) {
    if (!TryQueueMove(diff)) {
      return false;
    }

    Global.Services.Get<Timeline>().Advance();
    return true;
  }
}
