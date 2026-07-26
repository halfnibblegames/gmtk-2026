using System;
using Godot;
using HalfNibbleGame.Autoload;
using static HalfNibbleGame.Data.Constants;

namespace HalfNibbleGame.Replay;

public partial class TimelinePlayer : Node {

  [Signal]
  public delegate void PlaybackCompletedEventHandler();

  private Timeline? timeline;
  private double timeUntilNextFrame;

  public bool IsPlaying { get; private set; }

  public override void _Ready() {
    Global.Services.ProvideInScene(this);
  }

  public void Play(Timeline timelineToPlay) {
    if (IsPlaying) throw new Exception("Cannot play more than once");

    timeline = timelineToPlay;

    timeline.Reset();
    timeline.Advance(TimeBetweenRoundsPlayback);
    timeUntilNextFrame = TimeBetweenRoundsPlayback;
    IsPlaying = true;
  }

  public override void _Process(double delta) {
    if (!IsPlaying) return;

    timeUntilNextFrame -= delta;
    while (IsPlaying && timeUntilNextFrame <= 0) {
      if (timeline!.CurrentRound < timeline.TotalRoundCount) {
        timeline.Advance(TimeBetweenRoundsPlayback);
        timeUntilNextFrame += TimeBetweenRoundsPlayback;
      }
      else {
        IsPlaying = false;
        timeUntilNextFrame = 0;
        EmitSignalPlaybackCompleted();

        timeline = null;
      }
    }
  }
}
