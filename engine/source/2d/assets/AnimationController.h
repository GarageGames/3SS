//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ANIMATION_CONTROLLER_H_
#define _ANIMATION_CONTROLLER_H_

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ANIMATION_ASSET_H_
#include "2d/assets/AnimationAsset.h"
#endif

#ifndef _ASSET_PTR_H_
#include "assets/assetPtr.h"
#endif

///-----------------------------------------------------------------------------

class AnimationController : private AssetPtrCallback
{
private:
    AssetPtr<AnimationAsset>                mAnimationAsset;
    AssetPtr<AnimationAsset>                mLastAnimationAsset;
    S32                                     mLastFrameIndex;
    S32                                     mCurrentFrameIndex;
    U32                                     mMaxFrameIndex;
    F32                                     mCurrentTime;
    F32                                     mPausedTime;
    F32                                     mCurrentModTime;
    F32                                     mAnimationTimeScale;
    F32                                     mTotalIntegrationTime;
    F32                                     mFrameIntegrationTime;
    bool                                    mAutoRestoreAnimation;
    bool                                    mAnimationFinished;

private:
    virtual void onAssetRefreshed( AssetPtrBase* pAssetPtrBase );

public:
    AnimationController();
    virtual ~AnimationController();

    TextureHandle&                                  getImageTexture( void )                 { return mAnimationAsset->getImageMap()->getImageTexture(); };
    const ImageAsset::FrameArea&                    getCurrentImageFrameArea( void ) const  { return mAnimationAsset->getImageMap()->getImageFrameArea(getCurrentFrame()); };
    const AnimationAsset*                           getCurrentDataBlock( void ) const       { return mAnimationAsset.notNull() ? mAnimationAsset : NULL; };
    const StringTableEntry                          getCurrentAnimation( void ) const       { return mAnimationAsset.getAssetId(); };
    const U32                                       getCurrentFrame( void ) const           { return mAnimationAsset->getValidatedAnimationFrames()[mCurrentFrameIndex]; };
    const F32                                       getCurrentTime( void ) const            { return mCurrentTime; };
    bool                                            isAnimationFinished( void ) const       { return mAnimationFinished; };

    const AssetPtr<AnimationAsset>&                 getAnimationAsset( void ) const         { return mAnimationAsset; };

    void                                            setAnimationFrame( const U32 frameIndex );
    void                                            setAnimationTimeScale( const F32 scale ) { mAnimationTimeScale = scale; }
    inline F32                                      getAnimationTimeScale( void ) const     { return mAnimationTimeScale; }

    void                                            clearAssets( void )                     { mAnimationAsset.clear(); mLastAnimationAsset.clear(); }

    bool playAnimation( const AssetPtr<AnimationAsset>& animationAsset, const bool autoRestore );
    bool updateAnimation( const F32 elapsedTime );
    void stopAnimation( void );
    void resetTime( void );
};


#endif // _ANIMATION_CONTROLLER_H_
