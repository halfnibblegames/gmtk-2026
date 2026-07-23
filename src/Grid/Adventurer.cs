using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public partial class Adventurer : ReplayableGridObject {
  public override void _Input(InputEvent @event) {
    // TODO: should only happen for the active adventurer, not all of them
    if (@event.IsActionReleased(InputActions.Left)) {
      tryMove(Vector2I.Left);
    }
    if (@event.IsActionReleased(InputActions.Right)) {
      tryMove(Vector2I.Right);
    }
    if (@event.IsActionReleased(InputActions.Up)) {
      tryMove(Vector2I.Up);
    }
    if (@event.IsActionReleased(InputActions.Down)) {
      tryMove(Vector2I.Down);
    }
    if (@event.IsActionReleased(InputActions.Back)) {
      Global.Services.Get<Timeline>().Rollback();
    }
  }

  private bool tryMove(Vector2I diff) {
    if (!TryQueueMove(diff)) {
      return false;
    }

    Global.Services.Get<Timeline>().Advance();
    return true;
  }
}
