using Godot;

namespace HalfNibbleGame.Autoload;

public sealed partial class Prefabs : Node {

  [Export]
  public Texture2D ActionUp;
  [Export]
  public Texture2D ActionDown;
  [Export]
  public Texture2D ActionLeft;
  [Export]
  public Texture2D ActionRight;

  public override void _Ready() {
    Global.Services.ProvidePersistent(this);
  }
}
