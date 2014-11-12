//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior disables the owner in the scene to be used as a clonable 
/// template object.
/// </summary>
if (!isObject(TemplateBehavior))
{
   %template = new BehaviorTemplate(TemplateBehavior);
   
   %template.friendlyName = "Template";
   %template.behaviorType = "Game";
   %template.description  = "Disables an object unless it is a clone";
}

/// <summary>
/// This function sets the owner's enabled flag to false.
/// </summary>
/// <param name="scene">The scene with which the object is associated.</param>
function TemplateBehavior::onLevelLoaded(%this, %scene)
{
   %this.owner.enabled = false;
}
