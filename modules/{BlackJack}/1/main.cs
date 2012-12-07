//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeBlackjack()
{   
    addPathExpando("BlackjackTemplate", expandPath("./"));
    addPathExpando("BlackjackData", expandPath("./data"));
    
    // Load project data
    exec("./scripts/projectManagement.cs");
    
    Blackjack::initializeProject();
    
    // Load gui files
    TamlRead("./gui/mainScreen.gui.taml");
    TamlRead("./gui/splashScreen.gui.taml");
    TamlRead("./gui/helpScreen.gui.taml");
    TamlRead("./gui/tableSelect.gui.taml");
    TamlRead("./gui/tablePlay.gui.taml");
    TamlRead("./gui/credits.gui.taml");

    
    // If we are loading this module from the editor, do not proceed any further
    if ($LevelEditorActive)
        return;
        
    // Exec the game script files
    exec("./scripts/game.cs");
    exec("./scripts/gui/SplashScreen.cs"); 
    exec("./scripts/gui/helpScreen.cs");
    exec("./scripts/gui/tableSelect.cs");
    exec("./scripts/gui/tablePlay.cs");
    exec("./scripts/gui/credits.cs");
    
    // Startup the game project
    initializeGame();
}

function destroyBlackJack()
{
    Blackjack::closeProject();
}

/*
//enableDebugOutput( true );
dbgSetParameters( 6060, "password", false );

//------------------------------------------------------------------------------
// onStart
// This is the startup procedure for this iTorque 2D Project
//------------------------------------------------------------------------------
function onStart()
{
   // Timestamp the initialization process start
   $Debug::loadStartTime = getRealTime();
   
   // System version printout
   echo("\niEngine (" @ getEngineVersion() @ ") initializing...");
      
   // Load preferences.
   exec("scripts/system/properties.cs");
   
   // Load up helper modules and system
   exec("scripts/system/xml.cs");
   exec("scripts/system/system.cs");
   
   // Exec game scripts.   
   exec("scripts/game.cs");
   exec("scripts/audioDescriptions.cs");
   exec("scripts/audioProfiles.cs");
   
   // Exec GUI Scripts
   exec("gui/helpScreen.cs");
   exec("gui/tableSelect.cs");
   exec("gui/tablePlay.cs");
   exec("gui/credits.cs");

   
   _defaultGameconfigurationData();
   
   _loadGameConfigurationData();
   
   // Initialize the core systems (canvas, Scene, OpenAL, etc
   if (!isFunction("initializeSystem"))
   {
      messageBox( "Game Startup Error", "'initializeSystem' function could not be found." @
                  "\nThis could indicate a bad or corrupt scripts/system directory for your game." @
                  "\n\nThe Game will now shutdown because it cannot properly function", "Ok", "MIStop" );
      quit();
   }
   
   initializeSystem();
   
   // Initialize project management for game and editor
   if (!isFunction("initializeGame") || !isFunction("_initializeProject"))
   {
      messageBox("Game Startup Error", "'initializeGame'  or '_initializeProject' functions could not be found." @
                 "\nThis could indicate a system directory for your game." @
                 "\n\nThe Game will now shutdown because it cannot properly function", "Ok", "MIStop");
      quit();
   }
   
   // Responsible for loading behaviors, datablocks, brushes, etc
   _initializeProject();

   // Load up the in-game gui.
   // Size and orientation will be set in initializeGame()
   exec("gui/mainScreen.gui");
   
   // Load up the Splash Screen GUI and Script
   exec("gui/SplashScreen.gui");
   exec("scripts/gui/SplashScreen.cs");   
   
   // Load up the other GUIs
   exec("gui/helpScreen.gui");
   exec("gui/tableSelect.gui");
   exec("gui/tablePlay.gui");
   exec("gui/credits.gui");
   
   // Startup the game project
   initializeGame();
}

function onExit()
{
   // Save the default player profile
   if (isObject($DefaultPlayerProfile))
   {
      $DefaultPlayerProfile.updateFromPlayer($UserControlledPlayer);
      $DefaultPlayerProfile.createProfileXMLFile();
   }
   
   if($platform $= "windows" || $platform $= "macos")
      _saveGameConfigurationData();
}

//----------------------------
// BEGIN GAME START UP PROCESS
//----------------------------
onStart();
*/