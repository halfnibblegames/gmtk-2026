using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Data;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Controls;

public partial class TopLevelUI : Node {

  private const double transitionDuration = 0.5;
  private const double transitionBackDuration = transitionDuration;
  private const double inputHintDelay = 5.0;

  private bool isPlaybackReady {
    get;
    set {
      field = value;
      playbackButton.Visible = value;
      playbackButtonOverlay.Color = value ? new(0, 0, 0, 0.5f) : Colors.Transparent;
    }
  }

  [Export] private Control gameControl = null!;
  [Export] private Control currentAdventurer = null!;
  [Export] private Control spindownDice = null!;
  [Export] private Control timelineBar = null!;
  [Export] private ColorRect postProcessing = null!;
  [Export] private AudioStreamPlayer planningMusic = null!;
  [Export] private AudioStreamPlayer playingMusic = null!;

  [Export] private BaseButton playbackButton = null!;
  [Export] private ColorRect playbackButtonOverlay = null!;
  [Export] private Control inputHints = null!;

  private Tween? playbackTween;
  private Tween? inputTween;

  private double timeSinceLastInput;

  public override void _Ready() {
    var orchestrator = Global.Services.Get<Orchestrator>();
    orchestrator.LevelStarted += showGame;
    orchestrator.WinConditionChanged += isSonWinning => isPlaybackReady = isSonWinning;
    playbackButton.Pressed += startPlayback;
    resetTransforms();

    planningMusic.Play();
    playingMusic.Play();
  }

  public override void _Process(double delta) {
    if (playbackTween != null) return;

    timeSinceLastInput += delta;
    if (timeSinceLastInput >= inputHintDelay) {
      showInputHints();
    }
  }

  public override void _Input(InputEvent @event) {
    if (isPlaybackReady && @event.IsActionReleased(InputActions.Playback)) {
      startPlayback();
    }

    if (@event is InputEventKey or InputEventMouseButton) {
      hideInputHints();
    }
  }

  private void showInputHints() {
    inputTween?.Kill();

    inputTween = CreateTween();
    inputTween.TweenProperty(inputHints, "modulate", Colors.White, 0.25);
    inputTween.Play();
  }

  private void hideInputHints() {
    inputTween?.Kill();

    inputTween = CreateTween();
    inputTween.TweenProperty(inputHints, "modulate", Colors.Transparent, 0.15);
    inputTween.Play();

    timeSinceLastInput = 0;
  }

  private void showGame() {
    playbackTween?.Kill();
    playbackTween = GetTree().CreateTween();

    playbackTween
      .Chain()
      .TweenProperty(gameControl, "modulate", Colors.White, transitionBackDuration);
  }

  private void startPlayback() {
    if (!isPlaybackReady) return;

    Global.Services.Get<Orchestrator>().PreparePlayback();

    inputHints.Visible = false;

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
      .TweenProperty(playbackButton, "offset_transform_position", 600 * Vector2.Down, transitionDuration)
      .SetEase(Tween.EaseType.In);
    playbackTween
      .Parallel()
      .TweenProperty(playbackButtonOverlay, "color", Colors.Transparent, transitionDuration / 2)
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
      .TweenProperty(gameControl, "modulate", Colors.Transparent, transitionBackDuration);
    playbackTween
      .Parallel()
      .TweenProperty(planningMusic, "volume_linear", 1f, transitionBackDuration);
    playbackTween
      .Parallel()
      .TweenProperty(playingMusic, "volume_linear", 0f, transitionBackDuration);

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
    inputHints.Visible = true;
    gameControl.Modulate = Colors.Transparent;
    playbackButton.Visible = false;
    playbackButtonOverlay.Color = Colors.Transparent;
    currentAdventurer.OffsetTransformPosition = Vector2.Zero;
    spindownDice.OffsetTransformPosition = Vector2.Zero;
    timelineBar.OffsetTransformPosition = Vector2.Zero;
    playbackButton.OffsetTransformPosition = Vector2.Zero;
    setVignetteAmount(0);
    planningMusic.VolumeLinear = 1;
    playingMusic.VolumeLinear = 0;
    playbackTween = null;
  }

  private void setVignetteAmount(float amount) {
    ((ShaderMaterial) postProcessing.Material).SetShaderParameter("vignette_amount", amount);
  }
}
