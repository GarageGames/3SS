//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$levelLoading = false;


/// <summary>
/// This function initializes the main menu GUI
/// </summary>
function mainMenuGui::onWake(%this)
{
    menuCreditsButton.visible = mainMenuGui.showCreditsButton;

    if ($pref::Audio::channelVolume1)
    {
        menuSoundButton.setNormalImage(%this.soundBtnOn);
        menuSoundButton.setHoverImage(%this.soundBtnOn);
        menuSoundButton.setDownImage(%this.soundBtnOn);
    }
    else
    {
        menuSoundButton.setNormalImage(%this.soundBtnOff);
        menuSoundButton.setHoverImage(%this.soundBtnOff);
        menuSoundButton.setDownImage(%this.soundBtnOff);
    }

    if (!$TitleMusicHandle && !$TitleMusicSchedule)
    {
        alxStopAll();

        if (AssetDatabase.isDeclaredAsset(%this.kicker) && AssetDatabase.isDeclaredAsset(%this.music))
        {
            %time = alxGetAudioLength(%this.kicker) - 32;
            %this.playKicker(%time);
        }
        else if(!AssetDatabase.isDeclaredAsset(%this.kicker) && AssetDatabase.isDeclaredAsset(%this.music))
            %this.playTitle();
    }
        
    if (AssetDatabase.isDeclaredAsset(%this.buttonSound))
        $ButtonSound = %this.buttonSound;
    if (%this.fontSheet !$= "")
        $FontSheet = %this.fontSheet;

    // Only show the Exit button if we're not on iOS.
    if ($platform $= "windows" || $platform $= "macos")
        menuExitButton.Visible = true;
    else
        menuExitButton.Visible = false;

    GuiDefaultProfile.soundButtonDown = $ButtonSound;
}

/// <summary>
/// This function fires off a "kicker" music piece - it is non-looping and intended
/// as a lead-in for the main theme.  It then schedules the main title music to play.
/// </summary>
function mainMenuGui::playKicker(%this, %time)
{
    $TitleMusicHandle = alxPlay(%this.kicker);
    // we need a way to get the duration of a sound to make this work right.
    $TitleMusicSchedule = %this.schedule(%time, playTitle);  
}

/// <summary>
/// This function plays the main title music.  It is here because it's easier to 
/// schedule this if it is in its own function.
/// </summary>
function mainMenuGui::playTitle(%this)
{
    $TitleMusicHandle = alxPlay(%this.music);
}

/// <summary>
/// This function handles the start button by loading and launching the level select GUI.
/// </summary>
function menuPlayButton::onClick(%this)
{
   Canvas.pushDialog(worldSelectGui);
   Canvas.schedule(50, popDialog, mainMenuGui);
}

/// <summary>
/// This function shows the help screen GUI.
/// </summary>
function mainMenuHowToPlayButton::onClick(%this)
{
   Canvas.pushDialog(howToPlayGui);
}

/// <summary>
/// This function shows the credits GUI.
/// </summary>
function menuCreditsButton::onClick(%this)
{
   Canvas.pushDialog(creditsGui);
}

/// <summary>
/// This function will toggle the game's sound on and off
/// </summary>
function menuSoundButton::onClick(%this)
{
    if ($soundOn == true || $soundOn $= "")
    {
        $soundOn = false;
        for (%channel = 0; %channel < 32; %channel++)
        {
            alxSetChannelVolume(%channel, 0.0);
            $pref::Audio::channelVolume[%channel] = 0.0;
        }
        menuSoundButton.setNormalImage(mainMenuGui.soundBtnOff);
        menuSoundButton.setHoverImage(mainMenuGui.soundBtnOff);
        menuSoundButton.setDownImage(mainMenuGui.soundBtnOff);
        
        if ($TitleMusicSchedule)
            cancel($TitleMusicSchedule);

        alxStopAll();
        $TitleMusicHandle = "";
        $TitleMusicSchedule = "";
    }
    else
    {
        $soundOn = true;
        for (%channel = 0; %channel < 32; %channel++)
        {
            alxSetChannelVolume(%channel, 1.0);
            $pref::Audio::channelVolume[%channel] = 1.0;
        }
        menuSoundButton.setNormalImage(mainMenuGui.soundBtnOn);
        menuSoundButton.setHoverImage(mainMenuGui.soundBtnOn);
        menuSoundButton.setDownImage(mainMenuGui.soundBtnOn);

        if ($TitleMusicHandle $= "" && $TitleMusicSchedule $= "")
        {
            alxStopAll();
            if (AssetDatabase.isDeclaredAsset(mainMenuGui.kicker) && AssetDatabase.isDeclaredAsset(mainMenuGui.music))
                mainMenuGui.playKicker();
            else if(!AssetDatabase.isDeclaredAsset(mainMenuGui.kicker) && AssetDatabase.isDeclaredAsset(mainMenuGui.music))
                mainMenuGui.playTitle();
        }
    }
}

function menuExitButton::onClick(%this)
{
    // Exit the game.
    quit();
}