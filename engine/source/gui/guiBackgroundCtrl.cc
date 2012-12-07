//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/console.h"
#include "gui/guiBackgroundCtrl.h"

IMPLEMENT_CONOBJECT(GuiBackgroundCtrl);

//--------------------------------------------------------------------------
GuiBackgroundCtrl::GuiBackgroundCtrl() : GuiControl()
{
   mDraw = false;
   mIsContainer = true;
}

//--------------------------------------------------------------------------
void GuiBackgroundCtrl::onRender(Point2I offset, const RectI &updateRect)
{
   if ( mDraw )
      Parent::onRender( offset, updateRect );

   renderChildControls(offset, updateRect);
}


