//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WORLD_QUERY_H_
#define _WORLD_QUERY_H_

#ifndef _WORLD_QUERY_FILTER_H_
#include "2d/scene/WorldQueryFilter.h"
#endif

#ifndef _WORLD_QUERY_RESULT_H_
#include "2d/scene/WorldQueryResult.h"
#endif

///-----------------------------------------------------------------------------

class Scene;

///-----------------------------------------------------------------------------

class WorldQuery :
    protected b2DynamicTree,
    public b2QueryCallback,
    public b2RayCastCallback,
    public SimObject
{
public:
    WorldQuery( Scene* pScene );
    virtual         ~WorldQuery() {}

    /// Standard scope.
    S32             add( SceneObject* pSceneObject );
    void            remove( SceneObject* pSceneObject );
    bool            update( SceneObject* pSceneObject, const b2AABB& aabb, const b2Vec2& displacement );

    /// Always in scope.
    void            addAlwaysInScope( SceneObject* pSceneObject );
    void            removeAlwaysInScope( SceneObject* pSceneObject );

    //// World fixture queries.
    U32             fixtureQueryArea( const b2AABB& aabb );
    U32             fixtureQueryRay( const Vector2& point1, const Vector2& point2 );
    U32             fixtureQueryPoint( const Vector2& point );

    //// Render queries.
    U32             renderQueryArea( const b2AABB& aabb );
    U32             renderQueryRay( const Vector2& point1, const Vector2& point2 );
    U32             renderQueryPoint( const Vector2& point );

    /// World fixture & render queries.
    U32             anyQueryArea( const b2AABB& aabb );
    U32             anyQueryArea( const Vector2& lowerBound, const Vector2& upperBound );
    U32             anyQueryRay( const Vector2& point1, const Vector2& point2 );
    U32             anyQueryPoint( const Vector2& point );

    /// Filtering.
    inline void     setQueryFilter( const WorldQueryFilter& queryFilter ) { mQueryFilter = queryFilter; }
   
    /// Results.
    void            clearQuery( void );
    typeWorldQueryResultVector& getLayeredQueryResults( const U32 layer );
    typeWorldQueryResultVector& getQueryResults( void ) { return mQueryResults; }
    inline U32      getQueryResultsCount( void ) const { return mQueryResults.size(); }
    inline bool     getIsRaycastQueryResult( void ) const { return mIsRaycastQueryResult; }
    void            sortRaycastQueryResult( void );

    /// Callbacks.
    virtual bool    ReportFixture( b2Fixture* fixture );
    virtual F32     ReportFixture( b2Fixture* fixture, const b2Vec2& point, const b2Vec2& normal, F32 fraction );
    bool            QueryCallback( S32 proxyId );
    F32             RayCastCallback( const b2RayCastInput& input, S32 proxyId );

private:
    void            injectAlwaysInScope( void );
    static S32      QSORT_CALLBACK rayCastFractionSort(const void* a, const void* b);

private:
    Scene*                      mpScene;
    WorldQueryFilter            mQueryFilter;
    bool                        mCheckFixturePoint;
    b2Vec2                      mFixturePoint;
    typeWorldQueryResultVector  mLayeredQueryResults[MAX_LAYERS_SUPPORTED];
    typeWorldQueryResultVector  mQueryResults;
    bool                        mIsRaycastQueryResult;
    typeSceneObjectVector       mAlwaysInScopeSet;
    U32                         mMasterQueryKey;
};

#endif // _WORLD_QUERY_H_