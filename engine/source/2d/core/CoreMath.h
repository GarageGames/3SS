//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CORE_MATH_H_
#define _CORE_MATH_H_

#ifndef _MRANDOM_H_
#include "math/mRandom.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef B2_COLLISION_H
#include "box2d/Collision/b2Collision.h"
#endif

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

#ifndef _PLATFORMGL_H_
#include "platform/platformGL.h"
#endif

#ifndef _MMATH_H_
#include "math/mMath.h"
#endif

//-----------------------------------------------------------------------------

struct Vector2;

namespace CoreMath
{
extern MRandomLCG gRandomGenerator;

/// Returns a point on the given line AB that is closest to 'point'.
Vector2 mGetClosestPointOnLine( Vector2 &a, Vector2 &b, Vector2 &point);

/// Convert RectF to AABB.
inline b2AABB mRectFtoAABB( const RectF& rect )
{
    b2AABB aabb;
    b2Vec2 lower(rect.point.x, rect.point.y);
    b2Vec2 upper(lower.x + rect.len_x(), lower.y + rect.len_y() );
    aabb.lowerBound = lower;
    aabb.upperBound = upper;
    return aabb;
}

/// Random Float Range.
inline F32 mGetRandomF( F32 from, F32 to ) { return gRandomGenerator.randF( from, to ); }

/// Random Float.
inline F32 mGetRandomF( void ) { return gRandomGenerator.randF(); }

/// Random Integer Range.
inline S32 mGetRandomI( U32 from, U32 to ) { return gRandomGenerator.randI( from, to ); }

/// Random Integer.
inline S32 mGetRandomI( void ) { return gRandomGenerator.randI(); }

// Geometric helpers.
void mCalculateAABB( const b2Vec2* const pAABBVertices, const b2Transform& xf, b2AABB* pAABB );
void mCalculateOOBB( const b2Vec2* const pAABBVertices, const b2Transform& xf, b2Vec2* pOOBBVertices );
bool mPointInRectangle( const Vector2& point, const Vector2& rectMin, const Vector2& rectMax );
bool mLineRectangleIntersect( const Vector2& startPoint, const Vector2& endPoint, const Vector2& rectMin, const Vector2& rectMax, F32* pTime = NULL );

} // Namespace CoreMath.

#endif // _CORE_UTILITY_H_
