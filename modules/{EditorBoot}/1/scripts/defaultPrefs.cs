//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Network
$pref::Net::LagThreshold = 400;
$pref::Net::Port = 28000;

// Input
$pref::Input::LinkMouseSensitivity = 1;
$pref::Input::MouseEnabled = 0;
$pref::Input::JoystickEnabled = 0;
$pref::Input::KeyboardTurnSpeed = 0.1;

// Audio
$pref::Audio::driver = "OpenAL";
$pref::Audio::forceMaxDistanceUpdate = 0;
$pref::Audio::environmentEnabled = 0;
$pref::Audio::masterVolume   = 1.0;
$pref::Audio::channelVolume1 = 1.0;
$pref::Audio::channelVolume2 = 1.0;
$pref::Audio::channelVolume3 = 1.0;

// TGB
$pref::T2D::particleEngineQuantityScale = 1.0;
$pref::T2D::warnFileDeprecated = 1;
$pref::T2D::warnSceneOccupancy = 1;
$pref::T2D::fullscreen = 0;
$pref::loadLastProject = 0;
$pref::loadLastLevel = 1;

// iOS constants
$iOS::constant::iPhone = 0;
$iOS::constant::iPad = 1;
$iOS::constant::iPhone4 = 2;
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

// iOS settings
$pref::iOS::DeviceType = $iOS::constant::iPad;
$pref::iOS::ScreenOrientation = $iOS::constant::Landscape;
$pref::iOS::StatusBarType = 0;
$pref::iOS::EnableOrientationRotation  = 1;
$pref::iOS::EnableOtherOrientationRotation  = 0;

$preLaunchDeviceType = $pref::iOS::DeviceType;
$preLaunchFullsScreen = $pref::T2D::fullscreen;
$preLaunchScreenResolution = $pref::iOS::ScreenResolution;

// Server
$pref::Server::Name = "TGB Server";
$pref::Player::Name = "TGB Player";
$pref::Server::port = 28000;
$pref::Server::MaxPlayers = 32;
$pref::Server::RegionMask = 2;
$pref::Net::RegionMask = 2;
$pref::Master0 = "2:master.garagegames.com:28002";

// Video
$pref::ts::detailAdjust = 0.45;
$pref::Video::appliedPref = 0;
$pref::Video::disableVerticalSync = 1;
$pref::Video::screenShotFormat = "PNG";
$pref::OpenGL::gammaCorrection = 0.5;
$pref::OpenGL::force16BitTexture = "0";

// Here is where we will do the video device stuff, so it overwrites the defaults
// First set the PCI device variables (yes AGP/PCI-E works too)
initDisplayDeviceInfo();

// Default to OpenGL
$pref::Video::displayDevice = "OpenGL";
$pref::Video::preferOpenGL = 1;

// And not full screen
$pref::T2D::fullscreen = 0;

// This logic would better be in a kind of database file
switch$( $PCI_VEN )
{
   case "VEN_8086": // Intel
      $pref::Video::displayDevice = "D3D";
      $pref::Video::allowOpenGL = 0;
      
      // Force fullscreen on the 810E and 815G
      if( $PCI_DEV $= "DEV_1132" || $PCI_DEV $= "DEV_7125" )
         $pref::T2D::fullscreen = "1";
         
   case "VEN_1039": // SIS
      $pref::Video::allowOpenGL = 0;
      $pref::Video::displayDevice = "D3D";
      
   case "VEN_1106": // VIA
      $pref::Video::allowOpenGL = 0;
      $pref::Video::displayDevice = "D3D";
      
   case "VEN_5333": // S3
      $pref::Video::allowOpenGL = 0;
      $pref::Video::displayDevice = "D3D";
      
   case "VEN_1002": // ATI
      $pref::Video::displayDevice = "OpenGL";
      
      if( $PCI_DEV $= "DEV_5446" ) // Rage 128 Pro
      {
         $pref::Video::displayDevice = "D3D";
         $pref::Video::allowOpenGL = 0;
      }
         
   case "VEN_10DE": // NVIDIA
      $pref::Video::displayDevice = "OpenGL";
}