#! /bin/sh
# $1 STAGING_PATH Path to staged installer directory: 						/Users/buildAgent/TeamCityData/MMGD/Release/ContinuousMac
# $2 FONT_LOCATION Path to fonts we are going to install: 					/Users/buildAgent/TeamCityData/MMGD/Release/ContinuousMac/modules/{3SSFonts}/1/fonts
# $3 FONT_TARGET Path to copy fonts to				 						/Users/buildAgent/TeamCityData/MMGD/Release/
# $4 PRODUCT_NAME Name of the product we are creating:						Name of the product we are creating, basically 3StepStudioAlpha
STAGING_PATH="$1"
FONT_LOCATION="$2"
FONT_TARGET="$3"
PRODUCT_NAME="$4"

# Simulate the end-user's hard drive setup using /, /Applications, and /Library/Fonts
cd "$STAGING_PATH"
cd "../"
#mkdir "/"
#cd ":"
rm -rf "Applications"
rm -rf "Library"

mkdir "./Applications"
mkdir "./Applications/$PRODUCT_NAME"
mkdir "./Library"
mkdir "./Library/Fonts"

cp "$STAGING_PATH" "./:/$PRODUCT_NAME"

# Recursively copy all the fonts to the /FONT_TARGET/Library/Fonts location
cd "$FONT_LOCATION"
for i in `find . -name '*.ttf'`; do cp -p $i "$FONT_TARGET/Library/Fonts";done;
