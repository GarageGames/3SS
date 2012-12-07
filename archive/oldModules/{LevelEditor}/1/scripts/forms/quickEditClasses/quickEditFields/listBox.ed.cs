//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createListBox(%this, %accessor, %isProperty, %label, %items, %tooltip, %sort )
{
   if( %sort $= "" )
      %sort = true;
      
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
   
   %listBox = new GuiPopUpMenuCtrlEx() {
      canSaveDynamicFields = "0";
      class = QuickEditListBox;
      internalName = %accessor @ "List";
      Profile = "GuiPopupMenuProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      Position = "128 7";
      Extent = "152 20";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      tooltipprofile = "EditorToolTipProfile";
      ToolTip = %tooltip;
      hovertime = "100";
      maxLength = "1024";
      maxPopupHeight = "200";
      accessor = %accessor;
      isProperty = %isProperty;
      undoLabel = %label;
      object = %this.object;
   };
   
   %listBox.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %listBox @ ".updateProperty(" @ %listBox @ ".object" @ ");";
   for (%i = 0; %i < getFieldCount(%items); %i++)
      %listBox.add(getField(%items, %i), %i);
   
   %listBox.setProperty(%this.object);
   if( %sort )
      %listBox.sort();
   %container.add(%labelControl);
   %container.add(%listBox);
   
   %this.addProperty(%listBox);
   %this.add(%container);
   return %container;
}

function QuickEditListBox::setProperty(%this, %object)
{
   %this.setSelected(%this.findText(QuickEditField::getObjectValue(%this, %object)));
}

function QuickEditListBox::updateProperty(%this, %object)
{
   QuickEditField::updateProperty(%this, %object, %this.getTextById(%this.getSelected()));
}
