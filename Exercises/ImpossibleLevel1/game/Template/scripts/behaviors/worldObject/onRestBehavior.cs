//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior raises a signal when the owner enters a sleep state in the
/// physics sim.
/// </summary>
if (!isObject(onRestBehavior))
{
    %template = new BehaviorTemplate(onRestBehavior);

    %template.friendlyName = "On Rest Behavior";
    %template.behaviorType = "Effect";
    %template.description  = "Signal to interested behaviors that the object has come to rest.";
    
    %template.addBehaviorField(instanceName, "The behavior instance name.", string, "");

    %template.addBehaviorInput(Activate, "Activate", "Notify the behavior activate the rest action.");

    %template.addBehaviorOutput(Resting, "Resting", "Signals that the object is at rest");
}

/// <summary>
/// This function sets the default activated state to false.
/// </summary>
function onRestBehavior::onBehaviorAdd(%this)
{
    %this.owner.activated = false;
}

//function onRestBehavior::onLevelLoaded(%this, %scene)
//{
    //if (%this.owner.getAwake())
        //%this.onWake();
//}
//
//function onRestBehavior::onAddToScene(%this, %scene)
//{
    //%this.owner.activated = false;
    //if (%this.owner.getAwake())
        //%this.onWake();
//}

function onRestBehavior::onWake(%this)
{
    if (isObject($ActiveObjectSet) && %this.owner.getBodyType() !$= "static")
        $ActiveObjectSet.addObj(%this.owner);
}

/// <summary>
/// This function catches the onSleep callback from the physics system and raises
/// the Resting signal if the owner is activated for resting.
/// </summary>
function onRestBehavior::onSleep(%this)
{
    if (isObject($ActiveObjectSet))
        $ActiveObjectSet.removeObj(%this.owner);

    if (%this.owner.activated)
    {
        %this.owner.Raise(%this, Resting);
    }
}

/// <summary>
/// This function receives the activate signal and flags the behavior as active.
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function onRestBehavior::Activate(%this, %fromBehavior, %fromOutput)
{
    %this.owner.activated = true;
    %this.owner.setActive(true);
}

/// <summary>
/// This function is used to set the activated flag.  The activated flag is used 
/// to prevent the Resting signal from being raised before we want it to.
/// </summary>
/// <param name="flag">Sets the active state flag on the owner.</param>
function onRestBehavior::setActiveState(%this, %flag)
{
    %this.owner.activated = %flag;
    %this.owner.setActive(%flag);
}