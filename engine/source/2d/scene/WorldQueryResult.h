//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WORLD_QUERY_RESULT_H_
#define _WORLD_QUERY_RESULT_H_

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

///-----------------------------------------------------------------------------

class SceneObject;

///-----------------------------------------------------------------------------

struct WorldQueryResult
{
    WorldQueryResult() :
        mpSceneObject( NULL ),
        mShapeIndex( 0 ),
        mPoint( 0.0f, 0.0f ),
        mNormal( 0.0f, 0.0f ),
        mFraction( 0.0f )
    {
    }

    /// Initialize a non-ray-cast result.
    WorldQueryResult( SceneObject* pSceneObject ) :
        mpSceneObject( pSceneObject ),
        mShapeIndex( 0 ),
        mPoint( 0.0f, 0.0f ),
        mNormal( 0.0f, 0.0f ),
        mFraction( 0.0f )
    {
    }

    /// Initialize a ray-cast result.    
    WorldQueryResult( SceneObject* pSceneObject, const b2Vec2& point, const b2Vec2& normal, const F32 fraction, const U32 shapeIndex ) :
        mpSceneObject( pSceneObject ),
        mShapeIndex( shapeIndex ),
        mPoint( point ),
        mNormal( normal ),
        mFraction( fraction )
    {
    }

    b2Vec2          mPoint;
    b2Vec2          mNormal;
    F32             mFraction;
    SceneObject*    mpSceneObject;
    U32             mShapeIndex;
};

///-----------------------------------------------------------------------------

typedef Vector<WorldQueryResult> typeWorldQueryResultVector;

#endif // _WORLD_QUERY_RESULT_H_