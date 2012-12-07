#!/bin/sh

# $1 Package root directory
# $2 Package name
# $3 Version
# $4 PKG output directory

# This script creates a package for the 3StepStudio installer.

BUILT_PRODUCTS_DIR="$1"
PACKAGE_NAME="$2"
VERSION="$3"
OUTPUT_DIR="$4"

mkdir -p "$OUTPUT_DIR"

pkgbuild --root "$BUILT_PRODUCTS_DIR" \
    --identifier com.garagegames.threestepstudio.pkg \
    --install-location /Applications/3StepStudioAlpha \
    --version "$VERSION" \
    "$PACKAGE_NAME-tmp1.pkg"

productbuild --package "$PACKAGE_NAME-tmp1.pkg" \
    "$OUTPUT_DIR/$PACKAGE_NAME$VERSION.pkg"
    
    