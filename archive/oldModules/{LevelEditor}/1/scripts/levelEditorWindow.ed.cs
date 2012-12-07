//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function layerSelectionDown( %val )
{
   if (%val)
   {
      %objects = ToolManager.getAcquiredObjects();
      %object = %objects;
      if (%objects.getCount() == 0)
         return;
      else if (%objects.getCount() == 1)
         %object = %objects.getObject(0);
      QuickEdit::setObjectProperty(%object, "Layer", QuickEdit::getObjectProperty(%object, "Layer") - 1);
   }
}

function layerSelectionUp( %val )
{
   if (%val)
   {
      %objects = ToolManager.getAcquiredObjects();
      %object = %objects;
      if (%objects.getCount() == 0)
         return;
      else if (%objects.getCount() == 1)
         %object = %objects.getObject(0);
         
      QuickEdit::setObjectProperty(%object, "Layer", QuickEdit::getObjectProperty(%object, "Layer") + 1);
   }
}

function bringToFront( %val )
{
   if (%val)
   {
      %objects = ToolManager.getAcquiredObjects();
      %object = %objects;
      if (%objects.getCount() == 0)
         return;
      else if (%objects.getCount() == 1)
         %object = %objects.getObject(0);
         
      QuickEdit::setObjectProperty(%object, "Layer", 0);
   }
}

function sendToBack( %val )
{
   if (%val)
   {
      %objects = ToolManager.getAcquiredObjects();
      %object = %objects;
      if (%objects.getCount() == 0)
         return;
      else if (%objects.getCount() == 1)
         %object = %objects.getObject(0);
         
      QuickEdit::setObjectProperty(%object, "Layer", 31);
   }
}



function LevelBuilderBase::onResize( %this )
{
   // Set Frames min extent properly.   
   %SceneViewExt  = levelBuilderViewContainer.getExtent();
   %SceneViewExtY   = GetWord( %SceneViewExt, 1 );
   
   %SideBarExt    = sideBarContentContainer.getExtent();
   %SideBarExtY   = GetWord( %SideBarExt, 1 );
   
   // Set frame extents properly
   levelBuilderContentFrame.frameMinExtent( 0, 370, %SideBarExtY );   
   levelBuilderContentFrame.frameMinExtent( 1, 270, %SceneViewExtY );  
}