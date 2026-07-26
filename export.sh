
if [ -e "/Applications/Aseprite.app/Contents/MacOS/aseprite" ]; then
    ASEPRITE=/Applications/Aseprite.app/Contents/MacOS/aseprite
else
    echo "Please set your aseprite path!!"
    exit 1
fi

# Portraits
$ASEPRITE -b ./assets/ui/portraits.aseprite --layer Frame --scale 4 --save-as ./assets/ui/portrait_frame.png
$ASEPRITE -b ./assets/ui/portraits.aseprite --layer Rogue --scale 4 --save-as ./assets/ui/portrait_rogue.png
$ASEPRITE -b ./assets/ui/portraits.aseprite --layer Wizard --scale 4 --save-as ./assets/ui/portrait_wizard.png
$ASEPRITE -b ./assets/ui/portraits.aseprite --layer Barbarian --scale 4 --save-as ./assets/ui/portrait_barbarian.png

# Action sockets
$ASEPRITE -b ./assets/ui/actions.aseprite --layer Socket --frame-range 0,0 --save-as ./assets/ui/action_sockets.png
$ASEPRITE -b ./assets/ui/actions.aseprite --layer Pip --frame-range 0,0 --save-as ./assets/ui/action_pip.png
$ASEPRITE -b ./assets/ui/actions.aseprite --layer Action --frame-range 0,0 --save-as ./assets/ui/action_right.png
$ASEPRITE -b ./assets/ui/actions.aseprite --layer Action --frame-range 1,1 --save-as ./assets/ui/action_left.png
$ASEPRITE -b ./assets/ui/actions.aseprite --layer Action --frame-range 2,2 --save-as ./assets/ui/action_up.png
$ASEPRITE -b ./assets/ui/actions.aseprite --layer Action --frame-range 3,3 --save-as ./assets/ui/action_down.png

# Buttons
$ASEPRITE -b ./assets/ui/switch_button.aseprite --frame-range 0,0 --scale 4 --save-as ./assets/ui/switch_button.png
$ASEPRITE -b ./assets/ui/switch_button.aseprite --frame-range 1,1 --scale 4 --save-as ./assets/ui/switch_button_pressed.png
$ASEPRITE -b ./assets/ui/button.aseprite --frame-range 0,0 --scale 4 --save-as ./assets/ui/button.png
$ASEPRITE -b ./assets/ui/button.aseprite --frame-range 1,1 --scale 4 --save-as ./assets/ui/button_pressed.png

# Other UI elements
$ASEPRITE -b ./assets/ui/d20.aseprite --scale 6 --save-as ./assets/ui/d20.png
$ASEPRITE -b ./assets/ui/action_border.aseprite --layer Wizard --scale 2 --save-as ./assets/ui/action_border.png

echo "Assets exported successfully <3"