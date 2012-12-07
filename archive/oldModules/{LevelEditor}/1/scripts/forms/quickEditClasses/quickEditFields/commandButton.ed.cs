//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createCommandButton(%this, %command, %label, %fakeNamespace, %xposition, %xsize)
{
   %container = "";
   if ((%xposition !$= "") || (%xsize !$= ""))
   {
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
   }
   
   if (%xposition $= "")
      %xposition = 10;
   if (%xsize $= "")
      %xsize = 192;
      
   %checkBox = new GuiIconButtonCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = %xposition SPC "3";
      Extent = %xSize SPC "24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      text = %label;
      groupNum = "-1";
      buttonType = "PushButton";
      buttonMargin = "4 4";
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Center";
      textMargin = "4";      
   };
   
   if( %fakeNamespace !$= "" )
      %checkBox.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %fakeNamespace @ "::onClick(" @ %checkBox @ ");";
   else
      %checkBox.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %command;
   
   if (isObject(%container))
   {
      %container.add(%checkBox);
      %this.add(%container);
      return %container;
   }
   
   %this.add(%checkBox);
   return %checkBox;
}
