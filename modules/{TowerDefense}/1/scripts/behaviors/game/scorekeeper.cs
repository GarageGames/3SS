//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior keeps track of enemy deaths, enemy escapes and updates the 
/// appropriate money, score and life displays.
/// </summary>
if (!isObject(ScorekeeperBehavior))
{
   %template = new BehaviorTemplate(ScorekeeperBehavior);
   
   %template.friendlyName = "Scorekeeper";
   %template.behaviorType = "Global";
   %template.description  = "Keep and update scores";

   %template.addBehaviorField(damageSound, "Sound profile played when the owner takes damage", enum, "None", %enemyTargetAudioProfiles);
   %template.addBehaviorField(deathSound, "Sound profile played when the owner dies", enum, "None", %enemyTargetAudioProfiles);
   %template.addBehaviorField(winSound, "Sound profile played when the player wins", enum, "None", %enemyTargetAudioProfiles);
}

/// <summary>
/// This function handles basic initialization tasks.
/// </summary>
function ScorekeeperBehavior::onAddToScene(%this)
{
   //echo(" -- ScorekeeperBehavior::onAddToScene()");
   %this.active = false;
      
   %this.Score = 0;
   %this.totalEnemyCount = 0;
   %this.totalEnemies = 0;
   %this.totalKills = 0;
   
   if (!$LevelEditorActive)
      ScheduleManager.scheduleEvent(250, %this, "ScorekeeperBehavior::initialize", %this);
}

/// <summary>
/// This function handles pre-level setup.
/// </summary>
function ScorekeeperBehavior::onLevelLoaded(%this, %scene)
{
   //echo (" -- ScorekeeperBehavior::onLevelLoaded()");
   if (%this.active == false)
   {
      %this.Score = 0;
      %this.totalEnemyCount = 0;
      %this.totalEnemies = 0;
      %this.totalKills = 0;
      
      ScheduleManager.scheduleEvent(250, %this, "ScorekeeperBehavior::initialize", %this);
   }
}

/// <summary>
/// This function handles final behavior setup before use.
/// </summary>
function ScorekeeperBehavior::initialize(%this)
{
   // this clears the ScoreActorBehavior reports of spawn for template objects
   %this.totalEnemyCount = 0;
   %this.totalKills = 0;
   
   UpdateScore(%this.Score);
 
   %this.active = true;

   //echo(" >> ScorekeeperBehavior::initialize() - "@%this.Funds@" : "@%this.Score@" : "@%this.lives);
}

/// <summary>
/// This function handles damaging the player by subtracting the appropriate number of lives.
/// It also calls to lose() if the player is out of lives.
/// </summary>
/// <param name="amount">The number of lives to subtract from the player.</param>
/// <param name="assailant">The enemy who caused the damage.</param>
/// <return>Returns void if there are no lives remaining, or true otherwise.</return>
function ScorekeeperBehavior::takeDamage(%this, %amount, %assailant)
{
   if (%this.owner.lastAssailant == %assailant)
      return true;
      
   %this.owner.lastAssailant = %assailant;
 
   %lives = SceneBehaviorObject.callOnBehaviors(getLives);
   %lives -= %amount;
   %this.reportEnemyEscape(%assailant);

   SceneBehaviorObject.callOnBehaviors(setLives, %lives);
   UpdateLives(%lives);

   //echo(" -- Lives : " @ %lives);
   if (%lives <= 0)
   {
      //echo(" -- Base has died");
      %this.lose();

      return;
   }
   
   // Play damage sound
   if (%this.damageSound !$= "None" && %this.damageSound !$= "")
   {
      alxPlay(%this.damageSound);
   }
   
   return true;
}

/// <summary>
/// This function returns the object that is the target of the damage.  The current 
/// implementation does not use this, instead relying on the PathFollowingBehavior::onPathNodeReached()
/// callback to determine if the enemy has reached the end of the path.
/// </summary>
/// <return>Returns the assigned damage target object.</return>
function ScorekeeperBehavior::getDefendObj(%this)
{
   return %this.DefendObject;
}

/// <summary>
/// This function reports the spawn of a new enemy.  This is used in headcounting 
/// to determine when all enemies in the level have been destroyed.
/// </summary>
/// <param name="enemy">The ID of the new enemy.</param>
function ScorekeeperBehavior::reportEnemySpawn(%this, %enemy)
{
   if (%this.active == true)
   {
      %this.totalEnemyCount++;
      %enemy.activeEnemy = true;
   }
   
   //echo(" >> ScorekeeperBehavior::reportEnemySpawn() - " @ %enemy @ " : "  @ %this.totalEnemyCount @ " : " @ %this.totalEnemies);
}

/// <summary>
/// This function is called on by the WaveControllerBehavior to set the total number 
/// of enemies in the level.
/// </summary>
function ScorekeeperBehavior::setEnemyTotal(%this, %count)
{
   %this.totalEnemies += %count;

   //echo(" >> ScorekeeperBehavior::setEnemyTotal() - " @ %this.totalEnemies);
}

/// <summary>
/// This function reports the enemy's death and increments the total kills.
/// </summary>
/// <param name="enemy">The enemy that has been killed.</param>
function ScorekeeperBehavior::reportEnemyDeath(%this, %enemy)
{
   if (%this.active == true && %enemy.activeEnemy == true)
   {
      %this.totalKills++;
      %enemy.activeEnemy = false;
   }
   
   //echo(" >> ScorekeeperBehavior::reportEnemyDeath() - " @ %enemy @ " : " @ %this.totalKills @ " / " @ %this.totalEnemies);
   
   return true;
}

/// <summary>
/// This function reports the enemy's escape and updates the score with zero score or
/// money because escape is a bad thing.
/// </summary>
/// <param name="enemy">The escaping enemy.</param>
function ScorekeeperBehavior::reportEnemyEscape(%this, %enemy)
{
   if (%this.active == true && %enemy.activeEnemy == true)
   {
      %this.totalKills++;
      %enemy.activeEnemy = false;
   }
   %this.updateScore(0, 0);   
   //echo(" >> ScorekeeperBehavior::reportEnemyEscape() - " @ %enemy @ " : " @ %this.totalKills @ " / " @ %this.totalEnemies);
   
   return true;
}

/// <summary>
/// This function updates the current score using the provided values.  It also handles
/// triggering the win condition.
/// </summary>
/// <param name="points">The number of points to adjust the score by.</param>
/// <param name="money">The amount of money to adjust the player funds by.</param>
function ScorekeeperBehavior::updateScore(%this, %points, %money)
{
   // Update the total score and currency values, then update the display
   %this.Score += %points;
   %funds = SceneBehaviorObject.callOnBehaviors(getFunds);
   %funds += %money;
   SceneBehaviorObject.callOnBehaviors(setFunds, %funds);
   
   UpdateFunds(%funds);
   UpdateScore(%this.Score);

   if (%this.totalKills == (%this.totalEnemies))
   {
      //echo(" >> ScorekeeperBehavior::updateScore() all enemies destroyed");
      %this.win();
   }
   //echo(" >> ScorekeeperBehavior::updateScore(" @ %points @ ", " @ %money @ ")");
   //echo(" >> ScorekeeperBehavior::updateScore() - " @ %this.totalKills @ " / " @ %this.totalEnemies);
}

/// <summary>
/// This function is called if all enemies have been eliminated from the level and
/// the player still has at least one life.
/// </summary>
function ScorekeeperBehavior::win(%this)
{
   // Play Win sound
   if (%this.winSound !$= "None" && %this.winSound !$= "")
   {
      alxPlay(%this.winSound);
   }

   MainScene.gameOver = true;
   ScheduleManager.setPause(true);
   Canvas.pushDialog(winGui);
}

/// <summary>
/// This function is called if the player has fewer than one life left.
/// </summary>
function ScorekeeperBehavior::lose(%this)
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

/// <summary>
/// This function is used by the General Settings tool to set the audio profile 
/// to be used when an enemy damages the player.
/// </summary>
/// <param name="sound">The name of the audio profile to use for the player damage sound.</param>
function ScorekeeperBehavior::setDamageSound(%this, %sound)
{
    %this.damageSound = %sound;
}

/// <summary>
/// This function is used by the General Settings tool to access the audio profile that
/// is used when an enemy damages the player.
/// </summary>
/// <return>Returns the name of the audio profile that is played when the player is damaged.</param>
function ScorekeeperBehavior::getDamageSound(%this)
{
    return %this.damageSound;
}