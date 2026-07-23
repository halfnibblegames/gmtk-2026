using System;
using System.Collections.Generic;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Planning;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public abstract partial class SimulatedGridObject : MovingGridObject, ISimulated {

  private Vector2I storedCoords;
  private readonly PlanExecutor planExecutor;

  protected SimulatedGridObject() {
    planExecutor = new PlanExecutor(this);
  }

  public override void _Ready() {
    base._Ready();
    AddToGroup(Groups.Simulated);
  }

  public void Advance(RoundInfo info) {
    planExecutor.Advance(info);
  }

  public void Reset() {
    planExecutor.Reset();
    Forward = Vector2I.Zero;
    TeleportTo(storedCoords);
  }

  public void Snapshot() {
    storedCoords = Coords;
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

    public void Advance(RoundInfo info) {
      if (info.RoundNumber != currentRound) {
        throw new Exception("Round numbers aren't matching");
      }

      if (actions.Count <= currentRound) {
        GD.PushWarning("Tried executing more rounds than the plan accounts for");
        return;
      }

      actions[currentRound++].Do(target);
    }

    public void Reset() {
      currentRound = 0;
    }
  }
}
