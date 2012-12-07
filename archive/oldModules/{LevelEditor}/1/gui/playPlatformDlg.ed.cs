//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$platformPlaySelection = "iPad";

function showPlayPlatform()
{
   Canvas.pushDialog(PlayPlatformDlg);
   
   deviceListBox.clear();
   deviceListBox.add("iPhone", 0);
   deviceListBox.add("iPad", 1);
   deviceListBox.add("Desktop", 2);

   deviceListBox.setSelected(deviceListBox.findText($platformPlaySelection));
}

function updatePlatformSelection()
{
   $platformPlaySelection = deviceListBox.getText();
   echo("Device selection changed");
}

function playPlatform(%device)
{  
   checkForMissingDatablocks();
   
   $preLaunchDeviceType = $pref::iOS::DeviceType;
   $preLaunchFullsScreen = $pref::T2D::fullscreen;
   $preLaunchScreenResolution = $pref::iOS::ScreenResolution;
   
   switch$(%device)
   {
      case "iPhone":
         echo("Now playing iPhone build");
         $pref::iOS::DeviceType       = $iOS::constant::iPhone;
         $pref::Video::defaultResolution = "480 320";
            
      case "iPad":
         echo("Now playing iPad build");
         $pref::Video::defaultResolution = "1024 768";
         
      case "Desktop":
         echo("Now playing Desktop Build");
         $pref::Video::defaultResolution = "1024 768";
   }
   echo("Saving file for iOS Build settings ! " @ %fname);
   
   %fname = expandPath("^project/common/commonConfig.xml");
   %prefsDSO = "^project/scripts/system/prefs.cs";
   fileDelete(%prefsDSO);
   
   _saveGameConfigurationData();// %fname );
   
   Canvas.popDialog(PlayPlatformDlg);
   
   runGame();
}