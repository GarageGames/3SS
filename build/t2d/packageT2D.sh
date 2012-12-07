#!/bin/sh

pushd ../
source common.sh
popd

VERSION=0
OUTPUTDIR="$BUILDDIR/output"
DEMOLIST=""
BUILDINSTALLER=1
BUILDZIP=1
MACTRIALNAME="TorqueGameBuilder_152.app"


# Parse the command line
echo "Parsing command line arguments..."
while [ $# -ge 1 ]
do
   case $1 in
      -v | --version )
         VERSION=$2
         VERSION=${VERSION%%_*}
         shift
         ;;
      -o | --outputDir ) 
         OUTPUTDIR=$2
         shift
         ;;
      -d | --demoList )
         DEMOLIST=$2
         shift
         ;;
      --installers )
         BUILDINSTALLER=1
         ;;
      --noinstallers )
         BUILDINSTALLER=0
         ;;
      --zips )
         BUILDZIP=1
         ;;
      --nozips )
         BUILDZIP=0
         ;;
   esac
   shift
done

# Make the output directory an absolute path.
echo "Resolving output directory $OUTPUTDIR..."
rm -rf "$OUTPUTDIR"
mkdir -p "$OUTPUTDIR"
pushd "$OUTPUTDIR"
OUTPUTDIR=`pwd`
popd
   
echo "   Version: $VERSION"
echo "   Output Directory: $OUTPUTDIR"
echo "   Demo List: $DEMOLIST"
echo "   Build Installers: $BUILDINSTALLER"
echo "   Build Zips: $BUILDZIP"
echo ""

echo "Building Trial..."
./buildTrial.sh
echo "Trial build complete."

echo "Building Engine..."
./buildEngine.sh
echo "Engine build complete."

echo "Building Documentation..."
./buildDocumentation.sh
echo "Documentation build complete."

echo "Building Staging Area..."
./buildStaging.sh "$DEMOLIST"
echo "Staging area build complete."

if [ $BUILDINSTALLER = 1 ]; then
   echo "Building Installers..."
   ./buildInstallers.sh "$VERSION" "$OUTPUTDIR"
   echo "Installers build complete."
fi

if [ $BUILDZIP = 1 ]; then
   echo "Building Zips..."
   ./buildZips.sh "$VERSION" "$OUTPUTDIR"
   echo "Zips build complete."
fi

if [ $PLAT = "macosx" ]; then
   echo "Zipping Mac installers... "
   cd "$OUTPUTDIR"
   tar -zcf TorqueGameBuilderTrial.app.tgz $MACTRIALNAME

   if [ $? != 0 ]; then
      echo "ERROR: Could not find the trial binary $MACTRIALNAME, so could not create the tarball for the downloadable installer."
   fi

   find . -name "*.app" -print0 | xargs -0 -n 1 -I % zip -qmTry9 %.zip %
   echo "Done zipping Mac installers."
fi
 