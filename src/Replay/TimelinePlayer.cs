using System;
using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Replay;

public partial class TimelinePlayer : Node {
  private const double timeBetweenFrames = 60.0 / 120; // 120 BPM;

  private Timeline? timeline;
  private double timeUntilNextFrame;
  private int roundsLeft;

  public bool IsPlaying { get; private set; }

  public override void _Ready() {
    Global.Services.ProvideInScene(this);
  }

  public void Play(int roundCount) {
    if (IsPlaying) throw new Exception("Cannot play more than once");

    timeline = Global.Services.Get<Timeline>();
    timeline.Advance();
    timeUntilNextFrame = timeBetweenFrames;
    roundsLeft = roundCount - 1;
    IsPlaying = true;
  }

  public override void _Process(double delta) {
    if (!IsPlaying) return;

    timeUntilNextFrame -= delta;
    while (IsPlaying && timeUntilNextFrame <= 0) {
      if (roundsLeft > 0) {
        timeline!.Advance();
        timeUntilNextFrame += timeBetweenFrames;
        roundsLeft--;
      }
      else {
        timeline!.Reset();
        IsPlaying = false;
        timeUntilNextFrame = 0;
      }
    }
  }
}
