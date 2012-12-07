//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function getSaveDefaultFile( %fileName )
{
    if ( $platform $= "macos" )
        return fileBase( %fileName );
    
    return %fileName;
}

/// T2DProject::getLevelSaveName - Open a Native File dialog and retrieve the
///  location to save the current document.  *NOTE* this will change soon to 
///  accomodate a document aware project interface that can be easily expanded
///  upon and customized by a 3rd party developer or someone else who might be
///  generally worth supporting with good API's :)
///
/// @arg defaultFileName   The FileName to default in the field and to be selected when a path is opened
function T2DProject::getLevelSaveName( %this, %defaultFileName )
{
   if( %defaultFileName !$= "" )
      %levelFullPath = %defaultFileName;
   else
      %levelFullPath = %this.currentLevelFile;
   
   %dlg = new SaveFileDialog()
   {
      Filters           = $T2D::LevelSpec;
      DefaultPath       = filePath(%levelFullPath);
      DefaultFile       = getSaveDefaultFile(%levelFullPath);
      ChangePath        = true;
      OverwritePrompt   = true;
   };
   
   if(%dlg.Execute())
   {
      %filename = %dlg.FileName;
      %filespec = $T2D::LevelSpec;
      
      %ext = strstr( %filespec, "." );
      %extEnd = strstr( %filespec, ")" );
      if( %extEnd != -1 )
         %ext = getSubStr( %filespec, %ext, %extEnd - %ext );
         
      if( fileExt( %filename ) !$= %ext )
         %filename = %filename @ %ext;
      
      %dlg.delete();
      
      return %filename;
   }
   
   %dlg.delete();
   
   return "";   
}

function T2DProject::getLevelOpenName( %this, %defaultFileName )
{
   if( %defaultFileName !$= "" )
      %levelFullPath = %defaultFileName;
   else
      %levelFullPath = %this.currentLevelFile;
   
   %dlg = new OpenFileDialog()
   {
      Filters        = $T2D::LevelSpec;
      DefaultPath    = filePath( %defaultFileName );
      DefaultFile    = %defaultFileName;
      ChangePath     = false;
      MustExist      = true;
   };
         
   if(%dlg.Execute())
   {
      %filename = %dlg.FileName;      
      %dlg.delete();
      return %filename;
   }
   
   %dlg.delete();
   return "";   
}
