//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_H_
#include "Sprite.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _STRINGBUFFER_H_
#include "string/stringBuffer.h"
#endif

// Script bindings.
#include "Sprite_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(Sprite);

//------------------------------------------------------------------------------

Sprite::Sprite()
{
}

//------------------------------------------------------------------------------

Sprite::~Sprite()
{
}

//------------------------------------------------------------------------------

void Sprite::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();
}

//------------------------------------------------------------------------------

void Sprite::sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // Static mode?
    if ( isStaticMode() )
    {
        // Yes, so finish if no image-map.
        if ( mImageAsset.isNull() )
            return;

        // Fetch current frame area.
        ImageAsset::FrameArea::TexelArea frameTexelArea = mImageAsset->getImageFrameArea( mImageFrame ).mTexelArea;

        // Flip texture coordinates appropriately.
        frameTexelArea.setFlip( mFlipX, mFlipY );
   
        // Fetch lower/upper texture coordinates.
        const Vector2& texLower = frameTexelArea.mTexelLower;
        const Vector2& texUpper = frameTexelArea.mTexelUpper;
    
        // Submit batched quad.
        pBatchRenderer->SubmitQuad(
            mRenderOOBB[0],
            mRenderOOBB[1],
            mRenderOOBB[2],
            mRenderOOBB[3],
            Vector2( texLower.x, texUpper.y ),
            Vector2( texUpper.x, texUpper.y ),
            Vector2( texUpper.x, texLower.y ),
            Vector2( texLower.x, texLower.y ),
            mImageAsset->getImageTexture() );

        return;
    }
    
    // Finish if no animation.
    if ( mAnimationAsset.isNull() )
        return;

    // Fetch current frame area.
    ImageAsset::FrameArea::TexelArea frameTexelArea = mpAnimationController->getCurrentImageFrameArea().mTexelArea;

    // Flip texture coordinates appropriately.
    frameTexelArea.setFlip( mFlipX, mFlipY );
   
    // Fetch lower/upper texture coordinates.
    const Vector2& texLower = frameTexelArea.mTexelLower;
    const Vector2& texUpper = frameTexelArea.mTexelUpper;
    
    // Submit batched quad.
    pBatchRenderer->SubmitQuad(
        mRenderOOBB[0],
        mRenderOOBB[1],
        mRenderOOBB[2],
        mRenderOOBB[3],
        Vector2( texLower.x, texUpper.y ),
        Vector2( texUpper.x, texUpper.y ),
        Vector2( texUpper.x, texLower.y ),
        Vector2( texLower.x, texLower.y ),
        mpAnimationController->getImageTexture() );
}


