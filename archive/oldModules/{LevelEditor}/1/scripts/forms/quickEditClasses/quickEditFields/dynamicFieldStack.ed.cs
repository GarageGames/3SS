//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LBQuickEditContent::createDynamicFieldStack(%this)
{
   %stack = new GuiStackControl() {
      StackingType = "Vertical";
      class = "LBQuickEditDynamicFieldStack";
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
   };
   
   %this.addProperty(%stack);
   %this.add(%stack);
   return %stack;
}

function LBQuickEditDynamicFieldStack::createDynamicField(%this, %name, %value)
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 32";
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
      text = %name;
      maxLength = "1024";
   };
   
   %editControl = new GuiTextEditCtrl() {
      class = DynamicFieldTextEdit;
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "128 5";
      Extent = "64 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      text = %value;
      oldValue = %value;
      base = %this;
   };
   
   %button = new GuiIconButtonCtrl() {
      canSaveDynamicFields = "0";
      class  = LBQuickEditDeleteFieldButton;
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "200 5";
      Extent = "22 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      iconBitmap = "^{EditorAssets}/gui/iconDelete.png";
      sizeIconToButton = "1";
      fieldEdit = %editControl;
      base = %this;
   };
   
   %editControl.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %editControl @ ".updateProperty(" @ %this @ ".object" @ ", " @ %name @ ");";
   %editControl.validate = %editControl @ ".updateProperty(" @ %this @ ".object" @ ", " @ %name @ ");";
   %button.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %button @ ".deleteField(" @ %this @ ".object" @ ", " @ %name @ ");";
   
   %container.add(%labelControl);
   %container.add(%editControl);
   %container.add(%button);
   
   %this.add(%container);
   return %container;
}

function LBQuickEditDynamicFieldStack::createAddDynamicField(%this, %name, %value)
{
   %container = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorContainerProfile";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "300 42";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
   };

   %nameControl = new GuiTextEditCtrl() {
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16 16";
      Extent = "96 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      text = "Field Name";
   };
   
   %valueControl = new GuiTextEditCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorTextEdit";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "128 16";
      Extent = "64 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      maxLength = "1024";
      historySize = "0";
      password = "0";
      tabComplete = "0";
      sinkAllKeyEvents = "0";
      password = "0";
      passwordMask = "*";
      text = "Field Value";
   };
   
   %button = new GuiIconButtonCtrl() {
      canSaveDynamicFields = "0";
      class  = LBQuickEditAddFieldButton;
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "200 16";
      Extent = "22 22";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      iconBitmap = "^{EditorAssets}/gui/iconAdd.png";
      sizeIconToButton = "1";
      valueControl = %valueControl;
      nameControl = %nameControl;
      base = %this;
   };
   
   %valueControl.altCommand = "ToolManager.getLastWindow().setFirstResponder(); " @ %button @ ".addField(" @ %this @ ".object" @ ");";
   %button.command = "ToolManager.getLastWindow().setFirstResponder(); " @ %button @ ".addField(" @ %this @ ".object" @ ");";
   
   %container.add(%nameControl);
   %container.add(%valueControl);
   %container.add(%button);
   
   %this.add(%container);
   return %container;
}

function LBQuickEditAddFieldButton::addField(%this, %object)
{
   %field = getWord(%this.nameControl.getText(), 0);
   %newValue = %this.valueControl.getText();
   
   if ((%field $= "") || (%newValue $= ""))
      return;
      
   %command = "%object." @ %field @ " = \"" @ %newValue @ "\";";
   eval(%command);

   %undo = new UndoScriptAction()
   {
      actionName = "Added Dynamic Field";
      class = UndoDynamicFieldQuickEdit;
      field = %field;
      oldValue = "";
      newValue = %newValue;
      object = %object;
      base = %this.base;
   };
   %undo.addToManager(LevelBuilderUndoManager);
      
   %base = %this.base;
   %base.schedule(0, "setProperty", %object);
}

function LBQuickEditDeleteFieldButton::deleteField(%this, %object, %name)
{
   %this.fieldEdit.setText("");
   %this.fieldEdit.updateProperty(%object, %name);
}

function LBQuickEditDynamicFieldStack::setProperty(%this, %object)
{
   %this.deleteChildren();
   
   %count = %object.getDynamicFieldCount();
   for (%i = 0; %i < %count; %i++)
   {
      %name = %object.getDynamicField(%i);
      %value = %object.getFieldValue(%name);
      
      // Shameless hack to keep scene graph cameraSize and cameraPosition fields from showing up.
      if ((%name $= "cameraSize") || (%name $= "cameraPosition"))
         continue;
      
      %this.createDynamicField(%name, %value);
   }
   
   %this.createAddDynamicField();
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "resize");
   ToolManager.getLastWindow().setFirstResponder();
}

function DynamicFieldTextEdit::updateProperty(%this, %object, %field)
{
   %newValue = %this.getText();
   %oldValue = %this.oldValue;
   %this.oldValue = %newValue;
   eval("%object." @ %field @ " = \"" @ %newValue @ "\";");
   
   if (%newValue !$= %oldValue)
   {
      %undoName = "Changed Dynamic Field";
      if (%newValue $= "")
         %undoName = "Deleted Dynamic Field";
      %undo = new UndoScriptAction()
      {
         actionName = %undoName;
         class = UndoDynamicFieldQuickEdit;
         field = %field;
         oldValue = %oldValue;
         newValue = %newValue;
         object = %object;
         base = %this.base;
      };
      %undo.addToManager(LevelBuilderUndoManager);
      %this.base.schedule(0, "setProperty", %object);
   }
}

function LBQuickEditDynamicFieldStack::deleteChildren(%this)
{
   while (%this.getCount())
      %this.getObject(0).delete();
}

function UndoDynamicFieldQuickEdit::undo(%this)
{
   %command = "%this.object." @ %this.field @ " = \"" @ %this.oldValue @ "\";";
   eval(%command);
   if (ToolManager.isAcquired(%this.object) && (ToolManager.getAcquiredObjectCount() == 1))
      %this.base.setProperty(%this.object);
}

function UndoDynamicFieldQuickEdit::redo(%this)
{
   %command = "%this.object." @ %this.field @ " = \"" @ %this.newValue @ "\";";
   eval(%command);
   if (ToolManager.isAcquired(%this.object) && (ToolManager.getAcquiredObjectCount() == 1))
      %this.base.setProperty(%this.object);
}
