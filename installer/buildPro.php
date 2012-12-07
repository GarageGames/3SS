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
$PRODUCT_NAME = "iTorque2D";

// What version should we append to the end of our installer?
$VERSION = "1_5_Preview2";

// The list of games to copy over
$GAMES = array(
               "MusicPlayerExample",
               "Aquarium",
			   "GameCenterTest",
			   "RainyDay"
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
$EXCLUDES = array("builtLibs","Link",".svn","CVS","prefs.cs","config.cs","commonPrefs.cs",".ncb",".user",".dso",".edso",".ml",".obj",".ilk","_DEBUG.","_debug.","console.log","staging");

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
}

// Up a folder to root
chdir("..");
// Store the current working directory
$ROOTDIR =  getcwd();

// Compile the projects
echo "\n*** Compiling projects\n";

if ($MAC)
   CompileXCode("$ROOTDIR/engine/compilers/".$SETTINGS['builddir'], "Torque2D.xcodeproj");
else
{
  echo("TEST DIR");
  echo("$ROOTDIR\\engine\\compilers\\".$SETTINGS['builddir']);
  
  CompileSolution("$ROOTDIR\\engine\\compilers\\".$SETTINGS['builddir'], "T2D SDK.sln");
  foreach ( $GAMES as $folder )
  	CompileSolution("$ROOTDIR\\MyProjects\\".$folder."\\buildFiles\\VisualStudio2008", "iTorque2DGame.sln");
}

// Setup the staging area
echo "\n*** Setting up staging directory\n";
if (file_exists("./installer/pro/staging"))
   RemoveDir("./installer/pro/staging");

mkdir("./installer/pro/staging");

// Copy the files into the staging area
echo "\n*** Copying files to staging directory\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

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
MoveDir("./documentation", "pro/staging/documentation");

// Make sure we are in the root directory
chdir($ROOTDIR);

echo "\n      o Copying root files...\n";
CopyFile("./tools/bitrock/installerAssets/EULA/t2d/END_USER_LICENSE_AGREEMENT.pdf","installer/pro/staging/END_USER_LICENSE_AGREEMENT.pdf", true);
//CopyFile("./END_USER_LICENSE_AGREEMENT.pdf","pro/staging/END_USER_LICENSE_AGREEMENT.pdf", true);

// Copy the engine files
echo "\n      o Copying engine files...\n";
CopyDir("./engine", "./installer/pro/staging/engine", true, true);

// Copy the game files
echo "\n      o Copying sample project files...\n";
mkdir("./installer/pro/staging/MyProjects");

foreach ( $GAMES as $folder )
   CopyDir("./MyProjects/$folder", "./installer/pro/staging/MyProjects/$folder", true, true);

CopyFile("./MyProjects/Aquarium/projectFiles/iTorque2DGame.exe", "./tgb/templates/commonFiles/iTorque2DGame.exe", true);

// Update any TGBGame.exe's that are in the games folders
//CopyFile("./tgb/gameData/T2DProject/TGBGame.exe", "./installer/pro/staging/games/AStarDemo/TGBGame.exe", true);
//CopyDir("./tgb/gameData/T2DProject/TGBGame.app", "./installer/pro/staging/games/AStarDemo/TGBGame.app", true);

// Copy the editor files
echo "\n      o Copying editor files...\n";
CopyDir("./tgb", "./installer/pro/staging/tgb", true, true);

// Copy the tools files
echo "\n      o Copying editor files...\n";
mkdir("./installer/pro/staging/tools");

CopyDir("./tools/UnChaos", "./installer/pro/staging/tools/UnChaos", true, true);

echo "\n*** Removing stuff we don't want\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

// Trial files
RemoveDir("./installer/pro/staging/engine/compilers/VisualStudio 2008 (Trial)");
RemoveDir("./installer/pro/staging/engine/compilers/Xcode (Trial)");
RemoveFile("./installer/pro/staging/tgb/main.toolscompile.cs");

echo "\n*** Building installer\n";

// Make sure we are in the root directory
chdir($ROOTDIR);

$install_name = $PRODUCT_NAME."_".$VERSION;

if ($MAC)
{
   $install_name = str_replace(" ", "", $PRODUCT_NAME)."-Pro";
   $install_name = $install_name."-Mac.zip";
}
else
   $install_name = $install_name."_Win.exe";

if ($MAC)
   system("ditto -ck --sequesterRsrc ./installer/pro/staging \"./installer/pro/".$install_name."\"");
else
   RunCmd($NSIS." /DPRODUCT_NAME=\"".$PRODUCT_NAME."\" /DVERSION=\"".$VERSION."\" \"./installer/pro/win32/installer.nsi\"", $ROOTDIR);

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

   CopyFile("./installer/pro/".$install_name, $dst, true);
}

?>
