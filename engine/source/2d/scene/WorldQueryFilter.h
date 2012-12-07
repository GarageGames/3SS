//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WORLD_QUERY_FILTER_H_
#define _WORLD_QUERY_FILTER_H_

#ifndef _UTILITY_H_
#include "2d/core/Utility.h"
#endif

///-----------------------------------------------------------------------------

struct WorldQueryFilter
{
    WorldQueryFilter()
        {
            resetQuery();
        }

    WorldQueryFilter(
        const U32 sceneLayerMask,
        const U32 sceneGroupMask,
        const bool enabledFilter,
        const bool visibleFilter,
        const bool pickingAllowedFilter,
        const bool alwaysInScopeFilter ) :        
        mSceneLayerMask( sceneLayerMask ),
        mSceneGroupMask( sceneGroupMask ),
        mEnabledFilter( enabledFilter ),
        mVisibleFilter( visibleFilter ),
        mPickingAllowedFilter( pickingAllowedFilter ),
        mAlwaysInScopeFilter( alwaysInScopeFilter )
        {
        }

    void resetQuery( void )
    {
        mSceneLayerMask       = MASK_ALL;
        mSceneGroupMask       = MASK_ALL;
        mEnabledFilter        = true;
        mVisibleFilter        = false;
        mPickingAllowedFilter = true;
        mAlwaysInScopeFilter  = false;
    }

    inline void     setEnabledFilter( const bool filter )           { mEnabledFilter = filter; }
    inline bool     getEnabledFilter( void ) const                  { return mEnabledFilter; }
    inline void     setVisibleFilter( const bool filter )           { mVisibleFilter = filter; }
    inline bool     getVisibleFilter( void ) const                  { return mVisibleFilter; }
    inline void     setPickingAllowedFilter( const bool filter )    { mPickingAllowedFilter = filter; }
    inline bool     getPickingAllowedFilter( void ) const           { return mPickingAllowedFilter; }
    inline void     setAlwaysInScopeFilter( const bool filter )     { mAlwaysInScopeFilter = filter; }
    inline bool     getAlwaysInScopeFilter( void ) const            { return mAlwaysInScopeFilter; }
    
    U32     mSceneLayerMask;
    U32     mSceneGroupMask;
    bool    mEnabledFilter;
    bool    mVisibleFilter;
    bool    mPickingAllowedFilter;
    bool    mAlwaysInScopeFilter;
};

#endif // _WORLD_QUERY_FILTER_H_