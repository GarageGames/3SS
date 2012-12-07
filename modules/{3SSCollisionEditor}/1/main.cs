//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initialize3SSCollisionEditor(%scopeSet)
{
    //-----------------------------------------------------------------------------
    // Initialization
    //----------------------------------------------------------------------------- 
    $CollisionEditor::MaxShapes = 8;
    $CollisionEditor::MaxPoints = 8;
    
    $CollisionEditor::RotationRate = 1;
    $CollisionEditor::ScaleRate = 1;
    
    $CollisionEditor::SelectionToolIndex = 1;
    $CollisionEditor::SizeToolIndex = 2;
    $CollisionEditor::RotateToolIndex = 3;
    $CollisionEditor::AddShapeToolIndex = 4;
    $CollisionEditor::DeleteShapeToolIndex = 5;
    $CollisionEditor::AddPointToolIndex = 6;
    $CollisionEditor::DeletePointToolIndex = 7;
    
    $CollisionEditor::activeTool = $CollisionEditor::SelectionToolIndex;
    
    $CollisionEditor::squareShapePoints = "-0.5 -0.5 0.5 -0.5 0.5 0.5 -0.5 0.5";
    $CollisionEditor::circleShapeRadius = "0.25";
    $CollisionEditor::triangleShapePoints = "-0.0025 0.5 0.5 -0.5 -0.5 -0.5";
    
    $CollisionEditor::SmoothRotating = false;
    $CollisionEditor::RotationScheduleId = -1;
    
    //-----------------------------------------------------------------------------
    // Execute scripts
    //-----------------------------------------------------------------------------
    exec("./scripts/gui.cs");
    exec("./scripts/3SSCollisionEditor.cs");
    exec("./scripts/3SSCollisionPreview.cs");
    exec("./scripts/3SSCollisionPreviewTransformControls.cs");

    //-----------------------------------------------------------------------------
    // Load GUIs
    //----------------------------------------------------------------------------- 
    %scopeSet.add( TamlRead("./gui/3SSCollisionEditorGui.gui.taml") );
}

function destroy3SSCollisionEditor()
{
    if (isObject(CollisionEditorGui))
    {
        // close it down, if it's open.
        if (CollisionEditorGui.open)
            CollisionEditorGui.close();
    }
}