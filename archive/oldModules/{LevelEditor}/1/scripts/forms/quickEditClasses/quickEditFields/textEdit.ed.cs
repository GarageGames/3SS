//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createTextEdit(%this, %accessor, %precision, %label, %tooltip, %spatial)
{
   %extent = 64;
   if (%precision $= "TEXT")
      %extent = 200;
   
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 24";
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
      Position = "16 3";
      Extent = "100 18";
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
      internalName = %accessor @ "TextEdit";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "128 1";
      Extent = %extent SPC "22";
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
      precision = %precision;
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
      useWords = false;
   };
   
   %editControl.setProperty(%this.object);
   %editControl.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   %editControl.validate = %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%editControl);
   
   %this.addProperty(%editControl, %spatial);
   %this.add(%container);
   return %container;
}


function LBQuickEditContent::createTextEditProperty(%this, %accessor, %precision, %label, %tooltip, %spatial)
{
   %extent = 64;
   if (%precision $= "TEXT")
      %extent = 200;
      
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
   };

   %labelControl = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHLBold";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16 3";
      Extent = "100 18";
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
      internalName = %accessor @ "TextEdit";
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "100 1";
      Extent = %extent SPC "22";
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
      precision = %precision;
      accessor = %accessor;
      isProperty = true;
      undoLabel = %label;
      object = %this.object;
      useWords = false;
   };
   
   %editControl.setProperty(%this.object);
   %editControl.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   %editControl.validate = %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%editControl);
   
   %this.addProperty(%editControl, %spatial);
   %this.add(%container);
   return %container;
}


//----------------------------
//-Mat add button 
function LBQuickEditContent::addButton(%this, %label, %callBack, %tooltip, %extent, %position )
{
   
   if(!%extent)
   {
       %extent = "200 24";
   }
   
   if(!%position)
   {
      %position = "10 0";
   }
   
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 30";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
   };

   %button = new GuiButtonCtrl() {
      canSaveDynamicFields = "0";
      Profile = "GuiButtonProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = %position;
      Extent = %extent;
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      Command = %callBack;
      Text = %label;
   };
   
   %container.add( %button );
   %this.add(%container);
   return %button;
}
//----------------------------

function QuickEditTextEdit::setProperty(%this, %object)
{
   %this.Text = QuickEditField::getObjectValue(%this, %object);
}

function QuickEditTextEdit::updateProperty(%this, %object)
{
   QuickEditField::updateProperty(%this, %object, %this.getText());
}
