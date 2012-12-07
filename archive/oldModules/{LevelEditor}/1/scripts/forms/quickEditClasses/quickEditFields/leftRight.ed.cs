//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createLeftRight(%this, %accessor1, %accessor2, %label, %tooltip)
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
   
   %leftButton = new GuiButtonCtrl() {
      class = QuickEditLeftRightButton;
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
      accessor = %accessor1;
      reverseAccessor = %accessor2;
      undoLabel = %label;
      object = %this.object;
   };
   
   %leftButton.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %leftButton @ ".updateProperty(" @ %leftButton @ ".object" @ ");";
   
   %rightButton = new GuiButtonCtrl() {
      class = QuickEditLeftRightButton;
      internalName = %property @ "RightButton";
      canSaveDynamicFields = "0";
      Profile = "EditorButtonRight";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "150 7";
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
      accessor = %accessor2;
      reverseAccessor = %accessor1;
      undoLabel = %label;
      object = %this.object;
   };
   
   %rightButton.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %rightButton @ ".updateProperty(" @ %rightButton @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%leftButton);
   %container.add(%rightButton);
   
   %this.addProperty(%leftButton);
   %this.addProperty(%rightButton);
   
   %this.add(%container);
   return %container;
}

function QuickEditLeftRightButton::setProperty(%this, %object)
{
}

function QuickEditLeftRightButton::updateProperty(%this, %object)
{
   %result = %object;
   eval("%result = %object." @ %this.accessor @ "();");
   if (%result)
   {
      %undo = new UndoScriptAction()
      {
         actionName = "Changed Layer Ordering";
         class = UndoLRQuickEdit;
         reverseCommand = %this.reverseAccessor;
         command = %this.accessor;
         object = %object;
      };
      %undo.addToManager(LevelBuilderUndoManager);
   }
}

function UndoLRQuickEdit::undo(%this)
{
   %command = "%this.object." @ %this.reverseCommand @ "();";
   eval(%command);
}

function UndoLRQuickEdit::redo(%this)
{
   %command = "%this.object." @ %this.command @ "();";
   eval(%command);
}
