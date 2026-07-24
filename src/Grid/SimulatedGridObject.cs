using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public abstract partial class SimulatedGridObject : MovingGridObject, ISimulated, IMortal {

  private readonly Plan plan = new();
  private readonly History<RoundState> history = new();

  // TODO: should probably be more complex
  private bool dead;
  private int stunnedTurns;

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    AddToGroup(Groups.Mortal);
  }

  public void Advance(RoundContext context) {
    history.Push(new RoundState(Coords, Forward, dead, stunnedTurns));
    var action = plan.GetActionForRound(context.RoundNumber);
    action?.Do(context, this);

    if (stunnedTurns > 0) {
      stunnedTurns--;
    }
  }

  public void ResetToRound(int roundNumber) {
    var roundState = history.LastKnownStateInRound(roundNumber);
    TeleportTo(roundState.Coords);
    Forward = roundState.Forward;
    dead = roundState.Dead;
    stunnedTurns = roundState.StunnedTurns;

    Visible = !dead;
    Modulate = new Color(1, 1, 1);
    Scale = Vector2.One;

    history.InvalidateFrom(roundNumber);
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
      context.RegisterOutcome(Die);
    }
  }

  public void Die() {
    GD.Print("Oh dear, you're dead!");
    dead = true;
  }

  public void Stun(int turnCount) {
    GD.Print($"Oof! stunned for {turnCount} turns");
    stunnedTurns = turnCount;
  }

  protected override bool IsMovementPrevented() {
    return dead || stunnedTurns > 0;
  }

  private readonly record struct RoundState(Vector2I Coords, Vector2I Forward, bool Dead, int StunnedTurns);
}
