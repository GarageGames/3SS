//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// The DirectionalAnimationBehavior sets the owner's animation based on the direction that the
/// owner is moving/facing.
/// </summary>
if (!isObject(DirectionalAnimationBehavior))
{
   %template = new BehaviorTemplate(DirectionalAnimationBehavior);
   
   %template.friendlyName = "Directional Animation";
   %template.behaviorType = "AI";
   %template.description  = "Set the A* Actor's movement animation based on direction";
}

/// <summary>
/// This function handles basic initialzation tasks.
/// </summary>
function DirectionalAnimationBehavior::onAddToScene(%this)
{
   %this.type = %this.owner.getClassName();
   if (%this.type $= "t2dAnimatedSprite")
      %this.typeID = 1;
   else
      %this.typeID = 0;
   
   %this.updateFrequency = 150; // ms
   
   %this.enabled = false;
}

/// <summary>
/// This function checks the direction that the owner is facing.  Directions are divided into 
/// quadrants.  It then sets the appropriate animation on the owner.
/// </summary>
function DirectionalAnimationBehavior::testDirection(%this)
{
   if (!%this.enabled)
      return;
   
   if(%this.owner.dead)
      return;
   
   if(MainScene.getScenePause())
   {
      // Paused so skip the logic and reschedule
      %this.rescheduleTest();
      return;
   }
   
   // %this.owner.nextNodeCoord should contain the next node in the A* path for the actor.
   %vector = %this.owner.getLinearVelocity();
   %targetRotation = -1 * mRadToDeg(mAtan(%vector.y, %vector.x));
   
   if (%targetRotation <= 180 && %targetRotation > 135)
   {
      if (%this.direction == 1)
      {
         %this.rescheduleTest();
         return;
      }

      %this.direction = 1;
      //echo(" -- getAnimationFrame = " @ %currentFrame);
      if (%this.owner.animationSet.MoveWestMirror !$= "")
         %this.owner.setFlipX(true);
      else
         %this.owner.setFlipX(false);
      %this.owner.playAnimation(%this.owner.animationSet.MoveWestAnim, false);
      //%this.owner.BlendColor = "1.0 1.0 0.0 0.5";
      //echo(" -- moving West : " @ %targetRotation);
   }
   else if (%targetRotation <= 135 && %targetRotation > 45)
   {
      if (%this.direction == 2)
      {
         %this.rescheduleTest();
         return;
      }

      %this.direction = 2;
      //echo(" -- getAnimationFrame = " @ %currentFrame);
      if (%this.owner.animationSet.MoveSouthMirror !$= "")
      {
         %this.owner.setFlipY(true);
      }
      else
      {
         %this.owner.setFlipY(false);
      }
      %this.owner.playAnimation(%this.owner.animationSet.MoveSouthAnim, false);
      //%this.owner.BlendColor = "1.0 1.0 0.0 1.0";
      //echo(" -- moving South : " @ %targetRotation);
   }
   else if (%targetRotation <= 45 && %targetRotation > -45)
   {
      if (%this.direction == 3)
      {
         %this.rescheduleTest();
         return;
      }

      %this.direction = 3;
      //echo(" -- getAnimationFrame = " @ %currentFrame);
      if (%this.owner.animationSet.MoveEastMirror !$= "")
      {
         %this.owner.setFlipX(true);
      }
      else
      {
         %this.owner.setFlipX(false);
      }
      %this.owner.playAnimation(%this.owner.animationSet.MoveEastAnim, false);
      //%this.owner.BlendColor = "1.0 0.0 0.0 1.0";
      //echo(" -- moving East : " @ %targetRotation);
   }
   else if (%targetRotation <= -45 && %targetRotation > -135)
   {
      if (%this.direction == 4)
      {
         %this.rescheduleTest();
         return;
      }

      %this.direction = 4;
      //echo(" -- getAnimationFrame = " @ %currentFrame);
      // Temporarily flip west anim
      if (%this.owner.animationSet.MoveNorthMirror !$= "")
      {
         %this.owner.setFlipY(true);
      }
      else
      {
         %this.owner.setFlipY(false);
      }
      %this.owner.playAnimation(%this.owner.animationSet.MoveNorthAnim, false);
      //%this.owner.BlendColor = "0.0 0.0 1.0 1.0";
      //echo(" -- moving North : " @ %targetRotation);
   }
   else if (%targetRotation <= -135 && %targetRotation > -180)
   {
      if (%this.direction == 1)
      {
         %this.rescheduleTest();
         return;
      }

      %this.direction = 1;
      //echo(" -- getAnimationFrame = " @ %currentFrame);
      if (%this.owner.animationSet.MoveWestMirror !$= "")
      {
         %this.owner.setFlipX(true);
      }
      else
      {
         %this.owner.setFlipX(false);
      }
      %this.owner.playAnimation(%this.owner.animationSet.MoveWestAnim, false);
      //%this.owner.BlendColor = "1.0 1.0 0.0 1.0";   
      //echo(" -- moving West 1b : " @ %targetRotation);
   }
   else
   {
      if (%this.direction == 2)
      {
         %this.rescheduleTest();
         return;
      }

      %this.direction = 2;
      //echo(" -- getAnimationFrame = " @ %currentFrame);
      if (%this.owner.animationSet.MoveWestMirror !$= "")
      {
         %this.owner.setFlipX(true);
      }
      else
      {
         %this.owner.setFlipX(false);
      }
      %this.owner.playAnimation(%this.owner.animationSet.MoveWestAnim, false);
      //%this.owner.BlendColor = "1.0 1.0 0.0 1.0";
      //echo(" -- moving West direction : " @ %targetRotation);
   }
   
   %this.rescheduleTest();
}

/// <summary>
/// This function handles scheduling the polling for the object's direction.
/// </summary>
function DirectionalAnimationBehavior::rescheduleTest(%this)
{
   %this.updateSchedule = %this.schedule(%this.updateFrequency, testDirection);
}

/// <summary>
/// This function returns a string representation of the owner's facing.
/// </summary>
/// <return>The name of the cardinal direction that the owner is facing.</return>
function DirectionalAnimationBehavior::getDirection(%this)
{
   switch (%this.direction)
   {
      case 1:
         return "West";
      case 2:
         return "South";
      case 3:
         return "East";
      case 4:
         return "North";
   }
}

/// <summary>
/// This function enables the directional animation test.  Used to reduce unnecessary 
/// scheduling in the editor.
/// </summary>
function DirectionalAnimationBehavior::enableDirectionalAnimation(%this)
{
   %this.enabled = true;
   %this.direction = 0;
   
   if (%this.typeID == 1)
   {
      %this.rescheduleTest();
   }
}

/// <summary>
/// This function disables the directional animation test.  Used to reduce unnecessary 
/// scheduling in the editor.
/// </summary>
function DirectionalAnimationBehavior::disableDirectionalAnimation(%this)
{
   %this.enabled = false;
   cancel(%this.updateSchedule);
}