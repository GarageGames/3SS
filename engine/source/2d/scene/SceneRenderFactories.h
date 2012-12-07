//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_RENDER_FACTORIES_H_
#define _SCENE_RENDER_FACTORIES_H_

#ifndef _FACTORY_CACHE_H_
#include "memory/factoryCache.h"
#endif

//-----------------------------------------------------------------------------

class SceneRenderRequest;
class SceneRenderQueue;

//-----------------------------------------------------------------------------

extern FactoryCache<SceneRenderRequest> SceneRenderRequestFactory;
extern FactoryCache<SceneRenderQueue> SceneRenderQueueFactory;

#endif // _SCENE_RENDER_FACTORIES_H_
