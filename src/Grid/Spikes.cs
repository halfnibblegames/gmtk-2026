using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public partial class Spikes : StaticGridObject, IHazard, ISimulated {
  [Export] private bool initiallyExtended;
  private bool extended;

  // Lazily initialized
  private AnimatedSprite2D sprite = null!;

  public bool IsHazardous => extended;

  public override void _Ready() {
    AddToGroup(Groups.Simulated);
    AddToGroup(Groups.Hazard);
    sprite = GetNode<AnimatedSprite2D>("Sprite");
    base._Ready();
  }

  public void Advance(RoundContext context) {
    extended = !extended;

    if (extended) {
      sprite.Play();
    }
    else {
      sprite.PlayBackwards();
    }
  }

  public void Reset() {
    extended = initiallyExtended;

    sprite.Stop();
    if (extended) {
      sprite.Frame = sprite.SpriteFrames.GetFrameCount(sprite.Animation) - 1;
    }
    else {
      sprite.Frame = 0;
    }
  }
}
