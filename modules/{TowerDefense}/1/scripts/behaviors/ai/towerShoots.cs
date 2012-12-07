//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$ShortestRange = 2;
$ShortRange = 3;
$AverageRange = 4;
$LongRange = 5;
$LongestRange = 6;

$SlowestRateOfFire = 1;
$SlowRateOfFire = 3;
$AverageRateOfFire = 5;
$FastRateOfFire = 7;
$FastestRateOfFire = 10;

%towerShootAudioProfiles = "None" TAB "TowerShootFiringSound1" TAB "TowerShootFiringSound2" TAB "TowerShootFiringSound3";

/// <summary>
/// TowerShootsBehavior allows an object to select targets and fire projectiles at them.
/// </summary>
if (!isObject(TowerShootsBehavior))
{
   %template = new BehaviorTemplate(TowerShootsBehavior);
   
   %template.friendlyName = "Tower Shoots";
   %template.behaviorType = "AI";
   %template.description  = "Set the object to shoot at the nearest enemy and play attack animations if it has them.";

   %template.addBehaviorField(projectile, "The projectile to clone and shoot", object, "", SceneObject);
   %template.addBehaviorField(range, "Range in map cells in which the Tower will fire", int, "2");
   %template.addBehaviorField(rateOfFire, "The Rate at which the Tower fires its projectiles (10 being the fastest)", int, "1");
   %template.addBehaviorField(fireStartDelay, "Time before first shot (seconds)", float, "0.5");
   %template.addBehaviorField(fireSound, "Sound profile played when the tower shoots", enum, "", %towerShootAudioProfiles);
   %template.addBehaviorField(retargetAfterShot, "Whether the owner picks a new target after each shot", bool, 0);

   %template.addBehaviorField(ScaleTime, "Scale the animation time to coincide with rate of fire", bool, 1);
   %template.addBehaviorField(NorthAnim, "The object's North attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(NEastAnim, "The object's Northeast attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(EastAnim, "The object's East attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(SEastAnim, "The object's Southeast attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(SouthAnim, "The object's South attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(SWestAnim, "The object's Southwest attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(WestAnim, "The object's West attack animation", object, "", AnimationAsset);
   %template.addBehaviorField(NWestAnim, "The object's Northwest attack animation", object, "", AnimationAsset);
}

/// <summary>
/// This handles some basic initialization tasks.
/// </summary>
/// <param name="scene">The scene that the behavior is associated with</param>
function TowerShootsBehavior::onAddToScene(%this, %scene)
{
    //echo(" -- TowerShootsBehavior::onAddToScene("@%this@") : " @ %this.owner);
   %this.isToggled = false;
   %this.target = "";   // start with no target
   //%this.schedule(%this.fireStartDelay * 1000, "fire");
   %type = %this.owner.getClassName();
   switch$(%type)
   {
      case "t2dAnimatedSprite":
         %this.type = 1;
      case "t2dStaticSprite":
         %this.type = 2;
   }
   %this.schedule(25, "setup");
}

/// <summary>
/// This handles some more advanced initialization tasks, including setting up the 
/// grid sizes used by the tower's range determination for targeting, the tower's correct
/// rate of fire, animation loading and scaling for fire rate.
/// </summary>
function TowerShootsBehavior::setup(%this)
{
   %this.isToggled = false;
   %this.target = "";   // start with no target
   %type = %this.owner.getClassName();
   switch$(%type)
   {
      case "t2dAnimatedSprite":
         %this.type = 1;
      case "t2dStaticSprite":
         %this.type = 2;
   }

   %gridSize = $TowerPlaceGrid.getWidth() / $TowerPlaceGrid.getCellCountX();
   if (%this.range > 10)
      %this.range = 10;
      
   if (%this.range < 1)
      %this.range = 1;
      
   // The radius for targeting and projectile range is the provided tower range
   // plus one half of a grid space, which is half a meter.
   %this.radius = %this.range + 0.5;
   //echo(" -- radius : " @ %this.radius);

   if (%this.rateOfFire > 10)
      %this.rateOfFire = 10;
   
   if (%this.rateOfFire < 1)
      %this.rateOfFire = 1;
      
   %flippedRateOfFire = mAbs(%this.rateOfFire - 10);

   %this.fireRate = 0.25 + (2.25 * (%flippedRateOfFire / 9));
   
   %this.appliesEffect = %this.projectile.callOnBehaviors("getAppliesEffect");
   //echo(" ** TowerShootsBehavior " @ %this @ " projectile : " @ %this.projectile);

   %this.projectileSpeed = 3;
   
   if (%this.type == 1)
   {
       // For towers that use animated sprites, set up the idle and firing animations for
       // each of the 8 directions supported.
      %this.idleAnim = %this.owner.animationSet.defaultAnimation;

      %this.animArray[0] = %this.owner.animationSet.IdleNorthAnim;
      %this.animFireArray[0] = %this.owner.animationSet.FiringNorthAnim;
      
      if (isObject(%this.owner.animationSet.IdleNortheastAnim))
      {
         %this.animArray[1] = %this.owner.animationSet.IdleNortheastAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using North Idle Animation for Northeast Idle Animation");
         %this.animArray[1] = %this.owner.animationSet.IdleNorthAnim;
      }
      if (isObject(%this.owner.animationSet.FiringNortheastAnim))
      {
         %this.animFireArray[1] = %this.owner.animationSet.FiringNortheastAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using North Firing Animation for Northeast Firing Animation");
         %this.animFireArray[1] = %this.owner.animationSet.FiringNorthAnim;
      }
         
      %this.animArray[2] = %this.owner.animationSet.IdleEastAnim;
      %this.animFireArray[2] = %this.owner.animationSet.FiringEastAnim;
      
      if (isObject(%this.owner.animationSet.IdleSoutheastAnim))
      {
         %this.animArray[3] = %this.owner.animationSet.IdleSoutheastAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using East Idle Animation for Southeast Idle Animation");
         %this.animArray[3] = %this.owner.animationSet.IdleEastAnim;
      }
      if (isObject(%this.owner.animationSet.FiringSoutheastAnim))
      {
         %this.animFireArray[3] = %this.owner.animationSet.FiringSoutheastAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using East Firing Animation for Southeast Firing Animation");
         %this.animFireArray[3] = %this.owner.animationSet.FiringEastAnim;
      }

      %this.animArray[4] = %this.owner.animationSet.IdleSouthAnim;
      %this.animFireArray[4] = %this.owner.animationSet.FiringSouthAnim;
      
      if (isObject(%this.owner.animationSet.IdleSouthwestAnim))
      {
         %this.animArray[5] = %this.owner.animationSet.IdleSouthwestAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using South Idle Animation for Southwest Idle Animation");
         %this.animArray[5] = %this.owner.animationSet.IdleSouthAnim;
      }
      if (isObject(%this.owner.animationSet.FiringSouthwestAnim))
      {
         %this.animFireArray[5] = %this.owner.animationSet.FiringSouthwestAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using South Firing Animation for Southwest Firing Animation");
         %this.animFireArray[5] = %this.owner.animationSet.FiringSouthAnim;
      }
         
      %this.animArray[6] = %this.owner.animationSet.IdleWestAnim;
      %this.animFireArray[6] = %this.owner.animationSet.FiringWestAnim;

      if (isObject(%this.owner.animationSet.IdleNorthwestAnim))
      {
         %this.animArray[7] = %this.owner.animationSet.IdleNorthwestAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using West Idle Animation for Northwest Idle Animation");
         %this.animArray[7] = %this.owner.animationSet.IdleWestAnim;
      }
      if (isObject(%this.owner.animationSet.FiringNorthwestAnim))
      {
         %this.animFireArray[7] = %this.owner.animationSet.FiringNorthwestAnim;
      }
      else
      {
         echo(" ## - " @ %this.owner.getName() @ " Using West Firing Animation for Northwest Firing Animation");
         %this.animFireArray[7] = %this.owner.animationSet.FiringWestAnim;
      }
      
      %this.animFlipArray[0] = %this.owner.animationSet.IdleNorthMirror;
      %this.animFireFlipArray[0] = %this.owner.animationSet.FiringNorthMirror;
      
      %this.animFlipArray[1] = %this.owner.animationSet.IdleNortheastMirror;
      %this.animFireFlipArray[1] = %this.owner.animationSet.FiringNortheastMirror;

      %this.animFlipArray[2] = %this.owner.animationSet.IdleEastMirror;
      %this.animFireFlipArray[2] = %this.owner.animationSet.FiringEastMirror;

      %this.animFlipArray[3] = %this.owner.animationSet.IdleSoutheastMirror;
      %this.animFireFlipArray[3] = %this.owner.animationSet.FiringSoutheastMirror;

      %this.animFlipArray[4] = %this.owner.animationSet.IdleSouthMirror;
      %this.animFireFlipArray[4] = %this.owner.animationSet.FiringSouthMirror;

      %this.animFlipArray[5] = %this.owner.animationSet.IdleSouthwestMirror;
      %this.animFireFlipArray[5] = %this.owner.animationSet.FiringSouthwestMirror;

      %this.animFlipArray[6] = %this.owner.animationSet.IdleWestMirror;
      %this.animFireFlipArray[6] = %this.owner.animationSet.FiringWestMirror;

      %this.animFlipArray[7] = %this.owner.animationSet.IdleNorthwestMirror;
      %this.animFireFlipArray[7] = %this.owner.animationSet.FiringNorthwestMirror;

      %this.owner.fireOnStartAnim = %this.owner.animationSet.fireOnStart;

        for (%i = 0; %i < 8; %i++)
        {
            // For each direction, determine the correct animation speed scale for the
            // tower's fire animation and set up the tower's projectile fire time - 
            // either at the start of the fire animation or at the end of it.
            %anim = %this.animArray[%i];
            %animTime = %anim.animationTime;
            %this.animScale[%i] = (%animTime / (%this.fireRate * 0.75));
            if (%this.animScale[%i] < 1.0)
            %this.animScale[%i] = 1.0;
            
            if (!%this.owner.fireOnStartAnim)
            {
                %fireAnim = %this.animFireArray[%i];
                %animTime = %fireAnim.animationTime;
                %this.animFireScale[%i] = (%animTime / (%this.fireRate * 0.75));
                if (%this.animFireScale[%i] < 1.0)
                %this.animFireScale[%i] = 1.0;
            }
            else
                %this.animFireScale[%i] = 1.0;
            //echo(" -- AnimTower fire rate : " @ %this.fireRate @ " : animation time : " @ %animTime @ " animScale : " @ %this.animScale[%i]);
        }
        %this.owner.animationName = %this.animArray[%this.idleAnim];
        %this.owner.setAnimation(%this.animArray[%this.idleAnim]);
        %temp = %this.owner.getAnimation();
        %this.owner.playAnimation(%this.animArray[%this.idleAnim]);
        
        %size = %this.animArray[%this.idleAnim].imageMap.getFrameSize(0);
        %size = Vector2Scale(%size, $TDMetersPerPixel);
        %this.owner.setSize(%size);
   }
    //echo(" -- TowerShootsBehavior::setup("@%this@") : " @ %this.owner);
}

/// <summary>
/// This global function sets the flipped state of a sprite.
/// </summary>
/// <param name="direction">The direction to flip the object, Horizontal, Vertical or Diagonal.</param>
function setFlip(%direction, %owner)
{
    switch$(%direction)
    {
        case "":
            %owner.setFlipX(false);
            %owner.setFlipY(false);
        
        case "Horizontal":
            %owner.setFlipX(true);
            %owner.setFlipY(false);
        
        case "Vertical":
            %owner.setFlipX(false);
            %owner.setFlipY(true);
        
        case "Diagonal":
            %owner.setFlipX(true);
            %owner.setFlipY(true);
    }
}

/// <summary>
/// This function finds a vector to the tower's target and determines which directional
/// animation to play.
/// </summary>
function TowerShootsBehavior::testDirection(%this)
{
   %temp = Vector2Sub(%this.owner.position, %this.target.position);
   %vector = Vector2Normalize(%temp);
   %targetRotation = mRadToDeg(mAtan(%vector.x, %vector.y)) + 180;

   if (%targetRotation >= 0 && %targetRotation < 22.5)
   {
      if (%this.direction == 0)
         return;

      %this.direction = 0;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 22.5 && %targetRotation < 67.5)
   {
      if (%this.direction == 1)
         return;

      %this.direction = 1;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 67.5 && %targetRotation < 112.5)
   {
      if (%this.direction == 2)
         return;

      %this.direction = 2;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 112.5 && %targetRotation < 157.5)
   {
      if (%this.direction == 3)
         return;

      %this.direction = 3;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 157.5 && %targetRotation < 202.5)
   {
      if (%this.direction == 4)
         return;

      %this.direction = 4;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 202.5 && %targetRotation < 247.5)
   {
      if (%this.direction == 5)
         return;

      %this.direction = 5;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 247.5 && %targetRotation < 292.5)
   {
      if (%this.direction == 6)
         return;

      %this.direction = 6;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else if (%targetRotation >= 292.5 && %targetRotation < 337.5)
   {
      if (%this.direction == 7)
         return;

      %this.direction = 7;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   else
   {
      if (%this.direction == 0)
         return;

      %this.direction = 0;
      setFlip(%this.animFireFlipArray[%this.direction], %this.owner);
   }
   //echo(" >- targetRotation : " @ %targetRotation @ " >- direction : " @ %this.direction);
}

/// <summary>
/// This function gets the range from the tower to a target.
/// </summary>
/// <param name="obj">The desired target object.</param>
/// <return>True if obj is in range, false if not</return>
function TowerShootsBehavior::isInRange(%this, %obj)
{
   if (!%this.owner.centerPoint)
      %towerPos = %this.owner.position;
   else
      %towerPos = %this.owner.centerPoint;

   %dist = Vector2Distance(%obj.getPosition(), %towerPos);

   if(%dist < %this.radius)
   {
      return true;
   }
   
   return false;
}

/// <summary>
/// This function attempts to shoot a projectile at a target within the tower's range.
/// It polls for targets at the tower's fire rate.
/// </summary>
function TowerShootsBehavior::fire(%this)
{
   //echo(" -- Tower " @ %this.owner @ " attempting to fire");
   
   if(MainScene.getScenePause())
   {
      //echo(" -- TowerShootsBehavior::fire() SCENE IS PAUSED");
      %this.schedule(%this.genDelay(), "fire");
      return;
   }
   
   //echo(" -- Tower " @ %this.owner @ " begin fire");
   if (!%this.owner.isToggled)
   {
       echo(" -- Tower " @ %this.owner @ " is not toggled on : " @ %this.owner.isToggled);
      return;
   }

    if (!isObject(%this.projectile))
    {
        echo(" -- Tower " @ %this.owner @ " : projectile object is not correct");
        %this.schedule(%this.genDelay(), "fire");
        return;
    }
   //echo(" -- Tower " @ %this.owner @ " finding target");
   // do we have a target?   
   if (isObject(%this.target))
   {
      if (%this.target.dead)
      {
         // Target is dead
         %this.target = "";
      }
      else if (!%this.isInRange(%this.target))
      {
         // Target is no longer in range
         %this.target = "";
      }
      else if (!TowerTargetSet.isMember(%this.target))
      {
         // Target is no longer a valid target
         %this.target = "";
      }
      else if (%this.retargetAfterShot)
      {
         // If we pick a new target after each shot
         %this.target = "";
      }
   }
      
   if (!isObject(%this.target))
   {
      %this.target = %this.getTarget();
   }
   
   if (isObject(%this.target) && %this.owner.canFire)
   {
      if (%this.type == 1)
         %this.idle = false;

      %this.testDirection();
      // fire shoot animation
      //echo(" -- TowerShootsBehavior::fire() - testing direction - " @ %this.direction @ " Owner type : " @ %this.type);
      if (%this.type == 1)
      {
         %this.owner.setSpeedFactor(%this.animScale[%this.direction]);
         if (%this.owner.fireOnStartAnim)
         {
            %this.shootProjectile();
            %this.owner.playAnimation(%this.animFireArray[%this.direction]);
         }
         else
            %this.owner.playAnimation(%this.animFireArray[%this.direction]);

         //echo(" -- TowerShootsBehavior::fire() - animation : " @ %this.animFireArray[%this.direction] @ " Speed Factor : " @ %this.animScale[%this.direction] @ " Direction : " @ %this.direction);
      }
      else
         %this.shootProjectile();
   }
   else if (!isObject(%this.target))
   {
      if (%this.type == 1)
      {
         if (%this.idle == false)
         {
            %this.idle = true;
            %this.owner.setSpeedFactor(1);
            setFlip(%this.animFlipArray[%this.direction], %this.owner);
            %this.owner.playAnimation(%this.animArray[%this.direction]);
         }
      }
   }
   
   %this.schedule(%this.genDelay(), "fire");
}

function TowerShootsBehavior::getTarget(%this)
{
   %targetList = "";   
   
   // get a new target from TowerTargetSet
      %pos = %this.owner.getPosition();
      %pickList = MainScene.pickArea(%pos.x-%this.radius, %pos.y+%this.radius, %pos.x+%this.radius, %pos.y-%this.radius, MainScene.validEnemyGroupMask);
      
      // As we are comparing a box pick area with enemy AABB's we need to do an
      // additional range check.  Otherwise, we can get target's that are OK
      // when checking these boxes, but not OK when we want a range circle.
      %size = getWordCount(%pickList);
      for (%i=0; %i<%size; %i++)
      {
         %possibleTarget = getWord(%pickList, %i);
         %dist = Vector2Distance(%pos, %possibleTarget.getPosition());
         
         if (%dist <= (%this.radius))
         {
            %targetList = %possibleTarget SPC %targetList;
         }
      }
     
   %sortedList = %targetList;  
   
   // If the tower has an effect, sort with preference to enemies without that effect
   %towerEffectSlotName = %this.owner.slot; 
   %towerEffectType = %this.projectile.callOnBehaviors("getAppliesEffect");  

   if (%towerEffectType !$= "ERR_CALL_NOT_HANDLED")
   {
      // Sort by whether the enemies have a effect of the same TYPE as the current tower (slow/poison)
      %sortedList = getLowestObjectsFromList(%sortedList, "checkEffectType", %towerEffectType);
   
      // Sort by whether the ememies have a worse effect than the current tower based on effect SLOT NAME
      %sortedList = getLowestObjectsFromList(%sortedList, "checkEffectName", %towerEffectSlotName);
   }

   // Get targets with lowest health
   %sortedList = getLowestObjectsFromList(%sortedList, "callOnBehaviors", "getHealth");  
      
   // Sort by distance to exit
   %sortedList = getLowestObjectsFromList(%sortedList, "callOnBehaviors", "getCachedRemainingPathDistance");
    
   return getWord(%sortedList, 0);  
}

function SceneObject::checkEffectName(%this, %towerSlotName)
{
   if (!isObject(%this.EffectSet))
      return 0;   
   
   for (%i = 0; %i < %this.EffectSet.getCount(); %i++)
   {
      %existingSlotName = %this.EffectSet.getObject(%i).aggressor.slot;

      // if towerSlotName > existingSlotName, return 0
      // if towerSlotName = existingSlotName, return 1
      // if towerSlotName < existingSlotName, return 2
      return compareTowerNames(%existingSlotName, %towerSlotName);
   }
   
   return 0;
}

function SceneObject::checkEffectType(%this, %effectBehavior)
{
   if (!isObject(%this.EffectSet))
      return 0; 
      
   for (%i = 0; %i < %this.EffectSet.getCount(); %i++)
   {
      %aggressorTowerShootsBehavior = %this.EffectSet.getObject(%i).aggressor.getBehavior("TowerShootsBehavior");
      if (isObject(%aggressorTowerShootsBehavior))
         %existingEffectType = %aggressorTowerShootsBehavior.projectile.callOnBehaviors("getAppliesEffect");      
      
      if (%existingEffectType == %effectBehavior)
         return 1;      
   }
   
   return 0;
}

function TowerShootsBehavior::sortByHealth(%this, %targetList)
{
   
}

function TowerShootsBehavior::sortByEffect(%this, %targetList)
{
   
}

function TowerShootsBehavior::shootProjectile(%this)
{
   if (!%this.owner.canFire)
      return;
      
   //%projectile = %this.projectile.clone();
   %projectile = MainScene.getProjectileFromCache(%this.projectile);
   %projectile.setSceneLayer(%this.owner.getSceneLayer() - 1);
   
   // Remove this projectile when the target is no longer valid.
   %projectile.removeWithTarget = true;
   %projectile.aggressor = %this.owner;
   
   // Play fire sound
   if(%this.fireSound !$= "None" && %this.fireSound !$= "")
   {
     alxPlay(%this.fireSound);
   }

   if (!%this.owner.centerPoint)
      %position = %this.owner.Position;
   else
      %position = %this.owner.centerPoint;
   
   if(%this.direction > -1)
   {
      %index = %this.direction * 2;
      %muzzlePoint = getWords(%this.owner.LinkPoints, %index, %index + 1);
      
      %muzzlePoint.x *= (%this.owner.getSizeX()/2);
      %muzzlePoint.y *= (%this.owner.getSizeY()/-2);
      
      %position.x += %muzzlePoint.x;
      %position.y += %muzzlePoint.y;
   }
   %x = getWord(%position, 0);
   %y = getWord(%position, 1);
   %projectile.setPosition(%x SPC %y);

   // We need to set up the projectile's position before this call as some behaviors rely on it   
   %projectile.callOnBehaviors(setupProjectile, %this.target, %this.projectileSpeed, %this.radius, true);
}

/// <summary>
/// This function gets the fire rate delay time in milliseconds.
/// </summary>
/// <return>The tower's fire delay in milliseconds</return>
function TowerShootsBehavior::genDelay(%this)
{
   // note: we currently don't want a random delay variance.
   return %this.fireRate * 1000;
}

/// <summary>
/// This function toggles the fire state of the tower.
/// </summary>
/// <param name="state">If passed true, enables firing and schedules the tower's next attack.  If false, disables firing.</param>
function TowerShootsBehavior::toggle(%this, %state)
{
   %this.owner.isToggled = %state;
   
   //echo(" -- TowerShootsBehavior::toggle("@%this@", "@%state@" - "@%this.owner.isToggled);
   if(%this.owner.isToggled)
   {
      %this.schedule(%this.genDelay(), "fire");
      //%this.schedule(%this.fireStartDelay * 1000, "fire");
   }
}

/// <summary>
/// This function is used by the Tower Tool to access the tower's range
/// </summary>
/// <return>Returns the tower's range in world units</return>
function TowerShootsBehavior::getRadius(%this)
{
   return %this.radius;
}

/// <summary>
/// This callback function fires the tower's projectile when the fire animation ends
/// if the tower is supposed to fire on animation end.
/// </summary>
function TowerShootsBehavior::onAnimationEnd(%this)
{
    //echo(" -- TowerShootsBehavior::onAnimationEnd()");
    if (%this.owner.fireOnStartAnim || !%this.isToggled)
    {
        setFlip(%this.animFlipArray[%this.direction], %this.owner);
        %this.owner.playAnimation(%this.animArray[%this.direction]);
        return;
    }
    %this.shootProjectile();
    %this.owner.setSpeedFactor(1);
    setFlip(%this.animFlipArray[%this.direction], %this.owner);
    %this.owner.playAnimation(%this.animArray[%this.direction]);
}

/// <summary>
/// This function is used by the Tower Tool to access the tower's range in user-friendly
/// vocabulary.
/// </summary>
/// <return>Returns Shortest, Short, Average, Long or Longest for the tower tool range display</return>
function TowerShootsBehavior::getRange(%this)
{
   switch (%this.range)
   {
      case $ShortestRange:
         return "Shortest";   
      
      case $ShortRange:
         return "Short";
      
      case $AverageRange:
         return "Average";
      
      case $LongRange:
         return "Long";
      
      case $LongestRange:
         return "Longest";
   }
}

/// <summary>
/// This function is used by the Tower Tool to set the tower's range in user-friendly terms.
/// </summary>
/// <param name="range">A string value of Shortest, Short, Average, Long or Longest that sets the tower's range to the value contained in the approprate global</param>
function TowerShootsBehavior::setRange(%this, %range)
{
   switch$ (%range)
   {
      case "Shortest":
         %this.range = $ShortestRange;      
      
      case "Short":
         %this.range = $ShortRange;
      
      case "Average":
         %this.range = $AverageRange;
      
      case "Long":
         %this.range = $LongRange;
      
      case "Longest":
         %this.range = $LongestRange;
   }
}


/// <summary>
/// This function is used by the Tower Tool to access the tower's rate of fire in user-friendly terms.
/// </summary>
/// <return>A value representing the tower's fire rate: Slowest, Slow, Average, Fast or Fastest</return>
function TowerShootsBehavior::getRateOfFire(%this)
{
   switch (%this.rateOfFire)
   {
      case $SlowestRateOfFire:
         return "Slowest";   
      
      case $SlowRateOfFire:
         return "Slow";
      
      case $AverageRateOfFire:
         return "Average";
      
      case $FastRateOfFire:
         return "Fast";
      
      case $FastestRateOfFire:
         return "Fastest";
   }
}

/// <summary>
/// This function is used to get the tower's rate of fire as a factor from 1 to 10.
/// </summary>
/// <return>Returns an integer representation of the tower's fire speed from 1(slowest) to 10(fastest)</return>
function TowerShootsBehavior::getRateOfFireValue(%this)
{
   return %this.rateOfFire;
}

/// <summary>
/// This function is used by the Tower Tool to set the tower's rate of fire using user-friendly terms.
/// </summary>
/// <param name="rateOfFire">A string value of Slowest, Slow, Average, Fast or Fastest that sets rate of fire value based on the appropriate global value.</param>
function TowerShootsBehavior::setRateOfFire(%this, %rateOfFire)
{
   switch$ (%rateOfFire)
   {
      case "Slowest":
         %this.rateOfFire = $SlowestRateOfFire;      
      
      case "Slow":
         %this.rateOfFire = $SlowRateOfFire;
      
      case "Average":
         %this.rateOfFire = $AverageRateOfFire;
      
      case "Fast":
         %this.rateOfFire = $FastRateOfFire;
      
      case "Fastest":
         %this.rateOfFire = $FastestRateOfFire;
   }
}

/// <summary>
/// This function is used by the Tower Tool to access the tower's projectile type.
/// </summary>
/// <return>The SimObject name of the projectile template object that this tower uses.</return>
function TowerShootsBehavior::getProjectile(%this)
{
   return %this.projectile;
}

/// <summary>
/// This function is used by the Tower Tool to set the tower's projectile type.
/// </summary>
/// <param name="projectile">The SimObject name of the projectile template object that this tower will use.</param>
function TowerShootsBehavior::setProjectile(%this, %projectile)
{
   %this.projectile = %projectile;
}

/// <summary>
/// This function is used by the Tower Tool to access the tower's firing sound audio profile.
/// </summary>
/// <return>The name of the audio profile for the sound that will be played when the tower fires.</param>
function TowerShootsBehavior::getFireSound(%this)
{
   return %this.fireSound;
}

/// <summary>
/// This function is used by the Tower Tool to set the tower's firing sound audio profile.
/// </summary>
/// <param name="fireSound">The name of the audio profile to use for this tower's fire sound.</param>
function TowerShootsBehavior::setFireSound(%this, %fireSound)
{
   %this.fireSound = %fireSound;
}