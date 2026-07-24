using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public partial class Portal : StaticGridObject, ISimulated {
  [Export] private PackedScene? adventurerScene;

  [Export] private int turnCount;
  private int turnsLeft;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    Reset();
  }

  public void Advance(RoundContext context) {
    turnsLeft = turnCount - context.RoundNumber - 1;
  }

  public void Reset() {
    turnsLeft = turnCount;
  }

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
