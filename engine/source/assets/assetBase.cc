//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_BASE_H
#include "assetBase.h"
#endif

#ifndef _ASSET_MANAGER_H
#include "assetManager.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

// Script bindings.
#include "assetBase_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( AssetBase );

//-----------------------------------------------------------------------------

AssetBase::AssetBase() :
    mAcquireReferenceCount( 0 ),
    mpOwningAssetManager( NULL ),
    mAssetInitialized( false )
{
    // Generate an asset definition.
    mpAssetDefinition = new AssetDefinition();
}

//-----------------------------------------------------------------------------

AssetBase::~AssetBase()
{
    // If the asset manager does not own the asset then we own the
    // asset definition so delete it.
    if ( !getOwned() )
        delete mpAssetDefinition;
}

//-----------------------------------------------------------------------------

void AssetBase::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    // Asset configuration.
    addProtectedField( ASSET_BASE_ASSETNAME_FIELD, TypeString, 0, &setAssetName, &getAssetName, &writeAssetName, "The name of the asset.  The is not a unique identification like an asset Id." );
    addProtectedField( ASSET_BASE_ASSETDESCRIPTION_FIELD, TypeString, 0, &setAssetDescription, &getAssetDescription, &writeAssetDescription, "The simple description of the asset contents." );
    addProtectedField( ASSET_BASE_CATEGORY_FIELD, TypeString, 0, &setAssetCategory, &getAssetCategory, &writeAssetCategory, "An arbitrary category that can be used to categorized assets." );
    addProtectedField( ASSET_BASE_AUTOUNLOAD_FIELD, TypeBool, 0, &setAssetAutoUnload, &getAssetAutoUnload, &writeAssetAutoUnload, "Whether the asset is automatically unloaded when an asset is released and has no other acquisitions or not." );
    addProtectedField( ASSET_BASE_ASSETINTERNAL_FIELD, TypeBool, 0, &setAssetInternal, &getAssetInternal, &writeAssetInternal, "Whether the asset is used internally only or not." );
    addProtectedField( ASSET_BASE_ASSETPRIVATE_FIELD, TypeBool, 0, &defaultProtectedNotSetFn, &getAssetPrivate, &defaultProtectedNotWriteFn, "Whether the asset is private or not." );
}

//-----------------------------------------------------------------------------

void AssetBase::setAssetDescription( const char* pAssetDescription )
{
    // Fetch asset description.
    StringTableEntry assetDescription = StringTable->insert(pAssetDescription);

    // Ignore no change.
    if ( mpAssetDefinition->mAssetDescription == assetDescription )
        return;

    // Update.
    mpAssetDefinition->mAssetDescription = assetDescription;

    // Refresh the asset.
    refreshAsset();
}

//-----------------------------------------------------------------------------

void AssetBase::setAssetCategory( const char* pAssetCategory )
{
    // Fetch asset category.
    StringTableEntry assetCategory = StringTable->insert(pAssetCategory);

    // Ignore no change.
    if ( mpAssetDefinition->mAssetCategory == assetCategory )
        return;

    // Update.
    mpAssetDefinition->mAssetCategory = assetCategory;

    // Refresh the asset.
    refreshAsset();
}

//-----------------------------------------------------------------------------

void AssetBase::setAssetAutoUnload( const bool autoUnload )
{
    // Ignore no change.
    if ( mpAssetDefinition->mAssetAutoUnload == autoUnload )
        return;

    // Update.
    mpAssetDefinition->mAssetAutoUnload = autoUnload;

    // Refresh the asset.
    refreshAsset();
}

//-----------------------------------------------------------------------------

void AssetBase::setAssetInternal( const bool assetInternal )
{
    // Ignore no change,
    if ( mpAssetDefinition->mAssetInternal == assetInternal )
        return;

    // Update.
    mpAssetDefinition->mAssetInternal = assetInternal;

    // Refresh the asset.
    refreshAsset();
}

//-----------------------------------------------------------------------------

StringTableEntry AssetBase::expandAssetFilePath( const char* pAssetFilePath ) const
{
    // Sanity!
    AssertFatal( pAssetFilePath != NULL, "Cannot expand a NULL asset path." );

    // Fetch asset file-path length.
    const U32 assetFilePathLength = dStrlen(pAssetFilePath);

    // Are there any characters in the path?
    if ( assetFilePathLength == 0 )
    {
        // No, so return empty.
        return StringTable->EmptyString;
    }

    char assetFilePathBuffer[1024];

    // Is the asset file-path an asset-relative path?
    if ( *pAssetFilePath != '#' )
    {
        // No, so expand the path in the usual way.
        Con::expandPath( assetFilePathBuffer, sizeof(assetFilePathBuffer), pAssetFilePath );
        return StringTable->insert( assetFilePathBuffer );
    }

    // Is the asset owned
    if ( !mpOwningAssetManager )
    {
        // No, so warn.
        Con::warnf( "AssetBase: Cannot expand relative asset path on asset that is not owned which will potentially cause an incorrect path: '%s'", pAssetFilePath );

        // Return the original file-path.
        // NOTE: Although the file-path is unchanged here, it's likely the caller might expand it again causing an incorrect relative path to be used (relative to the code-path).
        return StringTable->insert( pAssetFilePath );
    }

    // Yes, so is the asset file-path the correct length.
    if ( dStrlen(pAssetFilePath) == 1 )
    {
        // No, so warn.
        Con::warnf( "AssetBase: Cannot expand relative asset path as it is an invalid length: '%s'", pAssetFilePath );

        // Return the original file-path.
        // NOTE: Although the file-path is unchanged here, it's likely the caller might expand it again causing an incorrect relative path to be used (relative to the code-path).
        return StringTable->insert( pAssetFilePath );
    }

    // Format expanded path taking into account any missing slash.
    dSprintf( assetFilePathBuffer, sizeof(assetFilePathBuffer), "%s/%s", mpOwningAssetManager->getAssetPath( getAssetId() ), pAssetFilePath + (pAssetFilePath[1] == '/' ? 2 : 1 ) );

    return StringTable->insert( assetFilePathBuffer );       
}

//-----------------------------------------------------------------------------

StringTableEntry AssetBase::collapseAssetFilePath( const char* pAssetFilePath ) const
{
    // Sanity!
    AssertFatal( pAssetFilePath != NULL, "Cannot collapse a NULL asset path." );

    // Fetch asset file-path length.
    const U32 assetFilePathLength = dStrlen(pAssetFilePath);

    // Are there any characters in the path?
    if ( assetFilePathLength == 0 )
    {
        // No, so return empty.
        return StringTable->EmptyString;
    }

    char assetFilePathBuffer[1024];

    // Is the asset file-path already an asset-relative path?
    if ( *pAssetFilePath == '#' )
    {
        // Yes, so assume it's already collapsed.
        return StringTable->insert( assetFilePathBuffer );
    }

    // Is the asset owned
    if ( !mpOwningAssetManager )
    {
        // No, so we can only collapse the path using the platform layer.
        Con::collapsePath( assetFilePathBuffer, sizeof(assetFilePathBuffer), pAssetFilePath );
        return StringTable->insert( assetFilePathBuffer );
    }

    // Fetch asset base-path.
    StringTableEntry assetBasePath = mpOwningAssetManager->getAssetPath( getAssetId() );

    // Is the asset file-path location within the asset base-path?
    if ( Con::isBasePath( pAssetFilePath, assetBasePath ) )
    {
        // Yes, so fetch path relative to the asset base-path.
        StringTableEntry relativePath = Platform::makeRelativePathName( pAssetFilePath, assetBasePath );

        // Format the collapsed path.
        dSprintf( assetFilePathBuffer, sizeof(assetFilePathBuffer), "#%s", relativePath );
    }
    else
    {
        // No, so we can collapse the path using the platform layer.
        Con::collapsePath( assetFilePathBuffer, sizeof(assetFilePathBuffer), pAssetFilePath );
    }

    return StringTable->insert( assetFilePathBuffer );       
}

//-----------------------------------------------------------------------------

void AssetBase::reloadAsset( void )
{
    // Finish if asset is not owned or is not initialized.
    if ( mpOwningAssetManager == NULL || !mAssetInitialized )
        return;

    // Yes, so reload the asset via the asset manager.
    mpOwningAssetManager->reloadAsset( getAssetId() );
}

//-----------------------------------------------------------------------------

void AssetBase::refreshAsset( void )
{
    // Finish if asset is not owned or is not initialized.
    if ( mpOwningAssetManager == NULL || !mAssetInitialized )
        return;

    // Yes, so refresh the asset via the asset manager.
    mpOwningAssetManager->refreshAsset( getAssetId() );
}

//-----------------------------------------------------------------------------

bool AssetBase::releaseAssetReference( void )
{
    // Are there any acquisition references?
    if ( mAcquireReferenceCount == 0 )
    {
        // No, so warn.
        Con::warnf( "AssetBase: Cannot release asset reference as there are no current acquisitions." );

        // Return "unload" unless auto unload is off.
        return mpAssetDefinition->mAssetAutoUnload;
    }

    // Release reference.
    mAcquireReferenceCount--;

    // Are there any acquisition references?
    if ( mAcquireReferenceCount == 0 )
    {
        // No, so return "unload" unless auto unload is off.
        return mpAssetDefinition->mAssetAutoUnload;
    }

    // Return "don't unload".
    return false;
}

//-----------------------------------------------------------------------------

void AssetBase::setOwned( AssetManager* pAssetManager, AssetDefinition* pAssetDefinition )
{  
    // Sanity!
    AssertFatal( pAssetManager != NULL, "Cannot set asset ownership with NULL asset manager." );
    AssertFatal( mpOwningAssetManager == NULL, "Cannot set asset ownership if it is already owned." );
    AssertFatal( pAssetDefinition != NULL, "Cannot set asset ownership with a NULL asset definition." );
    AssertFatal( mpAssetDefinition != NULL, "Asset ownership assigned but has a NULL asset definition." );
    AssertFatal( mpAssetDefinition->mAssetName == pAssetDefinition->mAssetName, "Asset ownership differs by asset name." );
    AssertFatal( mpAssetDefinition->mAssetDescription == pAssetDefinition->mAssetDescription, "Asset ownership differs by asset description." );
    AssertFatal( mpAssetDefinition->mAssetCategory == pAssetDefinition->mAssetCategory, "Asset ownership differs by asset category." );
    AssertFatal( mpAssetDefinition->mAssetAutoUnload == pAssetDefinition->mAssetAutoUnload, "Asset ownership differs by asset auto-unload flag." );
    AssertFatal( mpAssetDefinition->mAssetInternal == pAssetDefinition->mAssetInternal, "Asset ownership differs by asset internal flag." );

    // Transfer asset definition ownership state.
    delete mpAssetDefinition;
    mpAssetDefinition = pAssetDefinition;

    // Flag as owned.
    // NOTE: This must be done prior to initializing the asset so any initialization can assume ownership.
    mpOwningAssetManager = pAssetManager;

    // Initialize the asset.
    initializeAsset();

    // Flag asset as initialized.
    mAssetInitialized = true;
}
