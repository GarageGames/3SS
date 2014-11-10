//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(LaunchableBehavior))
{
    %template = new BehaviorTemplate(LaunchableBehavior);
    %template.friendlyName = "Launchable";
    %template.behaviorType = "Launcher";
    %template.description = "Lets an object be launchable";
    
    // Fields
    %template.addBehaviorField(timeout, "Timeout period in seconds before the object is cleaned up", int, 30);    
    
    // Inputs
    %template.addBehaviorOutput(launched, "Launched Signal", "Signal that the owner has been launched");
}

/// <summary>
/// Raises a launched output on the owner, and schedules the owner for cleanup after a timeout period.
/// </summary>
function LaunchableBehavior::launch(%this)
{
    %this.owner.Raise(%this, launched);
    ScheduleManager.scheduleEvent(%this.timeout * 1000, GameEventManager, "postEvent", "_Cleanup", %this.owner);
}