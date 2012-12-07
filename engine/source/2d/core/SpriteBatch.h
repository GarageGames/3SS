//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_BATCH_H_
#define _SPRITE_BATCH_H_

#ifndef _SPRITE_BATCH_ITEM_H_
#include "2d/core/SpriteBatchItem.h"
#endif

#ifndef _SCENE_RENDER_OBJECT_H_
#include "2d/scene/SceneRenderObject.h"
#endif

#ifndef _MD5_H_
#include "algorithm/md5.h"
#endif

//------------------------------------------------------------------------------  

class SpriteBatch : public b2DynamicTree
{
    friend class b2DynamicTree;

public:
    struct LogicalPosition
    {
        LogicalPosition( const S32 argCount, const char* args[] )
        {
            mArgCount = argCount;
            mArgs = args;
        }

        inline StringTableEntry getKey( void ) const
        {
            // Format key buffer.
            char keyBuffer[4096];
            char* pKeyBuffer = keyBuffer;
            S32 bufferLength = sizeof(keyBuffer);
            for( S32 index = 0; index < mArgCount; ++index )
            {
                const S32 offset = dSprintf(pKeyBuffer, bufferLength, "%s ", mArgs[index] );
                pKeyBuffer += offset;
                bufferLength -= offset;
            }

            // Hash the sprite key.
            MD5 md5;
            return StringTable->insert( md5.digestString( keyBuffer ) );
        }

        S32                 mArgCount;
        const char**        mArgs;
    };

protected:
    typedef HashMap< U32, SpriteBatchItem* > typeSpriteBatchHash;
    typedef HashMap< StringTableEntry, U32 > typeSpriteKeyHash;
    typedef Vector< SpriteBatchItem* > typeSpriteItemVector;

    typeSpriteBatchHash             mSprites;
    typeSpriteKeyHash               mSpriteLookup;
    SpriteBatchItem*                mSelectedSprite;
    SceneRenderQueue::RenderSort    mSortMode;
    Vector2                         mDefaultSpriteStride;
    Vector2                         mDefaultSpriteSize;
    F32                             mDefaultSpriteAngle;

private:
    U32                             mMasterBatchId;
    b2Transform                     mWorldTransform;
    typeSpriteItemVector            mRenderQuery;
    bool                            mRenderAABBDirty;
    b2AABB                          mRenderAABB;

public:
    SpriteBatch();
    virtual ~SpriteBatch();

    void prepareRender( SceneRenderObject* pSceneRenderObject, const SceneRenderState* pSceneRenderState, SceneRenderQueue* pSceneRenderQueue );
    void render( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer );

    inline void setRenderAABBDirty( void ) { mRenderAABBDirty = true; }
    inline bool getRenderAABBDirty( void ) const { return mRenderAABBDirty; }
    const b2AABB& getRenderAABB( void ) { if ( mRenderAABBDirty ) updateRenderAABB(); return mRenderAABB; }

    const b2Transform& getBatchTransform( void ) const { return mWorldTransform; }
    virtual void copyTo( SpriteBatch* pSpriteBatch ) const;

    inline U32 getSpriteCount( void ) { return (U32)mSprites.size(); }

    U32 addSprite( const LogicalPosition& logicalPosition );
    bool removeSprite( void );
    virtual void clearSprites( void );

    inline void setSortMode( SceneRenderQueue::RenderSort sortMode ) { mSortMode = sortMode; }
    inline SceneRenderQueue::RenderSort getSortMode( void ) const { return mSortMode; }

    inline void setDefaultSpriteStride( const Vector2& defaultStride ) { mDefaultSpriteStride = defaultStride; }
    inline Vector2 getDefaultSpriteStride( void ) const { return mDefaultSpriteStride; }

    inline void setDefaultSpriteSize( const Vector2& defaultSize ) { mDefaultSpriteSize = defaultSize; }
    inline Vector2 getDefaultSpriteSize( void ) const { return mDefaultSpriteSize; }

    inline void setDefaultSpriteAngle( const F32 defaultAngle ) { mDefaultSpriteAngle = defaultAngle; }
    inline F32 getDefaultSpriteAngle( void ) const { return mDefaultSpriteAngle; }

    bool selectSprite( const LogicalPosition& logicalPosition );
    bool selectSpriteId( const U32 batchId );
    inline void deselectSprite( void ) { mSelectedSprite = NULL; }
    bool isSpriteSelected( void ) const { return mSelectedSprite != NULL; }

    void setSpriteImage( const char* pAssetId, const U32 imageFrame = 0 );
    StringTableEntry getSpriteImage( void ) const;
    void setSpriteImageFrame( const U32 imageFrame );
    U32 getSpriteImageFrame( void ) const;
    void setSpriteAnimation( const char* pAssetId, const bool autoRestore = false );
    StringTableEntry getSpriteAnimation( void ) const;
    void clearSpriteAsset( void );

    void setSpriteVisible( const bool visible );
    bool getSpriteVisible( void ) const;

    void setSpriteLocalPosition( const Vector2& localPosition );
    Vector2 getSpriteLocalPosition( void );

    void setSpriteAngle( const F32 localAngle );
    F32 getSpriteAngle( void ) const;

    void setSpriteDepth( const F32 depth );
    F32 getSpriteDepth( void ) const;

    void setSpriteSize( const Vector2& size );
    Vector2 getSpriteSize( void ) const;

    void setSpriteFlipX( const bool flipX );
    bool getSpriteFlipX( void ) const;

    void setSpriteFlipY( const bool flipY );
    bool getSpriteFlipY( void ) const;

    void setSpriteSortPoint( const Vector2& sortPoint );
    Vector2 getSpriteSortPoint( void ) const;

    void setSpriteBlendMode( const bool blendMode );
    bool getSpriteBlendMode( void ) const;
    void setSpriteSrcBlendFactor( GLenum srcBlendFactor );
    GLenum getSpriteSrcBlendFactor( void ) const;
    void setSpriteDstBlendFactor( GLenum dstBlendFactor );
    GLenum getSpriteDstBlendFactor( void ) const;
    void setSpriteBlendColor( const ColorF& blendColor );
    ColorF getSpriteBlendColor( void ) const;
    void setSpriteBlendAlpha( const F32 alpha );
    F32  getSpriteBlendAlpha( void ) const;
    
    void setSpriteAlphaTest( const F32 alphaTestMode );
    F32 getSpriteAlphaTest( void ) const;

    virtual StringTableEntry getSpriteKey( const LogicalPosition& logicalPosition ) const;

protected:
    void setBatchTransform( const b2Transform& worldTransform );

    SpriteBatchItem* createSprite( void );

    SpriteBatchItem* findSpriteKey( const StringTableEntry key );
    SpriteBatchItem* findSpriteId( const U32 batchId );

    void updateRenderAABB( void );

    virtual SpriteBatchItem* createSprite( const LogicalPosition& logicalPosition );

    void onTamlCustomWrite( TamlCollectionProperty* pSpritesProperty );
    void onTamlCustomRead( const TamlCollectionProperty* pSpritesProperty );

private:
    bool destroySprite( const U32 batchId );
    bool checkSpriteSelected( void ) const;

    // Render query.
    inline bool QueryCallback( S32 proxyId )
    {    
        // Fetch sprite batch item.    
        SpriteBatchItem* pSpriteBatchItem = static_cast<SpriteBatchItem*>( GetUserData( proxyId ) );

        // Ignore if not visible.
        if ( !pSpriteBatchItem->getVisible() )
            return true;

        // Use sprite batch item.
        mRenderQuery.push_back( pSpriteBatchItem );

        return true;
    }
};

#endif // _SPRITE_BATCH_H_
