using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame;

public partial class Adventurer : GridObject {
  public override void _Ready() {
    TeleportTo(new Vector2I(6, 6));
  }

  public override void _Input(InputEvent @event) {
    if (@event.IsActionReleased(InputActions.Left)) {
      move(Vector2I.Left);
    }
    if (@event.IsActionReleased(InputActions.Right)) {
      move(Vector2I.Right);
    }
    if (@event.IsActionReleased(InputActions.Up)) {
      move(Vector2I.Up);
    }
    if (@event.IsActionReleased(InputActions.Down)) {
      move(Vector2I.Down);
    }
  }

  private void move(Vector2I diff) {
    if (diff.LengthSquared() != 1) {
      GD.PushWarning($"Attempting to move {this} by more than one step: {diff}");
    }

    // TODO: make this a nicer move
    TeleportTo(Coords + diff);
  }
}
