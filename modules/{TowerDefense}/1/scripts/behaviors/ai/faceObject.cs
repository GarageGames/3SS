//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior causes an entity to face a designated object.
/// </summary>
if (!isObject(FaceObjectBehavior))
{
   %template = new BehaviorTemplate(FaceObjectBehavior);
   
   %template.friendlyName = "Face Object";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to face another object";

   %template.addBehaviorField(object, "The object to face", object, "", SceneObject);
   %template.addBehaviorField(turnSpeed, "The speed to rotate at (degrees per second). Use 0 to snap", float, 0.0);
   %template.addBehaviorField(rotationOffset, "The rotation offset (degrees)", float, 0.0);
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function FaceObjectBehavior::onBehaviorAdd(%this)
{
   %this.updateFrequency = 150; // ms
   %this.ready = false;
}

/// <summary>
/// This function updates the object's rotation.
/// </summary>
function FaceObjectBehavior::updateRotation(%this)
{
   if (!%this.ready)
      return;
   
   if (!isObject(%this.object))
      return;
   
   if(MainScene.getScenePause())
   {
      // Paused so skip the logic and reschedule
      %this.rescheduleUpdate();
      return;
   }
   
//   %vector = Vector2Sub(%this.object.position, %this.owner.position);
//   %targetRotation = mRadToDeg(mAtan(%vector.y, %vector.x)) + 90 + %this.rotationOffset;
   %targetRotation = t2dAngleToPoint(%this.object.position, %this.owner.position) + %this.rotationOffset;
   
   if (%this.turnSpeed == 0)
   {
      %this.owner.setAngle(%targetRotation);
   }
   else
   {
      %this.owner.rotateTo(%targetRotation, %this.turnSpeed, true, false, true, 0.1);
   }
      
   %this.rescheduleUpdate();
}

/// <summary>
/// This function schedules the next update.
/// </summary>
function FaceObjectBehavior::rescheduleUpdate(%this)
{
   %this.updateSchedule = %this.schedule(%this.updateFrequency, updateRotation);
}

/// <summary>
/// This function sets the object's rotation to face the target.
/// </summary>
/// <param name="target">The target object to face.</param>
function FaceObjectBehavior::setFaceTarget(%this, %target)
{
   %this.object = %target;
   
   if (isObject(%this.object))
   {
      // Do an immediate update when the object is set
      cancel(%this.updateSchedule);
      %this.ready = true;
      %this.updateRotation();
   }
   else
   {
      %this.ready = false;
      cancel(%this.updateSchedule);
      %this.owner.cancelRotateTo();
   }
}
