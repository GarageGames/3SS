//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior leaves a trail of images behind a moving object.
/// </summary>
if (!isObject(VanishBehavior))
{
    %template = new BehaviorTemplate(VanishBehavior);

    %template.friendlyName = "Vanish";
    %template.behaviorType = "Effect";
    %template.description  = "Cause the object to vanish with a specified animation.";

    %template.addBehaviorField(instanceName, "The behavior instance name.", string, "");
    %template.addBehaviorField(vanishTime, "The wait period in seconds before the object starts to vanish.", int, 3);
    %template.addBehaviorField(vanishDuration, "The wait period in seconds while the object vanishes.", int, 1);

    %template.addBehaviorInput(Vanish, "Vanish", "Notify the behavior to play its vanishing animation.");

    %template.addBehaviorOutput(Vanishing, "Vanishing", "Signals that the object is vanishing");
    %template.addBehaviorOutput(vanishedOutput, "vanishedOutput", "Signals that the object has vanished");
}

/// <summary>
/// This function handles initial setup of the VanishBehavior.
/// </summary>
function VanishBehavior::onAddToScene(%this)
{
    %this.owner.setSleepingCallback(true);
    %this.active = false;
    %this.vanishing = false;
}

/// <summary>
/// This function handles performing the configured vanishing act.
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function VanishBehavior::Vanish(%this, %fromBehavior, %fromOutput)
{
    ScheduleManager.scheduleEvent((1000 * %this.vanishTime), %this, "doVanish");
}

function VanishBehavior::doVanish(%this)
{
    %this.owner.setActive(false);

    if (!%this.vanishing)
    {
        %this.vanishing = true;
        %this.owner.Raise(%this, Vanishing);
        
        %this.schedule(%this.vanishDuration * 1000, "onVanishFinished");
    }
}

/// <summary>
/// Sets the owner visibility to false and cleans it up.
/// </summary>
function VanishBehavior::onVanishFinished(%this)
{
    %this.owner.setVisible(false);
    %this.owner.Raise(%this, vanishedOutput);
    GameEventManager.postEvent("_Cleanup", %this.owner);
}