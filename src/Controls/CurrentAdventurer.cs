using Godot;
using HalfNibbleGame;
using HalfNibbleGame.Autoload;
using System;

public partial class CurrentAdventurer : Control {

  private int frontFrameIndex = 0;
  private Tween? portraitRotationTween;
  [Export]
  private TextureButton SwitchButton;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
    Global.Services.Get<Orchestrator>().OnAdventurerChanged += OnAdventurerChanged;
    SwitchButton.Pressed += SwitchButtonOnPressed;
    base._Ready();
	}

  private void SwitchButtonOnPressed() {
    Global.Services.Get<Orchestrator>().FocusNextAdventurer();
  }

  private void OnAdventurerChanged() {
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
