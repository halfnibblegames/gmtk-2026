using Godot;
using HalfNibbleGame.Autoload;
using System;

public enum PipActions {
  Up,
  Down,
  Left,
  Right,
  Dash
}

public partial class ActionSocket : Control {
  [Export]
  private TextureRect Pip;
  [Export]
  private TextureRect Action;

  private PipActions? currentAction;

  public void SetCurrentAction(PipActions? action) {
    var pipIsVisible = action is not null;
    var prefabs = Global.Prefabs;
    Action.Texture = action switch {
      PipActions.Up => prefabs.ActionUp,
      PipActions.Down => prefabs.ActionDown,
      PipActions.Left => prefabs.ActionLeft,
      PipActions.Right => prefabs.ActionRight,
      _ => null
    };

    currentAction = action;
    Pip.Visible = pipIsVisible;
    Action.Visible = pipIsVisible;
  }
}
