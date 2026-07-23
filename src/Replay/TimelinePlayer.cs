using Godot;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Replay;

public partial class TimelinePlayer : Node {
  private const double timeBetweenFrames = 60.0 / 120; // 120 BPM;

  private Timeline? timeline;
  private double timeUntilNextFrame;
  private int roundsLeft;

  public override void _Ready() {
    Global.Services.ProvideInScene(this);
  }

  public void Play(int roundCount) {
    timeline = Global.Services.Get<Timeline>();
    timeline.Advance();
    timeUntilNextFrame = timeBetweenFrames;
    roundsLeft = roundCount - 1;
  }

  public override void _Process(double delta) {
    if (timeline is null || roundsLeft == 0) return;

    timeUntilNextFrame -= delta;
    while (timeUntilNextFrame <= 0 && roundsLeft > 0) {
      timeline.Advance();
      timeUntilNextFrame += timeBetweenFrames;
      roundsLeft--;
    }
  }
}
