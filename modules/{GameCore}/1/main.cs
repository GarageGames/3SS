//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeGameCore()
{   
    if ($LevelEditorActive)
        return;
        
    // Locate the cached font directory
    $Gui::fontCacheDirectory = expandPath("game/template/data/fonts");
    
    $cursorControlled = false;

    // Execute basic system and class scripts  
    exec("./gui/profiles.cs");
    exec("./scripts/canvas.cs");
    exec("./scripts/openal.cs");      
    exec("./scripts/properties.cs");
    exec("./scripts/Scene.cs");
    exec("./scripts/SceneObject.cs");
    exec("./scripts/SceneWindow.cs");
    exec("./scripts/simSet.cs");
    exec("./scripts/levelManagement.cs");
    exec("./scripts/animationSet.cs");
    exec("./scripts/gameEventManager.cs");
    
    // This will load the core systems, like Canvas and OpenAL    
    initializeSystem();
}

function destroyGameCore()
{
    if ($LevelEditorActive)
        return;
        
    if(isFunction("shutdownProject"))
        shutdownProject();

    alxStopAll();
    shutdownOpenAL();
    destroyGameEventManager();
}

function initializeSystem()
{
    // Now load the last game configuration, if it exists
    _loadGameConfigurationData();

    // Initialize the canvas.
    initializeCanvas( $Game::ProductName );

    // Start up the audio system.
    initializeOpenAL();

    // Set a default cursor.
    Canvas.setCursor(DefaultCursor);
    Canvas.showCursor();

    // Create the game event manager
    initializeGameEventManager();
    
    // Load up the Splash Screen GUI and Script
    TamlRead("./gui/SplashScreen.gui.taml");
    exec("./scripts/SplashScreen.cs");
    
    // Load the console dropdown
    TamlRead("./gui/consoleGui.gui.taml");
    exec("./scripts/console.cs");
    
    GlobalActionMap.bind(keyboard, "ctrl tilde", toggleConsole);
}

//------------------------------------------------------------------------------
// Helper function used for loading separate packages
// This relies on the mod path having a main.cs
//------------------------------------------------------------------------------
function loadDir( %dir )
{
    // Set Mod Paths.
    setModPaths(getModPaths() @ ";" @ %dir);

    // Execute Boot-strap file.
    exec(%dir @ "/main.cs");
}

//------------------------------------------------------------------------------
// Cursor toggle functions.
//------------------------------------------------------------------------------
function showCursor()
{
    if ($cursorControlled)
        lockMouse(false);
        
    Canvas.cursorOn();
}

function hideCursor()
{
    if ($cursorControlled)
        lockMouse(true);
        
    Canvas.cursorOff();
}

//------------------------------------------------------------------------------
// Rounds a number
//------------------------------------------------------------------------------
function mRound(%num)
{
    if((%num-mFloor(%num)) >= 0.5)
        %value = mCeil(%num);
    else
        %value = mFloor(%num);

    return %value;
}