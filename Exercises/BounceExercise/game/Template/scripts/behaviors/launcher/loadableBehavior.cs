//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(LoadableBehavior))
{
    %template = new BehaviorTemplate(LoadableBehavior);
    %template.friendlyName = "Loadable";
    %template.behaviorType = "Launcher";
    %template.description = "Lets an object be loadable";
    
    // Inputs
    %template.addBehaviorOutput(loaded, "Loaded Signal", "Signal that the owner has been loaded");
}

/// <summary>
/// Raises the loaded output on the owner.
/// </summary>
function LoadableBehavior::load(%this)
{
    %this.owner.Raise(%this, loaded);
}