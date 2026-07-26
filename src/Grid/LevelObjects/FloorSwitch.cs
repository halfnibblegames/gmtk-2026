using System.Linq;
using Godot;
using HalfNibbleGame.Adventurers;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class FloorSwitch : StaticGridObject, ISimulated {

  [Export] private Door? door;

  // Lazily initialized
  private Sprite2D upSprite = null!;
  private Sprite2D downSprite = null!;

  private int pressedRound = int.MaxValue;
  private bool active;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    upSprite = GetNode<Sprite2D>("SpriteUp");
    downSprite = GetNode<Sprite2D>("SpriteDown");
  }

  public void Advance(RoundContext context) {
    context.RegisterOutcome(() => checkPressed(context));
  }

  public void ResetToRound(RoundContext context) {
    if (active && context.RoundNumber <= pressedRound) {
      deactivate();
      context.RegisterOutcome(() => checkPressed(context));
    }
  }

  private void checkPressed(RoundContext context) {
    // If any adventurer is on this tile after a round, then we break the next round whether the adventurer moves or not
    if (context.ObjectsInTile(Coords).Any()) {
      pressedRound = context.RoundNumber;
      // Because we check after the movement of a round, we can safely already activate this switch
      activate(context.RoundDuration);
    }
  }

  private void activate(double duration) {
    active = true;
    door?.Open(duration);
    upSprite.Visible = false;
    downSprite.Visible = true;
  }

  private void deactivate() {
    door?.Close();
    active = false;
    upSprite.Visible = true;
    downSprite.Visible = false;
  }
}
