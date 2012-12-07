//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBQEParticleEffect = GuiFormManager::AddFormContent( "LevelBuilderQuickEditClasses", "ParticleEffect", "LBQEParticleEffect::CreateForm", "LBQEParticleEffect::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQEParticleEffect::CreateForm( %contentCtrl, %quickEditObj )
{    
   
   %base = %contentCtrl.createBaseStack("LBQESceneObjectClass", %quickEditObj);
   %rollout = %base.createRolloutStack("Particle Effect", true);
   
   // Create Effect Persistence Buttons
   %emitterStack = %rollout.createEffectPersistence( %quickEditObj );
   
   %rollout.createSpacer(12);
      
   // Create Effect Graph Editing Button
   %button = new GuiIconButtonCtrl(ShowEffectGraphButton) {
      canSaveDynamicFields = "0";
      class  = TGBEffectFieldList;
      superClass = TGBGraphFieldList;
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "6 4";
      Extent = "16 24";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      iconBitmap = "^{EditorAssets}/gui/iconGraphLine.png";
      sizeIconToButton = "0";
      text = "Edit Effect Graph";
      textLocation = "Center";
      ButtonMargin = "12 3";
      fieldsObject = %quickEditObj;
   };
   %rollout.add( %button );
   
   %rollout.createSpacer(12);
     
   // Effect Life Mode
   %effectModeCtrl = %rollout.createEnumList("effectMode", true, "Effect Mode", "The Life Mode of the Selected Particle Effect", "ParticleEffect", "effectMode");
   %effectTimeStack = %rollout.createHideableStack("(" @ %base @ ".object.effectMode $= \"INFINITE\");");
   %effectTimeStack.addControlDependency(%effectModeCtrl);
   %effectTimeStack.createTextEditProperty( "effectTime", 3, "Effect Lifetime", "This determines the length of the particle effect life");
   %rollout.createTextEdit ("cameraIdleDistance", 3, "Idle Distance", "The distance from all cameras when the effect should become idle (0 or less disables feature)");
   %rollout.createCheckBox("effectCollisionStatus", "Use Effect Collisions");
        
   // Create Emitter Chain
   %emitterStack = %rollout.createParticleEmitterStack();
      
   %contentCtrl.add(%base);

   //*** Return back the base control to indicate we were successful
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBParticleEditToolProperties::SaveForm( %formCtrl )
{
   // Nothing.
}


function LBQuickEditContent::createEffectPersistence(%this, %object )
{
   %container = new GuiControl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorPanelLight";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "324 186";
      Extent = "230 37";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };
   
   %loadEffectButton = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      HorizSizing = "right";
      VertSizing = "bottom";
      class = ParticleLoadEffectButton;
      position = "6 7";
      Extent = "94 23";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      text = "Load Effect";
      groupNum = "-1";
      buttonType = "PushButton";
      buttonMargin = "4 4";
      iconBitmap = "^{EditorAssets}/gui/iconOpen.png";
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Right";
      textMargin = "4";
      parentObject = %this;
   };
   
   %saveEffectButton = new GuiIconButtonCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorButton";
      class = ParticleSaveEffectButton;
      HorizSizing = "left";
      VertSizing = "bottom";
      position = "130 7";
      Extent = "94 23";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      text = "Save Effect";
      groupNum = "-1";
      buttonType = "PushButton";
      buttonMargin = "4 4";
      iconBitmap = "^{EditorAssets}/gui/iconSave.png";
      iconLocation = "Left";
      sizeIconToButton = "0";
      textLocation = "Right";
      textMargin = "4";
      parentObject = %this;      
   };
   
   %this.effectObject = %object;
   
   %container.add(%loadEffectButton);
   %container.add(%saveEffectButton);
   
   %this.add(%container);
   
   return %container;
}


function ParticleSaveEffectButton::onClick( %this )
{
   if( !isObject( $editingeffectObject ) )
      return;
      
   %callback = "GuiFormManager::BroadcastContentMessage( \"LevelBuilderSidebarCreate\", 0, \"refresh\" );";
   ParticleEditor::saveEffect( $editingeffectObject, %callback, true );
}

function ParticleLoadEffectButton::onClick( %this )
{
   %effectObject = $editingeffectObject;
   if( !isObject( %effectObject ) )
      return;

   // Fetch file name to open  
   if (fileName(%effectObject.effectFile) !$= $particleEditor::NewEffectFileName)
      %currentFile = %effectObject.effectFile;
   else 
      %currentFile = "";
  
   %fileName = ParticleEffect::getLevelOpenName( %currentFile );
   
   // If none, return
   if( %fileName $= "" )
      return;
   
   %position = %effectObject.getPosition();
   %size = %effectObject.getSize();
      
   %effectObject.loadEffect( %fileName );
   %effectObject.playEffect( true );
   
   %effectObject.setSize( GetWord( %size, 0 ), GetWord( %size, 1 ) );
   %effectObject.setPosition( GetWord( %position, 0 ), GetWord( %position, 1 ) );
      

   // Update Global Effect Var      
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateEffectObject" SPC %effectObject );   
   // Update Quick Edit
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "syncQuickEdit" SPC %effectObject );
   // refresh the particle object library
   GuiFormManager::BroadcastContentMessage( "LevelBuilderSidebarCreate", 0, "refresh" );

}