<?php

// This build script is intended for building the
// downloadable installer for the entire Torque SDK

// Include the global functions we use in general
include("scripts/globalFuncs.php");

// Include the functions we use for file manipulation
include("scripts/fileFuncs.php");

// Include the functions we use for generating and
// compiling the projects
include("scripts/compileFuncs.php");

// Include the settings for the various compilers available
include("scripts/compilerConfigs.php");

// What should the installer name be?
$PRODUCT_NAME = "Torque Game Builder";

// What version should we append to the end of our installer?
$VERSION = "1.7.5";

// The list of games to copy over
$GAMES = array(
               "AStarDemo",
               "BehaviorPlayground",
               "BehaviorShooter",
               "Blackjack",
               "MBFiles",
               "Reactor",
               "Solitaire",
               "TutorialBase"
               );

// Some default/useful variables
$MAC = false;
$COMPILER = 'VS2008';
$NSIS = "\"".$PROGRAMROOT."\\NSIS\\makensis\"";
$OUTPUT_FOLDER = "";

// Parse command line
foreach($argv as $v) {
   if(false !== strpos($v, '=')) {
      $parts = explode('=', $v);

      $value = 0;
      if (!strcasecmp("true", $parts[1]))
         $value = 1;

      if (!strcasecmp("output", $parts[0]))
         $OUTPUT_FOLDER = $parts[1];

      if (!strcasecmp("compiler", $parts[0]))
         $COMPILER = $parts[1];
   }
}

// Are we compiling for a Mac?
if ($COMPILER == "XCODE")
   $MAC = true;

// Get our compiler settings (defined in compilerConfig.h)
$SETTINGS = $COMPILERS[$COMPILER];

// Output the build settings
echo("\n*** Outputting our build settings:\n");
echo("***   o COMPILER              : ".$COMPILER."\n");
echo("***   o OUTPUT_FOLDER         : ".$OUTPUT_FOLDER."\n");
echo("***   o INCREDIBUILD AVAILABLE: ".$INCREDIBUILD_AVAILABLE."\n");
echo("***   o BUILD COMMAND         : ".$SETTINGS['buildcmd']."\n");
echo("***   o BUILD OPTIONS         : ".$SETTINGS['options']."\n");
echo("***   o BUILD DIRECTORY       : ".$SETTINGS['builddir']."\n");
echo("***   o ENVIRONMENT CONFIG    : ".$SETTINGS['envvar']."\n");

// Set up our excludes for this build
$EXCLUDES = array("builtLibs","Link",".svn","CVS","prefs.cs","config.cs","commonPrefs.cs",".ncb",".user",".ml",".obj",".ilk","console.log","staging");

// Exclude the platform/repo specific binaries
if ($MAC)
{
   $EXCLUDES[] = ".bat";
   $EXCLUDES[] = ".dll";
   $EXCLUDES[] = ".exe";
   $EXCLUDES[] = "rTorqueGameBuilder.exe";
   $EXCLUDES[] = "rTorque Game Builder.app";
   $EXCLUDES[] = "Torque Game Builder Debug.app";
   $EXCLUDES[] = "TGBGame Debug.app";
}
else
{
   $EXCLUDES[] = ".command";
   $EXCLUDES[] = "rTorqueGameBuilder.exe";
   $EXCLUDES[] = "rTorque Game Builder.app";
   $EXCLUDES[] = "TorqueGameBuilder_DEBUG.exe";
   $EXCLUDES[] = "TGBGame_DEBUG.exe";
   $EXCLUDES[] = "glu2d3d_DEBUG.dll";
   $EXCLUDES[] = "opengl2d3d_DEBUG.dll";
}

// Up a folder to root
chdir("..");
// Store the current working directory
$ROOTDIR =  getcwd();

// Compile the projects
echo "\n*** Compiling projects\n";

if ($MAC)
   CompileXCode("$ROOTDIR/engine/compilers/".$SETTINGS['builddir']." (Trial)", "Torque2D.xcodeproj");
else
   CompileSolution("$ROOTDIR\\engine\\compilers\\".$SETTINGS['builddir']." (Trial)", "T2D SDK.sln");

// Back to the root folder
chdir($ROOTDIR);

// Setup the staging area
echo "\n*** Setting up staging directory\n";
if (file_exists("./installer/binary/staging"))
   RemoveDir("./installer/binary/staging");

mkdir("./installer/binary/staging");

// Make sure we are in the root directory
chdir($ROOTDIR);

// Copy the files into the staging area
echo "\n*** Copying files to staging directory\n";

// Extract the archive to installer/documentation
echo "\n*** Extracting docs archive\n";

// Move down into the installer directory
chdir("installer");

if (file_exists("./documentation"))
   RemoveDir("./documentation");

if ($MAC)
	system("ditto -xk documentation.zip .");
else
	system("unzip documentation.zip");

// Move the extracted docs to staging
echo "\n*** Copying docs to staging\n";
MoveDir("./documentation", "binary/staging/documentation");

// Make sure we are in the root directory
chdir($ROOTDIR);

echo "\n      o Copying root files...\n";
//CopyFile("./tools/bitrock/installerAssets/EULA/t2d/EULA_Indie_Binary.txt","installer/binary/staging/EULA_Indie_Binary.txt", true);

// Copy the game files
echo "\n      o Copying game files...\n";
mkdir("./installer/binary/staging/games");

foreach ( $GAMES as $folder )
   CopyDir("./games/$folder", "./installer/binary/staging/games/$folder", true, true);

// Update any TGBGame.exe's that are in the games folders
CopyFile("./tgb/gameData/T2DProject/TGBGame.exe", "./installer/binary/staging/games/AStarDemo/TGBGame.exe", true);
CopyDir("./tgb/gameData/T2DProject/TGBGame.app", "./installer/binary/staging/games/AStarDemo/TGBGame.app", true);

// Copy the editor files
echo "\n      o Copying editor files...\n";
CopyDir("./tgb", "./installer/binary/staging/tgb", true, true);

// Copy the tools files
echo "\n      o Copying editor files...\n";
mkdir("./installer/binary/staging/tools");

CopyDir("./tools/UnChaos", "./installer/binary/staging/tools/UnChaos", true, true);

// Trial assets
echo "\n      o Copying trial assets into place...\n";
CopyDir("./trial/tgb/register", "./installer/binary/staging/tgb/register", true, true);
CopyFile("./trial/tgb/tools/levelEditor/gui/TGBInsider.ed.cs", "./installer/binary/staging/tgb/tools/levelEditor/gui/TGBInsider.ed.cs", true);
CopyFile("./trial/tgb/tools/levelEditor/gui/TGBInsider.ed.gui", "./installer/binary/staging/tgb/tools/levelEditor/gui/TGBInsider.ed.gui", true);
CopyFile("./trial/tgb/tools/levelEditor/gui/images/TGBCommunity.gif", "./installer/binary/staging/tgb/tools/levelEditor/gui/images/TGBCommunity.gif", true);

// Compile the tools scripts
echo "\n*** Compiling tools scripts\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

if ($MAC)
{
   chdir("./installer/binary/staging/tgb/Torque Game Builder.app/Contents/MacOS");

   exec("./\"Torque Game Builder\" main.toolscompile.cs");
}
else
{
   chdir(".\\installer\\binary\\staging\\tgb");

   exec("\"TorqueGameBuilder.exe\" main.toolscompile.cs");
}

echo "\n*** Removing stuff we don't want\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

// Trial files
RemoveFile("./installer/binary/staging/tgb/main.toolscompile.cs");

// Tools scripts
RemoveFilePattern("./installer/binary/staging/tgb/tools", ".cs", "main.cs", true);
RemoveFilePattern("./installer/binary/staging/tgb/tools", ".gui", "", true);
RemoveFilePattern("./installer/binary/staging/tgb/tools", "main.cs.dso", "", true);

echo "\n*** Building installer\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

$install_name = $PRODUCT_NAME." ".$VERSION;

if ($MAC)
{
   $install_name = str_replace(" ", "", $PRODUCT_NAME)."-Binary";
   $install_name = $install_name."-Mac.zip";
}
else
   $install_name = $install_name."-Win.exe";

if ($MAC)
   system("ditto -ck --sequesterRsrc ./installer/binary/staging \"./installer/binary/".$install_name."\"");
else
   RunCmd($NSIS." /DPRODUCT_NAME=\"".$PRODUCT_NAME."\" /DVERSION=\"".$VERSION."\" \"./installer/binary/win32/installer.nsi\"", $ROOTDIR);

echo "\n*** Done building installer\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

if ($OUTPUT_FOLDER != "")
{
   echo "\n*** Copying installer to output folder\n";

   if (!is_dir($OUTPUT_FOLDER))
      mkdir($OUTPUT_FOLDER);

   $dst = $OUTPUT_FOLDER."/".$install_name;

   if (is_file($dst))
      unlink($dst);

   CopyFile("./installer/binary/".$install_name, $dst, true);
}

?>
