using Godot;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class Portal : StaticGridObject {
  [Export] private PackedScene? adventurerScene;

  public Adventurers.Adventurer? TryInstantiateAdventurer() {
    if (adventurerScene is null) {
      GD.PushWarning($"Spawn at {Coords} could not spawn an adventurer.");
      return null;
    }

    var adventurer = adventurerScene.Instantiate<Adventurers.Adventurer>();
    adventurer.TeleportTo(Coords);
    return adventurer;
  }
}
