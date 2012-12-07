//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createBehaviorStack(%this)
{
   %stack = new GuiStackControl() {
      StackingType = "Vertical";
      class = "QuickEditBehaviorStack";
      superclass = "LBQuickEditContent";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "300 250";
      MinExtent = "150 10";
      object = %this.object;
      rollout = %this.rolloutCtrl;
   };
   %stack.properties = new SimSet();
   
   %this.addProperty(%stack);
   %this.add(%stack);
   return %stack;
}

function QuickEditBehaviorStack::createBehaviorRollout(%this, %behavior)
{
   %template = %behavior.template;
   %name = %template.friendlyName;
   
   %rollout = new GuiRolloutCtrl() 
   {
      class = "BehaviorQuickEditRollout";
      superclass = LBQuickEditRollout;
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "323 0";
      Caption = %name;
      Margin = "7 4";
      DragSizable = false;
      container = true;
      parentRollout = %this.rollout;
   };
   
   %container = new GuiStackControl()
   {
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 36";
      Extent = "300 4";
   };
   
   %button = new GuiIconButtonCtrl()
   {
      class = RemoveBehaviorButton;
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "10 5";
      Extent = "192 22";
      iconBitmap = "modules/levelEditor/gui/images/iconDelete.png";
      text = "Remove This Behavior";
      buttonMargin = "4 4";
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Center";
      textMargin = "4";
      hovertime = "100";
      tooltip = "Remove this Behavior from the object";
      tooltipProfile = "EditorToolTipProfile";
      object = %this.object;
      behavior = %behavior;
   };
   
   %fieldContainer = new GuiStackControl()
   {
      class = "BehaviorFieldStack";
      superclass = "LBQuickEditContent";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 36";
      Extent = "300 4";
      object = %behavior;
   };
   %fieldContainer.properties = new SimSet();
   
   %count = %template.getBehaviorFieldCount();
   for( %i = 0; %i < %count; %i++ )
      BehaviorEditor::createFieldGui(%fieldContainer, %behavior, %i);

   %container.add( %fieldContainer );
   %container.add( %button );
   %rollout.add( %container );
   
   %this.addProperty( %fieldContainer );
   %this.add(%rollout);
   
   return %container;
}

function BehaviorQuickEditRollout::onCollapsed( %this )
{
   // Force resize
   %this.parentRollout.instantCollapse();
   %this.parentRollout.instantExpand();
}

function BehaviorQuickEditRollout::onExpanded( %this )
{
   // Force resize
   %this.parentRollout.instantCollapse();
   %this.parentRollout.instantExpand();
}

function RemoveBehaviorButton::onClick( %this )
{
   ToolManager.getLastWindow().setFirstResponder();

   %undo = new UndoScriptAction()
   {
      actionName = "Removed Behavior";
      class = UndoRemoveBehavior;
      object = %this.object;
      behavior = %this.behavior;
   };
   %this.object.removeBehavior(%this.behavior, false);
   %undo.addToManager(LevelBuilderUndoManager);
   
   schedule( 0, 0, updateQuickEdit );
}

function QuickEditBehaviorStack::setProperty(%this, %object)
{
   while (%this.getCount())
      %this.getObject(0).delete();
   
   %count = %object.getBehaviorCount();
   for ( %i = 0; %i < %count; %i++ )
   {
      %behaviorInstance = %object.getBehaviorByIndex(%i);
      %this.createBehaviorRollout( %behaviorInstance );
   }
   
   %this.object = %object;
   %count = %this.properties.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %control = %this.properties.getObject(%i);
      %control.object = %object;
      %control.setProperty(%object);
   }
}

function BehaviorFieldStack::setProperty( %this, %object )
{
   %count = %this.properties.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %control = %this.properties.getObject(%i);
      %control.setProperty(%control.object);
   }
}
