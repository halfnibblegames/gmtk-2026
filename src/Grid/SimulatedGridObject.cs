using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public abstract partial class SimulatedGridObject : MovingGridObject, ISimulated, IMortal {

  // TODO: should probably be more complex
  private bool dead;
  private int stunnedTurns;

  private Vector2I storedCoords;
  private readonly PlanExecutor planExecutor;

  protected SimulatedGridObject() {
    planExecutor = new PlanExecutor(this);
  }

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
    AddToGroup(Groups.Mortal);
  }

  public void Advance(RoundContext context) {
    planExecutor.Advance(context);
    if (stunnedTurns > 0) {
      stunnedTurns--;
    }
  }

  public void Reset() {
    planExecutor.Reset();
    Forward = Vector2I.Zero;
    dead = false;
    stunnedTurns = 0;
    Visible = true;
    TeleportTo(storedCoords);
  }

  public void CheckAgainstHazards(List<IHazard> hazards, RoundContext context) {
    if (dead) return;
    if (hazards.Any(h => h.Coords == Coords && h.IsHazardous)) {
      context.RegisterOutcome(Die);
    }
  }

  public void Snapshot() {
    storedCoords = Coords;
  }

  public void Die() {
    GD.Print("Oh dear, you're dead!");
    dead = true;
    Visible = false;
  }

  public void Stun(int turnCount) {
    GD.Print($"Oof! stunned for {turnCount} turns");
    stunnedTurns = turnCount;
  }

  protected override bool IsMovementPrevented() {
    return dead || stunnedTurns > 0;
  }

  // <== Hack
  public void QueueActions(List<IPlannedAction> actions) {
    planExecutor.SetPlan(actions);
  }
  // ==>

  private class PlanExecutor(SimulatedGridObject target) {
    private List<IPlannedAction> actions = [];
    private int currentRound;

    public void SetPlan(List<IPlannedAction> plan) {
      actions = plan;
    }

    public void Advance(RoundContext context) {
      if (context.RoundNumber != currentRound) {
        throw new Exception("Round numbers aren't matching");
      }

      if (actions.Count <= currentRound) {
        GD.PushWarning("Tried executing more rounds than the plan accounts for");
        return;
      }

      actions[currentRound++].Do(context, target);
    }

    public void Reset() {
      currentRound = 0;
    }
  }
}
