using Godot;

namespace HalfNibbleGame;

public partial class Orchestrator : Node {
  [Export] private Camera2D camera = null!;

  public Level? CurrentLevel { get; private set; }
  public GridObject? FocusedObject { get; private set; }

  public void ActivateLevel(Level level) {
    CurrentLevel = level;

    camera.LimitLeft = 0;
    camera.LimitRight = level.WidthInPixels;
    camera.LimitTop = 0;
    camera.LimitBottom = level.HeightInPixels;
  }

  public void FocusObject(GridObject obj) {
    if (FocusedObject is not null) {
      FocusedObject.Moved -= onFocusedObjectMoved;
    }

    FocusedObject = obj;
    FocusedObject.Moved += onFocusedObjectMoved;
    onFocusedObjectMoved(FocusedObject.Coords);
  }

  private void onFocusedObjectMoved(Vector2I newCoords) {
    if (CurrentLevel is not null) {
      camera.Position = CurrentLevel.GetTile(newCoords).Position;
    }
  }
}
