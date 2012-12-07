//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_RENDER_OBJECT_H_
#define _SCENE_RENDER_OBJECT_H_

//-----------------------------------------------------------------------------

#ifndef _SCENE_RENDER_QUEUE_H_
#include "2d/scene/SceneRenderQueue.h"
#endif

#ifndef _SCENE_RENDER_STATE_H_
#include "2d/scene/SceneRenderState.h"
#endif

#ifndef _BATCH_RENDER_H_
#include "2d/core/BatchRender.h"
#endif

//-----------------------------------------------------------------------------

class SceneRenderObject
{
public:
    SceneRenderObject() {}
    virtual ~SceneRenderObject() {}

    virtual bool isBatchRendered( void ) = 0;

    virtual bool getBatchIsolated( void ) = 0;

    virtual bool canRender( void ) const = 0;

    virtual void scenePrepareRender(const SceneRenderState* pSceneRenderState, SceneRenderQueue* pSceneRenderQueue ) = 0;

    virtual void sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer ) = 0;

    virtual void sceneRenderFallback( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer ) = 0;
};

#endif // _SCENE_RENDER_OBJECT_H_
