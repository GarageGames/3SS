//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlBinaryReader.h"

#ifndef _ZIPSUBSTREAM_H_
#include "io/zip/zipSubStream.h"
#endif

//-----------------------------------------------------------------------------

SimObject* TamlBinaryReader::read( FileStream& stream )
{
    // Sanity!
    AssertFatal( mpTaml->getFormatMode() == Taml::BinaryFormat, "Cannot read with a binary reader using a non-binary format mode." );

    // Read Taml signature.
    StringTableEntry tamlSignature = stream.readSTString();

    // Is the signature correct?
    if ( tamlSignature != StringTable->insert( TAML_SIGNATURE ) )
    {
        // Warn.
        Con::warnf("Taml: Cannot read binary file as signature is incorrect '%s'.", tamlSignature );
        return NULL;
    }

    // Read version Id.
    U32 versionId;
    stream.read( &versionId );

    // Read compressed flag.
    bool compressed;
    stream.read( &compressed );

    SimObject* pSimObject = NULL;

    // Is the stream compressed?
    if ( compressed )
    {
        // Yes, so attach zip stream.
        ZipSubRStream zipStream;
        zipStream.attachStream( &stream );

        // Parse element.
        pSimObject = parseElement( zipStream, versionId );

        // Detach zip stream.
        zipStream.detachStream();
    }
    else
    {
        // No, so parse element.
        pSimObject = parseElement( stream, versionId );
    }

    return pSimObject;
}

//-----------------------------------------------------------------------------

void TamlBinaryReader::resetParse( void )
{
    // Clear object reference map.
    mObjectReferenceMap.clear();
}

//-----------------------------------------------------------------------------

SimObject* TamlBinaryReader::parseElement( Stream& stream, const U32 versionId )
{
    SimObject* pSimObject = NULL;

    // Fetch element name.    
    StringTableEntry typeName = stream.readSTString();

    // Fetch object name.
    StringTableEntry objectName = stream.readSTString();

    // Read references.
    U32 tamlRefId;
    U32 tamlRefToId;
    stream.read( &tamlRefId );
    stream.read( &tamlRefToId );

    // Do we have a reference to Id?
    if ( tamlRefToId != 0 )
    {
        // Yes, so fetch reference.
        typeObjectReferenceHash::iterator referenceItr = mObjectReferenceMap.find( tamlRefToId );

        // Did we find the reference?
        if ( referenceItr == mObjectReferenceMap.end() )
        {
            // No, so warn.
            Con::warnf( "Taml: Could not find a reference Id of '%d'", tamlRefToId );
            return NULL;
        }

        // Return object.
        return referenceItr->value;
    }

    // Create type.
    pSimObject = Taml::createType( typeName );

    // Finish if we couldn't create the type.
    if ( pSimObject == NULL )
        return NULL;

    // Find Taml callbacks.
    TamlCallbacks* pCallbacks = dynamic_cast<TamlCallbacks*>( pSimObject );

    // Are there any Taml callbacks?
    if ( pCallbacks != NULL )
    {
        // Yes, so call it.
        mpTaml->tamlPreRead( pCallbacks );
    }

    // Parse attributes.
    parseAttributes( stream, pSimObject, versionId );

    // Register the object (name) appropriately.
    objectName == StringTable->EmptyString ? pSimObject->registerObject() : pSimObject->registerObject( objectName );

    // Do we have a reference Id?
    if ( tamlRefId != 0 )
    {
        // Yes, so insert reference.
        mObjectReferenceMap.insert( tamlRefId, pSimObject );
    }

    // Parse children.
    parseChildren( stream, pCallbacks, pSimObject, versionId );

    // Parse custom elements.
    TamlCollection customCollection;
    parseCustomElement( stream, pCallbacks, customCollection, versionId );

    // Are there any Taml callbacks?
    if ( pCallbacks != NULL )
    {
        // Yes, so call it.
        mpTaml->tamlPostRead( pCallbacks, customCollection );
    }

    // Return object.
    return pSimObject;
}

//-----------------------------------------------------------------------------

void TamlBinaryReader::parseCustomElement( Stream& stream, TamlCallbacks* pCallbacks, TamlCollection& customCollection, const U32 versionId )
{
    // Read custom element count.
    U32 customPropertyCount;
    stream.read( &customPropertyCount );

    // Finish if no custom properties.
    if ( customPropertyCount == 0 )
        return;

    // Iterate custom properties.
    for ( U32 propertyIndex = 0; propertyIndex < customPropertyCount; ++propertyIndex )
    {
        // Read custom element name.
        StringTableEntry propertyName = stream.readSTString();

        // Add collection property.
        TamlCollectionProperty* pCollectionProperty = customCollection.addCollectionProperty( propertyName );

        // Read property type alias count.
        U32 propertyTypeAliasCount;
        stream.read( &propertyTypeAliasCount );

        // Skip if no property type alias.
        if ( propertyTypeAliasCount == 0 )
            continue;

        // Iterate property type alias.
        for( U32 propertyTypeAliasIndex = 0; propertyTypeAliasIndex < propertyTypeAliasCount; ++propertyTypeAliasIndex )
        {
            // Read property type alias name.
            StringTableEntry propertyTypeAliasName = stream.readSTString();

            // Add property type alias.
            TamlPropertyTypeAlias* pPropertyTypeAlias = pCollectionProperty->addTypeAlias( propertyTypeAliasName );

            // Read property field count.
            U32 propertyFieldCount;
            stream.read( &propertyFieldCount );

            // Skip if no property fields.
            if ( propertyFieldCount == 0 )
                continue;

            // Iterate property fields.
            for( U32 propertyFieldIndex = 0; propertyFieldIndex < propertyFieldCount; ++propertyFieldIndex )
            {
                // Read is object field flag.
                bool isObjectField;
                stream.read( &isObjectField );

                // Is it an object field?
                if ( isObjectField )
                {
                    // Yes, so read reference field.
                    StringTableEntry fieldName = stream.readSTString();

                    // Read field object.
                    SimObject* pFieldObject = parseElement( stream, versionId );

                    // Add property field.
                    pPropertyTypeAlias->addPropertyField( fieldName, pFieldObject );
                }
                else
                {
                    // No, so read field name.
                    StringTableEntry propertyFieldName = stream.readSTString();

                    // Read field value.
                    char valueBuffer[MAX_TAML_PROPERTY_FIELDVALUE_LENGTH];
                    stream.readLongString( MAX_TAML_PROPERTY_FIELDVALUE_LENGTH, valueBuffer );

                    // Add property field.
                    pPropertyTypeAlias->addPropertyField( propertyFieldName, valueBuffer );
                }
            }
        }
    }

    // Do we have callbacks?
    if ( pCallbacks == NULL )
    {
        // No, so warn.
        Con::warnf( "Taml: Encountered custom data but object does not support custom data." );
        return;
    }

    // Custom read callback.
    mpTaml->tamlCustomRead( pCallbacks, customCollection );
}

//-----------------------------------------------------------------------------

void TamlBinaryReader::parseAttributes( Stream& stream, SimObject* pSimObject, const U32 versionId )
{
    // Sanity!
    AssertFatal( pSimObject != NULL, "Taml: Cannot parse attributes on a NULL object." );

    // Fetch attribute count.
    U32 attributeCount;
    stream.read( &attributeCount );

    // Finish if no attributes.
    if ( attributeCount == 0 )
        return;

    char valueBuffer[4096];

    // Iterate attributes.
    for ( U32 index = 0; index < attributeCount; ++index )
    {
        // Fetch attribute.
        StringTableEntry attributeName = stream.readSTString();
        stream.readLongString( 4096, valueBuffer );

        // We can assume this is a field for now.
        pSimObject->setPrefixedDataField( attributeName, NULL, valueBuffer );
    }
}

//-----------------------------------------------------------------------------

void TamlBinaryReader::parseChildren( Stream& stream, TamlCallbacks* pCallbacks, SimObject* pSimObject, const U32 versionId )
{
    // Sanity!
    AssertFatal( pSimObject != NULL, "Taml: Cannot parse children on a NULL object." );

    // Fetch children count.
    U32 childrenCount;
    stream.read( &childrenCount );

    // Finish if no children.
    if ( childrenCount == 0 )
        return;

    // Fetch the sim set.
    SimSet* pSimSet = dynamic_cast<SimSet*>( pSimObject );

    // Is this a sim set?
    if ( pSimSet == NULL )
    {
        // No, so warn.
        Con::warnf("Taml: Child element found under parent but object cannot have children." );
        return;
    }

    // Iterate children.
    for ( U32 index = 0; index < childrenCount; ++ index )
    {
        // Parse child element.
        SimObject* pChildSimObject = parseElement( stream, versionId );

        // Finish if child failed.
        if ( pChildSimObject == NULL )
            return;

        // Add child.
        pSimSet->addObject( pChildSimObject );

        // Perform callback.
        mpTaml->tamlAddParent( pCallbacks, pSimSet );
    }
}
