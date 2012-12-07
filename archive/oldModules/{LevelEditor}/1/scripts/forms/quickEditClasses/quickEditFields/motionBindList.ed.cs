//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$MotionBindMenu::accelerList = "accelx" TAB "accely" TAB "accelz" TAB 
                               "gravityx" TAB "gravityy" TAB "gravityz";
                  
$MotionBindMenu::gyroList =  "gyrox" TAB "gyroy" TAB "gyroz" TAB 
                             "yaw" TAB "pitch" TAB "roll";
                  
function LBQuickEditContent::createMotionBindList(%this, %accessor, %label, %tooltip, %addUndo, %isProperty)
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
      Class = "QuickEditMotionBindList";
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

function QuickEditMotionBindList::refresh(%this)
{
   %this.clear();
   
   %accelCount = getFieldCount($MotionBindMenu::accelerList);
   %this.addCategory("Accelerometer");
   for (%i = 0; %i < %accelCount; %i++)
   {
      %key = getField($MotionBindMenu::accelerList, %i);
      %this.add(%key, %i);
   }
   
   %this.accelCount = %accelCount;
   
   %gyroCount = getFieldCount($MotionBindMenu::gyroList);
   %this.addCategory("Gyroscope");
   for (%i = 0; %i < %gyroCount; %i++)
   {
      %key = getField($MotionBindMenu::gyroList, %i);
      %this.add(%key, %i + %accelCount);
   }
   
   %this.gyroCount = %gyroCount;
}

function QuickEditMotionBindList::setProperty(%this, %object)
{
   %this.refresh();
   
   %value = QuickEditField::getObjectValue(%this, %object);
   %key = getWord(%value, 1);
   %index = %this.findText(%key);
   %this.setSelected(%index);
}

function QuickEditMotionBindList::updateProperty(%this, %object)
{
   %device = "";
   %id = %this.getSelected();
   if (%id < %this.accelCount)
      %device = "Accelerometer";
   else if (%id < %this.accelCount + %this.gyroCount)
      %device = "Gyroscope";
   %value = %device SPC %this.getTextById(%id);
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   if (%oldValue $= "")
      %oldValue = "-";
   
   QuickEditField::updateProperty(%this, %object, %value, %oldValue);
}
