//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _COMPOSITE_SPRITE_H_
#include "2d/sceneobject/CompositeSprite.h"
#endif

#ifndef _SPRITE_BATCH_ITEM_H_
#include "2d/core/SpriteBatchItem.h"
#endif

#ifndef _RENDER_PROXY_H_
#include "2d/core/RenderProxy.h"
#endif

// Script bindings.
#include "2d/sceneobject/CompositeSprite_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(CompositeSprite);

//------------------------------------------------------------------------------

CompositeSprite::CompositeSprite()
{
    // Set as auto-sizing.
    mAutoSizing = true;
}

//------------------------------------------------------------------------------

CompositeSprite::~CompositeSprite()
{
}

//------------------------------------------------------------------------------

void CompositeSprite::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    /// Defaults.
    addProtectedField("DefaultSpriteStride", TypeVector2, Offset(mDefaultSpriteStride, CompositeSprite), &defaultProtectedSetFn, &defaultProtectedGetFn, &defaultProtectedWriteFn, "");
    addProtectedField("DefaultSpriteSize", TypeVector2, Offset(mDefaultSpriteSize, CompositeSprite), &defaultProtectedSetFn, &defaultProtectedGetFn, &defaultProtectedWriteFn, "");
    addProtectedField("DefaultSpriteAngle", TypeF32, Offset(mDefaultSpriteSize, CompositeSprite), &setDefaultSpriteAngle, &getDefaultSpriteAngle, &writeDefaultSpriteAngle, "");
    addField( "BatchIsolated", TypeBool, Offset(mBatchIsolated, CompositeSprite), &writeBatchIsolated, "");
    addField( "BatchSortMode", TypeEnum, Offset(mSortMode, CompositeSprite), &writeBatchSortMode, 1, &SceneRenderQueue::renderSortTable, "");
}

//-----------------------------------------------------------------------------

void CompositeSprite::preIntegrate( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
    // Are the spatials dirty?
    if ( getSpatialDirty() )
    {
        // Yes, so update the batch world transform.
        setBatchTransform( getRenderTransform() );
    }

    // Is the render AABB dirty?
    if ( getRenderAABBDirty() )
    {
        // Yes, so fetch the render AABB.
        const b2AABB& renderAABB = getRenderAABB();
    
        // Fetch render transform.
        const b2Transform renderTransform = getRenderTransform();

        // Calculate local render extents.
        b2Vec2 localLowerExtent = b2MulT( renderTransform, renderAABB.lowerBound );
        b2Vec2 localUpperExtent = b2MulT( renderTransform, renderAABB.upperBound );

        // Calculate size.
        const Vector2 size(
            mFabs(localUpperExtent.x > localLowerExtent.x ? localUpperExtent.x : localLowerExtent.x) * 2.0f,
            mFabs(localUpperExtent.y > localLowerExtent.y ? localUpperExtent.y : localLowerExtent.y) * 2.0f );

        // Set size.
        setSize( size );
    }

    // Call parent.
    Parent::preIntegrate( totalTime, elapsedTime, pDebugStats );
}

//-----------------------------------------------------------------------------

void CompositeSprite::integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
    // Call Parent.
    Parent::integrateObject( totalTime, elapsedTime, pDebugStats );

    // Finish if the spatials are NOT dirty.
    if ( !getSpatialDirty() )
        return;

    // Update the batch world transform.
    setBatchTransform( getRenderTransform() );
}

//-----------------------------------------------------------------------------

void CompositeSprite::interpolateObject( const F32 timeDelta )
{
    // Call parent.
    Parent::interpolateObject( timeDelta );

    // Finish if the spatials are NOT dirty.
    if ( !getSpatialDirty() )
        return;

    // Update the batch world transform.
    setBatchTransform( getRenderTransform() );
}

//-----------------------------------------------------------------------------

void CompositeSprite::scenePrepareRender( const SceneRenderState* pSceneRenderState, SceneRenderQueue* pSceneRenderQueue )
{
    // Prepare render.
    SpriteBatch::prepareRender( this, pSceneRenderState, pSceneRenderQueue );
}

//-----------------------------------------------------------------------------

void CompositeSprite::sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // Render.
    SpriteBatch::render( pSceneRenderState, pSceneRenderRequest, pBatchRenderer );
}

//------------------------------------------------------------------------------

void CompositeSprite::copyTo(SimObject* object)
{
    // Call to parent.
    Parent::copyTo(object); 

    // Call sprite batch.
    SpriteBatch::copyTo( dynamic_cast<SpriteBatch*>(object) );
}

//-----------------------------------------------------------------------------

void CompositeSprite::onTamlCustomWrite( TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomWrite( customCollection );

    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Fetch sprite count.
    const U32 spriteCount = getSpriteCount();

    // Finish if no sprites.
    if ( spriteCount == 0 )
        return;

    // Add sprites property.
    TamlCollectionProperty* pSpritesProperty = customCollection.addCollectionProperty( StringTable->insert("Sprites") );

    // Write property with sprite batch.
    SpriteBatch::onTamlCustomWrite( pSpritesProperty );
}

//-----------------------------------------------------------------------------

void CompositeSprite::onTamlCustomRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomRead( customCollection );

    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Find sprites collection.
    const TamlCollectionProperty* pSpritesProperty = customCollection.findProperty( StringTable->insert("Sprites") );

    // Finish if we don't have the property.
    if ( pSpritesProperty == NULL )
        return;

    // Read property with sprite batch.
    SpriteBatch::onTamlCustomRead( pSpritesProperty );
}

