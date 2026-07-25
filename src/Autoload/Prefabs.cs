using Godot;

namespace HalfNibbleGame.Autoload;

public sealed partial class Prefabs : Node {
  [Export] public PackedScene HistoryArrow { get; private set; } = null!;

  public override void _Ready() {
    Global.Services.ProvidePersistent(this);
  }
}
