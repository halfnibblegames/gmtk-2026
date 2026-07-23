using Godot;
using HalfNibbleGame.Grid;

namespace HalfNibbleGame.Planning;

public interface IPlannedAction {
  // TODO: replace with icon
  public string Name { get; }
  public StringName? Shortcut { get; }

  public PlayerAction AsEnum();
  public void Do(MovingGridObject target);
}
