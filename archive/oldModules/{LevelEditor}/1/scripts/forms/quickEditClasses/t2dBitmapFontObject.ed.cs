//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "BitmapFontObject", "LBQEBitmapFontObject::CreateContent", "LBQEBitmapFontObject::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEBitmapFontObject::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQEBitmapFontObjectClass", %quickEditObj);
   %rollout = %base.createRolloutStack("BitmapObject", true);
   
   %imageMapList = %rollout.createT2DDatablockList("ImageMap", "Image Map", "ImageAsset", "The Image to Display on This Font");
   %rollout.createTextEdit("Text", "TEXT", "Text", "Name the Object for Referencing in Script");
   %rollout.createListBox("textAlignment", true, "Alignment", "Left\tCenter\tRight\t", "", false);
   %rollout.createTextEditProperty("characterSize", "4", "Character Size", "");
   %rollout.createTextEditProperty("characterPadding", "3", "Character Padding", "");
   
   // Return Ref to Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQEBitmapFontObject::SaveContent( %contentCtrl )
{
   // Nothing.
}
