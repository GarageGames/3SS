//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeTowerDefense()
{
    $TDPixelsPerMeter = 64;
    $TDMetersPerPixel = 1 / $TDPixelsPerMeter;

    addPathExpando("TowerDefenseTemplate", expandPath("./"));
    addPathExpando("TowerDefenseData", expandPath("./data"));
    
    // Load project data
    exec("./scripts/projectManagement.cs");
    
    TowerDefense::initializeProject();
    
    //-----------------------------------------------------------------------------
    // Load GUIs
    //-----------------------------------------------------------------------------
    TamlRead("./gui/mainMenu.gui.taml");
    TamlRead("./gui/howToPlay.gui.taml");
    TamlRead("./gui/credits.gui.taml");
    TamlRead("./gui/pause.gui.taml");
    TamlRead("./gui/win.gui.taml");
    TamlRead("./gui/lose.gui.taml");
    TamlRead("./gui/levelselect.gui.taml");
    
    // If we are loading this module from the editor, do not proceed any further
    if ($LevelEditorActive)
        return;
        
    //-----------------------------------------------------------------------------
    // Load scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/game.cs");
    exec("./scripts/game/scene.cs");
    exec("./scripts/ai/pathAI.cs");
    exec("./scripts/ai/sorting.cs");
    exec("./scripts/game/paths.cs");
    exec("./scripts/gui/mainMenu.cs");   
    exec("./scripts/gui/howToPlay.cs");
    exec("./scripts/gui/credits.cs");
    exec("./scripts/gui/hud.cs");
    exec("./scripts/gui/pause.cs");
    exec("./scripts/gui/endGame.cs");
    exec("./scripts/gui/levelselect.cs");
    
    // Startup the game project
    initializeGame();
}

function destroyTowerDefense()
{
    TowerDefense::closeProject();
}