//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_XML_VISITOR_H_
#define _TAML_XML_VISITOR_H_

#ifndef TINYXML_INCLUDED
#include "persistence/tinyXML/tinyxml.h"
#endif

//-----------------------------------------------------------------------------

class TamlXmlParser;

//-----------------------------------------------------------------------------

class TamlXmlVisitor
{
private:
    friend class TamlXmlParser;

public:
    TamlXmlVisitor() {}
    virtual ~TamlXmlVisitor() {}

    /// Parsing.
    virtual bool parse( const char* pFilename ) = 0;

protected:
    virtual bool visit( TiXmlElement* pXmlElement, TamlXmlParser& xmlParser ) = 0;
    virtual bool visit( TiXmlAttribute* pAttribute, TamlXmlParser& xmlParser ) = 0;
};

#endif // _TAML_XML_VISITOR_H_