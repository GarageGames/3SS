//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
/// <summary>
/// This behavior handles keeping towers from attacking enemies before they enter the
/// playing field.
/// </summary>
if (!isObject(EnemySafeZoneBehavior))
{
   // Create this behavior from the blank BehaviorTemplate
   %template = new BehaviorTemplate(EnemySafeZoneBehavior);
   
   // friendlyName will be what is displayed in the editor
   // behaviorType organize this behavior in the editor
   // description briefly explains what this behavior does
   %template.friendlyName = "Enemy Safe Zone";
   %template.behaviorType = "Game";
   %template.description  = "Works with the enemy safe zone that toggles tower targeting";
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function EnemySafeZoneBehavior::onBehaviorAdd(%this)
{
   %this.OUT_OF_GAME = 0;
   %this.ENTERING_PLAY = 1;
   %this.ON_FIELD = 2;
   
   %this.state = %this.OUT_OF_GAME;

   %this.updateFrequency = 250; // ms
      
   %this.halfSizeX = %this.owner.getSizeX() * 0.5;
   %this.halfSizeY = %this.owner.getSizeY() * 0.5;
   %this.leaveSizeX = 0;
   %this.leaveSizeY = 0;
   
   %worldSize = MainScene.cameraSize;
   %this.worldHalfSizeX = getWord(%worldSize, 0) / 2.0;
   %this.worldHalfSizeY = getWord(%worldSize, 1) / 2.0;
   
   %this.enterMinX = %this.halfSizeX - %this.worldHalfSizeX;
   %this.enterMinY = %this.halfSizeY - %this.worldHalfSizeY;
   %this.enterMaxX = %this.worldHalfSizeX - %this.halfSizeX;
   %this.enterMaxY = %this.worldHalfSizeY - %this.halfSizeY;
   
   %this.leaveMinX = %this.leaveSizeX - %this.worldHalfSizeX;
   %this.leaveMinY = %this.leaveSizeY - %this.worldHalfSizeY;
   %this.leaveMaxX = %this.worldHalfSizeX - %this.leaveSizeX;
   %this.leaveMaxY = %this.worldHalfSizeY - %this.leaveSizeY;
   
   %this.ready = false;
}

/// <summary>
/// This function checks for enemies that have entered the field and sets their state 
/// to allow attacks.
/// </summary>
function EnemySafeZoneBehavior::checkZone(%this)
{
   if(%this.owner.dead || !%this.ready)
      return;
   
   if(MainScene.getScenePause())
   {
      // Paused so skip the logic and reschedule
      %this.recheduleCheck();
      return;
   }
   
   if (%this.state == %this.ENTERING_PLAY)
   {
      // Check if the enemy has crossed the safe zone boundary.
      if (EnemySafeZoneBoundaryEdge.getIsPointInOOBB(%this.owner.getPosition()))
      {
         // Now on the play field
         %this.state = %this.ON_FIELD;
         %this.owner.callOnBehaviors("enableTowerTarget");
         %this.owner.setSceneGroup(MainScene.validEnemyGroup);
      }
      
      %this.recheduleCheck();
   }
   else if (%this.state == %this.ON_FIELD)
   {
      // Check if the enemy has left the target zone
      if (!EnemyTargetZone.getIsPointInOOBB(%this.owner.getPosition()))
      {
         // The enemy is no longer a valid target
         %this.state = %this.OUT_OF_GAME;
         %this.owner.callOnBehaviors("disableTowerTarget");
      }
      else
      {
         %this.recheduleCheck();
      }
   }
}

/// <summary>
/// This function reschedules the checkZone() method.
/// </summary>
function EnemySafeZoneBehavior::recheduleCheck(%this)
{
   %this.updateSchedule = %this.schedule(%this.updateFrequency, checkZone);
}

/// <summary>
/// This function disables the enemy save zone.
/// </summary>
function EnemySafeZoneBehavior::disableEnemySafeZone(%this)
{
   %this.ready = false;
   // DAW: Not available in Box2D
   //%this.owner.setWorldLimit("OFF");
   cancel(%this.updateSchedule);
}

/// <summary>
/// This function resets the enemy to allow it to enter the f
/// </summary>
function EnemySafeZoneBehavior::resetEnemySafeZone(%this)
{
   %this.ready = true;
   
   // The enemy is not a tower target until it is completely on screen
   // DAW: Not available in Box2D
   //%this.owner.setWorldLimit("OFF");
   %this.owner.callOnBehaviors("disableTowerTarget");
   %this.state = %this.ENTERING_PLAY;

   %this.recheduleCheck();
   
   //%this.owner.setBlendColor(1, 0, 0);
}
