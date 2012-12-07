//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createSpacer(%this, %height)
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300" SPC %height;
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
   };
   
   %this.add(%container);
   return %container;
}
