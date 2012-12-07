//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$Gui::clipboardFile = expandPath("./clipboard.gui");

if (!isObject(GuiEditorClassProfile)) new GuiControlProfile (GuiEditorClassProfile)
{
   opaque = true;
   fillColor = "232 232 232";
   border = 1;
   borderColor   = "40 40 40 140";
   borderColorHL = "127 127 127";
   fontColor = "0 0 0";
   fontColorHL = "32 100 100";
   fixedExtent = true;
   justify = "center";
   bitmap = "^{EditorAssets}/data/images/scrollBar";
   hasBitmapArray = true;
};

if (!isObject(GuiBackFillProfile)) new GuiControlProfile (GuiBackFillProfile)
{
   opaque = true;
   fillColor = "0 94 94";
   border = true;
   borderColor = "255 128 128";
   fontType = "Arial";
   fontSize = 14;
   fontColor = "0 0 0";
   fontColorHL = "32 100 100";
   fixedExtent = true;
   justify = "center";
};

if (!isObject(GuiControlListPopupProfile)) new GuiControlProfile (GuiControlListPopupProfile)
{
   opaque = true;
   fillColor = "255 255 255";
   fillColorHL = "128 128 128";
   border = true;
   borderColor = "0 0 0";
   fontColor = "0 0 0";
   fontColorHL = "255 255 255";
   fontColorNA = "128 128 128";
   textOffset = "0 2";
   autoSizeWidth = false;
   autoSizeHeight = true;
   tab = true;
   canKeyFocus = true;
   bitmap = "^{EditorAssets}/data/images/dropDown";
   hasBitmapArray = true;
};

if (!isObject(GuiSceneEditProfile)) new GuiControlProfile(GuiSceneEditProfile)
{
   canKeyFocus = true;
   tab = true;
};

if(!isObject(GuiInspectorTextEditProfile)) new GuiControlProfile ("GuiInspectorTextEditProfile")
{
   // Transparent Background
   opaque = true;
   fillColor = "240 240 240";
   fillColorHL = "0 0 0";

   // No Border (Rendered by field control)
   border = 1;
   borderColor = "180 180 180";
   cursorColor = "255 255 255";
   borderColorHL = "255 0 0";
   borderThickness = "1";

   tab = true;
   canKeyFocus = true;

   // font
   fontType = "Arial";
   fontSize = 14;

   fontColor = "130 130 130";
   fontColorSEL = "255 255 255";
   fontColorHL = "255 255 255";
   fontColorNA = "255 255 255";
};


if (!isObject(GuiInspectorTextEditRightProfile)) new GuiControlProfile (GuiInspectorTextEditRightProfile : GuiInspectorTextEditProfile)
{
   justify = right;
};

if (!isObject(GuiInspectorGroupProfile)) new GuiControlProfile (GuiInspectorGroupProfile )
{
   fontType    = "Arial Bold";
   fontSize    = "15";
   
   fontColor = "255 255 255 255";
   fontColorHL = "25 25 25 220";
   fontColorNA = "128 128 128";
   
   justify = "left";
   opaque = false;
   border = false;
  
   bitmap = "^{EditorAssets}/data/images/rollout.png";
   
   textOffset = "20 -1";

};

if (!isObject(GuiInspectorFieldProfile)) new GuiControlProfile (GuiInspectorFieldProfile)
{
   // fill color
   opaque = false;
   fillColor = "255 255 255";
   fillColorHL = "128 128 128";
   fillColorNA = "244 244 244";

   // border color
   border = false;
   borderColor   = "190 190 190";
   borderColorHL = "156 156 156";
   borderColorNA = "64 64 64";
   
   bevelColorHL = "255 255 255";
   bevelColorLL = "0 0 0";
   
   bitmap = "^{EditorAssets}/data/images/rollout";

   // font
   fontType = "Arial";
   fontSize = 14;

   fontColor = "32 32 32";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";

   tab = true;
   canKeyFocus = true;
};

if (!isObject(GuiInspectorBackgroundProfile)) new GuiControlProfile (GuiInspectorBackgroundProfile : GuiInspectorFieldProfile)
{
   border = 5;
   cankeyfocus=true;
   tab = true;
};

if (!isObject(GuiInspectorTypeFileNameProfile)) new GuiControlProfile (GuiInspectorTypeFileNameProfile)
{
   // Transparent Background
   opaque = false;

   // No Border (Rendered by field control)
   border = 5;

   tab = true;
   canKeyFocus = true;

   // font
   fontType = "Arial";
   fontSize = 14;
   
   // Center text
   justify = "center";

   fontColor = "32 32 32";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";

   fillColor = "255 255 255";
   fillColorHL = "128 128 128";
   fillColorNA = "244 244 244";

   borderColor   = "190 190 190";
   borderColorHL = "156 156 156";
   borderColorNA = "64 64 64";
};

if (!isObject(InspectorTypeEnumProfile)) new GuiControlProfile (InspectorTypeEnumProfile : GuiInspectorFieldProfile)
{
   mouseOverSelected = true;
   bitmap = "^{EditorAssets}/data/images/scrollBar";
   hasBitmapArray = true;
   opaque=true;
   border=true;
   textOffset = "4 0";
};

if (!isObject(InspectorTypeCheckboxProfile)) new GuiControlProfile (InspectorTypeCheckboxProfile : GuiInspectorFieldProfile)
{
   bitmap = "^{EditorAssets}/data/images/checkBox";
   hasBitmapArray = true;
   opaque=false;
   border=false;
};

if (!isObject(GuiToolboxButtonProfile)) new GuiControlProfile (GuiToolboxButtonProfile : GuiWindowProfile)
{
   justify = "center";
   fontColor = "0 0 0";
   border = 0;
   textOffset = "0 0";   
};

if (!isObject(T2DDatablockDropDownProfile)) new GuiControlProfile (T2DDatablockDropDownProfile : GuiPopUpMenuProfile);

if (!isObject(GuiDirectoryTreeProfile)) new GuiControlProfile (GuiDirectoryTreeProfile : GuiTreeViewProfile)
{
   fontColor = "40 40 40";
   fontColorSEL= "80 80 80 175"; 
   fillColorHL = "0 60 150";
   fontColorNA = "240 240 240";
   fontType = "Arial";
   fontSize = 14;
};

if (!isObject(GuiDirectoryFileListProfile)) new GuiControlProfile (GuiDirectoryFileListProfile)
{
   fontColor = "40 40 40";
   fontColorSEL= "80 80 80 175"; 
   fillColorHL = "0 60 150";
   fontColorNA = "240 240 240";
   fontType = "Arial";
   fontSize = 14;
};

if (!isObject(GuiDragAndDropProfile)) new GuiControlProfile(GuiDragAndDropProfile)
{
};
