//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUIBITMAPCTRL_H_
#define _GUIBITMAPCTRL_H_

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif
#ifndef _TEXTURE_MANAGER_H_
#include "graphics/TextureManager.h"
#endif


/// Renders a bitmap.
class GuiBitmapCtrl : public GuiControl
{
private:
   typedef GuiControl Parent;

protected:
   static bool setBitmapName( void *obj, const char *data );
   static const char *getBitmapName( void *obj, const char *data );

   StringTableEntry mBitmapName;
   TextureHandle mTextureHandle;
   Point2I startPoint;
   bool mWrap;

   //Luma:	ability to specify source rect for image UVs
   bool		mUseSourceRect;
   RectI	mSourceRect;

public:
   //creation methods
   DECLARE_CONOBJECT(GuiBitmapCtrl);
   GuiBitmapCtrl();
   static void initPersistFields();

   //Parental methods
   bool onWake();
   void onSleep();
   void inspectPostApply();

   void setBitmap(const char *name,bool resize = false);
   void setBitmap(const TextureHandle &handle,bool resize = false);

   S32 getWidth() const       { return(mTextureHandle.getWidth()); }
   S32 getHeight() const      { return(mTextureHandle.getHeight()); }

   //Luma:	ability to specify source rect for image UVs
   void setSourceRect(U32 x, U32 y, U32 width, U32 height);
   void setUseSourceRect(bool bUse);

   void onRender(Point2I offset, const RectI &updateRect);
   void setValue(S32 x, S32 y);
};

#endif
