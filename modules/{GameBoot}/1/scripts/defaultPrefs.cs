//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// iOS Constants
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

/// Game
$Game::CompanyName         = "Independent";
$Game::ProductName         = "Test Template";
$Game::DefaultScene        = "data/levels/mainMenu.t2d";

/// iOS
$pref::iOS::ScreenOrientation   = $iOS::constant::Landscape;
$pref::iOS::ScreenDepth		    = 32;
$pref::iOS::UseGameKit          = 0;
$pref::iOS::UseMusic            = 0;
$pref::iOS::UseMoviePlayer      = 0;
$pref::iOS::UseAutoRotate       = 0;   
$pref::iOS::EnableOrientationRotation      = 1;   
$pref::iOS::StatusBarType       = 0;

/// Audio
$pref::Audio::driver = "OpenAL";
$pref::Audio::forceMaxDistanceUpdate = 0;
$pref::Audio::environmentEnabled = 0;
$pref::Audio::masterVolume   = 1.0;
$pref::Audio::channelVolume1 = 1.0;
$pref::Audio::channelVolume2 = 1.0;
$pref::Audio::channelVolume3 = 1.0;
$pref::Audio::sfxVolume = 1.0;
$pref::Audio::musicVolume = 1.0;

/// T2D
$pref::T2D::particleEngineQuantityScale = 1.0;
$pref::T2D::warnFileDeprecated = 1;
$pref::T2D::warnSceneOccupancy = 1;

/// Video
$pref::Video::appliedPref = 0;
$pref::Video::disableVerticalSync = 1;
$pref::Video::displayDevice = "OpenGL";
$pref::Video::preferOpenGL = 1;
$pref::Video::fullScreen = 0;
$pref::OpenGL::gammaCorrection = 0.5;
$pref::Video::defaultResolution = "1024 768";