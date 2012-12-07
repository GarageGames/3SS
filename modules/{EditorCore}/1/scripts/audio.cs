//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------
// initializeOpenAL
// Starts up the OpenAL driver.
//---------------------------------------------------------------------------------------------
function initializeOpenAL()
{
   echoSeparator();
   echo("Audio Initialization:");
   
   // Just in case it is already started.
   shutdownOpenAL();

   if($pref::Audio::driver $= "OpenAL")
   {
      if(!OpenALInitDriver())
      {
         error("   Failed to initialize driver.");
         $Audio::initFailed = true;
      }
      else
      {
         // Print out driver info.
         echo("   Vendor: " @ alGetString("AL_VENDOR"));
         echo("   Version: " @ alGetString("AL_VERSION"));  
         echo("   Renderer: " @ alGetString("AL_RENDERER"));
         echo("   Extensions: " @ alGetString("AL_EXTENSIONS"));
         
         // Set the master volume.
         alxListenerf(AL_GAIN_LINEAR, $pref::Audio::masterVolume);
         
         // Set the channel volumes.
         for (%channel = 1; %channel <= 8; %channel++)
            alxSetChannelVolume(%channel, $pref::Audio::channelVolume[%channel]);
         
         echo("");
      }
   }
   else
      error("   Failed to initialize audio system. Invalid driver.");
}

//---------------------------------------------------------------------------------------------
// shutdownOpenAL
//---------------------------------------------------------------------------------------------
function shutdownOpenAL()
{
   OpenALShutdownDriver();
}
