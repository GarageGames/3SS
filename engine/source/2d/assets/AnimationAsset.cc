//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/consoleTypes.h"
#include "2d/assets/ImageAsset.h"
#include "2d/assets/AnimationAsset.h"

// Script bindings.
#include "AnimationAsset_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(AnimationAsset);

//------------------------------------------------------------------------------

AnimationAsset::AnimationAsset() :    mAnimationTime(1.0f),
                                                    mAnimationCycle(true),
                                                    mRandomStart(false),
                                                    mAnimationIntegration(0.0f)
{
    // Set Vector Associations.
    VECTOR_SET_ASSOCIATION( mAnimationFrames );
    VECTOR_SET_ASSOCIATION( mValidatedFrames );    
}

//------------------------------------------------------------------------------

AnimationAsset::~AnimationAsset()
{
}

//------------------------------------------------------------------------------

void AnimationAsset::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    addProtectedField("ImageMap", TypeImageMapAssetPtr, Offset(mImageAsset, AnimationAsset), &setImageMap, &defaultProtectedGetFn, &defaultProtectedWriteFn, "");
    addProtectedField("AnimationFrames", TypeS32Vector, Offset(mAnimationFrames, AnimationAsset), &setAnimationFrames, &defaultProtectedGetFn, &writeAnimationFrames, "");
    addProtectedField("AnimationTime", TypeF32, Offset(mAnimationTime, AnimationAsset), &setAnimationTime, &defaultProtectedGetFn, &defaultProtectedWriteFn, "");
    addProtectedField("AnimationCycle", TypeBool, Offset(mAnimationCycle, AnimationAsset), &setAnimationCycle, &defaultProtectedGetFn, &writeAnimationCycle, "");
    addProtectedField("RandomStart", TypeBool, Offset(mRandomStart, AnimationAsset), &setRandomStart, &defaultProtectedGetFn, &writeRandomStart, "");
}

//------------------------------------------------------------------------------

bool AnimationAsset::onAdd()
{
    // Call Parent.
    if(!Parent::onAdd())
        return false;

    // Return Okay.
    return true;
}

//------------------------------------------------------------------------------

void AnimationAsset::onRemove()
{
    // Call Parent.
    Parent::onRemove();
}

//------------------------------------------------------------------------------

void AnimationAsset::onAssetRefresh( void ) 
{
    // Ignore if not yet added to the sim.
    if ( !isProperlyAdded() )
        return;

    // Call parent.
    Parent::onAssetRefresh();
}

//------------------------------------------------------------------------------

void AnimationAsset::setImageMap( const char* pAssetId )
{
    // Ignore no change.
    if ( mImageAsset.getAssetId() == StringTable->insert( pAssetId ) )
        return;

    // Update.
    mImageAsset = pAssetId;

    // Validate frames.
    validateFrames();

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void AnimationAsset::setAnimationFrames( const char* pAnimationFrames )
{
    // Clear any existing frames.
    mAnimationFrames.clear();

    // Fetch frame count.
    const U32 frameCount = StringUnit::getUnitCount( pAnimationFrames, " \t\n" );

    // Iterate frames.
    for( U32 frameIndex = 0; frameIndex < frameCount; ++frameIndex )
    {
        // Store frame.
        mAnimationFrames.push_back( dAtoi( StringUnit::getUnit( pAnimationFrames, frameIndex, " \t\n" ) ) );
    }

    // Validate frames.
    validateFrames();

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void AnimationAsset::setAnimationTime( const F32 animationTime )
{
    // Ignore no change,
    if ( mIsEqual( animationTime, mAnimationTime ) )
        return;

    // Update.
    mAnimationTime = animationTime;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void AnimationAsset::setAnimationCycle( const bool animationCycle )
{
    // Ignore no change.
    if ( animationCycle == mAnimationCycle )
        return;

    // Update.
    mAnimationCycle = animationCycle;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void AnimationAsset::setRandomStart( const bool randomStart )
{
    // Ignore no change.
    if ( randomStart == mRandomStart )
        return;

    // Update.
    mRandomStart = randomStart;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void AnimationAsset::validateFrames( void )
{
    // Clear validated frames.
    mValidatedFrames.clear();

    // Finish if we don't have a valid image-map asset.
    if ( mImageAsset.isNull() )
        return;

    // Fetch Animation Frame Count.
    const U32 animationFrameCount = (U32)mAnimationFrames.size();

    // Finish if no animation frames are specified.
    if ( animationFrameCount == 0 )
        return;

    // Fetch image-map frame count.
    const S32 imageMapFrameCount = (S32)mImageAsset->getFrameCount();

    // Finish if the image-map has no frames.
    if ( imageMapFrameCount == 0 )
        return;

    // Validate each specified frame.
    for ( U32 frameIndex = 0; frameIndex < animationFrameCount; ++frameIndex )
    {
        // Fetch frame.
        S32 frame = mAnimationFrames[frameIndex];

        // Valid Frame?
        if ( frame < 0 || frame >= imageMapFrameCount )
        {
            // No, warn.
            Con::warnf( "AnimationAsset::validateFrames() - Animation asset '%s' specifies an out-of-bound frame of '%d' (key-index:'%d') against image-map asset Id '%s'.",
                getAssetName(),
                frame,
                frameIndex,
                mImageAsset.getAssetId() );

            if ( frame < 0 )
                frame = 0;
            else if ( frame >= imageMapFrameCount )
                frame = imageMapFrameCount-1;
        }

        // Use frame.
        mValidatedFrames.push_back( frame );
    }
}

//------------------------------------------------------------------------------

void AnimationAsset::initializeAsset( void )
{
    // Call parent.
    Parent::initializeAsset();

    // Currently there is not specified initialization required.
}

