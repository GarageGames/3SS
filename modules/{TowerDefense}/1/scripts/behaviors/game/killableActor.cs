//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

%KillableActorAudioProfiles = "None" TAB "EnemyDeathSound" TAB "BaseAttackedSound";

/// <summary>
/// This behavior handles entity deaths.
/// </summary>
if (!isObject(KillableActorBehavior))
{
   %template = new BehaviorTemplate(KillableActorBehavior);
   
   %template.friendlyName = "Killable Actor";
   %template.behaviorType = "Game";
   %template.description  = "Handle actor death";

   %template.addBehaviorField(DeathAnimation, "The animation to play on death", object, "", AnimationAsset);
   %template.addBehaviorField(explodeEffect, "The particle effect to play on death", object, "", ParticleEffect);
   %template.addBehaviorField(DeathSound, "Sound profile played when the owner dies", enum, "", %KillableActorAudioProfiles);
}

/// <summary>
/// This function handles the entity's death.
/// </summary>
/// <param name="direction">The direction the actor is facing.</param>
/// <param name="instant">Set to true to tell this enemy to die without playing any animations or effects.</param>
/// <param name="reason">The cause of death.</param>
function KillableActorBehavior::die(%this, %direction, %instant, %reason)
{
   if(%this.owner.dead)
      return;
   
   %this.owner.dead = true;
   %this.owner.callOnBehaviors("disableTowerTarget");
   %this.owner.setCollisionSuppress(true);
   %this.owner.setSceneGroup(MainScene.invalidEnemyGroup);
   
   %type = %this.owner.getClassName();

   %animName = "Death" @ %direction @ "Anim";
   %mirror = "";
   eval("%mirror = %this.owner.animationSet.Death" @ %direction @ "Mirror;");
   
   %animation = "";
   eval("%animation = %this.owner.animationSet." @ %animName @ ";");
   
    switch$(%mirror)
    {
        case "Horizontal":
            %this.owner.setFlipX(true);

        case "Vertical":
            %this.owner.setFlipY(true);
        
        case "":
            %this.owner.setFlipX(false);
            %this.owner.setFlipY(false);
    }
   
   if (!isObject(%animation))
   {
      %animation = %this.DeathAnimation;
   }

   if(isObject(%animation) && !%instant && %type $= "t2dAnimatedSprite")
   {
      if (%reason !$= "DealtDamage")
      {
          %this.owner.callOnBehaviors(score);
          
          if(%this.DeathSound !$= "" && %this.DeathSound !$= "None")
          {
             alxPlay(%this.DeathSound);
          }
      }

      %this.owner.playAnimation(%animation, false);

      //echo(" -- KillableActorBehavior::die() with animation: " @ %this.owner);
   }
   else
   {
      //%this.owner.callOnBehaviors(clearTowerTarget);
      //%this.owner.callOnBehaviors(cleanupHealthBar);
      //%this.owner.callOnBehaviors(clearDirectionalAnimation);
      if (%reason !$= "DealtDamage")
      {
          %this.owner.callOnBehaviors(score);
          
          if(%this.DeathSound !$= "" && %this.DeathSound !$= "None")
          {
             alxPlay(%this.DeathSound);
          }
      }

      //%this.owner.safeDelete();
      MainScene.returnEnemyToCache(%this.owner);
      //echo(" -- KillableActorBehavior::die(): " @ %this.owner);
   }
}

/// <summary>
/// This function handles the onAnimationEnd callback.  For Killable Actors, this returns 
/// them to the cache.
/// </summary>
function KillableActorBehavior::onAnimationEnd(%this)
{
   //echo(" -- KillableActorBehavior::onAnimationEnd("@%this.owner@")");
   //%this.owner.callOnBehaviors(clearTowerTarget);
   //%this.owner.callOnBehaviors(cleanupHealthBar);
   //%this.owner.callOnBehaviors(clearDirectionalAnimation);

   //%this.owner.safeDelete();
   MainScene.returnEnemyToCache(%this.owner);
}

/// <summary>
/// This function handles last second cleanup, removing all behaviors from the owner.
/// </summary>
function KillableActorBehavior::onSafeDelete(%this)
{
   //echo(" -- KillableActorBehavior::onSafeDelete("@%this.owner@")");
   %this.owner.clearBehaviors();
}

/// <summary>
/// This function is used by the Enemy Tool to access the name of the audio profile
/// that contains the death sound for this actor.
/// </summary>
function KillableActorBehavior::getDeathSound(%this)
{
   return %this.DeathSound;
}

/// <summary>
/// This function is used by the Enemy Tool to assign the death sound audio profile
/// that this actor will play on death.
/// </summary>
function KillableActorBehavior::setDeathSound(%this, %deathSound)
{
   %this.DeathSound = %deathSound;
}