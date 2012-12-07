//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SPRITE_H_
#define _SPRITE_H_

#ifndef _SPRITE_BASE_H_
#include "2d/core/SpriteBase.h"
#endif

//------------------------------------------------------------------------------

class Sprite : public SpriteBase
{
    typedef SpriteBase Parent;

public:
    Sprite();
    virtual ~Sprite();

    static void initPersistFields();

    virtual void sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer );

    /// Declare Console Object.
    DECLARE_CONOBJECT( Sprite );
};

#endif // _SPRITE_H_
