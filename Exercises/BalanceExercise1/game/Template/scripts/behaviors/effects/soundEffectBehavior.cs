//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior plays a sound on a raised signal
/// </summary>
if (!isObject(SoundEffectBehavior))
{
    %template = new BehaviorTemplate(SoundEffectBehavior);

    %template.friendlyName = "SoundEffectBehavior";
    %template.behaviorType = "Effect";
    %template.description  = "Play a sound effect for object events.";

    %template.addBehaviorField(sound, "The object's trail object animation.", object, "PL_DefaultSound", AudioAsset);
    %template.addBehaviorField(instanceName, "The animation name.", string, "");

    %template.addBehaviorInput(PlaySound, "Play Sound", "Tells the behavior that it should play its sound effect.");
    %template.addBehaviorInput(stopSoundInput, "Stop Sound", "Tells the behavior that it should stop its sound effect.");
    %template.addBehaviorInput(PlaySoundNoInterrupt, "Play Sound No Interrupt", "Tells the behavior that it should play its sound if it is not already playing.");
}

/// <summary>
/// This input receives the PlaySound signal to play a sound effect
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function SoundEffectBehavior::PlaySound(%this, %fromBehavior, %fromOutput)
{
    // Don't trigger sounds when tools are active
    if ($LevelEditorActive)  
        return;  
        
    if (%this.owner.currentSound)
    {
        alxStop(%this.owner.currentSound);
    }

    if ( AssetDatabase.isDeclaredAsset(%this.sound) )
    {
        %this.owner.currentSound = alxPlay(%this.sound);
    }
}

/// <summary>
/// This input receives the StopSoundInput signal to stop a sound effect
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function SoundEffectBehavior::stopSoundInput(%this, %fromBehavior, %fromOutput)
{
    if (%this.owner.currentSound)
    {
        alxStop(%this.owner.currentSound);
        %this.owner.currentSound = "";
    }
}

/// <summary>
/// This input receives the PlaySound signal to play a sound effect.
/// It does not cancel the currentSound if it is already playing.
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function SoundEffectBehavior::PlaySoundNoInterrupt(%this, %fromBehavior, %fromOutput)
{
    // Don't trigger sounds when tools are active
    if ($LevelEditorActive)  
        return;      
    
    if (alxIsPlaying(%this.owner.currentSound)) 
        return;   
    
    if ( AssetDatabase.isDeclaredAsset(%this.sound) )
    {
        %this.owner.currentSound = alxPlay(%this.sound);
    }
}