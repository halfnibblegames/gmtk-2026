using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid.LevelObjects;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;
using Adventurer = HalfNibbleGame.Adventurers.Adventurer;

namespace HalfNibbleGame;

public partial class Orchestrator : Node {
  [Export] private Camera2D camera = null!;

  public delegate void AdventurerChangedEventHandler(Adventurer adventurer);
  public delegate void LevelStartedEventHandler();

  public Levels.Level? CurrentLevel { get; private set; }

  public Timeline? Timeline;

  public event AdventurerChangedEventHandler AdventurerChanged = delegate {};
  public event Timeline.CountdownChangedEventHandler TimelineCountdownChanged = delegate {};
  public event LevelStartedEventHandler LevelStarted = delegate {};

  private bool levelActivated = true;
  private readonly List<Adventurer> adventurers = [];
  private readonly List<HistoryArrow> historyArrows = [];
  private int focusedAdventurerIndex = -1;
  private double playbackTimeRemaining;
  private bool hasWon;
  private IPlannedAction? actionAfterPlayback;

  private Adventurer? focusedAdventurer => focusedAdventurerIndex >= 0 ? adventurers[focusedAdventurerIndex] : null;

  public IReadOnlyList<Adventurer> Adventurers => adventurers;

  public void SetLevel(Levels.Level level) {
    if (CurrentLevel is not null) {
      cleanUpPreviousLevel();
    }

    CurrentLevel = level;
    levelActivated = false;
    hasWon = false;
    Timeline = new Timeline(GetTree(), level.RoundCount);
    Timeline.CountdownChanged += (c, t) => TimelineCountdownChanged(c, t);

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
    if (hasWon) return;

    if (!levelActivated && CurrentLevel is not null) {
      activateLevel();
    }

    if (playbackTimeRemaining > 0) {
      playbackTimeRemaining = Math.Max(0, playbackTimeRemaining - delta);
    }

    if (playbackTimeRemaining <= 0 && actionAfterPlayback is not null) {
      tryQueueAdventurerAction(actionAfterPlayback);
      actionAfterPlayback = null;
    }

    if (playbackTimeRemaining <= 0 && checkWinCondition()) {
      GD.Print("You win!");
      startPlayback();
    }
  }

  public override void _Input(InputEvent @event) {
    if (!levelActivated || focusedAdventurerIndex < 0 || hasWon) return;

    if (Timeline!.CurrentRound < Timeline.TotalRoundCount) {
      foreach (var action in focusedAdventurer!.AvailableActions) {
        var shortcut = action.Shortcut;
        if (shortcut is null) continue;
        if (!@event.IsActionReleased(shortcut)) continue;
        if (tryQueueAdventurerAction(action)) break;
      }
    }

    // Clear last action
    if (@event.IsActionReleased(InputActions.Back)) {
      clearLastAdventurerAction();
    }

    // Switch adventurers
    if (@event.IsActionReleased(InputActions.SwitchAdventurers)) {
      FocusNextAdventurer();
    }
  }

  private void activateLevel() {
    unfocusAdventurer();

    foreach (var portal in CurrentLevel!.AllPortals) {
      var adventurer = portal.TryInstantiateAdventurer();
      if (adventurer is null) continue;

      adventurer.Position = portal.Position;
      CurrentLevel.AddChild(adventurer);
      adventurers.Add(adventurer);

      var historyArrow = Global.Prefabs.HistoryArrow.Instantiate<HistoryArrow>();
      historyArrow.Name = $"{adventurer.Name}History";
      historyArrow.SetHistory(adventurer.History);
      adventurer.AddSibling(historyArrow);
      historyArrows.Add(historyArrow);
    }

    camera.ForceUpdateScroll();
    TimelineCountdownChanged(0, Timeline!.TotalRoundCount);
    LevelStarted();

    // Focus the adventurer here so that LevelStarted is triggered first
    if (adventurers.Count > 0) {
      FocusNextAdventurer();
    }

    levelActivated = true;
  }

  private void unfocusAdventurer() {
    focusedAdventurer?.Moved -= onAdventurerMoved;
    focusedAdventurerIndex = -1;
  }

  public void FocusNextAdventurer() {
    var nextIndex = (focusedAdventurerIndex + 1) % adventurers.Count;
    unfocusAdventurer();
    focusAdventurer(nextIndex);
  }

  private void focusNextAdventurerWithUnplannedMoves() {
    var startIndex = focusedAdventurerIndex;
    var index = (startIndex + 1) % adventurers.Count;
    while (index != startIndex) {
      if (adventurers[index].PlannedRoundCount < Timeline!.TotalRoundCount) {
        focusAdventurer(index);
        break;
      }
      index = (index + 1) % adventurers.Count;
    }
  }

  private void focusAdventurer(int index) {
    focusedAdventurerIndex = index;
    focusedAdventurer!.Moved += onAdventurerMoved;
    onAdventurerMoved(focusedAdventurer.Coords);

    var plannedRoundCount = focusedAdventurer!.PlannedRoundCount;
    // We don't move forward in time (yet?), so instead we only check if we need to go back in time.
    if (plannedRoundCount < Timeline!.CurrentRound) {
      Timeline.ResetToRound(plannedRoundCount);
    }
    AdventurerChanged(focusedAdventurer!);
  }

  private void onAdventurerMoved(Vector2I newCoords) {
    if (CurrentLevel is not null) {
      camera.Position = CurrentLevel.GetTile(newCoords).Position;
    }
  }

  private bool tryQueueAdventurerAction(IPlannedAction action) {
    if (focusedAdventurer is null) {
      return false;
    }

    if (!action.CheckValid(focusedAdventurer)) {
      return false;
    }

    // If we are still playing back the previous animation, we may already hit the new animation early.
    // We queue it up (overriding any previous actions) and immediately execute it after.
    if (playbackTimeRemaining > 0) {
      actionAfterPlayback = action;
      return true;
    }

    focusedAdventurer.SetActionForRound(Timeline!.CurrentRound, action);
    Timeline!.Advance();
    playbackTimeRemaining = Constants.TimeBetweenRounds;

    // Automatically focus the next adventurer that still needs moves if there is one.
    if (focusedAdventurer.PlannedRoundCount == Timeline.TotalRoundCount) {
      focusNextAdventurerWithUnplannedMoves();
    }

    return true;
  }

  private void clearLastAdventurerAction() {
    if (Timeline!.CurrentRound <= 0) return;
    focusedAdventurer?.ClearActionForRound(Timeline.CurrentRound - 1);
    Timeline.Rollback();
  }

  private bool checkWinCondition() {
    // The timeline needs to be advanced all the way to the last round to check the win condition.
    if (Timeline!.CurrentRound != Timeline.TotalRoundCount) return false;

    var portalLocations = CurrentLevel!.AllPortals;

    foreach (var adventurer in adventurers) {
      if (adventurer.PlannedRoundCount != Timeline.TotalRoundCount) {
        return false;
      }

      if (!adventurer.Alive) {
        return false;
      }

      if (portalLocations.All(p => p.Coords != adventurer.Coords)) {
        return false;
      }
    }

    foreach (var treasure in GetTree().GetNodesInGroup(Groups.Treasure).OfType<Treasure>()) {
      if (treasure.PickedUpBy is null) {
        return false;
      }
    }

    return true;
  }

  private void startPlayback() {
    hasWon = true;
    unfocusAdventurer();
    // TODO: follow the adventurer that picked up the loot
    historyArrows.ForEach(a => a.Visible = false);
    Global.Services.Get<TimelinePlayer>().Play(Timeline!, onPlaybackComplete);
  }

  private void onPlaybackComplete() {
    Global.Services.Get<GameProgression>().LoadNextLevel();
  }
}
