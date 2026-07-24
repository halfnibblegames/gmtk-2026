using Godot;
using HalfNibbleGame;
using HalfNibbleGame.Autoload;
using System;

public partial class SpindownDice : Control {
  [Export]
  private Label Countdown;
	public override void _Ready() {
    var orchestrator = Global.Services.Get<Orchestrator>();
    orchestrator.OnTimelineCountdownChanged += countdownChanged;
    var initialTimeline = orchestrator.Timeline!;
    countdownChanged(initialTimeline.CurrentRound, initialTimeline.TotalRoundCount);
    base._Ready();
	}

  private void countdownChanged(int currentRound, int totalRoundCount) {
    Countdown.Text = $"{totalRoundCount - currentRound}";
  }
}
