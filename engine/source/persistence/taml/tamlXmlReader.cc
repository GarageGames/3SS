//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlXmlReader.h"

//-----------------------------------------------------------------------------

SimObject* TamlXmlReader::read( FileStream& stream )
{
    // Sanity!
    AssertFatal( mpTaml->getFormatMode() == Taml::XmlFormat, "Cannot read with  a XML reader using a non-XML format mode." );

    // Create document.
    TiXmlDocument xmlDocument;

    // Load document from stream.
    if ( !xmlDocument.LoadFile( stream ) )
    {
        // Warn!
        Con::warnf("Taml: Could not load Taml XML file from stream.");
        return NULL;
    }

    // Parse root element.
    SimObject* pSimObject = parseElement( xmlDocument.RootElement() );

    // Reset parse.
    resetParse();

    return pSimObject;
}

//-----------------------------------------------------------------------------

void TamlXmlReader::resetParse( void )
{
    // Clear object reference map.
    mObjectReferenceMap.clear();
}

//-----------------------------------------------------------------------------

SimObject* TamlXmlReader::parseElement( TiXmlElement* pXmlElement )
{
    SimObject* pSimObject = NULL;

    // Fetch element name.
    StringTableEntry typeName = StringTable->insert( pXmlElement->Value() );

    // Fetch reference to Id.
    const U32 tamlRefToId = getTamlRefToId( pXmlElement );

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

    // No, so fetch reference Id.
    const U32 tamlRefId = getTamlRefId( pXmlElement );

    // Fetch object name.
    const char* pObjectName = getTamlObjectName( pXmlElement );

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
    parseAttributes( pXmlElement, pSimObject );

    // Register the object (name) appropriately.
    pObjectName == NULL ? pSimObject->registerObject() : pSimObject->registerObject( pObjectName );

    // Do we have a reference Id?
    if ( tamlRefId != 0 )
    {
        // Yes, so insert reference.
        mObjectReferenceMap.insert( tamlRefId, pSimObject );
    }

    // Fetch any children.
    TiXmlNode* pChildXmlNode = pXmlElement->FirstChild();

    TamlCollection customCollection;

    // Do we have any element children?
    if ( pChildXmlNode != NULL && pChildXmlNode->Type() == TiXmlNode::TINYXML_ELEMENT )
    {
        // Yes, so fetch the sim set.
        SimSet* pSimSet = dynamic_cast<SimSet*>( pSimObject );

        // Iterate children.
        for ( TiXmlElement* pChildXmlElement = dynamic_cast<TiXmlElement*>( pChildXmlNode ); pChildXmlElement; pChildXmlElement = pChildXmlElement->NextSiblingElement() )
        {
            // Is this a standard child element?
            if ( dStrchr( pChildXmlElement->Value(), '.' ) == NULL )
            {
                // Is this a sim set?
                if ( pSimSet == NULL )
                {
                    // No, so warn.
                    Con::warnf("Taml: Child element '%s' found under parent '%s' but object cannot have children.",
                        pChildXmlElement->Value(),
                        pXmlElement->Value() );

                    // Skip.
                    continue;
                }

                // Yes, so parse child element.
                SimObject* pChildSimObject = parseElement( pChildXmlElement );

                // Skip if the child was not created.
                if ( pChildSimObject == NULL )
                    continue;

                // Add child.
                pSimSet->addObject( pChildSimObject );

                // Perform callback.
                mpTaml->tamlAddParent( pCallbacks, pSimSet );
            }
            else
            {
                // No, so parse custom element.
                parseCustomElement( pChildXmlElement, customCollection );
            }
        }

        // Call custom read.
        mpTaml->tamlCustomRead( pCallbacks, customCollection );
    }

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

void TamlXmlReader::parseCustomElement( TiXmlElement* pXmlElement, TamlCollection& collection )
{
    // Is this a standard child element?
    const char* pPeriod = dStrchr( pXmlElement->Value(), '.' );

    // Sanity!
    AssertFatal( pPeriod != NULL, "Parsing extended element but no period character found." );

    // Fetch any property type alias.
    TiXmlNode* pPropertyTypeAliasXmlNode = pXmlElement->FirstChild();

    // Do we have any property type alias?
    if ( pPropertyTypeAliasXmlNode != NULL && pPropertyTypeAliasXmlNode->Type() == TiXmlNode::TINYXML_ELEMENT )
    {
        // Yes, so add collection property.
        TamlCollectionProperty* pCollectionProperty = collection.addCollectionProperty( pPeriod+1 );

        // Iterate type alias.
        for ( TiXmlElement* pPropertyTypeAliasXmlElement = dynamic_cast<TiXmlElement*>( pPropertyTypeAliasXmlNode ); pPropertyTypeAliasXmlElement; pPropertyTypeAliasXmlElement = pPropertyTypeAliasXmlElement->NextSiblingElement() )
        {
            // Add property type alias.
            TamlPropertyTypeAlias* pPropertyTypeAlias = pCollectionProperty->addTypeAlias( pPropertyTypeAliasXmlElement->Value() );

            // Iterate property field attributes.
            for ( TiXmlAttribute* pAttribute = pPropertyTypeAliasXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
            {
                // Insert attribute name.
                StringTableEntry attributeName = StringTable->insert( pAttribute->Name() );

                // Add property field.
                pPropertyTypeAlias->addPropertyField( attributeName, pAttribute->Value() );
            }

            // Fetch any children.
            TiXmlNode* pChildXmlNode = pPropertyTypeAliasXmlNode->FirstChild();

            // Do we have any element children?
            if ( pChildXmlNode != NULL && pChildXmlNode->Type() == TiXmlNode::TINYXML_ELEMENT )
            {
                // Yes, so iterate children.
                for ( TiXmlElement* pChildXmlElement = dynamic_cast<TiXmlElement*>( pChildXmlNode ); pChildXmlElement; pChildXmlElement = pChildXmlElement->NextSiblingElement() )
                {
                    // Fetch the reference field.
                    const char* pRefField = getTamlRefField( pChildXmlElement );

                    // Was a reference field found?
                    if ( pRefField == NULL )
                    {
                        // No, so warn.
                        Con::warnf( "Taml: Encountered a child element in a custom collection but it did not have a field reference using '%s'.", mTamlRefField );
                        continue;
                    }

                    // Parse the child element.
                    SimObject* pFieldObject = parseElement( pChildXmlElement );

                    // Add property field.
                    pPropertyTypeAlias->addPropertyField( pRefField, pFieldObject );
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void TamlXmlReader::parseAttributes( TiXmlElement* pXmlElement, SimObject* pSimObject )
{
    // Sanity!
    AssertFatal( pSimObject != NULL, "Taml: Cannot parse attributes on a NULL object." );

    // Iterate attributes.
    for ( TiXmlAttribute* pAttribute = pXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
    {
        // Insert attribute name.
        StringTableEntry attributeName = StringTable->insert( pAttribute->Name() );

        // Ignore if this is a Taml attribute.
        if (    attributeName == mTamlRefId ||
                attributeName == mTamlRefToId ||
                attributeName == mTamlObjectName ||
                attributeName == mTamlRefField )
                continue;

        // We can assume this is a field for now.
        pSimObject->setPrefixedDataField( attributeName, NULL, pAttribute->Value() );
    }
}

//-----------------------------------------------------------------------------

U32 TamlXmlReader::getTamlRefId( TiXmlElement* pXmlElement )
{
    // Iterate attributes.
    for ( TiXmlAttribute* pAttribute = pXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
    {
        // Insert attribute name.
        StringTableEntry attributeName = StringTable->insert( pAttribute->Name() );

        // Skip if not the correct attribute.
        if ( attributeName != mTamlRefId )
            continue;

        // Return it.
        return dAtoi( pAttribute->Value() );
    }

    // Not found.
    return 0;
}

//-----------------------------------------------------------------------------

U32 TamlXmlReader::getTamlRefToId( TiXmlElement* pXmlElement )
{
    // Iterate attributes.
    for ( TiXmlAttribute* pAttribute = pXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
    {
        // Insert attribute name.
        StringTableEntry attributeName = StringTable->insert( pAttribute->Name() );

        // Skip if not the correct attribute.
        if ( attributeName != mTamlRefToId )
            continue;

        // Return it.
        return dAtoi( pAttribute->Value() );
    }

    // Not found.
    return 0;
}

//-----------------------------------------------------------------------------

const char* TamlXmlReader::getTamlObjectName( TiXmlElement* pXmlElement )
{
    // Iterate attributes.
    for ( TiXmlAttribute* pAttribute = pXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
    {
        // Insert attribute name.
        StringTableEntry attributeName = StringTable->insert( pAttribute->Name() );

        // Skip if not the correct attribute.
        if ( attributeName != mTamlObjectName )
            continue;

        // Return it.
        return pAttribute->Value();
    }

    // Not found.
    return NULL;
}

//-----------------------------------------------------------------------------

const char* TamlXmlReader::getTamlRefField( TiXmlElement* pXmlElement )
{
    // Iterate attributes.
    for ( TiXmlAttribute* pAttribute = pXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
    {
        // Insert attribute name.
        StringTableEntry attributeName = StringTable->insert( pAttribute->Name() );

        // Skip if not the correct attribute.
        if ( attributeName != mTamlRefField )
            continue;

        // Return it.
        return pAttribute->Value();
    }

    // Not found.
    return NULL;
}


