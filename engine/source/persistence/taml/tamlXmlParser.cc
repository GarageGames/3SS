//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlXmlParser.h"

//-----------------------------------------------------------------------------

bool TamlXmlParser::parse( const char* pFilename, TamlXmlVisitor& visitor, const bool writeDocument )
{
    // Sanity!
    AssertFatal( pFilename != NULL, "Cannot parse a NULL filename." );

    char filenameBuffer[1024];
    Con::expandPath( filenameBuffer, sizeof(filenameBuffer), pFilename );

    FileStream stream;

    // File open for read?
    if ( !stream.open( filenameBuffer, FileStream::Read ) )
    {
        // No, so warn.
        Con::warnf("TamlXmlParser::parse() - Could not open filename '%s' for parse.", filenameBuffer );
        return false;
    }

    TiXmlDocument xmlDocument;

    // Load document from stream.
    if ( !xmlDocument.LoadFile( stream ) )
    {
        // Warn!
        Con::warnf("TamlXmlParser: Could not load Taml XML file from stream.");
        return false;
    }

    // Close the stream.
    stream.close();

    // Set parsing filename.
    mpParsingFilename = filenameBuffer;

    // Parse root element.
    parseElement( xmlDocument.RootElement(), visitor );

    // Reset parsing filename.
    mpParsingFilename = NULL;

    // Are we writing the document?
    if ( writeDocument )
    {
        // Yes, so open for write?
        if ( !stream.open( filenameBuffer, FileStream::Write ) )
        {
            // No, so warn.
            Con::warnf("TamlXmlParser::parse() - Could not open filename '%s' for write.", filenameBuffer );
            return false;
        }

        // Yes, so save the document.
        if ( !xmlDocument.SaveFile( stream ) )
        {
            // Warn!
            Con::warnf("TamlXmlParser: Could not save Taml XML document.");
            return false;
        }

        // Close the stream.
        stream.close();
    }

    return true;
}

//-----------------------------------------------------------------------------

bool TamlXmlParser::parseElement( TiXmlElement* pXmlElement, TamlXmlVisitor& visitor )
{
    // Visit this element (stop processing if instructed).
    if ( !visitor.visit( pXmlElement, *this ) )
        return false;

    // Parse attributes (stop processing if instructed).
    if ( !parseAttributes( pXmlElement, visitor ) )
        return false;

    // Fetch any children.
    TiXmlNode* pChildXmlNode = pXmlElement->FirstChild();

    // Do we have any element children?
    if ( pChildXmlNode != NULL && pChildXmlNode->Type() == TiXmlNode::TINYXML_ELEMENT )
    {
        // Iterate children.
        for ( TiXmlElement* pChildXmlElement = dynamic_cast<TiXmlElement*>( pChildXmlNode ); pChildXmlElement; pChildXmlElement = pChildXmlElement->NextSiblingElement() )
        {
            // Parse element (stop processing if instructed).
            if ( !parseElement( pChildXmlElement, visitor ) )
                return false;
        }
    }

    return true;
}

//-----------------------------------------------------------------------------

bool TamlXmlParser::parseAttributes( TiXmlElement* pXmlElement, TamlXmlVisitor& visitor )
{
    // Iterate attributes.
    for ( TiXmlAttribute* pAttribute = pXmlElement->FirstAttribute(); pAttribute; pAttribute = pAttribute->Next() )
    {
        // Visit this attribute (stop processing if instructed).
        if ( !visitor.visit( pAttribute, *this ) )
            return false;
    }

    return true;
}
