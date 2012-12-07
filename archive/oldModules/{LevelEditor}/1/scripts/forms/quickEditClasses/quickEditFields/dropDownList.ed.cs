//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createDropDownList(%this, %accessor, %label, %set, %additionalItems, %tooltip, %addUndo, %isProperty)
{
   if (%isProperty $= "")
      %isProperty = false;
   
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
   %imageMapList = new GuiPopUpMenuCtrlEx() 
   {
      canSaveDynamicFields = "0";
      DatablockFilter = %filter;
      Profile = "GuiPopupMenuProfile";
      Class = "QuickEditDropDownList";
      internalName = %accessor @ "DropDown";
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
      watchSet = %set;
      additionalItems = %additionalItems;
      addUndo = %addUndo;
      isProperty = %isProperty;
   };
   
   %imageMapList.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %imageMapList @ ".updateProperty(" @ %imageMaplist @ ".object" @ ");";
   
   %container.add(%labelControl);
   %container.add(%imageMapList);
   
   %imageMapList.setProperty(%this.object);
   %this.addProperty(%imageMapList);
   %this.add(%container);
   return %container;
}

function QuickEditDropDownList::refresh(%this)
{
   %this.clear();
   
   if (isObject(%this.watchSet))
   {
      %count = %this.watchSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %item = %this.watchSet.getObject(%i);
         %this.add(%item.getName(), %i);
      }
      
      %this.sort();
   }
   
   %additionalCount = getFieldCount(%this.additionalItems);
   for (%i = 0; %i < %additionalCount; %i++)
   {
      %item = getField(%this.additionalItems, %i);
      if (%item $= "BLANK")
         %item = " ";
      %this.add(%item, %i + %count);
   }
}

function QuickEditDropDownList::setProperty(%this, %object)
{
   %this.refresh();
   
   %value = QuickEditField::getObjectValue(%this, %object);
   %index = %this.findText(%value);
   %this.setSelected(%index);
}

function QuickEditDropDownList::updateProperty(%this, %object)
{
   %value = %this.getTextById(%this.getSelected());
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   if (%oldValue $= "")
      %oldValue = "-";
   
   QuickEditField::updateProperty(%this, %object, %this.getTextById(%this.getSelected()), %oldValue);
}
