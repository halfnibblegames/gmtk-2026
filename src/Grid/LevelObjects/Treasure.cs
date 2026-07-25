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
    context.RegisterOutcome(() => checkPickedUp(context));
  }

  public void ResetToRound(RoundContext context) {
    if (PickedUpBy is not null && context.RoundNumber <= pickedUpRound) {
      drop();
      context.RegisterOutcome(() => checkPickedUp(context));
    }
  }

  private void checkPickedUp(RoundContext context) {
    if (context.ObjectsInTile(Coords).OfType<Adventurer>().FirstOrDefault() is { } adventurer) {
      pickedUpRound = context.RoundNumber;
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
