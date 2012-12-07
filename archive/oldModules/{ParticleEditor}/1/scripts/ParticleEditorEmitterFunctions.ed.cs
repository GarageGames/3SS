//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Set Emitter Edit.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::setEmitterEdit(%this)
{
   // Grab the currently selected particle effect
   %particleEffect = LevelBuilderParticleEditor.getSelectedEffect();
   
   if(!isObject(%particleEffect))
      return;
   
   // Get Selected Row.
	  %row = ParticleEditorEffectEmittersList.getRowNumById(ParticleEditorEffectEmittersList.getSelectedId());
	  // Check for nothing selected.
	  if (%row == -1) return;
	
	  // Set Emitter Edit Mode.
	  %this.editMode = "EMITTER";
	  // Set Graph Edit Object to the Particle-Effect.
	  %this.setSelectedEmitter(%particleEffect.getEmitterObject(%row));
	
	  // Read Emitter Fields.	
	  %this.readEmitterFields();
	
	  // Show Emitter Panel.
	  T2DParticleEditorEmitterGuiTabBook.setVisible(true);
	  T2DParticleEditorEffectGuiTabBook.setVisible(false);	
}

//-----------------------------------------------------------------------------
// Initialize Emitter Edit.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::initEmitterEdit(%this)
{
	  // Initialise Emitter Type.
	  ParticleEditorEmitterComType.add("POINT", 0);
	  ParticleEditorEmitterComType.add("LINEX", 1);
	  ParticleEditorEmitterComType.add("LINEY", 2);
	  ParticleEditorEmitterComType.add("AREA", 3);
	
	  // Initialise Orientation.
	  ParticleEditorEmitterComOrientation.add("ALIGNED", 0);
	  ParticleEditorEmitterComOrientation.add("FIXED", 1);
	  ParticleEditorEmitterComOrientation.add("RANDOM", 2);
	
	  // Initialise Imagery.
	  ParticleEditorEmitterComImagery.add("IMAGEMAP", 0);
	  ParticleEditorEmitterComImagery.add("ANIMATION", 1);
	
	  // Source Blend Factors.
	  ParticleEditorEmitterComSrcBlend.add("ZERO", 0);
	  ParticleEditorEmitterComSrcBlend.add("ONE", 1);
	  ParticleEditorEmitterComSrcBlend.add("DST_COLOR", 2);
	  ParticleEditorEmitterComSrcBlend.add("ONE_MINUS_DST_COLOR", 3);
	  ParticleEditorEmitterComSrcBlend.add("SRC_ALPHA", 4);
	  ParticleEditorEmitterComSrcBlend.add("ONE_MINUS_SRC_ALPHA", 5);
	  ParticleEditorEmitterComSrcBlend.add("DST_ALPHA", 6);
	  ParticleEditorEmitterComSrcBlend.add("ONE_MINUS_DST_ALPHA", 7);
	  ParticleEditorEmitterComSrcBlend.add("SRC_ALPHA_SATURATE", 8);

	  // Destination Blend Factors.
	  ParticleEditorEmitterComDstBlend.add("ZERO", 0);
	  ParticleEditorEmitterComDstBlend.add("ONE", 1);
	  ParticleEditorEmitterComDstBlend.add("SRC_COLOR", 2);
	  ParticleEditorEmitterComDstBlend.add("ONE_MINUS_SRC_COLOR", 3);
	  ParticleEditorEmitterComDstBlend.add("SRC_ALPHA", 4);
	  ParticleEditorEmitterComDstBlend.add("ONE_MINUS_SRC_ALPHA", 5);
	  ParticleEditorEmitterComDstBlend.add("DST_ALPHA", 6);
	  ParticleEditorEmitterComDstBlend.add("ONE_MINUS_DST_ALPHA", 7);
	  
   ParticleEditorEmitterOptRenderAllGraphPoints.setValue(false);
	  ParticleEditorEmitterOptRenderGraphTooltip.setValue(true);
	  ParticleEditorEmitterOptAutoRemove.setValue(false);  
	  
	  ParticleEditorEmitterRadAddKeyOnMouseUp.setValue(true);
	  ParticleEditorEmitterRadAddKeyOnMouseMove.setValue(false);
}

function LevelBuilderParticleEditor::UpdateEmitterOptions(%this)
{
   %renderAllGraphPoints = ParticleEditorEmitterOptRenderAllGraphPoints.getValue();
	  %renderGraphTooltip = ParticleEditorEmitterOptRenderGraphTooltip.getValue();
	  %autoRemove = ParticleEditorEmitterOptAutoRemove.getValue();  
	  
	  %addKeyOnMouseUp = ParticleEditorEmitterRadAddKeyOnMouseUp.getValue;
	  %addKeyOnMouseMove = ParticleEditorEmitterRadAddKeyOnMouseMove.getValue(); 
	  
	  EmitterGraph.setAutoRemove(%autoRemove);
	  EmitterGraph.setRenderGraphTooltip(%renderGraphTooltip);
	  EmitterGraph.setRenderAll(%renderAllGraphPoints);
	  %this.EmitterUpdateOnMove = %addKeyOnMouseMove;
}

//-----------------------------------------------------------------------------
// Auto Apply Emitter.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::autoApplyEmitter(%this)
{
	  // Auto Apply Effect (if selected).
	  if ( %this::EmitterAutoApplyEdits )
		    %this.writeEmitterFields();
}

//-----------------------------------------------------------------------------
// Read Emitter Fields.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEmitterFields(%this)
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
	// Save Auto Apply Emitter Flag.
	%tempAutoApplyEdits = %this::EmitterAutoApplyEdits;
	// Disable Auto Apply.
	%this::EmitterAutoApplyEdits = false;
	
	// Read Name.
	ParticleEditorEmitterTextName.setValue( %particleEmitter.getEmitterName() );
	// Read Emitter Type.
	ParticleEditorEmitterComType.setSelected( ParticleEditorEmitterComType.findText( %particleEmitter.getEmitterType() ) );
	// Read Orientation.
	ParticleEditorEmitterComOrientation.setSelected( ParticleEditorEmitterComOrientation.findText( %particleEmitter.getParticleOrientation() ) );
	// Read Orientation Sub-Fields.	
	ParticleEditorEmitterTextAlignAngle.setValue( %particleEmitter.getAlignAngleOffset() );
	ParticleEditorEmitterOptAlignKeep.setValue( %particleEmitter.getAlignKeepAligned() );
	ParticleEditorEmitterTextFixedAngle.setValue( %particleEmitter.getFixedAngleOffset() );
	ParticleEditorEmitterTextRandomAngle.setValue( %particleEmitter.getRandomAngleOffset() );
	ParticleEditorEmitterTextRandomArc.setValue( %particleEmitter.getRandomArc() );
	// Read Pivot.
	ParticleEditorEmitterTextPivot.setValue( %particleEmitter.getPivotPoint() );
	// Read Fixed-Force Angle.
	ParticleEditorEmitterTextForceAngle.setValue( %particleEmitter.getFixedForceAngle() );
	// Read Imagery.
	ParticleEditorEmitterComImagery.setSelected( %particleEmitter.getUsingAnimation() ? 1 : 0 );
	
	// Read Imagery Sub-Fields.
	
	// ImageMaps...
	ParticleEditorEmitterComImageMap.clear();
	ParticleEditorEmitterComImageMapFrame.clear();
	
	// Enumerate imageMap Datablocks.
	for ( %n = 0; %n < $datablockSet.getCount(); %n++ )
	{
		// Fetch Datablock.
		%datablock = $datablockSet.getObject(%n);
		// Is it an imagemap?
		if  (%datablock.getClassName() $= "ImageAsset" )
			// Yes, so select it. 		
			ParticleEditorEmitterComImageMap.add( %datablock.getName(), %datablock.getId() );
	}
	// Select Active Image-Map.	
	ParticleEditorEmitterComImageMap.setSelected( ParticleEditorEmitterComImageMap.findText( getWord(%particleEmitter.getImageMapNameFrame(), 0) ) );

	
	// Animations...
	ParticleEditorEmitterComAnimation.clear();

	// Enumerate animation Datablocks.
	for ( %n = 0; %n < $datablockSet.getCount(); %n++ )
	{
		// Fetch Datablock.
		%datablock = $datablockSet.getObject(%n);
		// Is it an animation?
		if  (%datablock.getClassName() $= "AnimationAsset" )
			// Yes, so select it. 		
			ParticleEditorEmitterComAnimation.add( %datablock.getName(), %datablock.getId() );
	}
	// Select Active Animation.
	ParticleEditorEmitterComAnimation.setSelected( ParticleEditorEmitterComAnimation.findText( %particleEmitter.getAnimation() ) );	

	// Set Src/Dst Blend Factors.
	ParticleEditorEmitterComSrcBlend.setSelected( ParticleEditorEmitterComSrcBlend.findText( %particleEmitter.getSrcBlendFactor() ) );
	ParticleEditorEmitterComDstBlend.setSelected( ParticleEditorEmitterComDstBlend.findText( %particleEmitter.getDstBlendFactor() ) );
	
	// Read Misc Options.
	ParticleEditorEmitterOptBlendParticles.setValue( %particleEmitter.getBlendMode() );
	ParticleEditorEmitterOptIntenseParticles.setValue( %particleEmitter.getIntenseParticles() );
	ParticleEditorEmitterOptFixedAspect.setValue( %particleEmitter.getFixedAspect() );
	ParticleEditorEmitterOptSingleParticle.setValue( %particleEmitter.getSingleParticle() );
	ParticleEditorEmitterOptAttachPositionToEmitter.setValue( %particleEmitter.getAttachPositionToEmitter() );
	ParticleEditorEmitterOptAttachRotationToEmitter.setValue( %particleEmitter.getAttachRotationToEmitter() );
	ParticleEditorEmitterOptUseEffectEmission.setValue( %particleEmitter.getUseEffectEmission() );
	ParticleEditorEmitterOptRotateEmission.setValue( %particleEmitter.getLinkEmissionRotation() );
	ParticleEditorEmitterOptFirstInFrontOrder.setValue( %particleEmitter.getFirstInFrontOrder() );
	
	// Restore Auto Apply Emitter Flag.
	%this::EmitterAutoApplyEdits = %tempAutoApplyEdits;	
}

//-----------------------------------------------------------------------------
// Write Emitter Fields.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::writeEmitterFields()
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
	// Write Emitter Name.
	%particleEmitter.setEmitterName( ParticleEditorEmitterTextName.getValue() );
	// Write Emitter Type.
	%particleEmitter.setEmitterType( ParticleEditorEmitterComType.getText() );
	// Write Orientation.
	%particleEmitter.setParticleOrientationMode( ParticleEditorEmitterComOrientation.getText() );
	switch$( ParticleEditorEmitterComOrientation.getText() )
	{
		case "ALIGNED":
			%particleEmitter.setAlignAngleOffset( ParticleEditorEmitterTextAlignAngle.getValue() );
			%particleEmitter.setAlignKeepAligned( ParticleEditorEmitterOptAlignKeep.getValue() );
			
		case "FIXED":
			%particleEmitter.setFixedAngleOffset( ParticleEditorEmitterTextFixedAngle.getValue() );
			
		case "RANDOM":
			%particleEmitter.setRandomAngleOffset( ParticleEditorEmitterTextRandomAngle.getValue() );
			%particleEmitter.setRandomArc( ParticleEditorEmitterTextRandomArc.getValue() );
	}
	// Write Pivot Point.
	%particleEmitter.setPivotPoint( ParticleEditorEmitterTextPivot.getValue() );
	// Write Fixed-Force Angle.
	%particleEmitter.setFixedForceAngle( ParticleEditorEmitterTextForceAngle.getValue() );
	// Write Imagery.
	switch$( ParticleEditorEmitterComImagery.getText() )
	{
		case "IMAGEMAP":
			%particleEmitter.setImageMap( ParticleEditorEmitterComImageMap.getText(), ParticleEditorEmitterComImageMapFrame.getSelected() );
		
		case "ANIMATION":
			%particleEmitter.setAnimationName( ParticleEditorEmitterComAnimation.getText() );
	}
	
	// Set Blending.
	%particleEmitter.setBlending( ParticleEditorEmitterOptBlendParticles.getValue(), ParticleEditorEmitterComSrcBlend.getText(), ParticleEditorEmitterComDstBlend.getText() );

	// Write Misc Options.
	%particleEmitter.setIntenseParticles( ParticleEditorEmitterOptIntenseParticles.getValue() );
	%particleEmitter.setFixedAspect( ParticleEditorEmitterOptFixedAspect.getValue() );
	%particleEmitter.setSingleParticle( ParticleEditorEmitterOptSingleParticle.getValue() );
	%particleEmitter.setAttachPositionToEmitter( ParticleEditorEmitterOptAttachPositionToEmitter.getValue() );
	%particleEmitter.setAttachRotationToEmitter( ParticleEditorEmitterOptAttachRotationToEmitter.getValue() );
	%particleEmitter.setUseEffectEmission( ParticleEditorEmitterOptUseEffectEmission.getValue() );
	%particleEmitter.setLinkEmissionRotation( ParticleEditorEmitterOptRotateEmission.getValue() );
	%particleEmitter.setFirstInFrontOrder( ParticleEditorEmitterOptFirstInFrontOrder.getValue() );
}

//-----------------------------------------------------------------------------
// Read Graph Fields.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEmitterGraphFields(%this)
{
   %this.EmitterGraphFieldInternalNameArray[0] = "particlelife_base";
   %this.EmitterGraphFieldExternalNameArray[0] = "Particle Life Base";

   %this.EmitterGraphFieldInternalNameArray[1] = "particlelife_var";
   %this.EmitterGraphFieldExternalNameArray[1] = "Particle Life Var";

   %this.EmitterGraphFieldInternalNameArray[2] = "quantity_base";
   %this.EmitterGraphFieldExternalNameArray[2] = "Qauntity Base";

   %this.EmitterGraphFieldInternalNameArray[3] = "quantity_var";
   %this.EmitterGraphFieldExternalNameArray[3] = "Quantity Var";

   %this.EmitterGraphFieldInternalNameArray[4] = "sizex_base";
   %this.EmitterGraphFieldExternalNameArray[4] = "Size X Base";

   %this.EmitterGraphFieldInternalNameArray[5] = "sizex_var";
   %this.EmitterGraphFieldExternalNameArray[5] = "Size X Var";

   %this.EmitterGraphFieldInternalNameArray[6] = "sizex_life";
   %this.EmitterGraphFieldExternalNameArray[6] = "Size X Life";

   %this.EmitterGraphFieldInternalNameArray[7] = "sizey_base";
   %this.EmitterGraphFieldExternalNameArray[7] = "Size Y Base";

   %this.EmitterGraphFieldInternalNameArray[8] = "sizey_var";
   %this.EmitterGraphFieldExternalNameArray[8] = "Size Y Var";

   %this.EmitterGraphFieldInternalNameArray[9] = "sizey_life";
   %this.EmitterGraphFieldExternalNameArray[9] = "Size Y Life";

   %this.EmitterGraphFieldInternalNameArray[10] = "speed_base";
   %this.EmitterGraphFieldExternalNameArray[10] = "Speed Base";

   %this.EmitterGraphFieldInternalNameArray[11] = "speed_var";
   %this.EmitterGraphFieldExternalNameArray[11] = "Speed Var";

   %this.EmitterGraphFieldInternalNameArray[12] = "speed_life";
   %this.EmitterGraphFieldExternalNameArray[12] = "Speed Life";

   %this.EmitterGraphFieldInternalNameArray[13] = "spin_base";
   %this.EmitterGraphFieldExternalNameArray[13] = "Spin Base";

   %this.EmitterGraphFieldInternalNameArray[14] = "spin_var";
   %this.EmitterGraphFieldExternalNameArray[14] = "Spin Var";
   
   %this.EmitterGraphFieldInternalNameArray[15] = "spin_life";
   %this.EmitterGraphFieldExternalNameArray[15] = "Spin Life";
   
   %this.EmitterGraphFieldInternalNameArray[16] = "fixedforce_base";
   %this.EmitterGraphFieldExternalNameArray[16] = "Fixed Force Base";
   
   %this.EmitterGraphFieldInternalNameArray[17] = "fixedforce_var";
   %this.EmitterGraphFieldExternalNameArray[17] = "Fixed Force Var";
   
   %this.EmitterGraphFieldInternalNameArray[18] = "fixedforce_life";
   %this.EmitterGraphFieldExternalNameArray[18] = "Fixed Force Life";
   
   %this.EmitterGraphFieldInternalNameArray[19] = "randommotion_base";
   %this.EmitterGraphFieldExternalNameArray[19] = "Random Motion Base";
   
   %this.EmitterGraphFieldInternalNameArray[20] = "randommotion_var";
   %this.EmitterGraphFieldExternalNameArray[20] = "Random Motion Var";
   
   %this.EmitterGraphFieldInternalNameArray[21] = "randommotion_life";
   %this.EmitterGraphFieldExternalNameArray[21] = "Random Motion Life";
   
   %this.EmitterGraphFieldInternalNameArray[22] = "emissionforce_base";
   %this.EmitterGraphFieldExternalNameArray[22] = "Emission Force Base";
   
   %this.EmitterGraphFieldInternalNameArray[23] = "emissionforce_var";
   %this.EmitterGraphFieldExternalNameArray[23] = "Emission Force Var";
   
   %this.EmitterGraphFieldInternalNameArray[24] = "emissionangle_base";
   %this.EmitterGraphFieldExternalNameArray[24] = "Emission Angle Base";
   
   %this.EmitterGraphFieldInternalNameArray[25] = "emissionangle_var";
   %this.EmitterGraphFieldExternalNameArray[25] = "Emission Angle Var";
   
   %this.EmitterGraphFieldInternalNameArray[26] = "emissionarc_base";
   %this.EmitterGraphFieldExternalNameArray[26] = "Emission Arc Base";
   
   %this.EmitterGraphFieldInternalNameArray[27] = "emissionarc_var";
   %this.EmitterGraphFieldExternalNameArray[27] = "Emission Arc Var";
   
   %this.EmitterGraphFieldInternalNameArray[28] = "red_life";
   %this.EmitterGraphFieldExternalNameArray[28] = "Red Life";
   
   %this.EmitterGraphFieldInternalNameArray[29] = "green_life";
   %this.EmitterGraphFieldExternalNameArray[29] = "Green Life";
   
   %this.EmitterGraphFieldInternalNameArray[30] = "blue_life";
   %this.EmitterGraphFieldExternalNameArray[30] = "Blue Life";
   
   %this.EmitterGraphFieldInternalNameArray[31] = "visibility_life";
   %this.EmitterGraphFieldExternalNameArray[31] = "Visibility Life";

   %this.EmitterGraphFieldCount = 32;
						
						
   EmitterHiddenGraphFieldsList.clearItems();
   ParticleEditorEmitterTableGraphFieldsList.clearItems();
				
   for(%i=0;%i<%this.EmitterGraphFieldCount;%i++)
   {   
      emitterGraph.setGraphName(%i, %this.EmitterGraphFieldExternalNameArray[%i]);
      EmitterHiddenGraphFieldsList.addItem(%this.EmitterGraphFieldExternalNameArray[%i]);
      ParticleEditorEmitterTableGraphFieldsList.addItem(%this.EmitterGraphFieldExternalNameArray[%i]);
      EmitterGraph.setGraphHidden(%i, true);
   }
   
   %this.selectedEmitterGraphField = 0;
}

//-----------------------------------------------------------------------------
// Initialize Emitter Graphs
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::initEmitterGraphs(%this)
{
   // here we init all the graphs for the effect with the default settings
   // and set the max and min
   
   // particlelife_base
   emitterGraph.setGraphMax(0, 1, 10);   
   emitterGraph.setGraphMin(0, 0, 0);
   
   // particlelife_var
   emitterGraph.setGraphMax(1, 1, 10);   
   emitterGraph.setGraphMin(1, 0, 0);
   
   // quantity_base
   emitterGraph.setGraphMax(2, 1, 100);   
   emitterGraph.setGraphMin(2, 0, 0);
   
   // quantity_var
   emitterGraph.setGraphMax(3, 1, 100);   
   emitterGraph.setGraphMin(3, 0, 0);
   
   // sizex_base
   emitterGraph.setGraphMax(4, 1, 100);   
   emitterGraph.setGraphMin(4, 0, 0);
   
   // sizex_var
   emitterGraph.setGraphMax(5, 1, 100);   
   emitterGraph.setGraphMin(5, 0, 0);
   
   // sizex_life
   emitterGraph.setGraphMax(6, 1, 10);   
   emitterGraph.setGraphMin(6, 0, -10);
   
   // sizey_base
   emitterGraph.setGraphMax(7, 1, 100);   
   emitterGraph.setGraphMin(7, 0, 0);
   
   // sizey_var
   emitterGraph.setGraphMax(8, 1, 100);   
   emitterGraph.setGraphMin(8, 0, 0);
   
   // sizey_life
   emitterGraph.setGraphMax(9, 1, 10);   
   emitterGraph.setGraphMin(9, 0, -10);
   
   // speed_base
   emitterGraph.setGraphMax(10, 1, 50);   
   emitterGraph.setGraphMin(10, 0, 0);
   
   // speed_var
   emitterGraph.setGraphMax(11, 1, 100);   
   emitterGraph.setGraphMin(11, 0, 0);
   
   // speed_life
   emitterGraph.setGraphMax(12, 1, 50);   
   emitterGraph.setGraphMin(12, 0, 0);
   
   // spin_base
   emitterGraph.setGraphMax(13, 1, 1080);   
   emitterGraph.setGraphMin(13, 0, -1080);
   
   // spin_var
   emitterGraph.setGraphMax(14, 1, 1000);   
   emitterGraph.setGraphMin(14, 0, 0);
   
   // spin_life
   emitterGraph.setGraphMax(15, 1, 100);   
   emitterGraph.setGraphMin(15, 0, -100);
   
   // fixedforce_base
   emitterGraph.setGraphMax(16, 1, 250);   
   emitterGraph.setGraphMin(16, 0, -250);
   
   // fixedforce_var
   emitterGraph.setGraphMax(17, 1, 500);   
   emitterGraph.setGraphMin(17, 0, 0);
   
   // fixedforce_life
   emitterGraph.setGraphMax(18, 1, 100);   
   emitterGraph.setGraphMin(18, 0, -100);
   
   // randommotion_base
   emitterGraph.setGraphMax(19, 1, 1000);   
   emitterGraph.setGraphMin(19, 0, 0);
   
   // randommotion_var
   emitterGraph.setGraphMax(20, 1, 1000);   
   emitterGraph.setGraphMin(20, 0, 0);
   
   // randommotion_life
   emitterGraph.setGraphMax(21, 1, 100);   
   emitterGraph.setGraphMin(21, 0, -100);
   
   // emissionforce_base
   emitterGraph.setGraphMax(22, 1, 100);   
   emitterGraph.setGraphMin(22, 0, -100);
   
   // emissionforce_var
   emitterGraph.setGraphMax(23, 1, 200);   
   emitterGraph.setGraphMin(23, 0, 0);
   
   // emissionangle_base
   emitterGraph.setGraphMax(24, 1, 360);   
   emitterGraph.setGraphMin(24, 0, -360);
   
   // emissionangle_var
   emitterGraph.setGraphMax(25, 1, 720);   
   emitterGraph.setGraphMin(25, 0, 0);
   
   // emissionarc_base
   emitterGraph.setGraphMax(26, 1, 360);   
   emitterGraph.setGraphMin(26, 0, 0);
   
   // emissionarc_var
   emitterGraph.setGraphMax(27, 1, 720);   
   emitterGraph.setGraphMin(27, 0, 0);
   
   // red_life
   emitterGraph.setGraphMax(28, 1, 1);   
   emitterGraph.setGraphMin(28, 0, 0);
   
   // green_life
   emitterGraph.setGraphMax(29, 1, 1);   
   emitterGraph.setGraphMin(29, 0, 0);
   
   // blue_life
   emitterGraph.setGraphMax(30, 1, 1);   
   emitterGraph.setGraphMin(30, 0, 0);
   
   // visibility_life
   emitterGraph.setGraphMax(31, 1, 1);   
   emitterGraph.setGraphMin(31, 0, 0);
}


//-----------------------------------------------------------------------------
// Particle Orientation Select.
//-----------------------------------------------------------------------------
function ParticleEditorEmitterComOrientation::onSelect( %this, %id, %text )
{
	  // Show Appropriate Orientation Panel.
	  ParticleEditorEmitterPnlAligned.setVisible( %id == 0 );
	  ParticleEditorEmitterPnlFixed.setVisible( %id == 1 );
	  ParticleEditorEmitterPnlRandom.setVisible( %id == 2 );
}

//-----------------------------------------------------------------------------
// Imagery Select.
//-----------------------------------------------------------------------------
function ParticleEditorEmitterComImagery::onSelect( %this, %id, %text )
{
	  // Show Appropriate Imagery Controls.
	  ParticleEditorEmitterComImageMap.setVisible( %id == 0 );
	  ParticleEditorEmitterLblImageMap.setVisible( %id == 0 );
	  ParticleEditorEmitterComImageMapFrame.setVisible( %id == 0 );
	  ParticleEditorEmitterLblImageMapFrame.setVisible( %id == 0 );
	
	  ParticleEditorEmitterComAnimation.setVisible( %id == 1 );
	  ParticleEditorEmitterLblAnimation.setVisible( %id == 1 );
}

//-----------------------------------------------------------------------------
// ImageMap Select.
//-----------------------------------------------------------------------------
function ParticleEditorEmitterComImageMap::onSelect( %this, %id, %text )
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
	  // Fetch ImageMap Frame Count.
	  %frameCount = %id.getFrameCount();
	  // Add ImageMap Frames.
	  ParticleEditorEmitterComImageMapFrame.clear();
	  for ( %n = 0; %n < %frameCount; %n++ )
		    ParticleEditorEmitterComImageMapFrame.add( "Frame #" @ %n, %n );

	  // Fetch Current Frame.
	  %frame = getWord(%particleEmitter.getImageMapNameFrame(), 1);
	  // Select first frame if out of range.
	  if ( %frame >= %frameCount )
		    %frame = 0;
	  // Select Frame.
   ParticleEditorEmitterComImageMapFrame.setSelected( %frame );
}

function LevelBuilderParticleEditor::EmitterReturnInternalName(%this, %graph)
{
   for(%i=0;%i<%this.EmitterGraphFieldCount;%i++)
   {   
      if(%graph $= %this.EmitterGraphFieldExternalNameArray[%i])
         return(%this.EmitterGraphFieldInternalNameArray[%i]);
   }

   return false;
}

function LevelBuilderParticleEditor::EmitterGetNameIndex(%this, %graph)
{
   for(%i=0;%i<%this.EmitterGraphFieldCount;%i++)
   { 
      if(%graph $= %this.EmitterGraphFieldInternalNameArray[%i])
         return(%i);
   }  
   
   return -1; 
}

//-----------------------------------------------------------------------------
// Graph Field Select.
//-----------------------------------------------------------------------------
function ParticleEditorEmitterTableGraphFieldsList::onSelect( %this, %id, %text )
{
	  // Set Graph Field.
	  LevelBuilderParticleEditor.selectedEmitterGraphField = LevelBuilderParticleEditor.EmitterReturnInternalName(%text);
	
	  // Read Graph Keys.
	  LevelBuilderParticleEditor.readEmitterGraphKeys();
}

//-----------------------------------------------------------------------------
// Read Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::readEmitterGraphKeys(%this)
{   
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   // Clear Graph Keys.
	  ParticleEditorEmitterKeyList.clearItems();
	
	  %graph = %this.selectedEmitterGraphField;
	
   if(%graph $= 0)
      return;
	
	  // Select Effect Graph.
	  %particleEmitter.selectGraph( %graph );
	
	  // Fetch Key Count.
	  %keyCount = %particleEmitter.getDataKeyCount();
	
	  // Add Keys.
	  for ( %n = 0; %n < %keyCount; %n++ )
	  {
		    // Fetch Data Key.
		    %dataKey = %particleEmitter.getDataKey(%n);
		    // Fetch Key Components.
		    %time = getWord(%dataKey, 0);
		    %value = getWord(%dataKey, 1);
		    // Add to List.
		    ParticleEditorEmitterKeyList.addItem(%time TAB %value);
   }

   // Set Graph Time/Value Bounds.
	  %this.SelectedEmitterGraphMinTime = %particleEmitter.getMinTime();
	  %this.SelectedEmitterGraphMaxTime = %particleEmitter.getMaxTime();
	  %this.SelectedEmitterGraphMinValue = %particleEmitter.getMinValue();
	  %this.SelectedEmitterGraphMaxValue = %particleEmitter.getMaxValue();			
	  %this.SelectedEmitterGraphRepeat = %particleEmitter.getTimeRepeat();			

	  // Set Bound Info.
	  ParticleEditorEmitterTextGraphTimeBounds.setValue( %this.SelectedEmitterGraphMinTime SPC "to" SPC %this.SelectedEmitterGraphMaxTime );
	  ParticleEditorEmitterTextGraphValueBounds.setValue( %this.SelectedEmitterGraphMinValue SPC "to" SPC %this.SelectedEmitterGraphMaxValue );
	  ParticleEditorEmitterTextGraphRepeatBounds.setValue( "x" @ %this.SelectedEmitterGraphRepeat );
}

//-----------------------------------------------------------------------------
// Add Graph Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::addEmitterGraphKey(%this, %time, %value, %undoCall)
{
   if(%undoCall $= "")
      %undoCall = true;
   
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
	  // Check Time Bounds.
	  if ( %time < %this.SelectedEmitterGraphMinTime || %time > %this.SelectedEmitterGraphMaxTime )
	  {
		   // Problem so show Warning...
		   MessageBoxOK("Add Key - Invalid Time", %time SPC "key-time is outside allowed time for this graph!", "");
		   // Finish Here.
		   return;		
   }
	
	  // Check Value Bounds.
	  if ( %value < %this.SelectedEmitterGraphMinValue || %value > %this.SelectedEmitterGraphMaxValue )
	  {
		    // Problem so show Warning...
		    MessageBoxOK("Add Key - Invalid Value", %value SPC "key-value is outside allowed value for this graph!", "");
		    // Finish Here.
		    return;		
   }
   
   %index = %particleEmitter.getDataKeyCount();
   
	  // Add Key to Graph.
	  %particleEmitter.addDataKey( %time, %value );
	  
	  if(%undoCall)
	  {
	     %graph = %this.selectedEmitterGraphField;
	     %point = %time SPC %value;

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

	  // Read Graph Keys.
	  %this.readEmitterGraphKeys();	
}

//-----------------------------------------------------------------------------
// Edit Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::editEmitterGraphKey(%this)
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEmitterKeyList.getSelectedItem();
	  // Check Row Selected.
	  if ( %row == -1 ) return;
	
	  // Fetch Data Key.
	  %dataKey = %particleEmitter.getDataKey(%row);
	  // Set Dialog Time/Values.	
	  ParticleEditorEmitterTextEditGraphKeyTime.setValue( getWord(%dataKey, 0) );
	  ParticleEditorEmitterTextEditGraphKeyValue.setValue( getWord(%dataKey, 1) );
	
	  // Cannot Edit Key#0!
	  LabelEmitterEditGraphKeyTime.setVisible( (%row != 0) );
	  ParticleEditorEmitterTextEditGraphKeyTime.setVisible( (%row != 0) );
	
	  // Show Edit Dialog.
	  EmitterTableEditGraphKeyWindow.setVisible(true);
}

//-----------------------------------------------------------------------------
// Edit Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::writeEmitterGraphKey(%this, %time, %value)
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
      
	  // Get Selected Row.
	  %row = ParticleEditorEmitterKeyList.getSelectedItem();
	  // Check Row Selected.
	  if ( %row == -1 ) return;
	
	  // Select Effect Graph.
	  %particleEmitter.selectGraph( %this.selectedEmitterGraphField );
	
   %lastPoint = %particleEmitter.getDataKey( %row );
   %graph = %this.selectedEmitterGraphField;
	  %point = %time SPC %value;
	
	  // Remove the Key.
	  if ( %row != 0 )
	  {
      %particleEmitter.removeDataKey( %row );
		    
      %undo = new UndoScriptAction(){
         class = EmitterChangeGraphKey;
         actionName = "Emitter Change Graph Key"; 
         emitter = %particleEmitter;
         graph = %graph;
         newPoint = %point;
         newIndex = %row;
         lastPoint = %lastPoint;  
         lastIndex = %row;     
      };
	  } else
	  {
      %undo = new UndoScriptAction(){
         class = EmitterAddGraphKey;
         actionName = "Emitter Add Graph Key"; 
         emitter = %particleEmitter;
         graph = %graph;
         point = %row;
         index = %index;   
      };
	  }
	  
	  %undo.addToManager(LevelBuilderUndoManager); 
	  
	  // Add Graph Key.
	  %this.addEmitterGraphKey( %time, %value, false );
}

//-----------------------------------------------------------------------------
// Destroy Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::destroyEmitterGraphKey(%this)
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
	  // Get Selected Row.
	  %row = ParticleEditorEmitterKeyList.getSelectedItem();
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
   %particleEmitter.selectGraph( LevelBuilderParticleEditor.selectedEmitterGraphField );
	
   %point = %particleEmitter.getDataKey( %row );
	
   // Remove the Key.
   %particleEmitter.removeDataKey( %row );
	
   %graph = %this.selectedEmitterGraphField;
	  
   %undo = new UndoScriptAction(){
      class = EmitterRemoveGraphKey;
      actionName = "Emitter Remove Graph Key"; 
      emitter = %particleEmitter;
      graph = %graph;
      point = %point;
      index = %row;   
   };

   %undo.addToManager(LevelBuilderUndoManager); 
	
	  // Read Graph Keys.
	  %this.readEmitterGraphKeys();
}

//-----------------------------------------------------------------------------
// Clear Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::clearEmitterGraphKeys(%this)
{
   // Grab the currently selected particle effect
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   // Select Effect Graph.
	  %particleEmitter.selectGraph( LevelBuilderParticleEditor.selectedEmitterGraphField );
	
	  // Clear Data Keys.
	  %particleEmitter.clearDataKeys();
	
	  // Read Graph Keys.
	  %this.readEmitterGraphKeys();	
}

//-----------------------------------------------------------------------------
// Add Graph Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphAddEmitterGraphKey(%this, %graph, %time, %value)
{
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   %graph = %this.EmitterGetNameFromIndex(%graph);
   
   // Select the graph passed
   %particleEmitter.selectGraph( %graph );
   
   %graphMinTime = %particleEmitter.getMinTime();
   %graphMaxTime = %particleEmitter.getMaxTime();
   %graphMinValue = %particleEmitter.getMinValue();
   %graphMaxValue = %particleEmitter.getMaxValue();	
   
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
	  %particleEmitter.addDataKey( %time, %value );

	  // Read Graph Keys.
	  %this.GraphReadSelectedEmitterGraphKeys();
}

//-----------------------------------------------------------------------------
// Destroy Key.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphDestroyEmitterGraphKey(%this, %graph, %index)
{
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   %graph = %this.EmitterGetNameFromIndex(%graph);
   
	  // Check Row Selected.
	  if ( %index == -1 ) return;
	
	  // Cannot Remove Key#0!
	  if ( %index == 0 )
	  {
		    // Problem so show Warning...
		    MessageBoxOK("Remove Key - Invalid Key", "Cannot remove the first key in a graph!", "");
		    %this.GraphReadSelectedEmitterGraphKeys();
		    // Finish Here.
		    return;		
   }
	
	  // Select Emitter Graph.
	  %particleEmitter.selectGraph( %graph );
	
	  // Remove the Key.
	  %particleEmitter.removeDataKey( %index );
	
	  // Read Graph Keys.
	  %this.GraphReadSelectedEmitterGraphKeys();
}

//-----------------------------------------------------------------------------
// Read Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphReadEmitterGraphKeys(%this)
{   
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
	  
	  %itemList = EmitterHiddenGraphFieldsList.getSelectedItems();
	  %itemCount = getWordCount(%itemlist);
	  
	  for(%i=0;%i<%itemCount;%i++)
	  {
	     %item = getWord(%itemList, %i);
      %itemName = %this.EmitterReturnInternalName(EmitterHiddenGraphFieldsList.getItemText(%item));
      %graphIndex = %this.EmitterGetNameIndex(%itemName);
      
      EmitterGraph.clearGraph(%graphIndex);
         
      // Select Emitter Graph.
	     %particleEmitter.selectGraph( %itemName );
	     
      // Fetch Key Count.
	     %keyCount = %particleEmitter.getDataKeyCount();
	
	     // Add Keys.
	     for ( %n = 0; %n < %keyCount; %n++ )
	     {
         %graphMin = EmitterGraph.getGraphMin(%graphIndex);
	        %graphMinTime = getWord(%graphMin, 0);
	        %graphMinValue = getWord(%graphMin, 1);
	        %graphMax = EmitterGraph.getGraphMax(%graphIndex);
	        %graphMaxTime = getWord(%graphMax, 0);
	        %graphMaxValue = getWord(%graphMax, 1);
	        
		       // Fetch Data Key.
		       %dataKey = %particleEmitter.getDataKey(%n);
		       // Fetch Key Components.
		       %time = getWord(%dataKey, 0);
		       %value = getWord(%dataKey, 1);
		       
		       if(%time < %graphMinTime)
		       {
            EmitterGraph.setGraphMinX(%graphIndex, %time);
		       }
		       if(%time > %graphMaxTime)
		       {
            EmitterGraph.setGraphMaxX(%graphIndex, %time);
		       }
         if(%value < %graphMinValue)
		       {
            EmitterGraph.setGraphMinY(%graphIndex, %value);
		       }
		       if(%value > %graphMaxValue)
		       {
            EmitterGraph.setGraphMaxY(%graphIndex, %value);
		       }
		       
		       // Add to List.
		       EmitterGraph.addPlotPoint(%graphIndex , %time, %value, false);
      }
	  }
}

//-----------------------------------------------------------------------------
// Read Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphReadSelectedEmitterGraphKeys(%this)
{
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
   
   %selectedGraph = %this.EmitterReturnInternalName(%this.GraphGetSelectedEmitterGraph());
   
   if(%selectedGraph $= 0)
   {
      //error("invalid graph to select (" @ %selectedGraph @ ")");
      return;
   }
   
   // Clear Graph Keys.
	  EmitterGraph.clearGraph(%this.EmitterGetNameIndex(%selectedGraph));
	  
   %graphIndex = %this.EmitterGetNameIndex(%selectedGraph);
         
   // Select Emitter Graph.
	  %particleEmitter.selectGraph( %selectedGraph );

   // Fetch Key Count.
	  %keyCount = %particleEmitter.getDataKeyCount();
	
	  // Add Keys.
	  for ( %n = 0; %n < %keyCount; %n++ )
	  {
      %graphMin = EmitterGraph.getGraphMin(%graphIndex);
	     %graphMinTime = getWord(%graphMin, 0);
	     %graphMinValue = getWord(%graphMin, 1);
	     %graphMax = EmitterGraph.getGraphMax(%graphIndex);
	     %graphMaxTime = getWord(%graphMax, 0);
	     %graphMaxValue = getWord(%graphMax, 1);
	     
		    // Fetch Data Key.
		    %dataKey = %particleEmitter.getDataKey(%n);

		    // Fetch Key Components.
		    %time = getWord(%dataKey, 0);
		    %value = getWord(%dataKey, 1);
	
      if(%time < %graphMinTime)
      {
         EmitterGraph.setGraphMinX(%graphIndex, %time);
		    }
		    if(%time > %graphMaxTime)
		    {
         EmitterGraph.setGraphMaxX(%graphIndex, %time);
		    }
      if(%value < %graphMinValue)
		    {
         EmitterGraph.setGraphMinY(%graphIndex, %value);
		    }
		    if(%value > %graphMaxValue)
		    {
         EmitterGraph.setGraphMaxY(%graphIndex, %value);
		    }
		    
		    // Add to List.
		    EmitterGraph.addPlotPoint(%graphIndex , %time, %value, false);
   }
}

//-----------------------------------------------------------------------------
// Read Visible Graph Keys.
//-----------------------------------------------------------------------------
function LevelBuilderParticleEditor::GraphReadVisibleEmitterGraphKeys(%this)
{   
   // Grab the currently selected particle Emitter
   %particleEmitter = LevelBuilderParticleEditor.getSelectedEmitter();
   
   if(!isObject(%particleEmitter))
      return;
	  
	  %itemCount = EmitterVisibleGraphFieldsList.getItemCount();
	  
	  for(%i=0;%i<%itemCount;%i++)
	  {
      %itemName = %this.EmitterReturnInternalName(EmitterVisibleGraphFieldsList.getItemText(%i));
      %graphIndex = %this.EmitterGetNameIndex(%itemName);
      
      EmitterGraph.clearGraph(%graphIndex);
         
      // Select Emitter Graph.
	     %particleEmitter.selectGraph( %itemName );
	     
      // Fetch Key Count.
	     %keyCount = %particleEmitter.getDataKeyCount();
	
	     // Add Keys.
	     for ( %n = 0; %n < %keyCount; %n++ )
	     {
         %graphMin = EmitterGraph.getGraphMin(%graphIndex);
	        %graphMinTime = getWord(%graphMin, 0);
	        %graphMinValue = getWord(%graphMin, 1);
	        %graphMax = EmitterGraph.getGraphMax(%graphIndex);
	        %graphMaxTime = getWord(%graphMax, 0);
	        %graphMaxValue = getWord(%graphMax, 1);
	        
		       // Fetch Data Key.
		       %dataKey = %particleEmitter.getDataKey(%n);
		       // Fetch Key Components.
		       %time = getWord(%dataKey, 0);
		       %value = getWord(%dataKey, 1);
		       
		       if(%time < %graphMinTime)
		       {
            EmitterGraph.setGraphMinX(%graphIndex, %time);
		       }
		       if(%time > %graphMaxTime)
		       {
            EmitterGraph.setGraphMaxX(%graphIndex, %time);
		       }
         if(%value < %graphMinValue)
		       {
            EmitterGraph.setGraphMinY(%graphIndex, %value);
		       }
		       if(%value > %graphMaxValue)
		       {
            EmitterGraph.setGraphMaxY(%graphIndex, %value);
		       }
		       
		       // Add to List.
		       EmitterGraph.addPlotPoint(%graphIndex , %time, %value, false);
      }
	  }
}

function LevelBuilderParticleEditor::GraphGetSelectedEmitterGraph(%this)
{
   return LevelBuilderParticleEditor.graphEmitterSelectedGraph;   
}

function LevelBuilderParticleEditor::EmitterGetNameFromIndex(%this, %index)
{
   return %this.EmitterGraphFieldInternalNameArray[%index];
}