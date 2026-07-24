using Godot;
using HalfNibbleGame.Data;

namespace HalfNibbleGame.Grid.LevelObjects;

public partial class Door : StaticGridObject {
  // Lazily initialized
  private Sprite2D sprite = null!;

  private bool open;

  public override void _Ready() {
    base._Ready();
    sprite = GetNode<Sprite2D>("Sprite");
    Level!.RegisterTileModifier(Coords, applyCollisionToTile);
  }

  public void Open() {
    open = true;

    // Oh oh so hacky
    Animations.Animations.DoDelayed(Constants.TimeBetweenRounds * 0.75, () => sprite.Modulate = new Color(sprite.Modulate, 0.5f));
    Animations.Animations.DoDelayed(Constants.TimeBetweenRounds, () => {
      sprite.Modulate = new Color(1, 1, 1);
      sprite.Visible = false;
    });
  }

  public void Close() {
    open = false;
    sprite.Visible = true;
  }

  private Tile applyCollisionToTile(Tile tile) {
    return open ? tile : tile with { Collides = true };
  }
}
