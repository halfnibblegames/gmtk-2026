using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public abstract partial class SimulatedGridObject : MovingGridObject, ISimulated, IMortal {

  private readonly Plan plan = new();
  public History<RoundState> History { get; } = new();

  // TODO: should probably be more complex
  private bool desynced;
  private bool dead;

  public int PlannedRoundCount => plan.PlannedRoundCount;
  public bool Alive => !dead;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    AddToGroup(Groups.Mortal);
  }

  public void Advance(RoundContext context) {
    History.Push(new RoundState(Coords, Forward, desynced, dead));
    if (desynced) {
      return;
    }

    var action = plan.GetActionForRound(context.RoundNumber);
    if (action is null) {
      return;
    }

    // A previously planned action is no longer valid, which means something in the past changed to mess this up.
    // We go in a desynced state and stop executing the plan.
    if (!action.CheckValid(this)) {
      desynced = true;
      return;
    }
    action.Do(context, this);
  }

  public void ResetToRound(RoundContext context) {
    var roundState = History.LastKnownStateInRound(context.RoundNumber);
    TeleportTo(roundState.Coords);
    Forward = roundState.Forward;
    desynced = roundState.Desynced;
    dead = roundState.Dead;

    Visible = !dead;
    Modulate = new Color(1, 1, 1);
    Scale = Vector2.One;

    History.InvalidateFrom(context.RoundNumber);
  }

  public void SetActionForRound(int roundNumber, IPlannedAction action) {
    plan.SetActionForRound(roundNumber, action);
  }

  public void ClearActionForRound(int roundNumber) {
    plan.ClearActionForRound(roundNumber);
  }

  public void CheckAgainstHazards(List<IHazard> hazards, RoundContext context) {
    if (dead) return;
    if (hazards.Any(h => h.Coords == Coords && h.Hazardous)) {
      context.RegisterOutcome(() => {
        Die();
        Visible = false;
      });
    }
  }

  public void Die() {
    GD.Print("Oh dear, you're dead!");
    dead = true;
  }

  public readonly record struct RoundState(Vector2I Coords, Vector2I Forward, bool Desynced, bool Dead);
}
