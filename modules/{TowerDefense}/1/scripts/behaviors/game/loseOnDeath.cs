//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior causes the player to lose the game if the owner dies.
/// </summary>
if (!isObject(LoseOnDeathBehavior))
{
   %template = new BehaviorTemplate(LoseOnDeathBehavior);
   
   %template.friendlyName = "Lose on death";
   %template.behaviorType = "Game";
   %template.description  = "Upon actor death, lose the game";
}

/// <summary>
/// This function sets game over, displays the lose screen and pauses the scene.
/// </summary>
function LoseOnDeathBehavior::lose(%this)
{
   if (%this.dying)
      return;
   
   %this.dying = true;
   
   MainScene.gameOver = true;
   //echo(" -- LoseOnDeathBehavior::lose() - gameOver = " @ MainScene.gameOver);
   Canvas.pushDialog(loseGui);
   ScheduleManager.setPause(true);
   MainScene.schedule(100, "setScenePause", true);
}