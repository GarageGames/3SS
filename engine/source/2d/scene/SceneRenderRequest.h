//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_RENDER_REQUEST_H_
#define _SCENE_RENDER_REQUEST_H_

#ifndef _SCENE_RENDER_FACTORIES_H_
#include "2d/scene/SceneRenderFactories.h"
#endif

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

#ifndef _COLOR_H_
#include "graphics/color.h"
#endif

//-----------------------------------------------------------------------------

class SceneRenderObject;
class SceneRenderQueue;

//-----------------------------------------------------------------------------

class SceneRenderRequest : public IFactoryObjectReset
{
public:
    SceneRenderRequest() : mpIsolatedRenderQueue(NULL)
    {
        resetState();
    }

    ~SceneRenderRequest() {}

    /// Sets mandatory configuration.
    inline SceneRenderRequest* set(
        SceneRenderObject* pSceneRenderObject,
        const Vector2& worldPosition,
        const F32 depth,
        const Vector2& sortPoint = Vector2::getZero(),
        const S32 serialId = 0,
        void* pCustomData1 = NULL,
        void* pCustomData2 = NULL,
        S32 customDataKey1 = 0,
        S32 customDataKey2 = 0)
    {
        // Sanity!
        AssertFatal( pSceneRenderObject != NULL, "Cannot submit a NULL scene render object." );

        mpSceneRenderObject = pSceneRenderObject;
        mWorldPosition = worldPosition;
        mDepth = depth;
        mSortPoint = sortPoint;
        mSerialId = serialId;
        mpCustomData1 = pCustomData1;
        mpCustomData2 = pCustomData2;
        mCustomDataKey1 = customDataKey1;
        mCustomDataKey2 = customDataKey2;

        return this;
    }

    /// Reset request state.
    virtual void resetState( void )
    {
        mpSceneRenderObject = NULL;
        mWorldPosition.SetZero();
        mDepth = 0.0f;
        mSortPoint.SetZero();
        mSerialId = 0;

        mBlendMode = true;
        mSrcBlendFactor = GL_SRC_ALPHA;
        mDstBlendFactor = GL_ONE_MINUS_SRC_ALPHA;
        mBlendColor = ColorF(1.0f,1.0f,1.0f,1.0f);
        mAlphaTest = -1.0f;

        mpCustomData1 = NULL;
        mpCustomData2 = NULL;
        mCustomDataKey1 = 0;
        mCustomDataKey2 = 0;

        if ( mpIsolatedRenderQueue != NULL )
        {
            SceneRenderQueueFactory.cacheObject( mpIsolatedRenderQueue );
            mpIsolatedRenderQueue = NULL;
        }
    }

public:
    SceneRenderObject*  mpSceneRenderObject;
    Vector2             mWorldPosition;
    F32                 mDepth;
    Vector2             mSortPoint;
    S32                 mSerialId;

    bool                mBlendMode;
    GLenum              mSrcBlendFactor;
    GLenum              mDstBlendFactor;
    ColorF              mBlendColor;
    F32                 mAlphaTest;

    void*               mpCustomData1;
    void*               mpCustomData2;
    S32                 mCustomDataKey1;
    S32                 mCustomDataKey2;

    SceneRenderQueue*   mpIsolatedRenderQueue;
};

#endif // _SCENE_RENDER_REQUEST_H_
