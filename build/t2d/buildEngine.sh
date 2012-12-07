#!/bin/sh

pushd ../
source common.sh
popd

echo "Generating projects with t2d.conf..."
pushd "$TOOLSDIR/projectGenerator"
 if [ "$PLAT" = "win32" ]; then
   ./generateProjects.sh ./config/t2d.conf
 else
   echo "------------------------------------------------------------------------------"
   echo "NOTE: Project generator only creates windows projects at the moment."
   echo "      The project for this platform has been manually created."
   echo "------------------------------------------------------------------------------"
 fi
popd

if [ "$PLAT" = "win32" ]; then
   echo "Compiling engine with Visual Studio 2005..."
   buildVC8.bat
elif [ "$PLAT" = "macosx" ]; then
   xcodebuild -project "../../engine/compilers/Xcode/Torque2d.xcodeproj" -target "Torque Game Builder" build 
   xcodebuild -project "../../engine/compilers/Xcode/Torque2d.xcodeproj" -target "TGBGame" build 
elif [ "$PLAT" = "linux" ]; then
   echo "Error: Compiling on Linux isn't implemented yet!"
fi
