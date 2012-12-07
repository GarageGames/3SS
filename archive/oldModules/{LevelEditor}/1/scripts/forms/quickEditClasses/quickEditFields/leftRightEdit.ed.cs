//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createLeftRightEdit(%this, %accessor, %min, %max, %change, %label, %tooltip, %addUndo)
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
   
   %editControl = new GuiTextEditCtrl() {
      class = QuickEditTextEdit;
      internalName = %property @ "TextEdit";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "146 5";
      Extent = "24 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      accessor = %accessor;
      undoLabel = %label;
      min = %min;
      max = %max;
      object = %this.object;
      addUndo = %addUndo;
   };
   
   %editControl.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   %editControl.validate = %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   
   %leftButton = new GuiButtonCtrl() {
      class = QuickEditLeftRightEditButton;
      internalName = %property @ "LeftButton";
      canSaveDynamicFields = "0";
      Profile = "EditorButtonLeft";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "128 7";
      Extent = "14 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "<";
      groupNum = "-1";
      buttonType = "PushButton";
      accessor = %accessor;
      undoLabel = %label;
      change = -%change;
      textEdit = %editControl;
      min = %min;
      max = %max;
      object = %this.object;
      addUndo = %addUndo;
   };
   
   %leftButton.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %leftButton @ ".updateProperty(" @ %leftButton @ ".object" @ ");";
   
   %rightButton = new GuiButtonCtrl() {
      class = QuickEditLeftRightEditButton;
      internalName = %property @ "RightButton";
      canSaveDynamicFields = "0";
      Profile = "EditorButtonRight";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "178 7";
      Extent = "14 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = ">";
      groupNum = "-1";
      buttonType = "PushButton";
      accessor = %accessor;
      undoLabel = %label;
      change = %change;
      textEdit = %editControl;
      min = %min;
      max = %max;
      object = %this.object;
      addUndo = %addUndo;
   };
   
   %rightButton.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %rightButton @ ".updateProperty(" @ %rightButton @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%editControl);
   %container.add(%leftButton);
   %container.add(%rightButton);
   
   %this.addProperty(%rightButton);
   %this.addProperty(%leftButton);
   %this.addProperty(%editControl);
   %this.add(%container);
   return %container;
}

function QuickEditLeftRightEditButton::setProperty(%this, %object)
{
   %this.textEdit.setProperty(%object);
}

function QuickEditLeftRightEditButton::updateProperty(%this, %object)
{
   eval("%min = " @ %this.min);
   eval("%max = " @ %this.max);
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   %newValue = %oldValue + %this.change;
   if ((%newValue < %min) || (%newValue > %max))
      return;
     
   QuickEditField::setObjectValue(%this, %object, %newValue);
   
   if (%this.addUndo || (%this.addUndo $= ""))
      QuickEditField::addUndo(%this, %object, %oldValue, %newValue);
   ToolManager.onQuickEdit(%object);
   
   %this.textEdit.setProperty(%object);
}
