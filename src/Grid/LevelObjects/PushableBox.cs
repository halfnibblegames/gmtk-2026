using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class PushableBox : MovingGridObject, ISimulated {
  private Vector2I lastKnownCoords = -1 * Vector2I.One;

  private readonly History<RoundState> history = new();

  public override void _Ready() {
    base._Ready();
    onMoved(Coords);
    Moved += onMoved;
    AddToGroup(Groups.Simulated);
  }

  private void onMoved(Vector2I newCoords) {
    if (newCoords == lastKnownCoords) return;

    Level!.UnregisterTileModifier(lastKnownCoords, makeTileCollide);
    lastKnownCoords = newCoords;
    Level.RegisterTileModifier(newCoords, makeTileCollide);
  }

  private Tile makeTileCollide(Tile tile) {
    return tile with { Collides = true, CollidesWith = this };
  }

  public void Advance(RoundContext context) {
    history.Push(new RoundState(Coords));
  }

  public void ResetToRound(RoundContext context) {
    var roundState = history.LastKnownStateInRound(context.RoundNumber);
    TeleportTo(roundState.Coords);

    history.InvalidateFrom(context.RoundNumber);
  }

  private sealed record RoundState(Vector2I Coords);
}
