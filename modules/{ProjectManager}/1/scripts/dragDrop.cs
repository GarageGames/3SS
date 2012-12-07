//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Subscribe to dragDrop events
if( isObject( LBProjectObj ) )
{
   Input::GetEventManager().subscribe( LBProjectObj, "BeginDropFiles" );
   Input::GetEventManager().subscribe( LBProjectObj, "DropFile" );
   Input::GetEventManager().subscribe( LBProjectObj, "EndDropFiles" );
}

function T2DProject::onBeginDropFiles( %this, %fileCount )
{   
   //error("% DragDrop - Beginning file dropping of" SPC %fileCount SPC " files.");
}
function T2DProject::onDropFile( %this, %filePath )
{
   // Check imagemap extension
   %fileExt = fileExt( %filePath );
   if( (%fileExt $= ".png") || (%fileExt $= ".jpg") || (%fileExt $= ".jpeg") )
      %this.onDropImageFile(%filePath);
   
   else if (%fileExt $= ".zip")
      %this.onDropZipFile(%filePath);
      
   else
      messageBox("Error", "Cannot open file format: " @ %fileExt);
}

function T2DProject::onDropZipFile(%this, %filePath)
{
    error("Deprecated T2DProject::onDropZipFile");
}

function T2DProject::onDropImageFile(%this, %filePath)
{
   // File Information madness
   %fileName         = %filePath;
   %fileOnlyName     = fileName( %fileName );
   %fileBase         = validateDatablockName(fileBase( %fileName ) @ "Sprite");
   
   // [neo, 5/17/2007 - #3117]
   // Check if the file being dropped is already in data/images or a sub dir by checking if
   // the file path up to length of check path is the same as check path.
   %checkPath    = expandPath( "^project/data/images/" );
   %fileOnlyPath = filePath( expandPath( %filePath ) );
   %fileBasePath = getSubStr( %fileOnlyPath, 0, strlen( %checkPath ) );
   
   if( %checkPath !$= %fileBasePath )
   {
      // No match so file is from outside images directory and we need to copy it in
      %fileNewLocation = expandPath("^project/data/images/") @ fileBase( %fileName ) @ fileExt( %fileName );
   
      // Move to final location
      if( !pathCopy( %filePath, %fileNewLocation ) )
         return;
   }
   else 
   {  
      // Already in images path somewhere so just link to it
      %fileNewLocation = %filePath;
   }
   
   addResPath( filePath( %fileNewLocation ) );
   
   %noExt = strreplace(%fileNewLocation, ".jpg", "");
   %noExt = strreplace(%noExt, ".jpeg", "");
   %noExt = strreplace(%noExt, ".png", "");
   %noExt = strreplace(%noExt, ".pvr", "");
   
   // Create Datablock
   %imap = new ImageAsset();
   %imap.setName( %fileBase );
   %imap.imageFile = %noExt;
   %imap.filterMode = "NONE";
   %imap.compile();
         
   // Bad Creation!
   if( !isObject( %imap ) )
      return;
      
   %this.addDatablock( %fileBase, false );
}

function T2DProject::onEndDropFiles( %this, %fileCount )
{
   //error("% DragDrop - Completed file dropping");

   %this.persistToDisk( true, false, false );
   
   // Update object library
   GuiFormManager::SendContentMessage($LBCreateSiderBar, %this, "refreshAll 1");

}