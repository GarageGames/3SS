//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_XMLWRITER_H_
#define _TAML_XMLWRITER_H_

#ifndef _TAML_H_
#include "taml.h"
#endif

#ifndef TINYXML_INCLUDED
#include "persistence/tinyXML/tinyxml.h"
#endif


//-----------------------------------------------------------------------------

class TamlXmlWriter
{
public:
    TamlXmlWriter( Taml* pTaml ) :
        mpTaml( pTaml )
    {
    }
    virtual ~TamlXmlWriter() {}

    /// Write.
    bool write( FileStream& stream, const TamlWriteNode* pTamlWriteNode );

private:
    Taml* mpTaml;

private:
    TiXmlNode* compileElement( const TamlWriteNode* pTamlWriteNode );
    void compileCustomElements( TiXmlElement* pXmlElement, const TamlWriteNode* pTamlWriteNode );
    void compileAttributes( TiXmlElement* pXmlElement, const TamlWriteNode* pTamlWriteNode );
};

#endif // _TAML_XMLWRITER_H_