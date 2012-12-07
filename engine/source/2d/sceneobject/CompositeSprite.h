//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _COMPOSITE_SPRITE_H_
#define _COMPOSITE_SPRITE_H_

#ifndef _SPRITE_BATCH_H_
#include "2d/core/SpriteBatch.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

//------------------------------------------------------------------------------  

class CompositeSprite : public SceneObject, public SpriteBatch
{
protected:
    typedef SceneObject Parent;

public:
    CompositeSprite();
    virtual ~CompositeSprite();

    static void initPersistFields();

    virtual void preIntegrate( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats );
    virtual void integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats );
    virtual void interpolateObject( const F32 timeDelta );

    virtual bool canPrepareRender( void ) const { return true; }
    
    virtual void scenePrepareRender( const SceneRenderState* pSceneRenderState, SceneRenderQueue* pSceneRenderQueue );
    
    virtual void sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer );

    virtual void copyTo( SimObject* object );

    /// Declare Console Object.
    DECLARE_CONOBJECT( CompositeSprite );

protected:
    virtual void onTamlCustomWrite( TamlCollection& customCollection );
    virtual void onTamlCustomRead( const TamlCollection& customCollection );

protected:
    static bool         setDefaultSpriteAngle(void* obj, const char* data)                  { STATIC_VOID_CAST_TO(CompositeSprite,SpriteBatch, obj)->setDefaultSpriteAngle(mDegToRad(dAtof(data))); return false; }
    static const char*  getDefaultSpriteAngle(void* obj, const char* data)                  { return Con::getFloatArg( mRadToDeg(STATIC_VOID_CAST_TO(CompositeSprite,SpriteBatch, obj)->getDefaultSpriteAngle()) ); }
    static bool         writeDefaultSpriteAngle( void* obj, StringTableEntry pFieldName )   { PREFAB_WRITE_CHECK(CompositeSprite); return mNotZero( static_cast<SpriteBatch*>(pCastObject)->getDefaultSpriteAngle() ); }
    static bool         writeBatchIsolated( void* obj, StringTableEntry pFieldName )        { PREFAB_WRITE_CHECK(CompositeSprite); return pCastObject->getBatchIsolated(); }
    static bool         writeBatchSortMode( void* obj, StringTableEntry pFieldName )        { PREFAB_WRITE_CHECK(CompositeSprite); return pCastObject->getSortMode() != SceneRenderQueue::RENDER_SORT_OFF; }
};

#endif // _COMPOSITE_SPRITE_H_
