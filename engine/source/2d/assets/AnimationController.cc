//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/gBitmap.h"
#include "console/consoleTypes.h"
#include "io/bitStream.h"
#include "2d/sceneobject/SceneObject.h"
#include "2d/assets/AnimationAsset.h"
#include "AnimationController.h"


//-----------------------------------------------------------------------------

AnimationController::AnimationController() :
    mCurrentFrameIndex(0),
    mLastFrameIndex(0),
    mMaxFrameIndex(0),
    mCurrentTime(0.0f),
    mPausedTime(0.0f),
    mAnimationTimeScale(1.0f),
    mTotalIntegrationTime(0.0f),
    mFrameIntegrationTime(0.0f),
    mAutoRestoreAnimation(false),
    mAnimationFinished(true)
{
    // Register for animation asset refresh notifications.
    mAnimationAsset.registerRefreshNotify( this );
}

//-----------------------------------------------------------------------------

AnimationController::~AnimationController()
{
}

//-----------------------------------------------------------------------------

void AnimationController::onAssetRefreshed( AssetPtrBase* pAssetPtrBase )
{
    // Attempt to restart the animation.
    playAnimation( mAnimationAsset, false );
}

//-----------------------------------------------------------------------------

bool AnimationController::playAnimation( const AssetPtr<AnimationAsset>& animationAsset, const bool autoRestore )
{
    // Stop animation.
    stopAnimation();

    // Finish if no animation asset.
    if ( animationAsset.isNull() )
        return true;

    // Fetch validated frames.
    const Vector<S32>& validatedFrames = animationAsset->getValidatedAnimationFrames();

    // Check we've got some frames.
    if ( validatedFrames.size() == 0 )
    {
        Con::warnf( "AnimationController::playAnimation() - Cannot play AnimationAsset datablock (%s) - Animation has no validated frames!", mAnimationAsset.getAssetId() );
        return false;
    }

    // Set last animation asset.
    if ( autoRestore )
        mLastAnimationAsset = mAnimationAsset;
    else
        mLastAnimationAsset.clear();

    // Set animation asset.
    mAnimationAsset = animationAsset;

    // Set Maximum Frame Index.
    mMaxFrameIndex = validatedFrames.size()-1;

    // Calculate Total Integration Time.
    mTotalIntegrationTime = mAnimationAsset->getAnimationTime();

    // Calculate Frame Integration Time.
    mFrameIntegrationTime = mTotalIntegrationTime / validatedFrames.size();

    // No, so random Start?
    if ( mAnimationAsset->getRandomStart() )
    {
        // Yes, so calculate start time.
        mCurrentTime = CoreMath::mGetRandomF(0.0f, mTotalIntegrationTime*0.999f);
    }
    else
    {
        // No, so set first frame.
        mCurrentTime = 0.0f;
    }

    // Set Auto Restore Animation Flag.
    mAutoRestoreAnimation = autoRestore;

    // Reset animation finished flag.
    mAnimationFinished = false;

    // Do an initial animation update.
    updateAnimation(0.0f);

    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------

void AnimationController::stopAnimation( void )
{
    // Flag as animation finished.
    mAnimationFinished = true;
}

//-----------------------------------------------------------------------------

bool AnimationController::updateAnimation( const F32 elapsedTime )
{
    // Finish if animation asset is not valid.
    if ( mAnimationAsset.isNull() || mAnimationAsset->getImageMap().isNull() )
        return false;

    // Finish if animation has finished.
    if ( mAnimationFinished )
        return false;

    // Fetch validated frames.
    const Vector<S32>& validatedFrames = mAnimationAsset->getValidatedAnimationFrames();

    // Finish if there are no validated frames.
    if ( validatedFrames.size() == 0 )
        return false;

    // Calculate scaled time.
    const F32 scaledTime = elapsedTime * mAnimationTimeScale;

    // Update Current Time.
    mCurrentTime += scaledTime;

    // Check if the animation has finished.
    if ( !mAnimationAsset->getAnimationCycle() && mGreaterThanOrEqual(mCurrentTime, mTotalIntegrationTime) )
    {
        // Animation has finished.
        mAnimationFinished = true;

        // Are we restoring the animation?
        if ( mAutoRestoreAnimation )
        {
            // Yes, so play last animation.
            playAnimation( mLastAnimationAsset, false );
        }
        else
        {
            // No, so fix Animation at end of frames.
            mCurrentTime = mTotalIntegrationTime - (mFrameIntegrationTime * 0.5f);
        }
    }

    // Update Current Mod Time.
    mCurrentModTime = mFmod( mCurrentTime, mTotalIntegrationTime );

    // Calculate Current Frame.
    mCurrentFrameIndex = (S32)(mCurrentModTime / mFrameIntegrationTime);

    // Fetch frame.
    S32 frame = validatedFrames[mCurrentFrameIndex];

    // Fetch image frame count.
    const S32 imageFrameCount = mAnimationAsset->getImageMap()->getFrameCount();

    // Clamp frames.
    if ( frame < 0 )
        frame = 0;
    else if (frame >= imageFrameCount )
        frame = imageFrameCount-1;

    // Calculate if frame has changed.
    bool frameChanged = (mCurrentFrameIndex != mLastFrameIndex);

    // Reset Last Frame.
    mLastFrameIndex = mCurrentFrameIndex;

    // Return Frame-Changed Flag.
    return frameChanged;
}

//-----------------------------------------------------------------------------

void AnimationController::resetTime( void )
{
    // Rest Time.
    mCurrentTime = 0.0f;
}

//-----------------------------------------------------------------------------

void AnimationController::setAnimationFrame( const U32 frameIndex )
{
    // We *must* have a valid datablock and be still animating!
    if ( mAnimationAsset.isNull() )
    {
        // Warn.
        Con::warnf("AnimationController::setAnimationFrame() - Cannot set frame; animation is finished or is invalid!");
        return;
    }

    // Validate Frame Index?
    if ( (S32)frameIndex < 0 || frameIndex > mMaxFrameIndex )
    {
        // No, so warn.
        Con::warnf("AnimationController::setAnimationFrame() - Animation Frame-Index Invalid (frame#%d of %d in %s)", frameIndex, mMaxFrameIndex, mAnimationAsset.getAssetId() );
        // Finish here.
        return;
    }

    // Calculate current time.
    mCurrentTime = frameIndex*mFrameIntegrationTime;

    // Do an immediate animation update.
    updateAnimation(0.0f);
}