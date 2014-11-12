//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(InputTargetBehavior))
{
    %template = new BehaviorTemplate(InputTargetBehavior);
    %template.friendlyName = "Input Target";
    %template.behaviorType = "Input";
    %template.description = "Adds an input responder to the collision area of the owner of the behavior";

    // Outputs
    %template.addBehaviorOutput(inputDown, "Input Down Event", "Signals that a down event has occurred");
    %template.addBehaviorOutput(inputUp, "Input Up Event", "Signals that an up event has occured");
    %template.addBehaviorOutput(inputDrag, "Input Drag Event", "Signals that a drag event has occured");
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function InputTargetBehavior::onBehaviorAdd(%this)
{
    //TODO: make sure the owner reacts to touch events on its collision area
   
    // Clear the touch variables
    %this.touchId = "";
    %this.lastTouchId = "";
    %this.touchPosition = "";
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function InputTargetBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
}

/// <summary>
/// Handles touch down event and raises an output signal.
/// </summary>
function InputTargetBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    // If we already have have another touch event being handled, return
    if (%this.touchId !$= "")
        return;  
    else
        %this.touchId = %touchID;
   
    // Save the touch data
    %this.lastTouchId = %touchID;
    %this.touchPosition = %worldPos;
    
    // Add this behavior to list of input-locked objects
    sceneWindow2D.addLockedObject(%this);
   
    // Raise output event
    %this.owner.Raise(%this, inputDown);
}

/// <summary>
/// Handles touch up event and raises an output signal.
/// </summary>
function InputTargetBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    // If the touchId is not the same as the touchDown event, return
    if (%this.touchId !$= %touchID) 
        return; 
    else
        %this.touchId = "";
   
    // Save the touch data
    %this.lastTouchId = %touchID;
    %this.touchPosition = %worldPos;
    
    // Remove this behavior to list of input-locked objects
    sceneWindow2D.removeLockedObject(%this);
   
    // Raise output event
    %this.owner.Raise(%this, inputUp);
}

/// <summary>
/// Handles touch dragged event and raises an output signal.
/// </summary>
function InputTargetBehavior::onTouchDragged(%this, %touchID, %worldPos)
{
    // If the touchId is not the same as the touchDown event, return
    if (%this.touchId !$= %touchID) 
        return; 
      
    // Save the touch data
    %this.lastTouchId = %touchID;
    %this.touchPosition = %worldPos;
   
    // Raise output event
    %this.owner.Raise(%this, inputDrag);
}

/// <summary>
/// Gets the Id of the last touch event
/// </summary>
/// <return>int Touch Id of the last touch.</return>
function InputTargetBehavior::getLastTouchId(%this)
{
    return %this.lastTouchId;  
}

/// <summary>
/// Gets the position of the last touch event
/// </summary>
/// <return>Vector2F position of the last touch.</return>
function InputTargetBehavior::getTouchPosition(%this)
{
    return %this.touchPosition;  
}