//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createHideableStack(%this, %hiddenCheck)
{
   %stack = new GuiStackControl() {
      StackingType = "Vertical";
      class = "LBQuickEditHideableStack";
      superClass = "LBQuickEditContent";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "0";
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "300 250";
      MinExtent = "150 10";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      object = %this.object;
      parent = %this;
      placement = %this.getCount();
      hiddenCheck = %hiddenCheck;
      container = true;
   };
   
   %stack.spatialProperties = new SimSet();
   %stack.statusCheck = new SimSet();
   %stack.properties = new SimSet();
   $LB::QuickEditGroup.add( %stack.spatialProperties );
   $LB::QuickEditGroup.add( %stack.statusCheck );
   $LB::QuickEditGroup.add( %stack.properties );
   %this.addStatusCheck(%stack);
   %this.addProperty(%stack, true);
   %this.add(%stack);
   return %stack;
}

function LBQuickEditHideableStack::setProperty(%this, %object)
{
   %count = %this.properties.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %control = %this.properties.getObject(%i);
      %control.object = %object;
      %control.setProperty(%this.object);
   }
}

function LBQuickEditHideableStack::updateFields(%this)
{
   eval("%hide = " @ %this.hiddenCheck);
   
   if (%hide)
   {
      if (%this.parent.isMember(%this))
      {
         %this.parent.remove(%this);
         $LB::QuickEditGroup.add( %this );
         GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "resize");
      }
   }
   else
   {
      if (!%this.parent.isMember(%this))
      {
         %this.parent.add(%this);
         
         while (%this.placement >= %this.parent.getCount())
            %this.placement--;
         
         %this.parent.reOrderChild(%this, %this.parent.getObject(%this.placement));
         ToolManager.getLastWindow().setFirstResponder();
         GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "resize");
      }
   }
}

function LBQuickEditHideableStack::addControlDependency(%this, %control)
{
   %control.hideControl = %control.hideControl SPC %this;
}
