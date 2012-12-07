//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

%towerUpgradeAudioProfiles = "None" TAB "TowerUpgradeSound";

/// <summary>
/// This behavior allows tower objects to be upgraded for better versions.
/// </summary>
if (!isObject(TowerUpgradeBehavior))
{
   %template = new BehaviorTemplate(TowerUpgradeBehavior);
   
   %template.friendlyName = "Tower Upgrade";
   %template.behaviorType = "Game";
   %template.description  = "Specifies an Upgrade for the Tower";

   %template.addBehaviorField(upgrade, "The upgrade for the tower", object, "", SceneObject);
   %template.addBehaviorField(upgradeSound, "Sound profile played when the owner dies", enum, "", %towerUpgradeAudioProfiles);
}

/// <summary>
/// This function accesses the cost to upgrade to the next version.
/// </summary>
/// <return>Returns the monetary cost of the upgrade.</return>
function TowerUpgradeBehavior::getUpgradeCost(%this)
{
   %cost = %this.upgrade.callOnBehaviors(getCost);
   return %cost;
}

/// <summary>
/// This function returns the tower's upgrade availability.
/// </summary>
/// <return>Returns true because any tower that has this behavior can be upgraded.</return>
function TowerUpgradeBehavior::isUpgradeable(%this)
{
    return true;
}

/// <summary>
/// This function accesses the next tower in the upgrade chain.
/// </summary>
/// <return>Returns the name of the next tower in the upgrade chain.</return>
function TowerUpgradeBehavior::getUpgrade(%this)
{
   return %this.upgrade;
}

/// <summary>
/// This function accesses the audio profile of the sound to play when the 
/// tower is upgraded.
/// </summary>
/// <return>Returns the name of the upgrade sound audio profile.</return>
function TowerUpgradeBehavior::getUpgradeSound(%this)
{
   return %this.upgradeSound;
}