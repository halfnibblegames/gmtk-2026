using System;
using Godot;

namespace HalfNibbleGame.Scenes;

public partial class ControlPrompt {
  private static ControlData createData(ControlInput input) => input switch {
    ControlInput.ControllerButtonA => new ControlData(47, 42),
    ControlInput.ControllerButtonB => new ControlData(48, 43),
    ControlInput.ControllerButtonX => new ControlData(49, 44),
    ControlInput.ControllerButtonY => new ControlData(50, 45),

    ControlInput.ControllerButtonStart => new ControlData(707, 741),
    ControlInput.ControllerButtonBack => new ControlData(706, 740),

    ControlInput.ControllerButtonLeft => new ControlData(553, 587),
    ControlInput.ControllerButtonRight => new ControlData(554, 588),
    ControlInput.ControllerTriggerLeft => new ControlData(551, 585),
    ControlInput.ControllerTriggerRight => new ControlData(552, 586),

    ControlInput.ControllerDPadUp => new ControlData(34, 35, true),
    ControlInput.ControllerDPadRight => new ControlData(34, 36, true),
    ControlInput.ControllerDPadDown => new ControlData(34, 37, true),
    ControlInput.ControllerDPadLeft => new ControlData(34, 38, true),
    ControlInput.ControllerDPadAll => new ControlData(34, 41, true),

    ControlInput.MouseButtonLeft => new ControlData(76, 77, true),
    ControlInput.MouseButtonRight => new ControlData(76, 78, true),
    ControlInput.MouseButtonMiddle => new ControlData(76, 79, true),

    ControlInput.KeyboardKeyLeft => new ControlData(169, 441),
    ControlInput.KeyboardKeyRight => new ControlData(167, 439),
    ControlInput.KeyboardKeyUp => new ControlData(166, 438),
    ControlInput.KeyboardKeyDown => new ControlData(168, 440),

    ControlInput.KeyboardKeyShift => new ControlData(255, 527, new Vector2I(2, 1)),
    ControlInput.KeyboardKeyTab => new ControlData(189, 461, new Vector2I(2, 1)),
    ControlInput.KeyboardKeyBackspace => new ControlData(66, 338, new Vector2I(2, 1)),

    _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
  };

  private readonly record struct ControlData(int FrameNo, int PressedFrameNo, Vector2I Size, bool Invert = false) {
    public ControlData(int frameNo, int pressedFrameNo, bool invert = false)
      : this(frameNo, pressedFrameNo, Vector2I.One, invert) {}
  }
}
