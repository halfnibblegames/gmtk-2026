using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame;

public partial class Level : Node2D {
  [Export] private Orchestrator orchestrator = null!;

  // Lazily initialized
  private TileMapLayer tileMapLayer = null!;

  public int WidthInPixels => tileMapLayer.GetUsedRect().Size.X * tileMapLayer.TileSet.TileSize.X;
  public int HeightInPixels => tileMapLayer.GetUsedRect().Size.Y * tileMapLayer.TileSet.TileSize.Y;

  public override void _Ready() {
    tileMapLayer = GetNode<TileMapLayer>("MapLayer");
    orchestrator.ActivateLevel(this);
  }

  public Tile GetTile(Vector2I coords) {
    return Tile.FromTileData(coords, tileMapLayer.MapToLocal(coords), tileMapLayer.GetCellTileData(coords));
  }

  public Tile TileFromPosition(Vector2 position) {
    return GetTile(tileMapLayer.LocalToMap(position));
  }
}
