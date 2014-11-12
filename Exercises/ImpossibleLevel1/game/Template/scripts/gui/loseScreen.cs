//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// Displays the win screen with score, stars earned and the high score
/// </summary>
function loseGui::onWake(%this)
{
    alxStopAll();

    if (AssetDatabase.isDeclaredAsset(%this.music))
        %this.playing = alxPlay(%this.music);
}

function loseGui::onSleep(%this)
{
    alxStop(%this.playing);
}

/// <summary>
/// This function restarts the schedule manager, reloads the current level and
/// then cleans up the win GUI.
/// </summary>
function loseRestartButton::onClick(%this)
{
    if (isObject(MainScene))
        MainScene.delete();

    ScheduleManager.initialize();
    loadLevel("^PhysicsLauncherTemplate/data/levels/" @ $CurrentLevel @ ".scene.taml");
    
    if (isObject(GameMaster))
        GameMaster.callOnBehaviors(initializeLevel);

    HudGui.setup();
    helpGui.clear();
    helpGui.setup();
    Canvas.popDialog(loseGui);
}

/// <summary>
/// This function takes us back to the level select screen.
/// </summary>
function loseLevelSelectButton::onClick(%this)
{
    Canvas.pushDialog(levelSelectGui);
}