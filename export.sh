
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

# Other UI elements
$ASEPRITE -b ./assets/ui/action_border.aseprite --layer Wizard --scale 4 --save-as ./assets/ui/action_border.png
$ASEPRITE -b ./assets/ui/switch_button.aseprite --frame-range 0,0 --scale 4 --save-as ./assets/ui/switch_button.png
$ASEPRITE -b ./assets/ui/switch_button.aseprite --frame-range 1,1 --scale 4 --save-as ./assets/ui/switch_button_pressed.png

echo "Assets exported successfully <3"