using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HalfNibbleGame.Data;
using HalfNibbleGame.Grid;

namespace HalfNibbleGame.Replay;

public partial class HistoryArrow : TileMapLayer, ISimulated {

  private History<SimulatedGridObject.RoundState>? history;

  public override void _Ready() {
    AddToGroup(Groups.Simulated);
  }

  public void SetHistory(History<SimulatedGridObject.RoundState> hist) {
    history = hist;
  }

  public void Advance(RoundContext context) {
    context.RegisterOutcome(() => updateForRound(context.RoundNumber));
  }

  public void ResetToRound(int roundNumber) {
    updateForRound(roundNumber);
  }

  private void updateForRound(int round) {
    if (history is null) return;

    var endRound = Math.Min(round, history.Length - 1);

    if (endRound <= 0) {
      Clear();
      return;
    }

    var startLocation = history[0].Coords;
    var steps = Enumerable.Range(1, endRound)
      .SelectMany(i => toSteps(history[i].Coords - history[i - 1].Coords))
      .ToList();

    Clear();
    var coords = startLocation;
    for (var i = 0; i < steps.Count; i++) {
      coords += toDiff(steps[i]);

      var arrowPiece = toArrowPiece(steps[i], i < steps.Count - 1 ? steps[i + 1] : null);
      if (arrowPiece is not null) {
        setCell(coords, arrowPiece.Value);
      }
    }
  }

  private void setCell(Vector2I coords, ArrowPiece piece) {
    const int tilesetWidth = 4;
    var tile = new Vector2I(((int) piece) % tilesetWidth, ((int) piece) / tilesetWidth);
    SetCell(coords, 0, tile);
  }

  private enum Step {
    Left,
    Right,
    Up,
    Down,
  }

  // Keep this in the order of the tiles in the sprite for easy conversion to coordinates
  private enum ArrowPiece {
    UpEnd = 0,
    LeftEnd,
    LeftRight,
    RightEnd,
    UpDown,
    DownRight,
    DownLeft,
    Cursor,
    DownEnd,
    UpRight,
    UpLeft,
    Hazard
  }

  private static IEnumerable<Step> toSteps(Vector2I diff) {
    if (diff.X != 0 && diff.Y != 0) {
      throw new ArgumentException("We don't support diagonal steps");
    }

    return diff switch {
      { X: 0, Y: 0 } => [],
      { X: < 0 } => Enumerable.Repeat(Step.Left, -diff.X),
      { X: > 0 } => Enumerable.Repeat(Step.Right, diff.X),
      { Y: < 0 } => Enumerable.Repeat(Step.Up, -diff.Y),
      { Y: > 0 } => Enumerable.Repeat(Step.Down, diff.Y)
    };
  }

  private static Vector2I toDiff(Step step) => step switch {
    Step.Left => Vector2I.Left,
    Step.Right => Vector2I.Right,
    Step.Up => Vector2I.Up,
    Step.Down => Vector2I.Down,
    _ => throw new ArgumentOutOfRangeException(nameof(step), step, null)
  };

  private static ArrowPiece? toArrowPiece(Step from, Step? to) {
    return (from, to) switch {
      (Step.Left, null) => ArrowPiece.LeftEnd,
      (Step.Right, null) => ArrowPiece.RightEnd,
      (Step.Up, null) => ArrowPiece.UpEnd,
      (Step.Down, null) => ArrowPiece.DownEnd,
      (Step.Left, Step.Left) or (Step.Right, Step.Right) => ArrowPiece.LeftRight,
      (Step.Up, Step.Up) or (Step.Down, Step.Down) => ArrowPiece.UpDown,
      (Step.Up, Step.Right) or (Step.Left, Step.Down) => ArrowPiece.DownRight,
      (Step.Up, Step.Left) or (Step.Right, Step.Down) => ArrowPiece.DownLeft,
      (Step.Down, Step.Right) or (Step.Left, Step.Up) => ArrowPiece.UpRight,
      (Step.Down, Step.Left) or (Step.Right, Step.Up) => ArrowPiece.UpLeft,
      _ => null
    };
  }
}
