using Godot;
using HalfNibbleGame;
using HalfNibbleGame.Autoload;
using System;

public partial class TimelineBar : Control {
  [Export]
  private PackedScene socketScene;
  [Export]
  private HBoxContainer box;

  private int currentSocketCount;
  private int currentSocketWithPipCount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
    Global.Services.Get<Orchestrator>().TimelineCountdownChanged += OnTimelineChanged;
    base._Ready();
  }
  public void OnTimelineChanged(int currentRound, int roundCount) {
    while (currentSocketCount > roundCount) {
      box.RemoveChild(box.GetChild(0));
      currentSocketCount--;
    }

    while (currentSocketCount < roundCount) {
      box.AddChild(socketScene.Instantiate<ActionSocket>());
      currentSocketCount++;
    }

    if (currentSocketWithPipCount == currentRound)
      return;

    for (var i = 0; i < roundCount; i++) {
      var child = box.GetChild<ActionSocket>(i);
      child.SetCurrentAction(i <= currentRound - 1 ? PipActions.Dash : null);
    }
    currentSocketWithPipCount = currentRound;
  }
}
