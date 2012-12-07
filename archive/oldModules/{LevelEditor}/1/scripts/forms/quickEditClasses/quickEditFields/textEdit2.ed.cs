//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createTextEdit2(%this, %accessor1, %accessor2, %precision, %label, %subLabel1, %subLabel2, %tooltip, %spatial)
{
   %useWords = false;
   if(%accessor1 $= %accessor2)
      %useWords = true;
   
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 64";
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
   
   %editControl1 = new GuiTextEditCtrl() {
      class = QuickEditTextEdit;
      internalName = %accessor1 @ "TextEdit0";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "128 5";
      Extent = "64 22";
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
      accessor = %accessor1;
      undoLabel = %label SPC %subLabel1;
      precision = %precision;
      object = %this.object;
   };
   
   %subLabelControl1 = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorTextHLBoldLeft";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "204 8";
      Extent = "48 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = %subLabel1;
      maxLength = "1024";
   };
   
   %editControl2 = new GuiTextEditCtrl() {
      class = QuickEditTextEdit;
      internalName = %accessor2 @ "TextEdit1";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "128 31";
      Extent = "64 22";
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
      accessor = %accessor2;
      undoLabel = %label SPC %subLabel2;
      precision = %precision;
      object = %this.object;
   };
   
   %subLabelControl2 = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorTextHLBoldLeft";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "204 34";
      Extent = "48 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = %subLabel2;
      maxLength = "1024";
   };
   
   if (%useWords)
   {
      %editControl1.word = 0;
      %editControl2.word = 1;
   }
   
   %editControl1.setProperty(%this.object);
   %editControl1.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl1 @ ".updateProperty(" @ %editControl1 @ ".object" @ ");";
   %editControl1.validate = %editControl1 @ ".updateProperty(" @ %editControl1 @ ".object" @ ");";
   
   %editControl2.setProperty(%this.object);
   %editControl2.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl2 @ ".updateProperty(" @ %editControl2 @ ".object" @ ");";
   %editControl2.validate = %editControl2 @ ".updateProperty(" @ %editControl2 @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%subLabelControl1);
   %container.add(%subLabelControl2);
   %container.add(%editControl1);
   %container.add(%editControl2);
   
   %this.addProperty(%editControl1, %spatial);
   %this.addProperty(%editControl2, %spatial);
   %this.add(%container);
   return %container;
}
