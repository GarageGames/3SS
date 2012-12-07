//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlBinaryWriter.h"

#ifndef _ZIPSUBSTREAM_H_
#include "io/zip/zipSubStream.h"
#endif

//-----------------------------------------------------------------------------

bool TamlBinaryWriter::write( FileStream& stream, const TamlWriteNode* pTamlWriteNode, const bool compressed )
{
    // Sanity!
    AssertFatal( mpTaml->getFormatMode() == Taml::BinaryFormat, "Cannot write with a binary writer using a non-binary format mode." );
   
    // Write Taml signature.
    stream.writeString( StringTable->insert( TAML_SIGNATURE ) );

    // Write version Id.
    stream.write( mVersionId );

    // Write compressed flag.
    stream.write( compressed );

    // Are we compressed?
    if ( compressed )
    {
        // yes, so attach zip stream.
        ZipSubWStream zipStream;
        zipStream.attachStream( &stream );

        // Write element.
        writeElement( zipStream, pTamlWriteNode );

        // Detach zip stream.
        zipStream.detachStream();
    }
    else
    {
        // No, so write element.
        writeElement( stream, pTamlWriteNode );
    }

    return true;
}

//-----------------------------------------------------------------------------

void TamlBinaryWriter::writeElement( Stream& stream, const TamlWriteNode* pTamlWriteNode )
{
    // Fetch object.
    SimObject* pSimObject = pTamlWriteNode->mpSimObject;

    // Fetch element name.
    const char* pElementName = pSimObject->getClassName();

    // Write element name.
    stream.writeString( pElementName );

    // Fetch object name.
    const char* pObjectName = pTamlWriteNode->mpObjectName;

    // Write object name.
    stream.writeString( pObjectName != NULL ? pObjectName : StringTable->EmptyString );

    // Fetch reference Id.
    const U32 tamlRefId = pTamlWriteNode->mRefId;

    // Write reference Id.
    stream.write( tamlRefId );

    // Do we have a reference to node?
    if ( pTamlWriteNode->mRefToNode != NULL )
    {
        // Yes, so fetch reference to Id.
        const U32 tamlRefToId = pTamlWriteNode->mRefToNode->mRefId;

        // Sanity!
        AssertFatal( tamlRefToId != 0, "Taml: Invalid reference to Id." );

        // Write reference to Id.
        stream.write( tamlRefToId );

        // Finished.
        return;
    }

    // No, so write no reference to Id.
    stream.write( 0 );

    // Write attributes.
    writeAttributes( stream, pTamlWriteNode );

    // Write children.
    writeChildren( stream, pTamlWriteNode );

    // Write custom elements.
    writeCustomElements( stream, pTamlWriteNode );
}

//-----------------------------------------------------------------------------

void TamlBinaryWriter::writeCustomElements( Stream& stream, const TamlWriteNode* pTamlWriteNode )
{
    // Fetch custom collection.
    const TamlCollection& customCollection = pTamlWriteNode->mCustomCollection;

    // Write custom element count.
    stream.write( (U32)customCollection.size() );

    // Iterate collection properties.
    for( TamlCollection::const_iterator collectionPropertyItr = customCollection.begin(); collectionPropertyItr != customCollection.end(); ++collectionPropertyItr )
    {
        // Fetch collection property.
        TamlCollectionProperty* pCollectionProperty = *collectionPropertyItr;

        // Write custom element name.
        stream.writeString( pCollectionProperty->mPropertyName );

        // Fetch property type alias count.
        const U32 propertyTypeAliasCount = (U32)pCollectionProperty->size();

        // Write property count.
        stream.write( propertyTypeAliasCount );

        // Skip if no property type alias.
        if (propertyTypeAliasCount == 0 )
            continue;

        // Iterate property type alias.
        for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pCollectionProperty->begin(); propertyTypeAliasItr != pCollectionProperty->end(); ++propertyTypeAliasItr )
        {
            // Fetch property type alias.
            TamlPropertyTypeAlias* pPropertyTypeAlias = *propertyTypeAliasItr;

            // Write property type alias name.
            stream.writeString( pPropertyTypeAlias->mAliasName );

            // Write property field count.
            stream.write( (U32)pPropertyTypeAlias->size() );

            // Skip if no property fields.
            if ( pPropertyTypeAlias->size() == 0 )
                continue;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch object field flag,
                const bool isObjectField = pPropertyField->isObjectField();

                // Write flag.
                stream.write( isObjectField );

                // Is it an object field?
                if ( isObjectField )
                {
                    // Yes, so fetch write node.
                    const TamlWriteNode* pObjectWriteField = pPropertyField->getWriteNode();

                    // Write reference field.
                    stream.writeString( pObjectWriteField->mRefField );

                    // Write field object.
                    writeElement( stream, pObjectWriteField );
                }
                else
                {
                    // No, so write property attribute.
                    stream.writeString( pPropertyField->getFieldName() );
                    stream.writeLongString( MAX_TAML_PROPERTY_FIELDVALUE_LENGTH, pPropertyField->getFieldValue() );
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void TamlBinaryWriter::writeAttributes( Stream& stream, const TamlWriteNode* pTamlWriteNode )
{
    // Fetch fields.
    const Vector<TamlWriteNode::FieldValuePair*>& fields = pTamlWriteNode->mFields;

    // Write placeholder attribute count.
    stream.write( (U32)fields.size() );

    // Finish if no fields.
    if ( fields.size() == 0 )
        return;

    // Iterate fields.
    for( Vector<TamlWriteNode::FieldValuePair*>::const_iterator itr = fields.begin(); itr != fields.end(); ++itr )
    {
        // Fetch field/value pair.
        TamlWriteNode::FieldValuePair* pFieldValue = (*itr);

        // Write attribute.
        stream.writeString( pFieldValue->mName );
        stream.writeLongString( 4096, pFieldValue->mpValue );
    }
}

void TamlBinaryWriter::writeChildren( Stream& stream, const TamlWriteNode* pTamlWriteNode )
{
    // Fetch children.
    Vector<TamlWriteNode*>* pChildren = pTamlWriteNode->mChildren;

    // Do we have any children?
    if ( pChildren == NULL )
    {
        // No, so write no children.
        stream.write( (U32)0 );
        return;
    }

    // Write children count.
    stream.write( (U32)pChildren->size() );

    // Iterate children.
    for( Vector<TamlWriteNode*>::iterator itr = pChildren->begin(); itr != pChildren->end(); ++itr )
    {
        // Write child.
        writeElement( stream, (*itr) );
    }
}