using Godot;

namespace HalfNibbleGame.Autoload;

public sealed partial class Prefabs : Node {
  [Export] public PackedScene HistoryArrow { get; private set; } = null!;

  [Export] public Texture2D ActionUp { get; private set; } = null!;
  [Export] public Texture2D ActionDown { get; private set; } = null!;
  [Export] public Texture2D ActionLeft { get; private set; } = null!;
  [Export] public Texture2D ActionRight { get; private set; } = null!;

  public override void _Ready() {
    Global.Services.ProvidePersistent(this);
  }
}
