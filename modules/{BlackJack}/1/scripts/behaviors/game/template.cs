//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

if (!isObject(TemplateBehavior))
{
    %template = new BehaviorTemplate(TemplateBehavior);

    %template.friendlyName = "Template";
    %template.behaviorType = "Game";
    %template.description  = "Disables an object unless it is a clone";
}

/// <summary>
/// Called when a level with the behavior is finished loading
/// </summary>
function TemplateBehavior::onLevelLoaded(%this)
{
    %this.owner.enabled = false;
}
