//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// Pauses the Game and Displays the Pause Menu
/// </summary>
function pauseGui::onWake(%this)
{
    gameActionMap.pop();
    
    %scene = sceneWindow2D.getScene();
    %scene.setScenePause(true);
    ScheduleManager.setPause(true);

    // check our channel volume to determine if we're muted or not, then
    // set the button image appropriately.
    if ($pref::Audio::channelVolume1)
    {
        pauseSoundButton.setNormalImage(%this.soundBtnOn);
        pauseSoundButton.setHoverImage(%this.soundBtnOn);
        pauseSoundButton.setDownImage(%this.soundBtnOn);
    }
    else
    {
        pauseSoundButton.setNormalImage(%this.soundBtnOff);
        pauseSoundButton.setHoverImage(%this.soundBtnOff);
        pauseSoundButton.setDownImage(%this.soundBtnOff);
    }

    %levelName = fileBase($SelectedLevel);
    %levelName = strreplace(%levelName, ".scene", "");
    LevelNameLabel.text = %levelName;

    // This needs to be tested thoroughly - $LevelIndex is set in 
    // LevelSelectEvent::onMouseDown() in levelSelect.cs.  The index
    // calculation may need adjusting.
    LevelNumberLabel.text = $SelectedWorld + 1 @ " - " @ $LevelIndex + 1;
}

/// <summary>
/// Unpauses the game when the pause screen is cleared.
/// </summary>
function pauseGui::onDialogPop(%this)
{
    %scene = sceneWindow2D.getScene();
    if (isObject(%scene))
        %scene.setScenePause(false);

    ScheduleManager.setPause(false);
    gameActionMap.push();
}

//-----------------------------------------------------------------------------
// Button callbacks and utility functions
//-----------------------------------------------------------------------------

/// <summary>
/// Clears the pause screen to return to play.
/// </summary>
function pauseResumeButton::onClick(%this)
{
    %scene = sceneWindow2D.getScene();
    if (isObject(%scene))
        %scene.setScenePause(false);

    ScheduleManager.setPause(false);
    Canvas.popDialog(pauseGui);
}

/// <summary>
/// Toggles the sound mute state.  This also handles twiddling the sound button
/// images.
/// </summary>
function pauseSoundButton::onClick(%this)
{
    if ($soundOn == true || $soundOn $= "")
    {
        $soundOn = false;
        for (%channel = 0; %channel < 32; %channel++)
        {
            alxSetChannelVolume(%channel, 0.0);
            $pref::Audio::channelVolume[%channel] = 0.0;
        }
        pauseSoundButton.setNormalImage(pauseGui.soundBtnOff);
        pauseSoundButton.setHoverImage(pauseGui.soundBtnOff);
        pauseSoundButton.setDownImage(pauseGui.soundBtnOff);
        
        alxStopAll();
    }
    else
    {
        $soundOn = true;
        for (%channel = 0; %channel < 32; %channel++)
        {
            alxSetChannelVolume(%channel, 1.0);
            $pref::Audio::channelVolume[%channel] = 1;
        }
        pauseSoundButton.setNormalImage(pauseGui.soundBtnOn);
        pauseSoundButton.setHoverImage(pauseGui.soundBtnOn);
        pauseSoundButton.setDownImage(pauseGui.soundBtnOn);

        alxStopAll();
        if (AssetDatabase.isDeclaredAsset(MainScene.music))
            %this.playing = alxPlay(MainScene.music);
    }
}

/// <summary>
/// This function restarts the schedule manager, reloads the current level and
/// then cleans up the pause GUI.
/// </summary>
function pauseRestartButton::onClick(%this)
{
    //ScheduleManager.initialize();
    if (isObject(MainScene))
        MainScene.delete();

    loadLevel($SelectedLevel);

    HudGui.setup();
    helpGui.clear();
    helpGui.setup();
    Canvas.popDialog(pauseGui);
}

/// <summary>
/// This function takes us back to the level select screen.
/// </summary>
function pauseLevelSelectButton::onClick(%this)
{
    sceneWindow2D.endLevel(sceneWindow2D);
    Canvas.pushDialog(levelSelectGui);
}

/// <summary>
/// Displays any tutorials that are available for the current level.
/// </summary>
function pauseHelpButton::onClick(%this)
{
    if (helpGui.imageCount > 0)
        Canvas.pushDialog(helpGui);
}