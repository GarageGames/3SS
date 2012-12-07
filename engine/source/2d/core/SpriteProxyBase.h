//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_PROXY_BASE_H_
#define _SPRITE_PROXY_BASE_H_

#ifndef _BATCH_RENDER_H_
#include "BatchRender.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ANIMATION_ASSET_H_
#include "2d/assets/AnimationAsset.h"
#endif

#ifndef _ANIMATION_CONTROLLER_H_
#include "2d/assets/AnimationController.h"
#endif

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif

#ifndef _TICKABLE_H_
#include "platform/Tickable.h"
#endif

//------------------------------------------------------------------------------

class SpriteProxyBase : public virtual Tickable, public IFactoryObjectReset
{
protected:
    AssetPtr<ImageAsset>        mImageAsset;
    AssetPtr<AnimationAsset>    mAnimationAsset;

    bool                        mStaticMode;
    U32                         mImageFrame;
    bool                        mAnimationPaused;
    AnimationController*        mpAnimationController;

    bool                        mSelfTick;
    S32                         mReferenceCount;

public:
    SpriteProxyBase();
    virtual ~SpriteProxyBase();

    virtual void resetState( void );

    /// Integration.
    virtual bool update( const F32 elapsedTime );
    virtual void processTick();
    virtual void interpolateTick( F32 delta ) {};
    virtual void advanceTime( F32 timeDelta ) {};

    void render(
        const bool flipX,
        const bool flipY,
        const Vector2& vertexPos0,
        const Vector2& vertexPos1,
        const Vector2& vertexPos2,
        const Vector2& vertexPos3,
        BatchRender* pBatchRenderer ) const;

    void renderGui( GuiControl& owner, Point2I offset, const RectI &updateRect ) const;

    virtual void copyTo(SpriteProxyBase* pSpriteProxyBase) const;

    void clearAsset( void );

    // Image.
    inline bool setImage( const char* pImageAssetId ) { return setImage( pImageAssetId, mImageFrame ); }
    bool setImage( const char* pImageAssetId, const U32 frame );
    inline StringTableEntry getImage( void ) const { return mImageAsset.getAssetId(); };
    bool setImageFrame( const U32 frame );
    inline U32 getImageFrame( void ) const { return mImageFrame; };
    inline StringTableEntry getAnimation( void ) const { if ( mAnimationAsset.notNull() ) return mAnimationAsset.getAssetId(); else return StringTable->EmptyString; };

    /// Animation.
    bool setAnimation( const char* pAnimationAssetId, const bool autoRestore = false );
    inline AnimationController* getAnimationController( void ) const { return mpAnimationController; }
    inline bool isStaticMode( void ) const { return mStaticMode; }
    inline void pauseAnimation( const bool animationPaused ) { mAnimationPaused = animationPaused; }

    // Reference count.
    inline S32 addReference( void ) { return mReferenceCount++; }
    inline S32 removeReference( void ) { return --mReferenceCount; }

    /// Self ticking control.
    virtual void setProcessTicks( bool tick  ) { Tickable::setProcessTicks( mSelfTick ? tick : false ); }

protected:
    virtual void onAnimationEnd( void ) {}
};

#endif // _SPRITE_PROXY_BASE_H_
