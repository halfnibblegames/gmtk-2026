using Godot;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public interface IPlannedAction {
  // TODO: replace with icon
  public string Name { get; }
  public StringName? Shortcut { get; }

  public PlayerAction AsEnum();
  public void Do(RoundContext context, SimulatedGridObject target);
}
