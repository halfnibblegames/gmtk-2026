using System;
using System.Collections.Generic;
using System.Linq;
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
  private bool hasWon;
  private IPlannedAction? actionAfterPlayback;

  private Adventurer? focusedAdventurer => focusedAdventurerIndex >= 0 ? adventurers[focusedAdventurerIndex] : null;

  public void SetLevel(Levels.Level level) {
    if (CurrentLevel is not null) {
      cleanUpPreviousLevel();
    }

    CurrentLevel = level;
    levelActivated = false;
    hasWon = false;
    timeline = new Timeline(GetTree(), level.RoundCount);

    camera.LimitLeft = 0;
    camera.LimitRight = level.WidthInPixels;
    camera.LimitTop = 0;
    camera.LimitBottom = level.HeightInPixels;
  }

  private void cleanUpPreviousLevel() {
    CurrentLevel?.QueueFree();
    adventurers.ForEach(a => a.QueueFree());

    CurrentLevel = null;
    unfocusAdventurer();
    adventurers.Clear();
    playbackTimeRemaining = 0;
  }

  public override void _Ready() {
    Global.Services.ProvideInScene(this);
  }

  public override void _Process(double delta) {
    if (!levelActivated && CurrentLevel is not null) {
      activateLevel();
    }

    if (playbackTimeRemaining > 0) {
      playbackTimeRemaining = Math.Max(0, playbackTimeRemaining - delta);
    }

    if (playbackTimeRemaining <= 0 && actionAfterPlayback is not null) {
      queueAdventurerAction(actionAfterPlayback);
      actionAfterPlayback = null;
    }

    if (playbackTimeRemaining <= 0 && checkWinCondition()) {
      GD.Print("You win!");
      startPlayback();
    }
  }

  public override void _Input(InputEvent @event) {
    if (!levelActivated || focusedAdventurerIndex < 0 || hasWon) return;

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
    // If we are still playing back the previous animation, we may already hit the new animation early.
    // We queue it up (overriding any previous actions) and immediately execute it after.
    if (playbackTimeRemaining > 0) {
      actionAfterPlayback = action;
      return;
    }

    focusedAdventurer?.SetActionForRound(timeline!.CurrentRound, action);
    timeline!.Advance();
    playbackTimeRemaining = Constants.TimeBetweenRounds;
  }

  private void clearLastAdventurerAction() {
    if (timeline!.CurrentRound <= 0) return;
    focusedAdventurer?.ClearActionForRound(timeline.CurrentRound - 1);
    timeline.Rollback();
  }

  private bool checkWinCondition() {
    // The timeline needs to be advanced all the way to the last round to check the win condition.
    if (timeline!.CurrentRound != timeline.TotalRoundCount) return false;

    var portalLocations = CurrentLevel!.AllPortals;

    foreach (var adventurer in adventurers) {
      if (adventurer.PlannedRoundCount != timeline.TotalRoundCount) {
        return false;
      }

      if (!adventurer.Alive) {
        return false;
      }

      if (portalLocations.All(p => p.Coords != adventurer.Coords)) {
        return false;
      }
    }

    // TODO: check that the loot has been picked up
    return true;
  }

  private void startPlayback() {
    hasWon = true;
    unfocusAdventurer();
    Global.Services.Get<TimelinePlayer>().Play(timeline!, onPlaybackComplete);
  }

  private void onPlaybackComplete() {
    Global.Services.Get<GameProgression>().LoadNextLevel();
  }
}
