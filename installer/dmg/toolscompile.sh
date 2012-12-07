#! /bin/sh
# $1 CHECOUT_PATH /Applications/TeamCityData/MMGD/3StepStudioAlpha
# $2 WORKING_DIR tgb

# STAGING

# Move into the checkout dir, raise the permissions just to be safe
cd "$1"
chmod -R 777 "$1"

# Move to the working directory
cd "$2"

# Force the scripts to compile via sudo
./3StepStudio.app/Contents/MacOS/3StepStudio main.toolscompile.cs