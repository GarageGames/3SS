//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
if(!isObject(ImageBuilderZoomCursor)) new GuiCursor(ImageBuilderZoomCursor)
{
   hotSpot = "1 1";
   bitmapName = expandPath("^{EditorAssets}/gui/iconZoom");
};
   
if(!isObject(ImageBuilderZoomInCursor)) new GuiCursor(ImageBuilderZoomInCursor)
{
   hotSpot = "1 1";
   bitmapName = expandPath("^{EditorAssets}/gui/iconZoomIn");
}; 
    
if(!isObject(ImageBuilderZoomOutCursor)) new GuiCursor(ImageBuilderZoomOutCursor)
{
   hotSpot = "1 1";
   bitmapName = expandPath("^{EditorAssets}/gui/iconZoomOut");
};