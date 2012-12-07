//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_TAGS_MANIFEST_H
#define _ASSET_TAGS_MANIFEST_H

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

#ifndef _STRINGUNIT_H_
#include "string/stringUnit.h"
#endif

//-----------------------------------------------------------------------------

#define ASSETTAGS_TAGS_COLLECTION_NAME      "Tags"
#define ASSETTAGS_TAGS_TYPE_NAME            "Tag"
#define ASSETTAGS_TAGS_NAME_FIELD           "Name"

#define ASSETTAGS_ASSETS_COLLECTION_NAME    "Assets"
#define ASSETTAGS_ASSETS_TYPE_NAME          "Tag"
#define ASSETTAGS_ASSETS_ASSETID_FIELD      "AssetId"
#define ASSETTAGS_ASSETS_TAG_FIELD          "Name"

//-----------------------------------------------------------------------------

class AssetManager;

//-----------------------------------------------------------------------------

class AssetTagsManifest : public SimObject
{
    friend class AssetManager;

private:
    typedef SimObject Parent;
    typedef StringTableEntry typeAssetId;
    typedef StringTableEntry typeAssetTagName;

public:
    /// Asset location.
    class AssetTag
    {
    public:
        AssetTag( StringTableEntry tagName )
        {
            // Sanity!
            AssertFatal( tagName != NULL, "Cannot use a NULL tag name." );

            // Case sensitive tag name.
            mTagName = tagName;
        }

        bool containsAsset( typeAssetId assetId )
        {
            for ( Vector<typeAssetId>::iterator assetIdItr = mAssets.begin(); assetIdItr != mAssets.end(); ++assetIdItr )
            {
                if ( *assetIdItr == assetId )
                    return true;
            }

            return false;
        }

        void removeAsset( typeAssetId assetId )
        {
            for ( Vector<typeAssetId>::iterator assetIdItr = mAssets.begin(); assetIdItr != mAssets.end(); ++assetIdItr )
            {
                if ( *assetIdItr == assetId )
                {
                    mAssets.erase( assetIdItr );
                    return;
                }
            }
        }

        typeAssetTagName mTagName;
        Vector<typeAssetId> mAssets;
    };

    /// Asset/Tag database.
    typedef HashMap<typeAssetTagName, AssetTag*> typeTagNameHash;
    typedef HashTable<typeAssetId, AssetTag*> typeAssetToTagHash;

private:
    typeTagNameHash mTagNameDatabase;
    typeAssetToTagHash mAssetToTagDatabase;

private:
    StringTableEntry fetchTagName( const char* pTagName );
    AssetTag* findAssetTag( const char* pTagName );
    void renameAssetId( const char* pAssetIdFrom, const char* pAssetIdTo );

protected:
    virtual void onTamlCustomWrite( TamlCollection& customCollection );
    virtual void onTamlCustomRead( const TamlCollection& customCollection );

public:
    AssetTagsManifest();
    virtual ~AssetTagsManifest();

    /// Tagging.
    const AssetTag* createTag( const char* pTagName );
    bool renameTag( const char* pOldTagName, const char* pNewTagName );
    bool deleteTag( const char* pTagName );
    bool isTag( const char* pTagName );
    inline U32 getTagCount( void ) const { return (U32)mTagNameDatabase.size(); }
    StringTableEntry getTag( const U32 tagIndex );
    U32 getAssetTagCount( const char* pAssetId );
    StringTableEntry getAssetTag( const char* pAssetId, const U32 tagIndex );
    bool tag( const char* pAssetId, const char* pTagName );
    bool untag( const char* pAssetId, const char* pTagName );
    bool hasTag( const char* pAssetId, const char* pTagName );

    /// Declare Console Object.
    DECLARE_CONOBJECT( AssetTagsManifest );
};

#endif // _ASSET_TAGS_MANIFEST_H

