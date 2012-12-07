//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$CollisionShapePreviewTransformControlCount = 4;

function CollisionShapePreview::createTransformControls(%this)
{
    // Create the bounding box control
    if (isObject(%this.selectionBox))
        %this.selectionBox.delete(); 
    %this.selectionBox = new GuiMouseEventCtrl()
    {
        class="SelectionBoxMouseEventControl";           
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GUICollisionEditorSelectionBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="64 64";
        Visible="1";
        Active="0";
        lockMouse="0";
    };
    
    // Add control to the preview window
    CEPreviewWindow.add(%this.selectionBox);

    // List of x/y locations of the handle controls 
    // (1,1) = top right corner
    // (-1,-1) = bottom left corner
    %controlRelativeLocations = "-1 1 1 1 1 -1 -1 -1 0 1 1 0 0 -1 -1 0";
    
    // Create the transform handle controls
    for (%i = 0; %i < $CollisionShapePreviewTransformControlCount; %i++)
    {
        if (isObject(%this.transformHandle[%i]))
            %this.transformHandle[%i].delete();
            
        %x = getWord(%controlRelativeLocations, %i * 2);
        %y = getWord(%controlRelativeLocations, %i * 2 + 1);
        
        if (isObject(%this.transformHandleSpriteControl[%i]))
            %this.transformHandleSpriteControl[%i].delete();        
        
        %this.transformHandleSpriteControl[%i] = new GuiSpriteCtrl()
        {
            relativePositionX=getWord(%controlRelativeLocations, %i*2);
            relativePositionY=getWord(%controlRelativeLocations, %i*2 + 1);
            canSaveDynamicFields="1";
            isContainer="1";
            Profile="GUICollisionEditorSelectionBoxProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Extent="18 18";
            Visible="0";
            Active="1";
            Image="{3SSCollisionEditor}:transformToolHandleImageMap";
        };
        
        // Add control to the preview window
        CEPreviewWindow.add(%this.transformHandleSpriteControl[%i]);
        
        if(isObject(%this.transformHandleMouseEventControl[%i]))
            %this.transformHandleMouseEventControl[%i].delete();
        
        %this.transformHandleMouseEventControl[%i] = new GuiMouseEventCtrl()
        {
            class="TransformHandleMouseEventControl";
            relativePositionX=getWord(%controlRelativeLocations, %i*2);
            relativePositionY=getWord(%controlRelativeLocations, %i*2 + 1);            
            canSaveDynamicFields="1";
            isContainer="0";
            Profile="GuiTransparentProfile";
            HorizSizing="right";
            VertSizing="bottom";
            Position="0 0";
            Extent="18 18";
            Visible="1";
            Active="0";
            lockMouse="0";
        };
        
        // Add the mouse event control as a child of the sprite control
        %this.transformHandleSpriteControl[%i].add(%this.transformHandleMouseEventControl[%i]);
    }
    
    %this.setTransformControlsVisibility(false, false);
}

function CollisionShapePreview::setTransformControlsVisibility(%this, %baseVisible, %subControlsVisible)
{
    %this.selectionBox.setVisible(%baseVisible);    
    
    for (%i = 0; %i < $CollisionShapePreviewTransformControlCount; %i++)
    {
        %this.transformHandleSpriteControl[%i].setVisible(%subControlsVisible);  
        %this.transformHandleMouseEventControl[%i].setVisible(%subControlsVisible);  
    }
}

function CollisionShapePreview::updateTransformControls(%this, %boundingBox)
{
    %minX = getWord(%boundingBox, 0);
    %minY = getWord(%boundingBox, 1);
    %maxX = getWord(%boundingBox, 2);
    %maxY = getWord(%boundingBox, 3); 
    
    %sizeX = %maxX - %minX; 
    %sizeY = %maxY - %minY; 
    %centerX = (%minX + %maxX)/2; 
    %centerY = (%minY + %maxY)/2; 
    
    // Update box
    %windowPoint = %this.getWindowPoint(%minX, %maxY);
    %windowPoint2 = %this.getWindowPoint(%maxX, %minY);
    %this.selectionBox.setPosition(%windowPoint.x, %windowPoint.y);
    %this.selectionBox.setExtent(%windowPoint2.x - %windowPoint.x + 1, %windowPoint2.y - %windowPoint.y + 1);
    
    // Update handles
    for (%i = 0; %i < $CollisionShapePreviewTransformControlCount; %i++)
    {
        %control = %this.transformHandleSpriteControl[%i];
        %relativePositionX = %control.relativePositionX;
        %relativePositionY = %control.relativePositionY;
        
        // Calculate the position of the transform handle
        %positionX = %centerX + (%sizeX / 2) * %relativePositionX;
        %positionY = %centerY + (%sizeY / 2) * %relativePositionY;
        
        %windowPoint = %this.getWindowPoint(%positionX, %positionY);
        %control.setPosition(%windowPoint.x  - (%control.extent.x / 2), %windowPoint.y  - (%control.extent.y / 2));
    }
}

function TransformHandleMouseEventControl::onMouseEnter(%this, %modifier, %mousePoint, %mouseClickCount)
{
    %spriteControl = %this.getParent();
    
    %spriteControl.Image = "{3SSCollisionEditor}:transformToolHandle_hImageMap";
}

function TransformHandleMouseEventControl::onMouseLeave(%this, %modifier, %mousePoint, %mouseClickCount)
{
    %spriteControl = %this.getParent();
    
    %spriteControl.Image = "{3SSCollisionEditor}:transformToolHandleImageMap";
}

function TransformHandleMouseEventControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CEPreviewWindow.lockedControl = %this;
    
    %spriteControl = %this.getParent();
    
    %spriteControl.Image = "{3SSCollisionEditor}:transformToolHandle_dImageMap";
    
    // Save the drag start (world) point for use in scaling calculations
    %this.dragStartPoint = CEPreviewWindow.globalPointToWorldPoint(%mousePoint);
    %this.dragStartShapePosition = CEPreviewWindow.selectedShape.getPosition();
    %this.dragStartShapeSize = CEPreviewWindow.selectedShape.getSize();
}

function TransformHandleMouseEventControl::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CEPreviewWindow.lockedControl = "";
    
    %spriteControl = %this.getParent();
    
    %spriteControl.Image = "{3SSCollisionEditor}:transformToolHandle_hImageMap";
    
    %this.dragStartPoint = "";
}

function TransformHandleMouseEventControl::onMouseDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (CEPreviewWindow.lockedControl != %this)
    {
        CEPreviewWindow.lockedControl.onMouseDragged(%modifier, %mousePoint, %mouseClickCount);
        return;    
    } 
    
    // Set Scale-from-center flag if left or right Alt is held
    %scaleFromCenter = ((%modifier & 16) != 0) || ((%modifier & 32) != 0);
    
    // Set Preserve-aspect flag if left or right Shift is held
    %preserveAspect = ((%modifier & 1) != 0) || ((%modifier & 2) != 0);

    // Convert global mouse point to world coordinates
    %worldPoint = CEPreviewWindow.globalPointToWorldPoint(%mousePoint); 
    
    // Calculate the vector between the starting drag point and the current drag point
    %dragVector = Vector2Sub(%worldPoint, %this.dragStartPoint);

    // Scale the currently selected shape
    %this.scaleShape(CEPreviewWindow.selectedShape, %dragVector, %scaleFromCenter, %preserveAspect);
    
    //%position = CEPreviewWindow.selectedShape.getPosition();
    //%size = CEPreviewWindow.selectedShape.getSize();
    //%box = %position.x - %size.x/2 SPC %position.y - %size.y/2 SPC %position.x + %size.x/2 SPC %position.y + %size.y/2;
    %box = CEPreviewWindow.getBoundingBoxShape(CEPreviewWindow.selectedShape);    
    CEPreviewWindow.updateTransformControls(%box);
}

function TransformHandleMouseEventControl::scaleShape(%this, %shape, %scaleVector, %scaleFromCenter, %preserveAspect)
{
    // This code assumes that the shape is centered around the origin in local space
    // It also assumes that size is the distance from the center to one of the extents.
    
    %handlePositionX = %this.relativePositionX;
    %handlePositionY = %this.relativePositionY;
    
    // Calculate the extent change
    %extentDeltaX = %scaleVector.x * %handlePositionX;
    %extentDeltaY = %scaleVector.y * %handlePositionY;
    
    if (true)
    {
        %positionVector = %handlePositionX SPC %handlePositionY;
        %dotProduct = Vector2Dot(%scaleVector, Vector2Normalize(%positionVector));
        %extentDeltaVector = Vector2Scale(Vector2Normalize("1 1"), %dotProduct);
        %extentDeltaX = %extentDeltaVector.x;
        %extentDeltaY = %extentDeltaVector.y;
    }
    
    if (%scaleFromCenter)
    {
        %extentDeltaX *= 2;
        %extentDeltaY *= 2;
    }
    
    // Calculate scale values
    %scaleX = 1 + (%extentDeltaX / %this.dragStartShapeSize.x);
    %scaleY = 1 + (%extentDeltaY / %this.dragStartShapeSize.y);
    
    // Scale the shape
    %startSize = %this.dragStartShapeSize;
    %shape.setSize(%startSize.x * %scaleX, %startSize.y * %scaleY);  
   
    // Reposition the shape to maintain the anchor point
    if (%scaleFromCenter)
    {    
    }
    else
    {        
        %newSize = %shape.getSize();
        %delta = Vector2Sub(%newSize, %startSize);
        %adjustX = %handlePositionX * %delta.x * 0.5;
        %adjustY = %handlePositionY * %delta.y * 0.5;
        %shape.setPosition(%this.dragStartShapePosition.x + %adjustX, %this.dragStartShapePosition.y + %adjustY);
    }
}

function SelectionBoxMouseEventControl::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CEPreviewWindow.lockedControl = %this;
    
    // Save the drag start (world) point for use in transformation calculations
    %this.dragStartPoint = CEPreviewWindow.globalPointToWorldPoint(%mousePoint);

    if (isObject(CEPreviewWindow.selectedShape))
        %this.dragStartShapePosition = CEPreviewWindow.selectedShape.getPosition();
}

function SelectionBoxMouseEventControl::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CEPreviewWindow.lockedControl = "";
    
    %this.dragStartPoint = "";
}

function SelectionBoxMouseEventControl::onMouseDragged(%this, %modifier, %mousePoint, %mouseClickCount)
{
    // Ignore event if we are already dragging another control
    if (CEPreviewWindow.lockedControl != %this)
    {
        if( isObject(CEPreviewWindow.lockedControl) )
        {
            CEPreviewWindow.lockedControl.onMouseDragged(%modifier, %mousePoint, %mouseClickCount);
        }
        return;    
    } 
    
    // Convert global mouse point to world coordinates
    %worldPoint = CEPreviewWindow.globalPointToWorldPoint(%mousePoint); 
    
    // Calculate the vector between the starting drag point and the current drag point
    %dragVector = Vector2Sub(%worldPoint, %this.dragStartPoint);
    
    %this.translateShape(CEPreviewWindow.selectedShape, %dragVector);
    
    //%position = CEPreviewWindow.selectedShape.getPosition();
    //%size = CEPreviewWindow.selectedShape.getSize();
    //%box = %position.x - %size.x/2 SPC %position.y - %size.y/2 SPC %position.x + %size.x/2 SPC %position.y + %size.y/2;
    %box = CEPreviewWindow.getBoundingBoxShape(CEPreviewWindow.selectedShape);    
    CEPreviewWindow.updateTransformControls(%box);
}

function SelectionBoxMouseEventControl::translateShape(%this, %shape, %translateVector)
{
    if (!isObject(%shape))
        return;
    
    %newPosX = %this.dragStartShapePosition.x + %translateVector.x;
    %newPosY = %this.dragStartShapePosition.y + %translateVector.y;
    %shape.setPosition(%newPosX, %newPosY);
}

function CEPreviewWindow::globalPointToWorldPoint(%this, %globalPoint)
{
    %windowPosition = %this.getGlobalPosition();
    %windowPoint = Vector2Sub(%globalPoint, %windowPosition);
    %worldPoint = CEPreviewWindow.getWorldPoint(%windowPoint);
    return %worldPoint; 
}

