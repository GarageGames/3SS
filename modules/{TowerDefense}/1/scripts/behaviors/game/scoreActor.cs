//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior handles actors that interact with the game's score.
/// </summary>
if (!isObject(ScoreActorBehavior))
{
   %template = new BehaviorTemplate(ScoreActorBehavior);
   
   %template.friendlyName = "Score Actor";
   %template.behaviorType = "Game";
   %template.description  = "Handle scoring when actor is killed";

   %template.addBehaviorField(PointValue, "Add this amount to score on death", int, 50);
   %template.addBehaviorField(CurrencyValue, "Add this amount of money to player on death", int , 50);
   %template.addBehaviorField(ScorekeeperObject, "The scene object that has the scorekeeping behavior on it.", object, "GlobalBehaviorObject", SceneObject);
}

/// <summary>
/// This function handles basic initialization tasks.
/// </summary>
/// <param name="scene">The scene with which the owner is associated.</param>
function ScoreActorBehavior::onAddToScene(%this, %scene)
{
   if (%this.PointValue < 1)
      %this.PointValue = 1;
   if (%this.PointValue > 1000)
      %this.PointValue = 1000;
   
   if (%this.CurrencyValue < 1)
      %this.CurrencyValue = 1;
   if (%this.CurrencyValue > 1000)
      %this.CurrencyValue = 1000;
}

/// <summary>
/// This function handles reporting the owner's death and updating the game score.
/// </summary>
function ScoreActorBehavior::score(%this)
{
   if (%this.scored)
      return;
   
   %this.scored = true;
   // call the scorekeeper and update score
   //echo(" -- ScoreActorBehavior::score() + " @ %this.PointValue @ " : " @ %this.CurrencyValue);
   %this.ScorekeeperObject.callOnBehaviors(reportEnemyDeath, %this.owner);
   %this.ScorekeeperObject.callOnBehaviors(updateScore, %this.PointValue, %this.CurrencyValue);
}

/// <summary>
/// This function is used by the Enemy Tool to access the actor's score value in points.
/// </summary>
/// <return>The point value of the actor.</return>
function ScoreActorBehavior::getScoreValue(%this)
{
   return %this.PointValue;
}

/// <summary>
/// This function is used by the Enemy Tool to set the actor's score value in points.
/// </summary>
/// <param name="scoreValue">The desired point value for this actor.</param>
function ScoreActorBehavior::setScoreValue(%this, %scoreValue)
{
   %this.PointValue = %scoreValue;
}

/// <summary>
/// This function is used by the Enemy Tool to access the actor's value in money.
/// </summary>
/// <return>The amount of money awarded for killing this actor.</return>
function ScoreActorBehavior::getFundsValue(%this)
{
   return %this.CurrencyValue;
}

/// <summary>
/// This function is used by the Enemy Tool to set the money value of the actor.
/// </summary>
/// <param name="fundsValue">The desired amount of money to be awarded for killing this actor.</param>
function ScoreActorBehavior::setFundsValue(%this, %fundsValue)
{
   %this.CurrencyValue = %fundsValue;
}

/// <summary>
/// This function resets the actor's scored flag.  The scored flag is used to prevent an actor from 
/// being scored more than once.
/// </summary>
function ScoreActorBehavior::resetScoreActor(%this)
{
   %this.scored = false;
}

/// <summary>
/// This function retrieves the assigned Scorekeeper object.
/// </summary>
function ScoreActorBehavior::getScorekeeper(%this)
{
    return %this.ScorekeeperObject;
}
