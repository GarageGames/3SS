#!/bin/sh

# $1 Package root directory
# $2 Package name
# $3 Version
# $4 PKG output directory

# This script creates a dmg for the 3StepStudio installer.

BUILT_PRODUCTS_DIR="$1"
PACKAGE_NAME="$2"
VERSION="$3"
OUTPUT_DIR="$4"


echo "DMG commandline argument is $1 : $2 : $3 : $4"

mkdir -p "$OUTPUT_DIR"

/usr/bin/dmgcanvas "$BUILT_PRODUCTS_DIR/installer/dmg/ThreeStepStudio.dmgCanvas" "$OUTPUT_DIR/$PACKAGE_NAME$VERSION.dmg"

    
    