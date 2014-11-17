//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(ScaleBetweenPointsBehavior))
{
    %template = new BehaviorTemplate(ScaleBetweenPointsBehavior);
    %template.friendlyName = "Scale Between Points";
    %template.behaviorType = "Transform";
    %template.description = "Provides functionality for scaling an object between two points.";
    
    // List of possible directions to scale the object
    %template.scaleDirectionTypes = "Right" TAB "Left";    
    
    %template.addBehaviorField(instanceName, "The behavior instance name.", string, "");
    %template.addBehaviorField(attachmentStart, "Local attachment point on the start object", Default, "0 0");
    %template.addBehaviorField(attachmentEnd, "Local attachment point on the end object", Default, "0 0");
    %template.addBehaviorField(scaleDirection, "The direction in which the object is scaled.", enum, "Left", %template.scaleDirectionTypes);
    %template.addBehaviorField(minThicknessRatio, "The minumum thinkness as a ratio of the original height", Float, 0.1);
    %template.addBehaviorField(maxThicknessRatio, "The maximum thinkness as a ratio of the original height", Float, 1.0);
    %template.addBehaviorField(thicknessScalingRatio, "Thickness change multiplier for each unit of the original width that the object has been scaled", Float, 1.0);
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function ScaleBetweenPointsBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
}

/// <summary>
/// Sets the thickness scaling ratio. 
/// </summary>
/// <param name="ratio">The ratio that is multiplied by the base height for each unit of stretch distance.</param>
function ScaleBetweenPointsBehavior::setThicknessScalingRatio(%this, %ratio)
{
    %this.thicknessScalingRatio = %ratio;
}

/// <summary>
/// Returns the thickness scaling ratio.
/// </summary>
/// <return>The ratio that is multiplied by the base height for each unit of stretch distance.</return>
function ScaleBetweenPointsBehavior::getThicknessScalingRatio(%this)
{
    return %this.thicknessScalingRatio;   
}

/// <summary>
/// Sets both attachment points for the behavior. Coordinates are object-relative.
/// </summary>
/// <param name="startPoint">The starting attachment point X/Y coordinates.</param>
/// <param name="endPoint">The ending attachment point X/Y coordinates.</param>
function ScaleBetweenPointsBehavior::setAttachmentPoints(%this, %startPoint, %endPoint)
{
    %this.attachmentStart = %startPoint;
    %this.attachmentEnd = %endPoint; 
}

/// <summary>
/// Gets both attachment points for the behavior. Coordinates are object-relative.
/// </summary>
/// <return>attachmentStart X/Y attachmentEnd X/Y</return>
function ScaleBetweenPointsBehavior::getAttachmentPoints(%this)
{
    return %this.attachmentStart SPC %this.attachmentEnd;
}

/// <summary>
/// Sets the starting attachment point. Coordinates are object-relative.
/// </summary>
/// <param name="startPoint">Start point X/Y coordinates.</param>
function ScaleBetweenPointsBehavior::setAttachmentStartPoint(%this, %startPoint)
{
    %this.attachmentStart = %startPoint;
}

/// <summary>
/// returns the starting attachment point. Coordinates are object-relative.
/// </summary>
/// <return>Start point X/Y coordinates.</return>
function ScaleBetweenPointsBehavior::getAttachmentStartPoint(%this)
{
    return %this.attachmentStart;
}

/// <summary>
/// Sets the ending attachment point. Coordinates are object-relative.
/// </summary>
/// <param name="endPoint">End point X/Y coordinates.</param>
function ScaleBetweenPointsBehavior::setAttachmentEndPoint(%this, %endPoint)
{
    %this.attachmentEnd = %endPoint;
}

/// <summary>
/// Returns the ending attachment point. Coordinates are object-relative.
/// </summary>
/// <return>End point X/Y coordinates.</return>
function ScaleBetweenPointsBehavior::getAttachmentEndPoint(%this)
{
    return %this.attachmentEnd;
}

/// <summary>
/// Sets the two attachment points for the behavior to scale between.
/// Attachment points must be on objects.
/// </summary>
/// <param name="objectStart">The id of the first object to attach to.</param>
/// <param name="attachmentStart">The local attachment point on the first object.</param>
/// <param name="objectEnd">The id of the second object to attach to.</param>
/// <param name="attachmentEnd">The local attachment point of the second object.</param>
function ScaleBetweenPointsBehavior::attach(%this, %objectStart, %objectEnd)
{
    
    if(!isObject(%objectStart) || !isObject(%objectEnd))
    {
        error("ScaleBetweenPointsBehavior::attach - An object we are attempting to attach to does not exist");  
        return; 
    }
    
    %this.attachmentObjectStart = %objectStart;
    %this.attachmentObjectEnd = %objectEnd; 
    
    %this.update(%this.owner.getHeight());
}

/// <summary>
/// Clears the attachment points and attachment objects.
/// </summary>
function ScaleBetweenPointsBehavior::detach(%this)
{
    %this.attachmentObjectStart = "";
    %this.attachmentObjectEnd = "";
    %this.attachmentStart = "";
    %this.attachmentEnd = "";
}

/// <summary>
/// Get whether the behavior has been attached at both ends
/// </summary>
/// <return>Boolean indicating if the behavior is attached.</return>
function ScaleBetweenPointsBehavior::isAttached(%this)
{
    if(!isObject(%this.attachmentObjectStart) || !isObject(%this.attachmentObjectEnd))
        return false; 
        
    return true;
}

/// <summary>
/// Updates the scaling and thickness of the owner based on the attachment points
/// </summary>
/// <param name="thicknessScale">The distance scale with which to update the thickness.</param>
function ScaleBetweenPointsBehavior::update(%this, %thicknessScale)
{
    if(!isObject(%this.attachmentObjectStart) || !isObject(%this.attachmentObjectEnd))
    {
        warn("ScaleBetweenPointsBehavior::update - An attached object no longer exists.");  
        return; 
    }
    // Normalize the local points by the objects' sizes
    %attachmentStartX = %this.attachmentStart.x * %this.attachmentObjectStart.getSizeX() / 2;
    %attachmentStartY = %this.attachmentStart.y * %this.attachmentObjectStart.getSizeY() / 2;
    %attachmentEndX = %this.attachmentEnd.x * %this.attachmentObjectEnd.getSizeX() / 2;
    %attachmentEndY = %this.attachmentEnd.y * %this.attachmentObjectEnd.getSizeY() / 2;
    
    // Calculate world coordinates from the local points
    %worldPointStart = %this.attachmentObjectStart.getWorldPoint(%attachmentStartX SPC %attachmentStartY);
    %worldPointEnd = %this.attachmentObjectEnd.getWorldPoint(%attachmentEndX SPC %attachmentEndY);
    
    switch$(%this.scaleDirection) 
    {
        case "Right":
            %this.scale(%worldPointStart, %worldPointEnd, %thicknessScale);
            
        case "Left":
            %this.scale(%worldPointEnd, %worldPointStart, %thicknessScale);
    }
}

/// <summary>
/// Scales the object between 2 points and sets the thickness
/// </summary>
/// <param name="start">The start point in world coordinates.</param>
/// <param name="end">The end point in world coordinates.</param>
/// <param name="thicknessScale">Distance ratio used to scale the thickness.</param>
function ScaleBetweenPointsBehavior::scale(%this, %start, %end, %thicknessScale)
{
    // Store the original dimensions
    if (%this.baseWidth $= "")
        %this.baseWidth = %this.owner.getWidth();
    if (%this.baseHeight $= "")
        %this.baseHeight = %this.owner.getHeight();    
    
    %midpoint = Vector2Scale(Vector2Add(%start, %end), 0.5);
    
    %distance = Vector2Distance(%start, %end);
    %direction = Vector2Sub(%end, %start);
    %angle = mRadToDeg(mAtan(%direction.y, %direction.x));

    // Scale the scene object
    %this.owner.setWidth(%distance);
    
    // Set the height based on the thickness scaling ratio
    %minThickness = %this.baseHeight * %this.minThicknessRatio;
    %maxThickness = %this.baseHeight * %this.maxThicknessRatio;
    %thickness = %this.baseHeight * mPow(%this.thicknessScalingRatio, %thicknessScale);
    %thickness = mClamp(%thickness, %minThickness, %maxThickness);
    
    %this.owner.setHeight(%thickness);  
    
    // Position the object
    %this.owner.setPosition(%midpoint);
    
    // Rotate the object
    %this.owner.setAngle(%angle + %rotation);
}

