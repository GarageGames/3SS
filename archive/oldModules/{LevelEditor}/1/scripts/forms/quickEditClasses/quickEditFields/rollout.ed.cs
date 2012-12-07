//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createRolloutStack(%this, %label, %expanded)
{
   if (%expanded $= "")
      %expanded = false;
   
   %base = new GuiRolloutCtrl() 
   {
      class = LBQuickEditRollout;
      canSaveDynamicFields = "0";
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "323 0";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      Caption = %label;
      Margin = "7 4";
      DragSizable = false;
      container = true;
   };
  
   %stack = new GuiStackControl() 
   {
      class = "LBQuickEditRolloutStack";
      superClass = "LBQuickEditContent";
      StackingType = "Vertical";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "0";
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "300 24";
      MinExtent = "24 24";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      rolloutCtrl = %base;
      object = %this.object;
      container = true;
   };
   %base.add( %stack );
   
   %stack.spatialProperties = new SimSet();
   %stack.statusCheck = new SimSet();
   %stack.properties = new SimSet();

   $LB::QuickEditGroup.add( %stack.spatialProperties );
   $LB::QuickEditGroup.add( %stack.statusCheck );
   $LB::QuickEditGroup.add( %stack.properties );
   if (%expanded)
      %base.instantExpand();
   else
      %base.instantCollapse();
   
   %this.addStatusCheck(%stack);
   %this.addProperty(%stack, true);
   %this.add(%base);
   return %stack;
}

function LBQuickEditRollout::updateSpatial(%this, %object)
{
   %this.getObject(0).updateSpatial(%object);
}

function LBQuickEditRollout::onCollapsed( %this )
{
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "resize");
}

function LBQuickEditRollout::onExpanded( %this )
{
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "resize");
}

function LBQuickEditRolloutStack::setProperty(%this, %object)
{
   %count = %this.properties.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %control = %this.properties.getObject(%i);
      %control.object = %object;
      %control.setProperty(%this.object);
   }
}
