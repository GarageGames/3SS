//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _GUIIMAGEBUTTON_H_
#define _GUIIMAGEBUTTON_H_

#ifndef _GUIBUTTONCTRL_H_
#include "gui/buttons/guiButtonCtrl.h"
#endif
#ifndef _TEXTURE_MANAGER_H_
#include "graphics/TextureManager.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ASSET_PTR_H_
#include "assets/assetPtr.h"
#endif

//-----------------------------------------------------------------------------

class GuiImageButtonCtrl : public GuiButtonCtrl
{
private:
   typedef GuiButtonCtrl Parent;

protected:
    StringTableEntry                mNormalAssetId;
    StringTableEntry                mHoverAssetId;
    StringTableEntry                mDownAssetId;
    StringTableEntry                mInactiveAssetId;

    AssetPtr<ImageAsset>  mImageNormalAsset;
    AssetPtr<ImageAsset>  mImageHoverAsset;
    AssetPtr<ImageAsset>  mImageDownAsset;
    AssetPtr<ImageAsset>  mImageInactiveAsset;

    void renderButton( ImageAsset* pDatablock, const U32 frame, Point2I &offset, const RectI& updateRect);
    void renderNoImage( Point2I &offset, const RectI& updateRect );

protected:
    enum ButtonState
    {
        NORMAL,
        HOVER,
        DOWN,
        INACTIVE
    };

public:
   GuiImageButtonCtrl();
   bool onWake();
   void onSleep();
   void onRender(Point2I offset, const RectI &updateRect);

   static void initPersistFields();

   void setNormalImage( const char* pImageMapAssetId );
   inline StringTableEntry getNormalImage( void ) const { return mNormalAssetId; }
   void setHoverImage( const char* pImageMapAssetId );
   inline StringTableEntry getHoverImage( void ) const { return mHoverAssetId; }
   void setDownImage( const char* pImageMapAssetId );
   inline StringTableEntry getDownImage( void ) const { return mDownAssetId; }
   void setInactiveImage( const char* pImageMapAssetId );
   inline StringTableEntry getInactiveImage( void ) const { return mInactiveAssetId; }

   // Declare type.
   DECLARE_CONOBJECT(GuiImageButtonCtrl);

protected:
    static bool setNormalImage(void* obj, const char* data) { static_cast<GuiImageButtonCtrl*>(obj)->setNormalImage( data ); return false; }
    static const char* getNormalImage(void* obj, const char* data) { return static_cast<GuiImageButtonCtrl*>(obj)->getNormalImage(); }
    static bool setHoverImage(void* obj, const char* data) { static_cast<GuiImageButtonCtrl*>(obj)->setHoverImage( data ); return false; }
    static const char* getHoverImage(void* obj, const char* data) { return static_cast<GuiImageButtonCtrl*>(obj)->getHoverImage(); }
    static bool setDownImage(void* obj, const char* data) { static_cast<GuiImageButtonCtrl*>(obj)->setDownImage( data ); return false; }
    static const char* getDownImage(void* obj, const char* data) { return static_cast<GuiImageButtonCtrl*>(obj)->getDownImage(); }
    static bool setInactiveImage(void* obj, const char* data) { static_cast<GuiImageButtonCtrl*>(obj)->setInactiveImage( data ); return false; }
    static const char* getInactiveImage(void* obj, const char* data) { return static_cast<GuiImageButtonCtrl*>(obj)->getInactiveImage(); }
};

#endif //_GUIIMAGEBUTTON_H_
