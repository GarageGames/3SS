//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function getSaveFilename(%filespec, %callback, %currentFile)
{
   %dlg = new SaveFileDialog()
   {
      Filters = %filespec @ "|" @ %filespec;
      DefaultFile = %currentFile;
      ChangePath = false;
      OverwritePrompt = true;
   };
   
   if(filePath( %currentFile ) !$= "")
      %dlg.DefaultPath = filePath(%currentFile);
   else
      %dlg.DefaultPath = getCurrentDirectory();
      
   if(%dlg.Execute())
   {
      %ext = strstr( %filespec, "." );
      %extEnd = strstr( %filespec, ")" );

      if( %extEnd == -1 )
         %extEnd = strstr( %filespec, "|");

      if( %extEnd == -1 )
         %extEnd = strlen(%filespec);

      %ext = getSubStr( %filespec, %ext, %extEnd - %ext );

      %filename = %dlg.FileName;
      
      eval(%callback @ "(\"" @ %filename @ "\");");
   }
   
   %dlg.delete();
}