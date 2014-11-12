//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializePhysicsLauncher()
{   
    $PhysicsLauncherTools::PixelsPerMeter = 64;
    $PhysicsLauncherTools::MetersPerPixel = 1 / $PhysicsLauncherTools::PixelsPerMeter;
    
    addPathExpando("PhysicsLauncherTemplate", expandPath("./"));
    addPathExpando("PhysicsLauncherData", expandPath("./data"));
    
    // Load project data
    exec("./scripts/projectManagement.cs");
    
    PhysicsLauncher::initializeProject();
    
    // Load the GUI files
    exec("./gui/MLTextCenteredLgProfile.cs");
    exec("./gui/TextCenteredLgProfile.cs");
    exec("./gui/MLTextCenteredSmProfile.cs");
    exec("./gui/TextLeftSmProfile.cs");
    exec("./gui/TextCenteredSmProfile.cs");
    exec("./gui/TextRightSmProfile.cs");

    TamlRead("./gui/mainScreen.gui.taml");
    TamlRead("./gui/mainMenu.gui.taml");
    TamlRead("./gui/credits.gui.taml");
    TamlRead("./gui/pause.gui.taml");
    TamlRead("./gui/win.gui.taml");
    TamlRead("./gui/lose.gui.taml");
    TamlRead("./gui/levelSelect.gui.taml");
    TamlRead("./gui/worldSelect.gui.taml");
    TamlRead("./gui/hud.gui.taml");
    TamlRead("./gui/helpDialog.gui.taml");
    
    // If we are loading this module from the editor, do not proceed any further
    if ($LevelEditorActive)
        return;
        
    // Exec the game script files
    exec("./scripts/game.cs");
    exec("./scripts/gui/mainMenu.cs");
    exec("./scripts/gui/credits.cs");
    exec("./scripts/gui/pause.cs");
    exec("./scripts/gui/endGame.cs");
    exec("./scripts/gui/levelselect.cs");
    exec("./scripts/gui/worldselect.cs");
    exec("./scripts/gui/hud.cs");
    exec("./scripts/gui/helpDialog.cs");
    exec("./scripts/gui/winScreen.cs");
    exec("./scripts/gui/loseScreen.cs");
    
    // Startup the game project
    initializeGame();
}

function destroyPhysicsLauncher()
{
    PhysicsLauncher::closeProject();
}