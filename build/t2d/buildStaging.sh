#!/bin/sh

pushd ../
source common.sh
popd

DEMOLIST="$1"

# Some validation. We need to have the documentation, trial, and demos
# directories along with those that already exist in the repo.
if [ ! -d "$DOCUMENTATIONDIR" ]; then
   echo "Error: documentation directory does not exist!"
   exit 1
fi

if [ ! -d "$DEMODIR" ]; then
   echo "Error: demos directory does not exist!"
   exit 1
fi

if [ ! -d "$TRIALDIR" ]; then
   echo "Error: trial directory does not exist!"
   exit 1
fi

# Cleans up files unnecessary for the current platform.
cleanupPlatform ()
{
   if [ ! "$PLAT" = "win32" ]; then
      rm cleandso.bat
      rm glu2d3d.dll
      rm OpenAL32.dll
      rm opengl2d3d.dll
      rm unicows.dll
      rm TorqueGameBuilder.exe
   elif [ ! "$PLAT" = "macosx" ]; then
      rm CleanDSO.command
      rm -rf "Torque Game Builder.app"
   elif [ ! "$PLAT" = "linux" ]; then
      rm TorqueGameBuilder.bin
   fi
}

echo "Creating staging directory at $STAGINGDIR..."
rm -rf "$STAGINGDIR"
mkdir -p "$STAGINGDIR"
pushd "$STAGINGDIR"

echo "Generating projects with t2d.conf..."
pushd "$TOOLSDIR/projectGenerator"
/bin/bash ./generateProjects.sh ./config/fullSourceTree.conf
popd

# Exporting the folders individually since we don't necessarily want the
# whole source tree. The source files are copied manually.
echo "Exporting engine..."
mkdir -p engine
cd engine
svn export -q --force "$ENGINEDIR/bin"
svn export -q --force "$ENGINEDIR/compilers"
svn export -q --force "$ENGINEDIR/lib"

mkdir -p source
cd source
SOURCEDIR=`pwd`
cd ..

# On Windows, the manifest file will have CRLF line endings, which xargs
# doesn't like.
MANIFEST="$TOOLSDIR/projectGenerator/manifest/buildManifest_SourceTree.txt"
echo "Fixing line endings in manifest $MANIFEST..."
if [ "$PLAT" = "win32" ]; then
   dos2unix "$MANIFEST"
elif [ "$PLAT" = "macosx" ]; then
   flip -u "$MANIFEST"
fi

pushd "$ENGINEDIR/source"
echo "Copying source files in $MANIFEST..."
# Strip everything before the double slash in the manifest, and process the
# files it lists: first create the directories, then copy the files to the
# source output directory.
cat "$MANIFEST" | awk '{print substr($1,index($1,"//")+2,256)}' | xargs -n1 -I % dirname "$SOURCEDIR/%" | uniq | xargs -n1 mkdir -p
cat "$MANIFEST" | awk '{print substr($1,index($1,"//")+2,256)}' | xargs -n1 -I % cp -i % "$SOURCEDIR/%"
popd

cd ..

mkdir -p games
cd games


# In order to get this from the build system intact, we can't have any spaces
# in the variable saved build-system side.
DEMOLIST="${DEMOLIST//:/ }"
   
# Export the demos in the demo list. First search the demo directory, which
# is from the DocsAndDemos repo, then search the games directory, which is
# in the trunk repo.
for dir in $DEMOLIST
do
   if [ -d "$DEMODIR/$dir" ]; then
      echo "Exporting demo $dir from $DEMODIR..."
      svn export -q --force "$DEMODIR/$dir"
   elif [ -d "$GAMESDIR/$dir" ]; then
      echo "Exporting demo $dir from $GAMESDIR..."
      svn export -q --force "$GAMESDIR/$dir"
   else
      echo "Warning: Unable to find demo $dir."
   fi
done

cd ..

mkdir -p tgb
cd tgb

# Pro Version.

echo "Exporting TGB for pro version..."
svn export -q --force "$TGBDIR"
mv tgb pro
cd pro
if [ "$PLAT" = "win32" ]; then
   TGBPATH=`getPlatformPath "$STAGINGDIR/tgb/pro"`
   
   echo "Packing TorqueGameBuilder.exe with UPX..."
   $UPX -9kv "$TGBPATH/TorqueGameBuilder.exe"
   rm "$TGBPATH/TorqueGameBuilder.ex~"
   
   echo "Packing TGBGame.exe with UPX..."
   $UPX -9kv "$TGBPATH/gameData/T2DProject/TGBGame.exe"
   rm "$TGBPATH/gameData/T2DProject/TGBGame.ex~"
fi

cleanupPlatform
cd ..

# Trial Version. For Indie Binary licenses and the trial.

echo "Exporting TGB for trial version..."
svn export -q --force "$TGBDIR"
echo "Exporting TGB trial assets..."
svn export -q --force "$TRIALDIR/tgb"
# replace regular tgb app with trial tgb app
rm -rf "./tgb/$TORQUEAPP"
cp -R "$TRIALDIR/tgb/$TORQUEAPP" ./tgb

# force copy windows and mac trial games.
# we do this because svn export skips files that are not managed by svn.
echo "forcing copy of Game app"
rm -rf ./tgb/gameData/T2DProject/*.app
rm -rf ./tgb/gameData/T2DProject/*.exe
cp -R "$TRIALDIR"/tgb/gameData/T2DProject/*.app ./tgb/gameData/T2DProject/
cp -R "$TRIALDIR"/tgb/gameData/T2DProject/*.exe ./tgb/gameData/T2DProject/

mv tgb trial
cd trial
if [ "$PLAT" = "win32" ]; then
   TGBPATH=`getPlatformPath "$STAGINGDIR/tgb/trial"`
   
   echo "Packing TorqueGameBuilder.exe with UPX..."
   $UPX -9kv "$TGBPATH/TorqueGameBuilder.exe"
   rm "$TGBPATH/TorqueGameBuilder.ex~"
   
   echo "Packing TGBGame.exe with UPX..."
   $UPX -9kv "$TGBPATH/gameData/T2DProject/TGBGame.exe"
   rm "$TGBPATH/gameData/T2DProject/TGBGame.ex~"
fi

cleanupPlatform

echo "Compiling editor script files..."
ls
cp "$BUILDDIR/t2d/main.toolscompile.cs" ./
"./$TORQUEEXEC" main.toolsCompile.cs
rm main.toolscompile.cs

find . -iname "*.ed.cs" -print0  | xargs -0 rm -rf
find . -iname "*.ed.gui" -print0  | xargs -0 rm -rf
cd ..

# Binary Version. For Commercial Binary licenses.

echo "Exporting TGB for binary version..."
svn export -q --force "$TGBDIR"
mv tgb binary
cd binary
if [ "$PLAT" = "win32" ]; then
   TGBPATH=`getPlatformPath "$STAGINGDIR/tgb/binary"`
   
   echo "Packing TorqueGameBuilder.exe with UPX..."
   $UPX -9kv "$TGBPATH/TorqueGameBuilder.exe"
   rm "$TGBPATH/TorqueGameBuilder.ex~"
   
   echo "Packing TGBGame.exe with UPX..."
   $UPX -9kv "$TGBPATH/gameData/T2DProject/TGBGame.exe"
   rm "$TGBPATH/gameData/T2DProject/TGBGame.ex~"
fi

cleanupPlatform

echo "Compiling editor script files..."
cp "$BUILDDIR/t2d/main.toolscompile.cs" ./
"./$TORQUEEXEC" main.toolsCompile.cs
rm main.toolscompile.cs

find . -iname "*.ed.cs" -print0  | xargs -0 rm -rf
find . -iname "*.ed.gui" -print0  | xargs -0 rm -rf
cd ..
cd ..

mkdir -p tools
cd tools

# Exporting these individually so we don't ever accidentally ship something we
# shouldn't be (Bitrock).
echo "Exporting docBuilder..."
svn export -q --force "$TOOLSDIR/docBuilder"

echo "Exporting docGenerator..."
svn export -q --force "$TOOLSDIR/docGenerator"

echo "Exporting doxygen..."
svn export -q --force "$TOOLSDIR/doxygen"
if [ ! "$PLAT" = "win32" ]; then
   # remove the doxygen binaries to save space in non win32 distros
   echo "  Removing the doxygen binaries to save space in non win32 distros..."
   cd doxygen
   rm *.exe
   cd ..
fi

echo "Exporting projectGenerator..."
svn export -q --force "$TOOLSDIR/projectGenerator"

if [ "$PLAT" = "win32" ]; then
   echo "Exporting UPX..."
   svn export -q --force "$TOOLSDIR/upx"

   echo "Exporting PHP..."
   svn export -q --force "$TOOLSDIR/php"
fi
cd ..

echo "Exporting build scripts..."
svn export -q --force "$BUILDDIR"

echo "Exporting documentation..."
# svn export -q --force --ignore-externals "$DOCUMENTATIONDIR"
mkdir -p documentation
cd documentation
cp -r "$DOCUMENTATIONDIR/Output/TGB/documentation/content" ./
cp "$DOCUMENTATIONDIR/Output/TGB/documentation/Documentation Overview.html" ./
cd ..

popd
