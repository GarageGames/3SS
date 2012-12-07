//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createT2DFramePicker(%this, %accessor, %label, %tooltip, %addUndo)
{
   if (%addUndo $= "")
      %addUndo = true;
      
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 32";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
   };

   %labelControl = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHLBold";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16 8";
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = %label;
      maxLength = "1024";
   };
   %imageMapList = new guiButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      DatablockFilter = %filter;
      Profile = "EditorButton";
      Class = "QuickEditT2DFramePicker";
      internalName = %property @ "DBPicker";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "128 7";
      Extent = "24 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "";
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
      addUndo = %addUndo;
      precision = "0";
   };
   
   %imageMapList.contextPopup = LBObjectThumbnail::createImageMapFramePopup( %this );
   %imageMapList.scene = new Scene();
   
   %imageMapList.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %imageMapList @ ".showPicker();";
   
   %container.add(%labelControl);
   %container.add(%imageMapList);
   
   %imageMapList.setProperty(%this.object);
   %this.addProperty(%imageMapList);
   %this.add(%container);
   return %container;
}

function QuickEditT2DFramePicker::showPicker( %this )
{
   %globalPosition = %this.getGlobalPosition();
   %popupWidth = GetWord( %this.contextPopup.Dialog.getExtent(), 0 );
   %this.contextPopup.object = %this.object;
   
   // Show Popup at desired position
   %this.contextPopup.Show( GetWord( %globalPosition, 0 ) - %popupWidth, GetWord( %globalPosition, 1 ) );
}

function QuickEditT2DFramePicker::setProperty(%this, %object)
{
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   %this.setText( %oldValue );
}

function QuickEditT2DFramePicker::updateProperty(%this, %object)
{
   %value = %this.getTextById(%this.getSelected());
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   //if (%oldValue $= "")
      //%oldValue = "-";
   
   QuickEditField::updateProperty(%this, %object, %this.getTextById(%this.getSelected()), %oldValue);
}
