using System;
using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Adventurers;
using HalfNibbleGame.Autoload;

namespace HalfNibbleGame.Controls;

public partial class CurrentAdventurer : Control {

  private const float animationDuration = 0.4f;

  private int frontFrameIndex;
  private Tween? portraitRotationTween;
  private readonly List<AdventurerPortrait> portraits = [];

  // Lazily initialized
  private Orchestrator orchestrator = null!;

  [Export] private TextureButton? switchButton;
  [Export] private PackedScene portraitPrefab = null!;

  public override void _Ready() {
    orchestrator = Global.Services.Get<Orchestrator>();
    orchestrator.LevelStarted += onLevelStarted;
    orchestrator.AdventurerChanged += onAdventurerChanged;
    switchButton?.Pressed += switchButtonOnPressed;
    base._Ready();
  }

  private void switchButtonOnPressed() {
    orchestrator.FocusNextAdventurer();
  }

  private void onLevelStarted() {
    portraitRotationTween?.Kill();
    portraitRotationTween = null;

    portraits.ForEach(portrait => portrait.QueueFree());
    portraits.Clear();

    for (var i = 0; i < orchestrator.Adventurers.Count; i++) {
      var portrait = portraitPrefab.Instantiate<AdventurerPortrait>();
      AddChild(portrait);

      portrait.SetAdventurer(orchestrator.Adventurers[i]);
      if (i == 0) {
        portrait.MakeActive();
      }
      else {
        portrait.MakeInactive();
      }

      portrait.ZIndex = orchestrator.Adventurers.Count - i;
      portrait.Position = Vector2.One * 32f * i;

      portraits.Add(portrait);
    }

    frontFrameIndex = 0;
    switchButton?.Visible = portraits.Count > 1;
  }

  private void onAdventurerChanged(Adventurer adventurer) {
    var currentlyInFront = frontFrameIndex;
    var targetInFront = portraits.FindIndex(portrait => portrait.Adventurer == adventurer);

    if (targetInFront < 0) {
      throw new Exception("Could not find adventurer portrait to target");
    }
    if (targetInFront == currentlyInFront) {
      return;
    }

    var numberOfRotations = (targetInFront + portraits.Count - currentlyInFront) % portraits.Count;

    portraitRotationTween?.Kill();
    portraitRotationTween = GetTree().CreateTween();

    var durationPerRotation = animationDuration / numberOfRotations;

    for (var i = 0; i < numberOfRotations; i++) {
      var newFrontIndex = (currentlyInFront + i + 1) % portraits.Count;

      for (var k = 0; k < portraits.Count; k++) {
        var targetIndexForPortrait = (k + portraits.Count - newFrontIndex) % portraits.Count;
        var portrait = portraits[k];

        if (k == 0) {
          portraitRotationTween.Chain();
        }
        else {
          portraitRotationTween.Parallel();
        }

        portraitRotationTween
          .SetEase(Tween.EaseType.InOut)
          .TweenProperty(portrait, "position", Vector2.One * 32f * targetIndexForPortrait, durationPerRotation)
          .SetTrans(Tween.TransitionType.Elastic);
        portraitRotationTween
          .Parallel()
          .SetEase(Tween.EaseType.InOut)
          .TweenProperty(portrait, "z_index", portraits.Count - targetIndexForPortrait, durationPerRotation)
          .SetTrans(Tween.TransitionType.Spring);
      }
    }

    for (var k = 0; k < portraits.Count; k++) {
      if (k == targetInFront) {
        portraits[k].MakeActive();
      }
      else {
        portraits[k].MakeInactive();
      }
    }

    frontFrameIndex = targetInFront;
    portraitRotationTween.Play();
  }
}
