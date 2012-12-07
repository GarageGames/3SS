//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// <summary>
/// DealsDamageBehavior allows an object to deal damage to objects that have the 
/// TakesDamageBehavior assigned to them.
/// </summary>
if (!isObject(DealsDamageBehavior))
{
   %template = new BehaviorTemplate(DealsDamageBehavior);
   
   %template.friendlyName = "Deals Damage";
   %template.behaviorType = "Game";
   %template.description  = "Set the object to deal damage to TakesDamage objects it collides with";

   %template.addBehaviorField(strength, "The amount of damage the object deals", int, 10);
   %template.addBehaviorField(deleteOnHit, "Delete the object when it collides", bool, 0);
   %template.addBehaviorField(deleteOnDealDamage, "Delete the object when it deals damage", bool, 1);
   %template.addBehaviorField(attackAnim, "The object's attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(explodeEffect, "The particle effect to play on collision", object, "", ParticleEffect);
   %template.addBehaviorField(hitAllTowerEnemies, "Hit all tower enemies once that we come across", bool, 0);
}

/// <summary>
/// This function handles some basic initialization tasks.
/// </summary>
function DealsDamageBehavior::onBehaviorAdd(%this)
{
   if($LevelEditorActive)
      %this.owner.updateCallback = false;
      
   if (%this.strength < 0)
      %this.strength = 0;
   if (%this.strength > 1000)
      %this.strength = 1000;
      
   %this.owner.collisionCallback = false;
   %this.owner.collisionActiveSend = false;
   %this.owner.setCollisionSuppress(true);
   
   %this.pickRadius = %this.owner.getSizeX() * 0.125;
   
   %this.checkForHitFreq = 64; // ms

   %this.ready = false;
}

/// <summary>
/// This function polls to see if we're in hit range of anything and then tells us 
/// to try to deal damage to anything we've hit.
/// </summary>
function DealsDamageBehavior::checkForHit(%this)
{
   if (!%this.ready)
      return;
      
   if(MainScene.getScenePause())
   {
      // Paused so skip the logic and reschedule
      %this.rescheduleHitCheck();
      return;
   }
   
   if (%this.hitAllTowerEnemies)
   {
      %pos = %this.owner.getPosition();
      %pickList = MainScene.pickArea(%pos.x-%this.pickRadius, %pos.y+%this.pickRadius, %pos.x+%this.pickRadius, %pos.y-%this.pickRadius, MainScene.validEnemyGroupMask);
      %count = getWordCount(%pickList);
      for (%i=0; %i<%count; %i++)
      {
         %obj = getWord(%pickList, %i);
         if (!%this.hasEnemyAlreadyBeenHit(%obj))
         {
            %this.dealDamage(%obj);
            %this.enemyHitList = setWord(%this.enemyHitList, getWordCount(%this.enemyHitList), %obj);
         }
      }
   }
   else
   {
      %target = %this.owner.callOnBehaviors(getMoveTarget);
      if (isObject(%target))
      {
         %distance = Vector2Distance(%this.owner.position, %target.position);
         if (%distance < (%target.getSize() / 2) && %this.hit == false)
         {
            %this.hit = true;
            %this.dealDamage(%target);
         }
      }
   }

   %this.rescheduleHitCheck();
}

/// <summary>
/// This function tests that this damage dealer has not already done damage to the current
/// target yet.
/// </summary>
/// <param name="enemy">The object to test against.</param>
/// <return>Returns true if this damage dealer has damaged this enemy, or false if not.</return>
function DealsDamageBehavior::hasEnemyAlreadyBeenHit(%this, %enemy)
{
   %count = getWordCount(%this.enemyHitList);
   for (%i=0; %i<%count; %i++)
   {
      if (getWord(%this.enemyHitList, %i) $= %enemy)
         return true;
   }
   
   return false;
}

/// <summary>
/// This function attempts to deal damage to the target victim.  It also keeps track of whom
/// we've already hit and takes care of cleaning itself up as needed.
/// </summary>
/// <param name="victim">The object to attempt to damage.</param>
function DealsDamageBehavior::dealDamage(%this, %victim)
{
   //Check if an effect should be applied
   %wasHandled = %this.owner.callOnBehaviors("applyDamageEffect", %this, %victim, %this.owner.aggressor);
   if (%wasHandled == false || %wasHandled $= "ERR_CALL_NOT_HANDLED")
   {
      // No applied effect so attempt to apply the damage
      %wasHandled = %victim.callOnBehaviors("takeDamage", %this.strength, %this.owner);
   }
   //echo(" ** DealsDamageBehavior::dealDamage() was handled? " @ %wasHandled);
   
   %this.originalAnim = %this.owner.getAnimation();
   
   if (%this.hitAllTowerEnemies)
   {
      // Rather than play a hit animation, we spawn a hit sprite and continue on.  But only
      // if there is a hit animation sequence to play
      %animSet = %this.owner.animationSet;
      
      if (isObject(%animSet.HitAnim))
      {
         %hitanim = MainScene.getProjectileFromCache("ProjectileHitAnim");
         if (%hitanim != -1)
         {
            // We have a valid hit anim sprite to work with.  If we didn't then
            // don't do a thing.
            %hitanim.setPosition(%this.owner.getPosition());
            %hitanim.setLinearVelocity(0, 0);
            %hitanim.setSceneLayer(%this.owner.getSceneLayer());
            %hitanim.playAnimation(%animSet.HitAnim);
         }
      }
   }
   else
   {
      if (%this.owner.Type $= "Projectile" && isObject(%this.owner.animationSet))
      {
         //echo(" -- got animation set for : " @ %this);
         %animSet = %this.owner.animationSet;
         if (isObject(%animSet.HitAnim))
         {
            //echo(" -- got hit animation " @ %this.owner.animationSet.HitAnim @ " for " @ %this);
            %this.attackAnim = %animSet.HitAnim;
         }
      }

      if (isObject(%this.attackAnim) && %this.owner.Type $= "Projectile")
      {
         //echo(" -- playing hit animation : " @ %this);
         %this.owner.callOnBehaviors("setFaceTarget", "");
   //      %this.owner.setCollisionSuppress(true);
         //%this.owner.setSize(32, 32);
         %this.owner.setSpeedFactor(1);
         %this.owner.setAngle(0);
         %this.owner.playAnimation(%this.attackAnim);
         %this.owner.callOnBehaviors("disableMoveProjectile");
         cancel(%this.checkForHitSchedule);
      }
   }
   
   // If we're deleting on damage and we actually have a damageable object collision...
   if (%this.deleteOnHit && !isObject(%this.attackAnim))
   {
      %this.cleanup();
   }

   if (%this.deleteOnDealDamage && (%wasHandled != false && %wasHandled !$= "ERR_CALL_NOT_HANDLED") && !isObject(%this.attackAnim))
   {
      %this.cleanup();
   }
   %nextDamage = %this.owner.callOnBehaviors("getNextDamage");
   if (%nextDamage !$= "ERR_CALL_NOT_HANDLED" && %nextDamage < 1)
   {
       %this.cleanup();
   }
}

/// <summary>
/// This function handles cleaning the damage dealer up if it needs to be destroyed upon 
/// dealing damage or other circumstance.
/// </summary>
function DealsDamageBehavior::cleanup(%this)
{
   %this.explode(getWords(%contacts, 0, 1));
   %direction = %this.owner.callOnBehaviors("getDirection");
   %cannotDie = %this.owner.callOnBehaviors("die", %direction, true, "DealtDamage");
   if (%cannotDie $= "ERR_CALL_NOT_HANDLED")
   {
      if (%this.owner.Type $= "Projectile")
      {
         MainScene.returnProjectileToCache(%this.owner);
      }
      else
      {
         %this.owner.safeDelete();
      }
   }
}

/// <summary>
/// The onAnimationEnd callback is used to handle cleaning up objects that are destroyed 
/// after playing an attack or hit animation.
/// </summary>
function DealsDamageBehavior::onAnimationEnd(%this)
{
   //if (%this.owner.Type $= "Projectile")
      //echo(" -- played hit animation : " @ %this);
   if(%this.deleteOnHit == false && %this.deleteOnDealDamage == false)   
      %this.owner.playAnimation(%this.originalAnim);
   else
      %this.cleanup();
}

/// <summary>
/// This function plays an assigned particle effect
/// </summary>
/// <param name="position">The world position to play the particle effect at.</param>
function DealsDamageBehavior::explode(%this, %position)
{
   if (isObject(%this.explodeEffect))
   {
      %explosion = %this.explodeEffect.clone();
      %explosion.position = %position;
      %explosion.setEffectLifeMode("Kill", 1.0);
      %explosion.playEffect();
   }
}

/*
function DealsDamageBehavior::onCollision(%this, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts)
{
   if (%this.owner.collided $= %dstObj)
      return;
   
   %this.owner.collided = %dstObj;
   
   %this.dealDamage(%dstObj);   
   
   if (%this.deleteOnHit)
   {
      %this.explode(getWords(%contacts, 0, 1));
      %direction = %this.owner.callOnBehaviors("getDirection");
      %cannotDie = %this.owner.callOnBehaviors("die", %direction, true, "DealtDamage");
      if (%cannotDie $= "ERR_CALL_NOT_HANDLED")
      {
         if (%this.owner.Type $= "Projectile")
         {
            MainScene.returnProjectileToCache(%this.owner);
         }
         else
         {
            %this.owner.safeDelete();
         }
      }
   }
}*/

/// <summary>
/// This function handles scheduling hit polling
/// </summary>
function DealsDamageBehavior::rescheduleHitCheck(%this)
{
   %this.checkForHitSchedule = %this.schedule(%this.checkForHitFreq, checkForHit);
}

/// <summary>
/// This function prevents this object from dealing damage.
/// </summary>
function DealsDamageBehavior::disableDealsDamage(%this)
{
   %this.ready = false;
   cancel(%this.checkForHitSchedule);
}

/// <summary>
/// This function resets the object's damage attributes.
/// </summary>
function DealsDamageBehavior::resetDealsDamage(%this)
{
   %this.ready = true;
   %this.hit = false;
   %this.enemyHitList = "";
   
   %this.rescheduleHitCheck();
}

/// <summary>
/// This function is used by the Projectile Tool to access this object's attack strength,
/// or damage.
/// </summary>
/// <return>Returns the damage that is dealt by this object.</return>
function DealsDamageBehavior::getAttack(%this)
{
   return %this.strength;
}

/// <summary>
/// This function is used by the Projectile Tool to set the object's attack strength, or damage.
/// </summary>
/// <param name="attack">The damage that this object will deal to its target.</param>
function DealsDamageBehavior::setAttack(%this, %attack)
{
   %this.strength = %attack;
}

/// <summary>
/// This function is used by the Projectile Tool to set the object's delete on hit state.
/// </summary>
/// <param name="deleteOnHit">Set this to true if the object is destroyed when it hits a target, or false if not.</param>
function DealsDamageBehavior::setDeleteOnHit(%this, %deleteOnHit)
{
   %this.deleteOnHit = %deleteOnHit;
}

/// <summary>
/// This function is used by the Projectile Tool to set the object's delete on deal damage state.
/// </summary>
/// <param name="deleteOnDealDamage">Set this to true if the object is destroyed when it deals damage, or false if not.</param>
function DealsDamageBehavior::setDeleteOnDealDamage(%this, %deleteOnDealDamage)
{
   %this.deleteOnDealDamage = %deleteOnDealDamage;
}

/// <summary>
/// This function is used by the Projectile Tool to set the object's hit type.  This is used primarily for
/// the piercing damage behavior at the moment.
/// </summary>
/// <param name="hitAllTowerEnemies">Set this to true if the object should continue to deal damage to more than one target in its path, or false if not.</param>
function DealsDamageBehavior::setHitAllTowerEnemies(%this, %hitAllTowerEnemies)
{
   %this.hitAllTowerEnemies = %hitAllTowerEnemies;
}