//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function LevelBuilderSceneEdit::createUndo( %this, %objects, %fields, %name )
{
   // Create a new SimSet for the objects.
   %set = new SimSet();
   %count = %objects.getCount();
   %fieldCount = getWordCount( %fields );
   for( %i = 0; %i < %count; %i++ )
   {
      %ref = %objects.getObject( %i );
      %object = new ScriptObject()
      {
         ref = %ref;
      };
      
      for( %f = 0; %f < %fieldCount; %f++ )
      {
         %field = getWord( %fields, %f );
         %object.start[%field] = %object.ref.getFieldValue( %field );
      }
      
      %set.add( %object );
   }
      
   %undo = new UndoScriptAction()
   {
      class = "LevelBuilderUndoAction";
      objects = %set;
      fields = %fields;
      actionName = %name; 
   };
   
   return %undo;
}

function LevelBuilderUndoAction::finish( %this )
{
   %count = %this.objects.getCount();
   %fieldCount = getWordCount( %this.fields );
   %different = false;
   for( %i = 0; %i < %count; %i++ )
   {
      %object = %this.objects.getObject( %i );
      
      for( %f = 0; %f < %fieldCount; %f++ )
      {
         %field = getWord( %this.fields, %f );
         %object.end[%field] = %object.ref.getFieldValue( %field );
         
         if( %object.start[%field] !$= %object.end[%field] )
            %different = true;
      }
   }

   if( %different )
      %this.addToManager( LevelBuilderUndoManager );
   else
   {
      %this.objects.delete();
      %this.schedule( 0, delete );
   }
}

function LevelBuilderUndoAction::undo( %this )
{
   %count = %this.objects.getCount();
   %fieldCount = getWordCount( %this.fields );
   for( %i = 0; %i < %count; %i++ )
   {
      %object = %this.objects.getObject( %i );
      
      for( %f = 0; %f < %fieldCount; %f++ )
      {
         %field = getWord( %this.fields, %f );
         %object.ref.setFieldValue( %field, %object.start[%field] );
      }
   }
   
   ToolManager.updateAcquiredObjectSet();
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateExisting" );
}

function LevelBuilderUndoAction::redo( %this )
{
   %count = %this.objects.getCount();
   %fieldCount = getWordCount( %this.fields );
   for( %i = 0; %i < %count; %i++ )
   {
      %object = %this.objects.getObject( %i );
      
      for( %f = 0; %f < %fieldCount; %f++ )
      {
         %field = getWord( %this.fields, %f );
         %object.ref.setFieldValue( %field, %object.end[%field] );
      }
   }
   
   ToolManager.updateAcquiredObjectSet();
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateExisting" );
}
