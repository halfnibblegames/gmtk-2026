
if [ -e "/Applications/Aseprite.app/Contents/MacOS/aseprite" ]; then
    ASEPRITE=/Applications/Aseprite.app/Contents/MacOS/aseprite
else
    echo "Please set your aseprite path!!"
    exit 1
fi

# Portraits
$ASEPRITE -b ./assets/portraits.aseprite --layer Frame --scale 4 --save-as ./assets/portrait_frame.png
$ASEPRITE -b ./assets/portraits.aseprite --layer Rogue --scale 4 --save-as ./assets/portrait_rogue.png
$ASEPRITE -b ./assets/portraits.aseprite --layer Wizard --scale 4 --save-as ./assets/portrait_wizard.png

# Other UI elements
$ASEPRITE -b ./assets/action_border.aseprite --layer Wizard --scale 4 --save-as ./assets/action_border.png
$ASEPRITE -b ./assets/switch_button.aseprite --frame-range 0,0 --scale 4 --save-as ./assets/switch_button.png
$ASEPRITE -b ./assets/switch_button.aseprite --frame-range 1,1 --scale 4 --save-as ./assets/switch_button_pressed.png

echo "Assets exported successfully <3"