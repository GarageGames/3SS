//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
$Input::noGesture = -1;
$Input::pinchGesture = 0;
$Input::scaleGesture = 1;
$Input::swipeGesture = 2;
$Input::panGesture = 3;

// Called when an InputTracker object is created
function InputTracker::onAdd(%this)
{
}

// Called when an InputTracker object is destroyed
function InputTracker::onRemove(%this)
{
}

// Tells the InputTracker to start
function InputTracker::trackTouch(%this, %touchId, %worldPos)
{
    if (%touchId > %this.maxTouches - 1)
        return;
    
    %touchField = "touch" @ %touchID;
    
    %this.setFieldValue(%touchField, %worldPos);
}

function InputTracker::recordMove(%this, %touchId, %worldPos)
{
    if (%touchId > %this.maxTouches - 1)
        return;
    
    %touchField = "touch" @ %touchID;
    %lastTouchField = "lastTouch" @ %touchID;
    %lastTouchValue = %this.getFieldValue(%touchField);
    
    %newFieldValue = %worldPos;
    
    %this.setFieldValue(%touchField, %newFieldValue);
    %this.setFieldValue(%lastTouchField, %lastTouchValue);
}

function InputTracker::updateRecordedMove(%this, %touchId, %offsetX, %offsetY)
{
    if (%touchId > %this.maxTouches - 1)
        return;
        
    %touchField = "touch" @ %touchID;
    %touch = %this.getFieldValue(%touchField);
    %touchXValue = getWord(%touch, 0) - %offsetX;
    %touchYValue = getWord(%touch, 1) - %offsetY;
    
    %this.setFieldValue(%touchField, %touchXValue SPC %touchYValue);
}

function InputTracker::releaseTouches(%this, %touchId, %worldPos)
{
}

function InputTracker::calculatePinch(%this, %touchOne, %touchTwo)
{
}

function InputTracker::calculateScale(%this, %touchOne, %touchTwo)
{
}

function InputTracker::getHorizontalPanAmount(%this, %touchID)
{
    %touchField = "touch" @ %touchID;
    %lastTouchField = "lastTouch" @ %touchID;
    
    %touch = %this.getFieldValue(%touchField);
    %lastTouch = %this.getFieldValue(%lastTouchField);
    
    %touchXValue = getWord(%touch, 0);
    %lastTouchXValue = getWord(%lastTouch, 0);
    
    %xDifference = %touchXValue - %lastTouchXValue;
        
    return %xDifference;
}

function InputTracker::getVerticalPanAmount(%this, %touchID)
{
    %touchField = "touch" @ %touchID;
    %lastTouchField = "lastTouch" @ %touchID;
    
    %touch = %this.getFieldValue(%touchField);
    %lastTouch = %this.getFieldValue(%lastTouchField);
    
    %touchYValue = getWord(%touch, 1);
    %lastTouchYValue = getWord(%lastTouch, 1);
    %yDifference = %touchYValue - %lastTouchYValue;
        
    return %yDifference;
}