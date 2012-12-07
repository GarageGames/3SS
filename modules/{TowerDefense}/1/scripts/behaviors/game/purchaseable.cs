//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior allows the owner to be bought or sold by the player.
/// </summary>
if (!isObject(PurchaseableBehavior))
{
   %template = new BehaviorTemplate(PurchaseableBehavior);
   
   %template.friendlyName = "Purchaseable";
   %template.behaviorType = "Game";
   %template.description  = "This object can be bought using player's current currency";

   %template.addBehaviorField(Cost, "Take this amount of money from the player when bought", int , 0);
   %template.addBehaviorField(Value, "Give this percentage of Cost to the player when sold", int , 50);
   %template.addBehaviorField(ScorekeeperObject, "The scene object that has the scorekeeping behavior on it.", object, "GlobalBehaviorObject", SceneObject);
}

/// <summary>
/// This function handles basic initialization tasks.
/// </summary>
function PurchaseableBehavior::onBehaviorAdd(%this)
{
   if (%this.Cost < 0)
      %this.Cost = 0;
   if (%this.Cost > 1000)
      %this.Cost = 1000;
      
   if (%this.Value < 0)
      %this.Value = 0;
   if (%this.Value > 200)
      %this.Value = 200;
}

/// <summary>
/// This function handles purchase of the object by telling the Scorekeeper to 
/// update the player's funds.
/// </summary>
function PurchaseableBehavior::buy(%this)
{
   // call the scorekeeper and update money
   //echo(" -- PurchaseableBehavior::buy() : " @ %this.Cost);
   %cost = -1 * %this.Cost;
   %this.ScorekeeperObject.callOnBehaviors(updateScore, 0, %cost);
}

/// <summary>
/// This function handles sale of the object by telling the Scorekeeper to 
/// update the player's funds.
/// </summary>
function PurchaseableBehavior::sell(%this)
{
   // call the scorekeeper and update money
   %cash = mFloor(%this.Cost * (%this.Value / 100));
   //echo(" -- PurchaseableBehavior::sell() : " @ %cash);
   %this.ScorekeeperObject.callOnBehaviors(updateScore, 0, %cash);
}

/// <summary>
/// This function is used to access the owner's purchase price.
/// </summary>
/// <return>The purchase price of the item.</return>
function PurchaseableBehavior::getCost(%this)
{
   return %this.Cost;
}

/// <summary>
/// This function is used by the Tower Tool to set the item's purchase price.
/// </summary>
/// <param name="cost">The desired item purchase price.</param>
function PurchaseableBehavior::setCost(%this, %cost)
{
   %this.Cost = %cost;
}

/// <summary>
/// This function gets the sale value of the item.
/// </summary>
/// <return>The amount that the item can be sold for.</return>
function PurchaseableBehavior::getSellValue(%this)
{
   return %this.Value;
}

/// <summary>
/// This function is used by the Tower Tool to set the sale 
/// value of the item.
/// </summary>
/// <param name="sellValue">The desired amount that the item can be sold for.</param>
function PurchaseableBehavior::setSellValue(%this, %sellValue)
{
   %this.Value = %sellValue;
}