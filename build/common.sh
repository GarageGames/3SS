# Common variables and function for the various build scripts. This file
# should be sourced into any file that uses it like so:
# 
# pushd [path to this file]
# source common.sh
# popd
# 
# The push/pop dir is important since various paths are determined based
# on the location of this file.

# Automagically determine which platform we are on.
if [ -z $OS ]; then
   OS=`uname -s`
fi

case $OS in
   "Darwin" )     PLAT=macosx ;;
   "Windows_NT" ) PLAT=win32  ;;
   * )            PLAT=linux  ;;
esac

# Save off some directories.
BUILDDIR="`pwd`"
BASEDIR=$BUILDDIR/..
TOOLSDIR=$BASEDIR/tools
ENGINEDIR=$BASEDIR/engine
STAGINGDIR=$BUILDDIR/staging
TGBDIR=$BASEDIR/tgb
DEMODIR=$BASEDIR/demos
GAMESDIR=$BASEDIR/games
TRIALDIR=$BASEDIR/trial
DOCUMENTATIONDIR=$BASEDIR/documentation

# Various utilities that are available.
PHP=php
BITROCK=$TOOLSDIR/bitrock/bin/builder
DOXYGEN=doxygen
UPX=$TOOLSDIR/upx/upx

case $PLAT in
   "win32" )
      PHP=$TOOLSDIR/php/php
      DOXYGEN=./doxygen
      TORQUEAPP=TorqueGameBuilder.exe
      TORQUEEXEC=TorqueGameBuilder.exe
      ;;
   "macosx" )
      # Apple still ships php 4. 
      # Install the php 5 package from http://www.entropy.ch/software/macosx/php/ .
      PHP="/usr/bin/php"
         
      BITROCK="/Applications/BitRockMac/bin/Builder.app/Contents/MacOS/installbuilder.sh"
      DOXYGEN="/Applications/Doxygen"

      UPX="$TOOLSDIR/upx/upx-stub"
      TORQUEEXEC=Torque\ Game\ Builder.app/Contents/MacOS/Torque\ Game\ Builder
      TORQUEAPP=Torque\ Game\ Builder.app
      ;;
esac

# Converts a path to the correct path on the current platform. This is useful
# for getting the filename of PHP scripts, for instance, since PHP will not
# handle cygwin paths on Windows. Use it like this:
# 
# PATH=`getPlatformPath($PATH)`
getPlatformPath ()
{
   local platformPath=$1
   
   if [ "$PLAT" = "win32" ]; then
      platformPath=`cygpath -w $1`
   fi
   
   echo $platformPath
}
