//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _DEBUG_DRAW_H_
#define _DEBUG_DRAW_H_

#ifndef BOX2D_H
#include "box2d/Box2D.h"
#endif

#ifndef _MMATH_H_
#include "math/mMath.h"
#endif

//-----------------------------------------------------------------------------

class DebugDraw
{
public:
    DebugDraw() {}
    virtual ~DebugDraw() {}

    void DrawAABB( const b2AABB& aabb );
    void DrawOOBB( const b2Vec2* pOOBB );
    void DrawAsleep( const b2Vec2* pOOBB );
    void DrawCollisionShapes( b2Body* pBody );
    void DrawSortPoint( const b2Vec2& worldPosition, const b2Vec2& size, const b2Vec2& localSortPoint );

    void DrawJoints( b2World* pWorld );

    void DrawShape( b2Fixture* fixture, const b2Transform& xf, const b2Color& color );
    void DrawPolygon( const b2Vec2* vertices, int32 vertexCount, const b2Color& color);
    void DrawSolidPolygon( const b2Vec2* vertices, int32 vertexCount, const b2Color& color);
    void DrawCircle( const b2Vec2& center, float32 radius, const b2Color& color);
    void DrawSolidCircle( const b2Vec2& center, float32 radius, const b2Vec2& axis, const b2Color& color);
    void DrawSegment( const b2Vec2& p1, const b2Vec2& p2, const b2Color& color);
    void DrawTransform(const b2Transform& xf);
    void DrawPoint(const b2Vec2& p, float32 size, const b2Color& color);
};

#endif // _DEBUG_DRAW_H_
