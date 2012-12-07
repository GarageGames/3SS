//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$SoundContainerCurrentlyPlayingSound = null;
//$SoundContainerPlaySoundBitmap = "gui/images/playSoundIcon.png";
//$SoundContainerPauseSoundBitmap = "gui/images/pauseSoundIcon.png";

/// <summary>
/// Sets the state of a gui sound container based on an AudioAsset
/// </summary>
/// <param name="container">The container being set.</param>
/// <param name="AudioAsset">The audio profile to set from.</param>
function setSoundContainer(%container, %AudioAsset)
{
   // Set text field
   %textField = %container.findObjectByInternalName("SoundText", true);
   %textField.setText(%AudioAsset);
   %textField.setActive(false);
}

/// <summary>
/// Gets an AudioAsset from a gui sound container.
/// </summary>
/// <param name="container">The container to get the AudioAsset from.</param>
/// <return>An AudioAsset.</return>
function getAudioProfileFromSoundContainer(%container)
{
   %textField = %container.findObjectByInternalName("SoundText", true);
   %AudioAsset = %textField.getText();
   
   return %AudioAsset;
}

/// <summary>
/// Sets the imagemap of a sound button based on the isplaying flag. If isPlaying
/// is true, sets the "Pause" image. If isPlaying is false, sets the "Play" image.
/// </summary>
/// <param name="container">The container holding the sound button.</param>
/// <param name="isPlaying">boolean that determines if play or pause image is shown.</param>
function setSoundButtonBitmap(%container, %isPlaying)
{
   %soundButton = %container.findObjectByInternalName("playSoundButton", true);

   if (%isPlaying) 
      %soundButton.setBitmap(GetFullFileName($SoundContainerPauseSoundBitmap)); 
   else 
      %soundButton.setBitmap(GetFullFileName($SoundContainerPlaySoundBitmap));
}

/// <summary>
/// Checks if the sound has finished playing and updates
/// the sound button if it has.
/// </summary>
/// <param name="container">The sound container</param>
/// <param name="soundHandle">A handle to a playing sound.</param>
function updateSoundContainer(%container, %soundHandle)
{
   if (alxIsPlaying(%soundHandle))
   {
      schedule(100, 0, updateSoundContainer, %container, %soundHandle);
   }
   else 
   {
      setSoundButtonBitmap(%container, false);
      %container.isPlayingSound = false;
   }
}

/// <summary>
/// onAction callback from a sound button. Turn sound playing on/off
/// based on whether the sound is already playing.
/// </summary>
function playSoundButtonPressed()
{
   // Get the button control and audio profile for the active sound container
   $ActiveContainer = $ThisControl.getParent();
   %soundButton = $ActiveContainer.findObjectByInternalName("playSoundButton", true);
   %AudioAsset = getAudioProfileFromSoundContainer($ActiveContainer);

   // Turn off any currently playing sounds
   if (alxIsPlaying($SoundContainerCurrentlyPlayingSound))
      alxStop($SoundContainerCurrentlyPlayingSound);   
   
   // start playing a new sound
   $SoundContainerCurrentlyPlayingSound = alxPlay(%AudioAsset);
}

/// <summary>
/// onAction callback from a sound button.
/// </summary>
function stopSoundButtonPressed()
{
    $ActiveContainer = $ThisControl.getParent();
    
   if (alxIsPlaying($SoundContainerCurrentlyPlayingSound))
      alxStop($SoundContainerCurrentlyPlayingSound); 
}

/// <summary>
/// (Deprecated) onAction callback from a sound button. Turn sound playing on/off
/// based on whether the sound is already playing.
/// </summary>
function playSoundButtonPressedOld()
{
   // Get the button control and audio profile for the active sound container
   $ActiveContainer = $ThisControl.getParent();
   %soundButton = $ActiveContainer.findObjectByInternalName("playSoundButton", true);
   %AudioAsset = getAudioProfileFromSoundContainer($ActiveContainer);

   // Turn off any currently playing sounds
   if (alxIsPlaying($SoundContainerCurrentlyPlayingSound))
      alxStop($SoundContainerCurrentlyPlayingSound);   
   
   // If the current container is playing a sound, return
   if ($ActiveContainer.isPlayingSound == true)
   {
      $ActiveContainer.isPlayingSound = false;
      return;
   }
   
   // Otherwise, start playing a new sound
   $SoundContainerCurrentlyPlayingSound = alxPlay(%AudioAsset);
   setSoundButtonBitmap($ActiveContainer, true);
   $ActiveContainer.isPlayingSound = true;

   // Start update loop to check when the sound finishes
   updateSoundContainer($ActiveContainer, $SoundContainerCurrentlyPlayingSound);
}
