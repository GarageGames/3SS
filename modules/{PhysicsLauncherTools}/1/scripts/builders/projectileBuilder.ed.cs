//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Calculate Mass Conversion Ratio
$ProjectileBuilder::ToolMassMin = 0;
$ProjectileBuilder::ToolMassMax = 100;
$ProjectileBuilder::MassMin = 0.0;
$ProjectileBuilder::MassMax = 10.0;
$ProjectileBuilder::MassConversionRatio = ($ProjectileBuilder::ToolMassMax - $ProjectileBuilder::ToolMassMin)
                                                    /($ProjectileBuilder::MassMax - $ProjectileBuilder::MassMin);
// Calculate Friction Conversion Ratio
$ProjectileBuilder::ToolFrictionMin = 1;
$ProjectileBuilder::ToolFrictionMax = 100;
$ProjectileBuilder::FrictionMin = 0.01;
$ProjectileBuilder::FrictionMax = 1;
$ProjectileBuilder::FrictionConversionRatio = ($ProjectileBuilder::ToolFrictionMax - $ProjectileBuilder::ToolFrictionMin)
                                                    /($ProjectileBuilder::FrictionMax - $ProjectileBuilder::FrictionMin);
// Calculate Restitution Conversion Ratio
$ProjectileBuilder::ToolRestitutionMin = 0;
$ProjectileBuilder::ToolRestitutionMax = 100;
$ProjectileBuilder::RestitutionMin = 0;
$ProjectileBuilder::RestitutionMax = 1;
$ProjectileBuilder::RestitutionConversionRatio = ($ProjectileBuilder::ToolRestitutionMax - $ProjectileBuilder::ToolRestitutionMin)
                                                    /($ProjectileBuilder::RestitutionMax - $ProjectileBuilder::RestitutionMin);

$ProjectileBuilder::MaxPointValue = 100000;

/// <summary>
/// This function returns the projectile's calculated mass (0 ~ 100).
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the mass calculated based on density and area.</return>
function ProjectileBuilder::getProjectileMass(%projectile)
{
    %mass = PhysicsLauncherTools::getObjectMass(%projectile);
    %adjustedMass = %mass * $ProjectileBuilder::MassConversionRatio;
    return mRound(%adjustedMass);
}

/// <summary>
/// This function adjusts the projectile's density to achieve the desired mass
/// value.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="mass">The desired projectile mass.</param>
function ProjectileBuilder::setProjectileMass(%projectile, %mass)
{
    if (%mass > $ProjectileBuilder::ToolMassMax)
        %mass = $ProjectileBuilder::ToolMassMax;
    if (%mass < $ProjectileBuilder::ToolMassMin)
        %mass = $ProjectileBuilder::ToolMassMin;

    %adjustedMass = %mass / $ProjectileBuilder::MassConversionRatio;
    PhysicsLauncherTools::setObjectMass(%projectile, %adjustedMass);
}

/// <summary>
/// This function gets the projectile's current friction value (0 ~ 100).
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the converted friction value of the projectile.</return>
function ProjectileBuilder::getProjectileFriction(%projectile)
{
    %friction = %projectile.getDefaultFriction();
    %adjustedFriction = %friction * $ProjectileBuilder::FrictionConversionRatio;
    return mRound(%adjustedFriction);
}

/// <summary>
/// This function sets the desired friction value on the projectile.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="friction">The desired friction value.</param>
function ProjectileBuilder::setProjectileFriction(%projectile, %friction)
{
    if (%friction > $ProjectileBuilder::ToolFrictionMax)
        %friction = $ProjectileBuilder::ToolFrictionMax;
    if (%friction < $ProjectileBuilder::ToolFrictionMin)
        %friction = $ProjectileBuilder::ToolFrictionMin;

    %adjustedFriction = %friction / $ProjectileBuilder::FrictionConversionRatio;
    %projectile.setDefaultFriction(%adjustedFriction);
}

/// <summary>
/// This function gets the projectile's current restitution value (0 ~ 100).
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the converted restitution value.</param>
function ProjectileBuilder::getProjectileRestitution(%projectile)
{
    %restitution = %projectile.getDefaultRestitution();
    %adjustedRestitution = %restitution * $ProjectileBuilder::RestitutionConversionRatio;
    return mRound(%adjustedRestitution);
}

/// <summary>
/// This function sets the desired restitution value on the projectile.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="friction">The desired restitution value.</param>
function ProjectileBuilder::setProjectileRestitution(%projectile, %restitution)
{
    if (%restitution > $ProjectileBuilder::ToolRestitutionMax)
        %restitution = $ProjectileBuilder::ToolRestitutionMax;
    if (%restitution < $ProjectileBuilder::ToolRestitutionMin)
        %restitution = $ProjectileBuilder::ToolRestitutionMin;

    %adjustedRestitution = %restitution / $ProjectileBuilder::RestitutionConversionRatio;
    %projectile.setDefaultRestitution(%adjustedRestitution);
}

/// <summary>
/// This function sets the desired point value on the projectile.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="friction">The desired point value.</param>
function ProjectileBuilder::setProjectilePointValue(%projectile, %value)
{
    if (%value > $ProjectileBuilder::MaxPointValue)
        %value = $ProjectileBuilder::MaxPointValue;
    if (%value < 0)
        %value = 0;

    %projectile.PointValue = %value;
}

/// <summary>
/// This function gets the point value of the projectile.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the point value.</return>
function ProjectileBuilder::getProjectilePointValue(%projectile)
{
    return (%projectile.PointValue ? %projectile.PointValue : 1);
}

/// <summary>
/// This function sets the sound for the projectile's idle in launcher state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="profile">The asset ID of the state sound.</param>
function ProjectileBuilder::setIdleInLauncherSound(%projectile, %profile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "IdleInLauncher")
                %tempBeh.sound = %profile;
        }
    }
}

/// <summary>
/// This function gets the sound assigned to the projectile's idle in launcher state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the state sound.</return>
function ProjectileBuilder::getIdleInLauncherSound(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "IdleInLauncher")
                return %tempBeh.sound;
        }
    }
}

/// <summary>
/// This function sets the sound for the projectile's in air state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="profile">The asset ID of the state sound.</param>
function ProjectileBuilder::setInAirSound(%projectile, %profile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "InAir")
                %tempBeh.sound = %profile;
        }
    }
}

/// <summary>
/// This function gets the sound assigned to the projectile's in air state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the state sound.</return>
function ProjectileBuilder::getInAirSound(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "InAir")
                return %tempBeh.sound;
        }
    }
}

/// <summary>
/// This function sets the sound for the projectile's hit object state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="profile">The asset ID of the state sound.</param>
function ProjectileBuilder::setHitObjectSound(%projectile, %profile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Hit")
                %tempBeh.sound = %profile;
        }
    }
}

/// <summary>
/// This function gets the sound asssigned to the projectile's hit object state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the state sound.</return>
function ProjectileBuilder::getHitObjectSound(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Hit")
                return %tempBeh.sound;
        }
    }
}

/// <summary>
/// This function sets the sound for the projectile's disappear state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="profile">The asset ID of the state sound.</param>
function ProjectileBuilder::setDisappearSound(%projectile, %profile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Vanish")
                %tempBeh.sound = %profile;
        }
    }
}

/// <summary>
/// This function gets the sound assigned to the projectile's disappear state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the state sound.</return>
function ProjectileBuilder::getDisappearSound(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "SoundEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Vanish")
                return %tempBeh.sound;
        }
    }
}

/// <summary>
/// This function sets the image for the projectile's HUD selection button.  It expects
/// a set of button state images that have identical names and Up, Hover, Down and Inactive
/// suffixes and it sets all of the button state images appropriately if they exist.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="image">The asset ID of the button Up state image.</param>
function ProjectileBuilder::setSwitchButton(%projectile, %image, %state)
{
    // Stored on the object in the Icon field
    switch (%state)
    {
        case 0:
            %projectile.IconNormal = %image;
        case 1:
            %projectile.IconHover = %image;
        case 2:
            %projectile.IconDepressed = %image;
        case 3:
            %projectile.IconInactive = %image;
    }
}

/// <summary>
/// This function gets the images for the projectile's HUD selection button.  The 
/// images are returned as asset ID's in a space-delimited string.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID set of the button state images.</return>
function ProjectileBuilder::getSwitchButton(%projectile)
{
    // Stored on the object in the Icon field
    %imageSet = %projectile.IconNormal SPC %projectile.IconHover SPC %projectile.IconDepressed SPC %projectile.IconInactive;

    if (getWordCount(%imageSet) < 2 || %imageSet $= "   ")
        %imageSet = "{PhysicsLauncherTools}:NoPreview";

    return %imageSet;
}

/// <summary>
/// This function sets the image for the projectile's in-game tutorial.
/// </summary>
/// <param name="projectile">The projectile to set tutorial for.</param>
/// <param name="image">The tutorial image that will be shown in game.</param>
function ProjectileBuilder::setTutorialImage(%projectile, %image)
{
    %name = %projectile.getName();
    TutorialDataBuilder::createTutorial(%name, %image);
}

/// <summary>
/// This function gets the image for the projectile's in-game tutorial.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the current tutorial image.</return>
function ProjectileBuilder::getTutorialImage(%projectile)
{
    %name = %projectile.getName();
    return TutorialDataBuilder::getTutorialAsset(%name);

    echo(" @@@ invalid projectile or no projectile selected");
}

/// <summary>
/// This function sets the animation for the projectile to use in its idle in 
/// launcher state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="anim">The asset ID of the animation to use.</param>
function ProjectileBuilder::setIdleInLauncherAnim(%projectile, %anim, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "IdleInLauncher")
            {
                %tempBeh.setAsset(%anim);
                if (%frame !$= "")
                    %tempBeh.setFrame(%frame);
            }
        }
    }
}

/// <summary>
/// This function gets the current animation for the projectile's idle in launcher state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID set of the animation.</return>
function ProjectileBuilder::getIdleInLauncherAnim(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "IdleInLauncher")
                return %tempBeh.getAsset();
        }
    }
}

/// <summary>
/// This function sets the animation for the projectile to use in its 
/// in air state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="anim">The asset ID of the animation to use.</param>
function ProjectileBuilder::setInAirAnim(%projectile, %anim, %frame)
{
    // loop through behaviors and look for the In Air animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "InAir")
            {
                %tempBeh.setAsset(%anim);
                if (%frame !$= "")
                    %tempBeh.setFrame(%frame);
            }
        }
    }
}

/// <summary>
/// This function gets the current animation assigned to the in air state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the assigned animation.</return>
function ProjectileBuilder::getInAirAnim(%projectile)
{
    // loop through behaviors and look for the In Air animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "InAir")
                return %tempBeh.getAsset();
        }
    }
}

/// <summary>
/// This function sets the animation for the projectile to use for its hit state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="anim">The asset ID of the animation to assign.</param>
function ProjectileBuilder::setHitAnim(%projectile, %anim, %frame)
{
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Hit")
            {
                %tempBeh.setAsset(%anim);
                if (%frame !$= "")
                    %tempBeh.setFrame(%frame);
            }
        }
    }
}

/// <summary>
/// This function gets the currently assigned animation for the projectile's hit state.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the assigned animation.</return>
function ProjectileBuilder::getHitAnim(%projectile)
{
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Hit")
                return %tempBeh.getAsset();
        }
    }
}

/// <summary>
/// This function sets the animation to use for the projectile's vanish state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="anim">The asset ID of the animation to assign.</param>
function ProjectileBuilder::setVanishAnim(%projectile, %anim, %frame)
{
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "VanishAnim")
            {
                %tempBeh.setAsset(%anim);
                if (%frame !$= "")
                    %tempBeh.setFrame(%frame);
            }
        }
    }
}

/// <summary>
/// This function gets the projectile's currently assigned vanish animation.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the asset ID of the assigned animation.</return>
function ProjectileBuilder::getVanishAnim(%projectile)
{
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "VanishAnim")
                return %tempBeh.getAsset();
        }
    }
}

/// <summary>
/// This function sets the sprite sheet frame to assign to the projectile's 
/// idle in launcher state.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="frame">The desired frame of the assigned image.</param>
function ProjectileBuilder::setIdleInLauncherAnimFrame(%projectile, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "IdleInLauncher")
                %tempBeh.setFrame(%frame);
        }
    }
}

/// <summary>
/// This function gets the sprite sheet frame of the projectile's idle in launcher
/// image.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the frame of the assigned sprite to display.</return>
function ProjectileBuilder::getIdleInLauncherAnimFrame(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "IdleInLauncher")
                return %tempBeh.getFrame();
        }
    }
    return 0;
}

/// <summary>
/// This function sets the sprite sheet frame of the projectile's in air
/// image.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="frame">The desired frame of the assigned image.</param>
function ProjectileBuilder::setInAirAnimFrame(%projectile, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "InAir")
                %tempBeh.setFrame(%frame);
        }
    }
}

/// <summary>
/// This function gets the sprite sheet frame of the projectile's in air
/// image.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the frame of the assigned sprite to display.</return>
function ProjectileBuilder::getInAirAnimFrame(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "InAir")
                return %tempBeh.getFrame();
        }
    }
    return 0;
}

/// <summary>
/// This function sets the sprite sheet frame of the projectile's hit
/// image.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="frame">The desired frame of the assigned image.</param>
function ProjectileBuilder::setHitAnimFrame(%projectile, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Hit")
                %tempBeh.setFrame(%frame);
        }
    }
}

/// <summary>
/// This function gets the sprite sheet frame of the projectile's hit
/// image.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the frame of the assigned sprite to display.</return>
function ProjectileBuilder::getHitAnimFrame(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "Hit")
                return %tempBeh.getFrame();
        }
    }
    return 0;
}

/// <summary>
/// This function sets the sprite sheet frame of the projectile's vanish
/// image.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="frame">The desired frame of the assigned image.</param>
function ProjectileBuilder::setVanishAnimFrame(%projectile, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "VanishAnim")
                %tempBeh.setFrame(%frame);
        }
    }
}

/// <summary>
/// This function gets the sprite sheet frame of the projectile's vanish
/// image.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the frame of the assigned sprite to display.</return>
function ProjectileBuilder::getVanishAnimFrame(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "AnimationEffectBehavior")
        {
            if (%tempBeh.instanceName $= "VanishAnim")
                return %tempBeh.getFrame();
        }
    }
    return 0;
}

/// <summary>
/// This function sets the animation for the projectile's trail image.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="anim">The asset ID of the animation to assign.</param>
function ProjectileBuilder::setImageTrailAnim(%projectile, %anim, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "ImageTrailBehavior")
        {
            %tempBeh.asset = %anim;
            if (%frame !$= "")
                %tempBeh.setFrame(%frame);
        }
    }
}

/// <summary>
/// This function sets the animation for the projectile's trail image.
/// </summary>
/// <param name="projectile">The projectile to set.</param>
/// <param name="anim">The asset ID of the animation to assign.</param>
function ProjectileBuilder::setImageTrailFrame(%projectile, %frame)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "ImageTrailBehavior")
            %tempBeh.setFrame(%frame);
    }
}

/// <summary>
/// This function gets the animation assigned to the projectile's trail image.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the frame of the assigned sprite to display.</return>
function ProjectileBuilder::getImageTrailAnim(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "ImageTrailBehavior")
            return %tempBeh.asset;
    }
}

/// <summary>
/// This function gets the animation assigned to the projectile's trail image.
/// </summary>
/// <param name="projectile">The projectile to query.</param>
/// <return>Returns the frame of the assigned sprite to display.</return>
function ProjectileBuilder::getImageTrailFrame(%projectile)
{
    // loop through behaviors and look for the Idle In Launcher animation behavior
    for(%i = 0; %i < %projectile.getBehaviorCount(); %i++)
    {
        %tempBeh = %projectile.getBehaviorByIndex(%i);
        if (%tempBeh.template.getName() $= "ImageTrailBehavior")
            return %tempBeh.getFrame();
    }
}

/// <summary>
/// This function creates a new projectile from the default prefab.  If the default
/// prefab does not exist, it falls back on the createDefaultProjectile() method.
/// </summary>
/// <return>Returns a new projectile.</return>
function ProjectileBuilder::createProjectile()
{
    %templateObject = $PrefabTemplateSet.findObjectByInternalName("DefaultProjectile");
    if (isObject(%templateObject))
    {
        %object = %templateObject.clone();
        %object.Icon = %templateObject.Icon;
        %mass = ProjectileBuilder::getProjectileMass(%object);
        ProjectileBuilder::setProjectileMass(%object, %mass);
    }
    else
    {
        warn("createProjectile -- Creating Default Projectile.");
        %object = ProjectileBuilder::createDefaultProjectile();
        $PrefabTemplateSet.add(%object);
        %mass = ProjectileBuilder::getProjectileMass(%object);
        ProjectileBuilder::setProjectileMass(%object, %mass);
    }
    
    //Test
    //%testFile = expandPath("./test.taml");
    //TamlWrite(%object, %testFile);
        
    return %object;
}

/// <summary>
/// This function creates a projectile with base default values to be used as the
/// default projectile prefab.
/// </summary>
/// <return>Returns a new 'default' projectile.</return>
function ProjectileBuilder::createDefaultProjectile()
{
    %projectile = new Sprite(){
        animationName = "PL_DefaultProjectileAnim";
        AngularDamping="2.25";
        UseInputEvents="1";
		DefaultDensity="2.0";
		DefaultFriction="0.75";
		DefaultRestitution="0.3";
        CollisionCallback="1";
        SleepingCallback="1";
		Active="0";
		SceneLayer="15";
        IconDepressed="{PhysicsLauncherAssets}:GorillaButtonDownImageMap";
        IconHover="{PhysicsLauncherAssets}:GorillaButtonHoverImageMap";
        IconInactive="{PhysicsLauncherAssets}:GorillaButtonInactiveImageMap";
        IconNormal="{PhysicsLauncherAssets}:GorillaButtonUpImageMap";
		PointValue="10000";
        activated="0";
    };

    %projectile.createCircleCollisionShape(0.75);

    %projBeh = ProjectileBehavior.createInstance(); //0
    %projectile.addBehavior(%projBeh);

    %trailBeh = ImageTrailBehavior.createInstance(); //1
    %trailBeh.asset="{PhysicsLauncherAssets}:PL_DefaultPathAnim";
    %projectile.addBehavior(%trailBeh);

    %impactBeh = ImpactImageBehavior.createInstance(); //2
    %impactBeh.asset="{PhysicsLauncherAssets}:PL_DefaultDustAnim";
    %projectile.addBehavior(%impactBeh);

    %restBeh = onRestBehavior.createInstance(); //3
    %projectile.addBehavior(%restBeh);
    
    %launchSoundBeh = SoundEffectBehavior.createInstance();
    %launchSoundBeh.instanceName = "InAir";
    %launchSoundBeh.sound="{PhysicsLauncherAssets}:PL_LaunchSound";
    %projectile.addBehavior(%launchSoundBeh);

    %hitSoundBeh = SoundEffectBehavior.createInstance();
    %hitSoundBeh.instanceName = "Hit";
    %hitSoundBeh.sound="{PhysicsLauncherAssets}:PL_HitSound";
    %projectile.addBehavior(%hitSoundBeh);

    %vanishSoundBeh = SoundEffectBehavior.createInstance();
    %vanishSoundBeh.instanceName = "Vanish";
    %vanishSoundBeh.sound="{PhysicsLauncherAssets}:PL_VanishSound";
    %projectile.addBehavior(%vanishSoundBeh);

    %loadSoundBeh = SoundEffectBehavior.createInstance();
    %loadSoundBeh.instanceName = "IdleInLauncher";
    %loadSoundBeh.sound="{PhysicsLauncherAssets}:PL_StretchSound";
    %projectile.addBehavior(%loadSoundBeh);

    %idleAnimBeh = AnimationEffectBehavior.createInstance();
    %idleAnimBeh.instanceName = "Idle";
    %idleAnimBeh.asset="{PhysicsLauncherAssets}:PL_DefaultProjectileAnim";
    %projectile.addBehavior(%idleAnimBeh);

    %inLauncherAnimBeh = AnimationEffectBehavior.createInstance();
    %inLauncherAnimBeh.instanceName = "IdleInLauncher";
    %inLauncherAnimBeh.asset="{PhysicsLauncherAssets}:PL_GorillaIdleProjectileAnim";
    %projectile.addBehavior(%inLauncherAnimBeh);

    %inAirAnimBeh = AnimationEffectBehavior.createInstance();
    %inAirAnimBeh.instanceName = "InAir";
    %inAirAnimBeh.asset="{PhysicsLauncherAssets}:PL_GorillaFlightProjectileAnim";
    %projectile.addBehavior(%inAirAnimBeh);

    %hitAnimBeh = AnimationEffectBehavior.createInstance();
    %hitAnimBeh.instanceName = "Hit";
    %hitAnimBeh.asset="{PhysicsLauncherAssets}:PL_GorillaTumbleProjectileAnim";
    %projectile.addBehavior(%hitAnimBeh);

    %vanishAnimBeh = AnimationEffectBehavior.createInstance();
    %vanishAnimBeh.instanceName = "VanishAnim";
    %vanishAnimBeh.asset="{PhysicsLauncherAssets}:PL_DefaultDustAnim";
    %projectile.addBehavior(%vanishAnimBeh);

    %launchableBeh = LaunchableBehavior.createInstance(); //12
    %projectile.addBehavior(%launchableBeh);

    %loadableBeh = LoadableBehavior.createInstance(); //13
    %projectile.addBehavior(%loadableBeh);

    %vanishBeh = VanishBehavior.createInstance(); //14
    %projectile.addBehavior(%vanishBeh);

    %damageModBeh = DamageModifierBehavior.createInstance(); //15
    %damageModBeh.damageModifierGroups = $WorldObjectBuilder::ProjectileInstantKillDamageGroup;
    %damageModBeh.destroyOnDamage = 1;
    %projectile.addBehavior(%damageModBeh);

    %adjustGMBeh = AdjustGMValueBehavior.createInstance(); //16
    %adjustGMBeh.adjustment="-1";
    %projectile.addBehavior(%adjustGMBeh);

    %templateBeh = TemplateBehavior.createInstance(); //17
    %projectile.addBehavior(%templateBeh);

    %projectile.Connect(%projBeh, %trailBeh, StartMove, Start);
    %projectile.Connect(%projBeh, %trailBeh, EndMove, End);
    %projectile.Connect(%projBeh, %impactBeh, Hit, Impact);
    %projectile.Connect(%launchableBeh, %restBeh, launched, Activate);
    %projectile.Connect(%launchableBeh, %projBeh, launched, launchedInput);
    
    %projectile.Connect(%loadableBeh, %inLauncherAnimBeh, loaded, Play);
    %projectile.Connect(%launchableBeh, %inAirAnimBeh, launched, Play);
    %projectile.Connect(%projBeh, %hitAnimBeh, EndMove, Play);
    
    %projectile.Connect(%vanishBeh, %vanishSoundBeh, Vanishing, PlaySound);
    %projectile.Connect(%vanishBeh, %vanishAnimBeh, Vanishing, Play);
    %projectile.Connect(%projBeh, %hitSoundBeh, Hit, PlaySound);
    %projectile.Connect(%loadableBeh, %loadSoundBeh, loaded, PlaySound);
    %projectile.Connect(%launchableBeh, %launchSoundBeh, launched, PlaySound);
    %projectile.Connect(%launchableBeh, %adjustGMBeh, launched, adjustGMValue);
    
    %projectile.Connect(%restBeh, %vanishBeh, Resting, Vanish);

    //TamlWrite(%projectile, "./projectile.taml");
    return %projectile;
}

/// <summary>
/// This function opens the collision editor for the desired projectile.
/// </summary>
/// <param name="projectile">The projectile to edit.</param>
/// <param name="invokingGui">The tool to return to (usually the calling tool).</param>
function ProjectileBuilder::openCollisionEditor(%projectile, %invokingGui)
{
    %proxyObject = ProjectileBuilder::constructCollisionShapeProxy(%projectile);

    if(%invokingGui != CollisionSidebar.getId())
        CollisionSidebar.load(1, true);
    
    CollisionEditorGui.open(%proxyObject, %invokingGui);
    
    // Add state information
    CollisionEditorGui.clearStateEntries();
    CollisionEditorGui.addStateEntry("Idle in launcher", ProjectileBuilder::getIdleInLauncherAnim(%projectile), ProjectileBuilder::getIdleInLauncherAnimFrame(%projectile));
    CollisionEditorGui.addStateEntry("In air", ProjectileBuilder::getInAirAnim(%projectile), ProjectileBuilder::getInAirAnimFrame(%projectile));
    CollisionEditorGui.addStateEntry("Hit object", ProjectileBuilder::getHitAnim(%projectile), ProjectileBuilder::getHitAnimFrame(%projectile));
    CollisionEditorGui.addStateEntry("Disappear", ProjectileBuilder::getVanishAnim(%projectile), ProjectileBuilder::getVanishAnimFrame(%projectile));
}

/// <summary>
/// This function constructs a proxy for a projectile that can be passed to the collision editor.
/// </summary>
/// <param name="projectile">The projectile to create a proxy for.</param>
/// <return>Returns a projectile proxy object.</return>
function ProjectileBuilder::constructCollisionShapeProxy(%projectile)
{
    %proxyObject = new Sprite();
    %proxyObject.setPosition(%projectile.getPosition());
    %proxyObject.objectName = %projectile.getInternalName();
    
    %stateAssetID = ProjectileBuilder::getIdleInLauncherAnim(%projectile);
    %stateAsset = AssetDatabase.acquireAsset(%stateAssetID);
    %type = %stateAsset.getClassName();
    AssetDatabase.releaseAsset(%stateAssetID);

    switch$(%type)
    {
        case "AnimationAsset":
            %proxyObject.Animation=%stateAssetID;
        case "ImageAsset":
            %proxyObject.Image=%stateAssetID;
    }

    %proxyObject.setSizeFromAsset(%stateAssetID, $PhysicsLauncherTools::MetersPerPixel);

    PhysicsLauncherTools::copyCollisionShapes(%projectile, %proxyObject);
    
    return %proxyObject;
}

/// <summary>
/// This function sets collision shapes on a projectile from a proxy
/// </summary>
/// <param name="projectile">The projectile to set collision information on.</param>
/// <param name="proxyObject">The proxy that has the desired collision information.</param>
function ProjectileBuilder::setCollisionShapesFromProxy(%projectile, %proxyObject)
{
    PhysicsLauncherTools::copyCollisionShapes(%proxyObject, %projectile);
}

/// <summary>
/// This function looks for a projectile in a level file
/// </summary>
/// <param name="projectile">The projectile to find.</param>
/// <param name="%level">The file name of the level to search.</param>
/// <param name="%visitor">A TAML visitor object that will actually perform the search.</param>
function ProjectileBuilder::findProjectileInLevel(%projectile, %level, %visitor)
{
    %found = false;
    for (%i = 0; %i < 5; %i++)
    {
        if ( %visitor.containsValue(%level, "AvailProjectile"@%i, %projectile) )
            %found = true;
    }
    return %found;
}

/// <summary>
/// This function searches all level files in a project for a projectile
/// </summary>
/// <param name="projectile">The projectile to find.</param>
function ProjectileBuilder::findProjectileInAllLevels(%projectile)
{
    %path = expandPath("^{UserGame}/data/levels/"); 
    %fileSpec = "/*.scene.taml";   

    %blankFile = %path @ %fileSpec;
    %levelList = PhysicsLauncherTools::getLevelFileList();

    // create the visitor here to prevent needing to create and destroy this
    // object dozens or hundreds of times.
    %visitor = new TamlXmlFileVisitor();

    %dependencies = "";
    %count = getFieldCount(%levelList);
    for (%i = 0; %i < %count; %i++)
    {
        %file = %path @ getField(%levelList, %i) @ ".scene.taml";
        if ( ProjectileBuilder::findProjectileInLevel(%projectile, %file, %visitor) )
        {
            %levelName = fileBase(%file);
            %name = strreplace(%levelName, ".scene", "");
            %temp = %name @ " " @ %dependencies;
            %dependencies = %temp;
        }
    }
    %visitor.delete();
    return %dependencies;
}

/// <summary>
/// cleans a projectile and its number available from a level
/// </summary>
/// <param name="projectile">The projectile to remove from the scene</param>
/// <param name="level">The level file to remove the projectile from</param>
function ProjectileBuilder::removeProjectileFromLevel(%projectile, %level)
{
    %k = 0;
    for (%i = 0; %i < 5; %i++)
    {
        if (%level.AvailProjectile[%i] !$= %projectile)
        {
            %projectileList[%k] = %level.AvailProjectile[%i];
            %ammoList[%k] = %level.NumAvailable[%i];
            %k++;
        }
    }
    for (%i = 0; %i < 5; %i++)
    {
        if (%i < %k)
        {
            %level.AvailProjectile[%i] = %projectileList[%i];
            %level.NumAvailable[%i] = %ammoList[%i];
        }
        else
        {
            %level.AvailProjectile[%i] = "";
            %level.NumAvailable[%i] = "0";
        }
    }
}

/// <summary>
/// cleans a projectile and its number available from all levels
/// </summary>
/// <param name="projectile">The projectile to remove from the scene</param>
function ProjectileBuilder::removeProjectileFromAllLevels(%projectile, %level)
{
    %path = expandPath("^gameTemplate/data/levels"); 
    %fileSpec = "/*.scene.taml";   
    %pattern = %path @ %fileSpec;
    
    %file = findFirstFile(%pattern);

    while(%file !$= "")
    {
        %level = TamlRead(%file);
        ProjectileBuilder::removeProjectileFromLevel(%projectile, %level);
        %level.delete();
        %file = findNextFile(%pattern);
    }
}