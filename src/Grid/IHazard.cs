using Godot;

namespace HalfNibbleGame.Grid;

public interface IHazard {
  Vector2I Coords { get; }

  public bool Hazardous { get; }
}
