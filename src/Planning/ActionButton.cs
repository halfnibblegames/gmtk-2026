using Godot;

namespace HalfNibbleGame.Planning;

[Tool]
public partial class ActionButton : TextureButton {
  [Export]
  private PlayerAction assignedAction {
    get => playerAction?.AsEnum() ?? PlayerAction.None;
    set {
      playerAction = PlannedActions.FromEnum(value);
      updateVisuals();
    }
  }

  private IPlannedAction? playerAction;

  public override void _Ready() {
    updateVisuals();
  }

  private void updateVisuals() {
    GetNode<Label>("Label").Text = playerAction?.Name ?? "";
  }

  public override void _Pressed() {
    base._Pressed();
    if (playerAction is null) return;
    GetParent().GetParent<Planner>().FillNextSlot(playerAction);
  }
}
