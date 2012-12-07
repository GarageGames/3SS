//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Initialize Effect Edit.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::initEffectEdit(%this)
{
	  // Clear Life Mode.
	  ParticleEditorEffectComLifeMode.clear();	
	  // Initialize Life Mode.
	  ParticleEditorEffectComLifeMode.add("INFINITE", 0);
	  ParticleEditorEffectComLifeMode.add("CYCLE", 1);
	  
   ParticleEditorEffectOptRenderAllGraphPoints.setValue(false);
	  ParticleEditorEffectOptRenderGraphTooltip.setValue(true);
	  ParticleEditorEffectOptAutoRemove.setValue(false);  
	  
	  ParticleEditorEffectRadAddKeyOnMouseUp.setValue(true);
	  ParticleEditorEffectRadAddKeyOnMouseMove.setValue(false);
}

function LevelBuilderParticleEditor::UpdateEffectOptions(%this)
{
   %renderAllGraphPoints = ParticleEditorEffectOptRenderAllGraphPoints.getValue();
	  %renderGraphTooltip = ParticleEditorEffectOptRenderGraphTooltip.getValue();
	  %autoRemove = ParticleEditorEffectOptAutoRemove.getValue();  
	  
	  %addKeyOnMouseUp = ParticleEditorEffectRadAddKeyOnMouseUp.getValue;
	  %addKeyOnMouseMove = ParticleEditorEffectRadAddKeyOnMouseMove.getValue(); 
	  
	  effectGraph.setAutoRemove(%autoRemove);
	  effectGraph.setRenderGraphTooltip(%renderGraphTooltip);
	  effectGraph.setRenderAll(%renderAllGraphPoints);
	  %this.EmitterUpdateOnMove = %addKeyOnMouseMove;
}

//-----------------------------------------------------------------------------
// Initialize Effect Graphs
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::initEffectGraphs(%this)
{
   // here we init all the graphs for the effect with the default settings
   // first we set the max and min, we then add a point at the max and min
   
   // particlelife_scale
   effectGraph.setGraphMax(0, 1, 1);   
   effectGraph.setGraphMin(0, 0, 0);
   effectGraph.addPlotPoint(0, 0, 0);
   effectGraph.addPlotPoint(0, 1, 1);
   
   // quantity_scale
   effectGraph.setGraphMax(1, 1, 20);   
   effectGraph.setGraphMin(1, 0, 0);
   effectGraph.addPlotPoint(1, 0, 1);
   effectGraph.addPlotPoint(1, 1, 1);
   
   // sizex_scale
   effectGraph.setGraphMax(2, 1, 100);   
   effectGraph.setGraphMin(2, 0, 0);
   effectGraph.addPlotPoint(2, 0, 1);
   effectGraph.addPlotPoint(2, 1, 1);
   
   // sizey_scale
   effectGraph.setGraphMax(3, 1, 100);   
   effectGraph.setGraphMin(3, 0, 0);
   effectGraph.addPlotPoint(3, 0, 1);
   effectGraph.addPlotPoint(3, 1, 1);
   
   // speed_scale
   effectGraph.setGraphMax(4, 1, 100);   
   effectGraph.setGraphMin(4, 0, 0);
   effectGraph.addPlotPoint(4, 0, 1);
   effectGraph.addPlotPoint(4, 1, 1);
   
   // spin_scale
   effectGraph.setGraphMax(5, 1, 100);   
   effectGraph.setGraphMin(5, 0, -100);
   effectGraph.addPlotPoint(5, 0, 1);
   effectGraph.addPlotPoint(5, 1, 1);
   
   // fixedforce_scale
   effectGraph.setGraphMax(6, 1, 100);   
   effectGraph.setGraphMin(6, 0, -100);
   effectGraph.addPlotPoint(6, 0, 1);
   effectGraph.addPlotPoint(6, 1, 1);
   
   // randommotion_scale
   effectGraph.setGraphMax(7, 1, 100);   
   effectGraph.setGraphMin(7, 0, 0);
   effectGraph.addPlotPoint(7, 0, 1);
   effectGraph.addPlotPoint(7, 1, 1);
   
   // visibility_scale
   effectGraph.setGraphMax(8, 1, 100);   
   effectGraph.setGraphMin(8, 0, 0);
   effectGraph.addPlotPoint(8, 0, 1);
   effectGraph.addPlotPoint(8, 1, 1);
   
   // emissionforce_base
   effectGraph.setGraphMax(9, 1, 100);   
   effectGraph.setGraphMin(9, 0, -100);
   effectGraph.addPlotPoint(9, 0, 5);
   effectGraph.addPlotPoint(9, 1, 5);
   
   // emissionforce_var
   effectGraph.setGraphMax(10, 1, 200);   
   effectGraph.setGraphMin(10, 0, 0);
   effectGraph.addPlotPoint(10, 0, 0);
   effectGraph.addPlotPoint(10, 1, 0);
   
   // emissionangle_base
   effectGraph.setGraphMax(11, 1, 360);   
   effectGraph.setGraphMin(11, 0, -360);
   effectGraph.addPlotPoint(11, 0, 0);
   effectGraph.addPlotPoint(11, 1, 0);
   
   // emissionangle_var
   effectGraph.setGraphMax(12, 1, 720);   
   effectGraph.setGraphMin(12, 0, 0);
   effectGraph.addPlotPoint(12, 0, 0);
   effectGraph.addPlotPoint(12, 1, 0);
   
   // emissionarc_base
   effectGraph.setGraphMax(13, 1, 360);   
   effectGraph.setGraphMin(13, 0, 0);
   effectGraph.addPlotPoint(13, 0, 360);
   effectGraph.addPlotPoint(13, 1, 360);
   
   // emissionarc_var
   effectGraph.setGraphMax(14, 1, 720);   
   effectGraph.setGraphMin(14, 0, 0);
   effectGraph.addPlotPoint(14, 0, 0);
   effectGraph.addPlotPoint(14, 1, 0);
}


//-----------------------------------------------------------------------------
// Read Effect Fields.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEffectFields(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   // Read Effect Life Mode / Time.
	  %effectLife = %particleEffect.getEffectLifeMode();
	  ParticleEditorEffectComLifeMode.setSelected( ParticleEditorEffectComLifeMode.findText(getWord(%effectLife, 0)) );
   ParticleEditorEffectTextLifeModeTime.setValue( getWord(%effectLife, 1) );
   // Read Object Size.
	  ParticleEditorEffectTextSize.setValue( %particleEffect.getSize() );
}

function LevelBuilderParticleEditor::GraphGetSelectedEffectGraph(%this)
{
   return %this.graphEffectSelectedGraph;   
}

//-----------------------------------------------------------------------------
// Write Effect Fields.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::writeEffectFields(%this)
{
   error("Shouldn't get here - ::writeFields - used to write object size and effect life mode. JDD");
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   // Save Auto Apply Emitter Flag.
	  %tempAutoApplyEdits = %this::EffectAutoApplyEdits;
	  // Disable Auto Apply.
	  %this::EffectAutoApplyEdits = false;
	
	  // Write Effect Life Mode / Time.
	  %particleEffect.setEffectLifeMode( ParticleEditorEffectComLifeMode.getValue(), ParticleEditorEffectTextLifeModeTime.getValue() );
	  // Write Object Size.
	  %particleEffect.setSize( ParticleEditorEffectTextSize.getValue() );
	
	  // Restore Auto Apply Emitter Flag.
	  %this::EffectAutoApplyEdits = %tempAutoApplyEdits;	
}

//-----------------------------------------------------------------------------
// Auto Apply Effect.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::autoApplyEffect(%this)
{
	  // Auto Apply Effect (if selected).
	  if ( %this::EffectAutoApplyEdits )
		    %this.writeEffectFields();
}

//-----------------------------------------------------------------------------
// Set Effect Edit.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEffectEdit(%this)
{
	  // Set Effect Edit Mode.
	  %this.editMode = "EFFECT";
	
	  // Show Effect Panel.
	  T2DParticleEditorEmitterGuiTabBook.setVisible(false);
	  T2DParticleEditorEffectGuiTabBook.setVisible(true);	
	
	  // Read Emitter List.
	  %this.readEmitterList();
	  // Read Graph Fields.
	  //%this.readEffectGraphFields();
}


//-----------------------------------------------------------------------------
// Read Emitter List.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEmitterList(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Clear Emitter List.
	  ParticleEditorEffectEmittersList.clear();

	  // Fetch Emitter Count.
	  %emitterCount = %particleEffect.getEmitterCount();
	
	  // Add Emitter Name.
	  for ( %n = 0; %n < %emitterCount; %n++ )
	  {
		    // Fetch Emitter Object.
		    %emitterObject = %particleEffect.getEmitterObject(%n);
		    // Formulate Name / Visibility Flag.
		    if ( %emitterObject.getVisible() )
			      %emitterDesc = strupr(%emitterObject.getEmitterName());
		    else
			      %emitterDesc = "(hidden)" SPC strlwr(%emitterObject.getEmitterName());
		
      // Add to List.
      ParticleEditorEffectEmittersList.addRow( %n, %emitterDesc );
   }
}


//-----------------------------------------------------------------------------
// Set Emitter Up.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEmitterUp(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectEmittersList.getSelectedId();
	  // Check for nothing selected.
	  if (%row == -1) return;
	
	  // Cannot move first emitter up!
	  if ( %row == 0 )
		    return;
		
	  // Move Emitter Down.
	  %particleEffect.moveEmitter( %row, %row-1 );
	
	  // Read Emitter List.
	  %this.readEmitterList();
}


//-----------------------------------------------------------------------------
// Set Emitter Down.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEmitterDown(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectEmittersList.getSelectedId();
	  // Check for nothing selected.
	  if (%row == -1) return;
	
	  // Cannot move last emitter down!
	  if ( %row == (ParticleEditorEffectEmittersList.rowCount()-1) )
		    return;
		
	  // Move Emitter Down.
	  %particleEffect.moveEmitter( %row, %row+1 );
	
	  // Read Emitter List.
	  %this.readEmitterList();
}


//-----------------------------------------------------------------------------
// Set Emitter Visibility.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEmitterVisibility(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectEmittersList.getSelectedId();
	  // Check for nothing selected.
	  if (%row == -1) return;
	
	  // Fetch Emitter Object.
	  %emitterObject = %particleEffect.getEmitterObject(%row);
	
	  // Toggle Visibility.
	  %emitterObject.setVisible( !%emitterObject.getVisible() );

	  // Read Emitter List.
	  %this.readEmitterList();
	
	  // Play Effect.
	  %particleEffect.playEffect();
}


//-----------------------------------------------------------------------------
// Set Emitter Show All.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEmitterShowAll(%this, %status)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Show All Emitters.
	  for ( %n = 0; %n < ParticleEditorEffectEmittersList.rowCount(); %n++ )
		    %particleEffect.getEmitterObject(%n).setVisible(%status);

	  // Read Emitter List.
	  %this.readEmitterList();
	
	  // Play Effect.
	  %particleEffect.playEffect();
}


//-----------------------------------------------------------------------------
// Set Emitter Solo.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEmitterSolo(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectEmittersList.getSelectedId();
	  // Check for nothing selected.
	  if (%row == -1) return;
	
	  // Hide All Emitters.
	  for ( %n = 0; %n < ParticleEditorEffectEmittersList.rowCount(); %n++ )
		   %particleEffect.getEmitterObject(%n).setVisible(false);
	
	  // Show Selected Emitter Object.
	  %particleEffect.getEmitterObject(%row).setVisible(true);

	  // Read Emitter List.
	  %this.readEmitterList();
	
	  // Play Effect.
	  %particleEffect.playEffect();
}

//-----------------------------------------------------------------------------
// Create Emitter.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::createEmitter(%this, %emitterName )
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
      
   %emitterIndexNum = %particleEffect.getEmitterCount();
      
   // Add Emitter.
	  %emitterIndex = %particleEffect.addEmitter();

	  // Set Emitter Name (if we've got one).
	  if ( %emitterName !$= "" )
		    %emitterIndex.setEmitterName( %emitterName );
	
	  // Read Emitter List.
	  %this.readEmitterList();
	
  	// Select Emitter.
	  // NOTE:- Emitter Index / Row should be the same.
	  ParticleEditorEffectEmittersList.setSelectedRow( %emitterIndexNum );
	
	  %emitterID = %emitterIndex;
	
   %undo = new UndoScriptAction(){
      class = ParticleEditorAddEmitter;
      actionName = "Particle Editor Add Emitter"; 
      effect = %particleEffect;
      emitterID = %emitterID;
   };
   
   %undo.addToManager(LevelBuilderUndoManager); 
	
	  // Play Effect.
	  // NOTE:- Adding an emitter causes the effect to stop.
	  %particleEffect.playEffect();		
}


//-----------------------------------------------------------------------------
// Destroy Emitter.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::destroyEmitter(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectEmittersList.getSelectedId();
	  // Check for nothing selected.
	  if (%row == -1) return;
	
	  %emitterID = %particleEffect.getEmitterObject(%row);
	
	  // Remove the Emitter.
	  %particleEffect.removeEmitter( %emitterID, false );
	  
   %undo = new UndoScriptAction(){
      class = ParticleEditorRemoveEmitter;
      actionName = "Particle Editor Remove Emitter"; 
      effect = %particleEffect;
      emitterID = %emitterID;
   };
   
   ParticleEditorRecycleBin.add(%emitterID);
   
   %undo.addToManager(LevelBuilderUndoManager); 
	
	  // Read Emitter List.
	  %this.readEmitterList();
	
	  // Play Effect.
	  // NOTE:- Adding an emitter causes the effect to stop.
	  if(%particleEffect.getEmitterCount() > 0)
	     %particleEffect.playEffect();	
}


//-----------------------------------------------------------------------------
// Clear Emitter.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::ClearEmitters(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Clear Emitters from Effect.
	  %particleEffect.clearEmitters();

	  // Reinitialize GUI.
   %this.reinitializeGUI();
}

function LevelBuilderParticleEditor::initializeGui(%this)
{
	  // Initialise Effect Edit.
	  %this.initEffectEdit();
	
	  %this.initEffectGraphs();
	
	  // Initialise Emitter Edit.
	  %this.initEmitterEdit();
	
	  %this.initEmitterGraphs();
	
	  // Read Effect Fields.
	  %this.readEffectFields();

	  // Read Emitter List.
	  %this.readEmitterList();
		
   // Read Graph Fields.
   %this.readEmitterGraphFields();
 
   // Read Graph Fields.
   %this.readEffectGraphFields();
		
		 T2DParticleEditorEffectGuiTabBook.selectPage(0);
		 T2DParticleEditorEmitterGuiTabBook.selectPage(0);
		 T2DParticleEditorEmitterSettingsGuiTabBook.selectPage(0);
		
	  // Set Effect Edit.
	  %this.setEffectEdit();
	
   %this.UpdateEffectOptions();
   %this.UpdateEmitterOptions();
    
    // Set Editor Background.
    //setEditorBackground( $particleEditor::background );
    
   if(!isObject(ParticleEditorRecycleBin))
      new SimSet(ParticleEditorRecycleBin);
}

//-----------------------------------------------------------------------------
// Reinitialize GUI.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::reinitializeGUI(%this)
{
	  // Read Effect Fields.
	  %this.readEffectFields();

	  // Read Emitter List.
	  %this.readEmitterList();
	
	  // Set Effect Edit.
	  %this.setEffectEdit();
	
	  // Read Graph Keys.
	  %this.readEffectGraphKeys();	
	  
	  %this.UpdateEffectOptions();
	  %this.UpdateEmitterOptions();
}

//-----------------------------------------------------------------------------
// Read Graph Fields.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEffectGraphFields(%this)
{   
   %this.EffectGraphFieldInternalNameArray[0] = "particlelife_scale";
   %this.EffectGraphFieldExternalNameArray[0] = "Particle Life Scale";

   %this.EffectGraphFieldInternalNameArray[1] = "quantity_scale";
   %this.EffectGraphFieldExternalNameArray[1] = "Quantity Scale";

   %this.EffectGraphFieldInternalNameArray[2] = "sizex_scale";
   %this.EffectGraphFieldExternalNameArray[2] = "Size X Scale";

   %this.EffectGraphFieldInternalNameArray[3] = "sizey_scale";
   %this.EffectGraphFieldExternalNameArray[3] = "Size Y Scale";

   %this.EffectGraphFieldInternalNameArray[4] = "speed_scale";
   %this.EffectGraphFieldExternalNameArray[4] = "Speed Scale";

   %this.EffectGraphFieldInternalNameArray[5] = "spin_scale";
   %this.EffectGraphFieldExternalNameArray[5] = "Spin Scale";

   %this.EffectGraphFieldInternalNameArray[6] = "fixedforce_scale";
   %this.EffectGraphFieldExternalNameArray[6] = "Fixed Force Scale";

   %this.EffectGraphFieldInternalNameArray[7] = "randommotion_scale";
   %this.EffectGraphFieldExternalNameArray[7] = "Random Motion Scale";

   %this.EffectGraphFieldInternalNameArray[8] = "visibility_scale";
   %this.EffectGraphFieldExternalNameArray[8] = "Visibility Scale";

   %this.EffectGraphFieldInternalNameArray[9] = "emissionforce_base";
   %this.EffectGraphFieldExternalNameArray[9] = "Emission Force Base";

   %this.EffectGraphFieldInternalNameArray[10] = "emissionforce_var";
   %this.EffectGraphFieldExternalNameArray[10] = "Emission Force Variation";

   %this.EffectGraphFieldInternalNameArray[11] = "emissionangle_base";
   %this.EffectGraphFieldExternalNameArray[11] = "Emission Angle Base";

   %this.EffectGraphFieldInternalNameArray[12] = "emissionangle_var";
   %this.EffectGraphFieldExternalNameArray[12] = "Emission Angle Variation";

   %this.EffectGraphFieldInternalNameArray[13] = "emissionarc_base";
   %this.EffectGraphFieldExternalNameArray[13] = "Emission Arc Base";

   %this.EffectGraphFieldInternalNameArray[14] = "emissionarc_var";
   %this.EffectGraphFieldExternalNameArray[14] = "Emission Arc Variation";

   %this.EffectGraphFieldCount = 15;
   
   effectHiddenGraphFieldsList.clearItems();
   ParticleEditorEffectTableGraphFieldsList.clearItems();
						
   for(%i=0;%i<%this.EffectGraphFieldCount;%i++)
   {   
      effectGraph.setGraphName(%i, %this.EffectGraphFieldExternalNameArray[%i]);
      effectHiddenGraphFieldsList.addItem(%this.EffectGraphFieldExternalNameArray[%i]);
      ParticleEditorEffectTableGraphFieldsList.addItem(%this.EffectGraphFieldExternalNameArray[%i]);
      effectGraph.setGraphHidden(%i, true);
   }
   
   %this.selectedEffectGraphField = 0;
}

function LevelBuilderParticleEditor::EffectGetNameFromIndex(%this, %index)
{
   return %this.EffectGraphFieldInternalNameArray[%index];
}

//-----------------------------------------------------------------------------
// Read Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEffectGraphKeys(%this)
{   
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   // Clear Graph Keys.
	  ParticleEditorEffectKeyList.clearItems();
	
	  // Select Effect Graph.
	  %particleEffect.selectGraph( %this.selectedEffectGraphField );
	
	  // Fetch Key Count.
	  %keyCount = %particleEffect.getDataKeyCount();
	
	  // Add Keys.
	  for ( %n = 0; %n < %keyCount; %n++ )
	  {
		    // Fetch Data Key.
		    %dataKey = %particleEffect.getDataKey(%n);
		    // Fetch Key Components.
		    %time = getWord(%dataKey, 0);
		    %value = getWord(%dataKey, 1);
		    // Add to List.
		    ParticleEditorEffectKeyList.addItem(%time TAB %value);
   }

   // Set Graph Time/Value Bounds.
	  %this.SelectedEffectGraphMinTime = %particleEffect.getMinTime();
	  %this.SelectedEffectGraphMaxTime = %particleEffect.getMaxTime();
	  %this.SelectedEffectGraphMinValue = %particleEffect.getMinValue();
	  %this.SelectedEffectGraphMaxValue = %particleEffect.getMaxValue();			
	  %this.SelectedEffectGraphRepeat = %particleEffect.getTimeRepeat();			

	  // Set Bound Info.
	  ParticleEditorEffectTextGraphTimeBounds.setValue( %this.SelectedEffectGraphMinTime SPC "to" SPC %this.SelectedEffectGraphMaxTime );
	  ParticleEditorEffectTextGraphValueBounds.setValue( %this.SelectedEffectGraphMinValue SPC "to" SPC %this.SelectedEffectGraphMaxValue );
	  ParticleEditorEffectTextGraphRepeatBounds.setValue( "x" @ %this.SelectedEffectGraphRepeat );
}

//-----------------------------------------------------------------------------
// Read Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphReadEffectGraphKeys(%this)
{   
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
	
	  %itemList = effectHiddenGraphFieldsList.getSelectedItems();
	  %itemCount = getWordCount(%itemlist);
	  
	  for(%i=0;%i<%itemCount;%i++)
	  {
	     %item = getWord(%itemList, %i);
      %itemName = %this.EffectReturnInternalName(effectHiddenGraphFieldsList.getItemText(%item));
      %graphIndex = %this.EffectGetNameIndex(%itemName);
      
      effectGraph.clearGraph(%graphIndex);
         
      // Select Effect Graph.
	     %particleEffect.selectGraph( %itemName );
	     
      // Fetch Key Count.
	     %keyCount = %particleEffect.getDataKeyCount();
	
	     // Add Keys.
	     for ( %n = 0; %n < %keyCount; %n++ )
	     {
	        %graphMin = EffectGraph.getGraphMin(%graphIndex);
	        %graphMinTime = getWord(%graphMin, 0);
	        %graphMinValue = getWord(%graphMin, 1);
	        %graphMax = EffectGraph.getGraphMax(%graphIndex);
         %graphMaxTime = getWord(%graphMax, 0);
	        %graphMaxValue = getWord(%graphMax, 1);
	        
		       // Fetch Data Key.
		       %dataKey = %particleEffect.getDataKey(%n);
		       // Fetch Key Components.
		       %time = getWord(%dataKey, 0);
		       %value = getWord(%dataKey, 1);
		       
         if(%time < %graphMinTime)
         {
            EffectGraph.setGraphMinX(%graphIndex, %time);
		       }
		       if(%time > %graphMaxTime)
		       {
            EffectGraph.setGraphMaxX(%graphIndex, %time);
		       }
         if(%value < %graphMinValue)
		       {
            EffectGraph.setGraphMinY(%graphIndex, %value);
		       }
		       if(%value > %graphMaxValue)
		       {
            EffectGraph.setGraphMaxY(%graphIndex, %value);
		       }
		       
		       // Add to List.
		       effectGraph.addPlotPoint(%graphIndex , %time, %value, false);
      }
	  }
}

//-----------------------------------------------------------------------------
// Read Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphReadSelectedEffectGraphKeys(%this)
{  
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   %selectedGraph = %this.EffectReturnInternalName(%this.GraphGetSelectedEffectGraph());
   
   if(%selectedGraph $= 0)
   {
      //error("invalid graph to select (" @ %selectedGraph @ ")");
      return;
   }
   
   // Clear Graph Keys.
	  effectGraph.clearGraph(%this.EffectGetNameIndex(%selectedGraph));
	  
   %graphIndex = %this.EffectGetNameIndex(%selectedGraph);
         
   // Select Effect Graph.
	  %particleEffect.selectGraph( %selectedGraph );
	     
   // Fetch Key Count.
	  %keyCount = %particleEffect.getDataKeyCount();

	  // Add Keys.
	  for ( %n = 0; %n < %keyCount; %n++ )
	  {
      %graphMin = EffectGraph.getGraphMin(%graphIndex);
	     %graphMinTime = getWord(%graphMin, 0);
	     %graphMinValue = getWord(%graphMin, 1);
	     %graphMax = EffectGraph.getGraphMax(%graphIndex);
	     %graphMaxTime = getWord(%graphMax, 0);
	     %graphMaxValue = getWord(%graphMax, 1);
	     
		    // Fetch Data Key.
		    %dataKey = %particleEffect.getDataKey(%n);
	
		    // Fetch Key Components.
		    %time = getWord(%dataKey, 0);
		    %value = getWord(%dataKey, 1);
		    
      if(%time < %graphMinTime)
      {
         EffectGraph.setGraphMinX(%graphIndex, %time);
		    }
		    if(%time > %graphMaxTime)
		    {
         EffectGraph.setGraphMaxX(%graphIndex, %time);
		    }
      if(%value < %graphMinValue)
		    {
         EffectGraph.setGraphMinY(%graphIndex, %value);
		    }
		    if(%value > %graphMaxValue)
		    {
         EffectGraph.setGraphMaxY(%graphIndex, %value);
		    }
		    
		    // Add to List.
		    effectGraph.addPlotPoint(%graphIndex , %time, %value, false);
   }

}

//-----------------------------------------------------------------------------
// Read Visible Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphReadVisibleEffectGraphKeys(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
	
	  %itemCount = effectVisibleGraphFieldsList.getItemCount();
	  
	  for(%i=0;%i<%itemCount;%i++)
	  {
      %itemName = %this.EffectReturnInternalName(effectVisibleGraphFieldsList.getItemText(%i));
      %graphIndex = %this.EffectGetNameIndex(%itemName);
      
      effectGraph.clearGraph(%graphIndex);
         
      // Select Effect Graph.
	     %particleEffect.selectGraph( %itemName );
	     
      // Fetch Key Count.
	     %keyCount = %particleEffect.getDataKeyCount();
	
	     // Add Keys.
	     for ( %n = 0; %n < %keyCount; %n++ )
	     {
	        %graphMin = EffectGraph.getGraphMin(%graphIndex);
	        %graphMinTime = getWord(%graphMin, 0);
	        %graphMinValue = getWord(%graphMin, 1);
	        %graphMax = EffectGraph.getGraphMax(%graphIndex);
         %graphMaxTime = getWord(%graphMax, 0);
	        %graphMaxValue = getWord(%graphMax, 1);
	        
		       // Fetch Data Key.
		       %dataKey = %particleEffect.getDataKey(%n);
		       // Fetch Key Components.
		       %time = getWord(%dataKey, 0);
		       %value = getWord(%dataKey, 1);
		       
         if(%time < %graphMinTime)
         {
            EffectGraph.setGraphMinX(%graphIndex, %time);
		       }
		       if(%time > %graphMaxTime)
		       {
            EffectGraph.setGraphMaxX(%graphIndex, %time);
		       }
         if(%value < %graphMinValue)
		       {
            EffectGraph.setGraphMinY(%graphIndex, %value);
		       }
		       if(%value > %graphMaxValue)
		       {
            EffectGraph.setGraphMaxY(%graphIndex, %value);
		       }
		       
		       // Add to List.
		       effectGraph.addPlotPoint(%graphIndex , %time, %value, false);
      }
	  }
}


//-----------------------------------------------------------------------------
// Graph Field Select.
//-----------------------------------------------------------------------------
function ParticleEditorEffectTableGraphFieldsList::onSelect( %this, %id, %text )
{
	  // Set Graph Field.
	  LevelBuilderParticleEditor.selectedEffectGraphField = LevelBuilderParticleEditor.EffectReturnInternalName(%text);
	
	  // Read Graph Keys.
	  LevelBuilderParticleEditor.readEffectGraphKeys();
}

//-----------------------------------------------------------------------------
// Add Graph Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::addEffectGraphKey(%this, %time, %value, %undoCall)
{
   if(%undoCall $= "")
      %undoCall = true;
   
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Check Time Bounds.
	  if ( %time < %this.SelectedEffectGraphMinTime || %time > %this.SelectedEffectGraphMaxTime )
	  {
		   // Problem so show Warning...
		   MessageBoxOK("Add Key - Invalid Time", %time SPC "key-time is outside allowed time for this graph!", "");
		   // Finish Here.
		   return;		
   }
	
	  // Check Value Bounds.
	  if ( %value < %this.SelectedEffectGraphMinValue || %value > %this.SelectedEffectGraphMaxValue )
	  {
		    // Problem so show Warning...
		    MessageBoxOK("Add Key - Invalid Value", %value SPC "key-value is outside allowed value for this graph!", "");
		    // Finish Here.
		    return;		
   }
   
   %index = %particleEffect.getDataKeyCount();
   
	  // Add Key to Graph.
	  %particleEffect.addDataKey( %time, %value );
	  
	  if(%undoCall)
	  {
	     %graph = %this.selectedEffectGraphField;
	     %point = %time SPC %value;
	  
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
   
	  // Read Graph Keys.
	  %this.readEffectGraphKeys();	
}

//-----------------------------------------------------------------------------
// Add Graph Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphAddEffectGraphKey(%this, %graph, %time, %value)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   %graph = %this.EffectGetNameFromIndex(%graph);
   
   if(%graph $= 0)
      return;
   
   // Select the graph passed
   %particleEffect.selectGraph( %graph );
   
   %graphMinTime = %particleEffect.getMinTime();
   %graphMaxTime = %particleEffect.getMaxTime();
   %graphMinValue = %particleEffect.getMinValue();
   %graphMaxValue = %particleEffect.getMaxValue();	
   
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
	  %particleEffect.addDataKey( %time, %value );

	  // Read Graph Keys.
	  %this.GraphReadSelectedEffectGraphKeys();
}

//-----------------------------------------------------------------------------
// Edit Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::editEffectGraphKey(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectKeyList.getSelectedItem();
	  // Check Row Selected.
	  if ( %row == -1 ) return;
	
	  // Fetch Data Key.
	  %dataKey = %particleEffect.getDataKey(%row);
	  // Set Dialog Time/Values.	
	  ParticleEditorEffectTextEditGraphKeyTime.setValue( getWord(%dataKey, 0) );
	  ParticleEditorEffectTextEditGraphKeyValue.setValue( getWord(%dataKey, 1) );
	
	  // Cannot Edit Key#0!
	  LabelEffectEditGraphKeyTime.setVisible( (%row != 0) );
	  ParticleEditorEffectTextEditGraphKeyTime.setVisible( (%row != 0) );
	
	  // Show Edit Dialog.
	  EffectTableEditGraphKeyWindow.setVisible(true);
}

//-----------------------------------------------------------------------------
// Edit Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::writeEffectGraphKey(%this, %time, %value)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
      
	  // Get Selected Row.
	  %row = ParticleEditorEffectKeyList.getSelectedItem();
	  // Check Row Selected.
	  if ( %row == -1 ) return;
	
	  // Select Effect Graph.
	  %particleEffect.selectGraph( %this.selectedEffectGraphField );
	
	  %lastPoint = %particleEffect.getDataKey( %row );
   %graph = %this.selectedEffectGraphField;
	  %point = %time SPC %value;
	
	  // Remove the Key.
	  if ( %row != 0 )
	  {
      %particleEffect.removeDataKey( %row );
		   
      %undo = new UndoScriptAction(){
         class = EffectChangeGraphKey;
         actionName = "Effect Change Graph Key"; 
         effect = %particleEffect;
         graph = %graph;
         newPoint = %point;
         newIndex = %row;
         lastPoint = %lastPoint;  
         lastIndex = %row;      
      };
		 } else
		 {
      %undo = new UndoScriptAction(){
         class = EffectAddGraphKey;
         actionName = "Effect Add Graph Key"; 
         effect = %particleEffect;
         graph = %graph;
         point = %row;
         index = %index;   
      };
		 }
		 
		 %undo.addToManager(LevelBuilderUndoManager); 
	
	  // Add Graph Key.
	  %this.addEffectGraphKey( %time, %value, false );
}

//-----------------------------------------------------------------------------
// Destroy Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::destroyEffectGraphKey(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEffectKeyList.getSelectedItem();
	  // Check Row Selected.
	  if ( %row == -1 ) return;
	
	  // Cannot Remove Key#0!
	  if ( %row == 0 )
	  {
		    // Problem so show Warning...
		    MessageBoxOK("Remove Key - Invalid Key", "Cannot remove the first key in a graph!", "");
		    // Finish Here.
		    return;		
   }
	
	  // Select Effect Graph.
	  %particleEffect.selectGraph( %this.selectedEffectGraphField );
	
	  %point = %particleEffect.getDataKey( %row );
	
	  // Remove the Key.
	  %particleEffect.removeDataKey( %row );
		  
   %graph = %this.selectedEffectGraphField;
	  
   %undo = new UndoScriptAction(){
      class = EffectRemoveGraphKey;
      actionName = "Effect Remove Graph Key"; 
      effect = %particleEffect;
      graph = %graph;
      point = %point;
      index = %row;   
   };

   %undo.addToManager(LevelBuilderUndoManager); 
	
	  // Read Graph Keys.
	  %this.readEffectGraphKeys();
}

//-----------------------------------------------------------------------------
// Clear Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::clearEffectGraphKeys(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   // Select Effect Graph.
	  %particleEffect.selectGraph( %this.selectedEffectGraphField );
	
	  // Clear Data Keys.
	  %particleEffect.clearDataKeys();
	
	  // Read Graph Keys.
	  %this.readEffectGraphKeys();
}

//-----------------------------------------------------------------------------
// Destroy Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphDestroyEffectGraphKey(%this, %graph, %index)
{
   // Grab the currently selected particle effect
   %particleEffect = %this.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   %graph = %this.EffectGetNameFromIndex(%graph);
   
	  // Check Row Selected.
	  if ( %index == -1 ) return;
	
	  // Cannot Remove Key#0!
	  if ( %index == 0 )
	  {
		    // Problem so show Warning...
		    MessageBoxOK("Remove Key - Invalid Key", "Cannot remove the first key in a graph!", "");
		    // Finish Here.
		    return;		
   }
	
   if(%graph $= 0)
      return;
	
	  // Select Effect Graph.
	  %particleEffect.selectGraph( %graph );
	
	  // Remove the Key.
	  %particleEffect.removeDataKey( %index );
	
	  // Read Graph Keys.
	  %this.GraphReadSelectedEffectGraphKeys();
}


function LevelBuilderParticleEditor::EffectReturnInternalName(%this, %graph)
{
   for(%i=0;%i<%this.EffectGraphFieldCount;%i++)
   {   
      if(%graph $= %this.EffectGraphFieldExternalNameArray[%i])
         return(%this.EffectGraphFieldInternalNameArray[%i]);
   }

   return false;
}

function LevelBuilderParticleEditor::EffectGetNameIndex(%this, %graph)
{
   for(%i=0;%i<%this.EffectGraphFieldCount;%i++)
   { 
      if(%graph $= %this.EffectGraphFieldInternalNameArray[%i])
         return(%i);
   }  
   
   return -1; 
}