//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "ShapeVector", "LBQEShapeVector::CreateContent", "LBQEShapeVector::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEShapeVector::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQEShapeVectorClass", %quickEditObj);
   %rollout = %base.createRolloutStack("ShapeVector", true);
   
   %rollout.createCommandButton("shapeVectorEditor.open(" @ %quickEditObj @ ");", "Edit Polygon");
   %rollout.createColorPicker("LineColor", "Line Color", "Line Color");
   %rollout.createCheckBox("FillMode", "Fill Polygon", "", "", "", "", false);
   %rollout.createColorPicker("FillColor", "Fill Color", "Fill Color");
   %rollout.createCommandButton("shapeVectorEditor.open(" @ %quickEditObj @ "); shapeVectorEditor.saveHullAsCollisionPoly();", "Use Convex Hull as Collision Polygon");
   
   // Return Ref to Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEShapeVector::SaveContent( %contentCtrl )
{
   // Nothing.
}
