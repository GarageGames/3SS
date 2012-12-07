//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeParticleEditor()
{
   execPrefs("particleEditorPrefs.cs");
   
   exec("./scripts/newParticleEditor.ed.cs");
   exec("./scripts/ParticleEditorEffectFunctions.ed.cs");
   exec("./scripts/ParticleEditorEmitterFunctions.ed.cs");
   exec("./scripts/undo.ed.cs");
   exec("./scripts/fileDialogs.ed.cs");
}

function destroyParticleEditor()
{
   // Export Preferences.
   echo("Exporting Particle Editor preferences.");
   $Game::CompanyName = "GarageGames";
   $Game::ProductName = "3StepStudioAlpha";   
   export("$particleEditor::*", "particleEditorPrefs.cs", false, false);
}

$particleEditor::NewEffectFileName = "newEffect.eff";
$particleEditor::layerToSave = "";

function ParticleEditor::saveEffect( %effectObject, %callback, %forceSaveAs )
{
   $particleEditor::effectToSave = %effectObject;
   
   if( %forceSaveAs $= "" )
      %forceSaveAs = false;
   
   %fileName = "newEffect.eff";
   if(%effectObject.effectFile $= "")
      %effectObject.effectFile = %fileName;
   
   // Safeguard against the rare case when the filename gets set to ".../.eff"
   // which apparently is a writeable filename, but isn't writeable.
   if( fileBase( %effectObject.effectFile ) $= "" )
      %forceSaveAs = true;
   else
      %fileName = %effectObject.effectFile;
   
   if( %forceSaveAs || !(isFile( %effectObject.effectFile ) ) )
   {
      if (fileName(%effectObject.effectFile) !$= $particleEditor::NewEffectFileName)
         if(%effectObject.effectFile !$= "") // only if its valid
            %currentFile = %effectObject.effectFile;
         else
            %currentFile = expandPath( "^project/data/particles/untitled.eff" );
      else 
         %currentFile = expandPath( "^project/data/particles/untitled.eff" );

      %fileName = ParticleEffect::getLevelSaveName( %currentFile );
   }

   if( %fileName $= "" )
      return;
       
   %effectObject.saveEffect( %fileName );
   %effectObject.onEffectSaved();
   %effectObject.loadEffect( %fileName );
   %effectObject.playEffect( true );
   
   if( %callback !$= "" )
      eval( %callback );
   
   // Update quick edit so emitters are properly referenced.
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspectUpdate" );
}
