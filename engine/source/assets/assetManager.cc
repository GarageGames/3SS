//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "assetManager.h"

#ifndef _ASSET_MANIFEST_H
#include "AssetManifest.h"
#endif

#ifndef _ASSET_PTR_H_
#include "assetPtr.h"
#endif

#ifndef _TAML_ASSET_REFERENCED_VISITOR_H_
#include "tamlAssetReferencedVisitor.h"
#endif

#ifndef _TAML_ASSET_DECLARED_VISITOR_H_
#include "tamlAssetDeclaredVisitor.h"
#endif

#ifndef _TAML_ASSET_DECLARED_UPDATE_VISITOR_H_
#include "tamlAssetDeclaredUpdateVisitor.h"
#endif

#ifndef _TAML_ASSET_REFERENCED_UPDATE_VISITOR_H_
#include "tamlAssetReferencedUpdateVisitor.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

// Script bindings.
#include "assetManager_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( AssetManager );

//-----------------------------------------------------------------------------

AssetManager AssetDatabase;

//-----------------------------------------------------------------------------

AssetManager::AssetManager() :
    mLoadedInternalAssetsCount( 0 ),
    mLoadedExternalAssetsCount( 0 ),
    mLoadedPrivateAssetsCount( 0 ),
    mMaxLoadedInternalAssetsCount( 0 ),
    mMaxLoadedExternalAssetsCount( 0 ),
    mMaxLoadedPrivateAssetsCount( 0 ),
    mEchoInfo( false ),
    mIgnoreAutoUnload( false )
{
}

//-----------------------------------------------------------------------------

bool AssetManager::onAdd()
{
    // Call parent.
    if ( !Parent::onAdd() )
        return false;

    return true;
}

//-----------------------------------------------------------------------------

void AssetManager::onRemove()
{
    // Do we have an asset tags manifest?
    if ( !mAssetTagsManifest.isNull() )
    {
        // Yes, so remove it.
        mAssetTagsManifest->deleteObject();
    }

    // Call parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

void AssetManager::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    addField( "EchoInfo", TypeBool, Offset(mEchoInfo, AssetManager), "Whether the asset manager echos extra information to the console or not." );
    addField( "IgnoreAutoUnload", TypeBool, Offset(mIgnoreAutoUnload, AssetManager), "Whether the asset manager should ignore unloading of auto-unload assets or not." );
}

//-----------------------------------------------------------------------------

bool AssetManager::compileReferencedAssets( ModuleDefinition* pModuleDefinition )
{
    // Sanity!
    AssertFatal( pModuleDefinition != NULL, "Cannot add declared assets using a NULL module definition" );

    // Expand asset manifest location.
    char assetManifestFilePathBuffer[1024];
    Con::expandPath( assetManifestFilePathBuffer, sizeof(assetManifestFilePathBuffer), pModuleDefinition->getReferencedAssetManifest() );

    // Clear referenced assets.
    mReferencedAssets.clear();

    // Read manifest.
    Taml taml;
    AssetManifest* pAssetManifest = taml.read<AssetManifest>( assetManifestFilePathBuffer );

    // Did we load the manifest?
    if ( pAssetManifest == NULL )
    {
        // No, so warn.
        Con::warnf( "AssetManager::compileReferencedAssets() - Could not load referenced asset manifest '%s'.", assetManifestFilePathBuffer );
        return false;
    }

    // Get manifest.
    const AssetManifest::typeAssetLocationVector& manifest = pAssetManifest->getManifest();

    // Iterate locations.
    for( AssetManifest::typeAssetLocationVector::const_iterator locationItr = manifest.begin(); locationItr != manifest.end(); ++locationItr )
    {
        // Scan referenced assets at location.
        if ( !scanReferencedAssets( locationItr->mPath, locationItr->mExtension ) )
        {
            // Warn.
            Con::warnf( "AssetManager::compileReferencedAssets() - Could not scan referenced assets at location '%s' with extension '%s'.", locationItr->mPath, locationItr->mExtension );
        }
    }

    // Delete asset manifest.
    pAssetManifest->deleteObject();

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::addDeclaredAssets( ModuleDefinition* pModuleDefinition )
{
    // Sanity!
    AssertFatal( pModuleDefinition != NULL, "Cannot add declared assets using a NULL module definition" );

    // Does the module have any assets associated with it?
    if ( pModuleDefinition->getModuleAssets().size() > 0 )
    {
        // Yes, so warn.
        Con::warnf( "Asset Manager: Cannot add declared assets to module '%s' as it already has existing assets.", pModuleDefinition->getSignature() );
        return false;
    }

    // Expand asset manifest location.
    char assetManifestFilePathBuffer[1024];
    Con::expandPath( assetManifestFilePathBuffer, sizeof(assetManifestFilePathBuffer), pModuleDefinition->getDeclaredAssetManifest() );

    // Read manifest.
    Taml taml;
    AssetManifest* pAssetManifest = taml.read<AssetManifest>( assetManifestFilePathBuffer );

    // Did we load the manifest?
    if ( pAssetManifest == NULL )
    {
        // No, so warn.
        Con::warnf( "AssetManager::addDeclaredAssets() - Could not load declared asset manifest '%s'.", assetManifestFilePathBuffer );
        return false;
    }

    // Get manifest.
    const AssetManifest::typeAssetLocationVector& manifest = pAssetManifest->getManifest();

    // Iterate locations.
    for( AssetManifest::typeAssetLocationVector::const_iterator locationItr = manifest.begin(); locationItr != manifest.end(); ++locationItr )
    {
        // Fetch asset location.
        const AssetManifest::AssetLocation& assetLocation = *locationItr;
        
        // Scan declared assets at location.
        if ( !scanDeclaredAssets( assetLocation.mPath, assetLocation.mExtension, assetLocation.mRecurse, pModuleDefinition ) )
        {
            // Warn.
            Con::warnf( "AssetManager::addDeclaredAssets() - Could not scan declared assets at location '%s' with extension '%s'", locationItr->mPath, locationItr->mExtension );
        }
    }

    // Delete asset manifest.
    pAssetManifest->deleteObject();    

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::addSingleDeclaredAsset( ModuleDefinition* pModuleDefinition, const char* pAssetFilePath )
{
    // Sanity!
    AssertFatal( pModuleDefinition != NULL, "Cannot add single declared asset using a NULL module definition" );
    AssertFatal( pAssetFilePath != NULL, "Cannot add single declared asset using a NULL asset file-path." );

    // Expand asset file-path.
    char assetFilePathBuffer[1024];
    Con::expandPath( assetFilePathBuffer, sizeof(assetFilePathBuffer), pAssetFilePath );

    // Find the final slash which should be just before the file.
    char* pFileStart = dStrrchr( assetFilePathBuffer, '/' );

    // Did we find the final slash?
    if ( pFileStart == NULL )
    {
        // No, so warn.
        Con::warnf( "AssetManager::addSingleDeclaredAsset() - Could not add single declared asset file '%s' as file-path '%s' is not valid.",
            assetFilePathBuffer,
            pModuleDefinition->getModulePath() );
        return false;
    }

    // Terminate path at slash.
    *pFileStart = 0;

    // Move to next character which should be the file start.
    pFileStart++;

    // Scan declared assets at location.
    if ( !scanDeclaredAssets( assetFilePathBuffer, pFileStart, false, pModuleDefinition ) )
    {
        // Warn.
        Con::warnf( "AssetManager::addSingleDeclaredAsset() - Could not scan declared assets at location '%s' with extension '%s'.", assetFilePathBuffer, pFileStart );
        return false;
    }

    return true;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::addPrivateAsset( AssetBase* pAssetBase )
{
    // Sanity!
    AssertFatal( pAssetBase != NULL, "Cannot add a NULL private asset." );

    // Is the asset already added?
    if ( pAssetBase->mpAssetDefinition->mAssetId != StringTable->EmptyString )
    {
        // Yes, so warn.
        Con::warnf( "Cannot add private asset '%d' as it has already been assigned.", pAssetBase->mpAssetDefinition->mAssetId );
        return StringTable->EmptyString;
    }

    static U32 masterPrivateAssetId = 1;

    // Create asset definition.
    AssetDefinition* pAssetDefinition = new AssetDefinition();

    // Fetch source asset definition.
    AssetDefinition* pSourceAssetDefinition = pAssetBase->mpAssetDefinition;

    // Configure asset.
    pAssetDefinition->mpAssetBase = pAssetBase;
    pAssetDefinition->mAssetDescription = pSourceAssetDefinition->mAssetDescription;
    pAssetDefinition->mAssetCategory = pSourceAssetDefinition->mAssetCategory;
    pAssetDefinition->mAssetAutoUnload = false;
    pAssetDefinition->mAssetRefreshEnable = false;
    pAssetDefinition->mAssetType = StringTable->insert( pAssetBase->getClassName() );
    pAssetDefinition->mAssetLoadedCount = 1;
    pAssetDefinition->mAssetInternal = pSourceAssetDefinition->mAssetInternal;
    pAssetDefinition->mAssetPrivate = true;

    // Format asset name.
    char assetNameBuffer[256];
    dSprintf(assetNameBuffer, sizeof(assetNameBuffer), "%s_%d", pAssetDefinition->mAssetType, masterPrivateAssetId++ );    

    // Set asset identity.
    pAssetDefinition->mAssetName = StringTable->insert( assetNameBuffer );
    pAssetDefinition->mAssetId = pAssetDefinition->mAssetName;

    // Ensure that the source asset is fully synchronized with the new asset definition.
    pSourceAssetDefinition->mAssetName = pAssetDefinition->mAssetName;
    pSourceAssetDefinition->mAssetAutoUnload = pAssetDefinition->mAssetAutoUnload;
    pSourceAssetDefinition->mAssetInternal = pAssetDefinition->mAssetInternal;

    // Set ownership by asset manager.
    pAssetDefinition->mpAssetBase->setOwned( this, pAssetDefinition );

    // Store in declared assets.
    mDeclaredAssets.insert( pAssetDefinition->mAssetId, pAssetDefinition );

    // Increase the private loaded asset count.
    if ( ++mLoadedPrivateAssetsCount > mMaxLoadedPrivateAssetsCount )
        mMaxLoadedPrivateAssetsCount = mLoadedPrivateAssetsCount;

    return pAssetDefinition->mAssetId;
}

//-----------------------------------------------------------------------------

bool AssetManager::removeDeclaredAssets( ModuleDefinition* pModuleDefinition )
{
    // Sanity!
    AssertFatal( pModuleDefinition != NULL, "Cannot remove declared assets using a NULL module definition" );

    // Fetch module assets.
    ModuleDefinition::typeModuleAssetsVector& moduleAssets = pModuleDefinition->getModuleAssets();

    // Remove all module assets.
    while ( moduleAssets.size() > 0 )
    {
        // Fetch asset definition.
        AssetDefinition* pAssetDefinition = *moduleAssets.begin();

        // Remove this asset.
        removeSingleDeclaredAsset( pAssetDefinition->mAssetId );
    }

    // Info.
    if ( mEchoInfo )
        Con::printSeparator();

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::removeSingleDeclaredAsset( const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot remove single declared asset using NULL asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Find declared asset.
    typeDeclaredAssetsHash::iterator declaredAssetItr = mDeclaredAssets.find( assetId );

    // Did we find the declared asset?
    if ( declaredAssetItr == mDeclaredAssets.end() )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot remove single asset Id '%s' it could not be found.", assetId );
        return false;
    }

    // Fetch asset definition.
    AssetDefinition* pAssetDefinition = declaredAssetItr->value;

    // Is the asset private?
    if ( !pAssetDefinition->mAssetPrivate )
    {
        // No, so fetch module assets.
        ModuleDefinition::typeModuleAssetsVector& moduleAssets = pAssetDefinition->mpModuleDefinition->getModuleAssets();

        // Remove module asset.
        for ( ModuleDefinition::typeModuleAssetsVector::iterator moduleAssetItr = moduleAssets.begin(); moduleAssetItr != moduleAssets.end(); ++moduleAssetItr )
        {
            if ( *moduleAssetItr == pAssetDefinition )
            {
                moduleAssets.erase( moduleAssetItr );
                break;
            }
        }

        // Remove asset dependencies.
        removeAssetDependencies( pAssetId );
    }

    // Do we have an asset loaded?
    if ( pAssetDefinition->mpAssetBase.notNull() )
    {
        // Yes, so delete it.
        // NOTE: If anything is using this then this'll cause a crash.  Objects should always use safe reference methods however.
        pAssetDefinition->mpAssetBase->deleteObject();
    }

    // Remove from declared assets.
    mDeclaredAssets.erase( declaredAssetItr );

    // Info.
    if ( mEchoInfo )
    {
        Con::printf( "Asset Manager: Removing Asset Id '%s' of type '%s' in asset file '%s'.",
            pAssetDefinition->mAssetId,
            pAssetDefinition->mAssetType,
            pAssetDefinition->mAssetBaseFilePath );
    }

    // Destroy asset definition.
    delete pAssetDefinition;

    return true;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::getAssetName( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return NULL;
    }

    return pAssetDefinition->mAssetName;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::getAssetDescription( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return NULL;
    }

    return pAssetDefinition->mAssetDescription;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::getAssetCategory( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return NULL;
    }

    return pAssetDefinition->mAssetCategory;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::getAssetType( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return NULL;
    }

    return pAssetDefinition->mAssetType;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::getAssetFilePath( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return NULL;
    }

    return pAssetDefinition->mAssetBaseFilePath;
}

//-----------------------------------------------------------------------------

StringTableEntry AssetManager::getAssetPath( const char* pAssetId )
{
    // Fetch asset file-path.
    StringTableEntry assetFilePath = getAssetFilePath( pAssetId );

    // Finish if no file-path.
    if ( assetFilePath == NULL )
        return NULL;

    // Find the final slash which should be just before the file.
    const char* pFinalSlash = dStrrchr( assetFilePath, '/' );

    // Sanity!
    AssertFatal( pFinalSlash != NULL, "Should always be able to find final slash in the asset file-path." );

    // Fetch asset path.
    return StringTable->insertn( assetFilePath, pFinalSlash - assetFilePath );
}

//-----------------------------------------------------------------------------

ModuleDefinition* AssetManager::getAssetModuleDefinition( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return NULL;
    }

    return pAssetDefinition->mpModuleDefinition;
}

//-----------------------------------------------------------------------------

bool AssetManager::isAssetInternal( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return false;
    }

    return pAssetDefinition->mAssetInternal;
}

//-----------------------------------------------------------------------------

bool AssetManager::isAssetAutoUnload( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return false;
    }

    return pAssetDefinition->mAssetAutoUnload;
}

//-----------------------------------------------------------------------------

bool AssetManager::isAssetLoaded( const char* pAssetId )
{
    // Find asset definition.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find asset Id '%s'.", pAssetId );
        return false;
    }

    return pAssetDefinition->mpAssetBase != NULL;
}


//-----------------------------------------------------------------------------

bool AssetManager::isDeclaredAsset( const char* pAssetId )
{
    return findAsset( pAssetId ) != NULL;
}

//-----------------------------------------------------------------------------

bool AssetManager::doesAssetDependOn( const char* pAssetId, const char* pDependsOnAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot use NULL asset Id." );
    AssertFatal( pDependsOnAssetId != NULL, "Cannot use NULL depends-on asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Fetch depends-on asset Id.
    StringTableEntry dependsOnAssetId = StringTable->insert( pDependsOnAssetId );

    // Find depends-on entry.
    typeAssetDependsOnHash::iterator dependsOnItr = mAssetDependsOn.find( assetId );

    // Iterate all dependencies.
    while( dependsOnItr != mAssetDependsOn.end() && dependsOnItr->key == assetId )
    {
        // Finish if a depends on.
        if ( dependsOnItr->value == dependsOnAssetId )
            return true;

        // Next dependency.
        dependsOnItr++;
    }

    return false;
}

//-----------------------------------------------------------------------------

bool AssetManager::isAssetDependedOn( const char* pAssetId, const char* pDependedOnByAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot use NULL asset Id." );
    AssertFatal( pDependedOnByAssetId != NULL, "Cannot use NULL depended-on-by asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Fetch depended-on-by asset Id.
    StringTableEntry dependedOnByAssetId = StringTable->insert( pDependedOnByAssetId );

    // Find depended-on-by entry.
    typeAssetDependsOnHash::iterator dependedOnItr = mAssetIsDependedOn.find( assetId );

    // Iterate all dependencies.
    while( dependedOnItr != mAssetIsDependedOn.end() && dependedOnItr->key == assetId )
    {
        // Finish if depended-on.
        if ( dependedOnItr->value == dependedOnByAssetId )
            return true;

        // Next dependency.
        dependedOnItr++;
    }

    return false;
}

//-----------------------------------------------------------------------------

bool AssetManager::isReferencedAsset( const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot check if NULL asset Id is referenced." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Is asset Id the correct format?
    if ( StringUnit::getUnitCount( assetId, ASSET_SCOPE_SEPARATOR ) != 2 )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot check if asset Id '%s' is referenced as it is not the correct format.", assetId );
        return false;
    }

    return mReferencedAssets.count( assetId ) > 0;
}

//-----------------------------------------------------------------------------

bool AssetManager::renameDeclaredAsset( const char* pAssetIdFrom, const char* pAssetIdTo )
{
    // Sanity!
    AssertFatal( pAssetIdFrom != NULL, "Cannot rename from NULL asset Id." );
    AssertFatal( pAssetIdTo != NULL, "Cannot rename to NULL asset Id." );

    // Fetch asset Ids.
    StringTableEntry assetIdFrom = StringTable->insert( pAssetIdFrom );
    StringTableEntry assetIdTo   = StringTable->insert( pAssetIdTo );

    // Is asset Id from the correct format?
    if ( StringUnit::getUnitCount( assetIdFrom, ASSET_SCOPE_SEPARATOR ) != 2 )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as source asset Id is not the correct format.", assetIdFrom, assetIdTo );
        return false;
    }

    // Is asset Id to the correct format?
    if ( StringUnit::getUnitCount( assetIdTo, ASSET_SCOPE_SEPARATOR ) != 2 )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as target asset Id is not the correct format.", assetIdFrom, assetIdTo );
        return false;
    }

    // Does the asset Id from exist?
    if ( !mDeclaredAssets.contains( assetIdFrom ) )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as source asset Id is not declared.", assetIdFrom, assetIdTo );
        return false;
    }

    // Does the asset Id to exist?
    if ( mDeclaredAssets.contains( assetIdTo ) )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as target asset Id is already declared.", assetIdFrom, assetIdTo );
        return false;
    }

    // Split module Ids from asset Ids.
    StringTableEntry moduleIdFrom = StringTable->insert( StringUnit::getUnit( assetIdFrom, 0, ASSET_SCOPE_SEPARATOR ) );
    StringTableEntry moduleIdTo   = StringTable->insert( StringUnit::getUnit( assetIdTo, 0, ASSET_SCOPE_SEPARATOR ) );

    // Are the module Ids the same?
    if ( moduleIdFrom != moduleIdTo )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as the module Id cannot be changed.", assetIdFrom, assetIdTo );
        return false;
    }

    // Find asset definition.
    typeDeclaredAssetsHash::iterator assetDefinitionItr =  mDeclaredAssets.find( assetIdFrom );

    // Sanity!
    AssertFatal( assetDefinitionItr != mDeclaredAssets.end(), "Asset Manager: Failed to find asset." );

    // Fetch asset definition.
    AssetDefinition* pAssetDefinition = assetDefinitionItr->value;

    // Is this a private asset?
    if ( pAssetDefinition->mAssetPrivate )
    {
        // Yes, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as the source asset is private.", assetIdFrom, assetIdTo );
        return false;
    }

    // Setup declared update visitor.
    TamlAssetDeclaredUpdateVisitor assetDeclaredUpdateVisitor;
    assetDeclaredUpdateVisitor.setAssetIdFrom( assetIdFrom );
    assetDeclaredUpdateVisitor.setAssetIdTo( assetIdTo );

    // Update asset file declaration.
    if ( !assetDeclaredUpdateVisitor.parse( pAssetDefinition->mAssetBaseFilePath ) )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename declared asset Id '%s' to asset Id '%s' as the declared asset file could not be parsed: %s",
            assetIdFrom, assetIdTo, pAssetDefinition->mAssetBaseFilePath );
        return false;
    }

    // Update asset definition.
    pAssetDefinition->mAssetId = assetIdTo;
    pAssetDefinition->mAssetName = StringTable->insert( StringUnit::getUnit( assetIdTo, 1, ASSET_SCOPE_SEPARATOR ) );

    // Reinsert declared asset.
    mDeclaredAssets.erase( assetIdFrom );
    mDeclaredAssets.insert( assetIdTo, pAssetDefinition );

    // Info.
    if ( mEchoInfo )
    {
        Con::printf( "Asset Manager: Renaming declared Asset Id '%s' to Asset Id '%s'.", assetIdFrom, assetIdTo );
        Con::printSeparator();
    }

    // Rename asset dependencies.
    renameAssetDependencies( assetIdFrom, assetIdTo );

    // Do we have an asset tags manifest?
    if ( !mAssetTagsManifest.isNull() )
    {
        // Yes, so rename any assets.
        mAssetTagsManifest->renameAssetId( pAssetIdFrom, pAssetIdTo );

        // Save the asset tags.
        saveAssetTags();
    }

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::renameReferencedAsset( const char* pAssetIdFrom, const char* pAssetIdTo )
{
    // Sanity!
    AssertFatal( pAssetIdFrom != NULL, "Cannot rename from NULL asset Id." );
    AssertFatal( pAssetIdTo != NULL, "Cannot rename to NULL asset Id." );

    // Fetch asset Ids.
    StringTableEntry assetIdFrom = StringTable->insert( pAssetIdFrom );
    StringTableEntry assetIdTo   = StringTable->insert( pAssetIdTo );

    // Is asset Id from the correct format?
    if ( StringUnit::getUnitCount( assetIdFrom, ASSET_SCOPE_SEPARATOR ) != 2 )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename referenced asset Id '%s' to asset Id '%s' as source asset Id is not the correct format.", assetIdFrom, assetIdTo );
        return false;
    }

    // Is asset Id to the correct format?
    if ( StringUnit::getUnitCount( assetIdTo, ASSET_SCOPE_SEPARATOR ) != 2 )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename referenced asset Id '%s' to asset Id '%s' as target asset Id is not the correct format.", assetIdFrom, assetIdTo );
        return false;
    }

    // Does the asset Id to exist?
    if ( !mDeclaredAssets.contains( assetIdTo ) )
    {
        // No, so warn.
        Con::warnf("Asset Manager: Cannot rename referenced asset Id '%s' to asset Id '%s' as target asset Id is not declared.", assetIdFrom, assetIdTo );
        return false;
    }

    // Rename asset references.
    renameAssetReferences( assetIdFrom, assetIdTo );

    // Info.
    if ( mEchoInfo )
        Con::printSeparator();

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::releaseAsset( const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot release NULL asset Id." );

    // Find asset.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to release asset Id '%s' as it does not exist.", pAssetId );
        return false;
    }

    // Is the asset loaded?
    if ( pAssetDefinition->mpAssetBase == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to release asset Id '%s' as it is not acquired.", pAssetId );
        return false;
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Started releasing Asset Id '%s'...", pAssetId );

        // Fetch asset Id.
        StringTableEntry assetId = StringTable->insert( pAssetId );

        // Yes, so find any asset dependencies.
        typeAssetDependsOnHash::iterator assetDependenciesItr = mAssetDependsOn.find( assetId );

        // Do we have any asset dependencies?
        if ( assetDependenciesItr != mAssetDependsOn.end() )
        {
            // Yes, so show all dependency assets.
            Con::printf( "Asset Manager: Found dependencies for Asset Id '%s' of:", pAssetId );

            // Iterate all dependencies.
            while( assetDependenciesItr != mAssetDependsOn.end() && assetDependenciesItr->key == assetId )
            {
                // Info.
                Con::printf( "Asset Manager: > Asset Id '%s'", assetDependenciesItr->value );

                // Next dependency.
                assetDependenciesItr++;
            }
        }
    }

    // Release asset reference.
    if ( pAssetDefinition->mpAssetBase->releaseAssetReference() )
    {
        // Are we ignoring auto-unloaded assets?
        if ( mIgnoreAutoUnload )
        {
            // Yes, so info.
            if ( mEchoInfo )
            {
                Con::printf( "Asset Manager: Asset Id '%s' now has a reference count of '0' but ignoring auto-unloading of assets.",
                    pAssetId,
                    pAssetDefinition->mpAssetBase->getAcquiredReferenceCount() );
            }
        }
        else
        {
            // No, so info.
            if ( mEchoInfo )
            {
                Con::printf( "Asset Manager: Asset Id '%s' is being unloaded.", pAssetId );
            }

            // Unload the asset.
            unloadAsset( pAssetDefinition );
        }
    }
    // Info.
    else if ( mEchoInfo )
    {
        if ( pAssetDefinition->mpAssetBase->getAcquiredReferenceCount() > 0 )
        {
            Con::printf( "Asset Manager: Asset Id '%s' now has a reference count of '%d'.",
                pAssetId,
                pAssetDefinition->mpAssetBase->getAcquiredReferenceCount() );
        }
        else
        {
            Con::printf( "Asset Manager: Asset Id '%s' now has a reference count of '0' but set to not auto-unload.",
                pAssetId,
                pAssetDefinition->mpAssetBase->getAcquiredReferenceCount() );
        }
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printf( "Asset Manager: ... Finished releasing Asset Id '%s'.", pAssetId );
    }

    return true;
}

//-----------------------------------------------------------------------------

void AssetManager::purgeAssets( void )
{
    // Info.
    if ( mEchoInfo )
    {
        Con::printf( "Asset Manager: Started purging assets..." );
    }

    // Iterate asset definitions.
    for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
    {
        // Fetch asset definition.
        AssetDefinition* pAssetDefinition = assetItr->value;

        // Skip asset if private, not loaded or referenced.
        if (    pAssetDefinition->mAssetPrivate ||
                pAssetDefinition->mpAssetBase == NULL ||
                pAssetDefinition->mpAssetBase->getAcquiredReferenceCount() > 0 )
            continue;

        // Info.
        if ( mEchoInfo )
        {
            Con::printf( "Asset Manager: Purging asset Id '%s'...", pAssetDefinition->mAssetId );
        }

        // Unload the asset.
        unloadAsset( pAssetDefinition );
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printf( "Asset Manager: ... Finished purging assets." );
    }
}

//-----------------------------------------------------------------------------

bool AssetManager::getAssetSnapshot( AssetSnapshot* pAssetSnapshot, const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetSnapshot != NULL, "cannot get asset snapshot using NULL asset snapshot." );
    AssertFatal( pAssetId != NULL, "Cannot get asset snapshot NULL asset Id." );

    // Find asset.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to get asset snapshot of asset Id '%s' as it does not exist.", pAssetId );
        return false;
    }

    // Acquire asset.
    AssetBase* pAssetBase = acquireAsset<AssetBase>( pAssetId );

    // Did we acquire the asset?
    if ( pAssetBase == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to get asset snapshot of asset Id '%s' as it could not be acquired.", pAssetId );
        return false;
    }

    // Reset asset snapshot.
    pAssetSnapshot->resetSnapshot();

    // Fetch asset parent abstract class rep.
    // NOTE: I don't like referring to types in a string but we don't have much choice here.
    AbstractClassRep* pAssetBaseParentClassRep = AbstractClassRep::findClassRep( "AssetBase" )->getParentClass();

    // Fetch asset field list.
    const AbstractClassRep::FieldList& assetFieldList = pAssetBase->getFieldList();

    // Populate asset snapshot.
    for( Vector<AbstractClassRep::Field>::const_iterator assetFieldItr = assetFieldList.begin(); assetFieldItr != assetFieldList.end(); ++assetFieldItr )
    {
        // Skip abstract class rep fields.
        if (    assetFieldItr->type == AbstractClassRep::StartGroupFieldType ||
                assetFieldItr->type == AbstractClassRep::EndGroupFieldType ||
                assetFieldItr->type == AbstractClassRep::DepricatedFieldType )
            continue;

        // Fetch asset field name.
        StringTableEntry assetFieldName = assetFieldItr->pFieldname;

        // Skip asset parent field.
        if ( pAssetBaseParentClassRep->findField( assetFieldName ) != NULL )
            continue;

        // Fetch asset field value.
        const char* pFieldValue = pAssetBase->getDataField( assetFieldName, NULL );

        // Set asset snapshot field.
        pAssetSnapshot->setDataField( assetFieldName, NULL, pFieldValue );
    }

    // Release asset.
    releaseAsset( pAssetId );

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::setAssetSnapshot( AssetSnapshot* pAssetSnapshot, const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetSnapshot != NULL, "cannot get asset snapshot using NULL asset snapshot." );
    AssertFatal( pAssetId != NULL, "Cannot get asset snapshot NULL asset Id." );

    // Find asset.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to set asset snapshot of asset Id '%s' as it does not exist.", pAssetId );
        return false;
    }

    // Acquire asset.
    AssetBase* pAssetBase = acquireAsset<AssetBase>( pAssetId );

    // Did we acquire the asset?
    if ( pAssetBase == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to set asset snapshot of asset Id '%s' as it could not be acquired.", pAssetId );
        return false;
    }

    // Disable asset refresh so we don't perform a refresh on each field change.
    pAssetDefinition->mAssetRefreshEnable = false;

    // Fetch asset parent abstract class rep.
    // NOTE: I don't like referring to types in a string but we don't have much choice here.
    AbstractClassRep* pAssetBaseParentClassRep = AbstractClassRep::findClassRep( "AssetBase" )->getParentClass();

    // Fetch asset name field.
    StringTableEntry assetNameField = StringTable->insert( ASSET_BASE_ASSETNAME_FIELD );

    SimFieldDictionary* pSnapshotFields = pAssetSnapshot->getFieldDictionary();

    // Iterate snapshot dynamic fields.
    for( SimFieldDictionaryIterator fieldItr(pSnapshotFields); *fieldItr; ++fieldItr )
    {
        // Fetch dynamic field entry.
        const SimFieldDictionary::Entry* pFieldEntry = *fieldItr;

        // Fetch asset snapshot field name.
        StringTableEntry assetSnapshotField = pFieldEntry->slotName;

        // Skip asset name field.
        if ( assetSnapshotField == assetNameField )
            continue;

        // Skip asset parent field.
        if ( pAssetBaseParentClassRep->findField( assetSnapshotField ) != NULL )
            continue;

        // Set asset field.
        pAssetBase->setDataField( assetSnapshotField, NULL, pFieldEntry->value );
    }

    // Re-enable asset refresh.
    pAssetDefinition->mAssetRefreshEnable = true;

    // Refresh asset.
    refreshAsset( pAssetId );

    // Release asset.
    releaseAsset( pAssetId );

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::deleteAsset( const char* pAssetId, const bool deleteLooseFiles, const bool deleteDependencies )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot delete NULL asset Id." );

    // Find asset.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to delete asset Id '%s' as it does not exist.", pAssetId );
        return false;
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Started deleting Asset Id '%s'...", pAssetId );
    }  

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Are we deleting dependencies?
    if ( deleteDependencies )
    {
        Vector<typeAssetId> dependantAssets;

        // Yes, so find depended-on-by entry.
        typeAssetDependsOnHash::iterator dependedOnItr = mAssetIsDependedOn.find( assetId );

        // Iterate all dependencies.
        while( dependedOnItr != mAssetIsDependedOn.end() && dependedOnItr->key == assetId )
        {
            // Store asset Id.
            dependantAssets.push_back( dependedOnItr->value );

            // Next dependency.
            dependedOnItr++;
        }

        // Do we have any dependants?
        if ( dependantAssets.size() > 0 )
        {
            // Yes, so iterate dependants.
            for( Vector<typeAssetId>::const_iterator assetItr = dependantAssets.begin(); assetItr !=  dependantAssets.end(); ++assetItr )
            {
                StringTableEntry dependentAssetId = *assetItr;

                // Info.
                if ( mEchoInfo )
                {
                    Con::printSeparator();
                    Con::printf( "Asset Manager: Deleting Asset Id '%s' dependant of '%s.'", pAssetId, dependentAssetId );
                }

                // Delete dependant.
                deleteAsset( dependentAssetId, deleteLooseFiles, deleteDependencies );
            }
        }
    }

    // Remove asset references.
    removeAssetReferences( assetId );

    // Are we deleting loose files?
    if ( deleteLooseFiles )
    {
        // Yes, so remove loose files.
        Vector<StringTableEntry>& assetLooseFiles = pAssetDefinition->mAssetLooseFiles;
        for( Vector<StringTableEntry>::iterator looseFileItr = assetLooseFiles.begin(); looseFileItr != assetLooseFiles.end(); ++looseFileItr )
        {
            // Fetch loose file.
            StringTableEntry looseFile = *looseFileItr;

            // Delete the loose file.
            if ( !Platform::fileDelete( looseFile ) )
            {
                // Failed so warn.
                Con::warnf( "Asset Manager: Failed to delete the loose file '%s' while deleting asset Id '%s'.", looseFile, pAssetId );
            }
        }
    }

    // Fetch asset definition file.
    StringTableEntry assetDefinitionFile = pAssetDefinition->mAssetBaseFilePath;

    // Remove reference here as we're about to remove the declared asset.
    pAssetDefinition = NULL;

    // Remove asset.
    removeSingleDeclaredAsset( pAssetId );

    // Delete the asset definition file.
    if ( !Platform::fileDelete( assetDefinitionFile ) )
    {
        // Failed so warn.
        Con::warnf( "Asset Manager: Failed to delete the asset definition file '%s' while deleting asset Id '%s'.", assetDefinitionFile, pAssetId );
    }       

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Finished deleting Asset Id '%s'.", pAssetId );
    }

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::reloadAsset( const char* pAssetId )
{
	// Sanity!
    AssertFatal( pAssetId != NULL, "Cannot refresh NULL asset Id." );

    // Find asset.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

	// Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to reload asset Id '%s' as it does not exist.", pAssetId );
        return false;
    }

	pAssetDefinition->mpAssetBase->onAssetReload();

	return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::refreshAsset( const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot refresh NULL asset Id." );

    // Find asset.
    AssetDefinition* pAssetDefinition = findAsset( pAssetId );

    // Did we find the asset?
    if ( pAssetDefinition == NULL )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to refresh asset Id '%s' as it does not exist.", pAssetId );
        return false;
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Started refreshing Asset Id '%s'...", pAssetId );
    }    

    // Is the asset private?
    if ( pAssetDefinition->mAssetPrivate )
    {
        // Yes, so notify asset of asset refresh only.
        pAssetDefinition->mpAssetBase->onAssetRefresh();
    }
    // Is the asset definition allowed to refresh?
    else if ( pAssetDefinition->mAssetRefreshEnable )
    {
        // Yes, so fetch the asset.
        AssetBase* pAssetBase = pAssetDefinition->mpAssetBase;

        // Is the asset loaded?
        if ( pAssetBase != NULL )
        {
            // Yes, so notify asset of asset refresh.
            pAssetBase->onAssetRefresh();

            // Save asset.
            mTaml.write( pAssetBase, pAssetDefinition->mAssetBaseFilePath );
        
            // Remove asset dependencies.
            removeAssetDependencies( pAssetId );

            // Find any new dependencies.
            TamlAssetDeclaredVisitor assetDeclaredVisitor;

            // Parse the filename.
            if ( !assetDeclaredVisitor.parse( pAssetDefinition->mAssetBaseFilePath ) )
            {
                // Warn.
                Con::warnf( "Asset Manager: Failed to parse file containing asset declaration: '%s'.\nDependencies are now incorrect!", pAssetDefinition->mAssetBaseFilePath );
                return false;
            }

            // Fetch asset dependencies.
            TamlAssetDeclaredVisitor::typeAssetIdVector& assetDependencies = assetDeclaredVisitor.getAssetDependencies();

            // Fetch asset Id.
            StringTableEntry assetId = StringTable->insert( pAssetId );

            // Are there any asset dependences?
            if ( assetDependencies.size() > 0 )
            {
                // Yes, so iterate dependencies.
                for( TamlAssetDeclaredVisitor::typeAssetIdVector::iterator assetDependencyItr = assetDependencies.begin(); assetDependencyItr != assetDependencies.end(); ++assetDependencyItr )
                {
                    // Fetch dependency asset Id.
                    StringTableEntry dependencyAssetId = *assetDependencyItr;

                    // Insert depends-on.
                    mAssetDependsOn.insertEqual( assetId, dependencyAssetId );

                    // Insert is-depended-on.
                    mAssetIsDependedOn.insertEqual( dependencyAssetId, assetId );
                }
            }

            // Fetch asset loose files.
            TamlAssetDeclaredVisitor::typeLooseFileVector& assetLooseFiles = assetDeclaredVisitor.getAssetLooseFiles();

            // Clear any existing loose files.
            pAssetDefinition->mAssetLooseFiles.clear();

            // Are there any loose files?
            if ( assetLooseFiles.size() > 0 )
            {
                // Yes, so iterate loose files.
                for( TamlAssetDeclaredVisitor::typeLooseFileVector::iterator assetLooseFileItr = assetLooseFiles.begin(); assetLooseFileItr != assetLooseFiles.end(); ++assetLooseFileItr )
                {
                    // Store loose file.
                    pAssetDefinition->mAssetLooseFiles.push_back( *assetLooseFileItr );
                }
            }

            // Asset refresh notifications.
            for( typeAssetPtrRefreshHash::iterator refreshNotifyItr = mAssetPtrRefreshNotifications.begin(); refreshNotifyItr != mAssetPtrRefreshNotifications.end(); ++refreshNotifyItr )
            {
                // Fetch pointed asset.
                StringTableEntry pointedAsset = refreshNotifyItr->key->getAssetId();

                // Ignore if the pointed asset is not a dependency.
                if ( !doesAssetDependOn( pointedAsset, assetId ) )
                    continue;

                // Perform refresh notification callback.
                refreshNotifyItr->value->onAssetRefreshed( refreshNotifyItr->key );
            }

            // Find is-depends-on entry.
            typeAssetIsDependedOnHash::iterator isDependedOnItr = mAssetIsDependedOn.find( assetId );

            // Is asset depended on?
            if ( isDependedOnItr != mAssetIsDependedOn.end() )
            {
                // Yes, so compiled them.
                Vector<typeAssetId> dependedOn;

                // Iterate all dependencies.
                while( isDependedOnItr != mAssetIsDependedOn.end() && isDependedOnItr->key == assetId )
                {
                    dependedOn.push_back( isDependedOnItr->value );

                    // Next dependency.
                    isDependedOnItr++;
                }

                // Refresh depended-on assets.
                for ( Vector<typeAssetId>::iterator isDependedOnItr = dependedOn.begin(); isDependedOnItr != dependedOn.end(); ++isDependedOnItr )
                {
                    // Refresh dependency asset.
                    refreshAsset( *isDependedOnItr );
                }
            }
        }
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Finished refreshing Asset Id '%s'.", pAssetId );
    }

    return true;
}

//-----------------------------------------------------------------------------

void AssetManager::refreshAllAssets( const bool includeUnloaded )
{
    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Started refreshing ALL assets." );
    }

    Vector<typeAssetId> assetsToRelease;

    // Are we including unloaded assets?
    if ( includeUnloaded )
    {
        // Yes, so prepare a list of assets to release and load them.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset Id.
            typeAssetId assetId = assetItr->key;

            // Skip if asset is loaded.
            if ( assetItr->value->mpAssetBase != NULL )
                continue;

            // Note asset as needing a release.
            assetsToRelease.push_back( assetId );

            // Acquire the asset.
            acquireAsset<AssetBase>( assetId );
        }
    }

    // Refresh the current loaded assets.
    // NOTE: This will result in some assets being refreshed more than once due to asset dependencies.
    for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
    {
        // Skip private assets.
        if ( assetItr->value->mAssetPrivate )
            continue;

        // Refresh asset if it's loaded.
        refreshAsset( assetItr->key );
    }

    // Are we including unloaded assets?
    if ( includeUnloaded )
    {
        // Yes, so release the assets we loaded.
        for( Vector<typeAssetId>::iterator assetItr = assetsToRelease.begin(); assetItr != assetsToRelease.end(); ++assetItr )
        {
            releaseAsset( *assetItr );
        }
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Finished refreshing ALL assets." );
    }
}

//-----------------------------------------------------------------------------

void AssetManager::registerAssetPtrRefreshNotify( AssetPtrBase* pAssetPtrBase, AssetPtrCallback* pCallback )
{
    // Find an existing notification iterator.
    typeAssetPtrRefreshHash::iterator notificationItr = mAssetPtrRefreshNotifications.find( pAssetPtrBase );

    // Do we have one?
    if ( notificationItr != mAssetPtrRefreshNotifications.end() )
    {
        // Yes, so update the callback.
        notificationItr->value = pCallback;
        return;
    }

    // No, so add one.
    mAssetPtrRefreshNotifications.insert( pAssetPtrBase, pCallback );
}

//-----------------------------------------------------------------------------

void AssetManager::unregisterAssetPtrRefreshNotify( AssetPtrBase* pAssetPtrBase )
{
    mAssetPtrRefreshNotifications.erase( pAssetPtrBase );
}

//-----------------------------------------------------------------------------

bool AssetManager::loadAssetTags( ModuleDefinition* pModuleDefinition )
{
    // Sanity!
    AssertFatal( pModuleDefinition != NULL, "Cannot load asset tags manifest using a NULL module definition" );

    // Expand manifest location.
    char assetTagsManifestFilePathBuffer[1024];
    Con::expandPath( assetTagsManifestFilePathBuffer, sizeof(assetTagsManifestFilePathBuffer), pModuleDefinition->getAssetTagsManifest() );

    // Do we already have a manifest?
    if ( !mAssetTagsManifest.isNull() )
    {
        // Yes, so warn.
        Con::warnf( "Asset Manager: Cannot load asset tags manifest from module '%s' as one is already loaded.", pModuleDefinition->getSignature() );
        return false;
    }

    // Is the specified file valid?
    if ( Platform::isFile( assetTagsManifestFilePathBuffer ) )
    {
        // Yes, so read asset tags manifest.
        mAssetTagsManifest = mTaml.read<AssetTagsManifest>( assetTagsManifestFilePathBuffer );

        // Did we read the manifest?
        if ( mAssetTagsManifest.isNull() )
        {
            // No, so warn.
            Con::warnf( "Asset Manager: Failed to load asset tags manifest '%s' from module '%s'.", assetTagsManifestFilePathBuffer, pModuleDefinition->getSignature() );
            return false;
        }

        // Set asset tags module definition.
        mAssetTagsModuleDefinition = pModuleDefinition;
    }
    else
    {
        // No, so generate a new asset tags manifest.
        mAssetTagsManifest = new AssetTagsManifest();
        mAssetTagsManifest->registerObject();

        // Set asset tags module definition.
        mAssetTagsModuleDefinition = pModuleDefinition;

        // Save the asset tags.
        saveAssetTags();
    }

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::saveAssetTags( void )
{
    // Do we have an asset tags manifest?
    if ( mAssetTagsManifest.isNull() || mAssetTagsModuleDefinition.isNull() )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Failed to save asset tags manifest as one is not loaded." );
        return false;
    }

    // Expand manifest location.
    char assetTagsManifestFilePathBuffer[1024];
    Con::expandPath( assetTagsManifestFilePathBuffer, sizeof(assetTagsManifestFilePathBuffer), mAssetTagsModuleDefinition->getAssetTagsManifest() );

    // Save asset tags manifest.
    if ( !mTaml.write( mAssetTagsManifest, assetTagsManifestFilePathBuffer ) )
    {
        // Failed so warn.
        Con::warnf( "Asset Manager: Failed to save asset tags manifest '%s' from module '%s'.", assetTagsManifestFilePathBuffer, mAssetTagsModuleDefinition->getSignature() );
        return false;
    }

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::restoreAssetTags( void )
{
    // Do we already have a manifest?
    if ( mAssetTagsManifest.isNull() )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot restore asset tags manifest as one is not already loaded." );
        return false;
    }

    // Sanity!
    AssertFatal( mAssetTagsModuleDefinition != NULL, "Cannot restore asset tags manifest as module definition is NULL." );

    // Delete existing asset tags manifest.
    mAssetTagsManifest->deleteObject();

    // Reload asset tags manifest.
    return loadAssetTags( mAssetTagsModuleDefinition );
}

//-----------------------------------------------------------------------------

S32 QSORT_CALLBACK descendingAssetDefinitionLoadCount(const void* a, const void* b)
{
    // Fetch asset definitions.
    const AssetDefinition* pAssetDefinitionA  = *(AssetDefinition**)a;
    const AssetDefinition* pAssetDefinitionB  = *(AssetDefinition**)b;

    // Sort.
    return pAssetDefinitionB->mAssetLoadedCount - pAssetDefinitionA->mAssetLoadedCount;
}

//-----------------------------------------------------------------------------

void AssetManager::dumpDeclaredAssets( void ) const
{
    Vector<const AssetDefinition*> assetDefinitions;

    // Iterate asset definitions.
    for( typeDeclaredAssetsHash::const_iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
    {
        assetDefinitions.push_back( assetItr->value );
    }

    // Sort asset definitions.
    dQsort( assetDefinitions.address(), assetDefinitions.size(), sizeof(const AssetDefinition*), descendingAssetDefinitionLoadCount );

    // Info.
    Con::printSeparator();
    Con::printf( "Asset Manager: %d declared asset(s) dump as follows:", mDeclaredAssets.size() );
    Con::printBlankLine();

    // Iterate sorted asset definitions.
    for ( Vector<const AssetDefinition*>::iterator assetItr = assetDefinitions.begin(); assetItr != assetDefinitions.end(); ++assetItr )
    {
        // Fetch asset definition.
        const AssetDefinition* pAssetDefinition = *assetItr;

        // Info.
        Con::printf( "AssetId:'%s', LoadCount:%d, UnloadCount:%d, AutoUnload:%d, Loaded:%d, Internal:%d, Private: %d, Type:'%s', Module/Version:'%s'/'%d', File:'%s'",
            pAssetDefinition->mAssetId,
            pAssetDefinition->mAssetLoadedCount,
            pAssetDefinition->mAssetUnloadedCount,
            pAssetDefinition->mAssetAutoUnload,
            pAssetDefinition->mpAssetBase != NULL,
            pAssetDefinition->mAssetInternal,
            pAssetDefinition->mAssetPrivate,
            pAssetDefinition->mAssetType,
            pAssetDefinition->mpModuleDefinition->getModuleId(),
            pAssetDefinition->mpModuleDefinition->getVersionId(),
            pAssetDefinition->mAssetBaseFilePath );
    }

    // Info.
    Con::printSeparator();
    Con::printBlankLine();
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAllAssets( AssetQuery* pAssetQuery, const bool ignoreInternal, const bool ignorePrivate )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );

    // Reset result count.
    S32 resultCount = 0;

    // Iterate declared assets.
    for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
    {
        // Fetch asset definition.
        AssetDefinition* pAssetDefinition = assetItr->value;

        // Skip if internal and we're ignoring them.
        if ( ignoreInternal && pAssetDefinition->mAssetInternal )
            continue;

        // Skip if private and we're ignoring them.
        if ( ignorePrivate && pAssetDefinition->mAssetPrivate )
            continue;

        // Store as result.
        pAssetQuery->push_back( pAssetDefinition->mAssetId );

        // Increase result count.
        resultCount++;
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetName( AssetQuery* pAssetQuery, const char* pAssetName, const bool partialName )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pAssetName != NULL, "Cannot use NULL asset name." );

    // Reset asset name.
    StringTableEntry assetName = NULL;
    S32 partialAssetNameLength = 0;
        
    // Are we doing partial name search?
    if ( partialName ) 
    {
        // Yes, so fetch length of partial name.
        partialAssetNameLength = dStrlen( pAssetName );
    }
    else
    {
        // No, so fetch asset name.
        assetName = StringTable->insert( pAssetName );
    }

    // Reset result count.
    S32 resultCount = 0;

    // Iterate declared assets.
    for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
    {
        // Fetch asset definition.
        AssetDefinition* pAssetDefinition = assetItr->value;

        // Are we doing partial name search?
        if ( partialName ) 
        {
            // Yes, so fetch the length of this asset name.
            const S32 currentAssetNameLength = dStrlen( pAssetDefinition->mAssetName );

            // Skip if the query asset name is longer than the current asset name.
            if ( partialAssetNameLength > currentAssetNameLength )
                continue;
            
            // Skip if this is not the asset we want.
            if ( dStrnicmp( pAssetDefinition->mAssetName, pAssetName, partialAssetNameLength ) != 0 )
                continue;
        }
        else
        {
            // No, so skip if this is not the asset we want.
            if ( assetName != pAssetDefinition->mAssetName )
                continue;
        }

        // Store as result.
        pAssetQuery->push_back( pAssetDefinition->mAssetId );

        // Increase result count.
        resultCount++;
    }

    return resultCount;
}
    
//-----------------------------------------------------------------------------

S32 AssetManager::findAssetCategory( AssetQuery* pAssetQuery, const char* pAssetCategory, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pAssetCategory != NULL, "Cannot use NULL asset category." );

    // Fetch asset category.
    StringTableEntry assetCategory = StringTable->insert( pAssetCategory );

    // Reset result count.
    S32 resultCount = 0;

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = findAsset( *assetItr );

            // Skip if this is not the asset we want.
            if (    pAssetDefinition == NULL ||
                    pAssetDefinition->mAssetCategory != assetCategory )
                        continue;

            // Store as result.
            filteredAssets.push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // No, so iterate declared assets.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = assetItr->value;

            // Skip if this is not the asset we want.
            if ( assetCategory != pAssetDefinition->mAssetCategory )
                continue;

            // Store as result.
            pAssetQuery->push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }
    }

    return resultCount;
}

S32 AssetManager::findAssetAutoUnload( AssetQuery* pAssetQuery, const bool assetAutoUnload, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );

    // Reset result count.
    S32 resultCount = 0;

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = findAsset( *assetItr );

            // Skip if this is not the asset we want.
            if (    pAssetDefinition == NULL ||
                    pAssetDefinition->mAssetAutoUnload != assetAutoUnload )
                        continue;

            // Store as result.
            filteredAssets.push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // No, so iterate declared assets.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = assetItr->value;

            // Skip if this is not the asset we want.
            if ( assetAutoUnload != pAssetDefinition->mAssetAutoUnload )
                continue;

            // Store as result.
            pAssetQuery->push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetInternal( AssetQuery* pAssetQuery, const bool assetInternal, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );

    // Reset result count.
    S32 resultCount = 0;

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = findAsset( *assetItr );

            // Skip if this is not the asset we want.
            if (    pAssetDefinition == NULL ||
                    pAssetDefinition->mAssetInternal != assetInternal )
                        continue;

            // Store as result.
            filteredAssets.push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // No, so iterate declared assets.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = assetItr->value;

            // Skip if this is not the asset we want.
            if ( assetInternal != pAssetDefinition->mAssetInternal )
                continue;

            // Store as result.
            pAssetQuery->push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetPrivate( AssetQuery* pAssetQuery, const bool assetPrivate, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );

    // Reset result count.
    S32 resultCount = 0;

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = findAsset( *assetItr );

            // Skip if this is not the asset we want.
            if (    pAssetDefinition == NULL ||
                    pAssetDefinition->mAssetPrivate != assetPrivate )
                        continue;

            // Store as result.
            filteredAssets.push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // No, so iterate declared assets.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = assetItr->value;

            // Skip if this is not the asset we want.
            if ( assetPrivate != pAssetDefinition->mAssetPrivate )
                continue;

            // Store as result.
            pAssetQuery->push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetType( AssetQuery* pAssetQuery, const char* pAssetType, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pAssetType != NULL, "Cannot use NULL asset type." );

    // Fetch asset type.
    StringTableEntry assetType = StringTable->insert( pAssetType );

    // Reset result count.
    S32 resultCount = 0;

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = findAsset( *assetItr );

            // Skip if this is not the asset we want.
            if (    pAssetDefinition == NULL ||
                    pAssetDefinition->mAssetType != assetType )
                        continue;

            // Store as result.
            filteredAssets.push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // No, so iterate declared assets.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = assetItr->value;

            // Skip if this is not the asset we want.
            if ( assetType != pAssetDefinition->mAssetType )
                continue;

            // Store as result.
            pAssetQuery->push_back( pAssetDefinition->mAssetId );

            // Increase result count.
            resultCount++;
        }
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetDependsOn( AssetQuery* pAssetQuery, const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pAssetId != NULL, "Cannot use NULL asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Reset result count.
    S32 resultCount = 0;

    // Find depends-on entry.
    typeAssetDependsOnHash::iterator dependsOnItr = mAssetDependsOn.find( assetId );

    // Iterate all dependencies.
    while( dependsOnItr != mAssetDependsOn.end() && dependsOnItr->key == assetId )
    {
        // Store as result.
        pAssetQuery->push_back( dependsOnItr->value );

        // Next dependency.
        dependsOnItr++;

        // Increase result count.
        resultCount++;
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetIsDependedOn( AssetQuery* pAssetQuery, const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pAssetId != NULL, "Cannot use NULL asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Reset result count.
    S32 resultCount = 0;

    // Find depended-on entry.
    typeAssetIsDependedOnHash::iterator dependedOnItr = mAssetIsDependedOn.find( assetId );

    // Iterate all dependencies.
    while( dependedOnItr != mAssetIsDependedOn.end() && dependedOnItr->key == assetId )
    {
        // Store as result.
        pAssetQuery->push_back( dependedOnItr->value );

        // Next dependency.
        dependedOnItr++;

        // Increase result count.
        resultCount++;
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findInvalidAssetReferences( AssetQuery* pAssetQuery )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );

    // Reset result count.
    S32 resultCount = 0;

    // Iterate referenced assets.
    for( typeReferencedAssetsHash::iterator assetItr = mReferencedAssets.begin(); assetItr != mReferencedAssets.end(); ++assetItr )
    {
        // Find asset definition.
        AssetDefinition* pAssetDefinition = findAsset( assetItr->key );

        // Skip if the asset definition was found.
        if ( pAssetDefinition != NULL )
            continue;

        // Store as result.
        pAssetQuery->push_back( assetItr->key );

        // Increase result count.
        resultCount++;
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findTaggedAssets( AssetQuery* pAssetQuery, const char* pAssetTagNames, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pAssetTagNames != NULL, "Cannot use NULL asset tag name(s)." );

    // Do we have an asset tag manifest?
    if ( mAssetTagsManifest.isNull() )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Cannot find tagged assets as no asset tag manifest is present." );
        return 0;
    }

    // Reset result count.
    S32 resultCount = 0;

    const char* pTagSeparators = ",\t\n";

    // Fetch tag count.
    U32 assetTagCount = StringUnit::getUnitCount( pAssetTagNames, pTagSeparators );

    // Fetch asset tags.
    Vector<AssetTagsManifest::AssetTag*> assetTags;
    for( U32 tagIndex = 0; tagIndex < assetTagCount; ++tagIndex )
    {
        // Fetch asset tag name.
        const char* pTagName = StringUnit::getUnit( pAssetTagNames, tagIndex, pTagSeparators );

        // Fetch asset tag.
        AssetTagsManifest::AssetTag* pAssetTag = mAssetTagsManifest->findAssetTag( pTagName );

        // Did we find the asset tag?
        if ( pAssetTag == NULL )
        {
            // No, so warn.
            Con::warnf( "AssetTagsManifest: Asset Manager: Cannot find tagged assets of '%s' as it does not exist.  Ignoring tag.", pTagName );
            continue;
        }

        assetTags.push_back( pAssetTag );
    }

    // Fetch found asset tag count.
    assetTagCount = assetTags.size();

    // Did we find any tags?
    if ( assetTagCount == 0 )
    {
        // No, so warn.
        Con::warnf( "AssetTagsManifest: Asset Manager: No specified tagged assets found in '%s'.", pAssetTagNames );
        return 0;
    } 

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset Id.
            StringTableEntry assetId = *assetItr;

            // Skip if asset is not valid.
            if ( !isDeclaredAsset( assetId ) )
                continue;

            // Reset matched flag.
            bool assetTagMatched = false;

            // Iterate asset tags.
            for ( Vector<AssetTagsManifest::AssetTag*>::iterator assetTagItr = assetTags.begin(); assetTagItr != assetTags.end(); ++assetTagItr )
            {
                // Fetch asset tag.
                AssetTagsManifest::AssetTag* pAssetTag = *assetTagItr;

                // Skip if asset is not tagged.
                if ( !pAssetTag->containsAsset( assetId ) )
                    continue;
                
                // Flag as matched.
                assetTagMatched = true;
                break;
            }

            // Did we find a match?
            if ( assetTagMatched )
            {
                // Yes, so is asset already present?
                if ( !filteredAssets.containsAsset( assetId ) )
                {
                    // No, so store as result.
                    filteredAssets.push_back( assetId );

                    // Increase result count.
                    resultCount++;
                }
            }
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // Iterate asset tags.
        for ( Vector<AssetTagsManifest::AssetTag*>::iterator assetTagItr = assetTags.begin(); assetTagItr != assetTags.end(); ++assetTagItr )
        {
            // Fetch asset tag.
            AssetTagsManifest::AssetTag* pAssetTag = *assetTagItr;

            // Iterate tagged assets.
            for ( Vector<typeAssetId>::iterator assetItr = pAssetTag->mAssets.begin(); assetItr != pAssetTag->mAssets.end(); ++assetItr )
            {
                // Fetch asset Id.
                StringTableEntry assetId = *assetItr;

                // Skip if asset Id is already present.
                if ( pAssetQuery->containsAsset( assetId ) )
                    continue;

                // Store as result.
                pAssetQuery->push_back( assetId );

                // Increase result count.
                resultCount++;
            }
        }
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

S32 AssetManager::findAssetLooseFile( AssetQuery* pAssetQuery, const char* pLooseFile, const bool assetQueryAsSource )
{
    // Sanity!
    AssertFatal( pAssetQuery != NULL, "Cannot use NULL asset query." );
    AssertFatal( pLooseFile != NULL, "Cannot use NULL loose file." );

    // Expand loose file.
    char looseFileBuffer[1024];
    Con::expandPath(looseFileBuffer, sizeof(looseFileBuffer), pLooseFile, false );

    // Fetch asset loose file.
    StringTableEntry looseFile = StringTable->insert( looseFileBuffer );

    // Reset result count.
    S32 resultCount = 0;

    // Use asset-query as the source?
    if ( assetQueryAsSource )
    {
        AssetQuery filteredAssets;

        // Yes, so iterate asset query.
        for( Vector<StringTableEntry>::iterator assetItr = pAssetQuery->begin(); assetItr != pAssetQuery->end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = findAsset( *assetItr );

            // Fetch loose files.
            Vector<StringTableEntry>& assetLooseFiles = pAssetDefinition->mAssetLooseFiles;

            // Skip if this asset has no loose files.
            if ( assetLooseFiles.size() == 0 )
                continue;

            // Search the assets loose files.
            for( Vector<StringTableEntry>::iterator looseFileItr = assetLooseFiles.begin(); looseFileItr != assetLooseFiles.end(); ++looseFileItr )
            {
                // Is this the loose file we are searching for?
                if ( *looseFileItr != looseFile )
                    continue;

                // Store as result.
                filteredAssets.push_back( pAssetDefinition->mAssetId );

                // Increase result count.
                resultCount++;

                break;
            }
        }

        // Set asset query.
        pAssetQuery->set( filteredAssets );
    }
    else
    {
        // No, so iterate declared assets.
        for( typeDeclaredAssetsHash::iterator assetItr = mDeclaredAssets.begin(); assetItr != mDeclaredAssets.end(); ++assetItr )
        {
            // Fetch asset definition.
            AssetDefinition* pAssetDefinition = assetItr->value;

            // Fetch loose files.
            Vector<StringTableEntry>& assetLooseFiles = pAssetDefinition->mAssetLooseFiles;

            // Skip if this asset has no loose files.
            if ( assetLooseFiles.size() == 0 )
                continue;

            // Search the assets loose files.
            for( Vector<StringTableEntry>::iterator looseFileItr = assetLooseFiles.begin(); looseFileItr != assetLooseFiles.end(); ++looseFileItr )
            {
                // Is this the loose file we are searching for?
                if ( *looseFileItr != looseFile )
                    continue;

                // Store as result.
                pAssetQuery->push_back( pAssetDefinition->mAssetId );

                // Increase result count.
                resultCount++;

                break;
            }
        }
    }

    return resultCount;
}

//-----------------------------------------------------------------------------

bool AssetManager::scanDeclaredAssets( const char* pPath, const char* pExtension, const bool recurse, ModuleDefinition* pModuleDefinition )
{
    // Sanity!
    AssertFatal( pPath != NULL, "Cannot scan declared assets with NULL path." );
    AssertFatal( pExtension != NULL, "Cannot scan declared assets with NULL extension." );

    // Expand path location.
    char pathBuffer[1024];
    Con::expandPath( pathBuffer, sizeof(pathBuffer), pPath );

    // Find files.
    Vector<Platform::FileInfo> files;
    if ( !Platform::dumpPath( pathBuffer, files, recurse ? -1 : 0 ) )
    {
        // Failed so warn.
        Con::warnf( "Asset Manager: Failed to scan declared assets in directory '%s'.", pathBuffer );
        return false;
    }

    // Is the asset file-path located within the specified module?
    if ( !Con::isBasePath( pathBuffer, pModuleDefinition->getModulePath() ) )
    {
        // No, so warn.
        Con::warnf( "Asset Manager: Could not add declared asset file '%s' as file does not exist with module path '%s'",
            pathBuffer,
            pModuleDefinition->getModulePath() );
        return false;
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Scanning for declared assets in path '%s' for files with extension '%s'...", pathBuffer, pExtension );
    }

    // Fetch extension length.
    const U32 extensionLength = dStrlen( pExtension );

    // Fetch module assets.
    ModuleDefinition::typeModuleAssetsVector& moduleAssets = pModuleDefinition->getModuleAssets();

    TamlAssetDeclaredVisitor assetDeclaredVisitor;

    // Iterate files.
    for ( Vector<Platform::FileInfo>::iterator fileItr = files.begin(); fileItr != files.end(); ++fileItr )
    {
        // Fetch file info.
        Platform::FileInfo& fileInfo = *fileItr;

        // Fetch filename.
        const char* pFilename = fileInfo.pFileName;

        // Find filename length.
        const U32 filenameLength = dStrlen( pFilename );

        // Skip if extension is longer than filename.
        if ( extensionLength > filenameLength )
            continue;

        // Skip if extension not found.
        if ( dStricmp( pFilename + filenameLength - extensionLength, pExtension ) != 0 )
            continue;

        // Clear declared assets.
        assetDeclaredVisitor.clear();

        // Format full file-path.
        char assetFileBuffer[1024];
        dSprintf( assetFileBuffer, sizeof(assetFileBuffer), "%s/%s", fileInfo.pFullPath, fileInfo.pFileName );

        // Parse the filename.
        if ( !assetDeclaredVisitor.parse( assetFileBuffer ) )
        {
            // Warn.
            Con::warnf( "Asset Manager: Failed to parse file containing asset declaration: '%s'.", assetFileBuffer );
            continue;
        }

        // Fetch asset definition.
        AssetDefinition& foundAssetDefinition = assetDeclaredVisitor.getAssetDefinition();

        // Did we get an asset name?
        if ( foundAssetDefinition.mAssetName == StringTable->EmptyString )
        {
            // No, so warn.
            Con::warnf( "Asset Manager: Parsed file '%s' but did not encounter an asset.", assetFileBuffer );
            continue;
        }

        // Set module definition.
        foundAssetDefinition.mpModuleDefinition = pModuleDefinition;

        // Format asset Id.
        char assetIdBuffer[1024];
        dSprintf(assetIdBuffer, sizeof(assetIdBuffer), "%s%s%s",
            pModuleDefinition->getModuleId(),
            ASSET_SCOPE_SEPARATOR,
            foundAssetDefinition.mAssetName );

        // Set asset Id.
        foundAssetDefinition.mAssetId = StringTable->insert( assetIdBuffer );

        // Does this asset already exist?
        if ( mDeclaredAssets.contains( foundAssetDefinition.mAssetId ) )
        {
            // Yes, so warn.
            Con::warnf( "Asset Manager: Encountered asset Id '%s' in asset file '%s' but it conflicts with existing asset Id in asset file '%s'.",
                foundAssetDefinition.mAssetId,
                foundAssetDefinition.mAssetBaseFilePath,
                mDeclaredAssets.find( foundAssetDefinition.mAssetId )->value->mAssetBaseFilePath );

            continue;
        }

        // Create new asset definition.
        AssetDefinition* pAssetDefinition = new AssetDefinition( foundAssetDefinition );

        // Store in declared assets.
        mDeclaredAssets.insert( pAssetDefinition->mAssetId, pAssetDefinition );

        // Store in module assets.
        moduleAssets.push_back( pAssetDefinition );
        
        // Info.
        if ( mEchoInfo )
        {
            Con::printSeparator();
            Con::printf( "Asset Manager: Adding Asset Id '%s' of type '%s' in asset file '%s'.",
                pAssetDefinition->mAssetId,
                pAssetDefinition->mAssetType,
                pAssetDefinition->mAssetBaseFilePath );
        }

        // Fetch asset Id.
        StringTableEntry assetId = pAssetDefinition->mAssetId;

        // Fetch asset dependencies.
        TamlAssetDeclaredVisitor::typeAssetIdVector& assetDependencies = assetDeclaredVisitor.getAssetDependencies();

        // Are there any asset dependencies?
        if ( assetDependencies.size() > 0 )
        {
            // Yes, so iterate dependencies.
            for( TamlAssetDeclaredVisitor::typeAssetIdVector::iterator assetDependencyItr = assetDependencies.begin(); assetDependencyItr != assetDependencies.end(); ++assetDependencyItr )
            {
                // Fetch asset Ids.
                StringTableEntry dependencyAssetId = *assetDependencyItr;

                // Insert depends-on.
                mAssetDependsOn.insertEqual( assetId, dependencyAssetId );

                // Insert is-depended-on.
                mAssetIsDependedOn.insertEqual( dependencyAssetId, assetId );

                // Info.
                if ( mEchoInfo )
                {
                    Con::printf( "Asset Manager: Asset Id '%s' has dependency of Asset Id '%s'", assetId, dependencyAssetId );
                }
            }
        }

        // Fetch asset loose files.
        TamlAssetDeclaredVisitor::typeLooseFileVector& assetLooseFiles = assetDeclaredVisitor.getAssetLooseFiles();

        // Are there any loose files?
        if ( assetLooseFiles.size() > 0 )
        {
            // Yes, so iterate loose files.
            for( TamlAssetDeclaredVisitor::typeLooseFileVector::iterator assetLooseFileItr = assetLooseFiles.begin(); assetLooseFileItr != assetLooseFiles.end(); ++assetLooseFileItr )
            {
                // Fetch loose file.
                StringTableEntry looseFile = *assetLooseFileItr;

                // Info.
                if ( mEchoInfo )
                {
                    Con::printf( "Asset Manager: Asset Id '%s' has loose file '%s'.", assetId, looseFile );
                }

                // Store loose file.
                pAssetDefinition->mAssetLooseFiles.push_back( looseFile );
            }
        }
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: ... Finished scanning for declared assets in path '%s' for files with extension '%s'.", pathBuffer, pExtension );
        Con::printSeparator();
        Con::printBlankLine();
    }

    return true;
}

//-----------------------------------------------------------------------------

bool AssetManager::scanReferencedAssets( const char* pPath, const char* pExtension )
{
    // Sanity!
    AssertFatal( pPath != NULL, "Cannot scan referenced assets with NULL path." );
    AssertFatal( pExtension != NULL, "Cannot scan referenced assets with NULL extension." );

    // Expand path location.
    char pathBuffer[1024];
    Con::expandPath( pathBuffer, sizeof(pathBuffer), pPath );

    // Find files.
    Vector<Platform::FileInfo> files;
    if ( !Platform::dumpPath( pathBuffer, files, 0 ) )
    {
        // Failed so warn.
        Con::warnf( "Asset Manager: Failed to scan referenced assets in directory '%s'.", pathBuffer );
        return false;
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printSeparator();
        Con::printf( "Asset Manager: Scanning for referenced assets in path '%s' for files with extension '%s'...", pathBuffer, pExtension );
    }

    // Fetch extension length.
    const U32 extensionLength = dStrlen( pExtension );

    TamlAssetReferencedVisitor assetReferencedVisitor;

    // Iterate files.
    for ( Vector<Platform::FileInfo>::iterator fileItr = files.begin(); fileItr != files.end(); ++fileItr )
    {
        // Fetch file info.
        Platform::FileInfo& fileInfo = *fileItr;

        // Fetch filename.
        const char* pFilename = fileInfo.pFileName;

        // Find filename length.
        const U32 filenameLength = dStrlen( pFilename );

        // Skip if extension is longer than filename.
        if ( extensionLength > filenameLength )
            continue;

        // Skip if extension not found.
        if ( dStricmp( pFilename + filenameLength - extensionLength, pExtension ) != 0 )
            continue;

        // Clear referenced assets.
        assetReferencedVisitor.clear();

        // Format full file-path.
        char assetFileBuffer[1024];
        dSprintf( assetFileBuffer, sizeof(assetFileBuffer), "%s/%s", fileInfo.pFullPath, fileInfo.pFileName );

        // Format reference file-path.
        typeReferenceFilePath referenceFilePath = StringTable->insert( assetFileBuffer );

        // Parse the filename.
        if ( !assetReferencedVisitor.parse( referenceFilePath ) )
        {
            // Warn.
            Con::warnf( "Asset Manager: Failed to parse file containing asset references: '%s'.", referenceFilePath );
            continue;
        }

        // Fetch usage map.
        const TamlAssetReferencedVisitor::typeAssetReferencedHash& assetReferencedMap = assetReferencedVisitor.getAssetReferencedMap();

        // Do we have any asset references?
        if ( assetReferencedMap.size() > 0 )
        {
            // Info.
            if ( mEchoInfo )
            {
                Con::printSeparator();
            }

            // Iterate usage.
            for( TamlAssetReferencedVisitor::typeAssetReferencedHash::const_iterator usageItr = assetReferencedMap.begin(); usageItr != assetReferencedMap.end(); ++usageItr )
            {
                // Fetch asset name.
                typeAssetId assetId = usageItr->key;

                // Info.
                if ( mEchoInfo )
                {
                    Con::printf( "Asset Manager: Found referenced Asset Id '%s' in file '%s'.", assetId, referenceFilePath );
                }

                // Add referenced asset.
                addReferencedAsset( assetId, referenceFilePath );
            }
        }
    }

    // Info.
    if ( mEchoInfo )
    {
        Con::printf( "Asset Manager: ... Finished scanning for referenced assets in path '%s' for files with extension '%s'.", pathBuffer, pExtension );
        Con::printSeparator();
        Con::printBlankLine();
    }

    return true;
}

//-----------------------------------------------------------------------------

AssetDefinition* AssetManager::findAsset( const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot find NULL asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Find declared asset.
    typeDeclaredAssetsHash::iterator declaredAssetItr = mDeclaredAssets.find( assetId );

    // Find if we didn't find a declared asset Id.
    if ( declaredAssetItr == mDeclaredAssets.end() )
        return NULL;

    return declaredAssetItr->value;
}

//-----------------------------------------------------------------------------

void AssetManager::addReferencedAsset( StringTableEntry assetId, StringTableEntry referenceFilePath )
{
    // Sanity!
    AssertFatal( assetId != NULL, "Cannot add referenced asset with NULL asset Id." );
    AssertFatal( referenceFilePath != NULL, "Cannot add referenced asset with NULL reference file-path." );

    // Find referenced asset.
    typeReferencedAssetsHash::iterator referencedAssetItr = mReferencedAssets.find( assetId );

    // Did we find the asset?
    if ( referencedAssetItr == mReferencedAssets.end() )
    {
        // No, so add asset Id.
        mReferencedAssets.insertEqual( assetId, referenceFilePath );
    }
    else
    {
        // Yes, so add asset Id with a unique file.
        while( true )
        {
            // Finish if this file is already present.
            if ( referencedAssetItr->value == referenceFilePath )
                return;

            // Move to next asset Id.
            referencedAssetItr++;

            // Is this the end of referenced assets or a different asset Id?
            if ( referencedAssetItr == mReferencedAssets.end() ||
                referencedAssetItr->key != assetId )
            {
                // Yes, so add asset reference.
                mReferencedAssets.insertEqual( assetId, referenceFilePath );
                return;
            }
        };
    }
}

//-----------------------------------------------------------------------------

void AssetManager::renameAssetReferences( StringTableEntry assetIdFrom, StringTableEntry assetIdTo )
{
    // Sanity!
    AssertFatal( assetIdFrom != NULL, "Cannot rename asset references using NULL asset Id from." );
    AssertFatal( assetIdTo != NULL, "Cannot rename asset references using NULL asset Id to." );

    // Finish if the asset is not referenced.
    if ( !mReferencedAssets.count( assetIdFrom ) )
        return;

    // Setup referenced update visitor.
    TamlAssetReferencedUpdateVisitor assetReferencedUpdateVisitor;
    assetReferencedUpdateVisitor.setAssetIdFrom( assetIdFrom );
    assetReferencedUpdateVisitor.setAssetIdTo( assetIdTo );

    // Find first referenced asset Id.
    typeReferencedAssetsHash::iterator referencedAssetItr = mReferencedAssets.find( assetIdFrom );

    // Iterate references.
    while( true )
    {
        // Finish if end of references.
        if ( referencedAssetItr == mReferencedAssets.end() || referencedAssetItr->key != assetIdFrom )
            return;

        // Info.
        if ( mEchoInfo )
        {
            Con::printf( "Asset Manager: Renaming declared Asset Id '%s' to Asset Id '%s'.  Updating referenced file '%s'",
                assetIdFrom,
                assetIdTo,
                referencedAssetItr->value );
        }

        // Update asset file declaration.
        if ( !assetReferencedUpdateVisitor.parse( referencedAssetItr->value ) )
        {
            // No, so warn.
            Con::warnf("Asset Manager: Cannot rename referenced asset Id '%s' to asset Id '%s' as the referenced asset file could not be parsed: %s",
                assetIdFrom, assetIdTo, referencedAssetItr->value );
        }

        // Move to next reference.
        referencedAssetItr++;
    }
}

//-----------------------------------------------------------------------------

void AssetManager::removeAssetReferences( StringTableEntry assetId )
{
    // Sanity!
    AssertFatal( assetId != NULL, "Cannot rename asset references using NULL asset Id." );

    // Finish if the asset is not referenced.
    if ( !mReferencedAssets.count( assetId ) )
        return;

    // Setup referenced update visitor.
    TamlAssetReferencedUpdateVisitor assetReferencedUpdateVisitor;
    assetReferencedUpdateVisitor.setAssetIdFrom( assetId );
    assetReferencedUpdateVisitor.setAssetIdTo( StringTable->EmptyString );

    // Find first referenced asset Id.
    typeReferencedAssetsHash::iterator referencedAssetItr = mReferencedAssets.find( assetId );

    // Iterate references.
    while( true )
    {
        // Finish if end of references.
        if ( referencedAssetItr == mReferencedAssets.end() || referencedAssetItr->key != assetId )
            break;

        // Info.
        if ( mEchoInfo )
        {
            Con::printf( "Asset Manager: Removing Asset Id '%s' references from file '%s'",
                assetId,
                referencedAssetItr->value );
        }

        // Update asset file declaration.
        if ( !assetReferencedUpdateVisitor.parse( referencedAssetItr->value ) )
        {
            // No, so warn.
            Con::warnf("Asset Manager: Cannot remove referenced asset Id '%s' as the referenced asset file could not be parsed: %s",
                assetId,
                referencedAssetItr->value );
        }

        // Move to next reference.
        referencedAssetItr++;
    }

    // Remove asset references.
    mReferencedAssets.erase( assetId );
}

//-----------------------------------------------------------------------------

void AssetManager::renameAssetDependencies( StringTableEntry assetIdFrom, StringTableEntry assetIdTo )
{
    // Sanity!
    AssertFatal( assetIdFrom != NULL, "Cannot rename asset dependencies using NULL asset Id from." );
    AssertFatal( assetIdTo != NULL, "Cannot rename asset dependencies using NULL asset Id to." );

    // Rename via depends-on...
    while( mAssetDependsOn.count( assetIdFrom ) > 0 )
    {
        // Find depends-on.
        typeAssetDependsOnHash::iterator dependsOnItr = mAssetDependsOn.find( assetIdFrom );

        // Fetch dependency asset Id.
        StringTableEntry dependencyAssetId = dependsOnItr->value;

        // Find is-depends-on entry.
        typeAssetIsDependedOnHash::iterator isDependedOnItr = mAssetIsDependedOn.find( dependencyAssetId );

        // Sanity!
        AssertFatal( isDependedOnItr != mAssetIsDependedOn.end(), "Asset dependencies are corrupt!" );

        while( isDependedOnItr != mAssetIsDependedOn.end() && isDependedOnItr->key == dependencyAssetId && isDependedOnItr->value != assetIdFrom )
        {
            isDependedOnItr++;
        }

        // Sanity!
        AssertFatal( isDependedOnItr->key == dependencyAssetId && isDependedOnItr->value == assetIdFrom, "Asset dependencies are corrupt!" );
        
        // Remove is-depended-on.        
        mAssetIsDependedOn.erase( isDependedOnItr );

        // Remove depends-on.
        mAssetDependsOn.erase( dependsOnItr );

        // Insert depends-on.
        mAssetDependsOn.insertEqual( assetIdTo, dependencyAssetId );

        // Insert is-depended-on.
        mAssetIsDependedOn.insertEqual( dependencyAssetId, assetIdTo );
    }

    // Rename via is-depended-on...
    while( mAssetIsDependedOn.count( assetIdFrom ) > 0 )
    {
        // Find is-depended-on.
        typeAssetIsDependedOnHash::iterator isdependedOnItr = mAssetIsDependedOn.find( assetIdFrom );

        // Fetch dependency asset Id.
        StringTableEntry dependencyAssetId = isdependedOnItr->value;

        // Find depends-on entry.
        typeAssetDependsOnHash::iterator dependsOnItr = mAssetDependsOn.find( dependencyAssetId );

        // Sanity!
        AssertFatal( dependsOnItr != mAssetDependsOn.end(), "Asset dependencies are corrupt!" );

        while( dependsOnItr != mAssetDependsOn.end() && dependsOnItr->key == dependencyAssetId && dependsOnItr->value != assetIdFrom )
        {
            dependsOnItr++;
        }

        // Sanity!
        AssertFatal( dependsOnItr->key == dependencyAssetId && dependsOnItr->value == assetIdFrom, "Asset dependencies are corrupt!" );
        
        // Remove is-depended-on.        
        mAssetIsDependedOn.erase( isdependedOnItr );

        // Remove depends-on.
        mAssetDependsOn.erase( dependsOnItr );

        // Insert depends-on.
        mAssetDependsOn.insertEqual( dependencyAssetId, assetIdTo );

        // Insert is-depended-on.
        mAssetIsDependedOn.insertEqual( assetIdTo, dependencyAssetId );
    }
}

//-----------------------------------------------------------------------------

void AssetManager::removeAssetDependencies( const char* pAssetId )
{
    // Sanity!
    AssertFatal( pAssetId != NULL, "Cannot remove asset dependencies using NULL asset Id." );

    // Fetch asset Id.
    StringTableEntry assetId = StringTable->insert( pAssetId );

    // Remove from depends-on assets.
    while( mAssetDependsOn.count( assetId ) > 0 )
    {
        // Find depends-on.
        typeAssetDependsOnHash::iterator dependsOnItr = mAssetDependsOn.find( assetId );

        // Fetch dependency asset Id.
        StringTableEntry dependencyAssetId = dependsOnItr->value;

        // Find is-depends-on entry.
        typeAssetIsDependedOnHash::iterator isDependedOnItr = mAssetIsDependedOn.find( dependencyAssetId );

        // Sanity!
        AssertFatal( isDependedOnItr != mAssetIsDependedOn.end(), "Asset dependencies are corrupt!" );

        while( isDependedOnItr != mAssetIsDependedOn.end() && isDependedOnItr->key == dependencyAssetId && isDependedOnItr->value != assetId )
        {
            isDependedOnItr++;
        }

        // Sanity!
        AssertFatal( isDependedOnItr->key == dependencyAssetId && isDependedOnItr->value == assetId, "Asset dependencies are corrupt!" );

        // Remove is-depended-on.        
        mAssetIsDependedOn.erase( isDependedOnItr );

        // Remove depends-on.
        mAssetDependsOn.erase( dependsOnItr );
    }
}

//-----------------------------------------------------------------------------

void AssetManager::unloadAsset( AssetDefinition* pAssetDefinition )
{
    // Destroy the asset.
    pAssetDefinition->mpAssetBase->deleteObject();

    // Increase unloaded count.
    pAssetDefinition->mAssetUnloadedCount++;

    // Is the asset internal?
    if ( pAssetDefinition->mAssetInternal )
    {
        // Yes, so decrease internal loaded asset count.
        mLoadedInternalAssetsCount--;
    }
    else
    {
        // No, so decrease external loaded assets count.
        mLoadedExternalAssetsCount--;
    }

    // Is the asset private.
    if ( pAssetDefinition->mAssetPrivate )
    {
        // Yes, so decrease private loaded asset count.
        mLoadedPrivateAssetsCount--;

        // Remove it completely.
        removeSingleDeclaredAsset( pAssetDefinition->mAssetId );
    }
}

//-----------------------------------------------------------------------------

void AssetManager::onModulePreLoad( ModuleDefinition* pModuleDefinition )
{
    // Is a declared asset manifest specified?
    if ( pModuleDefinition->getDeclaredAssetManifest() != StringTable->EmptyString )
    {
        // Yes, so add declared assets.
        addDeclaredAssets( pModuleDefinition );
    }

    // Is an asset tags manifest specified?
    if ( pModuleDefinition->getAssetTagsManifest() != StringTable->EmptyString )
    {
        // Yes, so load the asset tags manifest.
        loadAssetTags( pModuleDefinition );
    }
}

//-----------------------------------------------------------------------------

void AssetManager::onModulePreUnload( ModuleDefinition* pModuleDefinition )
{
    // Is an asset tags manifest specified?
    if ( pModuleDefinition->getAssetTagsManifest() != StringTable->EmptyString )
    {
        // Yes, so save the asset tags manifest.
        saveAssetTags();

        // Do we have an asset tags manifest?
        if ( !mAssetTagsManifest.isNull() )
        {
            // Yes, so remove it.
            mAssetTagsManifest->deleteObject();
            mAssetTagsModuleDefinition = NULL;
        }
    }
}

//-----------------------------------------------------------------------------

void AssetManager::onModulePostUnload( ModuleDefinition* pModuleDefinition )
{
    // Is a declared asset manifest specified?
    if ( pModuleDefinition->getDeclaredAssetManifest() != StringTable->EmptyString )
    {
        // Yes, so remove declared assets.
        removeDeclaredAssets( pModuleDefinition );
    }
}
