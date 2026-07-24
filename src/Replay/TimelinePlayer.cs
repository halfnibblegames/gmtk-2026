using System;
using Godot;
using HalfNibbleGame.Autoload;
using static HalfNibbleGame.Data.Constants;

namespace HalfNibbleGame.Replay;

public partial class TimelinePlayer : Node {


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
    timeUntilNextFrame = TimeBetweenRounds;
    roundsLeft = roundCount - 1;
    IsPlaying = true;
  }

  public override void _Process(double delta) {
    if (!IsPlaying) return;

    timeUntilNextFrame -= delta;
    while (IsPlaying && timeUntilNextFrame <= 0) {
      if (roundsLeft > 0) {
        timeline!.Advance();
        timeUntilNextFrame += TimeBetweenRounds;
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
