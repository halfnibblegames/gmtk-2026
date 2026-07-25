using System.Linq;
using Godot;
using HalfNibbleGame.Adventurers;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class CrackedFloor : StaticGridObject, ISimulated {

  // Lazily initialized
  private Sprite2D floorSprite = null!;
  private Sprite2D gapSprite = null!;

  private int breakRound = int.MaxValue;
  private bool broken;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    floorSprite = GetNode<Sprite2D>("FloorSprite");
    gapSprite = GetNode<Sprite2D>("GapSprite");
    ResetToRound(new RoundContext(0));
    Level!.RegisterTileModifier(Coords, applyBrokenToTile);
  }

  public void Advance(RoundContext context) {
    if (context.RoundNumber == breakRound) {
      broken = true;
      updateVisuals();
    }

    if (!broken) {
      context.RegisterOutcome(() => checkForBreak(context.RoundNumber));
    }
  }

  private void checkForBreak(int roundNumber) {
    // If any adventurer is on this tile after a round, then we break the next round whether the adventurer moves or not
    // TODO: maybe this should not just check for adventurers
    if (GetTree().GetNodesInGroup(Groups.Simulated).OfType<Adventurer>().Any(a => a.Coords == Coords)) {
      breakRound = roundNumber + 1;
    }
  }

  public void ResetToRound(RoundContext context) {
    if (broken && context.RoundNumber <= breakRound) {
      broken = false;
      updateVisuals();
    }
    if (context.RoundNumber < breakRound) {
      breakRound = int.MaxValue;
    }
  }

  private void updateVisuals() {
    floorSprite.Visible = !broken;
    gapSprite.Visible = broken;
  }

  private Tile applyBrokenToTile(Tile tile) {
    return broken ? tile with { Pit = true } : tile;
  }
}
