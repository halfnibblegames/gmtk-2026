using Godot;

namespace HalfNibbleGame.Planning;

[Tool]
public partial class SlotButton : TextureButton {
  [Export] private Texture2D emptyTexture = null!;
  [Export] private Texture2D fullTexture = null!;

  public IPlannedAction? PlayerAction {
    get;
    set {
      field = value;
      updateVisuals();
    }
  }

  public override void _Ready() {
    updateVisuals();
  }

  private void updateVisuals() {
    TextureNormal = PlayerAction is null ? emptyTexture : fullTexture;
    GetNode<Label>("Label").Text = PlayerAction?.Name ?? "";
  }
}
