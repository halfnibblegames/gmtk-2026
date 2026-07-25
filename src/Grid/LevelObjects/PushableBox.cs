using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class PushableBox : MovingGridObject {
  private Vector2I lastKnownCoords = -1 * Vector2I.One;

  public override void _Ready() {
    base._Ready();
    onMoved(Coords);
    Moved += onMoved;
  }

  private void onMoved(Vector2I newCoords) {
    if (newCoords == lastKnownCoords) return;

    Level!.UnregisterTileModifier(lastKnownCoords);
    lastKnownCoords = newCoords;
    Level.RegisterTileModifier(newCoords, makeTileCollide);
  }

  private static Tile makeTileCollide(Tile tile) {
    return tile with { Collides = true };
  }
}
