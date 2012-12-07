//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Audio channel descriptions.
//------------------------------------------------------------------------------
$musicAudioType = 1;
$effectsAudioType = 2;

//------------------------------------------------------------------------------
// initializeOpenAL
// Starts up the OpenAL driver.
//------------------------------------------------------------------------------
function initializeOpenAL()
{
    // Just in case it is already started.
    shutdownOpenAL();

    echo("OpenAL Driver Init");

    if (!OpenALInitDriver())
    {
        echo("OpenALInitDriver() failed");
        $Audio::initFailed = true;
    }
    else
    {
        // Set the master volume.
        alxListenerf(AL_GAIN_LINEAR, $pref::Audio::masterVolume);

        // Set the channel volumes.
        for (%channel = 1; %channel <= 3; %channel++)
            alxSetChannelVolume(%channel, $pref::Audio::channelVolume[%channel]);

        echo("OpenAL Driver Init Success");
    }
}

//------------------------------------------------------------------------------
// shutdownOpenAL
//------------------------------------------------------------------------------
function shutdownOpenAL()
{
    OpenALShutdownDriver();
}