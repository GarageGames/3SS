//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _GUIIMAGEBUTTON_H_
#include "2d/gui/GuiImageButtonCtrl.h"
#endif

#ifndef _RENDER_PROXY_H_
#include "2d/core/RenderProxy.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _CONSOLE_H_
#include "console/console.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _GUICANVAS_H_
#include "gui/guiCanvas.h"
#endif

#ifndef _H_GUIDEFAULTCONTROLRENDER_
#include "gui/guiDefaultControlRender.h"
#endif

/// Script bindings.
#include "guiImageButtonCtrl_ScriptBindings.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(GuiImageButtonCtrl);

//-----------------------------------------------------------------------------

GuiImageButtonCtrl::GuiImageButtonCtrl() :
    mNormalAssetId( StringTable->EmptyString ),
    mHoverAssetId( StringTable->EmptyString ),
    mDownAssetId( StringTable->EmptyString ),
    mInactiveAssetId( StringTable->EmptyString )
{
    mBounds.extent.set(140, 30);
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    addProtectedField("NormalImage", TypeAssetId, Offset(mNormalAssetId, GuiImageButtonCtrl), &setNormalImage, &getNormalImage, "The image-map asset Id used for the normal button state.");
    addProtectedField("HoverImage", TypeAssetId, Offset(mHoverAssetId, GuiImageButtonCtrl), &setHoverImage, &getHoverImage, "The image-map asset Id used for the hover button state.");
    addProtectedField("DownImage", TypeAssetId, Offset(mDownAssetId, GuiImageButtonCtrl), &setDownImage, &getDownImage, "The image-map asset Id used for the Down button state.");
    addProtectedField("InactiveImage", TypeAssetId, Offset(mInactiveAssetId, GuiImageButtonCtrl), &setInactiveImage, &getInactiveImage, "The image-map asset Id used for the inactive button state.");
}

//-----------------------------------------------------------------------------

bool GuiImageButtonCtrl::onWake()
{
    // Call parent.
    if (!Parent::onWake())
        return false;

    // Is only the "normal" image specified?
    if (    mNormalAssetId != StringTable->EmptyString &&
            mHoverAssetId == StringTable->EmptyString &&
            mDownAssetId == StringTable->EmptyString &&
            mInactiveAssetId == StringTable->EmptyString )
    {
        // Yes, so use it for all states.
        mImageNormalAsset = mNormalAssetId;
        mImageHoverAsset = mNormalAssetId;
        mImageDownAsset = mNormalAssetId;
        mImageInactiveAsset = mNormalAssetId;
    }
    else
    {
        // No, so assign individual states.
        mImageNormalAsset = mNormalAssetId;
        mImageHoverAsset = mHoverAssetId;
        mImageDownAsset = mDownAssetId;
        mImageInactiveAsset = mInactiveAssetId;
    }
   
    return true;
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::onSleep()
{
    // Clear assets.
    mImageNormalAsset.clear();
    mImageHoverAsset.clear();
    mImageDownAsset.clear();
    mImageInactiveAsset.clear();

    // Call parent.
    Parent::onSleep();
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::setNormalImage( const char* pImageMapAssetId )
{
    // Sanity!
    AssertFatal( pImageMapAssetId != NULL, "Cannot use a NULL asset Id." );

    // Fetch the asset Id.
    mNormalAssetId = StringTable->insert(pImageMapAssetId);

    // Assign asset if awake.
    if ( isAwake() )
        mImageNormalAsset = mNormalAssetId;

    // Update control.
    setUpdate();
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::setHoverImage( const char* pImageMapAssetId )
{
    // Sanity!
    AssertFatal( pImageMapAssetId != NULL, "Cannot use a NULL asset Id." );

    // Fetch the asset Id.
    mHoverAssetId = StringTable->insert(pImageMapAssetId);

    // Assign asset if awake.
    if ( isAwake() )
        mImageHoverAsset = mHoverAssetId;

    // Update control.
    setUpdate();
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::setDownImage( const char* pImageMapAssetId )
{
    // Sanity!
    AssertFatal( pImageMapAssetId != NULL, "Cannot use a NULL asset Id." );

    // Fetch the asset Id.
    mDownAssetId = StringTable->insert(pImageMapAssetId);

    // Assign asset if awake.
    if ( isAwake() )
        mImageDownAsset = mDownAssetId;

    // Update control.
    setUpdate();
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::setInactiveImage( const char* pImageMapAssetId )
{
    // Sanity!
    AssertFatal( pImageMapAssetId != NULL, "Cannot use a NULL asset Id." );

    // Fetch the asset Id.
    mInactiveAssetId = StringTable->insert(pImageMapAssetId);

    // Assign asset if awake.
    if ( isAwake() )
        mImageInactiveAsset = mInactiveAssetId;

    // Update control.
    setUpdate();
}

//-----------------------------------------------------------------------------

void GuiImageButtonCtrl::onRender(Point2I offset, const RectI& updateRect)
{
    // Reset button state.
    ButtonState state = NORMAL;

    // Calculate button state.
    if ( mActive )
    {
        if ( mMouseOver )
            state = HOVER;

        if ( mDepressed || mStateOn )
            state = DOWN;
    }
    else
    {
        state = INACTIVE;
    }

    switch (state)
    {
        case NORMAL:
            {
                // Is the asset available?
                if ( mImageNormalAsset.notNull() )
                {
                    // Yes, so render it.
                    renderButton(mImageNormalAsset, 0, offset, updateRect);
                }
                else
                {
                    // No, so render no-image render-proxy.
                    renderNoImage( offset, updateRect );
                }
            } break;

        case HOVER:
            {
                // Is the asset available?
                if ( mImageHoverAsset.notNull() )
                {
                    // Yes, so render it.
                    renderButton(mImageHoverAsset, 0, offset, updateRect);
                }
                else
                {
                    // No, so render no-image render-proxy.
                    renderNoImage( offset, updateRect );
                }
            } break;

        case DOWN:
            {
                // Is the asset available?
                if ( mImageDownAsset.notNull() )
                {
                    // Yes, so render it.
                    renderButton(mImageDownAsset, 0, offset, updateRect);
                }
                else
                {
                    // No, so render no-image render-proxy.
                    renderNoImage( offset, updateRect );
                }
            }  break;

        case INACTIVE:
            {
                // Is the asset available?
                if ( mImageInactiveAsset.notNull() )
                {
                    // Yes, so render it.
                    renderButton(mImageInactiveAsset, 0, offset, updateRect);
                }
                else
                {
                    // No, so render no-image render-proxy.
                    renderNoImage( offset, updateRect );
                }
            } break;
    }
}

//------------------------------------------------------------------------------

void GuiImageButtonCtrl::renderButton( ImageAsset* pDatablock, const U32 frame, Point2I &offset, const RectI& updateRect )
{
    // Ignore an invalid datablock.
    if ( pDatablock == NULL )
        return;

    // Calculate source region.
    const ImageAsset::FrameArea::PixelArea& pixelArea = pDatablock->getImageFrameArea( frame ).mPixelArea;
    RectI sourceRegion( pixelArea.mPixelOffset, Point2I(pixelArea.mPixelWidth, pixelArea.mPixelHeight) );

    // Calculate destination region.
    RectI destinationRegion(offset, mBounds.extent);

    // Render image.
    dglClearBitmapModulation();
    dglDrawBitmapStretchSR( pDatablock->getImageTexture(), destinationRegion, sourceRegion );
    renderChildControls( offset, updateRect);
}

//------------------------------------------------------------------------------

void GuiImageButtonCtrl::renderNoImage( Point2I &offset, const RectI& updateRect )
{
    // Fetch the default "NoImageRenderProxy".
    RenderProxy* pNoImageRenderProxy = Sim::findObject<RenderProxy>( Con::getVariable( "$NoImageRenderProxy" ) );

    // Finish if no render proxy available.
    if ( pNoImageRenderProxy == NULL )
        return;

    // Render using render-proxy..
    pNoImageRenderProxy->renderGui( *this, offset, updateRect );

    // Update control.
    setUpdate();
}