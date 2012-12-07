#!/bin/bash

pushd ../
source common.sh
popd

VERSION=${1// /_}
VERSION=${VERSION//./}

BINARYEULA=EULA_Indie_Binary.txt
if [ ! -z "$3" ]; then
   BINARYEULA="$3"
fi

PROEULA=EULA_Indie_Pro.txt
if [ ! -z "$4" ]; then
   PROEULA="$4"
fi

# Make sure the staging directory exists
if [ ! -d "$STAGINGDIR" ]; then
   echo "Error: staging directory does not exist!"
   exit 1
fi

#get the full path to the output dir
pushd $2
OUTPUTDIR=`pwd`
popd

# create a zip of the Indy Binary version
echo "-------------------------------------------------------------------"
echo "Building Binary version zip... "
pushd "$STAGINGDIR"
zip -rqy9 "$OUTPUTDIR/TorqueGameBuilder_$VERSION.zip" documentation games tools

cd tgb
mv binary tgb
zip -rgqy9 "$OUTPUTDIR/TorqueGameBuilder_$VERSION.zip" tgb
mv tgb binary
cd ..

pushd $TOOLSDIR/bitrock/installerAssets/EULA/t2d
zip -gqy9 "$OUTPUTDIR/TorqueGameBuilder_$VERSION.zip" "$BINARYEULA"
popd
echo "done."

# create a zip of the Pro version
echo "-------------------------------------------------------------------"
echo "Building Pro version zip... "

zip -rqy9 "$OUTPUTDIR/TorqueGameBuilder_${VERSION}_Pro.zip" documentation games tools engine build

cd tgb
mv pro tgb
zip -rgqy9 "$OUTPUTDIR/TorqueGameBuilder_${VERSION}_Pro.zip" tgb
mv tgb pro
cd ..

pushd "$TOOLSDIR/bitrock/installerAssets/EULA/t2d"
zip -gqy9 "$OUTPUTDIR/TorqueGameBuilder_${VERSION}_Pro.zip" "$PROEULA"
popd
echo "done."
echo "-------------------------------------------------------------------"

popd
