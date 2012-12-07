//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Subscribe to application close event
if( isObject( LBProjectObj ) )
{
   Input::GetEventManager().subscribe( LBProjectObj, "ClosePressed" );   
   Input::GetEventManager().subscribe( LBProjectObj, "BeginShutdown" );
}


// Called when the application is beginning to shut down - Before onExit is called
function T2DProject::onBeginShutdown( %this )
{   
   // Prompt to save changes
   if( LevelBuilderUndoManager.getUndoCount() > 0 || LevelBuilderUndoManager.getRedoCount() > 0 )
   {
      %file = %this.currentLevelFile;
      %file = fileName( %file );
      %mbResult = checkSaveChanges( %file , false );
      if( %mbResult $= $MROk )
         %this.saveLevel();
   }  
}

// Called when the application is beginning to shut down - Before onExit is called
function T2DProject::onClosePressed( %this )
{   
   // Prompt to save changes
   if( LevelBuilderUndoManager.getUndoCount() > 0 || LevelBuilderUndoManager.getRedoCount() > 0 )
   {
      Canvas.setContent( LevelBuilderBase );
      Canvas.repaint();
      
      %file = %this.currentLevelFile;
      %file = fileName( %file );
      %mbResult = checkSaveChanges( %file , true );
      
      // return false to cancel the call to quit();
      if( %mbResult $= $MRCancel ) 
         return false;
      else if( %mbResult $= $MROk )
         %this.saveLevel();
   }
   
   // Prevent recursion when we shutdown
   LevelBuilderUndoManager.clearAll();
   
   // Returning true indicates we're ok to close
   return true; 
}


// 
function checkSaveChanges( %documentName, %cancelOption )
{
   %msg = "Do you want to save changes to document " @ %documentName @ "?";
   
   if( %cancelOption == true ) 
      %buttons = "SaveDontSaveCancel";
   else
      %buttons = "SaveDontSave";
      
   return MessageBox( "Torque Game Builder", %msg, %buttons, "Question" );   
}
