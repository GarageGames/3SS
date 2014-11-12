//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function initializeInputSystem()
{
    if (!isObject(PhysicsInputTracker))
    {
        new ScriptObject(PhysicsInputTracker)
        {
            class = InputTracker;
            currentGesture = $Input::noGesture;
            maxTouches = 2;
            touch0 = "0 0";
            touch1 = "0 0";
            lastTouch0 = "0 0";
            lastTouch1 = "0 0";
        };
    }
    
    // Create a new ActionMap for input binding
    new ActionMap(gameActionMap);

    if ($platform $= "iOS")
    {
        gameActionMap.bind(touchdevice, touchdown, "gameTouchDown");
        gameActionMap.bind(touchdevice, touchmove, "gameTouchMove");
        gameActionMap.bind(touchdevice, touchup, "gameTouchUp");
    }
    else
    {
        gameActionMap.bind(mouse, zaxis, "zoom");
        gameActionMap.bind(mouse, button0, "gameTouchDown");
        gameActionMap.bind(mouse, xaxis, "gameTouchMove");
    }

    // Enable DirectInput for Windows development
    $enableDirectInput = true;
    activateDirectInput();
}

//------------------------------------------------------------------------------
// gameTouchDown
// Called whenever a finger touches the screen, if moveMap is pushed
// For this demo, it only gets used in the multiTouch.t2d level
//
// %touchIDs - List of unique ID numbers, one per-finger
// %touchesX - List of coordinates in the X axis
// %touchesY - List of coordinates in the Y axis
//------------------------------------------------------------------------------
function Game::processTouchDown(%touchID, %worldPos)
{
    if(%touchID == 0 || %touchID == 1)
        PhysicsInputTracker.trackTouch(%touchID, %worldPos);
}

//------------------------------------------------------------------------------
// gameTouchMove
// Called whenever a finger drags across the screen, if moveMap is pushed
// For this demo, it only gets used in the multiTouch.t2d level
//
// %touchIDs - List of unique ID numbers, one per-finger
// %touchesX - List of coordinates in the X axis
// %touchesY - List of coordinates in the Y axis
//------------------------------------------------------------------------------
function Game::processTouchMove(%touchID, %worldPos)
{
}

//------------------------------------------------------------------------------
// gameTouchUp
// Called whenever a finger is lifted from the screen, if moveMap is pushed
// For this demo, it only gets used in the multiTouch.t2d level
//
// %touchIDs - List of unique ID numbers, one per-finger
// %touchesX - List of coordinates in the X axis
// %touchesY - List of coordinates in the Y axis
//------------------------------------------------------------------------------
function Game::processTouchUp(%touchID, %worldPos)
{
    if(%touchID == 0 || %touchID == 1)
        PhysicsInputTracker.releaseTouches(%touchID);
}

function Game::processTouchDrag(%touchID, %worldPos)
{
    if(%touchID != 0 && %touchID != 1)
        return;
        
    PhysicsInputTracker.recordMove(%touchID, %worldPos);
        
    if (!sceneWindow2D.canMoveCamera)
        return;   
        
    // Only the first touch will be used for panning
    %horizontalPanAmount = PhysicsInputTracker.getHorizontalPanAmount(0);
    %verticalPanAmount = PhysicsInputTracker.getVerticalPanAmount(0);
    
    %currentCameraPosition = sceneWindow2D.getCurrentCameraPosition();
    
    %oldXPosition = getWord(%currentCameraPosition, 0);
    %oldYPosition = getWord(%currentCameraPosition, 1);
    
    %newXPosition = %oldXPosition - %horizontalPanAmount;
    %newYPosition = %oldYPosition - %verticalPanAmount;
    
    PhysicsInputTracker.updateRecordedMove( %touchID, %horizontalPanAmount, %verticalPanAmount );

    %newPosition = %newXPosition SPC %newYPosition;
    
    if (sceneWindow2D.getIsCameraMounted())
        sceneWindow2D.dismount();
        
    sceneWindow2D.setCurrentCameraPosition(%newPosition);
    sceneWindow2D.clampCurrentCameraViewLimit();
}

function zoom(%amount)
{
    %currentZoom = sceneWindow2D.getCurrentCameraZoom();
    
    if (%amount > 0)
        %newZoom = %currentZoom + 0.1;
    else
        %newZoom = %currentZoom - 0.1;
    
    %newZoom = mClamp(%newZoom, 1, 3);
    
    sceneWindow2D.setCurrentCameraZoom(%newZoom);
}