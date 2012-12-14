//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$WorldObjectBuilder::WorldObjectPrefabName = "WorldObjectPrefab";

$WorldObjectBuilder::ProjectileInstantKillDamageGroup = 1;

$WorldObjectBuilder::WorldObjectSceneLayer = 15;

$WorldObjectBuilder::DisappearStateIndex = 100;

// Calculate Mass Conversion Ratio
$WorldObjectBuilder::ToolMassMin = 0;
$WorldObjectBuilder::ToolMassMax = 100;
$WorldObjectBuilder::MassMin = 0.0;
$WorldObjectBuilder::MassMax = 10.0;
$WorldObjectBuilder::MassConversionRatio = ($WorldObjectBuilder::ToolMassMax - $WorldObjectBuilder::ToolMassMin)
                                                    /($WorldObjectBuilder::MassMax - $WorldObjectBuilder::MassMin);
// Calculate Friction Conversion Ratio
$WorldObjectBuilder::ToolFrictionMin = 1;
$WorldObjectBuilder::ToolFrictionMax = 100;
$WorldObjectBuilder::FrictionMin = 0.01;
$WorldObjectBuilder::FrictionMax = 1;
$WorldObjectBuilder::FrictionConversionRatio = ($WorldObjectBuilder::ToolFrictionMax - $WorldObjectBuilder::ToolFrictionMin)
                                                    /($WorldObjectBuilder::FrictionMax - $WorldObjectBuilder::FrictionMin);
// Calculate Restitution Conversion Ratio
$WorldObjectBuilder::ToolRestitutionMin = 0;
$WorldObjectBuilder::ToolRestitutionMax = 100;
$WorldObjectBuilder::RestitutionMin = 0;
$WorldObjectBuilder::RestitutionMax = 1;
$WorldObjectBuilder::RestitutionConversionRatio = ($WorldObjectBuilder::ToolRestitutionMax - $WorldObjectBuilder::ToolRestitutionMin)
                                                    /($WorldObjectBuilder::RestitutionMax - $WorldObjectBuilder::RestitutionMin);
                                                    
// Hit Points
$WorldObjectBuilder::ToolHitPointsMin = 0;
$WorldObjectBuilder::ToolHitPointsMax = 1000;

// Points when destroyed
$WorldObjectBuilder::ToolPointsWhenDestroyedMin = 0;
$WorldObjectBuilder::ToolPointsWhenDestroyedMax = 1000000;

// Number of damage states
$WorldObjectBuilder::ToolNumberOfDamageStatesMin = 0;
$WorldObjectBuilder::ToolNumberOfDamageStatesMax = $DamageableBehaviorMaxDamageStates - 1;

$WorldObjectBuilder::MinSpeedDamageThreshold = 0.05;

// Factory function for WorldObjects
function WorldObjectBuilder::getNewWorldObject()
{
    //%templateObject = $PrefabTemplateSet.findObjectByInternalName("DefaultWorldObject");
    //%object = %templateObject.clone();
    //
    //if (!isObject(%object))
    //{
        //warn("createWorldObject -- Unable to create new WorldObject.");
        //return "";
    //}
        //
    //return %object;
    
    return WorldObjectBuilder::createWorldObjectTemplate();
}

function WorldObjectBuilder::createWorldObjectTemplate()
{
    // Generate a unique name for the object
    %worldObjectCount = 0;
    %worldObjectName = $WorldObjectBuilder::WorldObjectPrefabName @ %worldObjectCount;
    while (isObject(%worldObjectName))
    {
        %worldObjectCount++;
        %worldObjectName = $WorldObjectBuilder::WorldObjectPrefabName @ %worldObjectCount;
    }    
    
    %worldObject = new Sprite(%worldObjectName);
    %worldObject.animation = PL_DefaultWorldObjectAnim0;
    %worldObject.setBodyType("dynamic");
    %worldObject.setSceneLayer($WorldObjectBuilder::WorldObjectSceneLayer);
    
    %width = getWord(%worldObject.size, 0) - 0.05;
    %height = getWord(%worldObject.size, 1) - 0.05;
    %shape = %worldObject.createPolygonBoxCollisionShape(%width, %height);
    %worldObject.setCollisionCallback(true);
    
    %worldObjectBehavior = WorldObjectBehavior.createInstance();
    %worldObject.addBehavior(%worldObjectBehavior);
    PhysicsLauncherTools::setObjectMass(%worldObject, 1.0);
    %worldObjectBehavior.setObjectRestitution(0.3);
    
    %damageableBehavior = DamageableBehavior.createInstance();
    %damageableBehavior.setDamageStates("0.1 0.3 0.6 0.9 1.0");
    %damageableBehavior.setDamageStatesActiveMask("1 1 1 1 1");
    %damageableBehavior.startingHealth = 25;
    %damageableBehavior.minDamageThreshold = 1.0;
    %damageableBehavior.minSpeedThreshold = $WorldObjectBuilder::MinSpeedDamageThreshold;
    %worldObject.addBehavior(%damageableBehavior);
    WorldObjectBuilder::setDamageStateCount(%worldObject, $WorldObjectBuilder::ToolNumberOfDamageStatesMax);
    
    // Add damage animation behaviors
    %idleAnimationBehavior = AnimationEffectBehavior.createInstance();
    %idleAnimationBehavior.instanceName = "IdleAnimationEffectBehavior";
    %idleAnimationBehavior.setAsset("{PhysicsLauncherAssets}:PL_DefaultWorldObjectAnim1");
    %worldObject.addBehavior(%idleAnimationBehavior);
    %worldObject.Connect(%worldObjectBehavior, %idleAnimationBehavior, addedToSceneOutput, Play);
    
    %damageAnimationBehavior0 = AnimationEffectBehavior.createInstance();
    %damageAnimationBehavior0.instanceName = "DamageAnimationEffectBehavior0";
    %damageAnimationBehavior0.setAsset("{PhysicsLauncherAssets}:PL_DefaultWorldObjectAnim1");
    %worldObject.addBehavior(%damageAnimationBehavior0);
    %worldObject.Connect(%damageableBehavior, %damageAnimationBehavior0, $DamageableBehaviorOutputCallString @ "0", Play);
    
    %damageAnimationBehavior1 = AnimationEffectBehavior.createInstance();
    %damageAnimationBehavior1.instanceName = "DamageAnimationEffectBehavior1";
    %damageAnimationBehavior1.setAsset("{PhysicsLauncherAssets}:PL_DefaultWorldObjectAnim2");
    %worldObject.addBehavior(%damageAnimationBehavior1);
    %worldObject.Connect(%damageableBehavior, %damageAnimationBehavior1, $DamageableBehaviorOutputCallString @ "1", Play);
    
    %damageAnimationBehavior2 = AnimationEffectBehavior.createInstance();
    %damageAnimationBehavior2.instanceName = "DamageAnimationEffectBehavior2";
    %damageAnimationBehavior2.setAsset("{PhysicsLauncherAssets}:PL_DefaultWorldObjectAnim3");
    %worldObject.addBehavior(%damageAnimationBehavior2);
    %worldObject.Connect(%damageableBehavior, %damageAnimationBehavior2, $DamageableBehaviorOutputCallString @ "2", Play);
    
    %damageAnimationBehavior3 = AnimationEffectBehavior.createInstance();
    %damageAnimationBehavior3.instanceName = "DamageAnimationEffectBehavior3";
    %damageAnimationBehavior3.setAsset("{PhysicsLauncherAssets}:PL_DefaultWorldObjectAnim3");
    %worldObject.addBehavior(%damageAnimationBehavior3);
    %worldObject.Connect(%damageableBehavior, %damageAnimationBehavior3, $DamageableBehaviorOutputCallString @ "3", Play);

    // Add vanish behaviors
    %vanishBehavior = VanishBehavior.createInstance();
    %vanishBehavior.instanceName = "VanishBehavior";
    %vanishBehavior.vanishTime = 0;
    %vanishDuration.vanishDuration = 1;
    %worldObject.addBehavior(%vanishBehavior);
    %worldObject.Connect(%damageableBehavior, %vanishBehavior, $DamageableBehaviorOutputNoSkipCallString @ "4", Vanish);
    
    %damageAnimationBehavior4 = AnimationEffectBehavior.createInstance();
    %damageAnimationBehavior4.instanceName = "DamageAnimationEffectBehavior4";
    %damageAnimationBehavior4.setAsset("{PhysicsLauncherAssets}:PL_DefaultDustAnim");
    %worldObject.addBehavior(%damageAnimationBehavior4);
    %worldObject.Connect(%vanishBehavior, %damageAnimationBehavior4, Vanishing, Play);
    
    // Sound behaviors
    %idleSoundEffectBehavior = SoundEffectBehavior.createInstance();
    %idleSoundEffectBehavior.instanceName = "IdleSoundEffectBehavior";
    %idleSoundEffectBehavior.sound = PL_DefaultSound; 
    %worldObject.addBehavior(%idleSoundEffectBehavior);
    %worldObject.Connect(%worldObjectBehavior, %idleSoundEffectBehavior, addedToSceneOutput, PlaySound);
    
    %damageSoundEffectBehavior0 = SoundEffectBehavior.createInstance();
    %damageSoundEffectBehavior0.instanceName = "DamageSoundEffectBehavior0";
    %damageSoundEffectBehavior0.sound = PL_DefaultSound; 
    %worldObject.addBehavior(%damageSoundEffectBehavior0);
    %worldObject.Connect(%damageableBehavior, %damageSoundEffectBehavior0, $DamageableBehaviorOutputCallString @ "0", PlaySound);
    
    %damageSoundEffectBehavior1 = SoundEffectBehavior.createInstance();
    %damageSoundEffectBehavior1.instanceName = "DamageSoundEffectBehavior1";
    %damageSoundEffectBehavior1.sound = PL_DefaultSound; 
    %worldObject.addBehavior(%damageSoundEffectBehavior1);
    %worldObject.Connect(%damageableBehavior, %damageSoundEffectBehavior1, $DamageableBehaviorOutputCallString @ "1", PlaySound);
    
    %damageSoundEffectBehavior2 = SoundEffectBehavior.createInstance();
    %damageSoundEffectBehavior2.instanceName = "DamageSoundEffectBehavior2";
    %damageSoundEffectBehavior2.sound = PL_DefaultSound; 
    %worldObject.addBehavior(%damageSoundEffectBehavior2);
    %worldObject.Connect(%damageableBehavior, %damageSoundEffectBehavior2, $DamageableBehaviorOutputCallString @ "2", PlaySound);
    
    %damageSoundEffectBehavior3 = SoundEffectBehavior.createInstance();
    %damageSoundEffectBehavior3.instanceName = "DamageSoundEffectBehavior3";
    %damageSoundEffectBehavior3.sound = PL_DefaultSound; 
    %worldObject.addBehavior(%damageSoundEffectBehavior3);
    %worldObject.Connect(%damageableBehavior, %damageSoundEffectBehavior3, $DamageableBehaviorOutputCallString @ "3", PlaySound);
    
    %destroyedSoundEffectBehavior = SoundEffectBehavior.createInstance();
    %destroyedSoundEffectBehavior.instanceName = "DamageSoundEffectBehavior4";
    %destroyedSoundEffectBehavior.sound = PL_DefaultSound; 
    %worldObject.addBehavior(%destroyedSoundEffectBehavior);
    %worldObject.Connect(%damageableBehavior, %destroyedSoundEffectBehavior, $DamageableBehaviorOutputCallString @ "4", PlaySound);
    
    // Add score behehaviors
    %scoreState0 = AdjustGMValueBehavior.createInstance();    
    %scoreState0.adjustableValueField = Score;
    %scoreState0.adjustment = 100;
    %worldObject.addBehavior(%scoreState0);
    %worldObject.Connect(%damageableBehavior, %scoreState0, $DamageableBehaviorOutputNoSkipCallString @ "0", adjustGMValue);
    
    %scoreState1 = AdjustGMValueBehavior.createInstance();    
    %scoreState1.adjustableValueField = Score;
    %scoreState1.adjustment = 100;
    %worldObject.addBehavior(%scoreState1);
    %worldObject.Connect(%damageableBehavior, %scoreState1, $DamageableBehaviorOutputNoSkipCallString @ "1", adjustGMValue);

    %scoreState2 = AdjustGMValueBehavior.createInstance();    
    %scoreState2.adjustableValueField = Score;
    %scoreState2.adjustment = 100;
    %worldObject.addBehavior(%scoreState2);
    %worldObject.Connect(%damageableBehavior, %scoreState2, $DamageableBehaviorOutputNoSkipCallString @ "2", adjustGMValue);

    %scoreState3 = AdjustGMValueBehavior.createInstance();    
    %scoreState3.adjustableValueField = Score;
    %scoreState3.adjustment = 100;
    %worldObject.addBehavior(%scoreState3);
    %worldObject.Connect(%damageableBehavior, %scoreState3, $DamageableBehaviorOutputNoSkipCallString @ "3", adjustGMValue);

    %scoreState4 = AdjustGMValueBehavior.createInstance();    
    %scoreState4.adjustableValueField = Score;
    %scoreState4.adjustment = 100;
    %worldObject.addBehavior(%scoreState4);
    %worldObject.Connect(%damageableBehavior, %scoreState4, $DamageableBehaviorOutputNoSkipCallString @ "4", adjustGMValue);
    
    // Win Objective Behavior
    %winObjectiveBehvior = WinObjectiveBehavior.createInstance();
    %worldObject.addBehavior(%winObjectiveBehvior);
    
    %adjustGMValueBehaviorOnCreated = AdjustGMValueBehavior.createInstance();
    %adjustGMValueBehaviorOnCreated.adjustableValueField = "WinObjectiveCount";
    %adjustGMValueBehaviorOnCreated.adjustment = "1";
    %worldObject.addBehavior(%adjustGMValueBehaviorOnCreated);
    
    %adjustGMValueBehaviorOnDestroyed = AdjustGMValueBehavior.createInstance();
    %adjustGMValueBehaviorOnDestroyed.adjustableValueField = "WinObjectiveCount";
    %adjustGMValueBehaviorOnDestroyed.adjustment = "-1"; 
    %worldObject.addBehavior(%adjustGMValueBehaviorOnDestroyed);    
    
    %worldObject.Connect(%winObjectiveBehvior, %adjustGMValueBehaviorOnCreated, onCreatedOutput, adjustGMValue);
    %worldObject.Connect(%winObjectiveBehvior, %adjustGMValueBehaviorOnDestroyed, onDestroyedOutput, adjustGMValue);
    
    %worldObject.Connect(%vanishBehavior, %winObjectiveBehvior, vanishedOutput, onDestroyInput);    
    
    return %worldObject;
}

function WorldObjectBuilder::findObjectInLevel(%worldObject, %level)
{
    %count = %level.getSceneObjectCount();
    %name = %worldObject.getInternalName();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %level.getSceneObject(%i);
        if ( %obj.getInternalName() $= %name)
            return true;
    }
    return false;
}

function WorldObjectBuilder::findObjectInAllLevels(%worldObject)
{
    %path = expandPath("^{UserGame}/data/levels"); 
    %fileSpec = "/*.scene.taml";   
    %pattern = %path @ %fileSpec;

    %file = findFirstFile(%pattern);

    %dependencies = "";

    while(%file !$= "")
    {
        %level = TamlRead(%file);
        if ( WorldObjectBuilder::findObjectInLevel(%worldObject, %level) )
        {
            %levelName = fileBase(%file);
            %name = strreplace(%levelName, ".scene", "");
            %temp = %name @ " " @ %dependencies;
            %dependencies = %temp;
        }
        %level.delete();
        %file = findNextFile(%pattern);
    }
    return %dependencies;
}

function WorldObjectBuilder::setName(%object, %name)
{
    %object.setInternalName(%name);
}

function WorldObjectBuilder::getName(%object)
{
    return %object.getInternalName();
}

function WorldObjectBuilder::setMass(%object, %toolMass)
{
    // Scale mass fraction 
    %mass = %toolMass / $WorldObjectBuilder::MassConversionRatio;    
    
    PhysicsLauncherTools::setObjectMass(%object, %mass);
}

function WorldObjectBuilder::getMass(%object)
{
    %mass = PhysicsLauncherTools::getObjectMass(%object);
    
    %toolMass = mRound(%mass * $WorldObjectBuilder::MassConversionRatio);   
    
    return %toolMass;
}

function WorldObjectBuilder::setMoveableFlag(%object, %flagValue)
{
    %object.setBodyType(%flagValue ? "dynamic" : "static");
}

function WorldObjectBuilder::getMoveableFlag(%object)
{
    %flag = (%object.getBodyType() $= "dynamic") ? true : false;
    return %flag;
}

function WorldObjectBuilder::setPointsWhenDestroyed(%object, %points)
{
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);  
    
    // Calculate the points for each damage state
    %remainder = %points % (%damageStateCount + 1);
    %pointsPerState = (%points - %remainder) / (%damageStateCount + 1);
    %pointsForLastState = %pointsPerState + %remainder;
    
    %outputBehaviorTemplateName = "DamageableBehavior";
    %inputBehaviorTemplateName = "AdjustGMValueBehavior";
    %inputName = "adjustGMValue";
    
    // Loop through each damage state and set the points
    for (%i = 0; %i < %damageStateCount; %i++)
    {
        %outputName = $DamageableBehaviorOutputNoSkipCallString @ %i;
        
        // Get the correct adjustGMValueBehavior
        %adjustGMValueBehavior = PhysicsLauncherTools::getBehaviorFromConnection( %object,
                                                                                %outputBehaviorTemplateName, 
                                                                                %inputBehaviorTemplateName, 
                                                                                %outputName, 
                                                                                %inputName);
    
        // Check if we found the behavior
        if (%adjustGMValueBehavior $= "")
            continue;
            
        %adjustGMValueBehavior.adjustment = %pointsPerState;
    }
    
    // Set points for destroyed state
    %lastIndex = $WorldObjectBuilder::ToolNumberOfDamageStatesMax;
    %outputName = $DamageableBehaviorOutputNoSkipCallString @ %lastIndex;
    %adjustGMValueBehavior = PhysicsLauncherTools::getBehaviorFromConnection( %object,
                                                                            %outputBehaviorTemplateName, 
                                                                            %inputBehaviorTemplateName, 
                                                                            %outputName, 
                                                                            %inputName);
    if (%adjustGMValueBehavior !$= "")
        %adjustGMValueBehavior.adjustment = %pointsForLastState;
}

function WorldObjectBuilder::getPointsWhenDestroyed(%object)
{
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    %outputBehaviorTemplateName = "DamageableBehavior";
    %inputBehaviorTemplateName = "AdjustGMValueBehavior";
    %inputName = "adjustGMValue";
    
    // Loop through each damage state and count the points
    %totalPoints = 0;
    for (%i = 0; %i < %damageStateCount; %i++)
    {
        %outputName = $DamageableBehaviorOutputNoSkipCallString @ %i;
        
        // Get the correct adjustGMValueBehavior
        %adjustGMValueBehavior = PhysicsLauncherTools::getBehaviorFromConnection( %object,
                                                                                %outputBehaviorTemplateName,
                                                                                %inputBehaviorTemplateName, 
                                                                                %outputName, 
                                                                                %inputName);
    
        // Check if we found the behavior
        if (%adjustGMValueBehavior $= "")
            continue;
            
        %totalPoints += %adjustGMValueBehavior.adjustment;
    }
    
    // Add the Destroyed state
    %lastIndex = $WorldObjectBuilder::ToolNumberOfDamageStatesMax;
    %outputName = $DamageableBehaviorOutputNoSkipCallString @ %lastIndex;
    %adjustGMValueBehavior = PhysicsLauncherTools::getBehaviorFromConnection( %object,
                                                                            %outputBehaviorTemplateName,
                                                                            %inputBehaviorTemplateName, 
                                                                            %outputName, 
                                                                            %inputName);
    if (%adjustGMValueBehavior !$= "")
        %totalPoints += %adjustGMValueBehavior.adjustment;
    

    // Return the total points value
    return %totalPoints;
}

function WorldObjectBuilder::setFrictionLevel(%object, %toolFriction)
{
    %friction = %toolFriction / $WorldObjectBuilder::FrictionConversionRatio;
    
    %result = %object.callOnBehaviors("setObjectFriction", %friction);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::setFrictionLevel -- could not call setObjectFriction() on object " @ %object);
    }
}

function WorldObjectBuilder::getFrictionLevel(%object)
{
    %friction = %object.callOnBehaviors("getObjectFriction");    
    
    if (%friction $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::getFrictionLevel -- could not call getObjectFriction() on object " @ %object);
        return 0;
    }
    
    // Convert friction to friction level
    %toolFriction = mRound(%friction * $WorldObjectBuilder::FrictionConversionRatio);
    
    return %toolFriction;
}

function WorldObjectBuilder::setRestitutionLevel(%object, %toolRestitution)
{
    %restitution = %toolRestitution / $WorldObjectBuilder::RestitutionConversionRatio;
    
    %result = %object.callOnBehaviors("setObjectRestitution", %restitution);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::setRestitutionLevel -- could not call setObjectRestitution() on object " @ %object);
    }
}

function WorldObjectBuilder::getRestitutionLevel(%object)
{
    %restitution = %object.callOnBehaviors("getObjectRestitution");    
    
    if (%restitution $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::getRestitutionLevel -- could not call getObjectRestitution() on object " @ %object);
        return 0;
    }
    
    // Convert friction to friction level
    %toolRestitution = mRound(%restitution * $WorldObjectBuilder::RestitutionConversionRatio);
    
    return %toolRestitution;
}

function WorldObjectBuilder::setHitPoints(%object, %hitPoints)
{
    %result = %object.callOnBehaviors("setStartingHealth", %hitPoints);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::setHitPoints -- could not call setStartingHealth() on object " @ %object);
    }
}

function WorldObjectBuilder::getHitPoints(%object)
{
    %hitPoints = %object.callOnBehaviors("getStartingHealth");
    
    if (%hitPoints $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::getHitPoints -- could not call getStartingHealth() on object " @ %object);
        return 0;
    }
    
    return %hitPoints;
}

function WorldObjectBuilder::setIndestructableFlag(%object, %flagValue)
{
    %result = %object.callOnBehaviors("setIndestructableFlag", %flagValue);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::setIndestructableFlag -- could not call setIndestructableFlag() on object " @ %object);
    }
}

function WorldObjectBuilder::getIndestructableFlag(%object)
{
    %flagValue = %object.callOnBehaviors("getIndestructableFlag");
    
    if (%flagValue $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::getIndestructableFlag -- could not call getIndestructableFlag() on object " @ %object);
        return 0;
    }
    
    return %flagValue;
}

function WorldObjectBuilder::setDamageStateCount(%object, %number)
{
    %pointsWhenDestroyed = WorldObjectBuilder::getPointsWhenDestroyed(%object);    
    
    for (%i = 0; %i < $WorldObjectBuilder::ToolNumberOfDamageStatesMax; %i++)
    {
        if (%i < %number)
        {
            %fraction = (1.0 / (%number + 1)) * (%i + 1); 
            //echo ("setDamageStateCount, %fraction = " @ %fraction);           
            
            %result1 = %object.callOnBehaviors("setDamageStateIsActive", %i, 1);
            %result2 = %object.callOnBehaviors("setDamageStateForIndex", %i, %fraction);
        }
        else
        {
            %result1 = %object.callOnBehaviors("setDamageStateIsActive", %i, 0);
            %result2 = %object.callOnBehaviors("setDamageStateForIndex", %i, 0);
        }
            
        if (%result1 $= "ERR_CALL_NOT_HANDLED") 
        {
           warn("WorldObjectBuilder::setDamageStateCount -- could not call setDamageStateIsActive() on object " @ %object); 
        }
    }
    
    // Update Points on each
    WorldObjectBuilder::setPointsWhenDestroyed(%object, %pointsWhenDestroyed);
}

function WorldObjectBuilder::getDamageStateCount(%object)
{
    %count = 0;
    
    for (%i = 0; %i < $WorldObjectBuilder::ToolNumberOfDamageStatesMax; %i++)
    {
        %isActive = %object.callOnBehaviors("getDamageStateIsActive", %i);
        
        if (%isActive $= "ERR_CALL_NOT_HANDLED") 
        {
           warn("WorldObjectBuilder::getDamageStateCount -- could not call getDamageStateIsActive() on object " @ %object);
           %isActive = "0"; 
        }        
        
        if (%isActive !$= "0")
            %count++;
    }
    
    return %count;
}

function WorldObjectBuilder::setInstantKillFlag(%object, %flag)
{
    %groups = (%flag == true) ? $WorldObjectBuilder::ProjectileInstantKillDamageGroup : "0";  
    
    %result = %object.callOnBehaviors("setDamageModifierGroups", %groups);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
       warn("WorldObjectBuilder::setInstantKillFlag -- could not call setDamageModifierGroups() on object " @ %object); 
    }
}

function WorldObjectBuilder::getInstantKillFlag(%object)
{
    %groups = %object.callOnBehaviors("getDamageModifierGroups");
    
    if (%groups $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("WorldObjectBuilder::getInstantKillFlag -- could not call getDamageModifierGroups() on object " @ %object);
        return false;
    }

    if (t2dGetCommonElements(%groups, $WorldObjectBuilder::ProjectileInstantKillDamageGroup) !$= "")
        return true;
        
    return false;
}

function WorldObjectBuilder::setWinConditionFlag(%object, %flag)
{
    %result = %object.callOnBehaviors("setWinObjectiveIsActive", %flag);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
    {
        warn ("WorldObjectBuilder::setWinConditionFlag -- unable to call setWinObjectiveIsActive on object: " @ %object);
        return;
    }
}

function WorldObjectBuilder::getWinConditionFlag(%object)
{
    %isWinCondition = %object.callOnBehaviors("getWinObjectiveIsActive");
    
    if (%isWinCondition $= "ERR_CALL_NOT_HANDLED")
    {
        warn("WorldObjectBuilder::getWinConditionFlag -- unable to call getWinObjectiveIsActive on object: " @ %object);
        return false;   
    }
    
    return %isWinCondition;
}


//------------------------------------------------------------------------------
// Appearance
//------------------------------------------------------------------------------

function WorldObjectBuilder::getAnimationList(%object)
{
    %returnString = "";
    
    // Idle state
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, "IdleAnimationEffectBehavior");
    
    if (isObject(%animationEffectBehavior))
    {
        %animationLabel = "Idle";
        %animationAsset = %animationEffectBehavior.getAsset(); 
        %returnString = %animationLabel TAB %animationAsset;
    }
    
    // Damage states
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);
    for (%i = 0; %i < %damageStateCount; %i++)
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ %i;
        %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
        
        if (isObject(%animationEffectBehavior))
        {
            %animationLabel = "Damage State " @ %i + 1;
            %animationAsset = %animationEffectBehavior.getAsset(); 
            %returnString = %returnString TAB %animationLabel TAB %animationAsset;
        }
    }
    
    // Disappear state
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, "DamageAnimationEffectBehavior" @ 4);

    if (isObject(%animationEffectBehavior))
    {
        %animationLabel = "Disappear";
        %animationAsset = %animationEffectBehavior.getAsset(); 
        %returnString = %returnString TAB %animationLabel TAB %animationAsset;
    }
    
    return %returnString;
}

function WorldObjectBuilder::setImageForState(%object, %stateIndex, %imageAsset)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::setImageForState -- Invalid state index: " @ %stateIndex);
        return;
    }

    // Idle
    if (%stateIndex == 0)
    {
        %behaviorInstanceName = "IdleAnimationEffectBehavior";
    }
    // Disppear
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ %stateIndex - 1;
    }
    
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    if (!isObject(%animationEffectBehavior))
    {
        warn ("WorldObjectBuilder::setImageForState -- Unable to get behavior instance: " @ %behaviorInstanceName);
        return;
    }
            
    %animationEffectBehavior.setAsset(%imageAsset);
    
    // Resize the sprite if setting the idle state
    if (%stateIndex == 0)
    {
        %oldSize = %object.getSize();
        %object.setSizeFromAsset(%imageAsset, $PhysicsLauncherTools::MetersPerPixel);
        %newSize = %object.getSize();
        
        %scaleX = %newSize.x / %oldSize.x;
        %scaleY = %newSize.y / %oldSize.y;
        PhysicsLauncherTools::scaleCollisionShapes(%object, %scaleX, %scaleY);
    }
}

function WorldObjectBuilder::getImageForState(%object, %stateIndex)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::getImageForState -- Invalid state index: " @ %stateIndex);
        return "";
    }
    
    // Idle
    if (%stateIndex == 0)
    {
         %behaviorInstanceName = "IdleAnimationEffectBehavior";
    }
    // Disppear
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ %stateIndex - 1;
    }
    
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    if (isObject(%animationEffectBehavior))
            return %animationEffectBehavior.getAsset();
    
    return "";
}

function WorldObjectBuilder::setImageFrameForState(%object, %stateIndex, %imageFrame)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::setImageFrameForState -- Invalid state index: " @ %stateIndex);
        return;
    }
    
    // Idle
    if (%stateIndex == 0)
    {
         %behaviorInstanceName = "IdleAnimationEffectBehavior";
    }
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ %stateIndex - 1;   
    }
    
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    if (isObject(%animationEffectBehavior))
        %animationEffectBehavior.setFrame(%imageFrame);
}

function WorldObjectBuilder::getImageFrameForState(%object, %stateIndex)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::getImageFrameForState -- Invalid state index: " @ %stateIndex);
        return "";
    }
    
    // Idle
    if (%stateIndex == 0)
    {
         %behaviorInstanceName = "IdleAnimationEffectBehavior";
    }
    // Disappear
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ %stateIndex - 1; 
    }
    
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    if (isObject(%animationEffectBehavior))
        return %animationEffectBehavior.getFrame();
    
    return "";
}

function WorldObjectBuilder::getImageFrameCountForState(%object, %stateIndex)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::getImageFrameForState -- Invalid state index: " @ %stateIndex);
        return "";
    }
    
    // Idle
    if (%stateIndex == 0)
    {
         %behaviorInstanceName = "IdleAnimationEffectBehavior";
    }
    // Disappear
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageAnimationEffectBehavior" @ %stateIndex - 1;
        
    }
    
    %animationEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    if (isObject(%animationEffectBehavior))
        return %animationEffectBehavior.getFrameCount();
    
    return "";
}

//------------------------------------------------------------------------------
// Audio
//------------------------------------------------------------------------------

function WorldObjectBuilder::setSoundForState(%object, %stateIndex, %soundAsset)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::setSoundForState -- Invalid state index: " @ %stateIndex);
        return;
    }
    
    // Idle
    if (%stateIndex == 0)
    {
        %behaviorInstanceName = "IdleSoundEffectBehavior";
    }
    // Disappear
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageSoundEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageSoundEffectBehavior" @ %stateIndex - 1;
    }
    
    %soundEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    
    if (isObject(%soundEffectBehavior))
        %soundEffectBehavior.sound = %soundAsset;
}

function WorldObjectBuilder::getSoundForState(%object, %stateIndex)
{
    // Get the damage state count
    %damageStateCount = WorldObjectBuilder::getDamageStateCount(%object);    
    
    // Check if the state index is valid
    if (%stateIndex > %damageStateCount + 2 && %stateIndex != $WorldObjectBuilder::DisappearStateIndex)
    {
        warn("WorldObjectBuilder::getSoundForState -- Invalid state index: " @ %stateIndex);
        return "";
    }
    
    // Idle
    if (%stateIndex == 0)
    {
        %behaviorInstanceName = "IdleSoundEffectBehavior";
    }
    // Disappear
    else if (%stateIndex == $WorldObjectBuilder::DisappearStateIndex)
    {
        %behaviorInstanceName = "DamageSoundEffectBehavior" @ 4;
    }
    // Damage States
    else
    {
        %behaviorInstanceName = "DamageSoundEffectBehavior" @ %stateIndex - 1;
    }
    
    %soundEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%object, %behaviorInstanceName);
    
    if (isObject(%soundEffectBehavior))
        return %soundEffectBehavior.sound;
        
    return "";
}

function WorldObjectBuilder::openCollisionEditor(%object, %invokingGui)
{
    %proxyObject = WorldObjectBuilder::constructCollisionShapeProxy(%object);
    
    if(%invokingGui != CollisionSidebar.getId())
        CollisionSidebar.load(2, true);
    CollisionEditorGui.open(%proxyObject, %invokingGui);
    
    // Add state information
    CollisionEditorGui.clearStateEntries();
    
    %asset = WorldObjectBuilder::getImageForState(%object, 0);
    %frame = WorldObjectBuilder::getImageFrameForState(%object, 0);
    CollisionEditorGui.addStateEntry("Idle", %asset, %frame);
    
    for (%i = 0; %i < WorldObjectBuilder::getDamageStateCount(%object); %i++)
    {
        %asset = WorldObjectBuilder::getImageForState(%object, %i + 1);
        %frame = WorldObjectBuilder::getImageFrameForState(%object, %i + 1);
        CollisionEditorGui.addStateEntry("Damage State " @ %i + 1, %asset, %frame);
    }
    
    %asset = WorldObjectBuilder::getImageForState(%object, $WorldObjectBuilder::DisappearStateIndex);
    %frame = WorldObjectBuilder::getImageFrameForState(%object, $WorldObjectBuilder::DisappearStateIndex);
    CollisionEditorGui.addStateEntry("Disappear", %asset, %frame);
}

// Constructs a proxy for a world object that can be passed to the collision editor.
function WorldObjectBuilder::constructCollisionShapeProxy(%object)
{
    %proxyObject = new Sprite();
    %proxyObject.setSize(%object.getSize());
    %proxyObject.setPosition(%object.getPosition());
    %proxyObject.objectName = %object.getInternalName();

    if (%object.Image !$= "")
    {
        %static = true;
        %preview = %object.Image;
    }
    else
    {
        %static = false;
        %preview = %object.Animation;
    }

    %proxyObject.Animation=(%static == true ? "" : %preview);
    %proxyObject.Image=(%static == true ? %preview : "");

    PhysicsLauncherTools::copyCollisionShapes(%object, %proxyObject);
    
    return %proxyObject;
}

// Sets collision shapes on a world object from a proxy
function WorldObjectBuilder::setCollisionShapesFromProxy(%object, %proxyObject)
{
    PhysicsLauncherTools::copyCollisionShapes(%proxyObject, %object);
}
