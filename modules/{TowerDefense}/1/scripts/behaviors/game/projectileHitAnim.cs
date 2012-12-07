//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior handles playing a hit animation for projectiles that 
/// strike their target.
/// </summary>
if (!isObject(ProjectileHitAnimBehavior))
{
   %template = new BehaviorTemplate(ProjectileHitAnimBehavior);
   
   %template.friendlyName = "Projectile Hit Animation";
   %template.behaviorType = "Game";
   %template.description  = "Used on animated sprites that represent a hit on an enemy";
}

/// <summary>
/// This function plays the assigned animation.  This animation should NOT be looping.
/// </summary>
/// <param name="animation">The desired hit animation.</param>
function ProjectileHitAnimBehavior::playHitAnimation(%this, %animation)
{
      %this.owner.playAnimation(%animation);
}

/// <summary>
/// This function handles cleanup after the animation plays.
/// </summary>
function ProjectileHitAnimBehavior::onAnimationEnd(%this)
{
   %this.cleanup();
}

/// <summary>
/// This function handles returning the projectile to the cache.
/// </summary>
function ProjectileHitAnimBehavior::cleanup(%this)
{
   MainScene.returnProjectileToCache(%this.owner);
}
