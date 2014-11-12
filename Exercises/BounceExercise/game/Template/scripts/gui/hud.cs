//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$NumProjectileSlots = 5;
$NumProjectileTypes = 0;

/// <summary>
/// Initializes the HUD GUI upon push
/// </summary>
function HudGui::onDialogPush(%this)
{
    %this.setup();
}

function HudGui::playMusic(%this)
{
    if (AssetDatabase.isDeclaredAsset(MainScene.music))
        %this.playing = alxPlay(MainScene.music);
}

function HudGui::onDialogPop(%this)
{
    %this.setup();
    alxStop(%this.playing);
}

/// <summary>
/// Sets Up the HUD GUI
/// </summary>
function HudGui::setup(%this)
{
    HighScoreDisplay.setText(levelSelectGui.currentWorld.LevelHighScore[$LevelIndex]);
    ScoreDisplay.setText("0");

    $NumProjectileTypes = 0;
    
    for (%i = 0; %i < $NumProjectileSlots; %i++)
    {
        %projectile = MainScene.getFieldValue("AvailProjectile" @ %i);
        
        if (isObject(%projectile))
        {
            (ProjectileSlot @ %i).Projectile = %projectile;
            (ProjectileSlot @ %i).NormalImage = %projectile.IconNormal;
            (ProjectileSlot @ %i).HoverImage = %projectile.IconHover;
            (ProjectileSlot @ %i).DownImage = %projectile.IconDepressed;
            (ProjectileSlot @ %i).InactiveImage = %projectile.IconInactive;
            (ProjectileSlot @ %i @ CountDisplay).setText("x" @ MainScene.getFieldValue("NumAvailable" @ %i));
            (ProjectileSlot @ %i).setActive(MainScene.getFieldValue("NumAvailable" @ %i) > 0);
            
            $NumProjectileTypes++;
            
             if (%i == 0)
             {
                (ProjectileSlot @ %i).setVisible(true);
                (ProjectileSlot @ %i @ CountDisplay).setVisible(true);
                continue;
             }
        }
                
        (ProjectileSlot @ %i).setVisible(false);
        (ProjectileSlot @ %i @ CountDisplay).setVisible(false);
    }

    %newProjectile = clonePrefab(ProjectileSlot0.Projectile, true);
    ProjectileSlot::sizeProjectile(%newProjectile);

    GameMaster.callOnBehaviors(loadProjectileIntoLauncher, GameMaster.Launcher, %newProjectile);

    $ProjectileSelectionVisible = false;
    HudScoreContainer.setVisible(%this.showScore);

    alxStopAll();
    %this.playMusic();
}

/// <summary>
/// Pauses the Game and Displays the Pause Menu
/// </summary>
function hudPauseButton::onClick(%this)
{
    Canvas.pushDialog(pauseGui);
}

/// <summary>
/// Handles a Mouse Enter event for a Projectile Slot Container
/// </summary>
function ProjectileSlotContainer::onMouseEnter(%this)
{
    if (sceneWindow2D.lockedObjectSet.getCount() > 0)
        %this.setProfile("GuiModelessDialogProfile");
}

/// <summary>
/// Handles a Mouse Leave event for a Projectile Slot Container
/// </summary>
function ProjectileSlotContainer::onMouseLeave(%this)
{
    if (%this.profile $= "GuiModelessDialogProfile")
        %this.setProfile("GuiTranparentProfile");
}

/// <summary>
/// Handles a Mouse Up event for a Projectile Slot Container
/// </summary>
/// <param name="%touchId">The Touch ID associated with Mouse Up event</param>
/// <param name="%worldPos">The World Position of the Mouse Up event </param>
function ProjectileSlotContainer::onMouseUp(%this, %touchId, %worldPos)
{
    if (sceneWindow2D.lockedObjectSet.getCount() > 0)
        GameMaster.Launcher.callOnBehaviors("onTouchUp", %touchId, %worldPos);
}

/// <summary>
/// Handles a Mouse Enter event for a Projectile Slot button
/// </summary>
function ProjectileSlot::onMouseEnter(%this)
{
    if (sceneWindow2D.lockedObjectSet.getCount() > 0)
        %this.setProfile("GuiModelessDialogProfile");
}

/// <summary>
/// Handles a Mouse Leave event for a Projectile Slot button
/// </summary>
function ProjectileSlot::onMouseLeave(%this)
{
    if (%this.profile $= "GuiModelessDialogProfile")
        %this.setProfile("GuiDefaultProfile");
}

/// <summary>
/// Handles a Mouse Up event for a Projectile Slot button
/// </summary>
/// <param name="%touchId">The Touch ID associated with Mouse Up event</param>
/// <param name="%worldPos">The World Position of the Mouse Up event </param>
function ProjectileSlot::onMouseUp(%this, %touchId, %worldPos)
{
    if (sceneWindow2D.lockedObjectSet.getCount() > 0)
        GameMaster.Launcher.callOnBehaviors("onTouchUp", %touchId, %worldPos);
}

/// <summary>
/// Handles a Click event on the First Projectile Slot
/// </summary>
function ProjectileSlot0::onClick(%this)
{
    if (!$ProjectileSelectionVisible)
        ProjectileSlotContainer.setProjectileSelectionMenuVisiblity(true);
    else
        %this.selectProjectile(true);
}

/// <summary>
/// Handles a Click event on the a Projectile Slot
/// </summary>
function ProjectileSlot::onClick(%this)
{
    %this.selectProjectile(true);
}

/// <summary>
/// Sizes a Projectile based on its Idle In Launcher animation
/// </summary>
/// <param name="projectile">The Projectile to Size</param>
function ProjectileSlot::sizeProjectile(%projectile)
{
    %behaviorCount = %projectile.getBehaviorCount();
            
    %asset = "";

    for (%i = 0; %i < %behaviorCount; %i++)
    {
        %behavior = %projectile.getBehaviorByIndex(%i);
        
        if ((%behavior.template.getName() $= "AnimationEffectBehavior") && (%behavior.instanceName $= "IdleInLauncher"))
        {
            %asset = %behavior.asset;
            break;
        }
    }

    if (AssetDatabase.getAssetType(%asset) $= "AnimationAsset")
    {
        %animationAsset = AssetDatabase.acquireAsset(%asset);
        %animationImageMapAsset = AssetDatabase.acquireAsset(strchr(%animationAsset.imagemap, "{"));
        %projectile.setSize(Vector2Scale(%animationImageMapAsset.getFrameSize(0), $PhysicsLauncherTools::MetersPerPixel));
        AssetDatabase.releaseAsset(%animationImageMapAsset.getAssetId());
        AssetDatabase.releaseAsset(%animationAsset.getAssetId());
    }
    else
    {
        %imageMapAsset = AssetDatabase.acquireAsset(%asset);
        %projectile.setSize(Vector2Scale(%imageMapAsset.getFrameSize(0), $PhysicsLauncherTools::MetersPerPixel));
        AssetDatabase.releaseAsset(%imageMapAsset.getAssetId());
    }
}

/// <summary>
/// Attempts to Load the Selected Projectile
/// </summary>
/// <param name="loadIntoLauncher">True if the selected projectile should be Loaded Into the Launcher</param>
function ProjectileSlot::selectProjectile(%this, %loadIntoLauncher)
{
    if (ProjectileSlot0.Projectile.getInternalName() !$= %this.Projectile.getInternalName())
    {
        if (%loadIntoLauncher)
        {
            GameMaster.callOnBehaviors(unloadLauncher);

            %projectile = clonePrefab(%this.Projectile, true);
            
            ProjectileSlot::sizeProjectile(%projectile);
            
            GameMaster.callOnBehaviors(loadProjectileIntoLauncher, GameMaster.Launcher, %projectile);
        }
    
        // Cycle Projectiles in Slots
            
        ProjectileSlot0.Projectile = %this.Projectile;

        ProjectileSlot0.NormalImage = %this.Projectile.IconNormal;
        ProjectileSlot0.HoverImage = %this.Projectile.IconHover;
        ProjectileSlot0.DownImage = %this.Projectile.IconDepressed;
        ProjectileSlot0.InactiveImage = %this.Projectile.IconInactive;     
        ProjectileSlot0CountDisplay.setText("x" @ GameMaster.getFieldValue(%this.Projectile.callOnBehaviors(getAdjustableValueField)));   
        
        for (%i = 0; %i < $NumProjectileSlots; %i++)
        {
            if (%this.Projectile $= MainScene.getFieldValue("AvailProjectile" @ %i))
            {
                for (%j = 1; %j < $NumProjectileTypes; %j++)
                {
                    %i++;                                        
                                        
                    if (%i == $NumProjectileTypes)
                        %i = 0;
                        
                    %projectile = MainScene.getFieldValue("AvailProjectile" @ %i);
                    
                    if (isObject(%projectile))
                    {
                        (ProjectileSlot @ %j).Projectile = %projectile;
                        
                        (ProjectileSlot @ %j).NormalImage = %projectile.IconNormal;
                        (ProjectileSlot @ %j).HoverImage = %projectile.IconHover;
                        (ProjectileSlot @ %j).DownImage = %projectile.IconDepressed;
                        (ProjectileSlot @ %j).InactiveImage = %projectile.IconInactive;
                        
                        %projectileCount = GameMaster.getFieldValue(%projectile.callOnBehaviors(getAdjustableValueField));
                        (ProjectileSlot @ %j @ CountDisplay).setText("x" @ %projectileCount);
                        (ProjectileSlot @ %j).setActive(%projectileCount > 0);
                    }
                }
                
                break;
            }
        }
    }
    
    ProjectileSlotContainer.setProjectileSelectionMenuVisiblity(false);
}

function ProjectileSlotContainer::setProjectileSelectionMenuVisiblity(%this, %visible)
{
    $ProjectileSelectionVisible = %visible;
    
    for (%i = 1; %i < $NumProjectileSlots; %i++)
    {
        if (isObject((ProjectileSlot @ %i).Projectile))
        {
            (ProjectileSlot @ %i).setVisible($ProjectileSelectionVisible);
            (ProjectileSlot @ %i @ CountDisplay).setVisible($ProjectileSelectionVisible);
        }
    }
}