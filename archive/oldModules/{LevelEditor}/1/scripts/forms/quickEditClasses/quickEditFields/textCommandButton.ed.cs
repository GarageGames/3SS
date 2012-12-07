//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createTextCommandButton(%this, %command, %label, %accessor)
{
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
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
   };
   
   %checkBox = new GuiIconButtonCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "148 4";
      Extent = "78 24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      text = %label;
      groupNum = "-1";
      buttonType = "PushButton";
      buttonMargin = "4 4";
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Center";
      textMargin = "4";
   };
   
   %editControl = new GuiTextEditCtrl() {
      class = QuickEditCommandText;
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "10 5";
      Extent = "128 22";
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
   };
   
   %checkBox.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %command @ "(" @ %editControl @ ".getText());";
   
   %container.add(%checkBox);
   %container.add(%editControl);
   
   %this.addProperty(%editControl);
   %this.add(%container);
   return %container;
}

function QuickEditCommandText::setProperty(%this)
{
   %value = eval(%this.accessor);
   %this.setText(%value);
}
