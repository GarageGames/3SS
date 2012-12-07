//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_BATCH_ITEM_H_
#include "2d/core/SpriteBatchItem.h"
#endif

#ifndef _SPRITE_BATCH_H_
#include "2d/core/SpriteBatch.h"
#endif

#ifndef _SCENE_RENDER_REQUEST_H_
#include "2d/scene/SceneRenderRequest.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

//-----------------------------------------------------------------------------

static bool spriteBatchItemPropertiesInitialized = false;

static StringTableEntry spriteKeyName;
static StringTableEntry spriteVisibleName;
static StringTableEntry spriteLocalPositionName;
static StringTableEntry spriteLocalAngleName;
static StringTableEntry spriteDepthName;
static StringTableEntry spriteSizeName;
static StringTableEntry spriteFlipXName;
static StringTableEntry spriteFlipYName;
static StringTableEntry spriteSortPointName;
static StringTableEntry spriteBlendModeName;
static StringTableEntry spriteSrcBlendFactorName;
static StringTableEntry spriteDstBlendFactorName;
static StringTableEntry spriteBlendColor;
static StringTableEntry spriteAlphaTest;

//------------------------------------------------------------------------------

SpriteBatchItem::SpriteBatchItem() : mProxyId( -1 )
{
    // Are the sprite batch item properties initialized?
    if ( !spriteBatchItemPropertiesInitialized )
    {
        // No, so initialize...

        spriteKeyName              = StringTable->insert("Key");
        spriteVisibleName          = StringTable->insert("Visible");
        spriteLocalPositionName    = StringTable->insert("Position");
        spriteLocalAngleName       = StringTable->insert("Angle");
        spriteSizeName             = StringTable->insert("Size");
        spriteDepthName            = StringTable->insert("Depth");
        spriteFlipXName            = StringTable->insert("FlipX");
        spriteFlipYName            = StringTable->insert("FlipY");
        spriteSortPointName        = StringTable->insert("SortPoint");
        spriteBlendModeName        = StringTable->insert("BlendMode");
        spriteSrcBlendFactorName   = StringTable->insert("SrcBlendFactor");
        spriteDstBlendFactorName   = StringTable->insert("DstBlendFactor");
        spriteBlendColor           = StringTable->insert("BlendColor");
        spriteAlphaTest            = StringTable->insert("AlphaTest");

        // Flag as initialized.
        spriteBatchItemPropertiesInitialized = true;
    }

    resetState();
}

//------------------------------------------------------------------------------

SpriteBatchItem::~SpriteBatchItem()
{    
    resetState();
}

//------------------------------------------------------------------------------

void SpriteBatchItem::resetState( void )
{
    // Call parent.
    Parent::resetState();

    // Do we have a proxy.
    if ( mProxyId != -1 )
    {
        // Sanity!
        AssertFatal( mSpriteBatch != NULL, "Cannot remove proxy with NULL sprite batch." );

        // Destroy proxy.
        mSpriteBatch->DestroyProxy( mProxyId );
        mProxyId = -1;
    }

    mSpriteBatch = NULL;
    mBatchId = 0;
    mKey = NULL;

    mVisible = true;

    mLocalTransform.SetIdentity();
    mLocalPosition.SetZero();
    mDepth = 0.0f;
    mLocalAngle = 0.0f;
    setSize( Vector2( 1.0f, 1.0f ) );
    mRenderAABB.lowerBound.Set( -0.5f, -0.5f );
    mRenderAABB.upperBound.Set( 0.5f, 0.5f );

    mFlipX = false;
    mFlipY = false;

    mSortPoint.SetZero();

    mBlendMode = true;
    mSrcBlendFactor = GL_SRC_ALPHA;
    mDstBlendFactor = GL_ONE_MINUS_SRC_ALPHA;
    mBlendColor = ColorF(1.0f,1.0f,1.0f,1.0f);
    mAlphaTest = -1.0f;
    
    mLocalTransformDirty = true;
    mWorldTransformDirty = true;

    // Require self ticking.
    mSelfTick = true;
}

//------------------------------------------------------------------------------

void SpriteBatchItem::setBatchParent( SpriteBatch* pSpriteBatch, const U32 batchId )
{
    // Sanity!
    AssertFatal( pSpriteBatch != NULL, "Cannot assign a NULL batch parent." );
    AssertFatal( mSpriteBatch == NULL, "Cannot assign batch parent as one is already assigned." );
    AssertFatal( batchId != 0, "Cannot assign a zero batch Id." );

    // Assign.
    mSpriteBatch = pSpriteBatch;
    mBatchId = batchId;

    // Create proxy.
    mProxyId = mSpriteBatch->CreateProxy( mRenderAABB, this );
}

//------------------------------------------------------------------------------

void SpriteBatchItem::copyTo( SpriteBatchItem* pSpriteBatchItem ) const
{
    // Call parent.
    Parent::copyTo( pSpriteBatchItem );

    // Set sprite batch item.
    pSpriteBatchItem->setVisible( getVisible() );
    pSpriteBatchItem->setLocalPosition( getLocalPosition() );
    pSpriteBatchItem->setDepth( getDepth() );
    pSpriteBatchItem->setLocalAngle( getLocalAngle() );
    pSpriteBatchItem->setSize( getSize() );
    pSpriteBatchItem->setFlipX( getFlipX() );
    pSpriteBatchItem->setFlipY( getFlipY() );
    pSpriteBatchItem->setSortPoint( getSortPoint() );
    pSpriteBatchItem->setBlendMode( getBlendMode() );
    pSpriteBatchItem->setSrcBlendFactor( getSrcBlendFactor() );
    pSpriteBatchItem->setDstBlendFactor( getDstBlendFactor() );
    pSpriteBatchItem->setBlendColor( getBlendColor() );
    pSpriteBatchItem->setAlphaTest( getAlphaTest() );
    pSpriteBatchItem->setWorldTransformDirty();
}

//------------------------------------------------------------------------------

void SpriteBatchItem::prepareRender( SceneRenderRequest* pSceneRenderRequest )
{
    // Sanity!
    AssertFatal( pSceneRenderRequest != NULL, "Cannot prepare a sprite batch with a NULL scene render request." );

    pSceneRenderRequest->mWorldPosition = getWorldPosition();
    pSceneRenderRequest->mDepth = getDepth();
    pSceneRenderRequest->mSortPoint = getSortPoint();
    pSceneRenderRequest->mSerialId = getBatchId();
    pSceneRenderRequest->mBlendMode = getBlendMode();
    pSceneRenderRequest->mSrcBlendFactor = getSrcBlendFactor();
    pSceneRenderRequest->mDstBlendFactor = getDstBlendFactor();
    pSceneRenderRequest->mBlendColor = getBlendColor();
    pSceneRenderRequest->mAlphaTest = getAlphaTest();
}

//------------------------------------------------------------------------------

void SpriteBatchItem::render( BatchRender* pBatchRenderer )
{
    // Ensure the transform is up-to-date.
    if ( mLocalTransformDirty || mWorldTransformDirty )
        updateTransform();

    // Render.
    Parent::render( mFlipX, mFlipY,
                    mRenderOOBB[0],
                    mRenderOOBB[1],
                    mRenderOOBB[2],
                    mRenderOOBB[3],
                    pBatchRenderer );
}

//------------------------------------------------------------------------------

void SpriteBatchItem::updateTransform( void )
{
    // Sanity!
    AssertFatal( mSpriteBatch != NULL, "Cannot update transform with a NULL sprite batch." );
    AssertFatal( mProxyId != -1, "Cannot update transform without a proxy Id." );

    // Early-out if nothing to do.
    if ( !mLocalTransformDirty && !mWorldTransformDirty )
        return;

    // Set the sprite batch render AABB as dirty.
    mSpriteBatch->setRenderAABBDirty();
    
    // Is the local transform dirty?
    if ( mLocalTransformDirty )
    {
        // Yes, so flag the world transform as dirty as well.
        mWorldTransformDirty = true;

        // Update local transform.
        mLocalTransform.p = mLocalPosition;
        mLocalTransform.q.Set( mLocalAngle );

        // Calculate half size.
        const F32 halfWidth = mSize.x * 0.5f;
        const F32 halfHeight = mSize.y * 0.5f;

        // Set local size vertices.
        mLocalOOBB[0].Set( -halfWidth, -halfHeight );
        mLocalOOBB[1].Set( +halfWidth, -halfHeight );
        mLocalOOBB[2].Set( +halfWidth, +halfHeight );
        mLocalOOBB[3].Set( -halfWidth, +halfHeight );

        // Calculate local OOBB.
        CoreMath::mCalculateOOBB( mLocalOOBB, mLocalTransform, mLocalOOBB );

        // Flag local transform as NOT dirty.
        mLocalTransformDirty = false;
    }

    // Finish if the world transform is not dirty.
    if ( !mWorldTransformDirty )
        return;

    // Fetch world transform.
    const b2Transform& worldTransform = mSpriteBatch->getBatchTransform();

    // Calculate world OOBB.
    CoreMath::mCalculateOOBB( mLocalOOBB, worldTransform, mRenderOOBB );

    // Calculate AABB.
    b2Vec2 lower = mRenderOOBB[0];
    b2Vec2 upper = lower;
    for ( U32 index = 1; index < 4; ++index )
    {
        const b2Vec2 v = mRenderOOBB[index];
        lower = b2Min(lower, v);
        upper = b2Max(upper, v);
    }
    mRenderAABB.lowerBound = lower;
    mRenderAABB.upperBound = upper;

    // Move proxy.
    mSpriteBatch->MoveProxy( mProxyId, mRenderAABB, b2Vec2(0.0f, 0.0f) );

    // Flag world transform as NOT dirty.
    mWorldTransformDirty = false;
}

//------------------------------------------------------------------------------

void SpriteBatchItem::onTamlCustomWrite( TamlPropertyTypeAlias* pSpriteTypeAlias )
{
    // Write visible.
    if ( !mVisible )
        pSpriteTypeAlias->addPropertyField( spriteVisibleName, mVisible );

    // Write local position.
    pSpriteTypeAlias->addPropertyField( spriteLocalPositionName, mLocalPosition );

    // Write local angle.
    if ( mNotZero(mLocalAngle) )
        pSpriteTypeAlias->addPropertyField( spriteLocalAngleName, mRadToDeg(mLocalAngle) );

    // Write size.
    pSpriteTypeAlias->addPropertyField( spriteSizeName, mSize );

    // Write depth.
    if ( mNotZero(mDepth) )
        pSpriteTypeAlias->addPropertyField( spriteDepthName, mDepth );

    // Write flipX
    if ( mFlipX )
        pSpriteTypeAlias->addPropertyField( spriteFlipXName, mFlipX );

    // Write flipY
    if ( mFlipY )
        pSpriteTypeAlias->addPropertyField( spriteFlipYName, mFlipY );

    // Write sort point.
    if ( mSortPoint.notZero() )
        pSpriteTypeAlias->addPropertyField( spriteSortPointName, mSortPoint );

    // Write blend mode.
    if ( !mBlendMode )
        pSpriteTypeAlias->addPropertyField( spriteBlendModeName, mBlendMode );

    // Write source blend factor.
    if ( mSrcBlendFactor != GL_SRC_ALPHA )
        pSpriteTypeAlias->addPropertyField( spriteBlendModeName, getSrcBlendFactorDescription(mSrcBlendFactor) );
        
    // Write destination blend factor.
    if ( mDstBlendFactor != GL_ONE_MINUS_SRC_ALPHA )
        pSpriteTypeAlias->addPropertyField( spriteDstBlendFactorName, getDstBlendFactorDescription(mDstBlendFactor) );

    // Write blend color.
    if ( mBlendColor != ColorF(1.0f, 1.0f, 1.0f, 1.0f) )
        pSpriteTypeAlias->addPropertyField( spriteBlendColor, mBlendColor );

    // Write alpha test.
    if ( mAlphaTest >= 0.0f )
        pSpriteTypeAlias->addPropertyField( spriteAlphaTest, mAlphaTest );

    // Write key.
    pSpriteTypeAlias->addPropertyField( spriteKeyName, getKey() );
}

//------------------------------------------------------------------------------

void SpriteBatchItem::onTamlCustomRead( const TamlPropertyTypeAlias* pSpriteTypeAlias )
{
    // Sanity!
    AssertFatal( mSpriteBatch != NULL, "Cannot read sprite batch item with sprite batch." );

    // Iterate property fields.
    for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pSpriteTypeAlias->begin(); propertyFieldItr != pSpriteTypeAlias->end(); ++propertyFieldItr )
    {
        // Fetch sprite field.
        TamlPropertyField* pSpriteField = *propertyFieldItr;

        // Fetch sprite field name.
        StringTableEntry fieldName = pSpriteField->getFieldName();

        if ( fieldName == spriteVisibleName )
        {
            bool visible;
            pSpriteField->getFieldValue( visible );
            setVisible( visible );
        }
        else if ( fieldName == spriteLocalPositionName )
        {
            Vector2 localPosition;
            pSpriteField->getFieldValue( localPosition );
            setLocalPosition( localPosition );
        }
        else if ( fieldName == spriteLocalAngleName )
        {
            F32 localAngle;
            pSpriteField->getFieldValue( localAngle );
            setLocalAngle( mDegToRad( localAngle ) );
        }
        else if ( fieldName == spriteSizeName )
        {
            Vector2 size;
            pSpriteField->getFieldValue( size );
            setSize( size );
        }
        else if ( fieldName == spriteDepthName )
        {
            F32 depth;
            pSpriteField->getFieldValue( depth );
            setDepth( depth );
        }
        else if ( fieldName == spriteFlipXName )
        {
            bool flipX;
            pSpriteField->getFieldValue( flipX );
            setFlipX( flipX );
        }
        else if ( fieldName == spriteFlipYName )
        {
            bool flipY;
            pSpriteField->getFieldValue( flipY );
            setFlipY( flipY );
        }
        else if ( fieldName == spriteSortPointName )
        {
            Vector2 sortPoint;
            pSpriteField->getFieldValue( sortPoint );
            setSortPoint( sortPoint );
        }
        else if ( fieldName == spriteBlendModeName )
        {
            bool blendMode;
            pSpriteField->getFieldValue( blendMode );
            setBlendMode( blendMode );
        }
        else if ( fieldName == spriteSrcBlendFactorName )
        {
            setSrcBlendFactor( (GLenum)getSrcBlendFactorEnum( pSpriteField->getFieldValue() ) );
        }
        else if ( fieldName == spriteDstBlendFactorName )
        {
            setDstBlendFactor( (GLenum)getDstBlendFactorEnum( pSpriteField->getFieldValue() ) );
        }
        else if ( fieldName == spriteBlendColor )
        {
            ColorF blendColor;
            pSpriteField->getFieldValue( blendColor );
            setBlendColor( blendColor );
        }
        else if ( fieldName == spriteAlphaTest )
        {
            F32 alphaTest;
            pSpriteField->getFieldValue( alphaTest );
            setAlphaTest( alphaTest );
        }
        // Key.
        else if ( fieldName == spriteKeyName )
        {
            // Fetch sprite key.
            const char* pSpriteKey = pSpriteField->getFieldValue();

            // Is there any key data?
            if ( dStrlen( pSpriteKey ) == 0 )
            {
                // No, so warn.
                Con::warnf( "Encountered an empty sprite key.  This tile will no longer be addressable by logical position." );
                continue;
            }

            const char* pLogicalPositionSeparator = ",\t ";
            const S32 argCount = (S32)StringUnit::getUnitCount( pSpriteKey, pLogicalPositionSeparator );

            if ( argCount == 1 )
            {
                setKey( StringTable->insert( pSpriteKey ) );
                continue;
            }

            // Do we have a valid argument count?
            if ( argCount > 6 )
            {
                // No, so warn.
                Con::warnf( "Encountered an invalid logical position in sprite key '%s'.", pSpriteKey );
                continue;
            }

            // Format arguments.
            const char* args[6];
            for ( S32 index = 0; index < argCount; ++index )
            {
                args[index] = StringUnit::getUnit( pSpriteKey, index, pLogicalPositionSeparator );
            }

            // Set sprite key.
            setKey( mSpriteBatch->getSpriteKey( SpriteBatch::LogicalPosition( argCount, args ) ) );
        }
    }
}
