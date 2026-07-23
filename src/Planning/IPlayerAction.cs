using Godot;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public interface IPlayerAction : IReplayableAction {
  // TODO: replace with icon
  public string Name { get; }
  public StringName? Shortcut { get; }

  public PlayerAction AsEnum();
}
