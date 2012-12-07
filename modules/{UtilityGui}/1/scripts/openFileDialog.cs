//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function getLoadFilename(%filespec, %callback, %currentFile)
{
   %dlg = new OpenFileDialog()
   {
      Filters = %filespec @ "|" @ %filespec;
      DefaultFile = %currentFile;
      ChangePath = false;
      MustExist = true;
      MultipleFiles = false;
   };
   if(filePath( %currentFile ) !$= "")
      %dlg.DefaultPath = filePath(%currentFile);
   else
      %dlg.DefaultPath = getCurrentDirectory();
      
   if(%dlg.Execute())
      eval(%callback @ "(\"" @ %dlg.FileName @ "\");");
   
   %dlg.delete();
}