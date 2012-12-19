//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$CollisionEditorPreviewShapeFillColorSelected = "0.5 1 1 0.5";
$CollisionEditorPreviewShapeLineColorSelected = "0.5 1 1 1";

$CollisionEditorPreviewShapeFillColorUnselected = "0.5 1 1 0.1";
$CollisionEditorPreviewShapeLineColorUnselected = "0.5 1 1 1";

function CEPreviewWindow::onAdd(%this)
{
    %this.initialize();
}

function CEPreviewWindow::onRemove(%this)
{
    %this.destroy();
}

function CollisionShapePreview::initialize(%this)
{
    %this.selectedShape = "";
    
    %this.shapeList = new SimSet();
    
    if (!isObject(%this.scene))
    {
        %this.scene = new Scene()
        {
            cameraPosition = "0 0";
            cameraSize = "16 12";
            gravity = "0 0";
            layerSortMode0="Batch";
            layerSortMode1="Batch";
            layerSortMode2="Batch";
            layerSortMode3="Batch";
            layerSortMode4="Batch";
            layerSortMode5="Batch";
            layerSortMode6="Batch";
            layerSortMode7="Batch";
            layerSortMode8="Batch";
            layerSortMode9="Batch";
        };
        
        %this.setScene(%this.scene);
    }
    
    //%this.sceneEditor = new LevelBuilderSceneEdit(CollisionEditorSceneEdit);
    %this.objectWasModified = false;

    // Initialize the transform controls
    %this.createTransformControls();
    
    //%this.scene.setDebugOn(5);
}

function CollisionShapePreview::getBoundingBoxShape(%this, %shapeVector)
{
    if (!isObject(%shapeVector))
        return "";
        
    %minX = 0;
    %maxX = 0;
    %minY = 0;
    %maxY = 0;    
    
    // Handle circle
    if (%shapeVector.isCircle == true)
    {
        %minX = %shapeVector.Position.x - %shapeVector.circleRadius;
        %maxX = %shapeVector.Position.x + %shapeVector.circleRadius;
        %minY = %shapeVector.Position.y - %shapeVector.circleRadius;
        %maxY = %shapeVector.Position.y + %shapeVector.circleRadius;
        return %minX SPC %minY SPC %maxX SPC %maxY;
    }
    
    // Handle polygon
    
    %points = %shapeVector.getWorldPoly();
    
    return %this.getBoundingBoxPoints(%points);
}

function CollisionShapePreview::getBoundingBoxPoints(%this, %points)
{
    %firstPoint = true;
    for (%i = 0; %i < getWordCount(%points); %i += 2)
    {
        %pointX = getWord(%points, %i);
        %pointY = getWord(%points, %i + 1);
        if (%firstPoint)
        {
            %minX = %pointX;
            %maxX = %pointX;
            %minY = %pointY;
            %maxY = %pointY;
            %firstPoint = false;
        }
        else
        {
            %minX = mGetMin(%pointX, %minX);
            %maxX = mGetMax(%pointX, %maxX);
            %minY = mGetMin(%pointY, %minY);
            %maxY = mGetMax(%pointY, %maxY);
        }
    }
    
    return %minX SPC %minY SPC %maxX SPC %maxY;
}

function CollisionShapePreview::transformPoints(%this, %points, %positionTransform, %scaleTransform)
{
    %pointCount = getWordCount(%points)/2;    

    for (%i = 0; %i < %pointCount; %i++)
    {
        %pointIndex = %i * 2;
        %currentPoint = getWords(%points, %pointIndex, %pointIndex+1);
  
        %currentPoint = Vector2Sub(%currentPoint, %positionTransform);
        %currentPoint.x /= %scaleTransform.x;
        %currentPoint.y /= %scaleTransform.y;
        
        %points = setWord(%points, %pointIndex, %currentPoint.x);
        %points = setWord(%points, %pointIndex + 1, %currentPoint.y);
    }
    
    return %points;
}

function CollisionShapePreview::selectShape(%this, %shape)
{
    %this.selectedShape.fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
    %this.selectedShape = %shape;
    %this.selectedShape.fillColor = $CollisionEditorPreviewShapeFillColorSelected;
        
    %this.setTransformControlsVisibility(true, false);
    %box = %this.getBoundingBoxShape(CEPreviewWindow.selectedShape);
    %this.updateTransformControls(%box);
}

function CollisionShapePreview::deselectShape(%this, %shape)
{
    %this.selectedShape.fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
    %this.selectedShape = "";
}

function CollisionShapePreview::createSquareShape(%this)
{
    %squareShape = new ShapeVector()
    {
        GravityScale = 0;
        UpdateCallback = 0;
        SceneLayer = 0;
        LineColor = $CollisionEditorPreviewShapeLineColorUnselected;
        fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
        FillMode = "1";
        PolyList = $CollisionEditor::squareShapePoints;
        CollisionSuppress = true;
    };
    
    %squareShape.createPolygonCollisionShape(%squareShape.PolyList);
    
    return %squareShape;
}

function CollisionShapePreview::createTriangleShape(%this)
{
    %triangleShape = new ShapeVector()
    {
        GravityScale = 0;
        UpdateCallback = 0;
        SceneLayer = 0;
        LineColor = $CollisionEditorPreviewShapeLineColorUnselected;
        fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
        FillMode = "1";
        PolyList = $CollisionEditor::triangleShapePoints;
        CollisionSuppress = true;
    };
    
    %triangleShape.createPolygonCollisionShape(%triangleShape.PolyList);
    
    return %triangleShape;
}

function CollisionShapePreview::createCircleShape(%this)
{
    %circleShape = new ShapeVector()
    {
        size = $CollisionEditor::circleShapeRadius *2 SPC $CollisionEditor::circleShapeRadius * 2;
        GravityScale = 0;
        UpdateCallback = 0;
        SceneLayer = 0;
        isCircle = true;
        LineColor = $CollisionEditorPreviewShapeLineColorUnselected;
        fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
        FillMode = "1";
        circleRadius = $CollisionEditor::circleShapeRadius;
        PolyList = "0 0 0 0 0 0";
        CollisionSuppress = true;
    };
    
    %circleShape.createCircleCollisionShape(%circleShape.circleRadius);
    
    return %circleShape;
}

function CollisionShapePreview::destroy(%this)
{
    /*if (isObject(%this.selectionBox))
        %this.selectionBox.delete(); */
        
    if (isObject(%this.circleShape))
        %this.circleShape.delete();
        
    if (isObject(%this.triangleShape))
        %this.triangleShape.delete();
        
    if (isObject(%this.squareShape))
        %this.squareShape.delete();
    
    if (!isObject(%this.scene))
    {
        %this.scene.delete();
        %this.scene = "";
    }
    
    if (isObject(%this.shapeList))
    {
        while(%this.shapeList.getCount())
        {
            %object = %this.shapeList.getObject(0);
            %object.delete();
        }
    }
    
    %this.selectedShape = "";
}

function CollisionShapePreview::getPreviewShapeList(%this)
{
    return %this.shapeList;
}

function CollisionShapePreview::deleteShape(%this)
{
    if (!isObject(%this.selectedShape))
        return;
    
    %this.selectedShape.delete();
    %this.selectedShape = "";
    CEPreviewWindow.setTransformControlsVisibility(false, false);
}

function CollisionShapePreview::addPreviewObject(%this, %object)
{
    if (isObject(%this.previewObject))
        %this.previewObject.delete();
    
    %this.scene.deleteContents();
    %this.previewObject = %object;
    
    %this.scene.add(%this.previewObject);
    %this.previewObject.setPosition(0, 0);
  
    %half = mGetMax(%this.previewObject.getWidth(), %this.previewObject.getHeight());
    
    %this.setCurrentCameraArea(-%half, -%half, %half, %half);
    %this.setCurrentCameraZoom(1);
    %this.startCameraMove(0.5);
    
    %shapeCount = %this.previewObject.getCollisionShapeCount();
    
    for(%i = 0; %i < %shapeCount; %i++)
    {
        %shapeType = %this.previewObject.getCollisionShapeType(%i);
        
        if (%shapeType $= "circle")
        {
            %radius = %this.previewObject.getCircleCollisionShapeRadius(%i);
            %position = %this.previewObject.getCircleCollisionShapeLocalPosition(%i);
            
            %this.addCircleCollisionShape(%radius, %position);
        }
        else if (%shapeType $= "polygon")
        {
            %pointCount = %this.previewObject.getPolygonCollisionShapePointCount(%i);
            %points = "";
            
            for(%j = 0; %j < %pointCount; %j++)
            {
                %point = %this.previewObject.getPolygonCollisionShapeLocalPoint(%i, %j);
                
                if (%points $= "")
                    %points = %point;
                else
                    %points = %points SPC %point;
            }

            %box = %this.getBoundingBoxPoints(%points);
            
            %this.addPolygonCollisionShape(%points, %box);
        }
        
        CollisionEditor.shapeCount++;
        %remainingShapes = $CollisionEditor::MaxShapes - CollisionEditor.shapeCount;
        CollisionEditorGui.setRemainingShapeCount(%remainingShapes);
    }
    %this.setTransformControlsVisibility(false, false);
}

function CollisionShapePreview::addDecoratorObject(%this, %object, %position, %size)
{
    %this.scene.add(%object);
    
    if (%position !$= "")
        %object.setPosition(%position.x, %position.y);
    
    if (%size !$= "")
        %object.setSize(%size.x, %size.y);
        
    // If the decorator object is outside the current camera area, rescale the camera
    %currentMinX = getWord(%this.getCurrentCameraArea(), 0);
    %currentMinY = getWord(%this.getCurrentCameraArea(), 1);
    %currentMaxX = getWord(%this.getCurrentCameraArea(), 2);
    %currentMaxY = getWord(%this.getCurrentCameraArea(), 3);
    
    %objectMinX = %object.getPositionX() - (%object.getWidth() / 2);
    %objectMinY = %object.getPositionY() - (%object.getHeight() / 2);
    %objectMaxX = %object.getPositionX() + (%object.getWidth() / 2);
    %objectMaxY = %object.getPositionY() + (%object.getHeight() / 2);
    
    %newMinX = mGetMin(%currentMinX, %objectMinX);
    %newMinY = mGetMin(%currentMinY, %objectMinY);
    %newMaxX = mGetMax(%currentMaxX, %objectMaxX);
    %newMaxY = mGetMax(%currentMaxY, %objectMaxY);
    
    %this.setCurrentCameraArea(%newMinX, %newMinY, %newMaxX, %newMaxY);
}

function CollisionShapePreview::addCircleCollisionShape(%this, %radius, %position)
{
    %newShape = new ShapeVector()
    {
        size = %radius * 2 SPC %radius * 2;
        GravityScale = 0;
        Position = %position;
        UpdateCallback = 0;
        SceneLayer = 0;
        LineColor = $CollisionEditorPreviewShapeLineColorUnselected;
        fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
        FillMode = "1";
        IsCircle = true;
        CircleRadius = %radius;
        PolyList = "0 0 0 0 0 0";
        CollisionSuppress = true;
    };
    
    %newShape.shapeType = "circle";
    %newShape.createCircleCollisionShape(%radius, %position);
    
    %this.scene.add(%newShape);
    
    %this.shapeList.add(%newShape);
}

function CollisionShapePreview::addPolygonCollisionShape(%this, %points, %boundingBox)
{
    // Calculate size and postion from the bounding box of the points
    %centerX = (getWord(%boundingBox, 0) + getWord(%boundingBox, 2)) / 2;
    %centerY = (getWord(%boundingBox, 1) + getWord(%boundingBox, 3)) / 2;
    %sizeX = (getWord(%boundingBox, 2) - getWord(%boundingBox, 0));
    %sizeY = (getWord(%boundingBox, 3) - getWord(%boundingBox, 1));
    
    // Transform the points to the local space of the new shape
    %positionTransform = %centerX SPC %centerY;
    %scaleTransform = %sizeX SPC %sizeY;
    %points = %this.transformPoints(%points, %positionTransform, %scaleTransform);

    %newShape = new ShapeVector()
    {
        size = "1 1";
        position = %centerX SPC %centerY;
        GravityScale = 0;
        UpdateCallback = 0;
        SceneLayer = 0;
        LineColor = $CollisionEditorPreviewShapeLineColorUnselected;
        fillColor = $CollisionEditorPreviewShapeFillColorUnselected;
        FillMode = "1";
        PolyList = %points;
        CollisionSuppress = true;
    };
    
    %newShape.shapeType = "polygon";
    
    %newShape.createPolygonCollisionShape(%points);
    
    %this.scene.add(%newShape);
    %newShape.setPosition(%centerX, %centerY);
    %newShape.setSize(%sizeX, %sizeY);
    
    %this.shapeList.add(%newShape);
}

function CollisionShapePreview::addPreviewShape(%this, %shape)
{
    switch$(%shape)
    {
        case "square":
            %object = %this.createSquareShape();
            %object.shapeType = "polygon";
        case "triangle":
            %object = %this.createTriangleShape();
            %object.shapeType = "polygon";
        case "circle":
            %object = %this.createCircleShape();
            %object.shapeType = "circle";
        case "custom":
            error("Custom shape not supported");
    }
    
    %this.scene.add(%object);
    
    %points = %object.getWorldPoly();
    
    %this.shapeList.add(%object);
    
    %this.selectShape(%object);
}

function CollisionShapePreview::RotateSelectedObject(%this, %angleInDegrees, %continuous)
{
    if (%continuous)
    {
        cancel($CollisionEditor::RotationScheduleId);
      
        if (!$CollisionEditor::SmoothRotating)
            $CollisionEditor::SmoothRotating = true;
    }
   
    //%acquiredObject = %this.sceneEditor.getAcquiredObjects().getObject(0);
    %acquiredObject = %this.selectedShape;
    %acquiredObject.setAngle(%acquiredObject.getAngle() + %angleInDegrees);
   
    if (%acquiredObject.getAngle() > 360)
        %acquiredObject.setAngle(%acquiredObject.getAngle() - 360);
    else if (%acquiredObject.getAngle() < 0)
        %acquiredObject.setAngle(360 + %acquiredObject.getAngle());
      
    if (%continuous)
        $CollisionEditor::RotationScheduleId = %this.schedule(50, RotateSelectedObject, %angleInDegrees, true);
}

function CollisionShapePreview::cancelRotation(%this)
{
    cancel($CollisionEditor::RotationScheduleId);
   
    $CollisionEditor::RotationScheduleId = -1;

    $CollisionEditor::SmoothRotating = false;
}

function CERotateLeftMouseEvent::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    if (CEPreviewWindow.selectedShape.isCircle)
        return;
    
   // Schedule 1 degree rotation with double delay to allow for a potential quick rotate
   $CollisionEditor::RotationScheduleId = CEPreviewWindow.schedule(50 * 3, RotateSelectedObject, 1, true);
   
   CELeftRotateButton.Image = "{EditorAssets}:rotateArrowLeft_d";
}

function CERotateLeftMouseEvent::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    if (CEPreviewWindow.selectedShape.isCircle)
        return;
    
    if ($CollisionEditor::RotationScheduleId == -1)
        return;

    cancel($CollisionEditor::RotationScheduleId);
   
    $CollisionEditor::RotationScheduleId = -1;
   
    if (!$CollisionEditor::SmoothRotating)
        CEPreviewWindow.RotateSelectedObject(45, false);
    else
        $CollisionEditor::SmoothRotating = false;
        
    CELeftRotateButton.Image = "{EditorAssets}:rotateArrowLeft_h";
}

function CERotateLeftMouseEvent::onMouseEnter(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CELeftRotateButton.Image = "{EditorAssets}:rotateArrowLeft_h";
}

function CERotateLeftMouseEvent::onMouseLeave(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CEPreviewWindow.cancelRotation();
        
    CELeftRotateButton.Image = "{EditorAssets}:rotateArrowLeft";
}

function CERotateRightMouseEvent::onMouseDown(%this, %modifier, %mousePoint, %mouseClickCount)
{
    if (CEPreviewWindow.selectedShape.isCircle)
        return;
        
   // Schedule 1 degree rotation with double delay to allow for a potential quick rotate
   $CollisionEditor::RotationScheduleId = CEPreviewWindow.schedule(50 * 3, RotateSelectedObject, -1, true);
   
   CERightRotateButton.Image = "{EditorAssets}:rotateArrowRight_d";
}

function CERotateRightMouseEvent::onMouseUp(%this, %modifier, %mousePoint, %mouseClickCount)
{
    if (CEPreviewWindow.selectedShape.isCircle)
        return;
        
    if ($CollisionEditor::RotationScheduleId == -1)
        return;
        
    cancel($CollisionEditor::RotationScheduleId);

    $CollisionEditor::RotationScheduleId = -1;

    if (!$CollisionEditor::SmoothRotating)
        CEPreviewWindow.RotateSelectedObject(-45, false);
    else
        $CollisionEditor::SmoothRotating = false;
        
    CERightRotateButton.Image = "{EditorAssets}:rotateArrowRight_h";
}

function CERotateRightMouseEvent::onMouseEnter(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CERightRotateButton.Image = "{EditorAssets}:rotateArrowRight_h";
}

function CERotateRightMouseEvent::onMouseLeave(%this, %modifier, %mousePoint, %mouseClickCount)
{
    CEPreviewWindow.cancelRotation();
    
    CERightRotateButton.Image = "{EditorAssets}:rotateArrowRight";
}

function CEPreviewWindow::onTouchDown(%this, %mod, %worldPos, %count)
{
    echo("CEPreviewWindow::onTouchDown");
    
    // If the selection tool is active, allow selection of shapes
    if ($CollisionEditor::activeTool == $CollisionEditor::SelectionToolIndex)
    {
        %pickedShape = %this.pickShape(%worldPos);
        %this.selectShape(%pickedShape);
    }
}

function CEPreviewWindow::onTouchUp(%this, %mod, %worldPos, %count)
{
    if (!isObject(%this.lockedControl))
        return;
        
    %this.lockedControl.onMouseUp(%mod, %globalPoint, %count); 
}

function CEPreviewWindow::onTouchDragged(%this, %mod, %worldPos, %count)
{
    if (!isObject(%this.lockedControl))
        return;
        
    %windowPoint = %this.getWindowPoint(%worldPos);
    %globalPoint = Vector2Add(%windowPoint, %this.getGlobalPosition());

    %this.lockedControl.onMouseDragged(%mod, %globalPoint, %count); 
}

function CEPreviewWindow::pickShape(%this, %worldPos)
{
    // Get a list of picked shapes
    %pickArea = %this.getCurrentCameraArea();
    %pickList = %this.scene.pickArea(getWords(%pickArea, 0, 1), getWords(%pickArea, 2, 3));
    
    // Filter list to only include ShapeVectors
    %count = getWordCount(%pickList);
    %filteredPickList = "";
    for (%i = 0; %i < %count; %i++)
    {
        %object = getWord(%pickList, %i);
        
        if (%object.getClassName() !$= "ShapeVector")
            continue;
            
        if (%filteredPickList $= "")
            %filteredPickList = %object;
        else
            %filteredPickList = %filteredPickList SPC %object;
    }
    %pickList = %filteredPickList;
    
    // Filter list to only shapes whose polygons enclose the point
    %count = getWordCount(%pickList);
    %filteredPickList = "";
    for (%i = 0; %i < %count; %i++)
    {
        %shape = getWord(%pickList, %i);       
        
        %pointInShape = false;
        if (%shape.isCircle)
        {
            %distanceFromCenter = Vector2Distance(%shape.getPosition(), %worldPos);
            if (%distanceFromCenter <= %shape.circleRadius)
                %pointInShape = true;
        }
        else
        {
            %pointInShape = %this.pointInPolygon(%worldPos, %shape.getWorldPoly());
        }
        
        if (%pointInShape)
        {
            if (%filteredPickList $= "")
                %filteredPickList = %shape;
            else
                %filteredPickList = %filteredPickList SPC %shape;
        }
    }
    %pickList = %filteredPickList;

    // Check if the pickList contains the currently selected shape
    // If so, pick the next shape after it.
    // If not, pick the first shape in the pick list.
    %pickedShape = (getWordCount(%pickList) > 0) ? getWord(%pickList, 0) : "";
    for (%i = getWordCount(%pickList) - 1; %i >= 0; %i--)
    {
        %shape = getWord(%pickList, %i);
        
        if (%shape $= %this.selectedShape)
            break;
        
        %pickedShape = %shape;
    }
    
    // If the picked shape exists, set the selected shape to it
    return %pickedShape;
}

function CEPreviewWindow::pointInPolygon(%this, %point, %polygonPointList)
{
    %pointCount = getWordCount(%polygonPointList)/2;    
    
    if (%pointCount < 3)    
    {
        warn("CEPreviewWindow::pointInPolygon -- polygon must have at least 3 points");
        return false;
    }
    
    %signSum = 0;
    for (%i = 0; %i < %pointCount; %i++)
    {
        %pointIndex = %i * 2;
        %point1 = getWords(%polygonPointList, %pointIndex, %pointIndex+1);
  
        if (%i == %pointCount - 1)
            %point2 = getWords(%polygonPointList, 0, 1);
        else
            %point2 = getWords(%polygonPointList, %pointIndex+2, %pointIndex+3);
            
        %vec1 = VectorSub(%point1 SPC "0", %point SPC "0");
        %vec2 = VectorSub(%point2 SPC "0", %point SPC "0");     

        if (%vec1 $= %vec2)
            return false;        
        
        %cross = VectorCross(%vec1, %vec2);
            
        %signSum += (%cross.z >= 0) ? 1 : -1;
    }
    
    if (mAbs(%signSum) == %pointCount)
        return true;
    else
        return false;
}