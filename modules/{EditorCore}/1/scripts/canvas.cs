//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------
// initializeCanvas
// Constructs and initializes the default canvas window.
//---------------------------------------------------------------------------------------------
$canvasCreated = false;
function initializeCanvas(%windowName)
{
   // We must note this before we create the canvas for the first time
   // otherwise it will set the default res
   if( ! $pref::Video::windowedRes  && $pref::T2D::fullscreen != true )
      %doSetMaxRes = true;
   else
      %doSetMaxRes = false;
      
   
   // Don't duplicate the canvas.
   if($canvasCreated)
   {
      error("Cannot instantiate more than one canvas!");
      return;
   }
   
   videoSetGammaCorrection($pref::OpenGL::gammaCorrection);
   
   if (!createCanvas(%windowName))
   {
      error("Canvas creation failed. Shutting down.");
      quit();
   }
   
   
   if( %doSetMaxRes )
   {
      $pref::Video::windowedRes = "";
      // Get the list of valid resolutions.
      %resList = getResolutionList( $pref::Video::displayDevice );
      %resCount = getFieldCount(%resList);
      %deskRes = getDesktopResolution();
      
      // Loop through all the valid resolutions.
      for (%i = (%resCount - 1); %i >= 0; %i-- )
      {
         %res = getWords(getField(%resList, %i), 0, 2);
         
         if (firstWord(%res) >= firstWord(%deskRes))
            continue;
         if (getWord(%res, 1) >= getWord(%deskRes, 1))
            continue;

         if (firstWord(%res) <= firstWord($pref::Video::windowedRes))
            continue;
         if (getWord(%res, 1) <= getWord($pref::Video::windowedRes, 1))
            continue;

         // if we got through all those checks, this is the best res so far.
         $pref::Video::windowedRes = "1024 768";
      }
      
      %goodres = $pref::Video::windowedRes;
      setScreenMode( GetWord( %goodres, 0), GetWord( %goodres,1), GetWord( %goodres,2), false );
   }


   $canvasCreated = true;
}

//---------------------------------------------------------------------------------------------
// resetCanvas
// Forces the canvas to redraw itself.
//---------------------------------------------------------------------------------------------
function resetCanvas()
{
   if (isObject(Canvas))
      Canvas.repaint();
}
