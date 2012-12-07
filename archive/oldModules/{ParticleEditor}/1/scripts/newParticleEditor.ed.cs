//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LevelBuilderParticleEditor::onEffectRelinquished(%this, %effect)
{
   %this.clearGUI();
   %this.selectedEffect = "";
}

function LevelBuilderParticleEditor::onEffectAcquired(%this, %effect)
{
   %this.selectedEffect = %effect;
   %this.initializeGUI();
}

function LevelBuilderParticleEditor::getSelectedEffect(%this)
{
   return %this.selectedEffect;
}

function LevelBuilderParticleEditor::getSelectedEmitter(%this)
{
   return %this.selectedEmitter;   
}

function LevelBuilderParticleEditor::setSelectedEmitter(%this, %emitter)
{
   if(isObject(%emitter))
      %this.selectedEmitter = %emitter;   
}

function T2DParticleEditorEffectGuiTabBook::onTabSelected(%this, %tab)
{
   if(%tab $= "Graph")
   {
      LevelBuilderParticleEditor.GraphReadSelectedEffectGraphKeys();   
   } else if(%tab $= "Table")
   {
      LevelBuilderParticleEditor.readEffectGraphKeys();
   }
}

function EffectGraph::onMouseMove(%this, %pos)
{
   effectGraphMousePosition.setText(%pos);
}

function EffectGraph::onMouseDragged(%this, %pos)
{
   effectGraphMousePosition.setText(%pos);
}

function EffectGraph::onSetSelected(%this, %graph)
{
   %graph = LevelBuilderParticleEditor.EffectGraphFieldExternalNameArray[%graph];
   effectGraphSelectedGraph.setText(%graph);
   LevelBuilderParticleEditor.graphEffectSelectedGraph = %graph;
   
   %name = LevelBuilderParticleEditor.EffectReturnInternalName(%graph);
   %graphIndex = LevelBuilderParticleEditor.EffectGetNameIndex(%name);
   
   %graphMin = EffectGraph.getGraphMin(%graphIndex);
	  %graphMinTime = getWord(%graphMin, 0);
	  %graphMinValue = getWord(%graphMin, 1);
	  %graphMax = EffectGraph.getGraphMax(%graphIndex);
	  %graphMaxTime = getWord(%graphMax, 0);
	  %graphMaxValue = getWord(%graphMax, 1);
   
   ParticleEditorEffectTextEditMinValue.setText(%graphMinValue);
   ParticleEditorEffectTextEditMinTime.setText(%graphMinTime);
   ParticleEditorEffectTextEditMaxValue.setText(%graphMaxValue);
   ParticleEditorEffectTextEditMaxTime.setText(%graphMaxTime);
}

function EffectGraph::onPlotPointAdded(%this, %graph, %point, %index)
{
   LevelBuilderParticleEditor.GraphAddEffectGraphKey(%graph, getWord(%point, 0), getWord(%point, 1));
 
   // Grab the currently selected particle effect
   %particleEffect = LevelBuilderParticleEditor.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   %undo = new UndoScriptAction(){
      class = EffectAddGraphKey;
      actionName = "Effect Add Graph Key"; 
      effect = %particleEffect;
      graph = %graph;
      point = %point;
      index = %index;   
   };

   %undo.addToManager(LevelBuilderUndoManager); 
}

function EffectGraph::onPlotPointChangedUp(%this, %graph, %point, %lastIndex, %newIndex)
{
   if(!LevelBuilderParticleEditor.EffectUpdateOnMove)
   {
      if(%lastIndex != 0)
         LevelBuilderParticleEditor.GraphDestroyEffectGraphKey(%graph, %lastIndex);
      LevelBuilderParticleEditor.GraphAddEffectGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
   }
 
   // Grab the currently selected particle effect
   %particleEffect = LevelBuilderParticleEditor.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   %undo = new UndoScriptAction(){
      class = EffectChangeGraphKey;
      actionName = "Effect Change Graph Key"; 
      effect = %particleEffect;
      graph = %graph;
      newPoint = %point;
      newIndex = %newIndex;
      lastPoint = %this.pointOnSelected;  
      lastIndex = %this.indexOnSelected;      
   };

   %undo.addToManager(LevelBuilderUndoManager);
}

function EffectGraph::onPlotPointChangedMove(%this, %graph, %point, %lastIndex)
{
   if(LevelBuilderParticleEditor.EffectUpdateOnMove)
   {
      if(%lastIndex != 0)
      {
         LevelBuilderParticleEditor.GraphDestroyEffectGraphKey(%graph, %lastIndex);
         LevelBuilderParticleEditor.GraphAddEffectGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
      } else
      {
         if(getWord(%point, 0) != 0)
         {
            %this.resetSelectedPoint();
         } else
         {
            LevelBuilderParticleEditor.GraphAddEffectGraphKey(%graph, getWord(%point, 0), getWord(%point, 1));            
         }
      }
   }
}

function EffectGraph::onPlotPointRemoved(%this, %graph, %index, %point)
{
   LevelBuilderParticleEditor.GraphDestroyEffectGraphKey(%graph, %index); 
   
   // Grab the currently selected particle effect
   %particleEffect = LevelBuilderParticleEditor.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   %undo = new UndoScriptAction(){
      class = EffectRemoveGraphKey;
      actionName = "Effect Remove Graph Key"; 
      effect = %particleEffect;
      graph = %graph;
      point = %point;
      index = %index;   
   };

   %undo.addToManager(LevelBuilderUndoManager); 
}

function EffectGraph::onPlotPointSelectedMouseDown(%this, %index)
{
   %this.pointOnSelected = effectGraphMousePosition.getValue();
   %this.indexOnSelected = %index;
}

function EffectGraph::onPlotPointSelected(%this, %index)
{
   if(%index == 0)
   {
      %this.setPointXMovementClamped(true);      
   } else
   {
      %this.setPointXMovementClamped(false);      
   }
}

function effectVisibleGraphFieldsList::getItemIndex(%this, %name)
{
   %count = %this.getItemCount();

   for(%i=0;%i<%count;%i++)
   {         
      if(%name $= %this.getItemText(%i))
         return(%i);
   }
}

function effectHiddenGraphFieldsList::getItemIndex(%this, %name)
{
   %count = %this.getItemCount();

   for(%i=0;%i<%count;%i++)
   {   
      if(%name $= %this.getItemText(%i))
         return(%i);
   }
}

function LevelBuilderParticleEditor::EffectMoveToHidden(%this)
{
   %itemList = effectVisibleGraphFieldsList.getSelectedItems();
   %count = getWordCount(%itemList);

   for(%i=%count-1;%i>=0;%i--)
   {
      %index = getWord(%itemList, %i);
      %item = effectVisibleGraphFieldsList.getItemText(%index);
      
      if(%index == -1)
      {
         error("invalid index"); 
         return;
      }
      
      effectVisibleGraphFieldsList.deleteItem(%index);
      effectHiddenGraphFieldsList.addItem(%item);

      %internalName = %this.EffectReturnInternalName(%item);
      %internalNameIndex = %this.EffectGetNameIndex(%internalName);
      
      effectGraph.setGraphHidden(%internalNameIndex, true);
   } 
}

function LevelBuilderParticleEditor::EffectMoveToVisible(%this)
{
   %this.GraphReadEffectGraphKeys();
   
   %itemList = effectHiddenGraphFieldsList.getSelectedItems();
   %count = getWordCount(%itemList);
  
   for(%i=%count-1;%i>=0;%i--)
   {
      %index = getWord(%itemList, %i);
      %item = effectHiddenGraphFieldsList.getItemText(%index);
      
      if(%i == 0)
         %name = %item;
      
      if(%index == -1)
      {
         error("Particle Editor Effect - Move To Visible - invalid index (" @ %index @ ")"); 
         return;
      }

      %internalName = %this.EffectReturnInternalName(%item);
      %internalNameIndex = %this.EffectGetNameIndex(%internalName);
      
      effectGraph.setGraphHidden(%internalNameIndex, false);
      
      effectHiddenGraphFieldsList.deleteItem(%index);
      effectVisibleGraphFieldsList.addItem(%item, effectGraph.getGraphColor(%internalNameIndex));
   }
   
   %name = LevelBuilderParticleEditor.EffectReturnInternalName(%name);
   %index = LevelBuilderParticleEditor.EffectGetNameIndex(%name);
   effectGraph.setSelectedPlot(%index);
}

function effectVisibleGraphFieldsList::onSelect(%this, %id, %text)
{
   %name = LevelBuilderParticleEditor.EffectReturnInternalName(%text);
   %index = LevelBuilderParticleEditor.EffectGetNameIndex(%name);
   effectGraph.setSelectedPlot(%index);
}
function emitterVisibleGraphFieldsList::onSelect(%this, %id, %text)
{
   %name = LevelBuilderParticleEditor.EmitterReturnInternalName(%text);
   %graphIndex = LevelBuilderParticleEditor.EmitterGetNameIndex(%name);
   emitterGraph.setSelectedPlot(%graphIndex);
}

function LevelBuilderParticleEditor::EmitterMoveToHidden(%this)
{
   %itemList = EmitterVisibleGraphFieldsList.getSelectedItems();
   %count = getWordCount(%itemList);

   for(%i=%count-1;%i>=0;%i--)
   {
      %index = getWord(%itemList, %i);
      %item = EmitterVisibleGraphFieldsList.getItemText(%index);
      
      if(%index == -1)
      {
         error("Particle Editor Emitter - Move To Hidden - invalid index (" @ %index @ ")"); 
         return;
      }
      
      EmitterVisibleGraphFieldsList.deleteItem(%index);
      EmitterHiddenGraphFieldsList.addItem(%item);

      %internalName = %this.EmitterReturnInternalName(%item);
      %internalNameIndex = %this.EmitterGetNameIndex(%internalName);
      
      EmitterGraph.setGraphHidden(%internalNameIndex, true);
   } 
}

function LevelBuilderParticleEditor::EmitterMoveToVisible(%this)
{
   %this.GraphReadEmitterGraphKeys();
   
   %itemList = EmitterHiddenGraphFieldsList.getSelectedItems();
   %count = getWordCount(%itemList);
  
   for(%i=%count-1;%i>=0;%i--)
   {
      %index = getWord(%itemList, %i);
      %item = EmitterHiddenGraphFieldsList.getItemText(%index);
      
      if(%i == 0)
         %name = %item;
      
      if(%index == -1)
      {
         error("invalid index"); 
         return;
      }

      %internalName = %this.EmitterReturnInternalName(%item);
      %internalNameIndex = %this.EmitterGetNameIndex(%internalName);
      
      EmitterGraph.setGraphHidden(%internalNameIndex, false);
      
      EmitterHiddenGraphFieldsList.deleteItem(%index);
      EmitterVisibleGraphFieldsList.addItem(%item, EmitterGraph.getGraphColor(%internalNameIndex));
   }
   
   %name = %this.EmitterReturnInternalName(%name);
   %index = %this.EmitterGetNameIndex(%name);
   EmitterGraph.setSelectedPlot(%index);
}

function T2DParticleEditorEmitterGuiTabBook::onTabSelected(%this, %tab)
{
   if(%tab $= "Graph")
   {
      LevelBuilderParticleEditor.GraphReadSelectedEmitterGraphKeys();   
   } else if(%tab $= "Table")
   {
      LevelBuilderParticleEditor.readEmitterGraphKeys();
   }
}

function EmitterGraph::onMouseMove(%this, %pos)
{
   EmitterGraphMousePosition.setText(%pos);
}

function EmitterGraph::onMouseDragged(%this, %pos)
{
   EmitterGraphMousePosition.setText(%pos);
}

function EmitterGraph::onSetSelected(%this, %graph)
{
   %graph = LevelBuilderParticleEditor.EmitterGraphFieldExternalNameArray[%graph];
   EmitterGraphSelectedGraph.setText(%graph);
   LevelBuilderParticleEditor.graphEmitterSelectedGraph = %graph;
   
   %name = LevelBuilderParticleEditor.EmitterReturnInternalName(%graph);
   %graphIndex = LevelBuilderParticleEditor.EmitterGetNameIndex(%name);
   
   %graphMin = EmitterGraph.getGraphMin(%graphIndex);
	  %graphMinTime = getWord(%graphMin, 0);
	  %graphMinValue = getWord(%graphMin, 1);
	  %graphMax = EmitterGraph.getGraphMax(%graphIndex);
	  %graphMaxTime = getWord(%graphMax, 0);
	  %graphMaxValue = getWord(%graphMax, 1);
   
   ParticleEditorEmitterTextEditMinValue.setText(%graphMinValue);
   ParticleEditorEmitterTextEditMinTime.setText(%graphMinTime);
   ParticleEditorEmitterTextEditMaxValue.setText(%graphMaxValue);
   ParticleEditorEmitterTextEditMaxTime.setText(%graphMaxTime);
}
$count = 0;
function EmitterGraph::onPlotPointAdded(%this, %graph, %point, %index)
{
   LevelBuilderParticleEditor.GraphAddEmitterGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
   
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;

   %undo = new UndoScriptAction(){
      class = EmitterAddGraphKey;
      actionName = "Emitter Add Graph Key"; 
      emitter = %particleEmitter;
      graph = %graph;
      point = %point;
      index = %index; 
   };
   
   %undo.addToManager(LevelBuilderUndoManager); 
}

function EmitterGraph::onPlotPointChangedUp(%this, %graph, %point, %lastIndex, %newIndex)
{
   if(!LevelBuilderParticleEditor.EmitterUpdateOnMove)
   {
      if(%lastIndex != 0)
         LevelBuilderParticleEditor.GraphDestroyEmitterGraphKey(%graph, %lastIndex);
      LevelBuilderParticleEditor.GraphAddEmitterGraphKey(%graph, getWord(%point, 0), getWord(%point, 1));
   }
   
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   %undo = new UndoScriptAction(){
      class = EmitterChangeGraphKey;
      actionName = "Emitter Change Graph Key"; 
      emitter = %particleEmitter;
      graph = %graph;
      newPoint = %point;
      newIndex = %newIndex;
      lastPoint = %this.pointOnSelected;  
      lastIndex = %this.indexOnSelected;      
   };
   
   %undo.addToManager(LevelBuilderUndoManager); 
}

function EmitterGraph::onPlotPointChangedMove(%this, %graph, %point, %lastIndex)
{
   if(LevelBuilderParticleEditor.EmitterUpdateOnMove)
   {   
      if(%lastIndex != 0)
      {
         LevelBuilderParticleEditor.GraphDestroyEmitterGraphKey(%graph, %lastIndex);         
         LevelBuilderParticleEditor.GraphAddEmitterGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
      } else
      {
         if(getWord(%point, 0) != 0)
         {
            %this.resetSelectedPoint();
         } else
         {
            LevelBuilderParticleEditor.GraphAddEmitterGraphKey(%graph, getWord(%point, 0), getWord(%point, 1)); 
         }
      }
   }
}

function EmitterGraph::onPlotPointSelectedMouseDown(%this, %index)
{
   %this.pointOnSelected = emitterGraphMousePosition.getValue();
   %this.indexOnSelected = %index;
}

function EmitterGraph::onPlotPointSelected(%this, %index)
{
   if(%index == 0)
   {
      %this.setPointXMovementClamped(true);      
   } else
   {
      %this.setPointXMovementClamped(false);      
   }
}

function EmitterGraph::onPlotPointRemoved(%this, %graph, %index, %point)
{   
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   error("point = " @ %point);
   
   %undo = new UndoScriptAction(){
      class = EmitterRemoveGraphKey;
      actionName = "Emitter Remove Graph Key"; 
      emitter = %particleEmitter;
      graph = %graph;
      point = %point;
      index = %index;   
   };
   
   %undo.addToManager(LevelBuilderUndoManager); 
   
   LevelBuilderParticleEditor.GraphDestroyEmitterGraphKey(%graph, %index); 
}

function EmitterVisibleGraphFieldsList::getItemIndex(%this, %name)
{
   %count = %this.getItemCount();

   for(%i=0;%i<%count;%i++)
   {         
      if(%name $= %this.getItemText(%i))
         return(%i);
   }
}

function EmitterHiddenGraphFieldsList::getItemIndex(%this, %name)
{
   %count = %this.getItemCount();

   for(%i=0;%i<%count;%i++)
   {   
      if(%name $= %this.getItemText(%i))
         return(%i);
   }
}
