//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$SlowEffectType = 1;

/// <summary>
/// The AppliesSlowEffectBehavior causes the owner of the behavior to move at a 
/// reduced speed while its duration lasts.
/// </summary>
if (!isObject(AppliesSlowEffectBehavior))
{
   %template = new BehaviorTemplate(AppliesSlowEffectBehavior);
   
   %template.friendlyName = "Slow";
   %template.behaviorType = "AttackEffect";
   %template.description  = "Set the object to apply a slow effect to TakesDamage objects it collides with";

   %template.addBehaviorField(speedModifier, "Percentage of base speed that the target will move while under this effect.", int, 50);
   %template.addBehaviorField(slowDuration, "Time that the slow lasts (in seconds)", int, 3);
   //%template.addBehaviorField(slowEffect, "The particle effect to play on the target when it is slowed", object, "", ParticleEffect);
}

/// <summary>
/// This function handles some basic initialization tasks.
/// </summary>
function AppliesSlowEffectBehavior::onBehaviorAdd(%this)
{
   if (%this.speedModifier < 1.0)
      %this.speedModifier = 1.0;
   if (%this.speedModifier > 100.0)
      %this.speedModifier = 100.0;
   
   if (%this.slowDuration < 1.0)
      %this.slowDuration = 1.0;
   if (%this.slowDuration > 1000.0)
      %this.slowDuration = 1000.0;
}

/// <summary>
/// This function is used to identify time based target effect behaviors
/// </summary>
/// <return>Returns true because this is a time based target effect behavior.</return>
function AppliesSlowEffectBehavior::getAppliesEffect(%this)
{
   return $SlowEffectType;
}

/// <summary>
/// This function applies the slow effect to its target.
/// </summary>
/// <param name="victim">The target object that will receive the effect.</param>
/// <param name="dealsDamageBehavior">The instance of the DealsDamageBehavior that carries the applied effect.</param>
function AppliesSlowEffectBehavior::applySlowEffect(%this, %victim, %dealsDamageBehavior)
{
    if (!isObject(%victim))
    return;

    %victim.callOnBehaviors("takeDamage", %dealsDamageBehavior.strength, %dealsDamageBehavior.owner);

    if (!isObject(%victim.EffectSet))
    {
        %victim.EffectSet = new SimSet();
    }
    // If already under a slow effect, determine if we need to refresh
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
                %existingEffect.removeSlowEffect(%victim);
                
            case 1:
                // the aggressor is of the same type and of the same or higher
                // upgrade level so remove the old effect.
                %existingEffect.removeSlowEffect(%victim);
                
            case 2:
                // the aggressor is of a different type or of a lower upgrade
                // level, so ignore this effect and return without applying it.
                %ignore = true;
        }

        if (%ignore)
            return;
    }

    // Make sure the speed modifier is valid
    %filteredSpeedModifier = %this.speedModifier / 100;

    if(%filteredSpeedModifier <= 0)
        %filteredSpeedModifier = 0.001;

    // Apply slow
    //%victim.setMaxLinearVelocity(%victim.moveSpeed * %filteredSpeedModifier);
    %speed = %victim.callOnBehaviors("getMoveSpeed");
    %speed *= %filteredSpeedModifier;
    %victim.callOnBehaviors("setMoveSpeed", %speed);
    
    // Tint blue
    %victim.setBlendColor(0.2, 1.0, 1.0);

    // Flip the under effect flag
    //%victim.underSlowEffect = true;

    //%victim.resetPath();

    // Allow for permanent slow effect if -1 used for duration
    if(%this.slowDuration < 0)
        return;

    // Schedule the slow  
    %slowDurationMillis = %this.slowDuration * 1000;
    %this.slowEffectSchedule = %this.schedule(%slowDurationMillis, "removeSlowEffect", %victim);
    %victim.EffectSet.add(%this);
}

/// <summary>
/// This function causes the effect to remove itself from the target.
/// </summary>
/// <param name="victim">The object that is currently carrying this effect.</param>
function AppliesSlowEffectBehavior::removeSlowEffect(%this, %victim)
{
    cancel(%this.slowEffectSchedule);

    //echo("Removing slow effect");
    
    // Check to see if there are any other slow effects on this target.  If so, do not reset 
    // target's speed before removal.
    %lastSlow = true;
    
    for (%i = 0; %i < %victim.EffectSet.getCount(); %i++)
    {
        %effect = %victim.EffectSet.getObject(%i);
        if (%effect != %this && %effect.template.getName() $= "AppliesSlowEffectBehavior")
            %lastSlow = false;
    }

    // Before we try to remove the effect, we will check to ensure that our
    // victim hasn't met an untimely demise and that this is the last instance of a 
    // slow effect on the victim.
    if (isObject(%victim) && %lastSlow)
    {
        %victim.setBlendColor(1, 1, 1);
        //%victim.setMaxLinearVelocity(%victim.moveSpeed);
        %victim.callOnBehaviors("resetMoveSpeed");
        %victim.underSlowEffect = false;
    }
    // remove this instance of the effect from the list.
    if (%victim.EffectSet.isMember(%this))
        %victim.EffectSet.remove(%this);
}

/// <summary>
/// This function applies any damage caused by the effect
/// </summary>
/// <param name="dealsDamageBehavior">The instance of the DealsDamageBehavior associated with the projectile that carried the effect.</param>
/// <param name="victim">The target of the effect.</param>
/// <param name="aggressor">The object that fire the projectile that carried the effect.</param>
function AppliesSlowEffectBehavior::applyDamageEffect(%this, %dealsDamageBehavior, %victim, %aggressor)
{
    %this.aggressor = %aggressor;
    %this.applySlowEffect(%victim, %dealsDamageBehavior);
    //%this.owner.safeDelete();
    //MainScene.returnProjectileToCache(%this.owner);
    return true;
}

/// <summary>
/// This function is used by the Tower Tool to access the effect's slow percentage.
/// </summary>
/// <return>Returns the percentage that the victim's speed will be reduced.</return>
function AppliesSlowEffectBehavior::getEffectSlowPercent(%this)
{
   return %this.speedModifier;
}

/// <summary>
/// This function is used by the Tower Tool to set the effect's slow percentage.
/// </summary>
/// <param name="percent">The percentage by which to slow a victim of this effect.</param>
function AppliesSlowEffectBehavior::setEffectSlowPercent(%this, %percent)
{
   %this.speedModifier = %percent;
}

/// <summary>
/// This function is used by the Tower Tool to access the effect's duration
/// </summary>
/// <return>The duration in seconds of the slow effect.</return>
function AppliesSlowEffectBehavior::getEffectSlowDuration(%this)
{
   return %this.slowDuration;
}

/// <summary>
/// This function is used by the Tower Tool to set the effect's duration
/// </summary>
/// <param name="duration">The duration in seconds that the effect will persist.</param>
function AppliesSlowEffectBehavior::setEffectSlowDuration(%this, %duration)
{
   %this.slowDuration = %duration;
}

/// <summary>
/// This function sorts tower names by tower type and upgrade level.
/// </summary>
/// <param name="name">The name of the originator of the existing effect.</param>
/// <param name="name2">The name of the originator of the new effect that we're attempting to apply.</param>
function compareTowerNames(%name, %name2)
{
    %type = getSubStr(%name, 0, 6);
    %subType = getSubStr(%name, 6, 8);
    
    %type2 = getSubStr(%name2, 0, 6);
    %subType2 = getSubStr(%name2, 6, 8);
    
    %subtypeVal = 0;
    
    %subtypeVal2 = 0;
    
    if (%type $= %type2)
    {
        %sub = stripChars(%subType, "Upgrade");
        %sub2 = stripChars(%subType2, "Upgrade");
        
        %subtypeVal = %sub + 1;
        %subtypeVal2 = %sub2 + 1;
        
        // if the new effect is better than the old, return 0
        if (%subtypeVal < %subtypeVal2)
            return 0;
            
        // if the new effect is equal to the old, return 1
        if (%subtypeVal == %subtypeVal2)
            return 1;
    }
    
    // If the new effect is worse, return 2
    return 2;
}