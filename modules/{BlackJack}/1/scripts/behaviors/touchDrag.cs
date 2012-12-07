//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Create this behavior only if it does not already exist
if (!isObject(TouchDrag))
{
    // Create this behavior from the blank BehaviorTemplate
    // Name it TouchDrag
    %template = new BehaviorTemplate(TouchDrag);

    // friendlyName will be what is displayed in the editor
    // behaviorType organize this behavior in the editor
    // description briefly explains what this behavior does
    %template.friendlyName = "Touch Drag";
    %template.behaviorType = "Movement Styles";
    %template.description  = "Allows the user to drag the object using the touch screen";
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function TouchDrag::onBehaviorAdd(%this)
{
    %this.touchID = "";
    %this.owner.UseInputEvents = true;
}

/// <summary>
/// onTouchDown callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function TouchDrag::onTouchDown(%this, %touchID, %worldPos)
{
    // If we are already dragging, do nothing
    // Otherwise, enable the drag variable
    if (%this.touchID $= "")
    {
        %this.touchID = %touchID;
    }
}

/// <summary>
/// onTouchUp callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function TouchDrag::onTouchUp(%this, %touchID, %worldPos)
{
    if (%this.touchID $= %touchID)
    {
        %this.touchID = "";
    }
}

/// <summary>
/// onTouchDragged callback
/// </summary>
/// <param name="touchID">ID of the touch event.</param>
/// <param name="worldPos">Position of the touch event.</param>
function TouchDrag::onTouchDragged(%this, %touchID, %worldPos)
{
    if (%this.touchID $= "")
        %this.touchID = %touchID;

    if (%this.touchID $= %touchID)
    {
        %this.owner.setPosition(%worldPos);
    }
}