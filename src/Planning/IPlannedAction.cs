using Godot;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Planning;

public interface IPlannedAction {
  public StringName? Shortcut { get; }

  public bool CheckValid(SimulatedGridObject target);
  public void Do(RoundContext context, SimulatedGridObject target);
}
