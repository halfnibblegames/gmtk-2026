using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame;

public partial class Adventurer : ReplayableGridObject {
  public override void _Ready() {
    TeleportTo(new Vector2I(6, 6));
  }

  public override void _Input(InputEvent @event) {
    if (@event.IsActionReleased(InputActions.Left)) {
      TryMove(Vector2I.Left);
    }
    if (@event.IsActionReleased(InputActions.Right)) {
      TryMove(Vector2I.Right);
    }
    if (@event.IsActionReleased(InputActions.Up)) {
      TryMove(Vector2I.Up);
    }
    if (@event.IsActionReleased(InputActions.Down)) {
      TryMove(Vector2I.Down);
    }
    if (@event.IsActionReleased(InputActions.Back)) {
      TryRevertLastMove();
    }
  }
}
