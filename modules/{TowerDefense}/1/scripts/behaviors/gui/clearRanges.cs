//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// The ClearRangesBehavior is attached to the background image and handles
/// hiding the range circle object when it is clicked.
/// </summary>
if (!isObject(ClearRangesBehavior))
{
   %template = new BehaviorTemplate(ClearRangesBehavior);
   
   %template.friendlyName = "Clear Ranges";
   %template.behaviorType = "GUI";
   %template.description  = "Hide all active ShowRange and ToggleRange behaviors";
}

/// <summary>
/// This is called when the behavior is added to the object.  It just clears itself
/// and sets up to use input events.
/// </summary>
function ClearRangesBehavior::onBehaviorAdd(%this)
{
   %this.touchID = "";
   %this.owner.UseInputEvents = true;
}

/// <summary>
/// Handles hiding the range circle object.
/// </summary>
/// <param name="touchID">The index of the touch - indicates which of up to 11 simultaneous touch events this is.</param>
/// <param name="worldPos">The world position at which the touch event occured.</param>
function ClearRangesBehavior::onTouchDown(%this, %touchID, %worldPos)
{
   if ($TouchHandled)
   {
      $TouchHandled = false;
      
      return;      
   }
   
   if (isObject($TowerBeingPlaced))
      return;
      
   // Cruise through the scene and find range circle behaviors, then
   // toggle them all off
   if (isObject($RangeBehaviorCollection))
   {
      %count = $RangeBehaviorCollection.getCount();
      
      for (%i = 0; %i < %count; %i++)
      {
         %behavior = $RangeBehaviorCollection.getObject(%i);
         %behavior.hide();
      }
   }
}