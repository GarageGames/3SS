//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// This holds the last scene that was loaded.
$lastLoadedScene = "";
$CurrentLevelResources = "";
$CurrentLevel = "";

function getLastLoadedScene()
{
   return $lastLoadedScene;
}

/// <summary>
/// This function is deprecated.
/// </summary>
function transitionlevel()
{  
   // May need to repopulate this once we have multiple levels
}

/// <summary>
/// This function loads and launches a level that is selected in the $SelectedLevel global.
/// </summary>
function loadLevel(%levelPath)
{
    alxStopAll();
    $TitleMusicHandle = "";
    cancel($TitleMusicSchedule);
    $TitleMusicSchedule = "";

    // Load the .taml level file for this game
    sceneWindow2D.endLevel(sceneWindow2D);
    sceneWindow2D.loadLevel(%levelPath);
    $levelLoading = false;
    gameActionMap.push();
}