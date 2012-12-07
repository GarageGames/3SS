//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

%layoutObj = new GuiFormCtrl() {
   Caption = "Scene View";
   ContentLibrary = "LevelBuilder";
   Content = "Scene View";
   Movable = "0";
   HasMenu = "1";
   canSaveDynamicFields = "0";
   class = "FormControlClass";
   Profile = "EditorTransparentProfile";
   HorizSizing = "width";
   VertSizing = "height";
   position = "0 0";
   Extent = "985 1000";
   MinExtent = "20 20";
   canSave = "1";
   Visible = "1";
   hovertime = "1000";

};

GuiFormManager::RegisterLayout("LevelBuilder","Default",%layoutObj);
