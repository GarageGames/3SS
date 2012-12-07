//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$KeyBindMenu::keyList = "A" TAB "B" TAB "C" TAB "D" TAB "E" TAB "F" TAB "G" TAB
                        "H" TAB "I" TAB "J" TAB "K" TAB "L" TAB "M" TAB "N" TAB
                        "O" TAB "P" TAB "Q" TAB "R" TAB "S" TAB "T" TAB "U" TAB
                        "V" TAB "W" TAB "X" TAB "Y" TAB "Z" TAB "backspace" TAB
                        "tab" TAB "enter" TAB "pause" TAB "escape" TAB
                        "space" TAB "pagedown" TAB "pageup" TAB "end" TAB
                        "home" TAB "left" TAB "right" TAB "up" TAB "down" TAB
                        "print" TAB "insert" TAB "delete" TAB "numpad0" TAB
                        "numpad1" TAB "numpad2" TAB "numpad3" TAB "numpad4" TAB
                        "numpad5" TAB "numpad6" TAB "numpad7" TAB "numpad8" TAB
                        "numpad9" TAB "numpadmult" TAB "numpadadd" TAB
                        "numpadsep" TAB "numpadminus" TAB "numpaddecimal" TAB
                        "numpaddivide" TAB "numpadenter" TAB "f1" TAB "f2" TAB
                        "f3" TAB "f4" TAB "f5" TAB "f6" TAB "f7" TAB "f8" TAB
                        "f9" TAB "f10" TAB "f11" TAB "f12" TAB "minus" TAB
                        "equals" TAB "lbracket" TAB "rbracket" TAB
                        "backslash" TAB "semicolon" TAB "apostrophe" TAB
                        "comma" TAB "period" TAB "slash";

$KeyBindMenu::joystickList = "button0" TAB "button1" TAB "button2" TAB
                             "button3" TAB "button4" TAB "button5" TAB
                             "button6" TAB "button7" TAB "Button8" TAB
                             "Button9" TAB "Button10" TAB "Button11" TAB
                             "Button12" TAB "upov" TAB "dpov" TAB "rpov" TAB
                             "lpov" TAB "xaxis" TAB "yaxis" TAB "zaxis" TAB
                             "rxaxis" TAB "ryaxis" TAB "rzaxis";

function LBQuickEditContent::createKeyBindList(%this, %accessor, %label, %tooltip, %addUndo, %isProperty)
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
      Class = "QuickEditKeyBindList";
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

function QuickEditKeyBindList::refresh(%this)
{
   %this.clear();
   
   %keyCount = getFieldCount($KeyBindMenu::keyList);
   %this.addCategory("Keyboard");
   for (%i = 0; %i < %keyCount; %i++)
   {
      %key = getField($KeyBindMenu::keyList, %i);
      %this.add(%key, %i);
   }
   
   %this.keyCount = %keyCount;
   
   %mouseCount = 0;
   for (%i = 0; %i < %mouseCount; %i++)
   {
      %key = getField($KeyBindMenu::mouseList, %i);
      %this.add(%key, %i + %keyCount);
   }
   
   %this.mouseCount = %mouseCount;
   
   %joystickCount = getFieldCount($KeyBindMenu::joystickList);
   %this.addCategory("Joystick");
   for (%i = 0; %i < %joystickCount; %i++)
   {
      %key = getField($KeyBindMenu::joystickList, %i);
      %this.add(%key, %i + %keyCount + %mouseCount);
   }
   
   %this.joystickCount = %joystickCount;
}

function QuickEditKeyBindList::setProperty(%this, %object)
{
   %this.refresh();
   
   %value = QuickEditField::getObjectValue(%this, %object);
   %key = getWord(%value, 1);
   %index = %this.findText(%key);
   %this.setSelected(%index);
}

function QuickEditKeyBindList::updateProperty(%this, %object)
{
   %device = "";
   %id = %this.getSelected();
   if (%id < %this.keyCount)
      %device = "keyboard";
   else if (%id < %this.keyCount + %this.mouseCount)
      %device = "mouse0";
   else if (%id < %this.keyCount + %this.mouseCount + %this.joystickCount)
      %device = "joystick0";
   %value = %device SPC %this.getTextById(%id);
   %oldValue = QuickEditField::getObjectValue(%this, %object);
   if (%oldValue $= "")
      %oldValue = "-";
   
   QuickEditField::updateProperty(%this, %object, %value, %oldValue);
}
