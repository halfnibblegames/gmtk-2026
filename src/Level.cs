using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;

namespace HalfNibbleGame;

public partial class Level : Node2D {
  [Export] private Orchestrator orchestrator = null!;

  private TileMapLayer? cachedTileMapLayer;
  private TileMapLayer tileMapLayer {
    get {
      return cachedTileMapLayer ??= GetNode<TileMapLayer>("MapLayer");
    }
  }

  public int WidthInPixels => tileMapLayer.GetUsedRect().Size.X * tileMapLayer.TileSet.TileSize.X;
  public int HeightInPixels => tileMapLayer.GetUsedRect().Size.Y * tileMapLayer.TileSet.TileSize.Y;

  public List<Portal> AllPortals => GetChildren().OfType<Portal>().ToList();

  public override void _Ready() {
    orchestrator.SetLevel(this);
  }

  public Tile GetTile(Vector2I coords) {
    return Tile.FromTileData(coords, tileMapLayer.MapToLocal(coords), tileMapLayer.GetCellTileData(coords));
  }

  public Tile TileFromPosition(Vector2 position) {
    return GetTile(tileMapLayer.LocalToMap(position));
  }
}
