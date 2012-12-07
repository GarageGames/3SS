//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_BASE_H_
#define _SPRITE_BASE_H_

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _SPRITE_PROXY_BASE_H_
#include "2d/core/SpriteProxyBase.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _ASSET_PTR_H_
#include "assets/assetPtr.h"
#endif

//------------------------------------------------------------------------------

class SpriteBase : public SceneObject, public SpriteProxyBase
{
    typedef SceneObject Parent;

public:
    SpriteBase();
    virtual ~SpriteBase();

    static void initPersistFields();

    virtual void integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats );
    virtual bool canRender( void ) const { return (isStaticMode() && mImageAsset.notNull()) || (!isStaticMode() && mAnimationAsset.notNull()); }

    virtual void copyTo(SimObject* object);

    /// Declare Console Object.
    DECLARE_CONOBJECT( SpriteBase );

protected:
    virtual void onAnimationEnd( void );

protected:
    static bool setImage(void* obj, const char* data) { DYNAMIC_VOID_CAST_TO(SpriteBase, SpriteProxyBase, obj)->setImage(data); return false; };
    static const char* getImage(void* obj, const char* data) { return DYNAMIC_VOID_CAST_TO(SpriteBase, SpriteProxyBase, obj)->getImage(); }
    static bool writeImage( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(SpriteBase); if ( !pCastObject->isStaticMode() ) return false; return pCastObject->mImageAsset.notNull(); }
    static bool setFrame(void* obj, const char* data) { DYNAMIC_VOID_CAST_TO(SpriteBase, SpriteProxyBase, obj)->setImageFrame(dAtoi(data)); return false; };
    static bool writeFrame( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(SpriteBase); if ( !pCastObject->isStaticMode() ) return false; return pCastObject->mImageAsset.notNull(); }
    static bool setAnimation(void* obj, const char* data) { DYNAMIC_VOID_CAST_TO(SpriteBase, SpriteProxyBase, obj)->setAnimation(data, false); return false; };
    static const char* getAnimation(void* obj, const char* data) { return DYNAMIC_VOID_CAST_TO(SpriteBase, SpriteProxyBase, obj)->getAnimation(); }
    static bool writeAnimation( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(SpriteBase); if ( pCastObject->isStaticMode() ) return false; return pCastObject->mAnimationAsset.notNull(); }
};

#endif // _SPRITE_BASE_H_
