//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$SlingshotLauncherBuilder::launcherSceneGroupName = "LauncherSceneObjectGroup";
$SlingshotLauncherBuilder::LauncherObjectName = "LauncherObjectPrefab";
$SlingshotLauncherBuilder::CollisionObjectName = "CollisionObjectPrefab";
$SlingshotLauncherBuilder::ForkForegroundObjectName = "ForkForgroundObjectPrefab";
$SlingshotLauncherBuilder::ForkBackgroundObjectName = "ForkBackgroundObjectPrefab";
$SlingshotLauncherBuilder::SeatObjectName = "SeatObjectPrefab";
$SlingshotLauncherBuilder::Band0ObjectName = "Band0ObjectPrefab";
$SlingshotLauncherBuilder::Band1ObjectName = "Band1ObjectPrefab";

$SlingshotLauncherBuilder::rubberbandThicknessScalingRatio = 0.8;

// Default attachment points
$SlingshotLauncherBuilder::Band0AttachmentPointStartDefault = "-2.0 2.0";
$SlingshotLauncherBuilder::Band0AttachmentPointEndDefault = "0 0";
$SlingshotLauncherBuilder::Band1AttachmentPointStartDefault = "1.4 2.0";
$SlingshotLauncherBuilder::Band1AttachmentPointEndDefault = "0 0";

//
$SlingshotLauncherBuilder::LauncherObjectLayer = 4;
$SlingshotLauncherBuilder::CollisionObjectLayer = 16;
$SlingshotLauncherBuilder::ForkForegroundObjectLayer = 12;
$SlingshotLauncherBuilder::ForkBackgroundObjectLayer = 18;
$SlingshotLauncherBuilder::SeatObjectLayer = 13;
$SlingshotLauncherBuilder::Band0ObjectLayer = 14;
$SlingshotLauncherBuilder::Band1ObjectLayer = 17;

$SlingshotLauncherBuilder::ToolPowerDefault = 10;
$SlingshotLauncherBuilder::ToolPowerMin = 1;
$SlingshotLauncherBuilder::ToolPowerMax = 100;
$SlingshotLauncherBuilder::PowerMin = 1;
$SlingshotLauncherBuilder::PowerMax = 100;
$SlingshotLauncherBuilder::PowerConversionRatio = ($SlingshotLauncherBuilder::ToolPowerMax - $SlingshotLauncherBuilder::ToolPowerMin)
                                                    /($SlingshotLauncherBuilder::PowerMax - $SlingshotLauncherBuilder::PowerMin);

$SlingshotLauncherBuilder::ToolDistanceDefault = 5;
$SlingshotLauncherBuilder::ToolDistanceMin = 1;
$SlingshotLauncherBuilder::ToolDistanceMax = 20;
$SlingshotLauncherBuilder::DistanceMin = 0.5;
$SlingshotLauncherBuilder::DistanceMax = 10;
$SlingshotLauncherBuilder::DistanceConversionRatio = ($SlingshotLauncherBuilder::ToolDistanceMax - $SlingshotLauncherBuilder::ToolDistanceMin)
                                                    /($SlingshotLauncherBuilder::DistanceMax - $SlingshotLauncherBuilder::DistanceMin);
                                            
$SlingshotLauncherBuilder::LauncherObjectInternalName = "BuilderObject";
$SlingshotLauncherBuilder::ForkForegroundObjectInternalName = "SlingshotForegroundObject";
$SlingshotLauncherBuilder::ForkBackgroundObjectInternalName = "SlingshotBackgroundObject";
$SlingshotLauncherBuilder::CollisionObjectInternalName = "CollisionObject";
$SlingshotLauncherBuilder::Band0InternalName = "BandObject0";
$SlingshotLauncherBuilder::Band1InternalName = "BandObject1";
$SlingshotLauncherBuilder::SeatInternalName = "SeatObject";

// Factory function for WorldObjects
function SlingshotLauncherBuilder::getNewSlingshotLauncher()
{
    return SlingshotLauncherBuilder::createSlingshotLauncherTemplate();
}

function SlingshotLauncherBuilder::createSlingshotLauncherTemplate()
{
    //--------------------------------------------------------------------------
    // Create Launcher SceneObjectGroup
    //--------------------------------------------------------------------------

    // Generate a unique name for the group
    %launcherPrefabCount = 0;
    %launcherSceneGroupName = $SlingshotLauncherBuilder::launcherSceneGroupName @ %launcherPrefabCount;
    while (isObject(%launcherSceneGroupName))
    {
        %launcherPrefabCount++;
        %launcherSceneGroupName = $SlingshotLauncherBuilder::launcherSceneGroupName @ %launcherPrefabCount;
    }
    
    %launcherSimSet = new SceneObjectGroup(%launcherSceneGroupName); 
    
    //--------------------------------------------------------------------------
    // Create Builder Object
    //--------------------------------------------------------------------------
    %builderObject = new SceneObject($SlingshotLauncherBuilder::LauncherObjectName @ %launcherPrefabCount);
    %builderObject.setInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
    %launcherSimSet.add(%builderObject);
    //%builderObject.sceneObjectGroupName = %launcherSceneGroupName;
    %builderObject.setPosition(0, 0);  
    //%builderObject.setSize(4.0, 4.0);
    %builderObject.setBodyType("static");
    %builderObject.setCollisionLayers(5);
    %builderObject.setCollisionGroups(1);
    %builderObject.setSceneLayer($SlingshotLauncherBuilder::LauncherObjectLayer);
    %builderObject.setUseInputEvents(true);
    %builderObject.setVisible(true);
    
    %slingshotLauncherBuilderBehavior = SlingshotLauncherBuilderBehavior.createInstance(); 
    %builderObject.addBehavior(%slingshotLauncherBuilderBehavior);
    

    %maxDistance = $SlingshotLauncherBuilder::ToolDistanceDefault;
    %maxSpeed = $SlingshotLauncherBuilder::ToolPowerDefault;   
    
    //--------------------------------------------------------------------------
    // Setup collision object
    //--------------------------------------------------------------------------
    %collisionObject = new SceneObject($SlingshotLauncherBuilder::CollisionObjectName @ %launcherPrefabCount);
    %collisionObject.setInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    %launcherSimSet.add(%collisionObject);
    //%collisionObject.sceneObjectGroupName = %launcherSceneGroupName;
    %collisionObject.setPosition(0, 0);  
    //%collisionObject.setSize(1.0, 1.0);
    %collisionObject.setBodyType("static");
    %collisionObject.setSceneLayer($SlingshotLauncherBuilder::CollisionObjectLayer);
    %collisionObject.createPolygonCollisionShape("0 -0.2 -0.4 -1.5 0.4 -1.5");

    
    // Add pullbackLauncherBehavior
    %pullbackBehavior = PullbackLauncherBehavior.createInstance();
    %builderObject.addBehavior(%pullbackBehavior);
    
    SlingshotLauncherBuilder::setPower(%launcherSimSet, %maxSpeed);
    SlingshotLauncherBuilder::setStretchDistance(%launcherSimSet, %maxDistance);
    
    // Add InputTargetBehavior
    %inputTargetBehavior = InputTargetBehavior.createInstance();   
    %builderObject.addBehavior(%inputTargetBehavior); 
    
    // Add MoveSoundBehavior
    %stretchSoundBehavior = SoundEffectBehavior.createInstance();
    %stretchSoundBehavior.instanceName = "StretchSoundBehavior";
    %stretchSoundBehavior.sound = PL_StretchSound;
    %builderObject.addBehavior(%stretchSoundBehavior);
    
    // Connect behavior outputs from the inputTargetBehavior
    %builderObject.Connect(%inputTargetBehavior, %pullbackBehavior, inputDown, setTargetPosition);
    %builderObject.Connect(%inputTargetBehavior, %pullbackBehavior, inputUp, launch);
    %builderObject.Connect(%inputTargetBehavior, %pullbackBehavior, inputDrag, setTargetPosition);
    
    %builderObject.Connect(%pullbackBehavior, %slingshotLauncherBuilderBehavior, onLoad, onLoad);
    
    %builderObject.Connect(%inputTargetBehavior, %slingshotLauncherBuilderBehavior, inputDown, onMove);
    %builderObject.Connect(%inputTargetBehavior, %slingshotLauncherBuilderBehavior, inputDrag, onMove);
    %builderObject.Connect(%inputTargetBehavior, %slingshotLauncherBuilderBehavior, inputUp, onLaunch);
    
    %builderObject.Connect(%pullbackBehavior, %stretchSoundBehavior, stretchOutput, PlaySoundNoInterrupt);
    
    //--------------------------------------------------------------------------
    // Add slingshot graphics   
    //--------------------------------------------------------------------------
    %slingshotForeground = new Sprite($SlingshotLauncherBuilder::ForkForegroundObjectName @ %launcherPrefabCount);
    %slingshotForeground.setInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
    %launcherSimSet.add(%slingshotForeground);
    %slingshotForeground.setBodyType("static");
    %slingshotForeground.setSceneLayer($SlingshotLauncherBuilder::ForkForegroundObjectLayer);
    
    %slingshotBackground = new Sprite($SlingshotLauncherBuilder::ForkBackgroundObjectName @ %launcherPrefabCount);
    %slingshotBackground.setInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
    %launcherSimSet.add(%slingshotBackground);
    %slingshotBackground.setBodyType("static");
    %slingshotBackground.setSceneLayer($SlingshotLauncherBuilder::ForkBackgroundObjectLayer);
    
    SlingshotLauncherBuilder::setForkForegroundAsset(%launcherSimSet, "{PhysicsLauncherAssets}:SlingshotForegroundImageMap");
    SlingshotLauncherBuilder::setForkBackgroundAsset(%launcherSimSet, "{PhysicsLauncherAssets}:SlingshotBackgroundImageMap");
    
    // Update size of collision object to match graphics
    SlingshotLauncherBuilder::updateLauncherCollisionObjectSize(%launcherSceneGroupName);

    //--------------------------------------------------------------------------
    // Setup seat object
    //--------------------------------------------------------------------------
    %seat = new Sprite($SlingshotLauncherBuilder::SeatObjectName @ %launcherPrefabCount);
    %seat.setInternalName($SlingshotLauncherBuilder::SeatInternalName);
    %launcherSimSet.add(%seat);
    %seat.setVisible(false);
    %seat.setBodyType("static");
    %seat.setSceneLayer($SlingshotLauncherBuilder::SeatObjectLayer);
    SlingshotLauncherBuilder::setSeatAsset(%launcherSimSet, "{PhysicsLauncherAssets}:SlingshotSeatImageMap");
    
    //--------------------------------------------------------------------------
    // Set up rubberbands
    //--------------------------------------------------------------------------
    %band0 = new Sprite($SlingshotLauncherBuilder::Band0ObjectName @ %launcherPrefabCount);
    %band0.setInternalName($SlingshotLauncherBuilder::Band0InternalName);
    %launcherSimSet.add(%band0);
    %band0.setBodyType("static"); 
    %band0.setVisible(false);
    %band0.setSceneLayer($SlingshotLauncherBuilder::Band0ObjectLayer);
    SlingshotLauncherBuilder::setBandAsset(%launcherSimSet, 0, "{PhysicsLauncherAssets}:RubberbandImageMap");
    
    %scaleBetweenPointsBehavior0 = ScaleBetweenPointsBehavior.createInstance();
    %scaleBetweenPointsBehavior0.setAttachmentPoints($SlingshotLauncherBuilder::Band0AttachmentPointStartDefault, $SlingshotLauncherBuilder::Band0AttachmentPointEndDefault);
    %band0.addBehavior(%scaleBetweenPointsBehavior0);
    
    %band1 = new Sprite($SlingshotLauncherBuilder::Band1ObjectName @ %launcherPrefabCount);
    %band1.setInternalName($SlingshotLauncherBuilder::Band1InternalName);
    %launcherSimSet.add(%band1);
    %band1.setBodyType("static"); 
    %band1.setVisible(false);
    %band1.setSceneLayer($SlingshotLauncherBuilder::Band1ObjectLayer);
    SlingshotLauncherBuilder::setBandAsset(%launcherSimSet, 1, "{PhysicsLauncherAssets}:RubberbandImageMap"); 
    
    %scaleBetweenPointsBehavior1 = ScaleBetweenPointsBehavior.createInstance();
    %scaleBetweenPointsBehavior1.setAttachmentPoints($SlingshotLauncherBuilder::Band1AttachmentPointStartDefault, $SlingshotLauncherBuilder::Band1AttachmentPointEndDefault);
    %band1.addBehavior(%scaleBetweenPointsBehavior1);
    
    //--------------------------------------------------------------------------
    // Update Touch Target Shape
    //--------------------------------------------------------------------------
    SlingshotLauncherBuilder::updateTouchTargetShape(%launcherSimSet);
    
    return %launcherSimSet;
}

function SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
    
    if (!isObject(%object))
    {
        warn("SlingshotLauncherBuilder::getBuilderObject -- Unable to find BuilderObject");
        return "";
    }
    
    return %object;
}

function SlingshotLauncherBuilder::findLauncherInLevel(%launcherGroup, %level)
{
    return TamlVisitorContainsValue(%level, "internalName", %launcherGroup.getInternalName());
}

function SlingshotLauncherBuilder::findLauncherInAllLevels(%launcherGroup)
{
    %path = expandPath("^{UserGame}/data/levels"); 
    %fileSpec = "/*.scene.taml";   
    %pattern = %path @ %fileSpec;

    %file = findFirstFile(%pattern);

    %dependencies = "";

    while(%file !$= "")
    {
        if ( SlingshotLauncherBuilder::findLauncherInLevel(%launcherGroup, %file) )
        {
            %levelName = fileBase(%file);
            %name = strreplace(%levelName, ".scene", "");
            %temp = %name @ " " @ %dependencies;
            %dependencies = %temp;
        }
        %file = findNextFile(%pattern);
    }
    return %dependencies;
}

function SlingshotLauncherBuilder::setName(%launcherSceneObjectGroup, %name)
{
    %launcherSceneObjectGroup.setInternalName(%name);
}

function SlingshotLauncherBuilder::getName(%launcherSceneObjectGroup)
{
    return %launcherSceneObjectGroup.getInternalName();
}

function SlingshotLauncherBuilder::setPower(%launcherSceneObjectGroup, %toolPower)
{
    %builderObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup);
    
    %power = %toolPower / $SlingshotLauncherBuilder::PowerConversionRatio;
    
    %result = %builderObject.callOnBehaviors("setLauncherMaxSpeed", %power);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("SlingshotLauncherBuilder::setPower -- could not call setLauncherMaxSpeed() on object " @ %builderObject);
    } 
}

function SlingshotLauncherBuilder::getPower(%launcherSceneObjectGroup)
{
    %builderObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup);
    
    %power = %builderObject.callOnBehaviors("getLauncherMaxSpeed"); 
    
    if (%power $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("SlingshotLauncherBuilder::getPower -- could not call getLauncherMaxSpeed() on object " @ %builderObject);
        return 0;
    }  
    
    %toolPower = %power * $SlingshotLauncherBuilder::PowerConversionRatio;
    
    return %toolPower;
}

function SlingshotLauncherBuilder::setStretchDistance(%launcherSceneObjectGroup, %toolDistance)
{
    %builderObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup);
    
    %distance = %toolDistance / $SlingshotLauncherBuilder::DistanceConversionRatio;
    
    %result = %builderObject.callOnBehaviors("setLauncherMaxDistance", %distance);
    
    if (%result $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("SlingshotLauncherBuilder::setStretchDistance -- could not call setLauncherMaxDistance() on object " @ %builderObject);
    }  
}

function SlingshotLauncherBuilder::getStretchDistance(%launcherSceneObjectGroup)
{
    %builderObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup);
    
    %distance = %builderObject.callOnBehaviors("getLauncherMaxDistance"); 
    
    if (%distance $= "ERR_CALL_NOT_HANDLED") 
    {
        warn("SlingshotLauncherBuilder::getStretchDistance -- could not call getLauncherMaxDistance() on object " @ %builderObject);
        return 0;
    }  
    
    %toolDistance = %distance * $SlingshotLauncherBuilder::DistanceConversionRatio;
    
    return %toolDistance;
}

function SlingshotLauncherBuilder::updateTouchTargetShape(%launcherSceneObjectGroup)
{
    %launcherObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup);
    
    %launcherObject.clearCollisionShapes();
    
    %radius = %launcherObject.callOnBehaviors("getLauncherMaxDistance");
    
    if (%radius $= "ERR_CALL_NOT_HANDLED") 
    {
        warn ("SlingshotLauncherBuilder::updateTouchTargetShape -- unable to call getLauncherMaxDistance on behaviors");
        return;
    }
    
    // Calculate local center
    %point0 = SlingshotLauncherBuilder::getBandAttachmentStartPoint(%launcherSceneObjectGroup, 0);
    %point1 = SlingshotLauncherBuilder::getBandAttachmentStartPoint(%launcherSceneObjectGroup, 1);
    %localCenter = Vector2Scale(Vector2Add(%point0, %point1), 0.5);
    
    // Scale the center point by the size of the launcher
    %launcherHalfWidth = %launcherObject.getWidth() / 2;
    %launcherHalfHeight = %launcherObject.getHeight() / 2;
    %scaledCenter = %localCenter.x * %launcherHalfWidth SPC %localCenter.y * %launcherHalfHeight;
    
    %touchTargetShape = %launcherObject.createCircleCollisionShape(%radius, %scaledCenter);
    %launcherObject.setCollisionShapeIsSensor(%touchTargetShape, true);
}

function SlingshotLauncherBuilder::setPullbackEffect(%launcherSceneObjectGroup, %effect)
{
    %thicknessScalingRatio = 1.0;
    
    switch(%effect)
    {
        case 0: // stretch only
            %thicknessScalingRatio = 1.0;
            
        case 1: // stretch and shrink
            %thicknessScalingRatio = $SlingshotLauncherBuilder::rubberbandThicknessScalingRatio;
    }
    
    // Update the scaling on each rubberband
    %rubberbandObject0 = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::Band0InternalName);
    %rubberbandObject1 = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::Band1InternalName);
    
    %result = %rubberbandObject0.callOnBehaviors("setThicknessScalingRatio", %thicknessScalingRatio);
         
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn("SlingshotLauncherBuilder::setPullbackEffect -- could not call setThicknessScalingRatio on object " @ %rubberbandObject0); 
        
    %result = %rubberbandObject1.callOnBehaviors("setThicknessScalingRatio", %thicknessScalingRatio);
         
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn("SlingshotLauncherBuilder::setPullbackEffect -- could not call setThicknessScalingRatio on object " @ %rubberbandObject1); 
}

function SlingshotLauncherBuilder::getPullbackEffect(%launcherSceneObjectGroup)
{
    %rubberbandObject0 = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::Band0InternalName);    

    %thicknessScalingRatio = %rubberbandObject0.callOnBehaviors("getThicknessScalingRatio");  
    
    if (%thicknessScalingRatio == $SlingshotLauncherBuilder::rubberbandThicknessScalingRatio)
        return 1;
        
    return 0;
}

function SlingshotLauncherBuilder::setPullbackSound(%launcherSceneObjectGroup, %sound)
{
    %builderObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup); 
    
    %soundEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%builderObject, "StretchSoundBehavior");
    
    if (%soundEffectBehavior $= "")
    {
        warn("SlingshotLauncherBuilder::setPullbackSound -- Unable to find StretchSoundBehavior on object " @ %object);
        return;   
    }
    
    %soundEffectBehavior.sound = %sound;
}

function SlingshotLauncherBuilder::getPullbackSound(%launcherSceneObjectGroup)
{
    %builderObject = SlingshotLauncherBuilder::getBuilderObject(%launcherSceneObjectGroup);    
    
    %soundEffectBehavior = PhysicsLauncherTools::getBehaviorFromInstanceName(%builderObject, "StretchSoundBehavior");
    
    if (%soundEffectBehavior $= "")
    {
        warn("SlingshotLauncherBuilder::getPullbackSound -- Unable to find StretchSoundBehavior on object " @ %object);
        return;   
    }
    
    return %soundEffectBehavior.sound;
}

function SlingshotLauncherBuilder::setForkForegroundAsset(%launcherSceneObjectGroup, %asset, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);    
    
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setForkForegroundAsset -- could not find " @ $SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
        return;   
    }
    
    %object.setAsset(%asset);
    %object.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
    if ( %frame !$= "" && %object.isStaticMode() )
        %object.setFrame(%frame);

    // Update size of collision object to match graphics
    SlingshotLauncherBuilder::updateLauncherCollisionObjectSize(%launcherSceneObjectGroup);
}

function SlingshotLauncherBuilder::getForkForegroundAsset(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getForkForegroundAsset -- could not find " @ $SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
        return "";   
    }
    
    return %object.getAsset();
}

function SlingshotLauncherBuilder::getForkForegroundImageFrame(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getForkForegroundImageFrame -- could not find " @ $SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
        return "";   
    }
    if ( %object.isStaticMode() )
        %frame = %object.getFrame();
    return (%frame !$= "" ? %frame : 0);
}

function SlingshotLauncherBuilder::setForkForegroundImageFrame(%launcherSceneObjectGroup, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);    

    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setForkForegroundImageFrame -- could not find " @ $SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
        return;   
    }

    if ( %object.isStaticMode() )
        %object.setFrame(%frame);
}

function SlingshotLauncherBuilder::setForkBackgroundAsset(%launcherSceneObjectGroup, %asset, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);    
    
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setForkBackgroundAsset -- could not find " @ $SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
        return;   
    }
    
    %object.setAsset(%asset);
    %object.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
    if ( %frame !$= "" && %object.isStaticMode() )
        %object.setFrame(%frame);

    // Update size of collision object to match graphics
    SlingshotLauncherBuilder::updateLauncherCollisionObjectSize(%launcherSceneObjectGroup);
}

function SlingshotLauncherBuilder::setForkBackgroundImageFrame(%launcherSceneObjectGroup, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);    

    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setForkBackgroundAsset -- could not find " @ $SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
        return;   
    }

    if ( %object.isStaticMode() )
        %object.setFrame(%frame);
}

function SlingshotLauncherBuilder::getForkBackgroundAsset(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getForkBackgroundAsset -- could not find " @ $SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
        return "";   
    }
    
    return %object.getAsset();
}

function SlingshotLauncherBuilder::getForkBackgroundImageFrame(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getForkBackgroundImageFrame -- could not find " @ $SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
        return "";   
    }
    if ( %object.isStaticMode() )
        %frame = %object.getFrame();
    return (%frame !$= "" ? %frame : 0);
}

function SlingshotLauncherBuilder::setBandAsset(%launcherSceneObjectGroup, %bandIndex, %asset, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
    
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setBandAsset -- could not find BandObject" @ %bandIndex);
        return;   
    }    
    
    %object.setAsset(%asset);
    %object.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
    if ( %object.isStaticMode() )
        %object.setFrame(%frame);
}

function SlingshotLauncherBuilder::setBandImageFrame(%launcherSceneObjectGroup, %bandIndex, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
    
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setBandAsset -- could not find BandObject" @ %bandIndex);
        return;   
    }    
    
    if ( %object.isStaticMode() )
        %object.setFrame(%frame);
}

function SlingshotLauncherBuilder::getBandAsset(%launcherSceneObjectGroup, %bandIndex)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getForkBackgroundAsset -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    return %object.getAsset();
}

function SlingshotLauncherBuilder::getBandImageFrame(%launcherSceneObjectGroup, %bandIndex)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getForkBackgroundAsset -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    if ( %object.isStaticMode() )
        %frame = %object.getFrame();
    return (%frame !$= "" ? %frame : 0);
}

function SlingshotLauncherBuilder::setBandAttachmentPoints(%launcherSceneObjectGroup, %bandIndex, %start, %end)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setBandAttachmentPoints -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    %result = %object.callOnBehaviors("setAttachmentStartPoint", %start);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn ("SlingshotLauncherBuilder::setBandAttachmentPoints -- failed to call setAttachmentStartPoint");
    
    %result = %object.callOnBehaviors("setAttachmentEndPoint", %end);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn ("SlingshotLauncherBuilder::setBandAttachmentPoints -- failed to call setAttachmentEndPoint");
}

function SlingshotLauncherBuilder::getBandAttachmentPoints(%launcherSceneObjectGroup, %bandIndex)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getBandAttachmentPoints -- could not find BandObject" @ %bandIndex);
        return "";   
    }

    %start = SlingshotLauncherBuilder::getBandAttachmentStartPoint(%launcherSceneObjectGroup, %bandIndex);
    %end = SlingshotLauncherBuilder::getBandAttachmentEndPoint(%launcherSceneObjectGroup, %bandIndex);
    
    return %start SPC %end;
}

function SlingshotLauncherBuilder::setBandAttachmentStartPoint(%launcherSceneObjectGroup, %bandIndex, %start)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setBandAttachmentStartPoint -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    %result = %object.callOnBehaviors("setAttachmentStartPoint", %start);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn ("SlingshotLauncherBuilder::setBandAttachmentStartPoint -- failed to call setAttachmentStartPoint on behavior");
    
}

function SlingshotLauncherBuilder::getBandAttachmentStartPoint(%launcherSceneObjectGroup, %bandIndex)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getBandAttachmentStartPoint -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    %start = %object.callOnBehaviors("getAttachmentStartPoint");
    
    if (%start $= "ERR_CALL_NOT_HANDLED")
        warn ("SlingshotLauncherBuilder::getBandAttachmentStartPoint -- failed to call getAttachmentStartPoint on behavior");
        
    return %start;
}

function SlingshotLauncherBuilder::setBandAttachmentEndPoint(%launcherSceneObjectGroup, %bandIndex, %end)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setBandAttachmentEndPoint -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    %result = %object.callOnBehaviors("setAttachmentEndPoint", %end);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
        warn ("SlingshotLauncherBuilder::setBandAttachmentEndPoint -- failed to call setAttachmentEndPoint on behavior");
}

function SlingshotLauncherBuilder::getBandAttachmentEndPoint(%launcherSceneObjectGroup, %bandIndex)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName("BandObject" @ %bandIndex);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getBandAttachmentEndPoint -- could not find BandObject" @ %bandIndex);
        return "";   
    }
    
    %end = %object.callOnBehaviors("getAttachmentEndPoint");
    
    if (%end $= "ERR_CALL_NOT_HANDLED")
        warn ("SlingshotLauncherBuilder::getBandAttachmentEndPoint -- failed to call getAttachmentEndPoint on behavior");
        
    return %end;
}

function SlingshotLauncherBuilder::setSeatAsset(%launcherSceneObjectGroup, %asset, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::SeatInternalName);    
    
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setSeatAsset -- could not find SeatObject");
        return;   
    }
    
    %object.setAsset(%asset);
    %object.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
    if ( %object.isStaticMode() )
        %object.setFrame(%frame);
}

function SlingshotLauncherBuilder::setSeatImageFrame(%launcherSceneObjectGroup, %frame)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::SeatInternalName);    
    
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::setSeatAsset -- could not find SeatObject");
        return;   
    }
    
    if ( %object.isStaticMode() )
        %object.setFrame(%frame);
}

function SlingshotLauncherBuilder::getSeatAsset(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::SeatInternalName);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getSeatAsset -- could not find SeatObject");
        return "";   
    }
    
    return %object.getAsset();  
}

function SlingshotLauncherBuilder::getSeatImageFrame(%launcherSceneObjectGroup)
{
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::SeatInternalName);
     
    if (!isObject(%object)) 
    {
        warn("SlingshotLauncherBuilder::getSeatAsset -- could not find SeatObject");
        return "";   
    }
    
    if ( %object.isStaticMode() )
        %frame = %object.getFrame();
    return (%frame !$= "" ? %frame : 0);
}

// Calculates and sets the size of the collision object based so that it encloses
// the fork art sprites
function SlingshotLauncherBuilder::updateLauncherCollisionObjectSize(%launcherSceneObjectGroup)
{
    %foreground = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
    %background = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
    %collisionObject = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    
    %width = mGetMax(%foreground.getWidth(), %background.getWidth());
    %height = mGetMax(%foreground.getHeight(), %background.getHeight());
    
    %collisionObject.setSize(%width, %height);
    
    // TODO: update the collision shapes
}

function SlingshotLauncherBuilder::openCollisionEditor(%launcherSceneObjectGroup, %invokingGui)
{
    if( CollisionEditorGui.open )
        CollisionEditorGui.close();
    %proxyObject = SlingshotLauncherBuilder::constructCollisionShapeProxy(%launcherSceneObjectGroup);
    
    if(%invokingGui != CollisionSidebar.getId())
        CollisionSidebar.load(3, true);
    CollisionEditorGui.open(%proxyObject, %invokingGui);
    
    CollisionEditorGui.clearStateEntries();
    
    // Add decorators
    if (!isObject(SlingshotLauncherBuilderForkForegroundDecorator))
        %forkForegroundSprite = new Sprite(SlingshotLauncherBuilderForkForegroundDecorator);
        
    %asset = SlingshotLauncherBuilder::getForkForegroundAsset(%launcherSceneObjectGroup);
    SlingshotLauncherBuilderForkForegroundDecorator.setAsset(%asset);
    SlingshotLauncherBuilderForkForegroundDecorator.setFrame(0);
    SlingshotLauncherBuilderForkForegroundDecorator.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
        
    if (!isObject(SlingshotLauncherBuilderForkBackgroundDecorator))
        %forkBackgroundSprite = new Sprite(SlingshotLauncherBuilderForkBackgroundDecorator);
        
    %asset = SlingshotLauncherBuilder::getForkBackgroundAsset(%launcherSceneObjectGroup);
    SlingshotLauncherBuilderForkBackgroundDecorator.setAsset(%asset);
    SlingshotLauncherBuilderForkBackgroundDecorator.setFrame(0);  
    SlingshotLauncherBuilderForkBackgroundDecorator.setSizeFromAsset(%asset, $PhysicsLauncherTools::MetersPerPixel);
        
    CollisionEditorGui.addDecoratorObject(SlingshotLauncherBuilderForkForegroundDecorator);
    CollisionEditorGui.addDecoratorObject(SlingshotLauncherBuilderForkBackgroundDecorator);
}

// Constructs a proxy for a launcher group that can be passed to the collision editor.
function SlingshotLauncherBuilder::constructCollisionShapeProxy(%launcherSceneObjectGroup)
{
    // Get the collision object
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    echo( "Getting collision object: ", %launcherSceneObjectGroup, "->", %object);
    
    %proxyObject = new Sprite();
    %proxyObject.setSize(%object.getSize());
    %proxyObject.setPosition(%object.getPosition());
    %proxyObject.objectName = %launcherSceneObjectGroup.getInternalName();

    %previewObject = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectName);

    if (%previewObject.Image !$= "")
    {
        %static = true;
        %preview = %previewObject.Image;
    }
    else
    {
        %static = false;
        %preview = %previewObject.Animation;
    }

    %proxyObject.Animation=(%static == true ? "" : %preview);
    %proxyObject.Image=(%static == true ? %preview : "");

    PhysicsLauncherTools::copyCollisionShapes(%object, %proxyObject);
    
    return %proxyObject;
}

// Sets collision shapes on a launcher group from a proxy
function SlingshotLauncherBuilder::setCollisionShapesFromProxy(%launcherSceneObjectGroup, %proxyObject)
{
    // Get the collision object
    %object = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);    
    
    PhysicsLauncherTools::copyCollisionShapes(%proxyObject, %object);
}

function SlingshotLauncherBuilder::updateLauncherPrefabInAllLevels(%launcherPrefab)
{
    %path = expandPath("^gameTemplate/data/levels"); 
    %fileSpec = "/*.scene.taml";   
    %pattern = %path @ %fileSpec;
    
    %file = findFirstFile(%pattern);

    while(%file !$= "")
    {
        // Read level
        %scene = TamlRead(%file);
        
        // Get the launcher objects from the launcher groups
        %prefabLauncherObject = %launcherPrefab.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
        %levelLauncherObject = LauncherSceneGroup.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
        
        // Check if the launcher object of the prefab matches the launcher object in the level
        if (%levelLauncherObject.getPrefab() $= %prefabLauncherObject.getName())
        {
            SlingshotLauncherBuilder::setLevelLauncher(%launcherPrefab);
        }
        
        // Write level
        TamlWrite(%scene, %file);
        
        // Clean up level
        PhysicsLauncherTools::deleteSceneContents(%scene);
        %scene.delete();

        %file = findNextFile(%pattern);
    }
}

function SlingshotLauncherBuilder::replaceLauncherInAllLevels(%oldLauncherPrefab, %newLauncherPrefab)
{
    %path = expandPath("^gameTemplate/data/levels"); 
    %fileSpec = "/*.scene.taml";   
    %pattern = %path @ %fileSpec;
    
    %file = findFirstFile(%pattern);

    while(%file !$= "")
    {
        // Read level
        %scene = TamlRead(%file);
        
        // Get the launcher objects from the launcher groups
        %oldLauncherObject = %oldLauncherPrefab.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
        %newLauncherObject = %newLauncherPrefab.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
        
        // Check if the launcher object of the prefab matches the launcher object in the level
        if (%newLauncherObject.getName() $= %oldLauncherObject.getName())
        {
            SlingshotLauncherBuilder::setLevelLauncher(%newLauncherPrefab);
        }
        
        // Write level
        TamlWrite(%scene, %file);
        
        // Clean up level
        PhysicsLauncherTools::deleteSceneContents(%scene);
        %scene.delete();
        
        %file = findNextFile(%pattern);
    }
}

function SlingshotLauncherBuilder::setLevelLauncher(%launcherSceneObjectGroup)
{
    %levelLauncher = LauncherSceneGroup;  
    
    // Set the name of the launcher in the scene
    SlingshotLauncherBuilder::setName(%levelLauncher, SlingshotLauncherBuilder::getName(%launcherSceneObjectGroup));
    
    %launcherObj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
    %launcherPrefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
    %launcherObj.setPrefab(%launcherPrefab.getName());
    
    %foregroundObj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
    %foregroundPrefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkForegroundObjectInternalName);
    %foregroundObj.setPrefab(%foregroundPrefab.getName());
    %foregroundObj.setSize(%foregroundPrefab.getSize());
    
    %backgroundObj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
    %backgroundPrefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::ForkBackgroundObjectInternalName);
    %backgroundObj.setPrefab(%backgroundPrefab.getName());
    %backgroundObj.setSize(%backgroundPrefab.getSize());
    
    %collisionObj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    %collisionPrefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    %collisionObj.setPrefab(%collisionPrefab.getName());
    
    //TODO scale collision shapes
    
    %collisionObj.setSize(%collisionPrefab.getSize());
    

    %band0Obj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::Band0InternalName);
    %band0Prefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::Band0InternalName);
    %band0Obj.setPrefab(%band0Prefab.getName());
    %band0Obj.setSize(%band0Prefab.getSize());
    
    %band1Obj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::Band1InternalName);
    %band1Prefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::Band1InternalName);
    %band1Obj.setPrefab(%band1Prefab.getName());
    %band1Obj.setSize(%band1Prefab.getSize());
    
    %seatObj = %levelLauncher.findObjectByInternalName($SlingshotLauncherBuilder::SeatInternalName);
    %seatPrefab = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::SeatInternalName);
    %seatObj.setPrefab(%seatPrefab.getName());
    %seatObj.setSize(%seatPrefab.getSize());
}
