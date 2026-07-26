using Godot;

namespace HalfNibbleGame.Grid;

public abstract class MoveAnimation(MovingGridObject target, double duration) {

  public delegate MoveAnimation MoveAnimationFactory(MovingGridObject target, double duration);

  private double timeSinceStart;

  protected MovingGridObject Target => target;

  public bool IsComplete { get; private set; }

  public void Update(double elapsedTime) {
    timeSinceStart += elapsedTime;
    if (timeSinceStart > duration) {
      Complete();
      IsComplete = true;
    }
    else {
      var t = (float) Mathf.Clamp(timeSinceStart / duration, 0.0, 1.0);
      Animate(t);
    }
  }

  public void CompleteInstantly() {
    Complete();
    IsComplete = true;
  }

  protected abstract void Animate(float t);
  protected abstract void Complete();

  public static MoveAnimationFactory Move(Vector2 from, Vector2 to) {
    return (target, duration) => new NormalMoveAnimation(target, duration, from, to);
  }

  public static MoveAnimationFactory Fall(Vector2 from, Vector2 to) {
    return (target, duration) => new FallMoveAnimation(target, duration, from, to);
  }

  private class NormalMoveAnimation(MovingGridObject target, double duration, Vector2 from, Vector2 to) : MoveAnimation(target, duration) {
    protected override void Animate(float t) {
      Target.Position = from + t * (to - from);
    }

    protected override void Complete() {
      Target.Position = to;
    }
  }

  private class FallMoveAnimation(MovingGridObject target, double duration, Vector2 from, Vector2 to) : NormalMoveAnimation(target, duration, from, to) {
    private readonly Vector2 to = to;

    protected override void Animate(float t) {
      // We move in the first half of the animation only. Doing it this way allows us to do the fall animation in the
      // second half.
      // TODO: the movement here will now happen twice as fast.
      base.Animate(Mathf.Min(1.0f, t * 2));
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
