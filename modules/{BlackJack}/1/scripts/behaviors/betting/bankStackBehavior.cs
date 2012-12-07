//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
if (!isObject(BankStackBehavior))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it BankStackBehavior
    %template = new BehaviorTemplate(BankStackBehavior);

    // friendlyName will be what is displayed in the editor
    %template.friendlyName = "Bank Stack";

    // behaviorType organize this behavior in the editor
    %template.behaviorType = "Betting Component";

    // description briefly explains what this behavior does
    %template.description  = "Displays chip buttons for your bank stack";

    //--------------------
    // Behavior Fields
    //--------------------

    %template.addBehaviorField(isEnabled, "Whether or not the bank chip stack is enabled for the table", bool, true);
    %template.addBehaviorField(playerBank, "The player bank object this bank chip stack is connected to", object, "", SceneObject);
    %template.addBehaviorField(denomination, "The chip denomination of this bank stack", int, 1);
    %template.addBehaviorField(defaultBetArea, "The bet area object to use as the default location for bets from this behavior", object, "", SceneObject);
    %template.addBehaviorField(minDragDistance, "The distance in world coordinates required to drag a chip off the stack", float, 10);
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function BankStackBehavior::onBehaviorAdd(%this)
{
    %this.touchID = "";
    %this.owner.UseInputEvents = true;
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function BankStackBehavior::onAddToScene(%this, %scene)
{
    %this.isDraggingOffStack = false;
    %this.scene = %scene;
}

/// <summary>
/// Called when a level with the behavior is finished loading
/// </summary>
function BankStackBehavior::onLevelLoaded(%this)
{
    %this.updateScheduleHandle = %this.schedule(200, "updateLoop");
}

/// <summary>
/// Called when a level with the behavior is finished unloading
/// </summary>
function BankStackBehavior::onLevelEnded(%this)
{
    if (isEventPending(%this.updateScheduleHandle))
        cancel(%this.updateScheduleHandle);
}

/// <summary>
/// Periodically schedules an update of the behavior
/// </summary>
function BankStackBehavior::updateLoop(%this)
{
    if (!isObject(%this))
        return;

    if (isObject($UserControlledPlayer))
        %this.playerBank = $UserControlledPlayer.bank;

    %this.update();

    if (isEventPending(%this.updateScheduleHandle))
        cancel(%this.updateScheduleHandle);
    %this.updateScheduleHandle = %this.schedule(200, "updateLoop");
}

/// <summary>
/// Updates the visibility of the behavior's owner
/// according to how much cash is in the bank this behavior
/// is linked to.
/// </summary>
function BankStackBehavior::update(%this)
{
    if (isObject(%this.playerBank))
    {
        if (%this.playerBank.currentCash < %this.denomination || %this.isEnabled() == false)
            %this.owner.setVisible(false);
        else
            %this.owner.setVisible(true);
    }
}

/// <summary>
/// onTouchDown callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function BankStackBehavior::onTouchDown(%this, %touchID, %worldPos)
{
    if (%this.touchID $= "")
    {
        // If we are not in the betting stage, do not register mouse clicks
        if ($CurrentTable.state !$= "Ready")
            return;

        %this.touchID = %touchID;

        // Save the starting touch position
        %this.touchDownPosition = %worldPos;
    }
}

/// <summary>
/// onTouchUp callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function BankStackBehavior::onTouchUp(%this, %touchID, %worldPos)
{
    if (%this.touchID $= %touchID)
    {
        %this.touchID = "";

        // Remove cloned chip if it exists
        if (isObject(%this.clone))
            %this.clone.safeDelete();


        if (%this.isDraggingOffStack)
        {
            // Clear the dragging flag
            %this.isDraggingOffStack = false;

            // Find the BetAreaBehavior object that overlaps worldPos
            %targetBetArea = %this.getOverlappingObject(%worldPos, "BetAreaBehavior");
            echo(%targetBetArea);

            // If the object exists, get a handle to the behavior
            if (isObject(%targetBetArea))
            {
                // Check that the seat is owned by the current player
                if ((%targetBetArea != $UserControlledPlayer.seat.mainBetArea.getId()) &&
                        (%targetBetArea != $UserControlledPlayer.seat.sideBetArea.getId()))
                    return;

                // Use the behavior handle to add a bet to the object
                if (%targetBetArea.callOnBehaviors(isBetLegal, %this.denomination))
                {
                    %addBetSuccess = %targetBetArea.callOnBehaviors(addBet, %this.denomination, %this.playerBank);
                    %targetBetArea.callOnBehaviors(update);
                }
            }
        }
        else
        {
            if (!isObject($UserControlledPlayer.seat.mainBetArea))
                return;

            // Call the bet function from the defaultBetArea
            %betArea = $UserControlledPlayer.seat.mainBetArea;
            if (isObject(%betArea))
            {
                // add a bet to the object
                if (%betArea.callOnBehaviors(isBetLegal, %this.denomination))
                    %addBetSuccess = %betArea.callOnBehaviors(addBetWithAnimation, %this, %this.playerBank);
            }
        }

        %this.update();
    }
}

/// <summary>
/// onTouchDragged callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function BankStackBehavior::onTouchDragged(%this, %touchID, %worldPos)
{
    if (%this.touchID $= %touchID)
    {
        // If have already dragged far enough to set flag, do not check again
        if (%this.isDraggingOffStack == false)
        {
            // Calculate distance we have dragged from our touchDown point
            %dragDistance = Vector2Distance(%this.touchDownPosition, %worldPos);

            //Check if we have dragged far enough off stack
            if (%dragDistance >= %this.minDragDistance)
            {
                // Set dragging flag
                %this.isDraggingOffStack = true;

                // Clone the chip
                %this.clone = %this.owner.clone();
                //%dragBehavior = ChipDraggingBehavior.createInstance();
                //%dragBehavior.denomination = %this.denomination;
                //%dragBehavior.scene = %this.scene;
                //%clone.addBehavior(%dragBehavior);
                //%this.clone.position = %worldPos;

            }
        }


        if (isObject(%this.clone))
        {
            %this.clone.position = %worldPos;
        }
    }
}

/// <summary>
/// Returns the nearest object that overlaps the given worldPos
/// and that has the specified behavior.
/// </summary>
/// <param name="worldPos">The world coordinates that overlap is checked for.</param>
/// <param name="behaviorType">Behavior string that the overlap object must match.</param>
/// <return>An object ID if an overlapping object exists, or a blank string if not.</return>
function BankStackBehavior::getOverlappingObject(%this, %worldPos, %behaviorType)
{
    // Get a list of all objects in the scene
    %sceneObjectList = %this.scene.GetSceneObjectList();

    // Get a count of objects in the scene
    %numOfSceneObjects = %this.scene.GetSceneObjectCount();

    // Initialize variable for keeping track of the nearest object
    %nearestObject = "";

    // Iterate through the objects in the scene to see if they have the behavior
    for (%i = 0; %i < %numOfSceneObjects; %i++)
    {
        // Get a handle to the object's Behavior
        %currentObject = getWord(%sceneObjectList, %i);
        %behavior = %currentObject.getBehavior(%behaviorType);

        // If the behavior exists for that object
        if (isObject(%behavior))
        {
            // check if the worldPos is inside the object
            if (%currentObject.getIsPointInOOBB(%worldPos))
            {
                //echo(%currentObject);

                // Check if the object is the nearest we've found
                if (isObject(%nearestObject))
                {
                    %oldDistance = Vector2Distance(%nearestObject.position, %worldPos);
                    %currentDistance = Vector2Distance(%currentObject.position, %worldPos);
                    if (%currentDistance < %oldDistance)
                        %nearestObject = %currentObject;
                }
                else
                {
                    %nearestObject = %currentObject;
                }
            }
        }
    }

    return %nearestObject;
}

function BankStackBehavior::isEnabled(%this)
{
    return %this.isEnabled;
}

function BankStackBehavior::setEnabled(%this, %value)
{
    %this.isEnabled = %value;
}