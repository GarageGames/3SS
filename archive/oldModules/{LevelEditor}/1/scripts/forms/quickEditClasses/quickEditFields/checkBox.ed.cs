//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createCheckBox(%this, %accessor, %label, %tooltip, %spatial, %useInactive, %addUndo, %isProperty)
{
   if (%useInactive $= "")
      %useInactive = false;
   
   if (%addUndo $= "")
      %addUndo = true;
   
   if( %isProperty $= "" )
      %isProperty = false;
   
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 20";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
   };
   
   %checkBox = new GuiCheckBoxCtrl() {
      canSaveDynamicFields = "0";
      class = QuickEditCheckBox;
      internalName = %accessor @ "CheckBox";
      Profile = "EditorCheckBox";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16 1";
      Extent = "323 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = %label;
      groupNum = "-1";
      buttonType = "ToggleButton";
      undoLabel = %label;
      accessor = %accessor;
      precision = "TEXT"; //Allows a value of "" for false
      object = %this.object;
      useInactiveState = %useInactive;
      addUndo = %addUndo;
      isProperty = %isProperty;
   };
   %checkBox.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %checkBox @ ".updateProperty(" @ %checkBox @ ".object" @ ");";
   
   %container.add(%checkBox);
   
   %checkBox.setProperty(%this.object);
   %this.addProperty(%checkBox, %spatial);
   %this.add(%container);
   return %container;
}

function QuickEditCheckBox::setProperty(%this, %object)
{
   %this.setStateOn(QuickEditField::getObjectValue(%this, %object));
}

function QuickEditCheckBox::updateProperty(%this, %object)
{
   QuickEditField::updateProperty(%this, %object, %this.getValue());
}
