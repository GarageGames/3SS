//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// The BackgroundMusicBehavior handles playing the level background music.
/// </summary>
if (!isObject(BackgroundMusicBehavior))
{
    %template = new BehaviorTemplate(BackgroundMusicBehavior);

    %template.friendlyName = "Background Music";
    %template.behaviorType = "Global";
    %template.description = "Plays a set of looping background music";

    %backgroundSongs = "None" TAB "Song1" TAB "Song2" TAB "Song3" TAB "Song4";
    //%template.addBehaviorField(songProfile, "Background song to play.", enum, "None", %backgroundSongs);
    %template.addBehaviorField(songProfile, "Background song to play.", string, "");
    %template.addBehaviorField(levelLoadDelay, "The time delay from when the level loads before the behavior activates (in seconds)", float, 2.0);
}

/// <summary>
/// Handle onRemove event by stopping audio streams.
/// </summary>
function BackgroundMusicBehavior::onRemove(%this)
{
    if ($platform $= "iOS")
    {
        if (%this.musicStreamHandle)
            stopiPhoneAudioStream(%this.musicStreamHandle);
    }
    else
    {
        alxStopAll();
    }
}

/// <summary>
/// Handle onAddToScene event by scheduling music to play.
/// </summary>
function BackgroundMusicBehavior::onAddToScene(%this, %scene)
{
    if(!$LevelEditorActive)
        %this.schedule(%this.levelLoadDelay * 1000, "startMusic");
}

/// <summary>
/// Starts the background music playing
/// </summary>
function BackgroundMusicBehavior::startMusic(%this)
{
    if (%this.musicStreamHandle)
        alxStop(%this.musicStreamHandle);

    if (%this.songProfile !$= "None")
        %this.musicStreamHandle = alxPlay(%this.songProfile);
}

/// <summary>
/// Stops playing background music
/// </summary>
function BackgroundMusicBehavior::stopMusic(%this)
{
    alxStop(%this.musicStreamHandle);
}

function BackgroundMusicBehavior::getSongProfile(%this)
{
    return %this.songProfile;
}

/// <summary>
/// sets the song profile for the behavior
/// </summary>
function BackgroundMusicBehavior::setSongProfile(%this, %profile)
{
    %this.songProfile = %profile;
}


// Object-specific callbacks

/// <summary>
/// sets the sound profile for the start level when the level is loaded
/// </summary>
function StartMenuBackgroundMusicObject::onLevelLoaded(%this)
{
    %behavior = %this.callOnBehaviors("setSongProfile", $Save::GeneralSettings::Sound::MenuMusic);
}
