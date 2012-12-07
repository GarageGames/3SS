//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_MANIFEST_H
#include "assetManifest.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _TAML_COLLECTION_H_
#include "persistence/taml/tamlCollection.h"
#endif

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( AssetManifest );

//-----------------------------------------------------------------------------

void AssetManifest::onTamlCustomWrite( TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomWrite( customCollection );

    // Add property.
    TamlCollectionProperty* pProperty = customCollection.addCollectionProperty( ASSET_COLLECTION_NAME );

    // Finish if no asset locations.
    if ( mAssetLocations.size() == 0 )
        return;

    // Iterate asset locations.
    for( typeAssetLocationVector::iterator locationItr = mAssetLocations.begin(); locationItr != mAssetLocations.end(); ++locationItr )
    {
        // Add type alias.
        TamlPropertyTypeAlias* pTypeAlias = pProperty->addTypeAlias( ASSET_TYPE_NAME );

        // Add fields.
        pTypeAlias->addPropertyField( ASSET_PATH_FIELD_NAME, locationItr->mPath );
        pTypeAlias->addPropertyField( ASSET_EXTENSION_FIELD_NAME, locationItr->mExtension );
    }
}

//-----------------------------------------------------------------------------

void AssetManifest::onTamlCustomRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomRead( customCollection );

    // Find collection property name.
    const TamlCollectionProperty* pProperty = customCollection.findProperty( ASSET_COLLECTION_NAME );

    // Finish if we don't have a property name.
    if ( pProperty == NULL )
        return;

    // Fetch type alias name.
    StringTableEntry typeAliasName = StringTable->insert( ASSET_TYPE_NAME );

    // Iterate property type alias.
    for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pProperty->begin(); propertyTypeAliasItr != pProperty->end(); ++propertyTypeAliasItr )
    {
        // Fetch property type alias.
        const TamlPropertyTypeAlias* pTypeAlias = *propertyTypeAliasItr;

        // Skip if an unknown alias name.
        if ( pTypeAlias->mAliasName != typeAliasName )
            continue;

        // Fetch "path" field.
        const TamlPropertyField* pPathField = pTypeAlias->findField( ASSET_PATH_FIELD_NAME );

        // Do we find the field?
        if ( pPathField == NULL )
        {
            // No, so warn.
            Con::warnf( "AssetManifest::onTamlCustomRead() - Could not find '%s' field.", ASSET_PATH_FIELD_NAME );
            continue;
        }

        // Fetch "extension" field.
        const TamlPropertyField* pExtensionField = pTypeAlias->findField( ASSET_EXTENSION_FIELD_NAME );

        // Do we find the field?
        if ( pExtensionField == NULL )
        {
            // No, so warn.
            Con::warnf( "AssetManifest::onTamlCustomRead() - Could not find '%s' field.", ASSET_EXTENSION_FIELD_NAME );
            continue;
        }
        
        // Fetch "recurse" field.
        const TamlPropertyField* pRecurseField = pTypeAlias->findField( ASSET_RECURSE_FIELD_NAME );
        
        // Fetch recurse flag.
        bool recurse = false;
        if ( pRecurseField != NULL )
            pRecurseField->getFieldValue( recurse );

        // Store used asset location.
        AssetLocation assetLocation( pPathField->getFieldValue(), pExtensionField->getFieldValue(), recurse );
        mAssetLocations.push_back( assetLocation );
    }
}