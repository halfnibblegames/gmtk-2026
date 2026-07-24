using Godot;
using static HalfNibbleGame.Data.Constants;

namespace HalfNibbleGame.Grid;

public abstract class MoveAnimation(MovingGridObject target) {

  private double timeSinceStart;

  protected MovingGridObject Target => target;

  public void Update(double elapsedTime) {
    timeSinceStart += elapsedTime;
    var t = (float) Mathf.Clamp(timeSinceStart / TimeBetweenRounds, 0.0, 1.0);
    Animate(t);
  }

  protected abstract void Animate(float t);

  public static MoveAnimation Move(MovingGridObject target, Vector2 from, Vector2 to) {
    return new NormalMoveAnimation(target, from, to);
  }

  public static MoveAnimation Collide(MovingGridObject target, Vector2 from, Vector2 to) {
    return new CollideMoveAnimation(target, from, to);
  }

  public static MoveAnimation Fall(MovingGridObject target, Vector2 from, Vector2 to) {
    return new FallMoveAnimation(target, from, to);
  }

  private class NormalMoveAnimation(MovingGridObject target, Vector2 from, Vector2 to) : MoveAnimation(target) {
    protected override void Animate(float t) {
      // We move in the first half of the animation only. Doing it this way allows other animations to do something else
      // with the second half of the animation time.
      t = Mathf.Min(1.0f, t * 2);
      Target.Position = from + t * (to - from);
    }
  }

  private class CollideMoveAnimation(MovingGridObject target, Vector2 from, Vector2 to) : NormalMoveAnimation(target, from, to) {
    private readonly Vector2 from = from;
    private readonly Vector2 to = to;

    protected override void Animate(float t) {
      base.Animate(t);
      if (t < 0.5f) return;

      var velocity = (to - from).LengthSquared() < 1
        ? new Vector2(Target.Forward.X, Target.Forward.Y) * 32f
        : 2 * (to - from);

      if (t < 0.6f) {
        var overshoot = t - 0.5f;
        Target.Position = to + velocity * overshoot;
        return;
      }

      var t2 = t - 0.6f;
      var bounce = Mathf.Max(0, 0.1f - t2);
      Target.Position = to + velocity * bounce;

      var flashStrength = 1 - (2.5f * t2);
      Target.Modulate = new Color(1, 1, 1 - flashStrength);
    }
  }

  private class FallMoveAnimation(MovingGridObject target, Vector2 from, Vector2 to) : NormalMoveAnimation(target, from, to) {
    private readonly Vector2 to = to;

    protected override void Animate(float t) {
      base.Animate(t);
      if (t < 0.5f) return;

      var step = Mathf.FloorToInt((t - 0.5f) * 6);
      var alpha = Mathf.Max(0, 1.0f - (0.35f * step));
      Target.Modulate = new Color(Target.Modulate, alpha);
      var scale = 1.0f - 0.25f * step;
      Target.Scale = scale * Vector2.One;
      var offset = step * 1.5f * Vector2.Down;
      Target.Position = to + offset;
    }
  }
}
