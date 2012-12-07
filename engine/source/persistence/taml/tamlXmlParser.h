//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_XMLPARSER_H_
#define _TAML_XMLPARSER_H_

#ifndef _TAML_XML_VISITOR_H_
#include "tamlXmlVisitor.h"
#endif

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _TAML_H_
#include "taml.h"
#endif

//-----------------------------------------------------------------------------

class TamlXmlParser
{
public:
    TamlXmlParser() :
        mpParsingFilename( NULL ) {}
    virtual ~TamlXmlParser() {}

    /// Parse.
    bool parse( const char* pFilename, TamlXmlVisitor& visitor, const bool writeDocument );

    /// Filename.
    inline const char* getParsingFilename( void ) const { return mpParsingFilename; }

private:
    const char* mpParsingFilename;

private:
    bool parseElement( TiXmlElement* pXmlElement, TamlXmlVisitor& visitor );
    bool parseAttributes( TiXmlElement* pXmlElement, TamlXmlVisitor& visitor );
};

#endif // _TAML_XMLPARSER_H_