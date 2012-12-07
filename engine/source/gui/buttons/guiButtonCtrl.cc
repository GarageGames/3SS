//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/console.h"
#include "graphics/dgl.h"
#include "console/consoleTypes.h"
#include "platform/platformAudio.h"
#include "gui/guiCanvas.h"
#include "gui/buttons/guiButtonCtrl.h"
#include "gui/guiDefaultControlRender.h"

IMPLEMENT_CONOBJECT(GuiButtonCtrl);

GuiButtonCtrl::GuiButtonCtrl()
{
   mBounds.extent.set(140, 30);
   mButtonText = StringTable->EmptyString;
}

bool GuiButtonCtrl::onWake()
{
   if( !Parent::onWake() )
      return false;

   // Button Theme?
   if( mProfile->constructBitmapArray() >= 36 )
      mHasTheme = true;
   else
      mHasTheme = false;

   return true;

}
//--------------------------------------------------------------------------

void GuiButtonCtrl::onRender(Point2I      offset,
                             const RectI& updateRect)
{
   bool highlight = mMouseOver;
   bool depressed = mDepressed;

   ColorI fontColor   = mActive ? (highlight ? mProfile->mFontColorHL : mProfile->mFontColor) : mProfile->mFontColorNA;
   ColorI backColor   = mActive ? mProfile->mFillColor : mProfile->mFillColorNA;
   ColorI borderColor = mActive ? mProfile->mBorderColor : mProfile->mBorderColorNA;

   RectI boundsRect(offset, mBounds.extent);

   if( mProfile->mBorder != 0 && !mHasTheme )
   {
      if (mDepressed || mStateOn)
         renderFilledBorder( boundsRect, mProfile->mBorderColorHL, mProfile->mFillColorHL );
      else
         renderFilledBorder( boundsRect, mProfile->mBorderColor, mProfile->mFillColor );
   }
   else if( mHasTheme )
   {
      S32 indexMultiplier = 1;
      if ( mDepressed || mStateOn ) 
         indexMultiplier = 3;
      else if ( mMouseOver )
         indexMultiplier = 2;
      else if ( !mActive )
         indexMultiplier = 4;

      renderSizableBitmapBordersFilled( boundsRect, indexMultiplier, mProfile );
   }

   Point2I textPos = offset;
   if(depressed)
      textPos += Point2I(1,1);

   dglSetBitmapModulation( fontColor );
   renderJustifiedText(textPos, mBounds.extent, mButtonText);

   //render the children
   renderChildControls( offset, updateRect);
}

