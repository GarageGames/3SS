//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_RENDER_STATE_H_
#define _SCENE_RENDER_STATE_H_

//-----------------------------------------------------------------------------

class RectF;
class DebugStats;
struct b2AABB;

//-----------------------------------------------------------------------------

struct SceneRenderState
{
    SceneRenderState(
        const RectF& renderArea,
        U32 renderLayerMask,
        U32 renderGroupMask,
        const Vector2& renderScale,
        DebugStats* pDebugStats )
    {
        mRenderArea       = renderArea;
        mRenderAABB       = CoreMath::mRectFtoAABB( renderArea );
        mRenderScale      = renderScale;
        mRenderLayerMask  = renderLayerMask;
        mRenderGroupMask  = renderGroupMask;
        mpDebugStats      = pDebugStats;
    }

    RectF           mRenderArea;
    b2AABB          mRenderAABB;
    U32             mRenderLayerMask;
    U32             mRenderGroupMask;
    Vector2         mRenderScale;
    DebugStats*     mpDebugStats;

};

#endif // _SCENE_RENDER_STATE_H_
