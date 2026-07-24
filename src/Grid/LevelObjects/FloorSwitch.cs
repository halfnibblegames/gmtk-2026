using System.Linq;
using Godot;
using HalfNibbleGame.Adventurers;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class FloorSwitch : StaticGridObject, ISimulated {

  [Export] private Door? door;

  // Lazily initialized
  private Sprite2D sprite = null!;

  private int pressedRound = int.MaxValue;
  private bool active;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    sprite = GetNode<Sprite2D>("Sprite");
  }

  public void Advance(RoundContext context) {
    context.RegisterOutcome(() => checkPressed(context.RoundNumber));
  }

  private void checkPressed(int roundNumber) {
    // If any adventurer is on this tile after a round, then we break the next round whether the adventurer moves or not
    // TODO: maybe this should not just check for adventurers
    if (GetTree().GetNodesInGroup(Groups.Simulated).OfType<Adventurer>().Any(a => a.Coords == Coords)) {
      pressedRound = roundNumber;
      // Because we check after the movement of a round, we can safely already activate this switch
      activate();
    }
  }

  public void ResetToRound(int roundNumber) {
    if (active && roundNumber <= pressedRound) {
      deactivate();
    }
  }

  private void activate() {
    active = true;
    door?.Open();
  }

  private void deactivate() {
    door?.Close();
    active = false;
  }
}
