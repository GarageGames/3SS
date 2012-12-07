//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior handles level specific game attributes such as starting funds and 
/// lives.
/// </summary>
if (!isObject(LevelAttributesBehavior))
{
   %template = new BehaviorTemplate(LevelAttributesBehavior);
   
   %template.friendlyName = "Level Attributes";
   %template.behaviorType = "Game";
   %template.description  = "Holds information about this level";

   %template.addBehaviorField(StartFunds, "Amount of money that the player starts with", int , 100);
   %template.addBehaviorField(lives, "The number of enemies that can reach the target before you lose", int, 20);
}

/// <summary>
/// This function handles basic initialization tasks.
/// </summary>
function LevelAttributesBehavior::onAddToScene(%this)
{
   //echo(" -- LevelAttributesBehavior::onAddToScene()");
   %this.active = false;

   if (%this.StartFunds < 0)
      %this.StartFunds = 0;
   if (%this.StartFunds > 10000)
      %this.StartFunds = 10000;
      
   %this.Funds = %this.StartFunds;
   
   if (!$LevelEditorActive)
      ScheduleManager.scheduleEvent(250, %this, "LevelAttributesBehavior::initialize", %this);
}

/// <summary>
/// This function prepares the level for play by setting up starting funds and 
/// calling the behavior's initialize function.
/// </summary>
/// <param name="scene">The scene with which this behavior is to be associated.</param>
function LevelAttributesBehavior::onLevelLoaded(%this, %scene)
{
   //echo (" -- LevelAttributesBehavior::onLevelLoaded()");
   if (%this.active == false)
   {
      if (%this.StartFunds < 0)
         %this.StartFunds = 0;
      if (%this.StartFunds > 10000)
         %this.StartFunds = 10000;
         
      %this.Funds = %this.StartFunds;
      
      ScheduleManager.scheduleEvent(250, %this, "LevelAttributesBehavior::initialize", %this);
   }
}

/// <summary>
/// This function updates the funds and lives display and activates the level for
/// play.
/// </summary>
function LevelAttributesBehavior::initialize(%this)
{
   UpdateFunds(%this.Funds);
   UpdateLives(%this.lives);
 
   %this.active = true;
}

/// <summary>
/// This function is used by the Level Tool to access the number of lives that 
/// the player starts this level with.
/// </summary>
/// <return>Returns the number of lives the player has for this level.</return>
function LevelAttributesBehavior::getLives(%this)
{
   return %this.lives;
}

/// <summary>
/// This function is used by the Level Tool to set the number of lives that 
/// the player starts this level with.
/// </summary>
/// <param name="amount">The number of lives that the player will start the level with.</param>
function LevelAttributesBehavior::setLives(%this, %amount)
{
   %this.lives = %amount;
}

/// <summary>
/// This function is used by the Level Tool to access the amount of money available
/// to the player when the level starts.
/// </summary>
/// <return>Returns the amount of money the player has at the start of this level.</return>
function LevelAttributesBehavior::getStartFunds(%this)
{
   return %this.StartFunds;
}

/// <summary>
/// This function is used by the Level Tool to set the amount of money available to 
/// the player when the level starts.
/// </summary>
/// <param name="amount">The desired starting funds amount.</param>
function LevelAttributesBehavior::setStartFunds(%this, %amount)
{
   %this.StartFunds = %amount;
}

/// <summary>
/// This function is used to access the level funds during play.
/// </summary>
/// <return>Returns the player's current funds.</return>
function LevelAttributesBehavior::getFunds(%this)
{
   return %this.Funds;
}

/// <summary>
/// This function sets the level funds during play.
/// </summary>
/// <param name="amount">The desired funds value.</param>
function LevelAttributesBehavior::setFunds(%this, %amount)
{
   %this.Funds = %amount;
}
