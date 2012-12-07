//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _BATCH_RENDER_H_
#define _BATCH_RENDER_H_

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

#ifndef _UTILITY_H_
#include "2d/core/Utility.h"
#endif

#ifndef _DEBUG_STATS_H_
#include "2d/scene/DebugStats.h"
#endif

#ifndef _TEXTURE_MANAGER_H_
#include "graphics/TextureManager.h"
#endif

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _COLOR_H_
#include "graphics/color.h"
#endif

//-----------------------------------------------------------------------------

#define BATCHRENDER_BUFFERSIZE      (65535)
#define BATCHRENDER_MAXQUADS        (BATCHRENDER_BUFFERSIZE/6)

//-----------------------------------------------------------------------------

class BatchRender
{
public:
    BatchRender();
    virtual ~BatchRender();

    /// Set the strict order mode.
    inline void setStrictOrderMode( const bool strictOrder, const bool forceFlush = false )
    {
        // Ignore if no change.
        if ( !forceFlush && strictOrder == mStrictOrderMode )
            return;

        // Flush.
        flushInternal();

        // Update strict order mode.
        mStrictOrderMode = strictOrder;
    }

    /// Gets the strict order mode.
    inline bool getStrictOrderMode( void ) const { return mStrictOrderMode; }

    /// Turns-on blend mode with the specified blend factors and color.
    inline void setBlendMode( GLenum srcFactor, GLenum dstFactor, const ColorF& blendColor = ColorF(1.0f, 1.0f, 1.0f, 1.0f))
    {
        // Ignore no change.
        if (    mBlendMode &&
                mSrcBlendFactor == srcFactor &&
                mDstBlendFactor == dstFactor &&
                mBlendColor == blendColor )
                return;

        // Flush.
        flush( mpDebugStats->batchBlendStateFlush );

        mBlendMode = true;
        mSrcBlendFactor = srcFactor;
        mDstBlendFactor = dstFactor;
        mBlendColor = blendColor;
    }

    /// Turns-off blend mode.
    inline void setBlendOff( void )
    {
        // Ignore no change,
        if ( !mBlendMode )
            return;

        // Flush.
        flush( mpDebugStats->batchBlendStateFlush );

        mBlendMode = false;
    }

    /// Set alpha-test mode.
    inline void setAlphaTestMode( const F32 alphaTestMode )
    {
        // Ignore no change.
        if ( mAlphaTestMode == alphaTestMode )
            return;

        // Flush.
        flush( mpDebugStats->batchAlphaStateFlush );

        // Stats.
        mpDebugStats->batchAlphaStateFlush++;

        mAlphaTestMode = alphaTestMode;
    }

    /// Sets the batch enabled mode.
    inline void setBatchEnabled( const bool enabled )
    {
        // Ignore no change.
        if ( mBatchEnabled == enabled )
            return;

        // Flush.
        flushInternal();

        mBatchEnabled = enabled;
    }

    /// Gets the batch enabled mode.
    inline bool getBatchEnabled( void ) const { return mBatchEnabled; }

    /// Sets the debug stats to use.
    inline void setDebugStats( DebugStats* pDebugStats ) { mpDebugStats = pDebugStats; }

    /// Submit a quad for batching.
    /// Vertex and textures are indexed as:
    ///  3 ___ 2
    ///   |\  |
    ///   | \ |
    ///  0| _\|1
    void SubmitQuad(
            const Vector2& vertexPos0,
            const Vector2& vertexPos1,
            const Vector2& vertexPos2,
            const Vector2& vertexPos3,
            const Vector2& texturePos0,
            const Vector2& texturePos1,
            const Vector2& texturePos2,
            const Vector2& texturePos3,
            TextureHandle& texture,
            const ColorF& color = ColorF(-1.0f, -1.0f, -1.0f) );

    /// Render a quad immediately without affecting current batch.
    /// All render state should be set beforehand directly.
    /// Vertex and textures are indexed as:
    ///  3 ___ 2
    ///   |\  |
    ///   | \ |
    ///  0| _\|1
    static void RenderQuad(
            const Vector2& vertexPos0,
            const Vector2& vertexPos1,
            const Vector2& vertexPos2,
            const Vector2& vertexPos3,
            const Vector2& texturePos0,
            const Vector2& texturePos1,
            const Vector2& texturePos2,
            const Vector2& texturePos3 );

    /// Flush (render) any pending batches with a reason metric.
    void flush( U32& reasonMetric );

    /// Flush (render) any pending batches.
    void flush( void );

private:
    /// Flush (render) any pending batches.
    void flushInternal( void );

private:
    typedef Vector<U32> indexVectorType;
    typedef HashMap<U32, indexVectorType*> textureBatchType;

    VectorPtr< indexVectorType* > mIndexVectorPool;
    textureBatchType    mTextureBatchMap;

    const ColorF        NoColor;

    Vector2             mVertexBuffer[ BATCHRENDER_BUFFERSIZE ];
    Vector2             mTextureBuffer[ BATCHRENDER_BUFFERSIZE ];
    U16                 mIndexBuffer[ BATCHRENDER_BUFFERSIZE ];
    ColorF              mColorBuffer[ BATCHRENDER_BUFFERSIZE ];
   
    U32                 mQuadCount;
    U32                 mVertexCount;
    U32                 mTextureResidentCount;
    U32                 mIndexCount;
    U32                 mColorCount;

    bool                mBlendMode;
    GLenum              mSrcBlendFactor;
    GLenum              mDstBlendFactor;
    ColorF              mBlendColor;
    F32                 mAlphaTestMode;

    bool                mStrictOrderMode;
    TextureHandle       mStrictOrderTextureHandle;
    DebugStats*         mpDebugStats;

    bool                mBatchEnabled;

};

#endif