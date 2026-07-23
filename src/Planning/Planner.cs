using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

[Tool]
public partial class Planner : Panel {
  [Export] private PackedScene slotButtonScene = null!;

  [Export] private int slotsCount {
    get;
    set {
      field = value;
      onSlotsCountChanged();
    }
  } = 2;

  private readonly Plan plan = new();
  private List<SlotButton> slotButtons => GetNode<Container>("Slots").GetChildren().OfType<SlotButton>().ToList();

  public override void _Ready() {
    Global.Services.ProvideInScene(this);
    onSlotsCountChanged();
    onPlanChanged();
  }

  // <== Hack
  private Adventurer? adventurer;

  public void SetAdventurer(Adventurer adv) {
    adventurer = adv;
  }

  private void play() {
    if (!plan.Validate()) {
      GD.PushWarning("Plan is not valid");
    }
    else if (adventurer is null) {
      GD.PushError("Adventurer is null");
    }
    else {
      adventurer!.QueueActions(plan.Actions.Select(action => action!.ToReplayableAction(adventurer)));
      Global.Services.Get<TimelinePlayer>().Play(plan.Actions.Count);
    }
  }
  // ==>

  public override void _Input(InputEvent @event) {
    // Queue actions based on input.
    foreach (var action in Enum.GetValues<PlayerAction>()) {
      var actionRecord = PlayerActions.FromEnum(action);
      var shortcut = PlayerActions.FromEnum(action)?.Shortcut;
      if (shortcut is null) continue;
      if (@event.IsActionReleased(shortcut)) {
        FillNextSlot(actionRecord!);
      }
    }

    // Clear last action
    if (@event.IsActionReleased(InputActions.Back)) {
      clearLastSlot();
    }

    // Start playback
    if (@event.IsActionReleased(InputActions.Playback)) {
      play();
    }
  }

  public void FillNextSlot(IPlayerAction action) {
    var firstEmptySlot = plan.Actions.IndexOf(null);
    if (firstEmptySlot == -1) return;
    plan.Actions[firstEmptySlot] = action;
    onPlanChanged();
  }

  private void clearLastSlot() {
    var lastFilledSlot = slotButtons.FindLastIndex(s => s.PlayerAction is not null);
    if (lastFilledSlot == -1) return;
    plan.Actions[lastFilledSlot] = null;
    onPlanChanged();
  }

  private class Plan {
    public List<IPlayerAction?> Actions { get; } = [];

    public bool Validate() {
      return Actions.All(a => a is not null);
    }
  }

  // Update state to match
  private void onSlotsCountChanged() {
    updatePlanActionCount();
    updateSlotButtonCount();
  }

  private void onPlanChanged() {
    var buttons = slotButtons;
    if (plan.Actions.Count != buttons.Count) {
      throw new Exception("Slot buttons and plan action count don't match");
    }
    for (var i = 0; i < plan.Actions.Count; i++) {
      if (buttons[i].PlayerAction != plan.Actions[i]) {
        buttons[i].PlayerAction = plan.Actions[i];
      }
    }
  }

  private void updatePlanActionCount() {
    while (plan.Actions.Count < slotsCount) {
      plan.Actions.Add(null);
    }
    while (plan.Actions.Count > slotsCount) {
      plan.Actions.RemoveAt(plan.Actions.Count - 1);
    }
  }

  private void updateSlotButtonCount() {
    var slotsContainer = GetNodeOrNull<Container>("Slots");
    if (slotsContainer is null) return;
    var buttonCount = slotsContainer.GetChildCount();

    while (buttonCount > slotsCount) {
      buttonCount--;
      slotsContainer.GetChild(buttonCount).QueueFree();
    }

    while (buttonCount < slotsCount) {
      buttonCount++;
      var newButton = slotButtonScene.Instantiate<SlotButton>();
      newButton.Name = $"Slot{buttonCount}";
      slotsContainer.AddChild(newButton);
      newButton.Owner = GetTree().EditedSceneRoot;
    }
  }
}
