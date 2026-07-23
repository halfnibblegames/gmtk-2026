using Godot;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public interface IPlayerAction {
  // TODO: replace with icon
  public string Name { get; }
  public StringName? Shortcut { get; }

  public PlayerAction AsEnum();
  public IReplayableAction ToReplayableAction(MovingGridObject target);
}
