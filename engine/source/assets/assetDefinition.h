//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_DEFINITION_H
#define _ASSET_DEFINITION_H

#ifndef _STRINGTABLE_H_
#include "string/stringTable.h"
#endif

#ifndef _STRINGUNIT_H_
#include "string/stringUnit.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

//-----------------------------------------------------------------------------

class AssetBase;
class ModuleDefinition;

//-----------------------------------------------------------------------------

struct AssetDefinition
{
public:
    AssetDefinition() { reset(); }
    virtual ~AssetDefinition() {}

    virtual void reset( void )
    {
        mAssetLoading = false;
        mpModuleDefinition = NULL;
        mpAssetBase = NULL;
        mAssetBaseFilePath = StringTable->EmptyString;
        mAssetId = StringTable->EmptyString;
        mAssetLoadedCount = 0;
        mAssetUnloadedCount = 0;
        mAssetRefreshEnable = true;
        mAssetLooseFiles.clear();

        // Reset persisted state.
        mAssetName = StringTable->EmptyString;
        mAssetDescription = StringTable->EmptyString;
        mAssetAutoUnload = true;
        mAssetInternal = false;
        mAssetPrivate = false;
        mAssetType = StringTable->EmptyString;
        mAssetCategory = StringTable->EmptyString;
    }

    ModuleDefinition*           mpModuleDefinition;
    SimObjectPtr<AssetBase>     mpAssetBase;
    StringTableEntry            mAssetBaseFilePath;
    StringTableEntry            mAssetId;
    U32                         mAssetLoadedCount;
    U32                         mAssetUnloadedCount;
    bool                        mAssetRefreshEnable;
    Vector<StringTableEntry>    mAssetLooseFiles;

    /// Persisted state.
    StringTableEntry            mAssetName;
    StringTableEntry            mAssetDescription;
    bool                        mAssetAutoUnload;
    bool                        mAssetInternal; 
    bool                        mAssetPrivate;
    bool                        mAssetLoading;
    StringTableEntry            mAssetType;
    StringTableEntry            mAssetCategory;
};

#endif // _ASSET_DEFINITION_H

