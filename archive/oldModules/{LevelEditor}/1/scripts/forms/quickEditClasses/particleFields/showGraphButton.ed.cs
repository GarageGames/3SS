//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function ShowEmitterGraphButton::onClick( %this )
{
   if( !isObject( %this.Graph ) || !isObject( %this.contextPopup ) )
   {
      error("EmitterGraphButton::onClick - Unable to find emitter graph or context popup resource!");
      return;
   }
   
   
   // Retrieve Global Position
   %globalPosition = %this.getParent().getGlobalPosition();
   %popupWidth = GetWord( %this.contextPopup.Dialog.getExtent(), 0 );
   
   // Show Popup at desired position
   %this.contextPopup.Show( GetWord( %globalPosition, 0 ) - %popupWidth, GetWord( %globalPosition, 1 ) );
   ToolManager.getLastWindow().setFirstResponder();
}


function DeleteEmitterGraphButton::onClick( %this )
{
   if( %this.deleting == true )
      return;
   
   if( isObject( %this.fieldsObject ) && isObject( %this.base ) )
   {
      
      //if( %this.base.object.getEmitterCount() == 1 )      
      //{
         //MessageBoxOk("Error Deleting Emitter", "Your Particle Effect must have at least one emitter!" );
         //return;
      //}
      
      %this.setActive(false);
      %this.base.setActive(false);
      
      %this.deleting = true;
    
      %this.base.object.removeEmitter( %this.fieldsObject, false );
      %this.base.object.onChanged();
      %this.base.setActive(false);
      
      %undo = new UndoScriptAction(){
         class = RemoveEmitter;
         actionName = "Remove Emitter"; 
         effect = %this.base.object;
         emitterID = %this.fieldsObject;
         base = %this.base;
      };
   
      ParticleEditorRecycleBin.add(%this.fieldsObject);
   
      %undo.addToManager(LevelBuilderUndoManager); 
      
      %this.base.schedule( 300, setProperty, %this.base.object );
   }
}

function LBQuickEditAddEmitterButton::onClick( %this )
{   
   if( %this.deleting == true )
      return;
   
   if( isObject( %this.base.object ) && isObject( %this.nameControl ) )
   {
      %this.setActive(false);
      %this.base.setActive(false);
      
      %this.deleting = true;
      
      %emitter = %this.base.object.addEmitter();
      %emitter.setEmitterName( %this.nameControl.getText() );
      %this.base.object.onChanged();
      
      %undo = new UndoScriptAction(){
         class = AddEmitter;
         actionName = "Add Emitter"; 
         effect = %this.base.object;
         emitterID = %emitter;
         base = %this.base;
      };
      
      %undo.addToManager(LevelBuilderUndoManager); 
      
      %this.base.schedule( 300, setProperty, %this.base.object );
   }
}


function ShowEffectGraphButton::onClick( %this )
{
   if( !isObject( %this.Graph ) || !isObject( %this.contextPopup ) )
   {
      error("EmitterGraphButton::onClick - Unable to find emitter graph or context popup resource!");
      return;
   }
   
   %this.fieldsObject = $editingeffectObject;
   %graph = %this.contextPopup.Dialog.findObjectByInternalName("FieldGraph");
   if( isObject( %graph ) )
      %graph.fieldsObject = $editingeffectObject;
   %this.populateGraph( %this.selItem );
   
   // Retrieve Global Position
   %globalPosition = %this.getGlobalPosition();
   %popupWidth = GetWord( %this.contextPopup.Dialog.getExtent(), 0 );
   
   // Show Popup at desired position
   %this.contextPopup.Show( GetWord( %globalPosition, 0 ) - %popupWidth, GetWord( %globalPosition, 1 ) );
   ToolManager.getLastWindow().setFirstResponder();
}
