//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ASSET_QUERY_H
#include "assetQuery.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _TAML_COLLECTION_H_
#include "persistence/taml/tamlCollection.h"
#endif

// Script bindings.
#include "assetQuery_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( AssetQuery );

//-----------------------------------------------------------------------------

void AssetQuery::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    addProtectedField( "Count", TypeS32, 0, &defaultProtectedNotSetFn, &getCount, &writeCount, "Gets the number of results in the asset query." );
}

//-----------------------------------------------------------------------------

void AssetQuery::onTamlCustomWrite( TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomWrite( customCollection );

    // Add property.
    TamlCollectionProperty* pProperty = customCollection.addCollectionProperty( ASSETQUERY_COLLECTION_NAME );

    // Finish if no assets.
    if ( size() == 0 )
        return;

    // Iterate asset.
    for( Vector<StringTableEntry>::iterator assetItr = begin(); assetItr != end(); ++assetItr )
    {
        // Add type alias.
        TamlPropertyTypeAlias* pTypeAlias = pProperty->addTypeAlias( ASSETQUERY_TYPE_NAME );

        // Add fields.
        pTypeAlias->addPropertyField( ASSETQUERY_ASSETID_FIELD_NAME, *assetItr );
    }
}

//-----------------------------------------------------------------------------

void AssetQuery::onTamlCustomRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomRead( customCollection );

    // Find collection property name.
    const TamlCollectionProperty* pProperty = customCollection.findProperty( ASSETQUERY_COLLECTION_NAME );

    // Finish if we don't have a property name.
    if ( pProperty == NULL )
        return;

    // Fetch type alias name.
    StringTableEntry typeAliasName = StringTable->insert( ASSETQUERY_TYPE_NAME );

    // Iterate property type alias.
    for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pProperty->begin(); propertyTypeAliasItr != pProperty->end(); ++propertyTypeAliasItr )
    {
        // Fetch property type alias.
        const TamlPropertyTypeAlias* pTypeAlias = *propertyTypeAliasItr;

        // Skip if an unknown alias name.
        if ( pTypeAlias->mAliasName != typeAliasName )
            continue;

        // Fetch field.
        const TamlPropertyField* pField = pTypeAlias->findField( ASSETQUERY_ASSETID_FIELD_NAME );

        // Do we find the field?
        if ( pField == NULL )
        {
            // No, so warn.
            Con::warnf( "AssetQuery::onTamlCustomRead() - Could not find '%s' field.", ASSETQUERY_ASSETID_FIELD_NAME );
            continue;
        }

        // Store asset.
        push_back( pField->getFieldValue() );
    }
}