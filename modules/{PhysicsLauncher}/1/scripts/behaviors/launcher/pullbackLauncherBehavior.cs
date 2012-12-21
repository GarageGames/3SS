//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Unique collision group for the launcher
$PullbackLauncherCollisionGroup = 5;

// Determines how smoothly launcher collisions will be handled
// Smaller values mean more processing time, but better visual results
$PullbackLauncherCollisionCheckPrecision = 0.005;

if(!isObject(PullbackLauncherBehavior))
{
    %template = new BehaviorTemplate(PullbackLauncherBehavior);
    %template.friendlyName = "Pull-back Launcher";
    %template.behaviorType = "Launcher";
    %template.description = "Provides a pullback-type launcher for launching objects.";
       
    //%template.addBehaviorField(anchorPoints, "A list of anchor points that are used to calculate the start position of a projectile", Default, "");
    %template.addBehaviorField(maxSpeed, "The speed applied at maximum extension of the launcher", Float, 0.0);
    %template.addBehaviorField(maxDistance, "The maximum distance the launcher can be extended", Float, 0.0);
    %template.addBehaviorField(collisionObject, "The object that makes up the collision shape(s) of the launcher", object, "", SceneObject);
    %template.addBehaviorField(stretchThreshold, "Fraction of the maxDistance that will trigger the stretchOutput", Float, 0.5);

    // Inputs
    %template.addBehaviorInput(setTargetPosition, "Set Target Position", "Sets the target pullback position for the launcher");
    %template.addBehaviorInput(launch, "Launch Object", "Triggers the launching of the loaded object");
    
    // Outputs
    %template.addBehaviorOutput(onLoad, "On Load", "Output raised when the behavior has finished loading");
    %template.addBehaviorOutput(onUnload, "On Unload", "Output raised when the behavior has finished unloading");
    %template.addBehaviorOutput(onSetTarget, "On Set Target", "Output raised when the behavior has finished setting the target position");
    %template.addBehaviorOutput(onLaunch, "On Launch", "Output raised when the behavior has finished launching");
    %template.addBehaviorOutput(stretchOutput, "Stretch Output", "Output raised when the pullback distance reaches the stretch threshold");
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function PullbackLauncherBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
    
    // Create a field for the previous stretch distance
    // Used to check when we exceed the stretch threshold
    %this.lastStretchDistance = 0;
}

/// <summary>
/// Deletes the loaded object and raises the OnUnload signal
/// </summary>
function PullbackLauncherBehavior::unload(%this)
{
    if (!isObject(%this.loadedObject))
        return;
        
    %this.loadedObject.safeDelete();
    %this.loadedObject = "";
    
    %this.owner.Raise(%this, OnUnload);
}

/// <summary>
/// Sets the loadedObject for the behavior, positions the object
/// at the launch point
/// </summary>
/// <param name="object">The scene object to load in the launcher.</param>
function PullbackLauncherBehavior::load(%this, %object)
{
    if (isObject(%this.loadedObject))
        return;
        
    // Get local launch point
    %this.localLaunchPoint = %this.owner.getCircleCollisionShapeLocalPosition(0);

    // Save a reference to the loaded object
    %this.loadedObject = %object; 
    
    // Set the proxy collision shapes
    %this.setProxy(%this.loadedObject);

    // Get the launch point in world coordinates
    %this.target = %this.owner.getWorldPoint(%this.localLaunchPoint);  
    
    // Set the position of the object to the launch point
    %this.loadedObject.setPosition(%this.target);
    %this.loadedObject.addToScene(sceneWindow2D.getScene());
    %this.loadedObject.setSceneLayer(15);
    %this.loadedObject.callOnBehaviors("load");
    
    // Disable physics on the object
    %this.loadedObject.setActive(false);
    %this.loadedObject.setAwake(false);

    %this.owner.Raise(%this, onLoad);
}

/// <summary>
/// Input method that sets the position of the target based
/// on the getTouchPosition method defined by the fromBehavior.
/// </summary>
/// <param name="fromBehavior">The output behavior that triggered this input.</param>
/// <param name="fromOutput">The output method that triggered this input.</param>
function PullbackLauncherBehavior::setTargetPosition(%this, %fromBehavior, %fromOutput)
{
	// Lock the mouse so that we still get drag notifications when the user drags outside the window
    sceneWindow2D.lockMouse = true;
    
    if (!isObject(%this.loadedObject))  
        return;    
        
    // Get local launch point
    %this.localLaunchPoint = %this.owner.getCircleCollisionShapeLocalPosition(0);
        
    // Check if the from behevior has the required method
    if (!%fromBehavior.isMethod(getTouchPosition))
    {
          error("PullbackLauncherBehavior::setTargetPosition - [" @ %fromBehavior @"] does not contain the method getTouchPosition");
          return;         
    }
    
    sceneWindow2D.canMoveCamera = false;
    
    // Get the position target from the from behavior
    %targetPosition = %fromBehavior.getTouchPosition();
    
    // Get the launch point in world coordinates
    %worldLaunchPoint = %this.owner.getWorldPoint(%this.localLaunchPoint);
   
    // Clamp the position according to the maxDistance
    %distance = Vector2Distance(%targetPosition, %worldLaunchPoint);
    if (%distance > %this.maxDistance)
    {
        // Calculate vector from launch point to targetPosition
        %displacementVector = Vector2Sub(%targetPosition, %worldLaunchPoint);
      
        // Scale the vector down to have a length of maxDistance
        %scaledVector = Vector2Scale(%displacementVector, %this.maxDistance / %distance);
      
        // Get the new targetPosition
        %targetPosition = Vector2Add(%scaledVector, %worldLaunchPoint);
    }
    
    // Raycast on the launchers collision shape(s)
    %raycastDetails = "";
    %raycastTargetPosition = %targetPosition;
    
    if (%worldLaunchPoint !$= %targetPosition)
        %raycastDetails = %this.scene.pickRayCollision(%worldLaunchPoint, %targetPosition, -1, -1);
    
    if (%raycastDetails !$= "")
    {    
        %nearestPoint = %targetPosition;
        for (%i = 0; %i < getWordCount(%raycastDetails); %i+=7)
        {
            %object = getWord(%raycastDetails, %i);
            %shapeIndex = getWord(%raycastDetails, %i+6);
            
            %isSensor = %object.getCollisionShapeIsSensor(%shapeIndex);

            if ((%object !$= %this.loadedObject) && !(%isSensor))
            {
                %nearestPoint = getWords(%raycastDetails, %i+1, %i+2);
                break;
            }
        }

        %raycastTargetPosition = %nearestPoint;  
    }

     // Calculate the shapeCast distance
    %shapeCastDistance = Vector2Distance(%targetPosition, %worldLaunchPoint); 
    
     // Calculate the shapeCast direction
    %shapeCastDirection = Vector2Normalize(Vector2Sub(%targetPosition, %worldLaunchPoint));
    
    // Position the object proxy at the launch point
    %this.loadedObjectProxy.setPosition(%worldLaunchPoint);
    
    // Perform shape-cast for each collision shape
    %minDistance = %shapeCastDistance;
    for (%i = 0; %i < %this.loadedObjectProxy.getCollisionShapeCount(); %i++)
    {
        %shapeCastContacts = %this.loadedObjectProxy.ShapeCastComponent.castShape(%targetPosition, %i);
        
        if (%shapeCastContacts !$= "")
        {
            %object = getWord(%shapeCastContacts, 0);
            %shapeIndex = getWord(%shapeCastContacts, 1);
            %fraction = getWord(%shapeCastContacts, 2);
            
            // Ignore contacts with the behavior's owner
            if (%object $= %this.owner)
                continue;           
            
            // Calculate the object's displacement distance            
            %currentDistance = %shapeCastDistance * %fraction;
            
            // Check if it is less than the current minimum
            if (%currentDistance < %minDistance)
                %minDistance = %currentDistance;
        }
    }
    
    // Calculate the new targetPosition
    %displacementVector = Vector2Scale(%shapeCastDirection, %minDistance);
    %targetPosition = Vector2Add(%worldLaunchPoint, %displacementVector);
    
    // Set the angle and position of the loaded object
    %launchDirectionVector = Vector2Sub(%worldLaunchPoint, %targetPosition); 
    %this.loadedObject.setAngle(mRadToDeg(mAtan(%launchDirectionVector.y, %launchDirectionVector.x)));

    %this.loadedObject.setPosition(%targetPosition);
    
    // Update the target
    %this.target = %targetPosition;
    
    // Raise signal that the target has been set
    %this.owner.Raise(%this, onSetTarget);
    
    // Raise signal that the stretch threshold has been reached
    %distance = Vector2Distance(%targetPosition, %worldLaunchPoint);
    %thresholdDistance = %this.maxDistance * %this.stretchThreshold;
    if ((%distance > %thresholdDistance) && (%this.lastStretchDistance < %thresholdDistance))
        %this.owner.Raise(%this, stretchOutput);
    %this.lastStretchDistance = %distance;
    
    // Try to make the target point visibile
    %windowPosition = sceneWindow2D.getWindowPoint(%targetPosition);
    %camera = sceneWindow2D.getCurrentCameraArea();
    %viewLeft = getWord(%camera, 0);
    %viewRight = getWord(%camera, 2);
    %viewTop = getWord(%camera, 3);
    %viewBottom = getWord(%camera, 1);
    if( %targetPosition.x < %viewLeft || 
        %targetPosition.x > %viewRight ||
        %targetPosition.y < %viewBottom ||
        %targetPosition.y > %viewTop )
    {
        %viewWidth = %viewRight - %viewLeft;
        %viewHeight = %viewTop - %viewBottom;
        %centerX = %viewLeft + (%viewWidth * 0.5);
        %centerY = %viewBottom + (%viewHeight * 0.5);
        echo("Camera: " @ %centerX SPC %centerY SPC %viewLeft SPC %viewRight SPC %targetPosition.x);
        if( %targetPosition.x < %viewLeft  ||
            %targetPosition.x > %viewRight )
        {
            %centerX -= (%viewLeft - %targetPosition.x);
        }
        if( %targetPosition.y < %viewBottom ||
            %targetPosition.y > %viewTop )
        {
            %centerY -= (%viewBottom - %targetPosition.y);
        }
        echo("New Camera: " @ %centerX SPC %centerY);
        sceneWindow2D.setCurrentCameraPosition(%centerX, %centerY);
        sceneWindow2D.clampCurrentCameraViewLimit();
    }
}

/// <summary>
/// Gets the pull-back target position in world coordinates
/// </summary>
/// <return>Vector2F Position in world coordinates</return>
function PullbackLauncherBehavior::getTargetPosition(%this)
{
    return %this.target;
}

/// <summary>
/// Gets the pull-back loaded object's position in world coordinates
/// </summary>
/// <return>Vector2F Position in world coordinates</return>
function PullbackLauncherBehavior::getLoadedObjectPosition(%this)
{
    return %this.loadedObject.getPosition(); 
}

/// <summary>
/// Gets the launch point in world coordinates
/// </summary>
/// <return>Vector2F Position in world coordinates</return>
function PullbackLauncherBehavior::getLaunchPoint(%this)
{
    return %this.owner.getWorldPoint(%this.localLaunchPoint);
}

/// <summary>
/// Gets the currently loaded object if it exists
/// </summary>
/// <return>Object Id, or empty string if the object does not exist.</return>
function PullbackLauncherBehavior::getLoadedObject(%this)
{
    if (isObject(%this.loadedObject))
        return %this.loadedObject;
        
    return "";
}


/// <summary>
/// Input method that sets a linear velocity for the loaded object based
/// on the position of the target relative to the launch point.
/// </summary>
/// <param name="fromBehavior">The output behavior that triggered this input.</param>
/// <param name="fromOutput">The output method that triggered this input.</param>
function PullbackLauncherBehavior::launch(%this, %fromBehavior, %fromOutput)
{
    sceneWindow2D.lockMouse = false;
    if (!isObject(%this.loadedObject))  
        return;      

    // Get the launch point in world coordinates
    %worldLaunchPoint = %this.owner.getWorldPoint(%this.localLaunchPoint);
   
    // Calculate the velocity of the object according to its displacement from the launchPoint and maxSpeed
    %speed = (Vector2Distance(%worldLaunchPoint, %this.target) / %this.maxDistance) * %this.maxSpeed;
   
    %directionVector = Vector2Normalize(Vector2Sub(%worldLaunchPoint, %this.target));
   
    %velocityVector = Vector2Scale(%directionVector, %speed);
   
    // Enable object physics
    %this.loadedObject.setActive(true);
    %this.loadedObject.setAwake(true);
   
    sceneWindow2d.mount(%this.loadedObject);
    sceneWindow2D.canMoveCamera = false;
    
    // Set the velocity
    %this.loadedObject.setLinearVelocity(%velocityVector); 
    
    %this.loadedObject.callOnBehaviors("launch");    
    
    %this.loadedObject.setSceneLayer(15);
    %this.loadedObject = "";
    
    %this.owner.Raise(%this, onLaunch);
}

function PullbackLauncherBehavior::getLauncherMaxSpeed(%this)
{
    return %this.maxSpeed;
}

function PullbackLauncherBehavior::setLauncherMaxSpeed(%this, %newSpeed)
{
    %this.maxSpeed = %newSpeed;
}

function PullbackLauncherBehavior::getLauncherMaxDistance(%this)
{
    return %this.maxDistance;
}

function PullbackLauncherBehavior::setLauncherMaxDistance(%this, %newDistance)
{
    %this.maxDistance = %newDistance;
}

function PullbackLauncherBehavior::getLauncherCollisionObject(%this)
{
    return %this.collisionObject;
}

function PullbackLauncherBehavior::setLauncherCollisionObject(%this, %object)
{
    %this.collisionObject = %object;    
    
    // Set the launcher collision object to its own collision group
    if (isObject(%this.collisionObject))
        %this.collisionObject.setCollisionGroups($PullbackLauncherCollisionGroup);
}

/// <summary>
/// Creates an invisible, inactive proxy for the loaded object. Used for collision checks.
/// </summary>
/// <param name="object">The object the proxy is created from</param>
function PullbackLauncherBehavior::setProxy(%this, %object)
{
    // Delete previous proxy if it exists
    if (isObject(%this.loadedObjectProxy))
        %this.loadedObjectProxy.safeDelete();    
    
    // Create proxy
    %this.loadedObjectProxy = new SceneObject();
    %this.loadedObjectProxy.addToScene(%this.scene);
    %this.loadedObjectProxy.setVisible(false);
    %this.loadedObjectProxy.setBodyType("static");
    %this.loadedObjectProxy.ShapeCastComponent = new ShapeCastComponent();
    
    if (!%this.loadedObjectProxy.addComponents(%this.loadedObjectProxy.ShapeCastComponent))
    {
        error("PullbackLauncherBehavior::load -- Failed to register ShapeCastComponent");
        %this.loadedObjectProxy.ShapeCastComponent.safeDelete();
        return;
    }    
    
    // Clear the proxy object collision shapes
    %this.loadedObjectProxy.clearCollisionShapes();

    // Copy collision shapes from the loaded object
    for (%i = 0; %i < %object.getCollisionShapeCount(); %i++)
    {
        // Get the collision shape format string
        %shapeString = %object.formatCollisionShape(%i);
        
        // Create a new collision shape on the proxy from the format string
        %shapeIndex = %this.loadedObjectProxy.parseCollisionShape(%shapeString);
        %this.loadedObjectProxy.setCollisionShapeIsSensor(%shapeIndex, true);
        
        if (%shapeIndex == -1)
        {
            warn("PullbackLauncherBehavior::setProxy -- failed to set a collision shape on the proxy.");
        }
    }
    
    // Set proxy object properties
    %this.loadedObjectProxy.setSceneLayer(%object.getSceneLayer());
}
