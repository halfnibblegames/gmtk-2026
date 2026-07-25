using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;
using Portal = HalfNibbleGame.Grid.LevelObjects.Portal;

namespace HalfNibbleGame.Levels;

public partial class Level : Node2D {

  public delegate Tile TileModifier(Tile tile);

  [Export] public int RoundCount { get; private set; }

  private TileMapLayer? cachedTileMapLayer;
  private TileMapLayer tileMapLayer {
    get {
      return cachedTileMapLayer ??= GetNode<TileMapLayer>("MapLayer");
    }
  }
  private readonly Dictionary<Vector2I, TileModifier> tileModifiers = new();

  public int WidthInPixels => tileMapLayer.GetUsedRect().Size.X * tileMapLayer.TileSet.TileSize.X;
  public int HeightInPixels => tileMapLayer.GetUsedRect().Size.Y * tileMapLayer.TileSet.TileSize.Y;

  public List<Portal> AllPortals => GetChildren().OfType<Portal>().ToList();

  public Tile GetTile(Vector2I coords) {
    var tile = Tile.FromTileData(coords, tileMapLayer.MapToLocal(coords), tileMapLayer.GetCellTileData(coords));
    if (tileModifiers.TryGetValue(coords, out var modifier)) {
      tile = modifier.Invoke(tile);
    }
    return tile;
  }

  public Tile TileFromPosition(Vector2 position) {
    return GetTile(tileMapLayer.LocalToMap(position));
  }

  public void RegisterTileModifier(Vector2I coords, TileModifier modifier) {
    if (!tileModifiers.TryAdd(coords, modifier)) {
      throw new InvalidOperationException("Cannot add more than one modifier to a tile");
    }
  }

  public void UnregisterTileModifier(Vector2I coords) {
    tileModifiers.Remove(coords);
  }
}
