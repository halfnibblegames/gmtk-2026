using Godot;

namespace HalfNibbleGame;

public partial class Adventurer : GridObject {
  // Temporary debug code <==
  private static readonly Vector2I[] steps = [
    new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
  ];
  private double nextMoveIn = 1;
  private int nextMoveStep;
  // ==>

  public override void _Ready() {
    TeleportTo(new Vector2I(6, 6));
  }

  public override void _Process(double delta) {
    // Temporary debug code <==
    nextMoveIn -= delta;
    if (nextMoveIn <= 0) {
      TeleportTo(Coords + steps[nextMoveStep]);
      nextMoveIn = 1;
      nextMoveStep = (nextMoveStep + 1) % steps.Length;
    }
    // ==>

    base._Process(delta);
  }
}
