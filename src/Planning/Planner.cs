using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HalfNibbleGame.Planning;

[Tool]
public partial class Planner : Panel {
  [Export] private PackedScene slotButtonScene = null!;

  [Export] private int slotsCount {
    get;
    set {
      field = value;
      updateSlotButtons();
    }
  } = 2;

  private List<SlotButton> slots => GetNode<Container>("Slots").GetChildren().OfType<SlotButton>().ToList();

  public override void _Ready() {
    updateSlotButtons();
  }

  private void updateSlotButtons() {
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
      newButton.AssignedAction = PlayerAction.None;
      newButton.Owner = GetTree().EditedSceneRoot;
    }
  }

  public override void _Input(InputEvent @event) {
    foreach (var action in Enum.GetValues<PlayerAction>()) {
      var shortcut = PlayerActions.FromEnum(action)?.Shortcut;
      if (shortcut is null) continue;
      if (@event.IsActionReleased(shortcut)) {
        FillNextSlot(action);
      }
    }
  }

  public void FillNextSlot(PlayerAction action) {
    var firstEmptySlot = slots.FirstOrDefault(s => s.AssignedAction == PlayerAction.None);
    firstEmptySlot?.AssignedAction = action;
  }
}
