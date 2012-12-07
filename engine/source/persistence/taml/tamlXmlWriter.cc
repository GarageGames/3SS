//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlXmlWriter.h"

//-----------------------------------------------------------------------------

bool TamlXmlWriter::write( FileStream& stream, const TamlWriteNode* pTamlWriteNode )
{
    // Sanity!
    AssertFatal( mpTaml->getFormatMode() == Taml::XmlFormat, "Cannot write with a XML writer using a non-XML format mode." );

    // Create document.
    TiXmlDocument xmlDocument;

    // Compile root element.
    xmlDocument.LinkEndChild( compileElement( pTamlWriteNode ) );

    // Save document to stream.
    return xmlDocument.SaveFile( stream );
}

//-----------------------------------------------------------------------------

TiXmlNode* TamlXmlWriter::compileElement( const TamlWriteNode* pTamlWriteNode )
{
    // Fetch object.
    SimObject* pSimObject = pTamlWriteNode->mpSimObject;

    // Fetch element name.
    const char* pElementName = pSimObject->getClassName();

    // Create element.
    TiXmlElement* pElement = new TiXmlElement( pElementName );

    // Fetch reference Id.
    const U32 referenceId = pTamlWriteNode->mRefId;

    // Do we have a reference Id?
    if ( referenceId != 0 )
    {
        // Yes, so set reference Id attribute.
        pElement->SetAttribute( TAML_ID_ATTRIBUTE_NAME, referenceId );
    }

    // Do we have a reference to node?
    else if ( pTamlWriteNode->mRefToNode != NULL )
    {
        // Yes, so fetch reference to Id.
        const U32 referenceToId = pTamlWriteNode->mRefToNode->mRefId;

        // Sanity!
        AssertFatal( referenceToId != 0, "Taml: Invalid reference to Id." );

        // Set reference to Id attribute.
        pElement->SetAttribute( TAML_REFID_ATTRIBUTE_NAME, referenceToId );

        // Do we have a reference field?
        if ( pTamlWriteNode->mRefField != StringTable->EmptyString )
        {
            // Yes, so set attribute.
            pElement->SetAttribute( TAML_REF_FIELD_NAME, pTamlWriteNode->mRefField );
        }

        // Finish because we're a reference to another object.
        return pElement;
    }

    // Do we have a reference field?
    if ( pTamlWriteNode->mRefField != StringTable->EmptyString )
    {
        // Yes, so set attribute.
        pElement->SetAttribute( TAML_REF_FIELD_NAME, pTamlWriteNode->mRefField );
    }

    // Fetch object name.
    const char* pObjectName = pTamlWriteNode->mpObjectName;

    // Do we have a name?
    if ( pObjectName != NULL )
    {
        // Yes, so set name attribute.
        pElement->SetAttribute( TAML_OBJECTNAME_ATTRIBUTE_NAME, pObjectName );
    }

    // Compile attributes.
    compileAttributes( pElement, pTamlWriteNode );

    // Fetch children.
    Vector<TamlWriteNode*>* pChildren = pTamlWriteNode->mChildren;

    // Do we have any children?
    if ( pChildren )
    {
        // Yes, so iterate children.
        for( Vector<TamlWriteNode*>::iterator itr = pChildren->begin(); itr != pChildren->end(); ++itr )
        {
            // Write child element.
            pElement->LinkEndChild( compileElement( (*itr) ) );
        }
    }

    // Compile custom elements.
    compileCustomElements( pElement, pTamlWriteNode );

    return pElement;
}

//-----------------------------------------------------------------------------

void TamlXmlWriter::compileCustomElements( TiXmlElement* pXmlElement, const TamlWriteNode* pTamlWriteNode )
{
    // Fetch custom collection.
    const TamlCollection& customCollection = pTamlWriteNode->mCustomCollection;

    // Finish if no custom properties exist.
    if ( customCollection.size() == 0 )
        return;

    // Iterate collection properties.
    for( TamlCollection::const_iterator collectionPropertyItr = customCollection.begin(); collectionPropertyItr != customCollection.end(); ++collectionPropertyItr )
    {
        // Fetch collection property.
        TamlCollectionProperty* pCollectionProperty = *collectionPropertyItr;

        // Skip if nothing in property.
        if ( pCollectionProperty->size() == 0 )
            continue;

        // Format extended element name.
        char extendedElementNameBuffer[256];
        dSprintf( extendedElementNameBuffer, sizeof(extendedElementNameBuffer), "%s.%s", pXmlElement->Value(), pCollectionProperty->mPropertyName );
        StringTableEntry extendedElementName = StringTable->insert( extendedElementNameBuffer );

        // Create element.
        TiXmlElement* pExtendedPropertyElement = new TiXmlElement( extendedElementName );

        // Iterate property type alias.
        for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pCollectionProperty->begin(); propertyTypeAliasItr != pCollectionProperty->end(); ++propertyTypeAliasItr )
        {
            // Fetch property type alias.
            TamlPropertyTypeAlias* pPropertyTypeAlias = *propertyTypeAliasItr;

            // Create element.
            TiXmlElement* pPropertyElement = new TiXmlElement( pPropertyTypeAlias->mAliasName );

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Is it an object field?
                if ( pPropertyField->isObjectField() )
                {
                    // Yes, so write child element.
                    pPropertyElement->LinkEndChild( compileElement( pPropertyField->getWriteNode() ) );
                }
                else
                {
                    // No, so set property attribute.
                    pPropertyElement->SetAttribute( pPropertyField->getFieldName(), pPropertyField->getFieldValue() );
                }
            }

            // Write property element as child.
            pExtendedPropertyElement->LinkEndChild( pPropertyElement );
        }

        // Write extended property element as child.
        pXmlElement->LinkEndChild( pExtendedPropertyElement );
    }
}

//-----------------------------------------------------------------------------

void TamlXmlWriter::compileAttributes( TiXmlElement* pXmlElement, const TamlWriteNode* pTamlWriteNode )
{
    // Fetch fields.
    const Vector<TamlWriteNode::FieldValuePair*>& fields = pTamlWriteNode->mFields;

    // Ignore if no fields.
    if ( fields.size() == 0 )
        return;

    // Iterate fields.
    for( Vector<TamlWriteNode::FieldValuePair*>::const_iterator itr = fields.begin(); itr != fields.end(); ++itr )
    {
        // Fetch field/value pair.
        TamlWriteNode::FieldValuePair* pFieldValue = (*itr);

        // Set field attribute.
        pXmlElement->SetAttribute( pFieldValue->mName, pFieldValue->mpValue );
    }
}