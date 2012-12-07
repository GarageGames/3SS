//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// This behavior makes use of the path follow steering component.

// Used to initialize cachedRemainingPathDistance
$MaxPathDistance = 10000;

$SlowestSpeed = 20;
$SlowSpeed = 30;
$AverageSpeed = 40;
$FastSpeed = 50;
$FastestSpeed = 60;

/// <summary>
/// This function enables or disables the onButtonUp callback on the advance wave button
/// </summary>
if (!isObject(PathFollowingBehavior))
{
   // Create this behavior from the blank BehaviorTemplate
   %template = new BehaviorTemplate(PathFollowingBehavior);
   
   // friendlyName will be what is displayed in the editor
   // behaviorType organize this behavior in the editor
   // description briefly explains what this behavior does
   %template.friendlyName = "Path Following";
   %template.behaviorType = "Movement Styles";
   %template.description  = "Provides A* path following generated from a Grid.";

   %template.addBehaviorField(destination, "The target destination for this actor.", object, "", Grid);
   %template.addBehaviorField(moveSpeed, "The actor's movement speed.", int, 10);
   %template.addBehaviorField(maxForce, "Maximum forces applied due to steering.", float, 100.0);
   %template.addBehaviorField(forceMultiplier, "Modifies the AI steering applied force on the object.", float, 1.0);
   %template.addBehaviorField(pathGrid, "Grid used for A* path finding.", object, "", Grid);
   %template.addBehaviorField(wayPointDistance, "Radius around path way points that defines their touch zone.", float, 0.05);
   %template.addBehaviorField(pathOffsetRangeX, "Range used to randomly calculate an offset from the path's waypoints in the X direction.", float, 0.0);
   %template.addBehaviorField(pathOffsetRangeY, "Range used to randomly calculate an offset from the path's waypoints in the Y direction.", float, 0.0);
}

/// <summary>
/// This function handles initialization tasks
/// </summary>
function PathFollowingBehavior::onBehaviorAdd(%this)
{
   if ($LevelEditorActive)
   {
      // Handle the fact that C++ AI behaviors are not compiled with the tools
      %this.Component = 0;
      return;
   }
   
   // Create the movement component
   %this.Component = new AIMovementComponent();
   %this.originalForce = %this.maxForce;
   %this.originalMultiplier = %this.forceMultiplier;
   
   %this.owner.AIMovementComponent = %this.Component;
   
   // Copy all of the behavior information to the component
   copyBehaviorToComponent(%this, %this.Component);
   
   // set this behavior as the AI movement callback destination
   %this.Component.setCallbackDestination(%this);
   
   // Set the path offset
   %xoffset = %this.pathOffsetRangeX / 2.0 * (getRandom() * 2.0 - 1.0);
   %yoffset = %this.owner.getSizeY() / 2.0 - 0.05;
   %yoffset = %yoffset - %this.pathOffsetRangeY / 2.0 * (getRandom() * 2.0 - 1.0);
   %this.Component.pathOffset = %xoffset @ " " @ %yoffset;
   
   // Add the component
   if (!%this.owner.addComponents(%this.Component))
   {
      error("PathFollowingBehavior::onBehaviorAdd() - Failed to register AIMovement Component");
      %this.Component.safeDelete();
      return;
   }
   
   // Now add the steering
   %pathFollow = new AIPathFollowComponent();
   %this.Component.addComponents(%pathFollow);
   %this.AIComponent = %pathFollow;
   
   // Defaults
   %this.destinationCallback = false;
   %this.Component.setAIActive(false);
   
   %this.cachedRemainingPathDistance = $MaxPathDistance;
}

/// <summary>
/// This function handles preparing the behavior to be used upon addition to the scene
/// </summary>
function PathFollowingBehavior::onAddToScene(%this)
{
   // set unit speed from its baseSpeed dynamic field
   %this.owner.moveSpeed = %this.moveSpeed;
   %this.Component.forceMultiplier = 1; //30;
   
   %speedFraction = %this.moveSpeed / 60;
   %this.Component.maxSpeed = %speedFraction * 0.885;
   
   // Check if the owner should rotate to face the destination.  If so then
   // we issue a callback when we hit a way point.
   %check = %this.owner.callOnBehaviors("getFaceDestination");
   if (%check == true)
   {
      %this.destinationCallback = true;
   }
   else
   {
      %this.destinationCallback = false;
   }
}

/// <summary>
/// This function handles removal of the behavior.
/// </summary>
function PathFollowingBehavior::onBehaviorRemove(%this)
{
}

/// <summary>
/// This function tells the owner to create a path and follow it.  If the destination is 
/// not a Grid object the path creation algorithm won't be able to create a path.
/// </summary>
/// <param name="destination">The name of a pathgrid to use for the pathfinding behavior.</param>
function PathFollowingBehavior::goToDestination(%this, %destination)
{
   // pick a random destination
   //if (!isObject(DestinationSet))
   //{
      ////  we should have destinations in our level
      //error("No available destinations in level!");
      //%this.schedule( 250, "die");
      //return "0 0";
   //}
   //
   //%maxTargetCount = DestinationSet.getCount();
   //%targetDestinationIndex = getRandom(1, %maxtargetCount) - 1;
   //%targetDestination = DestinationSet.getObject(%targetDestinationIndex);
   //%targetCoords = %targetDestination.getPosition();
   if(isObject(%destination))
   {
      %this.pathGrid = %destination;
      %targetCoords = getGridEndPoint(%destination);
   }
   else
   {
      %this.pathGrid = %this.destination;
      %targetCoords = getGridEndPoint(%this.destination);
      echo(" ** destination undefined, using default DestinationPoint1");
   }
   
   %this.goToWorldDestination(%targetCoords);
}

/// <summary>
/// This function handles generating the path and actually starting the entity moving on it.
/// </summary>
/// <param name="destCoords">The world coordinates of the path's end point.</param>
function PathFollowingBehavior::goToWorldDestination(%this, %destCoords)
{
   %oldPath = %this.Component.getPathObject();
   if (%oldPath != 0)
   {
      %oldPath.safeDelete();
   }
   //echo(" ** " @ %destCoords);
   %this.destCoords = %destCoords;
   %pos = Vector2Sub(%this.owner.getPosition(), %this.Component.pathOffset);
   //%path = %this.pathGrid.createPath(%this.owner.getPosition(), %destCoords, true);
   %path = %this.pathGrid.createPath(%pos, %destCoords, true);
   %this.Component.setPathObject(%path); // NOTE: If the path ID is 0 (from an invalid path) then this call will just clear the path object.  No harm done, just no motion.
   %this.Component.setPathDestination(0);
   %this.Component.setPathLoop(false);
   %this.Component.setPathDecelerationRate(0.0);
   %this.Component.setPathWayPointDistance(%this.Component.wayPointDistance);
   %this.Component.setPathOffset(%this.Component.pathOffset);
}

/// <summary>
/// This function resets the entity's path and stops the entity.
/// </summary>
function PathFollowingBehavior::resetPathDestination(%this)
{
   %this.Component.setPathDestination(0);
}

/// <summary>
/// Callback for when the owner reaches a node on the path.
/// </summary>
/// <param name="component">The SimObjectID that called this method.  In this case, the AIPathFollowComponent instance</param>
/// <param name="nodeIndex">The path node that has been reached</param>
/// <param name="lastNode">True if this is the last node in a non-looping path</param>
function PathFollowingBehavior::onPathNodeReached(%this, %component, %nodeIndex, %lastNode)
{
   //echo("PathFollowingBehavior::onPathNodeReached( " @ %this SPC %component SPC %nodeIndex SPC %lastNode @ ")");

   if (%this.destinationCallback && !%lastNode)
   {
      %path = %component.getPathObject();
      if (%path != 0)
      {
         %node = %path.getNode(%nodeIndex+1);
         %pos = getWords(%node, 0, 1);
         %adjustedPos = Vector2Add(%pos, %this.Component.pathOffset);
         %this.owner.callOnBehaviors("onNextWayPoint", %pos, %adjustedPos);
      }
   }
   
   if (%lastNode == true)
   {
      //echo(" -- PathFollowingBehavior::onPathNodeReached() - End!");
      %this.owner.callOnBehaviors("dealDamage", GlobalBehaviorObject);   
   }
}

/// <summary>
/// Sets the maximum force that can be applied to the owner of the behavior.
/// </summary>
/// <param name="force">The maximum force that can be applied to this object.</param>
function PathFollowingBehavior::setMaxForce(%this, %force)
{
   %this.maxForce = %force;
}

/// <summary>
/// Sets the maximum force that can be applied to the owner of the behavior back to its 
/// default value.
/// </summary>
function PathFollowingBehavior::resetMaxForce(%this)
{
   echo(" >> resetMaxForce");
   %this.maxForce = %this.originalMaxForce;
}

/// <summary>
/// This function sets the force multiplier for the owner of the behavior.
/// </summary>
/// <param name="multiplier">The factor to multiply forces against this object by.</param>
function PathFollowingBehavior::setForceMultiplier(%this, %multiplier)
{
   %this.forceMultiplier = %multiplier;
}

/// <summary>
/// This function resets the force multiplier to its default value.
/// </summary>
function PathFollowingBehavior::resetForceMultiplier(%this)
{
   %this.forceMultiplier = %this.originalMultiplier;
}

/// <summary>
/// This function is used by the Enemy Tool to access the unit's speed.
/// </summary>
/// <return>Returns a string value to represent speed, Slowest, Slow, Average, Fast, Fastest.</return>
function PathFollowingBehavior::getSpeed(%this)
{
   switch (%this.moveSpeed)
   {
      case $SlowestSpeed:
         return "Slowest";   
      
      case $SlowSpeed:
         return "Slow";
      
      case $AverageSpeed:
         return "Average";
      
      case $FastSpeed:
         return "Fast";
      
      case $FastestSpeed:
         return "Fastest";
   }
}

/// <summary>
/// This function is used by the Enemy Tool to set the unit's movement speed.
/// </summary>
/// <param name="speed">A string representing relative speed, Slowest, Slow, Average, Fast, Fastest.</param>
function PathFollowingBehavior::setSpeed(%this, %speed)
{
   switch$ (%speed)
   {
      case "Slowest":
         %this.moveSpeed = $SlowestSpeed;      
      
      case "Slow":
         %this.moveSpeed = $SlowSpeed;
      
      case "Average":
         %this.moveSpeed = $AverageSpeed;
      
      case "Fast":
         %this.moveSpeed = $FastSpeed;
      
      case "Fastest":
         %this.moveSpeed = $FastestSpeed;
   }
}

/// <summary>
/// This function returns the path offset for the unit.
/// </summary>
/// <return>A vector string "X Y" that holds the current unit's path offset.</return>
function PathFollowingBehavior::getPathFollowingOffset(%this)
{
   return %this.Component.pathOffset;
}

/// <summary>
/// This function enables the pathfollowing behavior instance.
/// </summary>
function PathFollowingBehavior::enablePathFollowing(%this)
{
   %this.Component.setAIActive(true);
   
   %this.updatesActive = true;
   %this.updateCachedRemainingPathDistance();
}

/// <summary>
/// This function disables the pathfollowing behavior - this is used to prevent 
/// enemy units from wandering about in the editor.
/// </summary>
function PathFollowingBehavior::disablePathFollowing(%this)
{
   %this.Component.setAIActive(false);
   
   %this.updatesActive = false;
   if (isObject(%this.updateSchedule))
      cancel(%this.updateSchedule);
}

/// <summary>
/// This function returns the movement speed in m/s.
/// </summary>
/// <return>Returns unit speed in m/s.</return>
function PathFollowingBehavior::getMoveSpeed(%this)
{
    return %this.Component.maxSpeed;
}

/// <summary>
/// This function sets the movement speed in m/s.
/// </summary>
/// <param name="speed">The desired movement speed in m/s.</param>
function PathFollowingBehavior::setMoveSpeed(%this, %speed)
{
    %this.Component.maxSpeed = %speed;
}

/// <summary>
/// This function resets the unit's original speed.
/// </summary>
function PathFollowingBehavior::resetMoveSpeed(%this)
{
   // set unit speed from its baseSpeed dynamic field
   %this.owner.moveSpeed = %this.moveSpeed;
   %this.Component.forceMultiplier = 1; //30;
   
   %speedFraction = %this.moveSpeed / 60;
   %this.Component.maxSpeed = %speedFraction * 0.885;
}

// Get the total remaining distance along the path for the owner from its current position
// to the end of the path
function PathFollowingBehavior::getRemainingPathDistance(%this)
{
   if (!isObject(%this.AIComponent.getPathObject()))
      return $MaxPathDistance;
   
   return %this.AIComponent.getPathDistanceToEnd(%this.owner);  
}

function PathFollowingBehavior::getCachedRemainingPathDistance(%this)
{
   return %this.cachedRemainingPathDistance;  
}

function PathFollowingBehavior::updateCachedRemainingPathDistance(%this)
{
   %this.cachedRemainingPathDistance = %this.getRemainingPathDistance();
   
   if (%this.updatesActive)
      %this.updateSchedule = %this.schedule(1000, updateCachedRemainingPathDistance);
}
