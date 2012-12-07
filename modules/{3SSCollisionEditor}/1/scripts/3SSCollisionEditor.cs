//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function CollisionEditor::initialize(%this)
{
    CESelectButton.performClick();
            
    %this.shapeCount = 0;
    %this.pointCount = 0;
    
    if (isObject(%this.sourceObject))
        %this.sourceObject = "";
        
    if (isObject(%this.previewObject))
        %this.previewObject.delete();
        
    if (!isObject(CollisionEditor.decoratorObjectSet))
        CollisionEditor.decoratorObjectSet = new SimSet();
}

function CollisionEditor::setObject(%this, %object)
{
    %this.sourceObject = %object;
    %this.previewObject = %object.clone();
    %this.previewObject.sceneLayer = 15;
    %this.previewObject.class = "";
    %this.previewObject.superclass = "";
    %this.previewObject.clearBehaviors();
    %this.previewObject.setPrefab("");
    %this.previewObject.setFlipX(false);
    %this.previewObject.setFlipY(false);
        
    if (%this.previewObject.animation !$= "")
    {
        %this.previewObject.pauseAnimation(true);
        %this.playingAnimation = false;
    }
}

function CollisionEditor::addShape(%this, %type)
{
    if (%this.shapeCount >= 7)
        return;
        
    %this.shapeCount++;
    
    %remainingShapes = $CollisionEditor::MaxShapes - %this.shapeCount;
    
    CollisionEditorGui.setRemainingShapeCount(%remainingShapes);
    
    %this.ShapePreviewWindow.addPreviewShape(%type);
}

function CollisionEditor::deleteShape(%this)
{
    if (%this.shapeCount <= 0 || !isObject(CollisionEditor.ShapePreviewWindow.selectedShape))
        return;
    
    CollisionEditor.ShapePreviewWindow.deleteShape();
     
    %this.shapeCount--;
    
    %remainingShapes = $CollisionEditor::MaxShapes - %this.shapeCount;
    
    CollisionEditorGui.setRemainingShapeCount(%remainingShapes);
}

function CollisionEditor::setActiveTool(%this, %index)
{
    $CollisionEditor::activeTool = %index;
    %this.activeTool = %index;
}

function CollisionEditor::save(%this)
{
    %shapeList = %this.ShapePreviewWindow.getPreviewShapeList();
    %this.sourceObject.clearCollisionShapes();
    
    for(%i = 0; %i < %shapeList.getCount(); %i++)
    {
        %shape = %shapeList.getObject(%i);
        
        %type = %shape.shapeType;
        
        if (%type $= "polygon")
        {
            %points = %shape.getWorldPoly();
            
            %this.sourceObject.createPolygonCollisionShape(%points);
        }
        else if(%type $= "circle")
        {
            %radius = %shape.circleRadius;
            %position = %shape.position;
            %this.sourceObject.createCircleCollisionShape(%radius, %position);
        }
    }
    
    %this.invokingGui.onCollisionEditSave(%this.sourceObject);
}

function CollisionEditor::updateAnimation(%this, %state)
{
    if (%this.previewObject.animation $= "")
        return;

    switch$(%state)
    {
        case "play":
            %this.previewObject.setAnimationFrame(0);
            %this.previewObject.playAnimation(%this.previewObject.animation);
            %this.previewObject.pauseAnimation(false);
            CollisionEditor.playingAnimation = true;
            
        case "pause":
            %this.previewObject.pauseAnimation(true);
            CollisionEditor.playingAnimation = false;
            
        case "stop":
            %this.previewObject.stopAnimation();
            CollisionEditor.playingAnimation = false;
    }
}

function CollisionEditor::getPreviewObject(%this)
{
    return %this.previewObject;
}