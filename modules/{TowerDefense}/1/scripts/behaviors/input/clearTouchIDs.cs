//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$CapturedTouchIDsOwners = new SimSet();


/// <summary>
/// This behavior clears the touchIDs passed into touch events
/// </summary>
if (!isObject(ClearTouchIDsBehavior))
{
   %template = new BehaviorTemplate(ClearTouchIDsBehavior);
   
   %template.friendlyName = "Clear Touch IDs";
   %template.behaviorType = "Input";
   %template.description  = "Clears captured Touch IDs matching one passed to onTouchUp";
}

/// <summary>
/// This function initializes the behavior
/// </summary>
function ClearTouchIDsBehavior::onBehaviorAdd(%this)
{
   %this.owner.UseInputEvents = true;
}

/// <summary>
/// This function handles the onTouchUp callback.  It iterates through all 
/// objects that have been added to the $CapturedTouchIDsOwners set and 
/// calls the onTouchUp for them as well if the touchID matches.
/// </summary>
/// <param name="touchID">The touchID to search for.</param>
/// <param name="worldPos">The position of the touch event.</param>
function ClearTouchIDsBehavior::onTouchUp(%this, %touchID, %worldPos)
{
   if (isObject($CapturedTouchIDsOwners))
   {
      %count = $CapturedTouchIDsOwners.getCount();
      
      for (%i = 0; %i < %count; %i++)
      {
         %touchIDOwner = $CapturedTouchIDsOwners.getObject(%i);
         
         if (%touchIDOwner.touchID $= %touchID)
            %touchIDOwner.onTouchUp(%touchID, %worldPos);
      }
   }
}