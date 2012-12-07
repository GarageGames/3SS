//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// This module is reponsible setting variables and calling functions that
// will be used during the life of the entire editor. Examples include DSO
// preferences, reacting to specific OS requirements, etc.
function initializeGameBoot()
{
    if ($LevelEditorActive)
        return;
        
    // Seed the random number generator.
    setRandomSeed();
    
    // Timestamp the initialization process start
    $Debug::loadStartTime = getRealTime();
    
    exec("./scripts/defaultPrefs.cs");

    // Load the base tools groups.
    %loadSuccess = false;
    %loadSuccess = ModuleDatabase.LoadGroup("gameBase");
    
    // If the base loading finished, we are going to use an EventManager to post a message
    // This object will take over and handle loading of the next module set, as well
    // as pushing any special GUIs to the screen.
    if (%loadSuccess)
        GameEventManager.postEvent("_StartUpComplete", "");
}

function destroyGameBoot()
{
    if ($LevelEditorActive)
        return;
        
    // Unload the tools groups.
    ModuleDatabase.UnloadGroup( "gameBase" );
}

function onExit()
{
    if ($LevelEditorActive)
        return;
}