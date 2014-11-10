//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior uses alpha blending to fade the owner to transparent over a 
/// designated time.
/// </summary>
if (!isObject(fadeBehavior))
{
    %template = new BehaviorTemplate(fadeBehavior);

    %template.friendlyName = "Fade";
    %template.behaviorType = "Effect";
    %template.description  = "Cause the object to fade to transparent over a specified duration.";

    %template.addBehaviorField(vanishTime, "The wait period in seconds before the object starts to vanish.", int, 7);

    %template.addBehaviorField(fadeTime, "The time from vanishTime in seconds until the object has completely faded.", int, 3);
    
    %template.addBehaviorInput(Fade, "Fade", "Notify the behavior to start fading.");

    %template.addBehaviorOutput(Removing, "Removing", "Signals that the object is removing");
}

/// <summary>
/// This function receives the activate signal and flags the behavior as active.
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function fadeBehavior::Fade(%this, %fromBehavior, %fromOutput)
{
    %this.owner.Raise(%this, Removing);
}

/// <summary>
/// This function sends a removing signal in the event that anyone is interested.
/// It also starts the fading process on the way out.
/// </summary>
function fadeBehavior::Removing(%this)
{
    %this.schedule(%this.vanishTime * 1000, startFade);
}

/// <summary>
/// This function sets the fadecount so that the fade() method will take the 
/// appropriate time to blend the alpha of the owner to zero over the duration
/// of the fade-out period.
/// </summary>
function fadeBehavior::startFade(%this)
{
    %this.fadecount = (%this.fadeTime * 1000) / 32;
    %this.fadeOut();
}

/// <summary>
/// This function adjusts the owner's alpha blending to gradually fade it from
/// view, or simply sets the visibility to false if the fadeMode is set to instant.
/// </summary>
function fadeBehavior::fadeOut(%this)
{
    %this.fadecount--;
    if (%this.fadeCount >= 0)
    {
        %mult = %this.fadecount / ((%this.fadeTime * 1000) / 32);
        %this.owner.setBlendColor("1 1 1" SPC %mult);
        ScheduleManager.scheduleEvent(32, %this, "fadeOut");
    }
    else
        GameEventManager.postEvent("_Cleanup", %this.owner);
}