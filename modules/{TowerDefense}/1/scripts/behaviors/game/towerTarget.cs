//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
/// <summary>
/// This behavior sets the owner as a valid target for the TowerShootsBehavior.
/// </summary>
if (!isObject(TowerTargetBehavior))
{
   // Create this behavior from the blank BehaviorTemplate
   %template = new BehaviorTemplate(TowerTargetBehavior);
   
   // friendlyName will be what is displayed in the editor
   // behaviorType organize this behavior in the editor
   // description briefly explains what this behavior does
   %template.friendlyName = "Tower Target";
   %template.behaviorType = "Game";
   %template.description  = "Indicates this object is a possible tower target.";
}

/// <summary>
/// This function handles basic initialization tasks.
/// </summary>
function TowerTargetBehavior::onAddToScene(%this)
{
   if (!isObject(TowerTargetSet))
   {
      new SimSet(TowerTargetSet);
   }
   
   TowerTargetSet.add(%this.owner);
}

/// <summary>
/// This function allows towers to target the owner.
/// </summary>
function TowerTargetBehavior::enableTowerTarget(%this)
{
   TowerTargetSet.add(%this.owner);
}

/// <summary>
/// This function stops towers from attacking the owner.
/// </summary>
function TowerTargetBehavior::disableTowerTarget(%this)
{
   if (TowerTargetSet.isMember(%this.owner))
      TowerTargetSet.remove(%this.owner);
}

/// <summary>
/// This function stops towers from attacking the owner.
/// </summary>
function TowerTargetBehavior::clearTowerTarget(%this)
{
   if (TowerTargetSet.isMember(%this.owner))
      TowerTargetSet.remove(%this.owner);
}