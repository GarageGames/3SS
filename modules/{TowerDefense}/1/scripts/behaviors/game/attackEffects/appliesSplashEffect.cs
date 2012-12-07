//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$minSplashDamage = 0;
$maxSplashDamage = 1000;
$minSplashRadius = 1.0;
$maxSplashRadius = 1000.0;

/// <summary>
/// This behavior allows projectiles to cause area damage.
/// </summary>
if (!isObject(AppliesSplashEffectBehavior))
{
   %template = new BehaviorTemplate(AppliesSplashEffectBehavior);
   
   %template.friendlyName = "Splash";
   %template.behaviorType = "AttackEffect";
   %template.description  = "Applies damage over an area to all TakesDamage objects";

   %template.addBehaviorField(splashRadius, "The radius for the splash damage (in squares)", int, 1);
   %template.addBehaviorField(splashDamage, "The amount of damage applied to those caught in the radius", int, 10);
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function AppliesSplashEffectBehavior::onBehaviorAdd(%this)
{
    if (%this.splashDamage < $minSplashDamage)
        %this.splashDamage = $minSplashDamage;
    if (%this.splashDamage > $maxSplashDamage)
        %this.splashDamage = $maxSplashDamage;

    if (%this.splashRadius < $minSplashRadius)
        %this.splashRadius = $minSplashRadius;
    if (%this.splashRadius > $maxSplashRadius)
        %this.splashRadius = $maxSplashRadius;
}

/// <summary>
/// This function returns false because it does not apply a time based target effect.
/// </summary>
/// <return>Returns false because it does not apply a time based target effect.</return>
function AppliesSplashEffectBehavior::getAppliesEffect(%this)
{
   // While this is a projectile effect, it doesn't actually apply a
   // standing effect on an enemy.
   return false;
}

/// <summary>
/// This function returns true because it applies an area effect.
/// </summary>
/// <return>Returns true because it applies an area effect.</return>
function AppliesSplashEffectBehavior::getSplashEffect(%this)
{
    // allow projectile setup to poll for splash damage effect
    return true;
}

/// <summary>
/// This function applies splash area damage to all targets in its area of effect.
/// </summary>
/// <param name="dealsDamageBehavior">The DealsDamageBehavior associated with this event.</param>
/// <param name="victim">The target of the attack.</param>
function AppliesSplashEffectBehavior::applyDamageEffect(%this, %dealsDamageBehavior, %victim)
{
    // Our radius is based on the X square size of the tower path
    %radius = TowerPlacementGrid.getSizeX() / TowerPlacementGrid.getCellCountX() * %this.splashRadius;

    // Amount of damage comes from the DealsDamageBehavior   
    %damage = %dealsDamageBehavior.strength;

    // Amount of damage to objects caught in the area of effect
    %aoeDamage = %this.splashDamage;

    // Set our position to match the target
    if (isObject(%victim))
    {
        %projectilePos = %victim.getPosition();
        %this.owner.setPosition(%victim.getPositionX(), %victim.getPositionY());
    }
    else
        %projectilePos = %this.owner.getPosition();

    // Set our size to match the splash size
    %newsize = %radius * 2.0;
    %this.owner.setSize(%newsize, %newsize);

    // Apply damage to the one hit
    if (isObject(%victim))
        %victim.callOnBehaviors("takeDamage", %damage, %this.owner);

    // Apply the damage
    %count = TowerTargetSet.getCount();
    for (%i=0; %i<%count; %i++)
    {
        %obj = TowerTargetSet.getObject(%i);
        if (%obj != %victim)
        {
            %dist = Vector2Distance(%obj.getPosition(), %projectilePos);
            if (%dist <= %radius)
            {
                %obj.callOnBehaviors("takeDamage", %aoeDamage, %this.owner);
            }
        }
    }

    return true;
}

/// <summary>
/// This function is used by the Projectile Tool to access the damage radius.
/// </summary>
/// <return>The damage radius in meters.</return>
function AppliesSplashEffectBehavior::getEffectSplashRadius(%this)
{
   return %this.splashRadius;
}

/// <summary>
/// This function is used by the Projectile Tool to set the damage radius.
/// </summary>
/// <param name="radius">The desired effect radius in meters.</param>
function AppliesSplashEffectBehavior::setEffectSplashRadius(%this, %radius)
{
   %this.splashRadius = %radius;
}

/// <summary>
/// This function is used by the Projectile Tool to access the effect's area damage.
/// </summary>
/// <return>Returns the area damage of the effect.</param>
function AppliesSplashEffectBehavior::getEffectSplashDamage(%this)
{
   return %this.splashDamage;
}

/// <summary>
/// This function is used by the Projectile Tool to set the effect's area damage.
/// </summary>
/// <param name="damage">The effect's desired area damage.</param>
function AppliesSplashEffectBehavior::setEffectSplashDamage(%this, %damage)
{
   %this.splashDamage = %damage;
}