//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_MANIFEST_H
#define _ASSET_MANIFEST_H

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

#ifndef _STRINGUNIT_H_
#include "string/stringUnit.h"
#endif

//-----------------------------------------------------------------------------

#define ASSET_COLLECTION_NAME      "Search"
#define ASSET_TYPE_NAME            "Location"
#define ASSET_PATH_FIELD_NAME      "Path"
#define ASSET_EXTENSION_FIELD_NAME "Extension"
#define ASSET_RECURSE_FIELD_NAME   "Recurse"

//-----------------------------------------------------------------------------

class AssetManager;

//-----------------------------------------------------------------------------

class AssetManifest : public SimObject
{
    friend class AssetManager;

private:
    typedef SimObject Parent;

    /// Asset location.
    struct AssetLocation
    {
        AssetLocation() :
            mPath( StringTable->EmptyString ),
            mExtension( StringTable->EmptyString ),
            mRecurse( false )
        {
        }                    

        AssetLocation( const char* pPath, const char* pExtension, const bool recurse )
        {
            mPath = StringTable->insert( pPath );
            mExtension = StringTable->insert( pExtension );
            mRecurse = recurse;
        }

        StringTableEntry    mPath;
        StringTableEntry    mExtension;
        bool                mRecurse;
    };

    /// Used assets.
    typedef Vector<AssetLocation> typeAssetLocationVector;
    typeAssetLocationVector mAssetLocations;

protected:
    virtual void onTamlCustomWrite( TamlCollection& customCollection );
    virtual void onTamlCustomRead( const TamlCollection& customCollection );

public:
    AssetManifest() {}
    virtual ~AssetManifest() {}

    inline const typeAssetLocationVector& getManifest( void ) const { return mAssetLocations; }

    /// Declare Console Object.
    DECLARE_CONOBJECT( AssetManifest );
};

#endif // _ASSET_MANIFEST_H

