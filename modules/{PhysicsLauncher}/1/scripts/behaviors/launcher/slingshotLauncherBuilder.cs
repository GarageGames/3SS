//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$SlingshotLauncherBuilder::BaseDistanceScale = 1.0;

if(!isObject(SlingshotLauncherBuilderBehavior))
{
    %template = new BehaviorTemplate(SlingshotLauncherBuilderBehavior);

    %template.addBehaviorInput(onLoad, "On Load", "Trigger load functionality");
    %template.addBehaviorInput(onMove, "On Move", "Trigger move functionality");
    %template.addBehaviorInput(onLaunch, "On Launch", "Triggers launch functionality");
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function SlingshotLauncherBuilderBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
    
    %this.rubberBandCount = 0;
    %this.currentLoadedObject = ""; 
}

/// <summary>
/// Called on the behavior when the level has been loaded. The function 
/// is responsible for hooking up the behavior with all the component objects
/// such as the launcher, collisionObject, seat, and rubberbands. It currently finds
/// the objects by searching the sceneObjectGroup "LauncherSceneGroup" for
/// objects with the correct internal names.
/// </summary>
function SlingshotLauncherBuilderBehavior::onLevelLoaded(%this)
{
    // Get the SimGroup container the owner of this behavior
    %launcherSimGroup = LauncherSceneGroup;
    
    // Set lauchher
    %this.launcher = %this.owner;
    
    // Set Collision Object 
    %collisionObject = %launcherSimGroup.findObjectByInternalName("CollisionObject");
    
    if (isObject(%collisionObject))
        %this.launcher.callOnBehaviors("setLauncherCollisionObject", %collisionObject);
    
    // Set Seat
    %this.seatObject = %launcherSimGroup.findObjectByInternalName("SeatObject");
    
    // Get the rubberband objects
    for (%i = 0; %i < %launcherSimGroup.getCount(); %i++)
    {
        %object = %launcherSimGroup.getObject(%i);
        
        %internalName = %object.getInternalName();
        
        if (stripChars(%internalName, "0123456789") $= "BandObject")
        {
            %this.addRubberbandObject(%object);
        }
    }
}

/// <summary>
/// Sets the launcher's seat.
/// </summary>
/// <param name="sceneObject">Object to set for the seat.</param>
function SlingshotLauncherBuilderBehavior::setSeatObject(%this, %sceneObject)
{
    %this.seatObject = %sceneObject;
}

/// <summary>
/// Updates the position of the seat relative to the current position and orientation
/// of the loaded object. The seat is centered on the left edge of the loaded object
/// with the same orientation.
/// </summary>
function SlingshotLauncherBuilderBehavior::updateSeat(%this)
{
    if (!isObject(%this.seatObject))
    {
        error("SlingshotLauncherBuilderBehavior::updateSeatPosition - seatObject does not exist");
        return;
    }
    
    if (!isObject(%this.currentLoadedObject))
    {
        error("SlingshotLauncherBuilderBehavior::updateSeatPosition - loaded object does not exist");
        return;
    }
    
    %targetPoint = %this.launcher.callOnBehaviors("getLoadedObjectPosition");
    %worldLaunchPoint = %this.launcher.callOnBehaviors("getLaunchPoint");
    %direction = Vector2Sub(%worldLaunchPoint, %targetPoint);
    
    // If the direction vector is 0,0, add a small offset
    if (%direction.x == 0 && %direction.y == 0)
    {
        %direction.x = 0.01;   
    }
    
    // Set the angle
    %this.seatObject.setAngle(mRadToDeg(mAtan(%direction.y, %direction.x)));
    
    // Shift the seat's position based on the currentLoadedObject's width    
    %seatOffset = -1.0 * %this.currentLoadedObject.getWidth()/2;
    %offsetVector = Vector2Scale(Vector2Normalize(%direction), %seatOffset);  
    
    // Set position
    %this.seatObject.setPosition(Vector2Add(%targetPoint, %offsetVector));
}

/// <summary>
/// Attaches the rubberband object to the launcher object and the seat object
/// and stores a reference to the rubberband object.
/// </summary>
/// <param name="sceneObject">An object with the ScaleBetweenPointsBehavior to use as a launcher rubberband.</param>
function SlingshotLauncherBuilderBehavior::addRubberbandObject(%this, %sceneObject)
{
    if (!isObject(%sceneObject))
    {
        error("SlingshotLauncherBuilderBehavior::addRubberbandObject - sceneObject does not exist");
        return;
    }
    
    %attachmentPoints = %sceneObject.callOnBehaviors("getAttachmentPoints");
    
    if (%attachmentPoints $= "ERR_CALL_NOT_HANDLED")
    {
        error("SlingshotLauncherBuilderBehavior::addRubberbandObject - sceneObject does not have behavior method getAttachmentPoints");
        return;
    }
    
    %objectStart = %this.owner;
    %objectEnd = %this.seatObject;
    %localPointStart = getWords(%attachmentPoints, 0, 1);
    %localPointEnd = getWords(%attachmentPoints, 2, 3);

    %result = %sceneObject.callOnBehaviors("attach", %objectStart, %objectEnd);
    
    if (%result $= "ERR_CALL_NOT_HANDLED")
    {
        error("SlingshotLauncherBuilderBehavior::addRubberbandObject - sceneObject does not have behavior method attach");
        return;
    }
    
    %this.rubberBandArray[%this.rubberBandCount] = %sceneObject;
    
    %this.rubberBandCount++;
}

/// <summary>
/// Returns the number of rubberband objects added to the behavior.
/// </summary>
/// <return>Integer count of the number of rubberband objects.</return>
function SlingshotLauncherBuilderBehavior::getRubberbandCount(%this)
{
    return %this.rubberBandCount;
}

/// <summary>
/// Returns the rubberband object for the specified index.
/// </summary>
/// <param name="index">The index identifying the rubberband object.</param>
/// <return>SceneObject for the rubberband.</return>
function SlingshotLauncherBuilderBehavior::getRubberbandObject(%this, %index)
{
    if (%index >= %this.rubberBandCount)
    {
        warn("SlingshotLauncherBuilderBehavior::getRubberbandObject -- invalid rubberband index: " @ %index); 
        return "";  
    }
    
    return %this.rubberBandArray[%index];
}

/// <summary>
/// Updates the orientation and scaling of the rubberbands according to the
/// position of the loaded object. The thickness scale ratio for the rubberband
/// is calculated as the ratio of the stretch distance to a base distance scale.
/// </summary>
function SlingshotLauncherBuilderBehavior::updateRubberbands(%this)
{
    // Set the base distance to base the thickness scaling on
    %baseDistance = $SlingshotLauncherBuilder::BaseDistanceScale;    
    
    %targetPoint = %this.launcher.callOnBehaviors("getLoadedObjectPosition");
    %worldLaunchPoint = %this.launcher.callOnBehaviors("getLaunchPoint");
    %distance = Vector2Distance(%worldLaunchPoint, %targetPoint);
    %thicknessScaleRatio = %distance/%baseDistance;    
    
    for (%i = 0; %i < %this.rubberBandCount; %i++)
    {
        %this.rubberBandArray[%i].update(%thicknessScaleRatio);
    }
}

/// <summary>
/// Input function triggered when an object has been loaded into the launcher.
/// Adds the seat and rubberband objects to the scene and updates them.
/// </summary>
/// <param name="fromBehavior">The output behavior that triggered this input.</param>
/// <param name="fromOutput">The output method that triggered this input.</param>
function SlingshotLauncherBuilderBehavior::onLoad(%this, %fromBehavior, %fromOutput)
{
    %this.currentLoadedObject = %this.launcher.callOnBehaviors("getLoadedObject");
    
    if (!isObject(%this.currentLoadedObject))
    {
        error("SlingshotLauncherBuilderBehavior::onLoad - failed to get loaded object.");
        return;
    }
    
    // Add seat to scene
    %this.seatObject.setVisible(true);
    %this.seatObject.addToScene(%this.scene);    

    // Attach any rubberbands to the seat and launcher
    for (%i = 0; %i < %this.rubberBandCount; %i++)
    {
        %this.rubberBandArray[%i].setVisible(true);
        %this.rubberBandArray[%i].addToScene(%this.scene);
    }
    
    // Update seat
    %this.updateSeat();
        
    // Update the rubberbands
    %this.updateRubberbands();
}

/// <summary>
/// Input function triggered when the loaded object has been moved.
/// Calls update on the seat and rubberband objects.
/// </summary>
/// <param name="fromBehavior">The output behavior that triggered this input.</param>
/// <param name="fromOutput">The output method that triggered this input.</param>
function SlingshotLauncherBuilderBehavior::onMove(%this, %fromBehavior, %fromOutput)
{
    if (!isObject(%this.currentLoadedObject))
        return;    
    
    // Update seat
    %this.updateSeat();
        
    // Update the rubberbands
    %this.updateRubberbands();
}

/// <summary>
/// Input method called when the loaded object has been launched.
/// </summary>
/// <param name="fromBehavior">The output behavior that triggered this input.</param>
/// <param name="fromOutput">The output method that triggered this input.</param>
function SlingshotLauncherBuilderBehavior::onLaunch(%this, %fromBehavior, %fromOutput)
{
    // Do nothing if we have no object loaded
    if (!isObject(%this.currentLoadedObject))
    {
        %this.currentLoadedObject = "";
        return; 
    }
    
    // Clear the loaded object reference
    %this.currentLoadedObject = "";
    
    // Remove the rubberbands 
    for (%i = 0; %i < %this.rubberBandCount; %i++)
    {
        %this.scene.removeFromScene(%this.rubberBandArray[%i]);
    }   
    
    // Remove the seat
    %this.scene.removeFromScene(%this.seatObject);
}

