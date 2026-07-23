using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Planning;

namespace HalfNibbleGame.Grid;

public partial class Adventurer : SimulatedGridObject {

  // <== Hack
  public override void _Ready() {
    Global.Services.Get<Planner>().SetAdventurer(this);
    GetNode<AnimatedSprite2D>("Sprite").Play();
    base._Ready();
  }
  // ==>
}
