
if [ -e "/Applications/Aseprite.app/Contents/MacOS/aseprite" ]; then
    ASEPRITE=/Applications/Aseprite.app/Contents/MacOS/aseprite
else
    echo "Please set your aseprite path!!"
    exit 1
fi

# Portraits
$ASEPRITE -b ./assets/portraits.aseprite --layer Frame --save-as ./assets/frame.png
$ASEPRITE -b ./assets/portraits.aseprite --layer Rogue --save-as ./assets/rogue_portrait.png
$ASEPRITE -b ./assets/portraits.aseprite --layer Wizard --save-as ./assets/wizard_portrait.png

# Other UI elements
$ASEPRITE -b ./assets/action_border.aseprite --layer Wizard --scale 4 --save-as ./assets/action_border.png

echo "Assets exported successfully <3"