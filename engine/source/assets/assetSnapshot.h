//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_SNAPSHOT_H
#define _ASSET_SNAPSHOT_H

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

//-----------------------------------------------------------------------------

class AssetSnapshot : public SimObject
{
private:
    typedef SimObject Parent;

public:
    AssetSnapshot() {}
    virtual ~AssetSnapshot() {}

    /// Reset asset snapshot.
    inline void resetSnapshot() { clearDynamicFields(); }

    /// Declare Console Object.
    DECLARE_CONOBJECT( AssetSnapshot );
};

#endif // _ASSET_SNAPSHOT_H

