//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// The AppliesPiercingEffectBehavior is used to damage multiple enemies in the path of
/// the behavior owner.
/// </summary>
if (!isObject(AppliesPiercingEffectBehavior))
{
   %template = new BehaviorTemplate(AppliesPiercingEffectBehavior);
   
   %template.friendlyName = "Piercing";
   %template.behaviorType = "AttackEffect";
   %template.description  = "Applies damage to all TakesDamage objects it comes in contact with, with decreasing damage per hit.";

   %template.addBehaviorField(damageFalloff, "The percent damage is reduced for each object that is hit.", int, 25);
}

/// <summary>
/// This function handles basic setup - specifically, it clamps the damage falloff to a 
/// 0% to 100% range.
/// </summary>
function AppliesPiercingEffectBehavior::onBehaviorAdd(%this)
{
   if (%this.damageFalloff < 0)
      %this.damageFalloff = 0;
   if (%this.damageFalloff > 100)
      %this.damageFalloff = 100;
}

/// <summary>
/// This function is used to check effect behaviors to see if they apply a standing effect to
/// their targets.
/// </summary>
/// <return>Returns true if this behavior applies a lasting effect, or false if not.</return>
function AppliesPiercingEffectBehavior::getAppliesEffect(%this)
{
   // While this is a projectile effect, it doesn't actually apply a
   // standing effect on an enemy.
   return false;
}

/// <summary>
/// This function applies the damage effect to a target via the DealsDamageBehavior attached to 
/// the owner object.
/// </summary>
/// <param name="dealsDamageBehavior">The instance of the DealsDamageBehavior that is associated with this effect.</param>
/// <param name="victim">The effect target.</param>
function AppliesPiercingEffectBehavior::applyDamageEffect(%this, %dealsDamageBehavior, %victim)
{
   // Apply the damage
   %victim.callOnBehaviors("takeDamage", %this.nextDamage, %this.owner);
   %this.owner.callOnBehaviors("setFaceTarget", "");
   %this.owner.callOnBehaviors("setMoveTarget", "");
   
   // Reduce the next attack's damage
   %this.nextDamage = %this.nextDamage * ((100 - %this.damageFalloff) * 0.01);
   
   return true;
}

/// <summary>
/// This function accesses the damage that will be applied the next time it is triggered.
/// </summary>
/// <return>Returns the adjusted damage that will be applied the next time this effect is triggered.</return>
function AppliesPiercingEffectBehavior::getNextDamage(%this)
{
    return %this.nextDamage;
}

/// <summary>
/// This function is used by the Projectile Tool to access the damage falloff of this behavior.
/// </summary>
/// <return>Returns the damage falloff factor of this behavior.</return>
function AppliesPiercingEffectBehavior::getEffectDamageFalloff(%this)
{
   return %this.damageFalloff;
}

/// <summary>
/// This function is used by the Projectile Tool to set the damage falloff of this behavior.
/// </summary>
/// <param name="falloff">The falloff factor that will be multiplied against the base damage to get the adjusted damage for the effect.</param>
function AppliesPiercingEffectBehavior::setEffectDamageFalloff(%this, %falloff)
{
   %this.damageFalloff = %falloff;
}

/// <summary>
/// This function is used by the caching system to reset the effect's attributes when it 
/// is recycled to the cache
/// </summary>
function AppliesPiercingEffectBehavior::resetAttackEffect(%this)
{
   %this.nextDamage = %this.owner.callOnBehaviors("getAttack");
}
