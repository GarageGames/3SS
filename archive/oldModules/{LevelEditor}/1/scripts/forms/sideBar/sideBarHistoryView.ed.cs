//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBHistoryViewContent = GuiFormManager::AddFormContent( "LevelBuilderSidebarEdit", "HistoryView", "LBHistoryView::CreateForm", "LBHistoryView::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBHistoryView::CreateForm( %formCtrl )
{    

   %base = new GuiRolloutCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "323 231";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      Caption = "Undo History";
      Margin = "6 4";
      DragSizable = false;
      DefaultHeight = "200";
   };

   %scroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "323 231";
      MinExtent = "72 128";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   %base.add(%scroll);

   %treeObj = new GuiListBoxCtrl() {
      canSaveDynamicFields = "0";
      class = "LevelBuilderHistoryView";
      Profile = "EditorListBox";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "0 0";
      Extent = "323 231";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      AllowMultipleSelections = "0";
      FitParentWidth = "1";
      scroll = %scroll;
      base = %base;
      owner = %formCtrl;
   };

   %scroll.add( %treeObj );
   %formCtrl.add( %base );

   // Specify Message Control (Override getObject(0) on new Content which is default message control)
   %base.MessageControl = %treeObj;
   %base.sizeToContents();
   
   //*** Return back the base control to indicate we were successful
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBHistoryView::SaveForm( %formCtrl )
{
   // Nothing.
}

function LevelBuilderHistoryView::onWake(%this)
{
   %this.refresh();
}

//-----------------------------------------------------------------------------
// Form Content Functionality
//-----------------------------------------------------------------------------
function LevelBuilderHistoryView::onContentMessage( %this, %sender, %message )
{
   %command = getWord(%message, 0);
   %value = getWord(%message, 1);
   
   if (!isObject(%this.undoManager) && (%command !$= "setUndoManager"))
      return;

   if (%this.stackPointer > 0 && ( %this.stackPointer - 1 ) <= %this.getItemCount() )
      %this.clearItemColor(%this.stackPointer - 1);
            
   switch$( %command )
   {
      case "setUndoManager":
         %this.undoManager = %value;
         if (isObject(%value))
            %this.refresh();
         else
            %this.clear();
            
      case "addUndo":
         %this.addUndo();
            
      case "doUndo":
         %this.stackPointer--;
         
      case "doRedo":
         %this.stackPointer++;
         
      case "refresh":
         %this.refresh();
         
      case "clear":
         %this.clear();
   }
   
   if (%this.stackPointer > 0 && ( %this.stackPointer - 1 ) <= %this.getItemCount() )
      %this.setItemColor(%this.stackPointer - 1, "1.0 0.0 0.0");
      
   %this.scroll.scrollToBottom();
   %this.base.sizeToContents();
   %this.owner.updateStack();

}

function LevelBuilderHistoryView::addUndo(%this)
{
   %count = %this.getItemCount();
   for (%i = %this.stackPointer; %i < %count; %i++)
      %this.deleteItem(%this.stackPointer);
   
   %this.addItem(%this.undoManager.getNextUndoName());
   %this.stackPointer++;
}

function LevelBuilderHistoryView::onDoubleClick(%this)
{
   %undoTo = %this.getSelectedItem() + 1;
   %undoFrom = %this.stackPointer;
   
   if (%undoTo > %undoFrom)
   {
      for (%i = %undoFrom; %i < %undoTo; %i++)
         %this.undoManager.redo();
   }
   
   else
   {
      for (%i = %undoFrom; %i >= %undoTo; %i--)
         %this.undoManager.undo();
   }
   
   %this.refresh();
}

function LevelBuilderHistoryView::clear(%this)
{
   %this.clearItems();
}

function LevelBuilderHistoryView::refresh(%this)
{
   %this.clear();
 
   %this.stackPointer = %this.undoManager.getUndoCount();  
   for (%i = 0; %i < %this.stackPointer; %i++)
      %this.addItem(%this.undoManager.getUndoName(%i));
      
   for (%i = 0; %i < %this.undoManager.getRedoCount(); %i++)
      %this.addItem(%this.undoManager.getRedoName(%i));
   
   if (%this.stackPointer > 0)
      %this.setItemColor(%this.stackPointer - 1, "1.0 0.0 0.0");
}

function UndoManager::onAddUndo(%this)
{
   GuiFormManager::SendContentMessage($LBHistoryViewContent, %this, "addUndo");
}

function UndoManager::onUndo(%this)
{
   GuiFormManager::SendContentMessage($LBHistoryViewContent, %this, "doUndo");
}

function UndoManager::onRedo(%this)
{
   GuiFormManager::SendContentMessage($LBHistoryViewContent, %this, "doRedo");
}

function UndoManager::onClear(%this)
{
   GuiFormManager::SendContentMessage($LBHistoryViewContent, %this, "clear");
}

function UndoManager::onRemoveUndo(%this)
{
   GuiFormManager::SendContentMessage($LBHistoryViewContent, %this, "refresh");
}
