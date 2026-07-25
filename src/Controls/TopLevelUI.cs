using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Controls;

public partial class TopLevelUI : Node {

  private const double transitionDuration = 0.5;

  private bool isPlaybackReady {
    get;
    set {
      field = value;
      playbackButton.Visible = value;
    }
  }

  [Export] private Control currentAdventurer = null!;
  [Export] private Control spindownDice = null!;
  [Export] private Control timelineBar = null!;
  [Export] private ColorRect postProcessing = null!;
  [Export] private AudioStreamPlayer planningMusic = null!;
  [Export] private AudioStreamPlayer playingMusic = null!;

  [Export] private BaseButton playbackButton = null!;

  private Tween? playbackTween;

  public override void _Ready() {
    var orchestrator = Global.Services.Get<Orchestrator>();
    orchestrator.WinConditionChanged += isSonWinning => isPlaybackReady = isSonWinning;
    playbackButton.Pressed += startPlayback;
    resetTransforms();

    planningMusic.Play();
    playingMusic.Play();
  }

  public override void _Input(InputEvent @event) {
    if (isPlaybackReady && @event.IsActionReleased(InputActions.Playback)) {
      startPlayback();
    }
  }

  private void startPlayback() {
    if (!isPlaybackReady) return;

    Global.Services.Get<Orchestrator>().PreparePlayback();

    currentAdventurer.OffsetTransformEnabled = true;
    spindownDice.OffsetTransformEnabled = true;
    timelineBar.OffsetTransformEnabled = true;
    playbackButton.OffsetTransformEnabled = true;

    playbackTween?.Kill();
    playbackTween = GetTree().CreateTween();

    playbackTween
      .TweenMethod(new Callable(this, MethodName.setVignetteAmount), 0f, 1f, transitionDuration);

    playbackTween
      .Parallel()
      .TweenProperty(currentAdventurer, "offset_transform_position", 300 * Vector2.Left, transitionDuration)
      .SetEase(Tween.EaseType.In);
    playbackTween
      .Parallel()
      .TweenProperty(spindownDice, "offset_transform_position", 300 * Vector2.Right, transitionDuration)
      .SetEase(Tween.EaseType.In);
    playbackTween
      .Parallel()
      .TweenProperty(playbackButton, "offset_transform_position", 300 * Vector2.Right, transitionDuration)
      .SetEase(Tween.EaseType.In);
    playbackTween
      .Parallel()
      .TweenProperty(timelineBar, "offset_transform_position", 150 * Vector2.Down, transitionDuration)
      .SetEase(Tween.EaseType.In);
    playbackTween
      .Parallel()
      .TweenProperty(planningMusic, "volume_linear", 0f, transitionDuration);
    playbackTween
      .Parallel()
      .TweenProperty(playingMusic, "volume_linear", 1f, transitionDuration);

    playbackTween
      .Chain()
      .TweenCallback(new Callable(this, MethodName.startTimelinePlayback));
    playbackTween
      .Chain()
      .TweenAwait(new Signal(Global.Services.Get<TimelinePlayer>(), TimelinePlayer.SignalName.PlaybackCompleted));

    playbackTween
      .Chain()
      .TweenCallback(new Callable(this, MethodName.resetTransforms));
    playbackTween
      .Chain()
      .TweenCallback(new Callable(Global.Services.Get<GameProgression>(), GameProgression.MethodName.LoadNextLevel));
  }

  private void startTimelinePlayback() {
    Global.Services.Get<TimelinePlayer>().Play(Global.Services.Get<Orchestrator>().Timeline!);
  }

  private void resetTransforms() {
    playbackButton.Visible = false;
    currentAdventurer.OffsetTransformPosition = Vector2.Zero;
    spindownDice.OffsetTransformPosition = Vector2.Zero;
    timelineBar.OffsetTransformPosition = Vector2.Zero;
    playbackButton.OffsetTransformPosition = Vector2.Zero;
    setVignetteAmount(0);
    planningMusic.VolumeLinear = 1;
    playingMusic.VolumeLinear = 0;
  }

  private void setVignetteAmount(float amount) {
    ((ShaderMaterial) postProcessing.Material).SetShaderParameter("vignette_amount", amount);
  }
}
