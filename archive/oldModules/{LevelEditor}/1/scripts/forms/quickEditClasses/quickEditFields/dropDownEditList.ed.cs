//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createDropDownEditList(%this, %accessor, %label, %set, %additionalItems, %tooltip, %addUndo)
{
   if (%addUndo $= "")
      %addUndo = true;
      
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 35";
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
   %editList = new GuiPopUpMenuCtrlEx() 
   {
      canSaveDynamicFields = "0";
      DatablockFilter = %filter;
      Profile = "GuiPopupMenuProfile";
      Class = "QuickEditDropDownEditList";
      internalName = %property @ "DropDown";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "128 7";
      Extent = "152 20";
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
      precision = "TEXT";
      watchSet = %set;
      additionalItems = %additionalItems;
      addUndo = %addUndo;
   };
   %editList.refresh();
   
   %editControl = new GuiTextEditCtrl() {
      class = QuickEditDropDownTextEdit;
      internalName = %accessor @ "TextEdit";
      canSaveDynamicFields = "0";
      Profile = "EditorDropDownTextEdit";
      HorizSizing = "width";
      VertSizing = "bottom";
      Position = "131 8";
      Extent = "130 16";
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
      precision = "TEXT";
      accessor = %accessor;
      undoLabel = %label;
      object = %this.object;
      watchSet = %set;
      dropDown = %editList;
   };
   
   %editControl.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   %editControl.validate = %editControl @ ".updateProperty(" @ %editControl @ ".object" @ ");";
   
   %editList.textEdit = %editControl;
   %editList.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %editList @ ".updateProperty(" @ %editList @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%editList);
   %container.add(%editControl);
   
   %editList.setProperty(%this.object);
   %this.addProperty(%editList);
   %this.add(%container);
   return %container;
}

function QuickEditDropDownTextEdit::updateProperty(%this, %object)
{
   %text = %this.getText();
   if (isObject(%this.watchset))
   {
      TileEditSet::addText(%this.watchSet, %text);
   }
   else
   {
      if (trim(%text) $= "") %testText = "BLANK";
      else %testText = %text;
      if (fieldpos(%this.dropDown.additionalItems, %testText, 0) == -1)
         %this.dropDown.additionalItems = %testtext @"\t"@ %this.dropdown.additionalItems;
   }
   %this.dropDown.refresh();
   %index = %this.dropDown.findText(%text);
   %this.dropDown.setSelected(%index);
   %this.dropDown.updateProperty(%object);
}

function QuickEditDropDownTextEdit::setProperty(%this, %object)
{
   %text = %this.getText();
   %index = %this.dropDown.findText(%text);
   %this.dropDown.setSelected(%index);
}

function QuickEditDropDownEditList::refresh(%this)
{
   
   %this.clear();
   if (isObject(%this.watchSet))
   {
      %count = %this.watchSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %item = %this.watchSet.getObject(%i);
         %this.add(%item.displayName, %i);
      }
   }
   %this.sort();
   
   %additionalCount = getFieldCount(%this.additionalItems);
   for (%i = 0; %i < %additionalCount; %i++)
   {
      %item = getField(%this.additionalItems, %i);
      if (%item $= "BLANK")
         %item = "";
      %this.add(%item, %i + %count);
   }
}

function QuickEditDropDownEditList::setProperty(%this, %object)
{
   %this.refresh();
   
   %value = QuickEditField::getObjectValue(%this, %object);
   %index = %this.findText(%value);
   if (isObject (%this.textEdit))
   {
      %this.textEdit.setText(%value);
   }
   %this.setSelected(%index);
}

function QuickEditDropDownEditList::updateProperty(%this, %object)
{
   %value = %this.getTextById(%this.getSelected());
   %oldValue = QuickEditField::getObjectValue(%this, %object);
//   if (%oldValue $= "")
//      %oldValue = "-";
   QuickEditField::updateProperty(%this, %object, %this.getTextById(%this.getSelected()), %oldValue);
}

function QuickEditDropDownEditList::onSelect(%this, %item)
{
   %text = %this.getTextById(%item);
   if (isObject (%this.textEdit))
   {
      %this.textEdit.setText(%text);
   }
}
