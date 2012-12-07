//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "Path", "LBQEPath::CreateContent", "LBQEPath::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEPath::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQEPathClass", %quickEditObj);
   %rollout = %base.createRolloutStack("Path", true);
   %rollout.createEnumList("pathType", false, "Path Type", "The Type of Interpolation to Use for the Path", "Path", "pathType");
   
   // Return Ref to Base.
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEPath::SaveContent( %contentCtrl )
{
   // Nothing.
}
