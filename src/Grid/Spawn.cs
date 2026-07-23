using Godot;

namespace HalfNibbleGame.Grid;

public partial class Spawn : StaticGridObject {
  [Export] private PackedScene? adventurerScene;

  public Adventurer? TryInstantiateAdventurer() {
    if (adventurerScene is null) {
      GD.PushWarning($"Spawn at {Coords} could not spawn an adventurer.");
      return null;
    }

    var adventurer = adventurerScene.Instantiate<Adventurer>();
    adventurer.TeleportTo(Coords);
    adventurer.Snapshot();
    return adventurer;
  }
}
