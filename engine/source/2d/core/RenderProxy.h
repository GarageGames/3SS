//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _RENDER_PROXY_H_
#define _RENDER_PROXY_H_

#ifndef _BATCH_RENDER_H_
#include "BatchRender.h"
#endif

#ifndef _SPRITE_PROXY_BASE_H_
#include "2d/core/SpriteProxyBase.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

//------------------------------------------------------------------------------

class RenderProxy : public SimObject, public SpriteProxyBase
{
    typedef SimObject               Parent;

public:
    RenderProxy();
    virtual ~RenderProxy();

    static void initPersistFields();

    virtual void copyTo(SimObject* object);

    /// Declare Console Object.
    DECLARE_CONOBJECT( RenderProxy );

protected:
    static bool setImage(void* obj, const char* data) { DYNAMIC_VOID_CAST_TO(RenderProxy, SpriteProxyBase, obj)->setImage( data ); return false; };
    static const char* getImage(void* obj, const char* data) { return DYNAMIC_VOID_CAST_TO(RenderProxy, SpriteProxyBase, obj)->getImage(); }
    static bool writeImage( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(RenderProxy); if ( !pCastObject->isStaticMode() ) return false; return pCastObject->mImageAsset.notNull(); }
    static bool setImageFrame(void* obj, const char* data) { DYNAMIC_VOID_CAST_TO(RenderProxy, SpriteProxyBase, obj)->setImageFrame(dAtoi(data)); return false; };
    static bool writeImageFrame( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(RenderProxy); if ( !pCastObject->isStaticMode() ) return false; return pCastObject->mImageAsset.notNull(); }
    static bool setAnimation(void* obj, const char* data) { DYNAMIC_VOID_CAST_TO(RenderProxy, SpriteProxyBase, obj)->setAnimation(data, false); return false; };
    static const char* getAnimation(void* obj, const char* data) { return DYNAMIC_VOID_CAST_TO(RenderProxy, SpriteProxyBase, obj)->getAnimation(); }
    static bool writeAnimation( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(RenderProxy); if ( pCastObject->isStaticMode() ) return false; return pCastObject->mAnimationAsset.notNull(); }
};

#endif // _RENDER_PROXY_H_
