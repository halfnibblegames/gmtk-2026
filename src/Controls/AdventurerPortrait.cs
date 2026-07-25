using Godot;
using HalfNibbleGame.Adventurers;

namespace HalfNibbleGame.Controls;

public partial class AdventurerPortrait : TextureRect {
  // Lazily initialized
  private TextureRect portrait = null!;
  private ColorRect overlay = null!;

  public Adventurer? Adventurer { get; private set; }

  public override void _Ready() {
    portrait = GetNode<TextureRect>("Portrait");
    overlay = GetNode<ColorRect>("Overlay");
  }

  public void SetAdventurer(Adventurer adventurer) {
    Adventurer = adventurer;
    portrait.Texture = adventurer.Portrait;
  }

  public void MakeActive() {
    overlay.Visible = false;
  }

  public void MakeInactive() {
    overlay.Visible = true;
  }
}
