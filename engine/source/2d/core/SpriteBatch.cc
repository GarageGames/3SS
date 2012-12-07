//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_BATCH_H_
#include "SpriteBatch.h"
#endif

#ifndef _SCENE_RENDER_OBJECT_H_
#include "2d/scene/SceneRenderObject.h"
#endif

//------------------------------------------------------------------------------

SpriteBatch::SpriteBatch() :
    mMasterBatchId( 1 ),
    mSelectedSprite( NULL ),
    mSortMode( SceneRenderQueue::RENDER_SORT_OFF ),
    mDefaultSpriteStride( 1.0f, 1.0f),
    mDefaultSpriteSize( 1.0f, 1.0f ),
    mDefaultSpriteAngle( 0.0f )

{
    mWorldTransform.SetIdentity();
    mRenderAABB.lowerBound.Set(-0.5f, -0.5f);
    mRenderAABB.upperBound.Set(0.5f, 0.5f);
    mRenderAABBDirty = true;
}

//------------------------------------------------------------------------------

SpriteBatch::~SpriteBatch()
{
    clearSprites();
}

//-----------------------------------------------------------------------------

void SpriteBatch::prepareRender( SceneRenderObject* pSceneRenderObject, const SceneRenderState* pSceneRenderState, SceneRenderQueue* pSceneRenderQueue )
{
    // Set the sort mode.
    pSceneRenderQueue->setSortMode( getSortMode() );

    // Perform render query.
    mRenderQuery.clear();
    Query( this, pSceneRenderState->mRenderAABB );

    // Iterate the sprite batch.
    for( typeSpriteItemVector::iterator spriteItr = mRenderQuery.begin(); spriteItr != mRenderQuery.end(); ++spriteItr )
    {
        // Fetch sprite batch Item.
        SpriteBatchItem* pSpriteBatchItem = *spriteItr;

        // Skip if not visible.
        if ( !pSpriteBatchItem->getVisible() )
            continue;

        // Create a render request.
        SceneRenderRequest* pSceneRenderRequest = pSceneRenderQueue->createRenderRequest();

        // Prepare batch item.
        pSpriteBatchItem->prepareRender( pSceneRenderRequest );

        // Set identity.
        pSceneRenderRequest->mpSceneRenderObject = pSceneRenderObject;

        // Set custom data.
        pSceneRenderRequest->mpCustomData1 = pSpriteBatchItem;
    }

    // Clear render query.
    mRenderQuery.clear();
}

//------------------------------------------------------------------------------

void SpriteBatch::render( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // Fetch sprite batch Item.
    SpriteBatchItem* pSpriteBatchItem = (SpriteBatchItem*)pSceneRenderRequest->mpCustomData1;

    // Batch render.
    pSpriteBatchItem->render( pBatchRenderer );
}

//------------------------------------------------------------------------------

void SpriteBatch::copyTo( SpriteBatch* pSpriteBatch ) const
{
    // Clear any existing sprites.
    pSpriteBatch->clearSprites();

    // Set master sprite Id.
    pSpriteBatch->mMasterBatchId = mMasterBatchId;

    // Set sort mode.
    pSpriteBatch->setSortMode( getSortMode() );

    // Set sprite default size and angle.
    pSpriteBatch->setDefaultSpriteStride( getDefaultSpriteStride() );
    pSpriteBatch->setDefaultSpriteSize( getDefaultSpriteSize() );
    pSpriteBatch->setDefaultSpriteAngle( getDefaultSpriteAngle() );

    // Copy sprites.   
    for( typeSpriteBatchHash::const_iterator spriteItr = mSprites.begin(); spriteItr != mSprites.end(); ++spriteItr )
    {        
        // Create sprite batch item.        
        SpriteBatchItem* pSpriteBatchItem = pSpriteBatch->createSprite();

        // Push a copy to it.
        spriteItr->value->copyTo( pSpriteBatchItem );
    }
}

//------------------------------------------------------------------------------

U32 SpriteBatch::addSprite( const LogicalPosition& logicalPosition )
{
    // Create sprite layout.
    mSelectedSprite = createSprite( logicalPosition );

    // Finish if no sprite created.
    if ( mSelectedSprite == NULL )
        return 0;

    // Insert into look-up.
    mSpriteLookup.insert( mSelectedSprite->getKey(), mSelectedSprite->getBatchId() );

    return mSelectedSprite->getBatchId();;
}

//------------------------------------------------------------------------------

bool SpriteBatch::removeSprite( void )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return false;

    // Remove the sprite.
    mSpriteLookup.erase( mSelectedSprite->getKey() );

    // Destroy the sprite.
    destroySprite( mSelectedSprite->getBatchId() );

    // Reset the selected sprite.
    mSelectedSprite = NULL;

    return true;
}

//------------------------------------------------------------------------------

void SpriteBatch::clearSprites( void )
{
    // Deselect any sprite.
    deselectSprite();

    // Clear sprite look-up.
    mSpriteLookup.clear();

    // Cache all sprites.
    for( typeSpriteBatchHash::iterator spriteItr = mSprites.begin(); spriteItr != mSprites.end(); ++spriteItr )
    {
        SpriteBatchItemFactory.cacheObject( spriteItr->value );
    }
    mSprites.clear();
    mMasterBatchId = 0;
}

//------------------------------------------------------------------------------

bool SpriteBatch::selectSprite( const LogicalPosition& logicalPosition )
{
    // Fetch sprite key.
    const StringTableEntry spriteKey = getSpriteKey( logicalPosition );

    // Select sprite.
    mSelectedSprite = findSpriteKey( spriteKey );

    // Finish if we selected the sprite.
    if ( mSelectedSprite != NULL )
        return true;

    // Not selected so warn.
    Con::warnf( "Cannot select sprite at logical position '%s' as one does not exist.", spriteKey );

    return false;
}

//------------------------------------------------------------------------------

bool SpriteBatch::selectSpriteId( const U32 batchId )
{
    // Select sprite.
    mSelectedSprite = findSpriteId( batchId );

    // Finish if we selected the sprite.
    if ( mSelectedSprite != NULL )
        return true;

    // Not selected so warn.
    Con::warnf( "Cannot select sprite Id '%d' as it does not exist.", batchId );

    return false;
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteImage( const char* pAssetId, const U32 imageFrame )
{
    // Sanity!
    AssertFatal( pAssetId, "Cannot set sprite image using a NULL asset Id." );

    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set image and frame.
    mSelectedSprite->setImage( pAssetId, imageFrame );
}

//------------------------------------------------------------------------------

StringTableEntry SpriteBatch::getSpriteImage( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return StringTable->EmptyString;

    // Get sprite image.
    return mSelectedSprite->getImage();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteImageFrame( const U32 imageFrame )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set image frame.
    mSelectedSprite->setImageFrame( imageFrame );
}

//------------------------------------------------------------------------------

U32 SpriteBatch::getSpriteImageFrame( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return 0;

    // Get image frame.
    return mSelectedSprite->getImageFrame();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteAnimation( const char* pAssetId, const bool autoRestore )
{
    // Sanity!
    AssertFatal( pAssetId, "Cannot set sprite animation using a NULL asset Id." );

    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set animation.
    mSelectedSprite->setAnimation( pAssetId, autoRestore );
}

//------------------------------------------------------------------------------

StringTableEntry SpriteBatch::getSpriteAnimation( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return StringTable->EmptyString;

    // Get animation.
    return mSelectedSprite->getAnimation();
}

//------------------------------------------------------------------------------

void SpriteBatch::clearSpriteAsset( void )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Clear the asset.
    mSelectedSprite->clearAsset();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteVisible( const bool visible )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set visibility.
    mSelectedSprite->setVisible( visible );
}

//------------------------------------------------------------------------------

bool SpriteBatch::getSpriteVisible( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return false;

    // Get visibility.
    return mSelectedSprite->getVisible();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteLocalPosition( const Vector2& localPosition )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set local position.
    mSelectedSprite->setLocalPosition( localPosition );
}

//------------------------------------------------------------------------------

Vector2 SpriteBatch::getSpriteLocalPosition( void )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return Vector2::getZero();

    // Get local position.
    return mSelectedSprite->getLocalPosition();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteAngle( const F32 localAngle )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set local angle.
    mSelectedSprite->setLocalAngle( localAngle );
}

//------------------------------------------------------------------------------

F32 SpriteBatch::getSpriteAngle( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return 0.0f;

    // Get local angle.
    return mSelectedSprite->getLocalAngle();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteDepth( const F32 depth )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set depth.
    mSelectedSprite->setDepth( depth );
}

//------------------------------------------------------------------------------

F32 SpriteBatch::getSpriteDepth( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return 0.0f;

    // Get depth.
    return mSelectedSprite->getDepth();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteSize( const Vector2& size )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set size.
    mSelectedSprite->setSize( size );
}

//------------------------------------------------------------------------------

Vector2 SpriteBatch::getSpriteSize( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return Vector2::getZero();

    // Get size.
    return mSelectedSprite->getSize();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteFlipX( const bool flipX )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set flip X.
    mSelectedSprite->setFlipX( flipX );
}

//------------------------------------------------------------------------------

bool SpriteBatch::getSpriteFlipX( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return false;

    // Get flip X.
    return mSelectedSprite->getFlipX();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteFlipY( const bool flipY )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set flip Y.
    mSelectedSprite->setFlipY( flipY );
}

//------------------------------------------------------------------------------

bool SpriteBatch::getSpriteFlipY( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return false;

    // Get flip Y.
    return mSelectedSprite->getFlipY();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteSortPoint( const Vector2& sortPoint )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set sort point.
    mSelectedSprite->setSortPoint( sortPoint );
}

//------------------------------------------------------------------------------

Vector2 SpriteBatch::getSpriteSortPoint( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return Vector2::getZero();

    // Get sort point.
    return mSelectedSprite->getSortPoint();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteBlendMode( const bool blendMode )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set blend mode.
    mSelectedSprite->setBlendMode( blendMode );
}

//------------------------------------------------------------------------------

bool SpriteBatch::getSpriteBlendMode( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return true;

    // Get blend mode.
    return mSelectedSprite->getBlendMode();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteSrcBlendFactor( GLenum srcBlendFactor )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set source blend factor.
    mSelectedSprite->setSrcBlendFactor( srcBlendFactor );
}

//------------------------------------------------------------------------------

GLenum SpriteBatch::getSpriteSrcBlendFactor( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return GL_SRC_ALPHA;

    // Get source blend factor.
    return mSelectedSprite->getSrcBlendFactor();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteDstBlendFactor( GLenum dstBlendFactor )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return ;

    // Set destination blend factor.
    mSelectedSprite->setDstBlendFactor( dstBlendFactor );
}

//------------------------------------------------------------------------------

GLenum SpriteBatch::getSpriteDstBlendFactor( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return GL_ONE_MINUS_SRC_ALPHA;

    // Get destination blend factor.
    return mSelectedSprite->getDstBlendFactor();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteBlendColor( const ColorF& blendColor )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set blend color.
    mSelectedSprite->setBlendColor( blendColor );
}

//------------------------------------------------------------------------------

ColorF SpriteBatch::getSpriteBlendColor( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return ColorF(1.0f, 1.0f, 1.0f);

    // Get blend color.
    return mSelectedSprite->getBlendColor();
}

//------------------------------------------------------------------------------

void SpriteBatch::setSpriteBlendAlpha( const F32 alpha )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set blend alpha.
    mSelectedSprite->setBlendAlpha( alpha );
}

//------------------------------------------------------------------------------

F32 SpriteBatch::getSpriteBlendAlpha( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return 0.0f;

    // Get blend alpha.
    return mSelectedSprite->getBlendAlpha();
}
   
//------------------------------------------------------------------------------

void SpriteBatch::setSpriteAlphaTest( const F32 alphaTestMode )
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return;

    // Set alpha-test mode.
    mSelectedSprite->setAlphaTest( alphaTestMode );
}

//------------------------------------------------------------------------------

F32 SpriteBatch::getSpriteAlphaTest( void ) const
{
    // Finish if a sprite is not selected.
    if ( !checkSpriteSelected() )
        return -1.0f;

    // Get alpha-test mode.
    return mSelectedSprite->getAlphaTest();
}

//------------------------------------------------------------------------------

void SpriteBatch::setBatchTransform( const b2Transform& worldTransform )
{
    // Update world transform.
    mWorldTransform = worldTransform;

    // Flag all sprite world transforms as dirty.
    for( typeSpriteBatchHash::iterator spriteItr = mSprites.begin(); spriteItr != mSprites.end(); ++spriteItr )
    {
        spriteItr->value->setWorldTransformDirty();
    }

    // Set the render AABB dirty.
    setRenderAABBDirty();
}

//------------------------------------------------------------------------------

SpriteBatchItem* SpriteBatch::createSprite( void )
{
    // Allocate batch Id.
    const U32 batchId = mMasterBatchId++;

    // Create sprite batch item,
    SpriteBatchItem* pSpriteBatchItem = SpriteBatchItemFactory.createObject();

    // Set batch parent.
    pSpriteBatchItem->setBatchParent( this, batchId );

    // Create sprite batch item,
    mSprites.insert( batchId, pSpriteBatchItem );

    return pSpriteBatchItem;
}

//------------------------------------------------------------------------------

SpriteBatchItem* SpriteBatch::findSpriteKey( const StringTableEntry key )
{
    // Find sprite.
    typeSpriteKeyHash::iterator spriteItr = mSpriteLookup.find( key );

    return spriteItr == mSpriteLookup.end() ? NULL : findSpriteId( spriteItr->value );
}

//------------------------------------------------------------------------------

SpriteBatchItem* SpriteBatch::findSpriteId( const U32 batchId )
{
    // Find sprite.
    typeSpriteBatchHash::iterator spriteItr = mSprites.find( batchId );

    return spriteItr != mSprites.end() ? spriteItr->value : NULL;
}

//------------------------------------------------------------------------------

void SpriteBatch::updateRenderAABB( void )
{
    // Finish if render AABB is not dirty.
    if ( !mRenderAABBDirty )
        return;

    // Do we have any sprites?
    if ( mSprites.size() == 0 )
    {
        // No, so reset render AABB.
        mRenderAABB.lowerBound.Set(-0.5f, -0.5f);
        mRenderAABB.upperBound.Set(0.5f, 0.5f);

        return;
    }
    else
    {
        // Fetch first sprite.
        typeSpriteBatchHash::iterator spriteItr = mSprites.begin();

        // Set render AABB to this sprite.
        mRenderAABB = spriteItr->value->getAABB();

        // Combine with the rest of the sprites.
        for( ; spriteItr != mSprites.end(); ++spriteItr )
        {
            mRenderAABB.Combine( spriteItr->value->getAABB() );
        }
    }

    // Flag as NOT dirty.
    mRenderAABBDirty = false;
}

//------------------------------------------------------------------------------

StringTableEntry SpriteBatch::getSpriteKey( const LogicalPosition& logicalPosition ) const
{
    // Do we have a valid logical position?
    if ( logicalPosition.mArgCount != 2 )
    {
        // No, so warn.
        Con::warnf( "Invalid logical position specified for composite sprite." );
        return 0;
    }

    return logicalPosition.getKey();
}

//------------------------------------------------------------------------------

SpriteBatchItem* SpriteBatch::createSprite( const LogicalPosition& logicalPosition )
{
    // Do we have a valid logical position?
    if ( logicalPosition.mArgCount != 2 )
    {
        // No, so warn.
        Con::warnf( "Invalid logical position specified for composite sprite." );
        return NULL;
    }

    // Fetch logical position.
    Vector2 position( dAtof(logicalPosition.mArgs[0]), dAtof(logicalPosition.mArgs[1]) );

    // Fetch sprite key.
    const StringTableEntry spriteKey = getSpriteKey( logicalPosition );

    // Does the sprite already exist?
    if ( findSpriteKey( spriteKey ) != NULL )
    {
        // Yes, so warn.
        Con::warnf( "Cannot add sprite at logical position '%s' as one already exists.", spriteKey );
        return NULL;
    }

    // Create the sprite.
    SpriteBatchItem* pSpriteBatchItem = createSprite();

    // Set sprite key.
    pSpriteBatchItem->setKey( spriteKey );

    // Set the sprite default position.
    pSpriteBatchItem->setLocalPosition( position.mult( getDefaultSpriteStride() ) );

    // Set the sprite default size and angle.
    pSpriteBatchItem->setSize( getDefaultSpriteSize() );
    pSpriteBatchItem->setLocalAngle( getDefaultSpriteAngle() );

    return pSpriteBatchItem;
}

//------------------------------------------------------------------------------

void SpriteBatch::onTamlCustomWrite( TamlCollectionProperty* pSpritesProperty )
{
    // Fetch property names.
    StringTableEntry spriteItemTypeName = StringTable->insert( "Sprite" );

    // Write all sprites.
    for( typeSpriteBatchHash::iterator spriteItr = mSprites.begin(); spriteItr != mSprites.end(); ++spriteItr )
    {
        // Add type alias.
        TamlPropertyTypeAlias* pSpriteTypeAlias = pSpritesProperty->addTypeAlias( spriteItemTypeName );
        
        // Write type with sprite item.
        spriteItr->value->onTamlCustomWrite( pSpriteTypeAlias );
    }
}

//------------------------------------------------------------------------------

void SpriteBatch::onTamlCustomRead( const TamlCollectionProperty* pSpritesProperty )
{
    // Fetch property names.
    StringTableEntry spriteItemTypeName = StringTable->insert( "Sprite" );

    // Iterate sprite item types.
    for( TamlCollectionProperty::const_iterator spriteTypeAliasItr = pSpritesProperty->begin(); spriteTypeAliasItr != pSpritesProperty->end(); ++spriteTypeAliasItr )
    {
        // Fetch sprite type alias.
        TamlPropertyTypeAlias* pSpriteTypeAlias = *spriteTypeAliasItr;

        // Fetch alias name.
        StringTableEntry aliasName = pSpriteTypeAlias->mAliasName;

        // Is this a known alias?
        if ( aliasName != spriteItemTypeName )
        {
            // No, so warn.
            Con::warnf( "SpriteBatch - Unknown custom type '%s'.", aliasName );
            continue;
        }

        // Create sprite.
        SpriteBatchItem* pSpriteBatchItem = createSprite();

        // Read type with sprite item.
        pSpriteBatchItem->onTamlCustomRead( pSpriteTypeAlias );

        // Fetch sprite key.
        const StringTableEntry spriteKey = pSpriteBatchItem->getKey();

        // Did we get a sprite key?
        if ( spriteKey != NULL )
        {
            // Yes, so insert into look-up
            mSpriteLookup.insert( pSpriteBatchItem->getKey(), pSpriteBatchItem->getBatchId() );
        }
    }
}

//------------------------------------------------------------------------------

bool SpriteBatch::destroySprite( const U32 batchId )
{
    // Find sprite.    
    SpriteBatchItem* pSpriteBatchItem = findSpriteId( batchId );

    // Finish if not found.
    if ( pSpriteBatchItem == NULL )
        return false;

    // Cache sprite.
    SpriteBatchItemFactory.cacheObject( pSpriteBatchItem );

    return true;
}

//------------------------------------------------------------------------------

bool SpriteBatch::checkSpriteSelected( void ) const
{
    // Finish if a sprite is selected.
    if ( mSelectedSprite != NULL )
        return true;

    // No, so warn,
    Con::warnf( "Cannot perform sprite operation no sprite is selected." );

    return false;
}
