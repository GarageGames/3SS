//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// The FaceDestinationBehavior causes the owner to face it's movement destination.
/// </summary>
if (!isObject(FaceDestinationBehavior))
{
   %template = new BehaviorTemplate(FaceDestinationBehavior);
   
   %template.friendlyName = "Face Destination";
   %template.behaviorType = "AI";
   %template.description  = "Set the A* Actor to face its current movement destination";

   %template.addBehaviorField(turnSpeed, "The speed to rotate at (degrees per second). Use 0 to snap", float, 0.0);
   %template.addBehaviorField(rotationOffset, "The rotation offset (degrees)", float, 0.0);
}

/// <summary>
/// This function gets the face destination state.
/// </summary>
/// <return>Returns true - if it has this behavior, the object is supposed to face its destination.</return>
function FaceDestinationBehavior::getFaceDestination(%this)
{
   // Indicate that yes, an object with this behavior should rotate to face
   // its destination.
   return true;
}

/// <summary>
/// This function handles updating facing when the owner reaches a path waypoint.
/// </summary>
/// <param name="waypointPos">The index of the touch event associated with this call.</param>
/// <param name="adjustedPos">The world position of the touch event.</param>
function FaceDestinationBehavior::onNextWayPoint(%this, %waypointPos, %adjustedPos)
{
   //%vector = Vector2Sub(%adjustedPos, %this.owner.getPosition());
   //%targetRotation = mRadToDeg(mAtan(%vector.y, %vector.x)) + 90 + %this.rotationOffset;
   %targetRotation = t2dAngleToPoint(%adjustedPos, %this.owner.getPosition()) +180 + %this.rotationOffset;
   if (%this.turnSpeed == 0)
   {
      %this.owner.setAngle(%targetRotation);
   }
   else
   {
      %this.owner.rotateTo(%targetRotation, %this.turnSpeed, true, false, true, 0.1);
   }
}
