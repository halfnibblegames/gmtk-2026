using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Levels;

namespace HalfNibbleGame;

public partial class GameProgression : Node2D {

  [Export] private PackedScene[] levels = [];

  private int currentLevelIndex = -1;
  private Level? currentLevel;

  public override void _Ready() {
    Global.Services.ProvideInScene(this);

    if (levels.Length == 0) {
      GD.PushWarning("No levels set. Cannot start the game.");
      return;
    }

    // We make sure Orchestrator is a child of this node, so that we can access it.
    loadLevel(0);
  }

  private void loadLevel(int levelIndex) {
    var instance = levels[levelIndex].Instantiate<Level>();
    AddChild(instance);
    currentLevel = instance;
    currentLevelIndex = levelIndex;

    Global.Services.Get<Orchestrator>().SetLevel(currentLevel);
  }
}
