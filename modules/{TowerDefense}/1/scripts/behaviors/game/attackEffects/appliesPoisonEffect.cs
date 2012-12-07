//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$PoisonEffectType = 2;

/// <summary>
/// This behavior handles a 'poison' damage over time effect.
/// </summary>
if (!isObject(AppliesPoisonEffectBehavior))
{
   %template = new BehaviorTemplate(AppliesPoisonEffectBehavior);
   
   %template.friendlyName = "Poison";
   %template.behaviorType = "AttackEffect";
   %template.description  = "Set the object to apply a poison effect to TakesDamage objects it collides with";

   %template.addBehaviorField(poisonDuration, "The time that the poison effect lasts (in seconds).", int, 3);
   %template.addBehaviorField(damagePerSecond, "The damage done to the afflicted target every second.", int, 5);
}

/// <summary>
/// This function performs basic initialization.
/// </summary>
function AppliesPoisonEffectBehavior::onBehaviorAdd(%this)
{
    if (%this.poisonDuration < 1.0)
        %this.poisonDuration = 1.0;
    if (%this.poisonDuration > 1000.0)
        %this.poisonDuration = 1000.0;

    if (%this.damagePerSecond < 1)
        %this.damagePerSecond = 1;
    if (%this.damagePerSecond > 1000)
        %this.damagePerSecond = 1000;
}

/// <summary>
/// This function returns true to indicate that this behavior applies a time 
/// based target effect.
/// </summary>
/// <return>Returns true because it applies a time based target effect.</return>
function AppliesPoisonEffectBehavior::getAppliesEffect(%this)
{
   return $PoisonEffectType;
}

/// <summary>
/// This function starts the poison effect if it is equal to or stronger than any 
/// poison effect that is on the same target.  Note that this makes certain assumptions, 
/// primarily that no other objects apply the same effect.
/// </summary>
/// <param name="victim">The ID of the target to apply the poison effect to.</param>
/// <param name="damagePerSecond">The damage per second that the poison will do to the victim.</param>
function AppliesPoisonEffectBehavior::applyPoisonEffect(%this, %victim, %dealsDamageBehavior)
{
    if (!isObject(%victim))
        return;

    %victim.callOnBehaviors("takeDamage", %dealsDamageBehavior.strength, %dealsDamageBehavior.owner);

    // If already under the effect cancel any pending damage
    if (!isObject(%victim.EffectSet))
    {
        %victim.EffectSet = new SimSet();
    }
    // If already under a poison effect, determine if we need to refresh
    // or override it.
    
    // We know the SimObject names of all of the aggressors, so if the name of this 
    // effect's aggressor is the same as one in the list, then we cancel that effect and
    // replace it with the newer one.  If the name of this aggressor is of a higher 
    // upgrade level then we cancel the current version and replace it with this one.
    // If the name of this aggressor is of a lower level than the existing one then this
    // one is discarded.

    %newName = %this.aggressor.slot;
    %ignore = false;
    for (%i = 0; %i < %victim.EffectSet.getCount(); %i++)
    {
        %existingEffect = %victim.EffectSet.getObject(%i);
        %existingName = %existingEffect.aggressor.slot;
        
        switch(compareTowerNames(%existingName, %newName))
        {
            case 0:
                // the aggressor is of the same type and of the same or higher
                // upgrade level so remove the old effect.
                %existingEffect.removePoisonEffect(%victim);
                
            case 1:
                // the aggressor is of the same type and of the same or higher
                // upgrade level so remove the old effect.
                %existingEffect.removePoisonEffect(%victim);
                
            case 2:
                // the aggressor is of a different type or of a lower upgrade
                // level, so ignore this effect and return without applying it.
                %ignore = true;
        }

        if (%ignore)
            return;
    }
    
    // Apply poison
    %victim.setBlendColor(0.2, 1, 0.2);
    
    // Set up fake ID base - randomize to avoid collision with other poison effects
    %this.fakeID = %this.getId() + getRandom(%this.poisonDuration, (%this.poisonDuration * 10));
    // Schedule the poison
    %this.causePoisonDamage(%victim, %this.damagePerSecond, %this.poisonDuration);
    %victim.EffectSet.add(%this);
}

/// <summary>
/// This function schedules a damage function and then reschedules itself for its duration.
/// </summary>
/// <param name="victim">The target of the damage.</param>
/// <param name="dps">The damage per second to apply to the victim.</param>
/// <param name="timeleft">The number of seconds remaining in the effect.</param>
function AppliesPoisonEffectBehavior::causePoisonDamage(%this, %victim, %dps, %timeleft)
{
    if (isObject(%victim))
    {
        %this.schedule(1000, "damageVictim", %victim, %dps);
        %this.fakeID++;
        %timeleft--;
        if (%timeleft > 0)
        {
            %this.poisonEffectSchedule = %this.schedule(1000, "causePoisonDamage", %victim, %dps, %timeleft);
        }
        else
        {
            %this.removePoisonEffect(%victim);
        }
    }
}

/// <summary>
/// This function applies the poison damage to the victim.
/// </summary>
/// <param name="victim">The target of the damage.</param>
/// <param name="dps">The damage to apply to the victim.</param>
/// <param name="timeleft">The remaining time in the effect.</param>
function AppliesPoisonEffectBehavior::damageVictim(%this, %victim, %dps)
{
    // Use %this.fakeID instead of %this or %this.owner for the damage source, otherwise
    // the victim can only take damage from this method once because the TakesDamage
    // behavior won't let anything take damage from the same source repeatedly.
    %victim.callOnBehaviors("takeDamage", %dps, %this.fakeID);
}

/// <summary>
/// This function removes the poison effect from the victim.
/// </summary>
/// <param name="victim">The ID of the victim to remove the effect from.</param>
function AppliesPoisonEffectBehavior::removePoisonEffect(%this, %victim)
{
    //echo("Removing poison effect");

    cancel(%this.poisonEffectSchedule);
    // Before we try to remove the effect, we will check to ensure that our
    // victim hasn't met an untimely demise.
    if (isObject(%victim))
    {
        %victim.setBlendColor(1, 1, 1);
        %victim.underPoisonEffect = false;
        %victim.schedule(0, removeBehavior, %this, false);
        if (%victim.EffectSet.isMember(%this))
            %victim.EffectSet.remove(%this);
    }
}

/// <summary>
/// This function is called by the DealsDamageBehavior in its dealDamage() method.  It is used
/// to pass the effect originator to the effect for use in determining if the incoming effect is 
/// more powerful than an effect already on the victim.
/// </summary>
/// <param name="dealsDamageBehavior">The DealsDamageBehavior instance associated with this event.</param>
/// <param name="victim">The target of the poison effect.</param>
/// <param name="aggressor">The tower from which this effect originated.</param>
function AppliesPoisonEffectBehavior::applyDamageEffect(%this, %dealsDamageBehavior, %victim, %aggressor)
{
    %this.aggressor = %aggressor;
    %this.applyPoisonEffect(%victim, %dealsDamageBehavior);
    return true;
}

/// <summary>
/// This function is used by the Projectile Tool to access the effect's duration.
/// </summary>
/// <return>The duration of the effect in seconds.</param>
function AppliesPoisonEffectBehavior::getEffectPoisonDuration(%this)
{
   return %this.poisonDuration;
}

/// <summary>
/// This function is used by the Projectile Tool to set the effect's duration.
/// </summary>
/// <param name="duration">The desired effect duration in seconds.</param>
function AppliesPoisonEffectBehavior::setEffectPoisonDuration(%this, %duration)
{
   %this.poisonDuration = %duration;
}

/// <summary>
/// This function is used by the Projectile Tool to access the effect's damage per second.
/// </summary>
/// <return>The effect's damage per second.</param>
function AppliesPoisonEffectBehavior::getEffectPoisonDamagePerSecond(%this)
{
    return %this.damagePerSecond;
}

/// <summary>
/// This function is used by the Projectile Tool to set the effect's damage per second.
/// </summary>
/// <param name="damage">The effect's desired damage per second.</param>
function AppliesPoisonEffectBehavior::setEffectPoisonDamagePerSecond(%this, %damage)
{
    %this.damagePerSecond = %damage;
}