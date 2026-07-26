using System;
using Godot;

namespace HalfNibbleGame.Controls;

public partial class CelebrationScreen : Control {
  public void SetCompletionTime(double completionTime) {
    var label = GetNode<Label>("TimeLabel");
    var formattedTime = TimeSpan.FromSeconds(completionTime).ToString(@"mm\:ss");
    label.Text = $"You completed the heists in: {formattedTime}";
  }
}
