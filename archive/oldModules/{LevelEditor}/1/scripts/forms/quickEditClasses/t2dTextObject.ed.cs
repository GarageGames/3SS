//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "TextObject", "LBQETextObject::CreateContent", "LBQETextObject::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQETextObject::CreateContent( %contentCtrl, %quickEditObj )
{
   %base = %contentCtrl.createBaseStack("LBQETextObjectClass", %quickEditObj);
   %rollout = %base.createRolloutStack("TextObject", true);
   
   %fonts = enumerateFonts();
   %rollout.createListBox( "font", true, "Font", %fonts );
   
   %rollout.createListBox("textAlign", true, "Alignment", "Left\tRight\tCenter\tJustify", "", false);
   %rollout.createTextEditProperty("lineHeight", "3", "Character Height", "", true);
   %rollout.createTextEditProperty("lineSpacing", "3", "Line Spacing", "");
   %rollout.createTextEditProperty("characterSpacing", "3", "Character Spacing", "");
   %rollout.createTextEditProperty("aspectRatio", "3", "Horizontal Scale", "", true);
   %rollout.createCheckBox("wordWrap", "Word Wrap", "", "", "", "", true);
   %rollout.createCheckBox("hideOverflow", "Hide Overflow", "", "", "", "", true);
   %rollout.createCheckBox("filter", "Filtered", "Enables or disables the filtering on the text object", "", "", "", true);
   %rollout.createCheckBox("noUnicode", "No Unicode", "Disables intensive unicode conversions (if unicode is not required)", "", "", "", true);
   %rollout.createCheckBox("integerPrecision", "Integer Precision", "Allows the text object to work on integer levels, avoiding half pixel imprecisions", "", "", "", true);
   
   // Return Ref to Base.
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQETextObject::SaveContent( %contentCtrl )
{
   // Nothing.
}
