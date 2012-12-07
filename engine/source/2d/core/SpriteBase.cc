//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_BASE_H_
#include "SpriteBase.h"
#endif

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#ifndef _STRINGBUFFER_H_
#include "string/stringBuffer.h"
#endif

// Script bindings.
#include "SpriteBase_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(SpriteBase);

//------------------------------------------------------------------------------

SpriteBase::SpriteBase()
{

}

//------------------------------------------------------------------------------

SpriteBase::~SpriteBase()
{
}

//------------------------------------------------------------------------------

void SpriteBase::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    addProtectedField("ImageMap", TypeImageMapAssetPtr, Offset(mImageAsset, SpriteBase), &setImage, &getImage, &writeImage, "");
    addProtectedField("Frame", TypeS32, Offset(mImageFrame, SpriteBase), &setFrame, &defaultProtectedGetFn, &writeFrame, "");
    addProtectedField("Animation", TypeAnimationAssetPtr, Offset(mAnimationAsset, SpriteBase), &setAnimation, &getAnimation, &writeAnimation, "");
}

//-----------------------------------------------------------------------------

void SpriteBase::integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats )
{
    // Call Parent.
    Parent::integrateObject( totalTime, elapsedTime, pDebugStats );

    // Update render proxy base.
    SpriteProxyBase::update( elapsedTime );
}

//------------------------------------------------------------------------------

void SpriteBase::copyTo(SimObject* object)
{
    // Call to parent.
    Parent::copyTo(object);

    // Cast to sprite.
    SpriteBase* pSpriteBase = static_cast<SpriteBase*>(object);

    // Sanity!
    AssertFatal(pSpriteBase != NULL, "SpriteBase::copyTo() - Object is not the correct type.");

    // Call render proxy base.
    SpriteProxyBase::copyTo( pSpriteBase );
}

//------------------------------------------------------------------------------

void SpriteBase::onAnimationEnd( void )
{
    // Do script callback.
    Con::executef( this, 1, "onAnimationEnd" );
}
