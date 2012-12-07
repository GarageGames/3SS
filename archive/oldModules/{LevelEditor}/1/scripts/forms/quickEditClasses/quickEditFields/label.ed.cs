//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createLabel(%this, %label, %doEvalLabel, %profile )  //profile applied to guiTextCtrl
{
   if (%profile $= "") %profile = "EditorFontHLLarge";
   
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
   };

   %labelControl = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = %profile;
      HorizSizing = "width";
      VertSizing = "height";
      Position = "16 2";
      Extent = "300 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "";
      label = %label;  //lets you put in a string OR a piece of script to be eval'ed
      doEval = %doEvalLabel;
      class = "QuickEditLabel";
      maxLength = "1024";
   };
   
   %container.add(%labelControl);
   %labelControl.setProperty(%this.object);
   %this.addproperty(%labelControl);
   %this.add(%container);
   return %container;
}

function quickEditLabel::setProperty(%this, %object)
{
   if (%this.doEval)
   {
      %command = "%text = " @ %this.label @ ";";   
      eval( %command );
   }
   else
      %text = %this.label;
   %this.setText( %text );
}
