//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

new SimSet(ParticleEditorRecycleBin);

//---------------------- Editor Undo Methods -----------------------
function AddEmitter::undo(%this)
{
   %this.effect.removeEmitter(%this.emitterID, false);
   
   ParticleEditorRecycleBin.add(%this.emitterID);
   
   if(%this.effect.getEmitterCount > 0)
      %this.effect.playEffect();
      
   %this.base.schedule( 300, setProperty, %this.effect );
}

function AddEmitter::redo(%this)
{
   %this.effect.addEmitter(%this.emitterID);
   
   ParticleEditorRecycleBin.remove(%this.emitterID);
   
   %this.effect.playEffect();
   
   %this.base.schedule( 300, setProperty, %this.effect );
}

function RemoveEmitter::undo(%this)
{
   %this.effect.addEmitter(%this.emitterID);
   
   ParticleEditorRecycleBin.remove(%this.emitterID);
   
   %this.effect.playEffect();
   
   %this.base.schedule( 300, setProperty, %this.effect );
}

function RemoveEmitter::redo(%this)
{
   %this.effect.removeEmitter(%this.emitterID, false);
   
   ParticleEditorRecycleBin.add(%this.emitterID);
   
   if(%this.effect.getEmitterCount > 0)
      %this.effect.playEffect();
      
   %this.base.schedule( 300, setProperty, %this.effect );
}

//---------------------- Effect Undo Methods -----------------------
function ChangeGraphKey::undo(%this)
{     
   // Select the graph passed
   %this.fieldsObject.selectGraph( %this.graph );
   
   // remove the data key
   %this.fieldsObject.removeDataKey( %this.newIndex );
   
   // Add Key to Graph.
	  %this.fieldsObject.addDataKey( getWord(%this.lastPoint, 0), getWord(%this.lastPoint, 1) );
	  
	  %this.editor.base.populateGraph( %this.editor.base.getSel() );
}

function ChangeGraphKey::redo(%this)
{
   // Select the graph passed
   %this.fieldsObject.selectGraph( %this.graph );
   
   %this.fieldsObject.removeDataKey( %this.lastIndex );
   
   // Add Key to Graph.
	  %this.fieldsObject.addDataKey( getWord(%this.newPoint, 0), getWord(%this.newPoint, 1) );
	  
	  %this.editor.base.populateGraph( %this.editor.base.getSel() );
}

function AddGraphKey::undo(%this)
{    
   // Select the graph passed
   %this.fieldsObject.selectGraph( %this.graph );
   
   %this.fieldsObject.removeDataKey( %this.index );
   
   %this.editor.base.populateGraph( %this.editor.base.getSel() );	
}

function AddGraphKey::redo(%this)
{ 
   // Select the graph passed
   %this.fieldsObject.selectGraph( %this.graph );
   
   // Add Key to Graph.
	  %this.fieldsObject.addDataKey( getWord(%this.point, 0), getWord(%this.point, 1) );
	  
	  %this.editor.base.populateGraph( %this.editor.base.getSel() );
}

function RemoveGraphKey::undo(%this)
{ 
   // Select the graph passed
   %this.fieldsObject.selectGraph( %this.graph );
   
   // Add Key to Graph.
	  %this.fieldsObject.addDataKey( getWord(%this.point, 0), getWord(%this.point, 1) );
	  
	  %this.editor.base.populateGraph( %this.editor.base.getSel() );	
}

function RemoveGraphKey::redo(%this)
{ 
   // Select the graph passed
   %this.fieldsObject.selectGraph( %this.graph );
   
   %this.fieldsObject.removeDataKey( %this.index );
   
   %this.editor.base.populateGraph( %this.editor.base.getSel() );	
}