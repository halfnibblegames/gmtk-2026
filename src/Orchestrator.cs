using System;
using Godot;
using HalfNibbleGame.Autoload;
using HalfNibbleGame.Grid;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame;

public partial class Orchestrator : Node {
  [Export] private Camera2D camera = null!;

  public Level? CurrentLevel { get; private set; }
  public MovingGridObject? FocusedObject { get; private set; }

  private bool levelActivated = true;

  public void SetLevel(Level level) {
    if (CurrentLevel is not null) {
      // TODO: in the future we want to do proper cleanup, but right now important cleanup is missing
      throw new Exception();
    }

    CurrentLevel = level;
    levelActivated = false;

    camera.LimitLeft = 0;
    camera.LimitRight = level.WidthInPixels;
    camera.LimitTop = 0;
    camera.LimitBottom = level.HeightInPixels;
  }

  public override void _Ready() {
    var timeline = new Timeline(GetTree());
    Global.Services.ProvideInScene(timeline);
  }

  public override void _Process(double delta) {
    if (!levelActivated && CurrentLevel is not null) {
      activateLevel();
    }
  }

  private void activateLevel() {
    unfocusObject();

    foreach (var spawn in CurrentLevel!.AllSpawns) {
      var adventurer = spawn.TryInstantiateAdventurer();
      if (adventurer is null) continue;

      adventurer.Orchestrator = this;
      AddSibling(adventurer);

      if (FocusedObject is null) {
        FocusObject(adventurer);
      }
    }

    levelActivated = true;
  }

  public void FocusObject(MovingGridObject obj) {
    if (FocusedObject is not null) {
      FocusedObject.Moved -= onFocusedObjectMoved;
    }

    FocusedObject = obj;
    FocusedObject.Moved += onFocusedObjectMoved;
    onFocusedObjectMoved(FocusedObject.Coords);
  }

  private void unfocusObject() {
    if (FocusedObject is not null) {
      FocusedObject.Moved -= onFocusedObjectMoved;
    }

    FocusedObject = null;
  }

  private void onFocusedObjectMoved(Vector2I newCoords) {
    if (CurrentLevel is not null) {
      camera.Position = CurrentLevel.GetTile(newCoords).Position;
    }
  }
}
