using Godot;

namespace HalfNibbleGame.Planning;

[Tool]
public partial class SlotButton : TextureButton {
  [Export] private Texture2D emptyTexture = null!;
  [Export] private Texture2D fullTexture = null!;

  [Export]
  public PlayerAction AssignedAction {
    get => playerAction?.AsEnum() ?? PlayerAction.None;
    set {
      playerAction = PlayerActions.FromEnum(value);
      updateVisuals();
    }
  }

  private IPlayerAction? playerAction;

  public override void _Ready() {
    updateVisuals();
  }

  private void updateVisuals() {
    TextureNormal = playerAction is null ? emptyTexture : fullTexture;
    GetNode<Label>("Label").Text = playerAction?.Name ?? "";
  }
}
