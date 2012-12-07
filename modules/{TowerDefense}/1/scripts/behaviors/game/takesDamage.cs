//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$damageStateUndamaged = 1;
$damageStateLightDamage = 2;
$damageStateHeavyDamage = 3;
$damageStateDead = 4;

%takesDamageAudioProfiles = "None" TAB "EnemyDeathSound" TAB "BaseAttackedSound";

/// <summary>
/// This behavior allows the associated object to take damage.
/// </summary>
if (!isObject(TakesDamageBehavior))
{
   %template = new BehaviorTemplate(TakesDamageBehavior);
   
   %template.friendlyName = "Takes Damage";
   %template.behaviorType = "Game";
   %template.description  = "Set the object to take damage from DealsDamage objects that collide with it";

   %template.addBehaviorField(health, "The amount of health the object has", int, 100);
   %template.addBehaviorField(lives, "The number of times the object can lose all its health", int, 3);
   %template.addBehaviorField(tintRedForDamage, "Tint the object red as it takes damage", bool, 0);
   %template.addBehaviorField(respawnTime, "The time between death and respawn (seconds)", float, 2.0);
   %template.addBehaviorField(invincibleTime, "The time after spawning before damage is applied (seconds)", float, 1.0);
   %template.addBehaviorField(respawnEffect, "The particle effect to play on spawn", object, "", ParticleEffect);
   %template.addBehaviorField(ignoreDamageFrom, "The object class this object should ignore damage from", text, "");

   %template.addBehaviorField(changeStateWithDamage, "If set, the background music and damage image will change based on the owners health", bool, 0);
   %template.addBehaviorField(damageState1Threshold, "Float value indicating at what health level to switch to first damage image.", float, 0.8);
   %template.addBehaviorField(damageState1Image, "If set, indicates the first damage state image.", object, "", ImageAsset);
   %template.addBehaviorField(damageState1Music, "If set, indicates the first damage state image.", object, "", ImageAsset);
   %template.addBehaviorField(damageState2Threshold, "Float value indicating at what health level to switch to second damage image.", float, 0.5);
   %template.addBehaviorField(damageState2Image, "If set, indicates the second damage state image.", object, "", ImageAsset);
   %template.addBehaviorField(damageState2Music, "If set, indicates the first damage state image.", object, "", ImageAsset);
   %template.addBehaviorField(damageState3Threshold, "Float value indicating at what health level to switch to third damage image.", float, 0.2);
   %template.addBehaviorField(damageState3Image, "If set, indicates the third damage state image.", object, "", ImageAsset);
   %template.addBehaviorField(damageState3Music, "If set, indicates the first damage state image.", object, "", t2d);
   
   %template.addBehaviorField(damageSound, "Sound profile played when the owner takes damage", enum, "None", %takesDamageAudioProfiles);
}

/// <summary>
/// This function handles basic initialization tasks.
/// </summary>
function TakesDamageBehavior::onBehaviorAdd(%this)
{
   %this.beginHealth = %this.health;
   %this.beginLives = %this.lives;
}

/// <summary>
/// This function handles more complex initialization tasks.
/// </summary>
function TakesDamageBehavior::onAddToScene(%this)
{
   %this.damageState = 1;

   %this.startHealth = %this.health;
   
   if (%this.owner.isMemberOfClass("t2dStaticSprite"))
      %this.startFrame = %this.owner.getFrame();
   
   if (%this.health < 1)
      %this.health = 1;
   if (%this.health > 10000)
      %this.health = 10000;
   
   if (%this.lives < 0)
      %this.lives = 0;
   if (%this.lives > 1000)
      %this.lives = 1000;
   
   %this.spawn();
}

/// <summary>
/// This function ensures that the same assailant does not do damage more than once, updates
/// the health of the entity, plays a damage sound if one is assigned, handles "killing" the 
/// entity if its health is zero or less and handles music state changes if so desired.
/// </summary>
/// <param name="amount">The amount of health to take from the entity.</param>
/// <param name="assailant">The originator of the damage.</param>
function TakesDamageBehavior::takeDamage(%this, %amount, %assailant)
{
   //echo(" -- " @ %this.owner @ " takes damage from " @ %assailant.owner.class);
   if (%this.invincible)
      return false;
   
   if (%this.owner.lastAssailant == %assailant)
      return false;
      
   %this.owner.lastAssailant = %assailant;

   %this.health -= %amount;

   //echo(" -- " @ %this.owner.class @ " taking "@%amount@" damage from " @%assailant.owner.class @ " - health remaining : " @ %this.health);

   if (%this.health <= 0)
   {
      %this.schedule(0, "kill", %assailant);

      return true;
   }
   
   // Play damage sound
   if (%this.damageSound !$= "None" && %this.damageSound !$= "")
   {
      alxPlay(%this.damageSound);
   }
   
   if (%this.tintRedForDamage)
   {
      %tint = %this.health / %this.startHealth;
      %this.owner.setBlendColor(1, %tint, %tint, 1);
   }
   
   if (%this.changeStateWithDamage)
   {
        %damagePercent = %this.health/%this.startHealth;
        %pulser = %this.owner.getBehavior("AlphaPulserBehavior");
                
        %oldDamageState = %this.damageState;
        
        if (%damagePercent == 1)
        {
            %this.damageState = $damageStateUndamaged;
        }
        else if (%damagePercent <= %this.damageState1Threshold && %damagePercent > %this.damageState2Threshold)
        {
            %this.damageState = $damageStateLightDamage;
            if(isObject(%this.damageState1Image))
               %this.owner.setImageMap(%this.damageState1Image);
            if(isObject(%pulser))
               %pulser.setDamageState(1);
        }
        else if (%damagePercent <= %this.damageState2Threshold && %damagePercent > %this.damageState3Threshold)
        {
            %this.damageState = $damageStateHeavyDamage;
            if(isObject(%this.damageState2Image))
               %this.owner.setImageMap(%this.damageState2Image);
            if(isObject(%pulser))
               %pulser.setDamageState(2);
        }
        else if (%damagePercent <= %this.damageState3Threshold) 
        {
            if(isObject(%this.damageState3Image))
               %this.owner.setImageMap(%this.damageState3Image);
            if(isObject(%pulser))
               %pulser.setDamageState(3);
        }
        else if (%damagePercent < 0)
        {
            //echo(" -- damage : " @ %damagePercent);
            %this.damageState = $damageStateDead;
        }
   
        // If the damage state has changed...
        if (%this.damageState != %oldDamageState)
        {
            // Change music
            SceneBehaviorObject.callOnBehaviors(startMusic, %this.damageState);
        }
   }
   
   return true;
}

/// <summary>
/// This function kills the entity.
/// </summary>
/// <param name="reason">The cause of death.</param>
function TakesDamageBehavior::kill(%this, %reason)
{
   //echo(" - TakesDamageBehavior::kill() - " @ %this.owner);
   %this.lives--;
   if (%this.lives <= 0)
   {
      // DAW: Not available in Box2D
      //%this.owner.setCollisionActive(0, 0);
      // DAW: Not available in Box2D
      //%this.owner.setUsesPhysics(0);
      %this.owner.active = false;
      %this.owner.alive = false;
      %direction = %this.owner.callOnBehaviors("getDirection");
      
      %this.owner.callOnBehaviors("die", %direction);
      
      return;
   }
   
   %this.invincible = true;
   %this.owner.visible = false;
   %this.owner.collisionActiveReceive = false;
   %this.schedule(%this.respawnTime * 1000, "spawn");
}

/// <summary>
/// This function makes the entity vulnerable to damage.
/// </summary>
function TakesDamageBehavior::setVincible(%this)
{
   %this.invincible = false;
}

/// <summary>
/// This function handles setup of the entity on creation.
/// </summary>
function TakesDamageBehavior::spawn(%this)
{
   %this.owner.collisionActiveReceive = true;
   %this.schedule(%this.invincibleTime * 1000, "setVincible");
   %this.health = %this.startHealth;
   %this.owner.setBlendColor(1, 1, 1, 1);
   %this.owner.visible = true;
   %this.owner.alive = true;
   
   if (%this.owner.isMemberOfClass("t2dStaticSprite"))
      %this.owner.setFrame(%this.startFrame);
   
   if (isObject(%this.respawnEffect))
   {
      %spawnEffect = %this.respawnEffect.clone();
      %spawnEffect.position = %this.owner.position;
      %spawnEffect.setEffectLifeMode("Kill", 1.0);
      %spawnEffect.playEffect();
   }
}

/// <summary>
/// This function is used by the Enemy Tool to access the health of the entity.
/// </summary>
/// <return>Returns the health of the entity.</return>
function TakesDamageBehavior::getHealth(%this)
{
   return %this.health;
}

/// <summary>
/// This function is used by the Enemy Tool to set the health of the entity.
/// </summary>
/// <param name="health">The total health of the entity.</param>
function TakesDamageBehavior::setHealth(%this, %health)
{
   %this.health = %health;
}

/// <summary>
/// This function is used by the cache system to clean up the entity before 
/// recycling it.
/// </summary>
function TakesDamageBehavior::resetTakesDamage(%this)
{
   %this.health = %this.beginHealth;
   %this.lives = %this.beginLives;
   %this.damageState = $damageStateUndamaged;
   // DAW: Not available in Box2D
   //%this.owner.setCollisionActive(1, 1);
   // DAW: Not available in Box2D
   //%this.owner.setUsesPhysics(1);
   %this.owner.alive = true;
}