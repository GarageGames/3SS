//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$LaucherReloadDelay = 1500;

if(!isObject(PhysicsLauncherGMBehavior))
{
    %template = new BehaviorTemplate(PhysicsLauncherGMBehavior);
    %template.friendlyName = "Physics Launcher Game Master";
    %template.behaviorType = "general";
    %template.description = "Game Master Behavior for the Physics Launcher";
    
    %template.addBehaviorField(scoreDisplayObject, "Object on which to Display the Score", object, "", BitmapFontObject);
    %template.addBehaviorField(projectileSlot0IconObject, "Object on which to Display the Projectile Slot 0 Icon", object, "", GuiBitmapButtonCtrl);
    %template.addBehaviorField(projectileSlot0CountDisplayObject, "Object on which to Display the Projectile Slot 0 Count", object, "", BitmapFontObject);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function PhysicsLauncherGMBehavior::onBehaviorAdd(%this)
{
    
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function PhysicsLauncherGMBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
    %this.gameOver = false;
}

function PhysicsLauncherGMBehavior::onLevelLoaded(%this, %scene)
{
    %this.initializeLevel();
    $ActiveObjectSet = new SimSet();
    %this.schedule(5000, checkGameScore);

    %this.owner.Launcher = LauncherSceneGroup.findObjectByInternalName("BuilderObject");
}

/// <summary>
/// Registered callback for the _Cleanup event - checks the current total projectile
/// count.  If there are no more available projectiles it schedules a call to 
/// checkLoseCondition() to give the last projectile time to destroy more win 
/// condition objects.
/// </summary>
function PhysicsLauncherGMBehavior::checkGameScore(%this)
{
    %objCount = $ActiveObjectSet.getCount();
    //echo(" @@@ active object count: " @ %objCount @ " win objective count : " @ %this.owner.WinObjectiveCount);
    if ($ActiveObjectSet.getCount() == 0)
    {
        %this.getProjectileCount();
        if (%this.owner.WinObjectiveCount == 0)
        {
            %this.gameOver = true;
            ScheduleManager.scheduleEvent(1000, %this, "checkWinCondition");
        }
        else if (%this.projectileCount <= 0 && %this.owner.WinObjectiveCount > 0)
        {
            %this.gameOver = true;
            %this.loseSchedule = ScheduleManager.scheduleEvent(1000, %this, "checkLoseCondition");
        }
    }

    if (!%this.gameOver)
        %this.checkSchedule = ScheduleManager.scheduleEvent(1000, %this, "checkGameScore");
}

/// <summary>
/// Initializes a Physics Launcher level
/// </summary>
function PhysicsLauncherGMBehavior::initializeLevel(%this)
{
    %this.owner.Score = 0;
    %this.updateScoreDisplay();

    for (%i = 0; %i < $NumProjectileSlots; %i++)
    {
        %projectile = MainScene.getFieldValue(AvailProjectile @ %i);
        
        %field = "Projectile" @ %i @ "Count";
        
        %this.owner.setFieldValue(%field, 0);
        
        if (isObject(%projectile))
        {
            %this.owner.setFieldValue(%field, MainScene.getFieldValue(NumAvailable @ %i));
            %projectile.callOnBehaviors(setAdjustableValueField, %field);
        }
    }
}

/// <summary>
/// Function that applies a value adjustment
/// </summary>
/// <param name="%adjustment">Field to adjust</param>
/// <param name="%adjustment">Amount by which to Adjust the Value</param>
function PhysicsLauncherGMBehavior::adjustValue(%this, %field, %adjustment)
{
    %this.owner.setFieldValue(%field, %this.owner.getFieldValue(%field) + %adjustment); 
    
    switch$ (%field)
    {
        case "Score":
            %this.updateScoreDisplay();
            
        case "Projectile0Count":
            %this.updateProjectileSlot0CountDisplay(Projectile0);
            
        case "Projectile1Count":
            %this.updateProjectileSlot0CountDisplay(Projectile1);
            
        case "Projectile2Count":
            %this.updateProjectileSlot0CountDisplay(Projectile2);
            
        case "Projectile3Count":
            %this.updateProjectileSlot0CountDisplay(Projectile3);
            
        case "Projectile4Count":
            %this.updateProjectileSlot0CountDisplay(Projectile4);
            
        default:
    }
}

/// <summary>
/// Updates the Score Display on the HUD
/// </summary>
function PhysicsLauncherGMBehavior::updateScoreDisplay(%this)
{
    if (isObject(%this.scoreDisplayObject))
        %this.scoreDisplayObject.setText(%this.owner.Score);
}

/// <summary>
/// Updates the Projectile Slot 0 Count Display on the HUD
/// </summary>
/// <param name="%projectile">Projectile currently assigned to Slot 0</param>
function PhysicsLauncherGMBehavior::updateProjectileSlot0CountDisplay(%this, %projectile)
{
    %this.projectileSlot0IconObject.setActive(false);
    
    %count = %this.owner.getFieldValue(%projectile @ Count);
    %this.projectileSlot0CountDisplayObject.setText("x" @ %count);
    
    if (%this.getProjectileCount() == 0)
        return;

    if (%count == 0)
    {
        for (%i = 1; %i < $NumProjectileSlots; %i++)
        {
            %nextProjectile = (ProjectileSlot @ %i).Projectile;
            
            if (%this.owner.getFieldValue(%nextProjectile.callOnBehaviors(getAdjustableValueField)) > 0)
            {
                (ProjectileSlot @ %i).selectProjectile(false);
                %nextProjectile = clonePrefab(%nextProjectile, true);
                ProjectileSlot::sizeProjectile(%nextProjectile);
                ScheduleManager.scheduleEvent($LaucherReloadDelay, %this, "loadProjectileIntoLauncher", %this.owner.Launcher, %nextProjectile);
                break;
            }
        }
    }
    else
    {
        %projectile =  clonePrefab(MainScene.getFieldValue(Avail @ %projectile), true);
        ProjectileSlot::sizeProjectile(%projectile);
        ScheduleManager.scheduleEvent($LaucherReloadDelay, %this, "loadProjectileIntoLauncher", %this.owner.Launcher, %projectile);
    }
}

/// <summary>
/// Loads the Given Projectile into the Launcher
/// </summary>
/// <param name="%projectile">Projectile to Load</param>
function PhysicsLauncherGMBehavior::loadProjectileIntoLauncher(%this, %launcher, %projectile)
{
    // get a current count of all available projectiles.
    %this.getProjectileCount();

    //echo(" @@@ loading " @ %projectile @ " : " @ %projectile.getClassName());

    if (isObject(%projectile) && (%this.projectileCount == 1))
        %this.lastProjectile = %projectile;

    %this.owner.Launcher.callOnBehaviors(load, %projectile);
    
    %this.projectileSlot0IconObject.setActive(true);
}

function PhysicsLauncherGMBehavior::unloadLauncher(%this)
{
    if (isObject(%this.owner.Launcher.callOnBehaviors(getLoadedObject)))
        %this.owner.Launcher.callOnBehaviors(unload);
}

function PhysicsLauncherGMBehavior::getProjectileCount(%this)
{
    %projectileCount = 0;
    
    for (%i = 0; %i < $NumProjectileSlots; %i++)
    {
        %value = MainScene.AvailProjectile[%i].PointValue;
        %projectileCount += %this.owner.getFieldValue("Projectile" @ %i @ "Count");
        %projectileScore += %this.owner.getFieldValue("Projectile" @ %i @ "Count") * %value;
    }
    %this.owner.Bonus = %projectileScore;
    %this.projectileCount = %projectileCount;
}

/// <summary>
/// Scheduled from the checkForLastProjectile callback if it receives an event and
/// the current projectile count <= 0.  It is called on a hard-coded 5 second 
/// schedule at the moment to give some time for the last projectile to actually 
/// destroy existing win condition objects.
/// </summary>
function PhysicsLauncherGMBehavior::checkLoseCondition(%this)
{
    Canvas.pushDialog(loseGui);
}

/// <summary>
/// Called if the final win condition object in the scene is destroyed.  Cancels
/// a scheduled call to checkLoseCondition() if one is pending.
/// </summary>
function PhysicsLauncherGMBehavior::checkWinCondition(%this)
{
    if (%this.loseSchedule)
        ScheduleManager.cancelEvent(%this.loseSchedule);

    %this.getProjectileCount();

    Canvas.pushDialog(winGui);
}