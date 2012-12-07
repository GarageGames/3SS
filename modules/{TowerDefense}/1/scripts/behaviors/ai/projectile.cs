//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior moves a projectile toward a target.  It also handles playing the 
/// projectile's travel and hit animations and limiting the projectile's range if needed.
/// </summary>
if (!isObject(MoveProjectileBehavior))
{
   %template = new BehaviorTemplate(MoveProjectileBehavior);
   
   %template.friendlyName = "Move Projectile";
   %template.behaviorType = "AI";
   %template.description  = "Set a projectile to move toward a target";

   %template.addBehaviorField(travelAnim, "The object's attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(cycleAnim, "If false the animation will play once scaled to fit travel time.", bool, 1);
   %template.addBehaviorField(trackTarget, "Projectile follows the target's motion", bool, 1);
   %template.addBehaviorField(limitRange, "Delete the projectile at range limit", bool, 1);
   %template.addBehaviorField(continueWithoutTarget, "Should the projectile continue its course without a target", bool, 0);
}

/// <summary>
/// This function performs basic initialization tasks.
/// </summary>
function MoveProjectileBehavior::onAddToScene(%this)
{
   %this.ready = false;
   %this.setupProperties();
}

/// <summary>
/// This function performs more complex initialization tasks.
/// </summary>
/// <param name="target">The ID of the projectile's target object.</param>
/// <param name="speed">The speed at which the projectile should move.</param>
/// <param name="range">The maximum range of the projectile.</param>
/// <param name="travelAnim">The name of the animation to play while the projectile is in flight.</param>
function MoveProjectileBehavior::setupProjectile(%this, %target, %speed, %range, %travelAnim)
{
    %this.setMoveTarget(%target);
    %this.speed = %speed;
    %this.range = %range;
    %this.setTravelAnim(%travelAnim);
    %this.owner.callOnBehaviors("setFaceTarget", %target);

    //%this.updateEvent = %this.schedule(150, updateMoveProjectile);
    %this.updateMoveProjectile();

    %splashProjectile = %this.owner.callOnBehaviors("getSplashEffect");
    if (%splashProjectile == true)
    {
        %this.splashProjectile = true;
    }
}

/// <summary>
/// This function sets the method for animating the projectile's travel animation.  If set to true and the
/// projectile's travel animation is not looping, the travel animation of the projectile will have its speed
/// scaled to play once in its entirety during the projectile's flight time.  Otherwise, it will simply play 
/// the travel animation at its normal speed.
/// </summary>
/// <param name="flag">True to scale the animation for travel time, false to play the animation at its normal speed.</param>
function MoveProjectileBehavior::setTravelAnim(%this, %flag)
{
   if (%flag == true)
   {
      if (isObject(%this.travelAnim) && %this.cycleAnim == false)
      {
         %this.owner.setAnimation(%this.travelAnim);
         %this.owner.setSpeedFactor(%this.animScale);
         %this.owner.playAnimation(%this.travelAnim, false, 0);
      }
   }
}

/// <summary>
/// This function sets the travel animation playback scale so that it will 
/// play back within the projectile's calculated travel time.
/// </summary>
function  MoveProjectileBehavior::setAnimScale(%this)
{
   %distance = VectorDist(%this.owner.startPosition, %this.target.position);
   %anim = %this.travelAnim;
   %frames = getFieldCount(%this.travelAnim.animationFrames);
   %frameTime = %this.travelAnim.animationTime;
   %animTime = %frames * %frameTime;
   %travelTime = %distance / %this.speed;
   %this.animScale = %animTime / %travelTime;
}

/// <summary>
/// This function updates the projectile's move state.
/// </summary>
function MoveProjectileBehavior::updateMoveProjectile(%this)
{
   if (!%this.ready)
      return;
      
   if (%this.owner.preFire)
   {
      %this.owner.startPosition = %this.owner.position;
      %this.owner.preFire = false;
      if (%this.cycleAnim == false)
      {
         %this.setAnimScale();
         %this.owner.setSpeedFactor(%this.animScale);
      }
   }
   
   if (%this.limitRange)
   {
      %traveled = VectorDist(%this.owner.startPosition, %this.owner.position);
      if (%traveled > %this.range)
      {
         //%this.owner.safeDelete();
         MainScene.returnProjectileToCache(%this.owner);
         return;
      }
   }
   
   // check for 'live' target.
   if (!isObject(%this.target))
   {
      if (%this.owner.removeWithTarget)
      {
         if (%this.owner.MoveProjectileBehaviorPreUpdate)
         {
            // didn't even get the first update
            //%this.owner.safeDelete();
            MainScene.returnProjectileToCache(%this.owner);
            return;
         }
         else
         {
            %this.updateEvent = %this.schedule(50, updateMoveTowardLast);
         }
      }
         
      return;
   }
   
   if (%this.target.dead)
   {
      // Target is valid, but dead.  If we're to continue without a target
      // then move but clear our target.
      if (%this.continueWithoutTarget)
      {
         %this.owner.callOnBehaviors("setFaceTarget", "");
         %this.owner.callOnBehaviors("setMoveTarget", "");

         %time = Vector2Distance(%this.owner.lastTargetPosition, %this.owner.position) / %this.speed * 1000;
         %this.owner.moveTo(%this.owner.lastTargetPosition, %time, false);
         
         return;
      }
   }
   
   // save last known good position in case target is removed.
   %this.owner.lastTargetPosition = %this.target.getPosition();
   %this.owner.MoveProjectileBehaviorPreUpdate = false;
   
   %time = Vector2Distance(%this.owner.lastTargetPosition, %this.owner.position) / %this.speed * 1000;
   %this.owner.moveTo(%this.owner.lastTargetPosition, %time, false);
   
   if (%this.trackTarget)
      %this.updateEvent = %this.schedule(50, updateMoveProjectile);
}

/// 
/// <summary>
/// This function moves the projectile towards the last known good target position.
/// </summary>
function MoveProjectileBehavior::updateMoveTowardLast(%this)
{
    if (!%this.ready)
        return;

    if (%this.limitRange)
    {
        %traveled = VectorDist(%this.owner.startPosition, %this.owner.position);
        if (%traveled > %this.range)
        {
            //%this.owner.safeDelete();
            MainScene.returnProjectileToCache(%this.owner);
            return;
        }
    }

    if (!isObject(%this.target) && !%this.continueWithoutTarget)
    {
        %time = Vector2Distance(%this.owner.lastTargetPosition, %this.owner.position) / %this.speed * 1000;
        %this.owner.moveTo(%this.owner.lastTargetPosition, %time, false);
    }

    if (%this.splashProjectile)
    {
        %distanceToTarget = Vector2Distance(%this.owner.lastTargetPosition, %this.owner.position);
        if (%distanceToTarget < 0.25)
            %this.onPositionTarget();

        %time = Vector2Distance(%this.owner.lastTargetPosition, %this.owner.position) / %this.speed * 1000;
        %this.owner.moveTo(%this.owner.lastTargetPosition, %time, false);
    }
    %this.updateEvent = %this.schedule(50, updateMoveTowardLast);
}

/// <summary>
/// This function sets the projectile's target object.
/// </summary>
/// <param name="target">The ID of the target object.</param>
function MoveProjectileBehavior::setMoveTarget(%this, %target)
{
   %this.target = %target;
   if (isObject(%target))
   {
      %this.owner.lastTargetPosition = %target.getPosition();
   }
}

/// <summary>
/// This function handles the projectile's behavior when it reaches its target position 
/// if its target no longer exists.
/// </summary>
function MoveProjectileBehavior::onPositionTarget(%this)
{
    // applyDamageEffect(%this, %dealsDamageBehavior, %victim)
    %damageBehavior = %this.owner.getBehavior("DealsDamageBehavior");
    if (%this.splashProjectile)
        %this.callOnBehaviors("applySplashEffect");
        
    if (!%this.continueWithoutTarget)
        MainScene.returnProjectileToCache(%this.owner);
}

/// <summary>
/// This function accesses the projectile's current target
/// </summary>
/// <return>The current target of the projectile.</param>
function MoveProjectileBehavior::getMoveTarget(%this)
{
    return %this.target;
}

/// <summary>
/// This function is used by the projectile cache to disable and clean up the 
/// projectile before returning it to the cache.
/// </summary>
function MoveProjectileBehavior::disableMoveProjectile(%this)
{
   %this.ready = false;
   
   %this.target = "";
   %this.range = "";
   %this.speed = "";
   %this.animScale = "";
   
   // DAW: Not available in Box2D
   //%this.owner.setPositionTargetOff();
   %this.owner.cancelMoveTo();
   cancel(%this.updateEvent);
}

/// <summary>
/// This function is used by the cache system to initialize the projectile for use
/// upon retrieval from the cache.
/// </summary>
function MoveProjectileBehavior::setupProperties(%this)
{
   %this.target = "";
   %this.range = "";
   %this.speed = "";
   %this.animScale = "";
   
   %this.owner.MoveProjectileBehaviorPreUpdate = true;
   %this.owner.preFire = true;
   
   %this.ready = true;
}

/// <summary>
/// This function is used to prepare the projectile for use.
/// </summary>
function MoveProjectileBehavior::enableMoveProjectile(%this)
{
   %this.setupProperties();
   //%this.updateEvent = %this.schedule(150, updateMoveProjectile);
}

/// <summary>
/// This function sets the maximum travel range of the projectile.
/// </summary>
/// <param name="limitRange">The maximum distance in world units that the projectile will travel before being destroyed.</param>
function MoveProjectileBehavior::setLimitRange(%this, %limitRange)
{
   %this.limitRange = %limitRange;
}

/// <summary>
/// This function sets a flag that allows the projectile to move in its last direction until it hits something even without 
/// an assigned target.
/// </summary>
/// <param name="continueWithoutTarget">True to allow the projectile to continue moving, false to delete projectile upon reaching target destination.</param>
function MoveProjectileBehavior::setContinueWithoutTarget(%this, %continueWithoutTarget)
{
   %this.continueWithoutTarget = %continueWithoutTarget;
}