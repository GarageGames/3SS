//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUISPRITECTRL_H_
#include "2d/gui/guiSpriteCtrl.h"
#endif

#ifndef _CONSOLE_H_
#include "console/console.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _RENDER_PROXY_H_
#include "2d/core/RenderProxy.h"
#endif

#include "guiSpriteCtrl_ScriptBindings.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(GuiSpriteCtrl);

//-----------------------------------------------------------------------------

GuiSpriteCtrl::GuiSpriteCtrl( void ) :
    mImageAssetId( StringTable->EmptyString ),
    mAnimationAssetId( StringTable->EmptyString ),
    mImageFrame( 0 ),
    mAnimationPaused( false ),
    mpAnimationController(NULL)
{
}

//-----------------------------------------------------------------------------

GuiSpriteCtrl::~GuiSpriteCtrl()
{
    // Destroy animation controller if required.
    if ( mpAnimationController != NULL )
    {
        delete mpAnimationController;
        mpAnimationController = NULL;
    }
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    addProtectedField( "Image", TypeAssetId, Offset(mImageAssetId, GuiSpriteCtrl), &setImage, &getImage, &writeImage, "The image-map asset Id used for the image." );
    addProtectedField( "Frame", TypeS32, Offset(mImageFrame, GuiSpriteCtrl), &setImageFrame, &defaultProtectedGetFn, &writeImageFrame, "The image-map frame used for the image." );
    addProtectedField( "Animation", TypeAssetId, Offset(mAnimationAssetId, GuiSpriteCtrl), &setAnimation, &getAnimation, &writeAnimation, "The animation to use.");
}

//-----------------------------------------------------------------------------

bool GuiSpriteCtrl::onWake()
{
    // Call parent.
    if (!Parent::onWake())
        return false;

    // Are we in static mode?
    if ( isStaticMode() )
    {
        // Set image asset if it's valid.
        if ( mImageAssetId != StringTable->EmptyString )
            mImageAsset = mImageAssetId;
    }
    else
    {
        // Set animation asset if it's valid.
        if ( mAnimationAssetId != StringTable->EmptyString )
        {
            // Create animation controller if required.
            if ( mpAnimationController == NULL )
                mpAnimationController = new AnimationController();

            // Play animation.
            mpAnimationController->playAnimation( mAnimationAssetId, false );

            // Turn-on tick processing.
            setProcessTicks( true );
        }
    }

    return true;
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::onSleep()
{
    // Clear assets.
    mImageAsset.clear();

    // Destroy animation controller if required.
    if ( mpAnimationController != NULL )
    {
        delete mpAnimationController;
        mpAnimationController = NULL;
    }    

    // Turn-off tick processing.
    setProcessTicks( false );

    // Call parent.
    Parent::onSleep();
}

//------------------------------------------------------------------------------

void GuiSpriteCtrl::processTick( void )
{
    // Are we in static mode?
    if ( isStaticMode() )
    {
        // Yes, so turn-off tick processing.
        setProcessTicks( false );

        return;
    }

    // Finish if no animation controller.
    if ( mpAnimationController == NULL )
        return;

    // Finish if the animation has finished.
    if ( mpAnimationController->isAnimationFinished() )
        return;

    // Finish if animation is paused.
    if ( mAnimationPaused )
        return;

    // Update the animation.
    mpAnimationController->updateAnimation( Tickable::smTickSec );

    // Finish if the animation has not finished.
    if ( !mpAnimationController->isAnimationFinished() )
        return;

    // Turn-off tick processing.
    setProcessTicks( false );
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::setImage( const char* pImageMapAssetId )
{
    // Sanity!
    AssertFatal( pImageMapAssetId != NULL, "Cannot use a NULL asset Id." );

    // Fetch the asset Id.
    mImageAssetId = StringTable->insert(pImageMapAssetId);

    // Reset image frame.
    mImageFrame = 0;

    // Assign asset if awake.
    if ( isAwake() )
        mImageAsset = mImageAssetId;
    
    // Set static mode.
    mStaticMode = true;

    // Reset animation.
    mAnimationAssetId = StringTable->EmptyString;

    // Destroy animation controller if required.
    if ( mpAnimationController != NULL )
    {
        delete mpAnimationController;
        mpAnimationController = NULL;
    }

    // Update control.
    setUpdate();
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::setImageFrame( const U32 imageFrame )
{
    // Set image frame.
    mImageFrame = imageFrame;

    // Update control.
    setUpdate();
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::setAnimation( const char* pAnimationAssetId )
{
    // Sanity!
    AssertFatal( pAnimationAssetId != NULL, "Cannot use a NULL asset Id." );

    // Fetch the asset Id.
    mAnimationAssetId = StringTable->insert(pAnimationAssetId);

    // Reset the image asset Id.
    mImageAssetId = StringTable->EmptyString;

    // Reset image frame.
    mImageFrame = 0;

    // Reset static mode.
    mStaticMode = false;

    // Assign asset if awake.
    if ( isAwake() )
    {
        // Set animation asset if it's valid.
        if ( mAnimationAssetId != StringTable->EmptyString )
        {
            // Create animation controller if required.
            if ( mpAnimationController == NULL )
                mpAnimationController = new AnimationController();

            // Play animation.
            mpAnimationController->playAnimation( mAnimationAssetId, false );

            // Turn-on tick processing.
            setProcessTicks( true );
        }
    }
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::play(void)
{
    // Sanity!
    if (mStaticMode)
        return;

    // Reset animation pause.
    mAnimationPaused = false;

    // Assign asset if awake.
    if ( isAwake() )
    {
        // Set animation asset if it's valid.
        if ( mAnimationAssetId != StringTable->EmptyString )
        {
            // Play animation.
            mpAnimationController->playAnimation( mAnimationAssetId, false );

            // Turn-on tick processing.
            setProcessTicks( true );
        }
    }
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::pause(bool flag)
{
    // Finish if not awake.
    if ( !isAwake() )
        return;

    // Finish if no animation.
    if ( mpAnimationController == NULL )
        return;

    // Pause the animation.
    mAnimationPaused = flag;
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::stop(void)
{
    // Finish if not awake.
    if ( !isAwake() )
        return;

    // Finish if no animation.
    if ( mpAnimationController == NULL )
        return;

    // Stop the animation.
    mAnimationPaused = false;
    mpAnimationController->stopAnimation();
}

//-----------------------------------------------------------------------------

void GuiSpriteCtrl::onRender( Point2I offset, const RectI &updateRect)
{
    // Are we in static mode?
    if ( isStaticMode() )
    {
        // Do we have a valid image to render?
        if ( mImageAsset.notNull() && mImageFrame < mImageAsset->getFrameCount() )
        {
            // Yes, so calculate source region.
            const ImageAsset::FrameArea::PixelArea& pixelArea = mImageAsset->getImageFrameArea( mImageFrame ).mPixelArea;
            RectI sourceRegion( pixelArea.mPixelOffset, Point2I(pixelArea.mPixelWidth, pixelArea.mPixelHeight) );

            // Calculate destination region.
            RectI destinationRegion(offset, mBounds.extent);

            // Render image.
            dglClearBitmapModulation();
            dglDrawBitmapStretchSR( mImageAsset->getImageTexture(), destinationRegion, sourceRegion );
        }
        else
        {
            // No, so render no-image render-proxy.
            renderNoImage( offset, updateRect );
        }
    }
    else
    {
        // Do we have a valid animation to render?
        if ( mpAnimationController != NULL && mpAnimationController->getAnimationAsset().notNull() )
        {
            // Yes, so calculate source region.
            const ImageAsset::FrameArea::PixelArea& pixelArea = mpAnimationController->getCurrentImageFrameArea().mPixelArea;
            RectI sourceRegion( pixelArea.mPixelOffset, Point2I(pixelArea.mPixelWidth, pixelArea.mPixelHeight) );

            // Calculate destination region.
            RectI destinationRegion(offset, mBounds.extent);

            // Render animation image.
            dglClearBitmapModulation();
            dglDrawBitmapStretchSR( mpAnimationController->getImageTexture(), destinationRegion, sourceRegion );

            // Update control.
            setUpdate();
        }
        else
        {
            // No, so render no-image render-proxy.
            renderNoImage( offset, updateRect );
        }
    }

    // Render child controls.
    renderChildControls(offset, updateRect);
}

//------------------------------------------------------------------------------

void GuiSpriteCtrl::renderNoImage( Point2I &offset, const RectI& updateRect )
{
    // Fetch the default "NoImageRenderProxy".
    RenderProxy* pNoImageRenderProxy = Sim::findObject<RenderProxy>( Con::getVariable( "$NoImageRenderProxy" ) );

    // Finish if no render proxy available.
    if ( pNoImageRenderProxy == NULL )
        return;

    // Render using render-proxy.
    pNoImageRenderProxy->renderGui( *this, offset, updateRect );

    // Update control.
    setUpdate();
}


