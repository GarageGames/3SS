//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _RENDER_PROXY_H_
#include "2d/core/RenderProxy.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _STRINGBUFFER_H_
#include "string/stringBuffer.h"
#endif

// Script bindings.
#include "RenderProxy_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(RenderProxy);

//------------------------------------------------------------------------------

RenderProxy::RenderProxy()
{
    // Require self ticking.
    mSelfTick = true;
}

//------------------------------------------------------------------------------

RenderProxy::~RenderProxy()
{
}

//------------------------------------------------------------------------------

void RenderProxy::initPersistFields()
{
   addProtectedField("ImageMap", TypeImageMapAssetPtr, Offset(mImageAsset, RenderProxy), &setImage, &getImage, &writeImage, "");
   addProtectedField("Frame", TypeS32, Offset(mImageFrame, RenderProxy), &setImageFrame, &defaultProtectedGetFn, &writeImageFrame, "");
   addProtectedField("Animation", TypeAnimationAssetPtr, Offset(mAnimationAsset, RenderProxy), &setAnimation, &getAnimation, &writeAnimation, "");

   Parent::initPersistFields();
}

//------------------------------------------------------------------------------

void RenderProxy::copyTo(SimObject* object)
{
    // Call to parent.
    Parent::copyTo(object);

    // Cast to sprite.
    RenderProxy* pRenderProxy = static_cast<RenderProxy*>(object);

    // Sanity!
    AssertFatal(pRenderProxy != NULL, "RenderProxy::copyTo() - Object is not the correct type.");

    // Call render proxy base.
    SpriteProxyBase::copyTo( pRenderProxy );
}
