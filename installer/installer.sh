#! /bin/sh
# $1 REPO_PATH Path to repo: 												/Users/buildAgent/TeamCityData/MMGD/Release/ContinuousMac
# $2 PROJECT_NAME Name of staging and DMG (no spaces): 						3StepStudioAlpha						
# $5 OUTPUT_PATH Output path												/Users/buildAgent/TeamCityData/MMGD/Release/DMGInstallerOutput
# $6 BUILD_NUMBER The build number passed by TeamCity

# STAGING

# Export the repo, then clear permissions just in case
cd "$1"

# Delete development files
rm -rf "Documentation"
rm -rf "tools"
rm -rf "installer"
rm -rf "MyProjects"
rm -rf "DemoArt"
rm -rf "build"

# Remove intermediate files
find . -name '*.pbxuser' -type f -delete
find . -name '*.mode1v3' -type f -delete
find . -name '*.log' -type f -delete

#Remove the .ed.cs and .ed.gui files
find . -name '*.ed.cs' -type f -delete
find . -name '*.ed.gui' -type f -delete

#Remove .bat and .command
find . -name '*.bat' -type f -delete
find . -name '*.command' -type f -delete

# Clean out unneeded *.cs

#Move into tools, remove all .cs, jump back out
#cd tools
#
#find . -name '*.cs' -type f -delete
#
#cd ..

#Move into Blackjack template game/scripts/behaviors, remove all .cs, jump up a level
#cd templates/BlackJack/game/scripts/behaviors

#find . -name '*.cs' -type f -delete

#cd ..

# Final cleanup of blackjack game/scripts
# rm -f audioDescriptions.cs
# rm -f game.cs

# cd ..

# cd gui

# find . -name '*.cs' -type f -delete

# cd ..

# cd ..
# cd ..

#Move into TowerDefense template game/scripts/behaviors, remove all .cs, jump up a level
# cd TowerDefense/game/scripts/behaviors

# find . -name '*.cs' -type f -delete

# cd ..

# # Move into system
# cd system

# find . -name '*.cs' -type f -delete

# cd ..

# # Move into ai
# cd ai

# find . -name '*.cs' -type f -delete

# cd ..

# # Move into game
# cd game

# find . -name '*.cs' -type f -delete

# cd ..

# # Move into gui
# cd gui

# find . -name '*.cs' -type f -delete

# cd ..

# # Final cleanup of towerdefense game/scripts
# rm -f audioDescriptions.cs
# rm -f game.cs

# cd ..

# cd gui

# find . -name '*.cs' -type f -delete

# cd ..

# cd ..
# cd ..

# cd ..

# cd ..
 
# Delete build directory, if it exists
rm -rf `find . -name 'build'`

# Delete Visual Studio, Xcode, and Xcode_iPhone folders if they exist
rm -rf `find . -name 'VisualStudio2008'`
rm -rf `find . -name 'VisualStudio2010'`
rm -rf `find . -name 'XCode'`
rm -rf `find . -name 'XCode_iPhone'`

#Delete resources
rm -rf `find . -name 'resources'`

#Delete Torsion
rm -rf `find . -name 'Torsion'`

rm -f main.toolscompile.cs
rm -f main.toolscompile.cs.dso
# Remove the compilers
cd engine
rm -rf "compilers"

# Remove all the source files, except headers
cd source
find . -name '*.cc' -type f -delete
find . -name '*.cpp' -type f -delete
find . -name '*.mm' -type f -delete

# PACKAGING STEP
#cd "$1"

# Create a directory for the dmg
#mkdir -p "$5/$2"

#echo "DMG commandline argument is $2/installer/dmg/ThreeStepStudio.dmgCanvas" "$5/$2$6.dmg"

#/usr/bin/dmgcanvas "$2/installer/dmg/ThreeStepStudio.dmgCanvas" "$5/$2$6.dmg"
