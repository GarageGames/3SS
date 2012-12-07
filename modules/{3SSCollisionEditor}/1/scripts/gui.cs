//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function CollisionEditorGui::onSleep(%this)
{
    if (%this.open)
        %this.close();
    EditorEventManager.postEvent("_TSSCollisionEditorClose");
}

function CollisionEditorGui::open(%this, %object, %invokingGui)
{
    EditorEventManager.postEvent("_TSSCollisionEditorOpen");

    if (%this.open)
        %this.close();
    
    %this.open = true;
        
    if (!isObject(CollisionEditor))
    {
        new ScriptObject(CollisionEditor)
        {
            showEditObject = true;
            showPolygon = true;
            showHull = true;
            showConvexViolations = true;
            clampToBounds = true;
            insertBetweenMode = true;
            showBackground = false;
            zoomEnable = false;
            polyControlsEnable = true;
            title = "Collision Editor";
            shapeCount = 0;
            currentPointCount = 0;
            ShapePreviewWindow = CEPreviewWindow;
            invokingGui = %invokingGui;
        };
    }
    
    if (isObject(%this.stateList))
        %this.stateList.deleteContents();
    else
        %this.stateList = new SimSet();
        
    CollisionEditor.initialize();
    
    if (!isObject(%object))
        return;
    
    CEViewDropdown.setSelected(1);
    CollisionEditor.setObject(%object);
    CollisionEditor.ShapePreviewWindow.addPreviewObject(CollisionEditor.previewObject);
}

function CollisionEditorGui::close(%this)
{
    if (isObject(CollisionEditor))
    {
        CollisionEditor.save();
        CollisionEditor.ShapePreviewWindow.destroy();
        
                // Remove all decorators
        while (CollisionEditor.decoratorObjectSet.getCount() > 0)
        {
            CollisionEditor.decoratorObjectSet.getObject(0).delete();
        }
        CollisionEditor.decoratorObjectSet.delete();
        
        CollisionEditor.delete();
    }
    
    %this.open = false;
}

// object - The object to add
// [position] - Optional position relocation
// [size] - Optional size change
function CollisionEditorGui::addDecoratorObject(%this, %object, %position, %size)
{
    %decoratorObject = %object.clone();
    %decoratorObject.sceneLayer = 15;
    %decoratorObject.class = "";
    %decoratorObject.superclass = "";
    %decoratorObject.clearBehaviors();
    %decoratorObject.setPrefab("");
    %decoratorObject.setFlipX(false);
    %decoratorObject.setFlipY(false);

    CollisionEditor.decoratorObjectSet.add(%decoratorObject);
    
    CollisionEditor.ShapePreviewWindow.addDecoratorObject(%decoratorObject, %position, %size);
}

function CollisionEditorGui::setRemainingShapeCount(%this, %count)
{
    CEShapeCountText.setText(%count);
}

function CollisionEditorGui::setRemainingPointCount(%this, %count)
{
    CEPointsRemainingTag.setText(%count);
}

function CESelectButton::onClick(%this)
{
    CESelectButton.NormalImage = "{3SSCollisionEditor}:largeSelectionNormalImage";
    CollisionEditor.setActiveTool($CollisionEditor::SelectionToolIndex);
    CELeftRotateButton.Visible = 0;
    CERightRotateButton.Visible = 0;
    CERotateRightMouseEvent.Active = 0;
    CERotateLeftMouseEvent.Active = 0;
    
    if (isObject(CEPreviewWindow.selectedShape))
    {
        CEPreviewWindow.setTransformControlsVisibility(true, false);
        %box = CEPreviewWindow.getBoundingBoxShape(CEPreviewWindow.selectedShape);
        CEPreviewWindow.updateTransformControls(%box);
    }
}

function CESizeButton::onClick(%this)
{
    CESelectButton.NormalImage = "{3SSCollisionEditor}:largeSelectionNormalImage";
    CollisionEditor.setActiveTool($CollisionEditor::SizeToolIndex);
    CELeftRotateButton.Visible = 0;
    CERightRotateButton.Visible = 0;
    CERotateRightMouseEvent.Active = 0;
    CERotateLeftMouseEvent.Active = 0;

    if (isObject(CEPreviewWindow.selectedShape))
    {
        CEPreviewWindow.setTransformControlsVisibility(true, true);
        %box = CEPreviewWindow.getBoundingBoxShape(CEPreviewWindow.selectedShape);
        CEPreviewWindow.updateTransformControls(%box);
    }
}

function CERotateButton::onClick(%this)
{
    CESelectButton.NormalImage = "{3SSCollisionEditor}:largeSelectionNormalImage";
    CollisionEditor.setActiveTool($CollisionEditor::RotateToolIndex);
    
    %boundingBox = CEPreviewWindow.getBoundingBoxShape(CEPreviewWindow.selectedShape);
    %minX = getWord(%boundingBox, 0);
    %maxX = getWord(%boundingBox, 2);
    %maxY = getWord(%boundingBox, 3); 
    
    %windowPoint = CEPreviewWindow.getWindowPoint(%minX, %maxY);
    %windowPoint2 = CEPreviewWindow.getWindowPoint(%maxX, %maxY);
    
    CELeftRotateButton.setPosition(%windowPoint.x - 15, %windowPoint.y-10);    
    CERightRotateButton.setPosition(%windowPoint2.x - 15, %windowPoint2.y-10);    
    
    CERightRotateButton.Visible = 1;
    CERightRotateButton.Image = "{EditorAssets}:rotateArrowRight";
    
    CELeftRotateButton.Visible = 1;
    CELeftRotateButton.Image = "{EditorAssets}:rotateArrowLeft";
    
    CERotateRightMouseEvent.Active = 1;
    CERotateLeftMouseEvent.Active = 1;
    
    CEPreviewWindow.setTransformControlsVisibility(false, false);
}

function CEAddShapeButton::onClick(%this)
{
    if (CollisionEditor.shapeCount >= 8 || CollisionEditorGui.shapeType $= "")
        return;
    
    switch$(CollisionEditorGui.shapeType)
    {
        case "square":
            CollisionEditor.ShapePreviewWindow.addPreviewShape("square");
        
        case "triangle":
            CollisionEditor.ShapePreviewWindow.addPreviewShape("triangle");
        
        case "circle":            
            CollisionEditor.ShapePreviewWindow.addPreviewShape("circle");
        
        case "custom":
            CollisionEditor.ShapePreviewWindow.addPreviewShape("custom");
    }
    
    CollisionEditor.shapeCount++;
    %remainingShapeCount = $CollisionEditor::MaxShapes - CollisionEditor.shapeCount;
    CollisionEditorGui.setRemainingShapeCount(%remainingShapeCount);
}

function CEDeleteShapeButton::onClick(%this)
{
    CollisionEditor.deleteShape();
}

function CESquareShapeButton::onClick(%this)
{
    CollisionEditorGui.shapeType = "square";
}

function CETriangleShapeButton::onClick(%this)
{
    CollisionEditorGui.shapeType = "triangle";
}

function CECircleShapeButton::onClick(%this)
{
    CollisionEditorGui.shapeType = "circle";
}

function CECustomShapeButton::onClick(%this)
{
    CollisionEditorGui.shapeType = "custom";
}

function CEPlayAnimButton::onClick(%this)
{
    if (CollisionEditor.playingAnimation)
    {
        CollisionEditor.updateAnimation("pause");
        CEPlayAnimButton.setNormalImage("{EditorAssets}:playImageMap");
        CEPlayAnimButton.setHoverImage("{EditorAssets}:play_hImageMap");
        CEPlayAnimButton.setDownImage("{EditorAssets}:play_dImageMap");
        CEPlayAnimButton.setInactiveImage("{EditorAssets}:play_iImageMap");
    }
    else
    {
        CollisionEditor.updateAnimation("play");
        CEPlayAnimButton.setNormalImage("{EditorAssets}:pauseImageMap");
        CEPlayAnimButton.setHoverImage("{EditorAssets}:pause_hImageMap");
        CEPlayAnimButton.setDownImage("{EditorAssets}:pause_dImageMap");
        CEPlayAnimButton.setInactiveImage("{EditorAssets}:pause_iImageMap");
    }
}

function CEStopAnimButton::onClick(%this)
{
    if (CollisionEditor.playingAnimation)
    {
        CollisionEditor.updateAnimation("stop");
        CEPlayAnimButton.setNormalImage("{EditorAssets}:playImageMap");
        CEPlayAnimButton.setHoverImage("{EditorAssets}:play_hImageMap");
        CEPlayAnimButton.setDownImage("{EditorAssets}:play_dImageMap");
        CEPlayAnimButton.setInactiveImage("{EditorAssets}:play_iImageMap");
    }
}

function CollisionEditorGui::addStateEntry(%this, %stateLabel, %stateAsset, %frame)
{
    CEViewDropdown.setActive(true);
    
    if (AssetDatabase.getAssetType(%stateAsset) $= "AnimationAsset")
    {
        CollisionEditor.playingAnimation = true;
        CEPlayAnimButton.setActive(true);
        CEStopAnimButton.setActive(true);
        CEPlayAnimButton.setNormalImage("{EditorAssets}:pauseImageMap");
        CEPlayAnimButton.setHoverImage("{EditorAssets}:pause_hImageMap");
        CEPlayAnimButton.setDownImage("{EditorAssets}:pause_dImageMap");
        CEPlayAnimButton.setInactiveImage("{EditorAssets}:pause_iImageMap");
    }
    
    %newState = new ScriptObject()
    {
        label = %stateLabel;
        asset = %stateAsset;
        frame = %frame;
    };
    
    %this.stateList.add(%newState);
    
    CEViewDropdown.add(%stateLabel, %this.stateList.getCount());
    CEViewDropdown.setSelected(1);
}

function CollisionEditorGui::clearStateEntries(%this)
{
    CEViewDropdown.clear();
    %this.stateList.deleteContents();
    CEViewDropdown.setActive(false);
}

function CEViewDropdown::onSelect(%this, %selectionId)
{
    %stateEntry = CollisionEditorGui.stateList.getObject(%selectionId - 1);
    
    if (!isObject(%stateEntry))
        return;
        
    %previewObject = CollisionEditor.getPreviewObject();
    
    if (AssetDatabase.getAssetType(%stateEntry.asset) $= "AnimationAsset")
    {
        %previewObject.Animation = %stateEntry.asset;
    }
    else
    {
        %previewObject.imageMap = %stateEntry.asset;
        %previewObject.Frame = %stateEntry.frame;
    }
}