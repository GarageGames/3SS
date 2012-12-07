//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createMask(%this, %accessor, %label, %start, %end, %tooltip)
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
   
   %stack = new GuiDynamicCtrlArrayControl() 
   {
      canSaveDynamicFields = "0";
      class = QuickEditMaskStack;
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      MinExtent = "16 16";
      canSave = "1";
      Visible = "1";
      internalName = "objectList";
      hovertime = "1000";
      colCount = "0";
      colSize = "16";
      rowSize = "16";
      rowSpacing = "2";
      colSpacing = "2";
      Position = "128 7";
      Extent = "110 0";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      start = %start;
      end = %end;
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
      precision = "TEXT";
   };
   
   %container.add(%labelControl);
   %container.add(%stack);
   
   for (%i = %start; %i <= %end; %i++)
   {
      %button = new GuiButtonCtrl() {
         class = QuickEditMaskButton;
         internalName = "MaskButton" @ %i;
         hovertime = "100";
         tooltip = %tooltip;
         tooltipProfile = "EditorToolTipProfile";
         accessor = %accessor;
         undoLabel = %label;
         canSaveDynamicFields = "0";
         Profile = "EditorButton";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "0 0";
         Extent = "16 16";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         groupNum = "-1";
         sizeIconToButton = "0";
         textLocation = "Center";
         textMargin = "4";
         buttonMargin = "4 4";        
         buttonType = "ToggleButton";
         text = %i;
         index = %i;
         parent = %stack;
         object = %this.object;
      };
      %button.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %button @ ".updateProperty(" @ %button @ ".object" @ ");";
      %stack.add(%button);
   }
   
//ALL/NONE BUTTONS   
   %allbutton = new GuiButtonCtrl() {
      class = QuickEditMaskAllButton;
      internalName = "MaskAllButton";
      hovertime = "100";
      tooltip = "Activate All";
      tooltipProfile = "EditorToolTipProfile";
      accessor = %accessor;
      undoLabel = %label;
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "164 97";
      Extent = "34 16";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      groupNum = "-1";
      sizeIconToButton = "0";
      textLocation = "Center";
      buttonType = "PushButton";
      text = "All";
//      index = %i;
      parent = %stack;
      object = %this.object;
   };
   
   %allbutton.command = %allbutton @ ".maskAll(" @ %stack @ ".object" @ ");";
   %container.add(%allButton);

   %nonebutton = new GuiButtonCtrl() {
      class = QuickEditMaskNoneButton;
      internalName = "MaskNoneButton";
      hovertime = "100";
      tooltip = "Deactivate All";
      tooltipProfile = "EditorToolTipProfile";
      accessor = %accessor;
      undoLabel = %label;
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "200 97";
      Extent = "34 16";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      groupNum = "-1";
      sizeIconToButton = "0";
      textLocation = "Center";
      textMargin = "4";
      buttonMargin = "4 4";        
      buttonType = "PushButton";
      text = "None";
//      index = %i;
      parent = %stack;
      object = %this.object;
   };
   
   %nonebutton.command = %nonebutton @ ".maskNone(" @ %stack @ ".object" @ ");";
   %container.add(%noneButton);   
//END ALL/NONE BUTTONS


   %container.setExtent(getWord(%container.getExtent(), 0), getWord(%stack.getExtent(), 1) + (getWord(%stack.getPosition(), 1) * 2));
   
   %this.addProperty(%stack);
   %this.add(%container);
   return %container;
}

function QuickEditMaskAllButton::maskAll(%this, %object)
{
//   %oldValue = QuickEditField::getObjectValue(%this, %object);
   %newValue = "0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31";
   QuickEditField::updateProperty(%this.parent, %object, trim(%newValue));
}   

function QuickEditMaskNoneButton::maskNone(%this, %object)
{
//   %oldValue = QuickEditField::getObjectValue(%this, %object);
   %newValue = "";
   QuickEditField::updateProperty(%this.parent, %object, trim(%newValue));
}  

function QuickEditMaskStack::setProperty(%this, %object)
{
   %value = QuickEditField::getObjectValue(%this, %object);
   
   // Turn all the buttons off.
   for (%i = %this.start; %i <= %this.end; %i++)
   {
      %button.object = %object;
      %button = %this.findObjectByInternalName("MaskButton" @ %i);
      %button.setStateOn(false);
   }
   
   // Then turn those on that are in the mask.
   %count = getWordCount(%value);
   for (%i = 0; %i < %count; %i++)
   {
      %button.object = %object;
      %index = getWord(%value, %i);
      %button = %this.findObjectByInternalName("MaskButton" @ %index);
      %button.setStateOn(true);
   }
}

function QuickEditMaskButton::updateProperty(%this, %object)
{
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   %newValue = %oldValue;
   %set = %this.getValue();

   %found = false;
   %count = getWordCount(%oldValue);
   for (%i = 0; %i < %count; %i++)
   {
      %word = getWord(%oldValue, %i);
      if (%word == %this.index)
      {
         %found = true;
         if (!%set)
         {
            %newValue = removeWord(%newValue, %i);
            break;
         }
      }
   }
   
   if (%set && !%found)
      %newValue = %newValue SPC %this.index;
   
   QuickEditField::updateProperty(%this.parent, %object, trim(%newValue));
}
