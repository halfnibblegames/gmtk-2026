using System;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public static class PlayerActions {

  public static IPlayerAction? FromEnum(PlayerAction action) => action switch {
    PlayerAction.None => null,
    PlayerAction.MoveLeft => new MoveAction(action, Vector2I.Left, "<", InputActions.Left),
    PlayerAction.MoveRight => new MoveAction(action, Vector2I.Right, ">", InputActions.Right),
    PlayerAction.MoveUp => new MoveAction(action, Vector2I.Up, "^", InputActions.Up),
    PlayerAction.MoveDown => new MoveAction(action, Vector2I.Down, "V", InputActions.Down),
    _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
  };

  private class MoveAction(PlayerAction asEnum, Vector2I diff, string name, StringName? shortcut) : IPlayerAction {
    public string Name => name;
    public StringName? Shortcut => shortcut;
    public PlayerAction AsEnum() => asEnum;

    public IReplayableAction ToReplayableAction(MovingGridObject target) {
      return Actions.Move(target, diff);
    }
  }
}
