//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function MainScene::onLevelLoaded(%this)
{
   Parent::onLevelLoaded(%this);
   
   // Some constants
   %this.validEnemyGroup = 2;
   %this.validEnemyGroupMask = 4; // Group 2 is set with 3rd bit
   %this.invalidEnemyGroup = 0;
   %this.enemyCacheMultiplier = 3;
   %this.projectileCacheMultiplier = 10;
   %this.projectileHitAnimMultiplier = 3; // Multiplied with projectileCacheMultiplier
   %this.capProjectileHitAnimNumber = true;
   %this.piercingProjectileBehavior = "AppliesPiercingEffectBehavior";
   
   %this.buildEnemyCache();
   
   %this.startProjectileTypeCounting();
   
   // Get the projectile types from the tower templates
   %count = $persistentObjectSet.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      if (%object.Type $= "Tower")
      {
         %projectile = %object.callOnBehaviors(getProjectile);
         if (%projectile !$= "ERR_CALL_NOT_HANDLED")
         {
            %amount = 1;
            if (%projectile.getBehavior(%this.piercingProjectileBehavior) != 0)
            {
               // Create more piercing projectils than normal ones as they stay
               // on the play field longer.
               %rof = %object.callOnBehaviors(getRateOfFireValue);
               if (%rof == $FastestRateOfFire)
               {
                  %amount = 7;
               }
               else if (%rof >= $FastRateOfFire)
               {
                  %amount = 5;
               }
               else
               {
                  %amount = 3;
               }
            }
            
            %this.addTowerProjectileType(%projectile, %amount);
         }
      }
   }

   // Get the projectile hit animation sprites
   for (%i=0; %i<%count; %i++)
   {
      %object = $persistentObjectSet.getObject(%i);
      if (%object.Type $= "ProjectileHitAnimSprite")
      {
         %this.addProjectileHitAnimType(%object.getName(), %this.projectileHitAnimMultiplier, %this.capProjectileHitAnimNumber);
      }
   }
   
   %this.endProjectileTypeCounting();
   
   %this.buildProjectileCache();
   
   ScheduleManager.initialize();
   //%this.setDebugOn("0 5");
}

function MainScene::onLevelEnded(%this)
{
   %this.destroyEnemyCache();
   
   if (isObject(%this.enemyTypeList))
      %this.enemyTypeList.delete();

   %this.destroyProjectileCache();
   
   if (isObject(%this.projectileTypeList))
      %this.projectileTypeList.delete();
}

//-----------------------------------------------------------------------------

function MainScene::clearEnemyTypeList(%this)
{
   if (isObject(%this.enemyTypeList))
      %this.enemyTypeList.delete();
   
   %this.enemyTypeList = new SimGroup();
}

function MainScene::startEnemyTypeCounting(%this)
{
   //echo("@@@ MainScene::startEnemyTypeCounting");
   
   if (!isObject(%this.enemyTypeList))
      %this.enemyTypeList = new SimGroup();
}

function MainScene::endEnemyTypeCounting(%this)
{
   //echo("@@@ MainScene::endEnemyTypeCounting");
   
   // Find the maximum number required for each enemy type
   %count = %this.enemyTypeList.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %enemy = %this.enemyTypeList.getObject(%i);
      if (%enemy.maxCount < %enemy.waveCount)
      {
         %enemy.maxCount = %enemy.waveCount;
      }
      %enemy.waveCount = 0;
   }
}

function MainScene::addWaveEnemyType(%this, %enemyName, %count)
{
   //echo("@@@ MainScene::addWaveEnemyType: " @ %enemyName);
   
   // Check if the enemy has already been created on the list
   %enemy = %this.enemyTypeList.findObjectByInternalName(%enemyName);
   if (%enemy != 0)
   {
      // Found enemy in list
      %enemy.waveCount = %enemy.waveCount + %count;
   }
   else
   {
      // Enemy not yet in the list
      %enemy = new ScriptObject() {
            internalName = %enemyName;
            waveCount = %count;
            maxCount = 1;
         };
      %this.enemyTypeList.add(%enemy);
   }
}

function MainScene::buildEnemyCache(%this)
{
   if (!isObject(%this.enemyCache))
   {
      %this.enemyCache = new SimGroup();
   }
   %this.enemyCache.clear();
   
   %count = %this.enemyTypeList.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %enemy = %this.enemyTypeList.getObject(%i);
      %template = %enemy.internalName;
      
      if (isObject(%template))
      {
         // Get the group for the cached objects
         %group = %this._findOrCreateEnemyTypeGroup(%template.getName());
         
         // Build out a number of objects into the cache
         %num = %enemy.maxCount * %this.enemyCacheMultiplier;
         for (%j=0; %j<%num; %j++)
         {
            %obj = %this._createEnemyObject(%template, %group);
            //%group.add(%obj);
            %this.returnEnemyToCache(%obj);
         }
      }
      else
      {
         error("Could not find enemy template " @ %template @ " for cache!");
      }
   }
}

function MainScene::destroyEnemyCache(%this)
{
   %this.enemyCache.clear();
   %this.enemyCache.delete();
}

function MainScene::getEnemyFromCache(%this, %name)
{
   // Find the right group
   %group = %this._findOrCreateEnemyTypeGroup(%name.getName());

   // Anyone home?
   if (%group.getCount() > 0)
   {
      // Get the first object
      %obj = %group.getObject(0);
      %group.remove(%obj);
      %obj.fromCacheCount++;
      
      //echo("@@@ Used enemy object " @ %obj.getId() @ " from cache [" @ %obj.fromCacheCount @ "]");
   }
   else
   {
      // Need to create a new enemy object
      %obj = %this._createEnemyObject(%name, %group);
      
      //echo("@@@ Had to create enemy object");
      warn("-- Enemy cache miss. Had to create new enemy object [" @ %name @ "]");
   }
   
   %this._resetEnemyObject(%obj);
   return %obj;
}

function MainScene::returnEnemyToCache(%this, %obj)
{
   //echo("@@@ MainScene::returnEnemyToCache " @ %obj @ " " @ %obj.fromTemplateGroup.internalName);
   
   %obj.callOnBehaviors("disablePathFollowing");
   %obj.callOnBehaviors("disableDealsDamage");
   %obj.callOnBehaviors("disableDirectionalAnimation");
   %obj.callOnBehaviors("disableTowerTarget");
   %obj.callOnBehaviors("disableEnemySafeZone");
   %obj.callOnBehaviors("cleanupHealthBar");
   
   // Clean up any damage effects
   cancel(%obj.poisonEffectSchedule);
   %obj.setBlendColor(1, 1, 1);
   cancel(%obj.slowEffectSchedule);
   // DAW: Not available in Box2D
   //%obj.setMaxLinearVelocity(%obj.moveSpeed);
   
   // Used by scene's pick methods
   %obj.setSceneGroup(%this.invalidEnemyGroup);
   
   %obj.updateCallback = false;
   
   %obj.enabled = false;
   %obj.setVisible(false);
   %obj.fromTemplateGroup.add(%obj);
   
   //echo("@@@ MainScene::returnEnemyToCache " @ %obj @ " " @ %obj.fromTemplateGroup.internalName @ " [" @ %obj.fromTemplateGroup.getCount() @ "]");
}

function MainScene::_findOrCreateEnemyTypeGroup(%this, %name)
{
   %group = %this.enemyCache.findObjectByInternalName(%name);
   if (%group != 0)
      return %group;
   
   %group = new SimGroup() {
         internalName = %name;
      };
   %this.enemyCache.add(%group);
   
   return %group;
}

function MainScene::_createEnemyObject(%this, %template, %group)
{
   %obj = %template.clone(true);
   %obj.fromTemplate = %template;
   %obj.fromTemplateGroup = %group;
   %obj.fromCacheCount = 0;
   %obj.updateCallback = false;
   
   return %obj;
}

function MainScene::_resetEnemyObject(%this, %obj)
{
   %obj.enabled = true;
   %obj.setVisible(true);
   
   // Used by scene's pick methods
   //%obj.setSceneGroup(%this.validEnemyGroup);
   
   // Reset damage effects
   %obj.underPoisonEffect = false;
   %obj.poisonEffectSchedule = "";
   %obj.underSlowEffect = false;
   %obj.slowEffectSchedule = "";
   
   %obj.callOnBehaviors("enablePathFollowing");
   %obj.callOnBehaviors("enableDirectionalAnimation");
   //%obj.callOnBehaviors("enableTowerTarget");
   %obj.callOnBehaviors("resetTakesDamage");
   %obj.callOnBehaviors("resetScoreActor");
   %obj.callOnBehaviors("resetHealthBar");
   %obj.callOnBehaviors("resetEnemySafeZone");
   
   // These are set from KillableActorBehavior
   //%obj.setCollisionSuppress(false);
   %obj.dead = false;

   %obj.updateCallback = true;
}

//-----------------------------------------------------------------------------

function MainScene::clearProjectileTypeList(%this)
{
   if (isObject(%this.projectileTypeList))
      %this.projectileTypeList.delete();
   
   %this.projectileTypeList = new SimGroup();
}

function MainScene::startProjectileTypeCounting(%this)
{
   //echo("@@@ MainScene::startProjectileTypeCounting");
   
   if (!isObject(%this.projectileTypeList))
      %this.projectileTypeList = new SimGroup();
}

function MainScene::endProjectileTypeCounting(%this)
{
   //echo("@@@ MainScene::endProjectileTypeCounting");
}

function MainScene::addTowerProjectileType(%this, %projectileName, %amount)
{
   //echo("@@@ MainScene::addTowerProjectileType: " @ %projectileName);
   
   // Check if the projectile has already been created on the list
   %projectile = %this.projectileTypeList.findObjectByInternalName(%projectileName);
   if (%projectile != 0)
   {
      // Found projectile in list
      %projectile.count += %amount;
   }
   else
   {
      // Projectile not yet in the list
      %projectile = new ScriptObject() {
            internalName = %projectileName;
            count = %amount;
            capInstanceCount = false;
         };
      %this.projectileTypeList.add(%projectile);
   }
}

function MainScene::addProjectileHitAnimType(%this, %projectileName, %amount, %capInstanceCount)
{
   //echo("@@@ MainScene::addProjectileHitAnimType: " @ %projectileName);
   
   // Check if the projectile has already been created on the list
   %projectile = %this.projectileTypeList.findObjectByInternalName(%projectileName);
   if (%projectile != 0)
   {
      // Found projectile in list
      %projectile.count += %amount;
   }
   else
   {
      // Projectile not yet in the list
      %projectile = new ScriptObject() {
            internalName = %projectileName;
            count = %amount;
            capInstanceCount = %capInstanceCount;
         };
      %this.projectileTypeList.add(%projectile);
   }
}

function MainScene::buildProjectileCache(%this)
{
   if (!isObject(%this.projectileCache))
   {
      %this.projectileCache = new SimGroup();
   }
   %this.projectileCache.clear();
   
   %count = %this.projectileTypeList.getCount();
   for (%i=0; %i<%count; %i++)
   {
      %projectile = %this.projectileTypeList.getObject(%i);
      %template = %projectile.internalName;
      
      if (isObject(%template))
      {
         // Get the group for the cached objects
         %group = %this._findOrCreateProjectileTypeGroup(%template.getName());
         %group.capInstanceCount = %projectile.capInstanceCount;
         
         // Build out a number of objects into the cache
         %num = %projectile.count * %this.projectileCacheMultiplier;
         for (%j=0; %j<%num; %j++)
         {
            %obj = %this._createProjectileObject(%template, %group);
            //%group.add(%obj);
            %this.returnProjectileToCache(%obj);
         }
      }
      else
      {
         error("Could not find projectile template " @ %template @ " for cache!");
      }
   }
}

function MainScene::destroyProjectileCache(%this)
{
   %this.projectileCache.clear();
   %this.projectileCache.delete();
}

function MainScene::getProjectileFromCache(%this, %name)
{
   // Find the right group
   %group = %this._findOrCreateProjectileTypeGroup(%name.getName());

   // Anyone home?
   %validObject = false;
   if (%group.getCount() > 0)
   {
      // Get the first object
      %obj = %group.getObject(0);
      %group.remove(%obj);
      %obj.fromCacheCount++;
      
      %validObject = true;
      
      //echo("@@@ Used projectile object " @ %obj.getId() @ " from cache [" @ %obj.fromCacheCount @ "]");
   }
   else if (!%group.capInstanceCount)
   {
      // Need to create a new projectile object
      %obj = %this._createProjectileObject(%name, %group);
      
      %validObject = true;
      
      //echo("@@@ Had to create projectile object");
      warn("-- Projectile cache miss. Had to create new projectile object [" @ %name @ "]");
   }
   
   if (%validObject)
   {
      %this._resetProjectileObject(%obj);
      return %obj;
   }

   // We don't have a valid object, likely because we've reached the cap of the
   // amount of allowed instances.
   return -1;
}

function MainScene::returnProjectileToCache(%this, %obj)
{
   //echo("@@@ MainScene::returnProjectileToCache " @ %obj @ " " @ %obj.fromTemplateGroup.internalName);
   
   // Clean up projectile
   %obj.callOnBehaviors("setFaceTarget", "");
   %obj.callOnBehaviors("setMoveTarget", "");
   %obj.callOnBehaviors("disableDealsDamage");
   %obj.callOnBehaviors("disableMoveProjectile");
   %obj.SetCollisionSuppress(true);
   
   %obj.updateCallback = false;
   %obj.enabled = false;
   %obj.setVisible(false);
   %obj.fromTemplateGroup.add(%obj);
   
   //echo("@@@ MainScene::returnProjectileToCache " @ %obj @ " " @ %obj.fromTemplateGroup.internalName @ " [" @ %obj.fromTemplateGroup.getCount() @ "]");
}

function MainScene::_findOrCreateProjectileTypeGroup(%this, %name)
{
   %group = %this.projectileCache.findObjectByInternalName(%name);
   if (%group != 0)
      return %group;
   
   %group = new SimGroup() {
         internalName = %name;
      };
   %this.projectileCache.add(%group);
   
   return %group;
}

function MainScene::_createProjectileObject(%this, %template, %group)
{
   %obj = %template.clone(true);
   %obj.fromTemplate = %template;
   %obj.fromTemplateGroup = %group;
   %obj.fromCacheCount = 0;
   
   %obj.isProjectile = true;
   
   // DAW: Not available in Box2D
   //%obj.cacheMaxLinearVelocity = %obj.getMaxLinearVelocity();
   %obj.cacheOriginalAnim = %template.getAnimation();
   %obj.cacheOriginalSizeX = %template.getSizeX();
   %obj.cacheOriginalSizeY = %template.getSizeY();
   
   %obj.SetCollisionSuppress(true);
   
   %obj.updateCallback = false;
   %obj.enabled = false;
   
   // Build out collision shapes for those projectiles that need it
   if (%obj.getBehavior(%this.piercingProjectileBehavior) != 0)
   {
      %id = %obj.createCircleCollisionShape(%obj.cacheOriginalSizeX * 0.25);
      %obj.setCollisionShapeIsSensor(%id, true);
      %obj.setCollisionAgainst(SceneBehaviorObject);
      %obj.setCollisionCallback(true);
   }
   
   return %obj;
}

function MainScene::_resetProjectileObject(%this, %obj)
{
   %obj.enabled = true;
   %obj.setVisible(true);

   // Enable projectile here
   %obj.SetCollisionSuppress(false);
   %obj.hit = false;
   // DAW: Not available in Box2D
   //%obj.setMaxLinearVelocity(%obj.cacheMaxLinearVelocity);
   %obj.setLinearVelocity(0, 0);
   %obj.setAngularVelocity(0);
   %obj.setSize(%obj.cacheOriginalSizeX, %obj.cacheOriginalSizeY);
   %obj.playAnimation(%obj.cacheOriginalAnim);
   %obj.callOnBehaviors("enableMoveProjectile");
   %obj.callOnBehaviors("resetDealsDamage");
   %obj.callOnBehaviors("resetAttackEffect");
   %obj.updateCallback = true;
}

//-----------------------------------------------------------------------------

function MainScene::OnCollision( %this, %objA, %objB, %collisionDetails )
{
   if (%objA.getName() $= "SceneBehaviorObject")
   {
      if (%objB.isProjectile)
      {
         %this.returnProjectileToCache(%objB);
      }
   }
   else if (%objB.getName() $= "SceneBehaviorObject")
   {
      if (%objA.isProjectile)
      {
         %this.returnProjectileToCache(%objA);
      }
   }
}
