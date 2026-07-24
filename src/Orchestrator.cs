using System;
using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Data;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;
using Adventurer = HalfNibbleGame.Adventurers.Adventurer;

namespace HalfNibbleGame;

public partial class Orchestrator : Node {
  [Export] private Camera2D camera = null!;

  public Levels.Level? CurrentLevel { get; private set; }

  private Timeline? timeline;

  private bool levelActivated = true;
  private readonly List<Adventurer> adventurers = [];
  private int focusedAdventurerIndex = -1;
  private double playbackTimeRemaining;

  private Adventurer? focusedAdventurer => focusedAdventurerIndex >= 0 ? adventurers[focusedAdventurerIndex] : null;

  public void SetLevel(Levels.Level level) {
    if (CurrentLevel is not null) {
      cleanUpPreviousLevel();
    }

    CurrentLevel = level;
    levelActivated = false;
    timeline = new Timeline(GetTree(), level.RoundCount);

    camera.LimitLeft = 0;
    camera.LimitRight = level.WidthInPixels;
    camera.LimitTop = 0;
    camera.LimitBottom = level.HeightInPixels;
  }

  private void cleanUpPreviousLevel() {
    CurrentLevel = null;
    unfocusAdventurer();
    adventurers.Clear();
    playbackTimeRemaining = 0;
  }

  public override void _Ready() {
    Global.Services.ProvideInScene(this);
  }

  public override void _Process(double delta) {
    if (playbackTimeRemaining > 0) {
      playbackTimeRemaining = Math.Max(0, playbackTimeRemaining - delta);
    }

    if (!levelActivated && CurrentLevel is not null) {
      activateLevel();
    }
  }

  public override void _Input(InputEvent @event) {
    if (!levelActivated || focusedAdventurerIndex < 0 || playbackTimeRemaining > 0) return;

    if (timeline!.CurrentRound < timeline.TotalRoundCount) {
      foreach (var action in focusedAdventurer!.AvailableActions) {
        var shortcut = action.Shortcut;
        if (shortcut is null) continue;
        if (@event.IsActionReleased(shortcut)) {
          queueAdventurerAction(action);
        }
      }
    }

    // Clear last action
    if (@event.IsActionReleased(InputActions.Back)) {
      clearLastAdventurerAction();
    }

    // Switch adventurers
    if (@event.IsActionReleased(InputActions.SwitchAdventurers)) {
      focusNextAdventurer();
      var plannedRoundCount = focusedAdventurer!.PlannedRoundCount;
      // We don't move forward in time (yet?), so instead we only check if we need to go back in time.
      if (plannedRoundCount < timeline.CurrentRound) {
        timeline.ResetToRound(plannedRoundCount);
      }
    }
  }

  private void activateLevel() {
    unfocusAdventurer();

    foreach (var portal in CurrentLevel!.AllPortals) {
      var adventurer = portal.TryInstantiateAdventurer();
      if (adventurer is null) continue;

      adventurer.Orchestrator = this;
      AddSibling(adventurer);
      adventurers.Add(adventurer);
    }

    if (adventurers.Count > 0) {
      focusNextAdventurer();
    }

    levelActivated = true;
  }

  private void unfocusAdventurer() {
    focusedAdventurer?.Moved -= onAdventurerMoved;
    focusedAdventurerIndex = -1;
  }

  private void focusNextAdventurer() {
    var nextIndex = (focusedAdventurerIndex + 1) % adventurers.Count;
    unfocusAdventurer();
    focusedAdventurerIndex = nextIndex;
    focusedAdventurer!.Moved += onAdventurerMoved;
    onAdventurerMoved(focusedAdventurer.Coords);
  }

  private void onAdventurerMoved(Vector2I newCoords) {
    if (CurrentLevel is not null) {
      camera.Position = CurrentLevel.GetTile(newCoords).Position;
    }
  }

  private void queueAdventurerAction(IPlannedAction action) {
    focusedAdventurer?.SetActionForRound(timeline!.CurrentRound, action);
    timeline!.Advance();
    playbackTimeRemaining = Constants.TimeBetweenRounds;
  }

  private void clearLastAdventurerAction() {
    if (timeline!.CurrentRound <= 0) return;
    focusedAdventurer?.ClearActionForRound(timeline.CurrentRound - 1);
    timeline.Rollback();
  }
}
