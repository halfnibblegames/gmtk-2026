using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame;

public partial class Level : Node2D {
  private bool isActivated;

  [Export] private Orchestrator orchestrator = null!;

  // Lazily initialized
  private TileMapLayer tileMapLayer = null!;

  public override void _Ready() {
    tileMapLayer = GetNode<TileMapLayer>("MapLayer");
  }

  public override void _Process(double delta) {
    if (!isActivated) {
      orchestrator.ActivateLevel(this);
      isActivated = true;
    }
  }

  public Tile GetTile(Vector2I coords) {
    return Tile.FromTileData(coords, tileMapLayer.MapToLocal(coords), tileMapLayer.GetCellTileData(coords));
  }
}
