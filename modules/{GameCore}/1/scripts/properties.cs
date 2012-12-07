//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$iOS::constant::iPhone = 0;
$iOS::constant::iPad = 1;
$iOS::constant::iPhone4 = 2;
$iOS::constant::iPad3 = 3;

$iOS::constant::Landscape = 0;
$iOS::constant::Portrait = 1;
$iOS::constant::ResolutionFull = 0;
$iOS::constant::ResolutionSmall = 1;

$iOS::constant::iPhoneWidth = 480;
$iOS::constant::iPhoneHeight = 320;

$iOS::constant::iPhone4Width = 960;
$iOS::constant::iPhone4Height = 640;

$iOS::constant::iPadWidth = 1024;
$iOS::constant::iPadHeight = 768;

$iOS::constant::NewiPadWidth = 2048;
$iOS::constant::NewiPadHeight = 1536;

$iOS::constant::OrientationUnknown				= 0;
$iOS::constant::OrientationLandscapeLeft		= 1;
$iOS::constant::OrientationLandscapeRight		= 2;
$iOS::constant::OrientationPortrait				= 3;
$iOS::constant::OrientationPortraitUpsideDown	= 4;

// Load config data
function _loadGameConfigurationData()
{
    // Get the root directory for this game
    %rootDirectory = getMainDotCsDir();
    
    // Append the location of the prefs.cs file for the game template (temporary)
    %prefsScript = %rootDirectory @ "/game/template/scripts/prefs.cs";
    
    // If the prefs.cs file exists, execute it so we can get things like default resolution
    // and whether the game should run in fullscreen mode
    if (isFile(%prefsScript))
        exec(%prefsScript);
}