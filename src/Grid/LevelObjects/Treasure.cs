using System.Linq;
using Godot;
using HalfNibbleGame.Adventurers;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class Treasure : StaticGridObject, ISimulated {

  // Lazily initialized
  private Sprite2D sprite = null!;

  private int pickedUpRound = int.MaxValue;
  public Adventurer? PickedUpBy { get; private set; }

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    AddToGroup(Groups.Treasure);
    sprite = GetNode<Sprite2D>("Sprite");
  }

  public void Advance(RoundContext context) {
    context.RegisterOutcome(() => checkPickedUp(context.RoundNumber));
  }

  public void ResetToRound(RoundContext context) {
    if (PickedUpBy is not null && context.RoundNumber <= pickedUpRound) {
      drop();
      context.RegisterOutcome(() => checkPickedUp(context.RoundNumber));
    }
  }

  private void checkPickedUp(int roundNumber) {
    // If any adventurer is on this tile after a round, then we break the next round whether the adventurer moves or not
    // TODO: maybe this should not just check for adventurers
    if (GetTree().GetNodesInGroup(Groups.Simulated).OfType<Adventurer>().FirstOrDefault(a => a.Coords == Coords) is { } adventurer) {
      pickedUpRound = roundNumber;
      pickUp(adventurer);
    }
  }

  private void pickUp(Adventurer adventurer) {
    PickedUpBy = adventurer;
    sprite.Visible = false;
  }

  private void drop() {
    PickedUpBy = null;
    sprite.Visible = true;
  }
}
