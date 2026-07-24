using Godot;

namespace HalfNibbleGame.Data;

public record struct Tile(Vector2I Coords, Vector2 Position, bool Collides, bool Pit) {
  private const string collisionPropertyName = "Collision";
  private const string pitPropertyName = "Pit";

  public static Tile FromTileData(Vector2I coords, Vector2 position, TileData tileData) {
    return new Tile(
      coords,
      position,
      tileData.GetCustomData(collisionPropertyName).AsBool(),
      tileData.GetCustomData(pitPropertyName).AsBool());
  }
}
