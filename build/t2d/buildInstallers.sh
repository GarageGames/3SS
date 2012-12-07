#!/bin/bash

pushd ../
source common.sh
popd

# Make sure the staging directory exists
if [ ! -d "$STAGINGDIR" ]; then
   echo "Error: staging directory does not exist!"
   exit 1
fi

echo "Generating XML Installer Files"

MACINSTALLER=""
if [ "$PLAT" = "macosx" ]; then
   MACINSTALLER="--mac"
fi

PROJECTGENSCRIPT=`getPlatformPath "$TOOLSDIR/bitrock/installerXMLGenerator/generateBitrockProject.php"`
"$PHP" "$PROJECTGENSCRIPT" --indie --binary --version "$1" $MACINSTALLER
"$PHP" "$PROJECTGENSCRIPT" --indie --pro --version "$1" $MACINSTALLER
"$PHP" "$PROJECTGENSCRIPT" --commercial --binary --version "$1" $MACINSTALLER
"$PHP" "$PROJECTGENSCRIPT" --commercial --pro --version "$1" $MACINSTALLER
# removing the downloading/gettorquegamebuilder "$PHP" "$PROJECTGENSCRIPT" --downloading --version "$1" $MACINSTALLER

PROJECTDIR=`getPlatformPath "$TOOLSDIR/bitrock/projects/t2d"`

echo "Building Indy Binary/Trial Installer"
mv "$STAGINGDIR/tgb/trial" "$STAGINGDIR/tgb/tgb"
"$BITROCK" build "$PROJECTDIR/Indie_Binary.xml"
mv "$STAGINGDIR/tgb/tgb" "$STAGINGDIR/tgb/trial"

echo "Building Commercial Binary Installer"
mv "$STAGINGDIR/tgb/binary" "$STAGINGDIR/tgb/tgb"
"$BITROCK" build "$PROJECTDIR/Commercial_Binary.xml"
mv "$STAGINGDIR/tgb/tgb" "$STAGINGDIR/tgb/binary"

echo "Building Indy/Commercial Pro Installers"
mv "$STAGINGDIR/tgb/pro" "$STAGINGDIR/tgb/tgb"
"$BITROCK" build "$PROJECTDIR/Indie_Pro.xml"
"$BITROCK" build "$PROJECTDIR/Commercial_Pro.xml"
mv "$STAGINGDIR/tgb/tgb" "$STAGINGDIR/tgb/pro"

# removing the downloading/gettorquegamebuilder echo "Building Downloading Installer"
# removing the downloading/gettorquegamebuilder "$BITROCK" build "$PROJECTDIR/Downloading.xml"

OUTPUTDIR="$TOOLSDIR/bitrock/output"
if [ $PLAT = "macosx" ]; then
   OUTPUTDIR="/Applications/BitRockMac/output"
fi

#clear the output directory
echo -n "Clearing the output directory $2/*... "
rm -rf "$2"/*
echo "done."

pwd
mv "$OUTPUTDIR"/* "$2"
