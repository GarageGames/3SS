//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/// T2DProject::getLevelSaveName - Open a Native File dialog and retrieve the
///  location to save the current document.  *NOTE* this will change soon to 
///  accomodate a document aware project interface that can be easily expanded
///  upon and customized by a 3rd party developer or someone else who might be
///  generally worth supporting with good API's :)
///
/// @arg defaultFileName   The FileName to default in the field and to be selected when a path is opened

$T2D::ParticleSpec = "TGB Particle Effects (*.eff)|*.eff|All Files (*.*)|*.*|";

function ParticleEffect::getLevelSaveName( %defaultFileName )
{
   if( %defaultFileName $= "" )
      return "";
   
   %dlg = new SaveFileDialog()
   {
      Filters           = $T2D::ParticleSpec;
      DefaultPath       = filePath(%defaultFileName);
      DefaultFile       = getSaveDefaultFile(%defaultFileName);
      ChangePath        = true;
      OverwritePrompt   = true;
   };
         
   if(%dlg.Execute())
   {
      %filename = %dlg.FileName;
      %filespec = $T2D::ParticleSpec;
      
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

function ParticleEffect::getLevelOpenName( %defaultFileName )
{
   if( %defaultFileName $= "" )
      %defaultFileName = expandPath("^project/data/particles/untitled.eff");
   
   %dlg = new OpenFileDialog()
   {
      Filters        = $T2D::ParticleSpec;
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
