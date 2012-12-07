//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_QUERY_H
#define _ASSET_QUERY_H

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

#define ASSETQUERY_COLLECTION_NAME      "Results"
#define ASSETQUERY_TYPE_NAME            "Asset"
#define ASSETQUERY_ASSETID_FIELD_NAME   "AssetId"

//-----------------------------------------------------------------------------

class AssetQuery : public SimObject, public Vector<StringTableEntry>
{
private:
    typedef SimObject Parent;

protected:
    virtual void onTamlCustomWrite( TamlCollection& customCollection );
    virtual void onTamlCustomRead( const TamlCollection& customCollection );

    static const char* getCount(void* obj, const char* data) { return Con::getIntArg(static_cast<AssetQuery*>(obj)->size()); }
    static bool writeCount( void* obj, StringTableEntry pFieldName ) { return false; }

public:
    AssetQuery() {}
    virtual ~AssetQuery() {}

    /// SimObject overrides
    static void initPersistFields();

    /// Whether asset is contained or not.
    inline bool containsAsset( StringTableEntry assetId )
    {
        for( Vector<StringTableEntry>::const_iterator assetItr = begin(); assetItr != end(); ++assetItr )
        {
            if ( *assetItr == assetId )
                return true;
        }
        return false;
    }

    /// Set assets.
    inline void set( const Vector<StringTableEntry>& assetQuery ) { *((Vector<StringTableEntry>*)(this)) = assetQuery; }

    /// Declare Console Object.
    DECLARE_CONOBJECT( AssetQuery );
};

#endif // _ASSET_QUERY_H

