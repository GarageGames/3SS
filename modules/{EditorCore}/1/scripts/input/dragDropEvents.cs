//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

///
/// Public DragDrop Events
///
Input::GetEventManager().registerEvent( "BeginDropFiles" );
Input::GetEventManager().registerEvent( "DropFile" );
Input::GetEventManager().registerEvent( "EndDropFiles" );

function onDropBegin( %fileCount )
{
   //error("% DragDrop - Beginning file dropping of" SPC %fileCount SPC " files.");
   Input::GetEventManager().postEvent( "BeginDropFiles", %fileCount );
}
function onDropFile( %filePath )
{
   //error("  % DragDrop - Got File : " SPC %filePath );
   Input::GetEventManager().postEvent( "DropFile", %filePath );
}
function onDropEnd( %fileCount )
{
   
   //error("% DragDrop - Completed file dropping");
   Input::GetEventManager().postEvent( "EndDropFiles" );
}
