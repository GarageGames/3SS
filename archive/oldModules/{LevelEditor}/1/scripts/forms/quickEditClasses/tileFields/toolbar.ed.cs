//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createToolbar(%this)
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 44";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
   };
   
   %toolbar = new GuiStackControl() {
      StackingType = "Horizontal";
      class = QuickEditToolbar;
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "0";
      canSaveDynamicFields = "0";
      Profile = "EditorButtonLeft";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 10";
      Extent = "0 24";
      MinExtent = "24 24";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };
   
   %container.add(%toolbar);
   %this.add(%container);
   
   return %toolbar;
}

function QuickEditToolbar::addSpacer(%this)
{
   %spacer = new GuiSeparatorCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "GuiSeparatorProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "14 24";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      type = "Vertical";
      Invisible = true;
      BorderMargin = "3";
      LeftMargin = "0";
   };

   // Add spacer to toolbar
   %this.add( %spacer );
}

function QuickEditToolbar::addTool(%this, %name, %command, %texture)
{
   %button = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButtonToolbar";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "24 24";
      MinExtent = "8 2";
      canSave = "1";
      visible = "1";
      Command = %command;
      tooltipprofile = "GuiToolTipProfile";
      ToolTip = %name;
      hovertime = "100";
      textLocation = "None";
      textMargin = "4";
      buttonMargin = "4 4";     
      groupNum = "4829";
      buttonType = "RadioButton";
      iconBitmap = %texture;
   };

   // Add to toolbar
   %this.add( %button );
   
   return %button;
}
