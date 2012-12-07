//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditClass::createFloatWindow(%this, %objectName, %object, %label )
{
   %base = new GuiWindowCtrl( %objectName )
    {
      profile = "EditorToolWindowProfile";
      horizSizing = "right";
      vertSizing = "bottom";
      position = "0 74";
      extent = "156 200";
      minExtent = "140 200";
      visible = "1";
      text = %label;
      maxLength = "255";
      resizeWidth = "0";
      resizeHeight = "1";
      canMove = "1";
      canClose = "0";
      canMinimize = "0";
      canMaximize = "0";
      container=true;
    };
   %scroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorScrollProfile";
      class = "GuiThumbnailArray";
      internalName = "thumbnailScroll";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "5 13";
      Extent = "146 178";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "1";
      Margin = "6 2";
   };
   %base.add(%scroll);
  
   %stack = new GuiStackControl() 
   {
      class = "LBQuickEditFloatStack";
      superClass = "LBQuickEditContent";
      StackingType = "Vertical";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "1";
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "278 24";
      MinExtent = "24 24";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      base = %base;
      object = %object;
      container = true;
   };
   %scroll.add( %stack );
   
   %stack.spatialProperties = new SimSet();
   %stack.statusCheck = new SimSet();
   %stack.properties = new SimSet();

   $LB::QuickEditGroup.add( %stack.spatialProperties );
   $LB::QuickEditGroup.add( %stack.statusCheck );
   $LB::QuickEditGroup.add( %stack.properties );
   
   %base.stack = %stack;
   return %stack;
}

function LBQuickEditFloatStack::setProperty(%this, %object)
{
   %count = %this.properties.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %control = %this.properties.getObject(%i);
      %control.object = %object;
      %control.setProperty(%this.object);
   }
}

