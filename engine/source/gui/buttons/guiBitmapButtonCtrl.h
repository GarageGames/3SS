//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _GUIBITMAPBUTTON_H_
#define _GUIBITMAPBUTTON_H_

#ifndef _GUIBUTTONCTRL_H_
#include "gui/buttons/guiButtonCtrl.h"
#endif
#ifndef _TEXTURE_MANAGER_H_
#include "graphics/TextureManager.h"
#endif

enum ButtonState
{
    NORMAL,
    HILIGHT,
    DEPRESSED,
    INACTIVE
};

class GuiBitmapButtonCtrl : public GuiButtonCtrl
{
private:
   typedef GuiButtonCtrl Parent;

protected:
   StringTableEntry mBitmapName;
   StringTableEntry mBitmapNormal;
   StringTableEntry mBitmapHilight;
   StringTableEntry mBitmapDepressed;
   StringTableEntry mBitmapInactive;
   bool mIsLegacyVersion;

   TextureHandle mTextureNormal;
   TextureHandle mTextureHilight;
   TextureHandle mTextureDepressed;
   TextureHandle mTextureInactive;

   void renderButton(TextureHandle &texture, Point2I &offset, const RectI& updateRect);

public:
   DECLARE_CONOBJECT(GuiBitmapButtonCtrl);
   GuiBitmapButtonCtrl();

   static void initPersistFields();

   //Parent methods
   bool onWake();
   void onSleep();
   void inspectPostApply();

   void setBitmap(const char *name, ButtonState state);
   void setBitmap(const char *name);

   void onRender(Point2I offset, const RectI &updateRect);
};

class GuiBitmapButtonTextCtrl : public GuiBitmapButtonCtrl
{
   typedef GuiBitmapButtonCtrl Parent;
public:
   DECLARE_CONOBJECT(GuiBitmapButtonTextCtrl);
   void onRender(Point2I offset, const RectI &updateRect);
};

#endif //_GUI_BITMAP_BUTTON_CTRL_H
