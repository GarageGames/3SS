//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------
// TGBGraphFieldList Class - Example Use
//-----------------------------------------------------------------------------
//
// While this class may be used in this form, it is intended to be a pure virtual
// class that you derive your own specifics classes from.  It creates the basic
// functionality needed by a next/prev selection list
//
// See TGBEffectFieldList and TGBEmitterFieldList for real-world examples
//
//-----------------------------------------------------------------------------
//
//%MySelectionList = new GuiControl() 
//{
//   class                   = TGBGraphFieldList;
//   fieldsObject             = %field.getID();
//};



function TGBGraphFieldList::onAdd(%this)
{
   // Set current entry to none.
   %this.currentEntry = -1;

   // Create the list
   %this.list = new SimSet();
   
   // Add to our cleanup group.
   $LB::QuickEditGroup.add( %this.list );
   
   // Construct Graph GUI
   %this.constructGraph();   
}

function TGBGraphFieldList::onRemove(%this)
{
   // Remove the Item List
   %this.clear();
   
   // - Items destroyed proeprly in %this.clear but
   //   it recreates %this.list, so remove it.
   if( isObject(%this.list) )
      %this.list.delete();
      
   // Destroy Graph GUI
   %this.destroyGraph();

}

//-----------------------------------------------------------------------------
// Selection List Callbacks
//-----------------------------------------------------------------------------
function TGBGraphFieldList::OnSelChange( %this, %oldSelection, %newSelection )
{
   
   // Fetch Item Descriptor
   %itemDescriptor = %oldSelection.value;
   if( isObject( %itemDescriptor ) )
   {   
      // Fetch other fundamentals
      %fieldGraph = %this.Graph.findObjectByInternalName( "FieldGraph" );
      %graphIndex = %this.GetIndexByValue( %itemDescriptor );
      
      if( !isObject( %fieldGraph ) || %graphIndex == -1 )
         return;
        
      %fieldGraph.clearGraph(%graphIndex);
   }

   // Populate Graph   
   %this.populateGraph( %newSelection );
   
   // Update DropDown
   %label = %this.Graph.findObjectByInternalName( "FieldGraphName" );
   %index = %this.GetIndexByValue( %newSelection.Value );
   if( isObject( %label ) && ( %index != -1 ) && ( %graphIndex != %index ) ) 
      %label.setSelected( %index, false );
   
}

//-----------------------------------------------------------------------------
// Action Methods
//-----------------------------------------------------------------------------
function TGBGraphFieldList::clear( %this )
{
   if( isObject(%this.list) )
   {
      // Delete all the list entries and clear the SimSet 
      while( %this.list.getCount() > 0 )
      {
         if( isObject( %this.list.getObject( 0 ) ) )
            %this.list.getObject( 0 ).delete();
         //%this.list.remove( 0 );
      }
      // Delete the list
      %this.list.delete();
   }

   %this.list = new SimSet();
   %this.currentEntry = -1;
}

function TGBGraphFieldList::addItem( %this, %text, %value )
{
   // Should update?
   if( %this.list.getCount() == 0 )
      %shouldUpdate = true;
   else
      %shouldUpdate = false;

   // Construct the new entry
   %entry = new ScriptObject()
   {
      text = %text;
      value = %value;
   };

   // Add the entry to our list
   %this.list.add( %entry );
   
   // Add to our cleanup group.
   $LB::QuickEditGroup.add( %entry );   

   // Update the users view
   if( %shouldUpdate )
      %this.updateDisplay();
      
   // Inserts always are at the end of the list
   // so we assume that count - 1 was our index. JDD 
   return %this.list.getCount() - 1;
}

function TGBGraphFieldList::NextItem( %this )
{
   // Sanity!
   if( !isObject( %this.list) || %this.list.getCount() <= 1 )
      return;

   // No entry? Default to first.
   if( %this.currentEntry == -1 )
   {
      %this.SetSel( %this.list.getObject( 0 ) );
      return;
   }

   for( %i = 0; %i < %this.list.getCount(); %i++ )
   {
      %object = %this.list.getObject( %i );
      if( %object == %this.currentEntry )
      {
         // Loop around?
         if( %i + 1 == %this.list.getCount() )
            %this.SetSel( %this.list.getObject( 0 ) );
         else
            %this.SetSel( %this.list.getObject( %i + 1 ) );

         return;
      }
   }

   // If we get here, something has gone terribly wrong
   // So we'll just default to the first entry.
   %this.SetSel( %this.list.getObject( 0 ) );
}


function TGBGraphFieldList::PreviousItem( %this )
{
   // Stub so that either works
   %this.Prev();
}

function TGBGraphFieldList::PrevItem( %this )
{
   // Sanity!
   if( !isObject( %this.list ) || %this.list.getCount() <= 1 )
      return;

   // No entry? Default to first.
   if( %this.currentEntry == -1 )
   {
      %this.SetSel( %this.list.getObject( 0 ) );
      return;
   }

   for( %i = 0; %i < %this.list.getCount(); %i++ )
   {
      %object = %this.list.getObject( %i );
      if( %object == %this.currentEntry )
      {
         // Loop around?
         if( %i - 1 < 0 )
            %this.SetSel( %this.list.getObject( %this.list.getCount() - 1  ) );
         else
            %this.SetSel( %this.list.getObject( %i - 1 ) );

         return;
      }
   }

   // If we get here, something has gone terribly wrong
   // So we'll just default to the last entry.
   %this.SetSel( %this.list.getObject( %this.list.getCount() - 1 ) );
}


function TGBGraphFieldList::SetSel( %this, %entry )
{
   // Sanity
   if( !isObject(%this.list) || !isObject( %entry ) )
      return;
   
   // Notify user of selection change
   %this.OnSelChange( %this.currentEntry, %entry );
   
   // Store new entry
   %this.currentEntry = %entry;
   
   // Update Display
   %this.updateDisplay();
   
}

function TGBGraphFieldList::GetSel( %this )
{
   return %this.currentEntry;
}

function TGBGraphFieldList::RemoveById( %this, %id )
{
   // Sanity
   if( !isObject(%this.list) )
      return -1;

   // Remove
   if( %this.list.getCount() <= %index )
      %this.list.remove( %id );
}

function TGBGraphFieldList::UpdateDisplay( %this )
{
   if( %this.currentEntry == -1 )
   {
      if( isObject(%this.list) && %this.list.getCount() != 0 )
         %this.currentEntry = %this.list.getObject( 0 );
      else
         return;
   }
}

//-----------------------------------------------------------------------------
// Item Retrieval Methods
//-----------------------------------------------------------------------------
function TGBGraphFieldList::GetItemByIndex( %this, %index )
{
   if( !isObject(%this.list) )
      return -1;

   if( %this.list.getCount() <= %index )
      return %this.list.getObject(%index);

   // None found!
   return -1;

}

function TGBGraphFieldList::GetItemByText( %this, %text )
{
   if( !isObject(%this.list) )
      return -1;

   for(%i = 0; %i < %this.list.getCount(); %i++)
      if( %this.list.getObject(%i).text $= %text )
         return %this.list.getObject(%i);

   // None found!
   return -1;
}

function TGBGraphFieldList::GetItemByValue( %this, %value )
{
   for(%i = 0; %i < %this.list.getCount(); %i++)
      if( %this.list.getObject(%i).value $= %value || %this.list.getObject(%i).value == %value)
         return %this.list.getObject(%i);

   // None found!
   return -1;

}

function TGBGraphFieldList::GetIndexByValue( %this, %value )
{
   for(%i = 0; %i < %this.list.getCount(); %i++)
      if( %this.list.getObject(%i).value $= %value || %this.list.getObject(%i).value == %value)
         return %i;

   // None found!
   return -1;
}

//-----------------------------------------------------------------------------
// Initialize Graph
//-----------------------------------------------------------------------------
function TGBGraphFieldList::initGraph(%this)
{
   error("This should not be called as it is intended for stub purposes only");
}

function TGBGraphFieldList::DestroyGraph( %this )
{
   if( !isObject( %this.graph ) ) 
      return;
      
   // Call Graph Closing
   %this.onClose();      
      
   // Destruct.
   %this.graph.delete();
}

function TGBGraphFieldList::onClose( %this )
{
   %graph = %this.graph;
   %parent = %this.graph.getParent();

   // If we're pushed onto canvas, pop ourself      
   if( %graph.isAwake() && isObject( %parent ) )
   {
      if( %parent == Canvas.GetID() )
         Canvas.popDialog( %graph );
      else if( %parent.getParent() == Canvas.getID() )
         Canvas.popDialog( %parent );
   }  
}

function TGBGraphFieldList::ConstructGraph( %this )
{
   // New Graph
   %newGraph = new GuiControl() {
      canSaveDynamicFields = "0";
      Profile = "EditorButtonLeft";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "197 94";
      Extent = "271 287";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";

      new GuiParticleGraphCtrl() {
         canSaveDynamicFields = "0";
         internalName = "FieldGraph";
         class = "FieldListGraph";
         Profile = "GuiDefaultProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "8 29";
         Extent = "255 158";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         // Special Tags
         fieldsObject = %this.fieldsObject;
         base = %this;         
         
      };
      new GuiIconButtonCtrl() {
         canSaveDynamicFields = "0";
         internalName = "previousButton";
         Profile = "EditorButtonLeft";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "105 254";
         Extent = "80 24";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         Command = %this @ ".PrevItem();";
         hovertime = "1000";
         text = "Back";
         groupNum = "-1";
         buttonType = "PushButton";
         buttonMargin = "4 4";
         iconLocation = "Left";
         sizeIconToButton = "0";
         textLocation = "Center";
         textMargin = "4";
      };
      new GuiIconButtonCtrl() {
         canSaveDynamicFields = "0";
         internalName = "nextButton";
         Profile = "EditorButtonRight";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "183 254";
         Extent = "80 24";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         Command = %this @ ".NextItem();";
         hovertime = "1000";
         text = "Next";
         groupNum = "-1";
         buttonType = "PushButton";
         buttonMargin = "4 4";
         iconLocation = "Right";
         sizeIconToButton = "0";
         textLocation = "Center";
         textMargin = "4";
      };
      new GuiIconButtonCtrl() {
         canSaveDynamicFields = "0";
         internalName = "closeButton";
         Profile = "EditorButton";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "8 254";
         Extent = "68 24";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         Command = %this @ ".onClose();";
         hovertime = "1000";
         text = "Close";
         groupNum = "-1";
         buttonType = "PushButton";
         buttonMargin = "4 4";
         iconBitmap = expandPath("^{EditorAssets}/gui/iconCancel.png");
         iconLocation = "Left";
         sizeIconToButton = "0";
         textLocation = "Center";
         textMargin = "4";
      };
      new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextHLBoldRight";
         HorizSizing = "width";
         VertSizing = "bottom";
         position = "5 2";
         Extent = "81 27";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = "Current Field:";
         maxLength = "1024";
      };
      new GuiPopUpMenuCtrl() {
         canSaveDynamicFields = "0";
         internalName = "FieldGraphName";
         class = "FieldListDropDown";
         Profile = "GuiPopupMenuProfile";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "92 7";
         Extent = "171 19";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         maxPopupHeight = "200";
         sbUsesNAColor = "0";
         reverseTextList = "0";
         bitmapBounds = "16 16";
         // Special Tags
         class = "FieldListDropDown";
         fieldsObject = %this.fieldsObject;
         base = %this;         
         
      };
      new GuiTextCtrl() {

         canSaveDynamicFields = "0";
         Profile = "EditorDecorativeTextHLLeft";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "13 28";
         Extent = "35 21";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = "Value";
         maxLength = "1024";
         modal=false;
         active=false;
         
      };
      new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorDecorativeTextHLLeft";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "231 164";
         Extent = "35 21";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = "Time";
         maxLength = "1024";
         modal=false;
         active=false;
         
      };
      new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextEdit";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "105 194";
         Extent = "64 18";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
         // Special Tags
         internalName = "FieldGraphValueMin";
         class = "GraphFieldValueEditMin";
         fieldsObject = %this.fieldsObject;
         base = %this;      
      };
      new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextHLCenter";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "8 193";
         Extent = "94 19";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = " Value (Min/Max)";
         maxLength = "1024";
      };
      new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextHLCenter";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "8 221";
         Extent = "92 19";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = " Time (Min/Max)";
         maxLength = "1024";
      };
      new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextEdit";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "199 194";
         Extent = "64 18";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
         // Special Tags
         internalName = "FieldGraphValueMax";
         class = "GraphFieldValueEditMax";
         fieldsObject = %this.fieldsObject;
         base = %this;  
      };
      new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
         internalName = "FieldGraphTimeMin";
         Profile = "EditorTextEdit";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "105 221";
         Extent = "64 18";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
         // Special Tags
         internalName = "FieldGraphTimeMin";
         class = "GraphFieldTimeEditMin";
         fieldsObject = %this.fieldsObject;
         base = %this;                       
      };
      new GuiTextEditCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextEdit";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "199 221";
         Extent = "64 18";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         maxLength = "1024";
         historySize = "0";
         password = "0";
         tabComplete = "0";
         sinkAllKeyEvents = "0";
         password = "0";
         passwordMask = "*";
         // Special Tags
         internalName = "FieldGraphTimeMax";
         class = "GraphFieldTimeEditMax";
         fieldsObject = %this.fieldsObject;
         base = %this;                       
      };
      new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextHLBoldCenter";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "179 196";
         Extent = "12 12";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = "/";
         maxLength = "1024";
      };
      new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorTextHLBoldCenter";
         HorizSizing = "right";
         VertSizing = "bottom";
         position = "179 223";
         Extent = "12 12";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "1000";
         text = "/";
         maxLength = "1024";
      };
   };

   
   // Create Graph Context Container.
   %contextPopup = new ScriptObject() 
   {
      class      = ContextDialogContainer;
      //superclass = GraphFieldContextDialog;
      dialog     = %newGraph;
   };
      
   // Store on object.
   %this.Graph = %newGraph;
   %this.ContextPopup = %contextPopup;
   
   // Add to our cleanup group.
   $LB::QuickEditGroup.add( %newGraph );   
   
}

function TGBGraphFieldList::AddGraphEntry( %this, %graphDescription, %graphName, %minValue, %maxValue, %minTime, %maxTime )
{   
   // Fetch Graph Control
   %fieldGraph = %this.Graph.findObjectByInternalName( "FieldGraph" );
   %fieldGraphName = %this.Graph.findObjectByInternalName( "FieldGraphName" );
        
   // Create Item Descriptor Object.
   %itemDescriptor = new ScriptObject()
   {
      Description = %graphDescription;
      Name        = %graphName;
      MinValue    = %minValue;
      MaxValue    = %maxValue;
      MinTime     = %minTime;
      MaxTime     = %maxTime;
   };
   
   $LB::QuickEditGroup.add( %itemDescriptor );   
   
   // Add to List.
   %graphIndex = %this.AddItem( %graphDescription, %itemDescriptor );   
   if( %graphIndex == -1 )
   {
      // Failed to add, cleanup descriptor.
      %itemDescriptor.delete();
      
      // Failure.
      return false;
      
   }
   
   // Set Graph Bounds
   %fieldGraph.setGraphMax( %graphIndex, %maxTime, %maxValue );   
   %fieldGraph.setGraphMin( %graphIndex, %minTime, %minValue);
   
   // Add to drop down selection list with %graphIndex as ID.
   %fieldGraphName.Add( %graphDescription, %graphIndex );
   
   if( %graphIndex == 0 )
      %fieldGraphName.setSelected( %graphIndex, true );
   
   // Success.
   return true;
   
}


function TGBGraphFieldList::populateGraph( %this, %item )
{
   // Fetch Item Descriptor
   %itemDescriptor = %item.value;
   if( !isObject( %itemDescriptor ) )
      return;
   
   // Fetch other fundamentals
   %fieldGraph = %this.Graph.findObjectByInternalName( "FieldGraph" );
   %fieldGraphValueMin = %this.Graph.findObjectByInternalName( "FieldGraphValueMin" );
   %fieldGraphValueMax = %this.Graph.findObjectByInternalName( "FieldGraphValueMax" );
   %fieldGraphTimeMin = %this.Graph.findObjectByInternalName( "FieldGraphTimeMin" );
   %fieldGraphTimeMax = %this.Graph.findObjectByInternalName( "FieldGraphTimeMax" );   
   %fieldsObject = %this.fieldsObject;
   %graphIndex = %this.GetIndexByValue( %itemDescriptor );
   
   if(!isObject(%fieldsObject) || %graphIndex == -1 )
      return;
	  
   %fieldGraph.clearGraph(%graphIndex);
   %fieldGraph.setSelectedPlot( %graphIndex );
            
   // Select Correct Field Graph.
   %fieldsObject.selectGraph( %itemDescriptor.Name );

   // Fetch Key Count.
   %keyCount = %fieldsObject.getDataKeyCount();

   // Add Keys.
   for ( %n = 0; %n < %keyCount; %n++ )
   {
      %graphMin = %fieldGraph.getGraphMin(%graphIndex);
      %graphMinTime = getWord(%graphMin, 0);
      %graphMinValue = getWord(%graphMin, 1);
      %graphMax = %fieldGraph.getGraphMax(%graphIndex);
      %graphMaxTime = getWord(%graphMax, 0);
      %graphMaxValue = getWord(%graphMax, 1);

      // Fetch Data Key.
      %dataKey = %fieldsObject.getDataKey(%n);
      // Fetch Key Components.
      %time = getWord(%dataKey, 0);
      %value = getWord(%dataKey, 1);

      if(%time < %graphMinTime)
         %fieldGraph.setGraphMinX(%graphIndex, %time);
      if(%time > %graphMaxTime)
         %fieldGraph.setGraphMaxX(%graphIndex, %time);
      if(%value < %graphMinValue)
         %fieldGraph.setGraphMinY(%graphIndex, %value);
      if(%value > %graphMaxValue)
         %fieldGraph.setGraphMaxY(%graphIndex, %value);

      // Add to List.
      %fieldGraph.addPlotPoint(%graphIndex , %time, %value, false);
   }  

   // Update Min/Max's
   %fieldGraph.setGraphMinX(%graphIndex, %itemDescriptor.minTime);
   %fieldGraph.setGraphMaxX(%graphIndex, %itemDescriptor.maxTime);
   %fieldGraph.setGraphMinY(%graphIndex, %itemDescriptor.minValue);
   %fieldGraph.setGraphMaxY(%graphIndex, %itemDescriptor.maxValue);
   
   %fieldGraphTimeMin.text = %itemDescriptor.minTime;
   %fieldGraphTimeMax.text = %itemDescriptor.maxTime;
   %fieldGraphValueMin.text = %itemDescriptor.minValue;
   %fieldGraphValueMax.text = %itemDescriptor.maxValue;
}

//-----------------------------------------------------------------------------
// Graph Field DropDown Event
//-----------------------------------------------------------------------------
function FieldListDropDown::onSelect( %this, %id, %text )
{
   %itemDescriptor = %this.base.GetItemByText( %text );
   if( !isObject( %itemDescriptor ) ) //|| %this.base.getSel() == %itemDescriptor )
      return;
      
   // Set as selection.
   %this.base.setSel( %itemDescriptor );
}

//-----------------------------------------------------------------------------
// Graph Events
//-----------------------------------------------------------------------------
function FieldListGraph::onPlotPointAdded(%this, %graph, %point, %index)
{      
   %this.AddGraphKey(%graph, getWord(%point, 0), getWord(%point, 1));
   
   %undo = new UndoScriptAction(){
      editor = %this;
      fieldsObject = %this.fieldsObject;
      class = AddGraphKey;
      actionName = "Add Graph Key"; 
      effect = %this.base;
      graph = %this.base.getSel().Value.Name;
      point = %point;
      index = %index;   
   };

   %undo.addToManager(LevelBuilderUndoManager); 
   ToolManager.getLastWindow().setFirstResponder();
}

function FieldListGraph::onMouseMove(%this, %pos)
{
  %this.mouseMovePos = %pos;   
}

function FieldListGraph::onPlotPointSelectedMouseDown(%this, %index)
{
   %this.pointOnSelected = %this.mouseMovePos;
   %this.indexOnSelected = %index;
   ToolManager.getLastWindow().setFirstResponder();
}

function FieldListGraph::onPlotPointChangedUp(%this, %graph, %point, %lastIndex, %newIndex)
{
   // Destroy old value
   if(%lastIndex != 0)
     %this.DestroyGraphKey(%graph, %lastIndex);

   // Create New
   %this.AddGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
   
   %undo = new UndoScriptAction(){
      editor = %this;
      fieldsObject = %this.fieldsObject;
      class = ChangeGraphKey;
      actionName = "Change Graph Key"; 
      emitter = %this.base;
      graph = %this.base.getSel().Value.Name;
      newPoint = %point;
      newIndex = %newIndex;
      lastPoint = %this.pointOnSelected;  
      lastIndex = %this.indexOnSelected;      
   };
   
   %undo.addToManager(LevelBuilderUndoManager);
   ToolManager.getLastWindow().setFirstResponder();
}

function FieldListGraph::onPlotPointChangedMove(%this, %graph, %point, %lastIndex)
{
   if(%lastIndex != 0)
   {
      %this.DestroyGraphKey(%graph, %lastIndex);
      %this.AddGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
   } 
   else
   {
      if( getWord(%point, 0) != 0)
         %this.resetSelectedPoint();
      else
         %this.AddGraphKey(%graph, getWord(%point, 0), getWord(%point, 1));
   }
   ToolManager.getLastWindow().setFirstResponder();
}

function FieldListGraph::onPlotPointRemoved(%this, %graph, %index, %point)
{  
   %this.DestroyGraphKey(%graph, %index); 
   
   %undo = new UndoScriptAction(){
      editor = %this;
      fieldsObject = %this.fieldsObject;
      class = RemoveGraphKey;
      actionName = "Remove Graph Key"; 
      emitter = %this.base;
      graph = %this.base.getSel().Value.Name;
      point = %point;
      index = %index;   
   };
   
   %undo.addToManager(LevelBuilderUndoManager); 
   ToolManager.getLastWindow().setFirstResponder();
}

function FieldListGraph::onPlotPointSelected(%this, %index)
{   
   if(%index == 0)
      %this.setPointXMovementClamped(true);      
   else
      %this.setPointXMovementClamped(false);      
   ToolManager.getLastWindow().setFirstResponder();
}


//-----------------------------------------------------------------------------
// Add Graph Key.
//-----------------------------------------------------------------------------
function FieldListGraph::AddGraphKey(%this, %graph, %time, %value)
{
   %fieldsObject = %this.fieldsObject;
   
   if(!isObject(%fieldsObject))
      return;
   
   if( !isObject( %this.base ) || %this.base.getSel() == -1 )
      return;
      
   // Retrieve Graph Name from Item Descriptor
   %graph = %this.base.GetSel().Value.Name;
      
   // Select the graph passed
   %fieldsObject.selectGraph( %graph );
   
   %graphMinTime = %fieldsObject.getMinTime();
   %graphMaxTime = %fieldsObject.getMaxTime();
   %graphMinValue = %fieldsObject.getMinValue();
   %graphMaxValue = %fieldsObject.getMaxValue();	
   
	  // Check Time Bounds.
	  if ( %time < %graphMinTime || %time > %graphMaxTime )
	  {
		   // Problem so show Warning...
		   MessageBoxOK("Add Key - Invalid Time", %time SPC "key-time is outside allowed time for this graph (min = " @ %graphMinTime SPC "max = " @ %graphMaxTime @ "!", "");
		   // Finish Here.
		   return;		
   }
	
	  // Check Value Bounds.
	  if ( %value < %graphMinValue || %value > %graphMaxValue )
	  {
		    // Problem so show Warning...
		    MessageBoxOK("Add Key - Invalid Value", %value SPC "key-value is outside allowed value for this graph (min = " @ %graphMinValue SPC "max = " @ %graphMaxValue @ "!", "");
		    // Finish Here.
		    return;		
   }
   
	  // Add Key to Graph.
	  %fieldsObject.addDataKey( %time, %value );
	  %fieldsObject.onChanged();

	  // Read Graph Keys.
	  %this.base.populateGraph( %this.base.getSel() );
	  //%this.GraphReadSelectedEffectGraphKeys();
   ToolManager.getLastWindow().setFirstResponder();
}

//-----------------------------------------------------------------------------
// Clear Keys.
//-----------------------------------------------------------------------------
function FieldListGraph::clearGraphKeys(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   // Select Effect Graph.
	  %particleEffect.selectGraph( %this.selectedEffectGraphField );
	
	  // Clear Data Keys.
	  %particleEffect.clearDataKeys();
	  %particleEffect.onChanged();
	
	  // Read Graph Keys.
	  %this.base.populateGraph( %this.base.getSel() );
   ToolManager.getLastWindow().setFirstResponder();
}

//-----------------------------------------------------------------------------
// Destroy Key.
//-----------------------------------------------------------------------------
function FieldListGraph::DestroyGraphKey(%this, %graph, %index)
{
   %fieldsObject = %this.fieldsObject;
   
   if(!isObject(%fieldsObject))
      return;
       
	  // Check Row Selected.
	  if ( %index == -1 ) return;
	
	  // Cannot Remove Key#0!
	  if ( %index == 0 )
	  {
		    // Problem so show Warning...
		    //MessageBoxOK("Remove Key - Invalid Key", "Cannot remove the first key in a graph!", "");
		    %this.base.populateGraph( %this.base.getSel() );
		    // Finish Here.
		    return;		
   }
	
   if( !isObject( %this.base ) || %this.base.getSel() == -1 )
      return;
      
   // Retrieve Graph Name from Item Descriptor
   %graph = %this.base.GetSel().Value.Name;

	  // Select Effect Graph.
	  %fieldsObject.selectGraph( %graph );
	
	  // Remove the Key.
	  %fieldsObject.removeDataKey( %index );
	  %fieldsObject.onChanged();
	
	  // Read Graph Keys.
	  %this.base.populateGraph( %this.base.getSel() );
	  //%this.GraphReadSelectedEffectGraphKeys();
   ToolManager.getLastWindow().setFirstResponder();
}

// Min/Max Graph Time/Value Validators.
function GraphFieldValueEditMax::onValidate( %this )
{
   %fieldGraph = %this.base.Graph.findObjectByInternalName( "FieldGraph" );
   %graphIndex = %this.base.GetIndexByValue( %this.base.getSel().Value );
   
   if(!isObject(%fieldGraph) || %graphIndex == -1 )
      return;

   %minVal = %this.fieldsObject.getMinValue();
   %maxVal = %this.fieldsObject.getMaxValue();
   %value  = %this.getText();
   
   %selItem = %this.base.GetSel();
   if( isObject( %selItem ) )
   {
      if( %value >= %minVal && %value <= %maxVal )
         %selItem.value.maxValue = %value;
      else
         %this.setText( %selItem.value.maxValue );
         
      %fieldGraph.setGraphMaxY( %graphIndex, %selItem.value.maxValue );               
   }
}

function GraphFieldValueEditMin::onValidate( %this )
{
   %fieldGraph = %this.base.Graph.findObjectByInternalName( "FieldGraph" );
   %graphIndex = %this.base.GetIndexByValue( %this.base.getSel().Value );
   
   if(!isObject(%fieldGraph) || %graphIndex == -1 )
      return;

   %minVal = %this.fieldsObject.getMinValue();
   %maxVal = %this.fieldsObject.getMaxValue();
   %value  = %this.getText();
   
   %selItem = %this.base.GetSel();
   if( isObject( %selItem ) )
   {
      if( %value >= %minVal && %value <= %maxVal )
         %selItem.value.minValue = %value;
      else
         %this.setText( %selItem.value.minValue );
         
      %fieldGraph.setGraphMinY( %graphIndex, %selItem.value.minValue );               
   }
}

function GraphFieldTimeEditMax::onValidate( %this )
{
   %fieldGraph = %this.base.Graph.findObjectByInternalName( "FieldGraph" );
   %graphIndex = %this.base.GetIndexByValue( %this.base.getSel().Value );
   
   if(!isObject(%fieldGraph) || %graphIndex == -1 )
      return;
      
   %minVal = %this.fieldsObject.getMinTime();
   %maxVal = %this.fieldsObject.getMaxTime();
   %value  = %this.getText();
   
   %selItem = %this.base.GetSel();
   if( isObject( %selItem ) )
   {
      if( %value >= %minVal && %value <= %maxVal )
         %selItem.value.maxTime = %value;
      else
         %this.setText( %selItem.value.maxTime );
         
      %fieldGraph.setGraphMaxX( %graphIndex, %selItem.value.maxTime );               
   }
}

function GraphFieldTimeEditMin::onValidate( %this )
{
   %fieldGraph = %this.base.Graph.findObjectByInternalName( "FieldGraph" );
   %graphIndex = %this.base.GetIndexByValue( %this.base.getSel().Value );
   
   if(!isObject(%fieldGraph) || %graphIndex == -1 )
      return;

   %minVal = %this.fieldsObject.getMinTime();
   %maxVal = %this.fieldsObject.getMaxTime();
   %value  = %this.getText();
   
   %selItem = %this.base.GetSel();
   if( isObject( %selItem ) )
   {
      if( %value >= %minVal && %value <= %maxVal )
         %selItem.value.minTime = %value;
      else
         %this.setText( %selItem.value.minTime );
         
      %fieldGraph.setGraphMinX( %graphIndex, %selItem.value.minTime );               
   }
}

