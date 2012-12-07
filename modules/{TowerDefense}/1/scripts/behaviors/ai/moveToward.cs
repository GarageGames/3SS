//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior moves an object toward another object at a specified speed.
/// </summary>
if (!isObject(MoveTowardBehavior))
{
   %template = new BehaviorTemplate(MoveTowardBehavior);
   
   %template.friendlyName = "Move Toward";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to move toward another object";

   %template.addBehaviorField(target, "The object to move toward", object, "", SceneObject);
   %template.addBehaviorField(speed, "The speed to move toward the object at (world units per second)", float, 5.0);
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function MoveTowardBehavior::onAddToScene(%this)
{
   %this.owner.MoveTowardBehaviorPreUpdate = true;
   %this.schedule(150, updateMoveToward);
}

/// <summary>
/// This function updates the object's position.
/// </summary>
function MoveTowardBehavior::updateMoveToward(%this)
{
   // check for 'live' target.
   if (!isObject(%this.target))
   {
      if (%this.owner.removeWithTarget)
      {
         if (%this.owner.MoveTowardBehaviorPreUpdate)
         {
            // didn't even get the first update
            %this.owner.safeDelete();
         }
         else
         {
            %this.schedule(50, updateMoveTowardLast);
         }
      }
         
      return;
   }
   
   // save last known good position in case target is removed.
   %this.owner.lastTargetPosition = %this.target.getPosition();
   %this.owner.MoveTowardBehaviorPreUpdate = false;
   
   %this.owner.moveTo(%this.owner.lastTargetPosition, %this.speed, false, true);
   %this.schedule(150, updateMoveToward);
}

/// <summary>
/// This function moves the object towards the last known good position.
/// </summary>
function MoveTowardBehavior::updateMoveTowardLast(%this)
{
   %this.owner.moveTo(%this.owner.lastTargetPosition, %this.speed, false, true);
   %this.schedule(150, updateMoveTowardLast);
}

/// <summary>
/// This function sets the target object.
/// </summary>
/// <param name="target">The object to move toward.</param>
function MoveTowardBehavior::setTarget(%this, %target)
{
   %this.target = %target;
}

/// <summary>
/// This function handles arrival at our target location.
/// </summary>
function MoveTowardBehavior::onPositionTarget(%this)
{
   %this.owner.safeDelete();
}