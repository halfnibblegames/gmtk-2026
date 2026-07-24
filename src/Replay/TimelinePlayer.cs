using System;
using Godot;
using static HalfNibbleGame.Data.Constants;

namespace HalfNibbleGame.Replay;

public partial class TimelinePlayer : Node {

  private Timeline? timeline;
  private double timeUntilNextFrame;
  private int roundsLeft;

  public bool IsPlaying { get; private set; }

  public void Play(int roundCount) {
    if (IsPlaying) throw new Exception("Cannot play more than once");

    // TODO: get the timeline from somewhere
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
