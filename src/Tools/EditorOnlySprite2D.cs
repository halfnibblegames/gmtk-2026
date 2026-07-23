using Godot;

namespace HalfNibbleGame.Tools;

[Tool]
public partial class EditorOnlySprite2D : Sprite2D {
  public override void _Ready() {
    if (!Engine.IsEditorHint()) {
      Visible = false;
    }
    base._Ready();
  }
}
