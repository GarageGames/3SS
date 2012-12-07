//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
//
// Save config data
//
function _saveGameConfigurationData()
{
   populateAllFontCacheRange(32, 255);
   writeFontCache();

   $Game::DefaultScene = collapsePath($levelEditor::LastLevel);
   
   %file = expandPath("^project/scripts/system/prefs.cs");
   
   export("$Game::*", "^project/scripts/system/prefs.cs", true, false);
   export("$pref::iOS::ScreenOrientation", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::ScreenDepth", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::UseGameKit", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::UseMusic", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::UseMoviePlayer", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::UseAutoRotate", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::EnableOrientationRotation", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::iOS::StatusBarType", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::Audio::*", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::T2D::particleEngineQuantityScale", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::T2D::warnFileDeprecated", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::T2D::warnSceneOccupancy", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::OpenGL::*", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::Video::fullScreen", "^project/scripts/system/prefs.cs", true, true);
   export("$pref::Video::defaultResolution", "^project/scripts/system/prefs.cs", true, true);
   
   //the file has 2 lines, OTHER_LDFLAGS and IT2D_EDITOR_FLAGS
   %configpath = expandPath("^project/../../buildFiles/XCode_iPhone/3StepStudio/en.lproj/Config.xcconfig");
   %binaryConfigPath = expandPath("^project/../../buildFiles/Xcode_iOS/3StepStudio/en.lproj/Config.xcconfig");
   
   if(isFile(%configpath))
   {
      %fo = new FileObject();
      %fo.openForWrite(%configpath);

      //Write some lines in
      %fo.writeLine("IT2D_LDFLAGS = " @ makeLDFlags());
      %fo.writeLine("IT2D_EDITOR_FLAGS =" @ makePreprocessorFlags());
      %fo.writeLine("IT2D_SOURCE_PATH = " @ $headersDirectory);
         
      %fo.close();
      %fo.delete();
   }
   
   if(isFile(%binaryConfigPath))
   {
      %fo = new FileObject();
      %fo.openForWrite(%binaryConfigPath);

      //Write some lines in
      %fo.writeLine("IT2D_LDFLAGS = " @ makeLDFlags());
      %fo.writeLine("IT2D_EDITOR_FLAGS =" @ makePreprocessorFlags());
      %fo.writeLine("IT2D_SOURCE_PATH = " @ $headersDirectory);
         
      %fo.close();
      %fo.delete();
   }
   
   return true;
}

//
// Load config data
//
function _loadGameConfigurationData( %gamePath )
{
   %prefsFile = %gamePath @ "/scripts/system/prefs.cs";
   
   if(isFile(%prefsFile))
      exec(%prefsFile);
   else
      exec(%gamePath @ "/scripts/system/defaultPrefs.cs");
}

//
// Load default config data
//
function _defaultGameConfigurationData()
{
   ///
   /// Game Properties
   ///
   $Game::CompanyName         = "Independent";
   $Game::ProductName         = "defaultName";
   $Game::DefaultScene        = "data/levels/emptyLevel.t2d";

   ///
   /// iOS Properties
   ///
   $pref::iOS::DeviceType          = $iOS::constant::iPhone;
   $pref::iOS::ScreenOrientation   = $iOS::constant::Landscape;
   $pref::iOS::ScreenDepth		   = 32;

   // Turn all features off by default
   $pref::iOS::UseGameKit          = 0;
   $pref::iOS::UseMusic            = 0;
   $pref::iOS::UseMoviePlayer      = 0;
   $pref::iOS::StatusBarType       = 0;
   $pref::iOS::EnableOrientationRotation  = 1;
   $pref::iOS::EnableOtherOrientationRotation  = 0;
   
   // Torque 2D does not have true, GameKit networking support. 
   // The old socket network code is untested, undocumented and likely broken. 
   // This will eventually be replaced with GameKit. 
   // For now, it is confusing to even have a checkbox in the editor that no one uses or understands. 
   // If you are one of the few that uses this, uncomment the next line. -MP 1.5 
   //$pref::iOS::UseNetwork          = 0;
}
