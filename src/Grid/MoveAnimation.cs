using Godot;
using static HalfNibbleGame.Data.Constants;

namespace HalfNibbleGame.Grid;

public abstract class MoveAnimation(MovingGridObject target) {

  private double timeSinceStart;

  protected MovingGridObject Target => target;

  public void Update(double elapsedTime) {
    timeSinceStart += elapsedTime;
    if (timeSinceStart > TimeBetweenRounds) {
      Complete();
    }
    else {
      var t = (float) Mathf.Clamp(timeSinceStart / TimeBetweenRounds, 0.0, 1.0);
      Animate(t);
    }
  }

  public void CompleteInstantly() {
    Complete();
  }

  protected abstract void Animate(float t);
  protected abstract void Complete();

  public static MoveAnimation Move(MovingGridObject target, Vector2 from, Vector2 to) {
    return new NormalMoveAnimation(target, from, to);
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

    protected override void Complete() {
      Target.Position = to;
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

    protected override void Complete() {
      base.Complete();
      Target.Scale = Vector2.One;
      Target.Position = to;
      Target.Modulate = new Color(1, 1, 1);
      Target.Visible = false;
    }
  }
}
