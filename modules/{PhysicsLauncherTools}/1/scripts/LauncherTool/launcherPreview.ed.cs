//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$LauncherPreview::ForkAreaWidthFraction = 0.7;

function Lt_PreviewWindow::onAdd(%this)
{
    if (!isObject(%this.scene))
    {
        %this.scene = new Scene()
        {
            cameraPosition = "0 0";
            cameraSize = "16 12";
            gravity = "0 0";
        };
        
        %this.setScene(%this.scene);
    }
}

function Lt_PreviewWindow::onRemove(%this)
{
    if (isObject(%this.scene))
    {
        %this.scene.delete();
        %this.scene = "";
    }
}

function Lt_PreviewWindow::onWake(%this)
{
   %this.onExtentChange(%this.position SPC %this.extent);
   
   initializeLauncherPreview(%this);
}

function Lt_PreviewWindow::onExtentChange(%this, %newDimensions)
{
    
}

function initializeLauncherPreview(%sceneWindow)
{
    %windowExtents = %sceneWindow.getWindowExtents();
    %windowWidth = getWord(%windowExtents, 2);
    %windowHeight = getWord(%windowExtents, 3);
    
    %newMinX = (-1 * %windowWidth * $PhysicsLauncherTools::MetersPerPixel) / 2; 
    %newMaxX = (%windowWidth * $PhysicsLauncherTools::MetersPerPixel) / 2; 
    %newMinY = (-1 * %windowHeight * $PhysicsLauncherTools::MetersPerPixel) / 2;
    %newMaxY = (%windowHeight * $PhysicsLauncherTools::MetersPerPixel) / 2; 

    %sceneWindow.setCurrentCameraArea(%newMinX, %newMinY, %newMaxX, %newMaxY);
}

function clearLauncherPreview(%sceneWindow)
{
    %sceneWindow.scene.clear();
}

function refreshLauncherPreview(%sceneWindow, %launcherSceneObjectGroup)
{
    // Clear the preview
    clearLauncherPreview(%sceneWindow);       
    
    // Fork
    refreshLauncherPreviewFork(%sceneWindow, %launcherSceneObjectGroup);
    
    // Launcher
    %scale = %sceneWindow.forkForeground.scale;
    refreshLauncherPreviewLauncherObject(%sceneWindow, %launcherSceneObjectGroup, %sceneWindow.forkForeground.getPosition(), %scale);
    
    //  Seat
    refreshLauncherPreviewSeat(%sceneWindow, %launcherSceneObjectGroup);
    
    // Rubberbands
    refreshLauncherPreviewRubberbands(%sceneWindow, %launcherSceneObjectGroup);
    
    //Debug: Collision Shapes
    //%sceneWindow.refreshCollisionShapes(%launcherSceneObjectGroup);
        
    
    
}

function refreshLauncherPreviewFork(%sceneWindow, %launcherSceneObjectGroup)
{
    // Get window dimensions
    %windowExtents = %sceneWindow.getWindowExtents();
    %windowWidth = getWord(%windowExtents, 2);
    %windowHeight = getWord(%windowExtents, 3);    
    
    // Calculate fork area in meters
    %forkAreaWidth = $LauncherPreview::ForkAreaWidthFraction * %windowWidth * $PhysicsLauncherTools::MetersPerPixel;
    %forkAreaHeight = %windowHeight * $PhysicsLauncherTools::MetersPerPixel;    

    // Get fork image objects
    %foregroundObject = %launcherSceneObjectGroup.findObjectByInternalName("SlingshotForegroundObject");
    %backgroundObject = %launcherSceneObjectGroup.findObjectByInternalName("SlingshotBackgroundObject");
    
    // Calculate scaling fraction used for both fork images
    %maxWidth = mGetMax(%foregroundObject.getWidth(), %backgroundObject.getWidth());
    %maxHeight = mGetMax(%foregroundObject.getHeight(), %backgroundObject.getHeight()); 

    %widthScale = 1;
    %heightScale = 1;    
    
    if (%maxWidth > %forkAreaWidth)
        %widthScale = %forkAreaWidth / %maxWidth;
    if (%maxHeight > %forkAreaHeight)
        %heightScale = %forkAreaHeight / %maxHeight;
    
    %forkScale = mGetMin(%widthScale, %heightScale);
    
    // Get fork center position
    %forkCenterX = %windowWidth * ((1 - $LauncherPreview::ForkAreaWidthFraction) / 2) * $PhysicsLauncherTools::MetersPerPixel;
    %forkCenterY = 0;
    
    // Create foreground and background sprites
    if (!isObject(%sceneWindow.forkForeground))
        %sceneWindow.forkForeground = new Sprite();
    %sceneWindow.forkForeground.scale = %forkScale;
    %sceneWindow.forkForeground.setAsset(%foregroundObject.getAsset());
    %sceneWindow.forkForeground.setFrame(SlingshotLauncherBuilder::getForkForegroundImageFrame(%launcherSceneObjectGroup));
    %sceneWindow.forkForeground.setSize(%foregroundObject.getWidth() * %forkScale, %foregroundObject.getHeight() * %forkScale);
    %sceneWindow.forkForeground.setPosition(%forkCenterX, %forkCenterY);
    %sceneWindow.forkForeground.setSceneLayer($SlingshotLauncherBuilder::ForkForegroundObjectLayer);
    %sceneWindow.scene.add(%sceneWindow.forkForeground);
    
    if (!isObject(%sceneWindow.forkBackground))
        %sceneWindow.forkBackground = new Sprite();
    %sceneWindow.forkBackground.scale = %forkScale;
    %sceneWindow.forkBackground.setAsset(%backgroundObject.getAsset());
    %sceneWindow.forkBackground.setFrame(SlingshotLauncherBuilder::getForkBackgroundImageFrame(%launcherSceneObjectGroup));
    %sceneWindow.forkBackground.setSize(%backgroundObject.getWidth() * %forkScale, %backgroundObject.getHeight() * %forkScale);
    %sceneWindow.forkBackground.setPosition(%forkCenterX, %forkCenterY);
    %sceneWindow.forkBackground.setSceneLayer($SlingshotLauncherBuilder::ForkBackgroundObjectLayer);    
    %sceneWindow.scene.add(%sceneWindow.forkBackground);
}

function refreshLauncherPreviewLauncherObject(%sceneWindow, %launcherSceneObjectGroup, %position, %scale)
{
    // Get launcher object
    %launcherObject = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::LauncherObjectInternalName);
    
    if (!isObject(%sceneWindow.launcher))
        %sceneWindow.launcher = new SceneObject();
    %sceneWindow.launcher.scale = %scale;
    %sceneWindow.launcher.setPosition(%position);
    %sceneWindow.launcher.setSize(Vector2Scale(%launcherObject.getSize(), %scale));

    %sceneWindow.launcher.clearCollisionShapes();

    for (%i = 0; %i < %launcherObject.getCollisionShapeCount(); %i++)
    {
        // Get the collision shape format string
        %shapeString = %launcherObject.formatCollisionShape(%i);
        
        // Create a new collision shape on the proxy from the format string
        %shapeIndex = %sceneWindow.launcher.parseCollisionShape(%shapeString);
        %sceneWindow.launcher.setCollisionShapeIsSensor(%shapeIndex, true);
        
        if (%shapeIndex == -1)
        {
            warn("Lt_PreviewWindow::refreshLauncher -- failed to set a collision shape on the launcher object.");
        }
    } 
    
    //%sceneWindow.launcher.setDebugOn(5);   
    
    %sceneWindow.scene.add(%sceneWindow.launcher);
}

function refreshLauncherPreviewSeat(%sceneWindow, %launcherSceneObjectGroup)
{
    // Get window dimensions
    %windowExtents = %sceneWindow.getWindowExtents();
    %windowWidth = getWord(%windowExtents, 2);
    %windowHeight = getWord(%windowExtents, 3); 
    
    // Calculate seat area in meters
    %seatAreaWidthFraction = 1 - $LauncherPreview::ForkAreaWidthFraction;
    %seatAreaWidth = %seatAreaWidthFraction * %windowWidth * $PhysicsLauncherTools::MetersPerPixel;
    %seatAreaHeight = %windowHeight * $PhysicsLauncherTools::MetersPerPixel;   
    
    // Get seat object
    %seatObject = %launcherSceneObjectGroup.findObjectByInternalName("SeatObject");
    
    // Calculate seat scaling fraction
    %seatWidth = %seatObject.getWidth();
    %seatHeight = %seatObject.getHeight();    
    
    %widthScale = 1;
    %heightScale = 1;
      
    if (%seatWidth > %seatAreaWidth)
        %widthScale = %seatAreaWidth / %seatWidth;
    if (%seatHeight > %seatAreaHeight)
        %heightScale = %seatAreaHeight / %seatHeight;
    
    %seatScale = mGetMin(%widthScale, %heightScale);
    
    // Get seat center positions
    %seatCenterX = %windowWidth * ((%seatAreaWidthFraction - 1) / 2) * $PhysicsLauncherTools::MetersPerPixel;
    %seatCenterY = 0;
    
    // Create seat sprite
    if (!isObject(%sceneWindow.seat))
        %sceneWindow.seat = new Sprite();
    %sceneWindow.seat.scale = %seatScale;
    %sceneWindow.seat.setAsset(%seatObject.getAsset());
    %sceneWindow.seat.setFrame(SlingshotLauncherBuilder::getSeatImageFrame(%launcherSceneObjectGroup));
    %sceneWindow.seat.setSize(%seatObject.getWidth() * %seatScale, %seatObject.getHeight() * %seatScale);
    %sceneWindow.seat.setPosition(%seatCenterX, %seatCenterY);
    %sceneWindow.seat.setSceneLayer($SlingshotLauncherBuilder::SeatObjectLayer);
    %sceneWindow.scene.add(%sceneWindow.seat);
}

function refreshLauncherPreviewRubberbands(%sceneWindow, %launcherSceneObjectGroup)
{
    // Get rubberband objects
    %bandObject0 = %launcherSceneObjectGroup.findObjectByInternalName("BandObject0");
    %bandObject1 = %launcherSceneObjectGroup.findObjectByInternalName("BandObject1");
    
    // Get attachment points
    %attachmentPoints0 = SlingshotLauncherBuilder::getBandAttachmentPoints(%launcherSceneObjectGroup, 0);
    %attachmentPoints1 = SlingshotLauncherBuilder::getBandAttachmentPoints(%launcherSceneObjectGroup, 1);
    
    // Scale the attachment points by their attached objects' scales
    %launcherScale = %sceneWindow.launcher.scale;
    %seatScale = %sceneWindow.seat.scale;
    
    %band0StartX = getWord(%attachmentPoints0, 0);
    %band0StartY = getWord(%attachmentPoints0, 1);
    %band0EndX = getWord(%attachmentPoints0, 2);
    %band0EndY = getWord(%attachmentPoints0, 3);
    
    %band1StartX = getWord(%attachmentPoints1, 0);
    %band1StartY = getWord(%attachmentPoints1, 1);
    %band1EndX = getWord(%attachmentPoints1, 2);
    %band1EndY = getWord(%attachmentPoints1, 3);
    
    // Create band sprites
    if (!isObject(%sceneWindow.band0))
        %sceneWindow.band0 = new Sprite();
    %sceneWindow.band0.setAsset(%bandObject0.getAsset());
    %sceneWindow.band0.setFrame(SlingshotLauncherBuilder::getBandImageFrame(%launcherSceneObjectGroup, 0));
    %sceneWindow.band0.setSize(%bandObject0.getWidth() * %launcherScale, %bandObject0.getHeight() * %launcherScale);
    %sceneWindow.band0.setPosition(0, 0);
    %sceneWindow.band0.setSceneLayer($SlingshotLauncherBuilder::Band0ObjectLayer);
    %sceneWindow.scene.add(%sceneWindow.band0);
    
    %scaleBetweenPointsBehavior0 = ScaleBetweenPointsBehavior.createInstance();
    %scaleBetweenPointsBehavior0.setThicknessScalingRatio(%bandObject0.callOnBehaviors("getThicknessScalingRatio"));
    %sceneWindow.band0.addBehavior(%scaleBetweenPointsBehavior0);
    %scaleBetweenPointsBehavior0.setAttachmentPoints(%band0StartX SPC %band0StartY, %band0EndX SPC %band0EndY);
    %scaleBetweenPointsBehavior0.attach(%sceneWindow.launcher, %sceneWindow.seat);
    
    if (!isObject(%sceneWindow.band1))
        %sceneWindow.band1 = new Sprite();
    %sceneWindow.band1.setAsset(%bandObject1.getAsset());
    %sceneWindow.band1.setFrame(SlingshotLauncherBuilder::getBandImageFrame(%launcherSceneObjectGroup, 1));
    %sceneWindow.band1.setSize(%bandObject1.getWidth() * %launcherScale, %bandObject1.getHeight() * %launcherScale);
    %sceneWindow.band1.setPosition(0, 0);
    %sceneWindow.band1.setSceneLayer($SlingshotLauncherBuilder::Band1ObjectLayer);
    %sceneWindow.scene.add(%sceneWindow.band1);
    
    %scaleBetweenPointsBehavior1 = ScaleBetweenPointsBehavior.createInstance();
    %scaleBetweenPointsBehavior1.setThicknessScalingRatio(%bandObject1.callOnBehaviors("getThicknessScalingRatio"));
    %sceneWindow.band1.addBehavior(%scaleBetweenPointsBehavior1);
    %scaleBetweenPointsBehavior1.setAttachmentPoints(%band1StartX SPC %band1StartY, %band1EndX SPC %band1EndY);
    %scaleBetweenPointsBehavior1.attach(%sceneWindow.launcher, %sceneWindow.seat);
    
    // Update band stretching
    updateBandStretch(%sceneWindow.band0);
    updateBandStretch(%sceneWindow.band1);
    
    //
    // Controls
    //

    // Only update control for the main preview window
    if (%sceneWindow !$= Lt_PreviewWindow)
        return;

    // Create sprites
    %band0StartPositionX = %sceneWindow.launcher.getPositionX() + %band0StartX * (%sceneWindow.launcher.getWidth() / 2);
    %band0StartPositionY = %sceneWindow.launcher.getPositionY() + %band0StartY * (%sceneWindow.launcher.getHeight() / 2);
    %windowPoint = %sceneWindow.getWindowPoint(%band0StartPositionX, %band0StartPositionY); 
    Lt_BandForegroundStartControl.setPosition(%windowPoint.x - Lt_BandForegroundStartControl.getExtent().x/2, %windowPoint.y - (Lt_BandForegroundStartControl.getExtent().y/2));
    
    %band0EndPositionX = %sceneWindow.seat.getPositionX() + %band0EndX * (%sceneWindow.seat.getWidth() / 2);
    %band0EndPositionY = %sceneWindow.seat.getPositionY() + %band0EndY * (%sceneWindow.seat.getHeight() / 2);
    %windowPoint = %sceneWindow.getWindowPoint(%band0EndPositionX, %band0EndPositionY);    
    Lt_BandForegroundEndControl.setPosition(%windowPoint.x - Lt_BandForegroundEndControl.getExtent().x/2, %windowPoint.y - (Lt_BandForegroundEndControl.getExtent().y/2));
    
    %band1StartPositionX = %sceneWindow.launcher.getPositionX() + %band1StartX * (%sceneWindow.launcher.getWidth() / 2);
    %band1StartPositionY = %sceneWindow.launcher.getPositionY() + %band1StartY * (%sceneWindow.launcher.getHeight() / 2);
    %windowPoint = %sceneWindow.getWindowPoint(%band1StartPositionX, %band1StartPositionY);    
    Lt_BandBackgroundStartControl.setPosition(%windowPoint.x - Lt_BandBackgroundStartControl.getExtent().x/2, %windowPoint.y - (Lt_BandBackgroundStartControl.getExtent().y/2));
    
    %band1EndPositionX = %sceneWindow.seat.getPositionX() + %band1EndX * (%sceneWindow.seat.getWidth() / 2);
    %band1EndPositionY = %sceneWindow.seat.getPositionY() + %band1EndY * (%sceneWindow.seat.getHeight() / 2);
    %windowPoint = %sceneWindow.getWindowPoint(%band1EndPositionX, %band1EndPositionY);    
    Lt_BandBackgroundEndControl.setPosition(%windowPoint.x - Lt_BandBackgroundEndControl.getExtent().x/2, %windowPoint.y - (Lt_BandBackgroundEndControl.getExtent().y/2));
}

function Lt_PreviewWindow::refreshCollisionShapes(%this, %launcherSceneObjectGroup)
{
    %collisionObject = %launcherSceneObjectGroup.findObjectByInternalName($SlingshotLauncherBuilder::CollisionObjectInternalName);
    if (!isObject(%this.collisionObject))
        %this.collisionObject = new SceneObject();
    %this.collisionObject.scale = %this.launcher.scale;
    %this.collisionObject.setPosition(%this.launcher.getPosition());
    %this.collisionObject.setSize(Vector2Scale(%collisionObject.getSize(), %this.collisionObject.scale));

    %this.collisionObject.clearCollisionShapes();

    for (%i = 0; %i < %collisionObject.getCollisionShapeCount(); %i++)
    {
        // Get the collision shape format string
        %shapeString = %collisionObject.formatCollisionShape(%i);
        
        // Create a new collision shape on the proxy from the format string
        %shapeIndex = %this.collisionObject.parseCollisionShape(%shapeString);
        %this.collisionObject.setCollisionShapeIsSensor(%shapeIndex, true);
        
        if (%shapeIndex == -1)
        {
            warn("Lt_PreviewWindow::refreshLauncher -- failed to set a collision shape on the launcher object.");
        }
    } 
    
    // Add bounding box shape
    %shape = %this.collisionObject.createPolygonBoxCollisionShape(%this.collisionObject.getWidth(), %this.collisionObject.getHeight());
    %this.collisionObject.setCollisionShapeIsSensor(%shape, true);
    
    %this.collisionObject.setDebugOn("DEBUG_COLLISION_SHAPES"); 
    
    %this.scene.add(%this.collisionObject);  
}

function Lt_PreviewWindow::saveBandPositions(%this, %launcherSceneObjectGroup)
{
    %startScale = 1;
    %endScale = 1;

    %newStartPoint0 = Vector2Scale(Lt_PreviewWindow.band0.callOnBehaviors("getAttachmentStartPoint"), %startScale);
    %newStartPoint1 = Vector2Scale(Lt_PreviewWindow.band1.callOnBehaviors("getAttachmentStartPoint"), %startScale);
    %newEndPoint0 = Vector2Scale(Lt_PreviewWindow.band0.callOnBehaviors("getAttachmentEndPoint"), %endScale);
    %newEndPoint1 = Vector2Scale(Lt_PreviewWindow.band1.callOnBehaviors("getAttachmentEndPoint"), %endScale);
    
    // Error check
    if (%newStartPoint0 $= "ERR_CALL_NOT_HANDLED" ||
        %newStartPoint1 $= "ERR_CALL_NOT_HANDLED" ||
        %newEndPoint0 $= "ERR_CALL_NOT_HANDLED" ||
        %newEndPoint1 $= "ERR_CALL_NOT_HANDLED")
    {
        warn("Lt_PreviewWindow::saveBandPositions -- Error getting attachment points from behaviors");
        return;
    }     
    
    SlingshotLauncherBuilder::setBandAttachmentPoints(%launcherSceneObjectGroup, 0, %newStartPoint0, %newEndPoint0);
    SlingshotLauncherBuilder::setBandAttachmentPoints(%launcherSceneObjectGroup, 1, %newStartPoint1, %newEndPoint1);

    // Update the touch target shape
    SlingshotLauncherBuilder::updateTouchTargetShape(%launcherSceneObjectGroup);   
    
    // Refresh preview
    refreshLauncherPreviewLauncherObject(%this, %launcherSceneObjectGroup, %this.forkForeground.getPosition(), %this.forkForeground.scale);
    
    // Refresh Scroll bar preview
    LauncherTool.refreshScrollBarPreview();
}

//
// BandForegroundStart
//

function Lt_BandForegroundStartMouseEventControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{  
    Lt_PreviewWindow.lockedControl = %this;
}

function Lt_BandForegroundStartMouseEventControl::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
        return;  
        
    Lt_PreviewWindow.lockedControl = "";
    
    // Save out the band info
    Lt_PreviewWindow.saveBandPositions(LauncherTool.currentObject);
}

function Lt_BandForegroundStartMouseEventControl::onMouseDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
    {
        Lt_PreviewWindow.lockedControl.onMouseDragged(%modifier, %mousePoint, %mouseClickCount);
        return;    
    } 
        
    startControlDragged(Lt_BandForegroundStartControl, Lt_PreviewWindow.band0, %mousePoint, true);  
}

//
// BandForegroundEnd
//

function Lt_BandForegroundEndMouseEventControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{  
    Lt_PreviewWindow.lockedControl = %this;    
}

function Lt_BandForegroundEndMouseEventControl::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
        return;  
        
    Lt_PreviewWindow.lockedControl = "";
    
    // Save out the band info
    Lt_PreviewWindow.saveBandPositions(LauncherTool.currentObject);
}

function Lt_BandForegroundEndMouseEventControl::onMouseDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
    {
        Lt_PreviewWindow.lockedControl.onMouseDragged(%modifier, %mousePoint, %mouseClickCount);
        return;    
    }
        
    endControlDragged(Lt_BandForegroundEndControl, Lt_PreviewWindow.band0, %mousePoint, false);    
}

//
// BandBackgroundStart
//

function Lt_BandBackgroundStartMouseEventControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{ 
    Lt_PreviewWindow.lockedControl = %this;    
}

function Lt_BandBackgroundStartMouseEventControl::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
        return;  
        
    Lt_PreviewWindow.lockedControl = "";
    
    // Save out the band info
    Lt_PreviewWindow.saveBandPositions(LauncherTool.currentObject);
}

function Lt_BandBackgroundStartMouseEventControl::onMouseDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
    {
        Lt_PreviewWindow.lockedControl.onMouseDragged(%modifier, %mousePoint, %mouseClickCount);
        return;    
    }
        
    startControlDragged(Lt_BandBackgroundStartControl, Lt_PreviewWindow.band1, %mousePoint, true);    
}

//
// BandBackgroundEnd
//

function Lt_BandBackgroundEndMouseEventControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    Lt_PreviewWindow.lockedControl = %this;    
}

function Lt_BandBackgroundEndMouseEventControl::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
        return;      
    
    Lt_PreviewWindow.lockedControl = "";
    
    // Save out the band info
    Lt_PreviewWindow.saveBandPositions(LauncherTool.currentObject);
}

function Lt_BandBackgroundEndMouseEventControl::onMouseDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (Lt_PreviewWindow.lockedControl != %this)
    {
        Lt_PreviewWindow.lockedControl.onMouseDragged(%modifier, %mousePoint, %mouseClickCount);
        return;    
    }
    
    endControlDragged(Lt_BandBackgroundEndControl, Lt_PreviewWindow.band1, %mousePoint, false);    
}

function updateBandStretch(%band)
{
    // Calculate the thickness scale (just use a constant value for distance in the tool)
    %distance = 2;
    %thicknessScale = %distance / $SlingshotLauncherBuilder::BaseDistanceScale;
    
    // Call update on the scaleBetweenPointsBehavior
    %band.callOnBehaviors("update", %thicknessScale);
}

function startControlDragged(%control, %band, %mousePoint)
{
    %windowPosition = Lt_PreviewWindow.getGlobalPosition();
    %position = Vector2Sub(%mousePoint, %windowPosition);
    
    // Check to make sure the position is within the launcher area bounds
    %topLeft = (1 - $LauncherPreview::ForkAreaWidthFraction) * Lt_PreviewWindow.getExtent().x SPC "0";
    %bottomRight = Lt_PreviewWindow.getExtent();
    %position = restrictControlToBoundary(%control, %position, %topLeft, %bottomRight);
    
    // Set the position of the control
    %control.setPosition(%position.x - %control.getExtent().x/2, %position.y - %control.getExtent().y/2);

    // Calculate attachment point
    %launcherScale = Lt_PreviewWindow.launcher.scale;
    %launcherPosition = Lt_PreviewWindow.launcher.getPosition();
    %launcherWorldPosition = Lt_PreviewWindow.getWorldPoint(%position);
    %delta = Vector2Sub(%launcherWorldPosition, %launcherPosition);
    %attachmentPointX = %delta.x / (Lt_PreviewWindow.launcher.getWidth()/2); 
    %attachmentPointY = %delta.y / (Lt_PreviewWindow.launcher.getHeight()/2);
    %attachmentPoint = %attachmentPointX SPC %attachmentPointY;

    // Set attachment point for band
    %band.callOnBehaviors("setAttachmentStartPoint", %attachmentPoint);

    // Update the band stretching
    updateBandStretch(%band);
}

function endControlDragged(%control, %band, %mousePoint)
{
    %windowPosition = Lt_PreviewWindow.getGlobalPosition();
    %position = Vector2Sub(%mousePoint, %windowPosition);
    
    // Check to make sure the position is within the launcher area bounds
    %topLeft = "0 0";
    %bottomRight = (1 - $LauncherPreview::ForkAreaWidthFraction) * Lt_PreviewWindow.getExtent().x  SPC Lt_PreviewWindow.getExtent().y;
    %position = restrictControlToBoundary(%control, %position, %topLeft, %bottomRight);
    
    // Set the position of the control
    %control.setPosition(%position.x - %control.getExtent().x/2, %position.y - %control.getExtent().y/2);

    // Calculate attachment point
    %seatScale = Lt_PreviewWindow.seat.scale;
    %seatPosition = Lt_PreviewWindow.seat.getPosition();
    %seatWorldPosition = Lt_PreviewWindow.getWorldPoint(%position);
    %delta = Vector2Sub(%seatWorldPosition, %seatPosition);
    %attachmentPointX = %delta.x / (Lt_PreviewWindow.seat.getWidth()/2); 
    %attachmentPointY = %delta.y / (Lt_PreviewWindow.seat.getHeight()/2);
    %attachmentPoint = %attachmentPointX SPC %attachmentPointY;

    // Set attachment point for band
    %band.callOnBehaviors("setAttachmentEndPoint", %attachmentPoint);

    // Update the band stretching
    updateBandStretch(%band);
}

function restrictControlToBoundary(%control, %position, %topLeft, %bottomRight)
{
    %halfControlWidth = %control.getExtent().x / 2;
    %halfControlHeight = %control.getExtent().y / 2;
    
    %minX = %topLeft.x + %halfControlWidth;
    %maxX = %bottomRight.x - %halfControlWidth;
    %minY = %topLeft.y + %halfControlHeight;
    %maxY = %bottomRight.y - %halfControlHeight;
    
    %positionX = %position.x;
    %positionY = %position.y;
    
    // X-boundaries
    if (%positionX > %maxX)
        %positionX = %maxX;
    if (%positionX < %minX)
        %positionX = %minX;
    
    // Y-boundaries
    if (%positionY > %maxY)
        %positionY = %maxY;
    if (%positionY < %minY)
        %positionY = %minY;
        
    return %positionX SPC %positionY;
}

// Handles Up events that occur off the control being dragged so they
// are still passed to that control
function Lt_PreviewWindow::onTouchUp(%this, %touchID, %worldPos)
{
    if (!isObject(%this.lockedControl))
        return;  

    %windowPoint = %this.getWindowPoint(%worldPos);
    
    %globalPoint = Vector2Add(%windowPoint, %this.getGlobalPosition());
    
    %modifier = 0;
    %mouseClickCount = 1;
    %this.lockedControl.onMouseUp(%modifier, %globalPoint, %mouseClickCount);     
}

// Handles touch events that leave the control being dragged so they
// are still passed to that control
function Lt_PreviewWindow::onTouchDragged(%this, %touchID, %worldPos)
{
    if (!isObject(%this.lockedControl))
        return;  

    %windowPoint = %this.getWindowPoint(%worldPos);
    
    %globalPoint = Vector2Add(%windowPoint, %this.getGlobalPosition());
    
    %modifier = 0;
    %mouseClickCount = 1;
    %this.lockedControl.onMouseDragged(%modifier, %globalPoint, %mouseClickCount);     
}

