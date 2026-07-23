using Godot;

namespace HalfNibbleGame;

public partial class Orchestrator : Node {
  public Level? CurrentLevel { get; private set; }

  public void ActivateLevel(Level level) {
    CurrentLevel = level;
  }
}
