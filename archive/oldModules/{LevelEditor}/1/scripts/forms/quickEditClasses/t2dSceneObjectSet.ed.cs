//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "SceneObjectSet", "LBQESceneObjectSet::CreateContent", "LBQESceneObjectSet::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQESceneObjectSet::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQESceneObjectClass", %quickEditObj);
   
   %sceneObjectRollout = %base.createRolloutStack("Selected Object Set", true);
   %sceneObjectRollout.createTextEdit2("PositionX", "PositionY", 3, "Position", "X", "Y", "Position", true);
   %sceneObjectRollout.createTextEdit2("Width", "Height", 3, "Size", "Width", "Height", "Size", true);
   %sceneObjectRollout.createTextEdit ("Angle", 3, "Angle", "Angle", true);
   %sceneObjectRollout.createCheckBox ("FlipX", "Flip Horizontal", "Flip Horizontal", true);
   %sceneObjectRollout.createCheckBox ("FlipY", "Flip Vertical", "Flip Vertical", true);
   %sceneObjectRollout.createLeftRightEdit("SceneLayer", "0;", "31;", 1, "Scene Layer", "Scene Rendering Layer");
   %sceneObjectRollout.createLeftRightEdit("SceneGroup", "0;", "31;", 1, "Scene Group", "Scene Group");
   
   %alignRollout = %base.createRolloutStack( "Align", true );
   %alignRollout.createAlignTools( false );
   
   // Return Ref to Base.
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQESceneObjectSet::SaveContent( %contentCtrl )
{
   // Nothing.
}
