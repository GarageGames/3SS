//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createEnumList(%this, %accessor, %isProperty, %label, %tooltip, %enumClass, %enum )
{
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
      class = QuickEditEnumList;
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
   %listBox.setEnumContent(%enumClass, %enum);
   
   %container.add(%labelControl);
   %container.add(%listBox);
   
   %this.addProperty(%listBox);
   %this.add(%container);
   return %container;
}

function QuickEditEnumList::setProperty(%this, %object)
{
   %this.setSelected(%this.findText(QuickEditField::getObjectValue(%this, %object)));
}

function QuickEditEnumList::updateProperty(%this, %object)
{
   QuickEditField::updateProperty(%this, %object, %this.getTextById(%this.getSelected()));
}
