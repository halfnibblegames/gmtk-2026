using Godot;
using HalfNibbleGame.Adventurers;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Controls;

public partial class CurrentAdventurer : Control {

  private int frontFrameIndex;
  private Tween? portraitRotationTween;

  // Lazily initialized
  private Orchestrator orchestrator = null!;

  [Export] private TextureButton? switchButton;

  public override void _Ready() {
    orchestrator = Global.Services.Get<Orchestrator>();
    orchestrator.AdventurerChanged += onAdventurerChanged;
    switchButton?.Pressed += switchButtonOnPressed;
    base._Ready();
  }

  private void switchButtonOnPressed() {
    orchestrator.FocusNextAdventurer();
  }

  private void onLevelStarted() {
    // TODO: initialize the portraits dynamically based on orchestrator adventurers
  }

  private void onAdventurerChanged(Adventurer adventurer) {
    var indexOfFrameToBringToFront = (frontFrameIndex + 1) % 2;
    var frontFrame = GetNode<Control>($"Frame{frontFrameIndex + 1}");
    var backFrame = GetNode<Control>($"Frame{indexOfFrameToBringToFront + 1}");

    frontFrame.GetNode<ColorRect>("Overlay").Visible = true;
    backFrame.GetNode<Control>("Overlay").Visible = false;

    portraitRotationTween?.Kill();
    portraitRotationTween = GetTree().CreateTween();
    // Moving the front frame to the back.
    portraitRotationTween
      .Parallel()
      .SetEase(Tween.EaseType.InOut)
      .TweenProperty(frontFrame, "position", new Vector2(32, 32), 0.4f)
      .SetTrans(Tween.TransitionType.Elastic);
    portraitRotationTween
      .Parallel()
      .SetEase(Tween.EaseType.InOut)
      .TweenProperty(frontFrame, "z_index", 0, 0.4f)
      .SetTrans(Tween.TransitionType.Spring);

    // Moving the back frame to the front.
    portraitRotationTween
      .Parallel()
      .SetEase(Tween.EaseType.InOut)
      .TweenProperty(backFrame, "position", Vector2.Zero, 0.4f)
      .SetTrans(Tween.TransitionType.Elastic);
    portraitRotationTween
      .Parallel()
      .SetEase(Tween.EaseType.InOut)
      .TweenProperty(backFrame, "z_index", 1, 0.4f)
      .SetTrans(Tween.TransitionType.Bounce);
    portraitRotationTween.Play();

    frontFrameIndex = indexOfFrameToBringToFront;
  }
}
