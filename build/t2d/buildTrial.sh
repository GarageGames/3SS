#!/bin/sh

pushd ../
source common.sh
popd

# Make sure the trial directory exists
if [ ! -d "$TRIALDIR" ]; then
   echo "   Error: trial directory does not exist!"
   exit 1
fi

# Generate the trial project files.
echo "Generating trial projects with t2d.conf..."
pushd "$TOOLSDIR/projectGenerator"
 if [ "$PLAT" = "win32" ]; then
    ./generateProjects.sh "$TRIALDIR/tools/projectGenerator/config/t2d.conf"
 else
   echo "------------------------------------------------------------------------------"
   echo "NOTE: Project generator only creates windows projects at the moment."
   echo "      The project for this platform has been manually created."
   echo "------------------------------------------------------------------------------"
 fi
popd

if [ "$PLAT" = "win32" ]; then
   echo "Compiling trial with Visual Studio 2005..."
   buildVC8.bat
elif [ "$PLAT" = "macosx" ]; then
   echo "copying trial xcode file"
   svn export --force "../../trial/engine/compilers/Xcode/trial.xcodeproj" "../../engine/compilers/Xcode/trial.xcodeproj"
   xcodebuild -project "../../engine/compilers/Xcode/trial.xcodeproj" -target TrialRelease build 
   echo "removing trial xcode file"
   rm -rf "../../engine/compilers/Xcode/trial.xcodeproj"
elif [ "$PLAT" = "linux" ]; then
   echo "Error: Compiling on Linux isn't implemented yet!"
fi
