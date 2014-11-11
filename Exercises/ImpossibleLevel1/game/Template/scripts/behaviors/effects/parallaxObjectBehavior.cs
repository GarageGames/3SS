//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
if (!isObject(ParallaxObjectBehavior))
{
    %template = new BehaviorTemplate(ParallaxObjectBehavior);
   
    %template.friendlyName = "Parallax Object";
    %template.behaviorType = "Camera";
    %template.description  = "Changes object position based on camera movement.";

    %template.addBehaviorField(horizontalScrollSpeed, "Percentage of horizontal scroll speed", float, 100);
    %template.addBehaviorField(verticalScrollSpeed, "Percentage of vertical scroll speed", float, 100);
    %template.addBehaviorField(tileable, "Toggles whether the image will tile when stretched", bool, false);    
}

function ParallaxObjectBehavior::onBehaviorAdd(%this)
{
    if (!$LevelEditorActive)    
        %this.owner.setUpdateCallback(true);
}

function ParallaxObjectBehavior::onLevelLoaded(%this, %scenegraph)
{
	%currentPosition = sceneWindow2d.getCurrentCameraRenderPosition();
	%this.oldPositionX = getWord(%currentPosition, 0);
	%this.oldPositionY = getWord(%currentPosition, 1);
}

function ParallaxObjectBehavior::onUpdate(%this)
{
    if ($LevelEditorActive)    
        %this.owner.setUpdateCallback(false);
    // Get the current position of the camera
    %currentPosition = scenewindow2d.getCurrentCameraRenderPosition();
	
	%this.currentPositionX = getword(%currentPosition, 0);
	%this.currentPositionY = getword(%currentPosition, 1);
	
	// Calculate how far the camera has moved since the last update
	%deltaX = %this.currentPositionX - %this.oldPositionX;
	%deltaY = %this.currentPositionY - %this.oldPositionY;
	
	// Store the current position for use in the next update
	%this.oldPositionX = %this.currentPositionX;
	%this.oldPositionY = %this.currentPositionY;
	
	// Calculate the scroll rate based on the delta and behavior specified speed
	%horizontalScrollRate = %deltaX * %this.horizontalScrollSpeed;
	%verticalScrollRate = %deltaY * %this.verticalScrollSpeed;
	
	// Don't bother unless the delta is below the threshold of 0.1
	if (mAbs(%deltaX) < 0.1)
        %this.owner.setScrollX(0);
	else
	    %this.owner.setScrollX(%horizontalScrollRate);
	
	// Don't bother unless the delta is below the threshold of 0.1    
	if (mAbs(%deltaY) < 0.1)
        %this.owner.setScrollY(0);
    else
        %this.owner.setScrollY(%verticalScrollRate);
}

function ParallaxObjectBehavior::getHorizontalScrollSpeed(%this)
{
    return %this.horizontalScrollSpeed;
}

function ParallaxObjectBehavior::setHorizontalScrollSpeed(%this, %horizontalScrollSpeed)
{
    %this.horizontalScrollSpeed = %horizontalScrollSpeed;
}

function ParallaxObjectBehavior::getTileable(%this)
{
    return %this.tileable;
}

function ParallaxObjectBehavior::setTileable(%this, %tileable)
{
    %this.tileable = %tileable;
}