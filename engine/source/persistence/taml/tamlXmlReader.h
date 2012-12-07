//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_XMLREADER_H_
#define _TAML_XMLREADER_H_

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _TAML_H_
#include "taml.h"
#endif

#ifndef TINYXML_INCLUDED
#include "persistence/tinyXML/tinyxml.h"
#endif

//-----------------------------------------------------------------------------

class TamlXmlReader
{
public:
    TamlXmlReader( Taml* pTaml ) :
        mpTaml( pTaml )
    {
        mTamlRefId      = StringTable->insert( TAML_ID_ATTRIBUTE_NAME );
        mTamlRefToId    = StringTable->insert( TAML_REFID_ATTRIBUTE_NAME );
        mTamlRefField   = StringTable->insert( TAML_REF_FIELD_NAME );
        mTamlObjectName = StringTable->insert( TAML_OBJECTNAME_ATTRIBUTE_NAME );
    }

    virtual ~TamlXmlReader() {}

    /// Read.
    SimObject* read( FileStream& stream );

private:
    Taml*               mpTaml;
    StringTableEntry    mTamlRefId;
    StringTableEntry    mTamlRefToId;
    StringTableEntry    mTamlObjectName;
    StringTableEntry    mTamlRefField;

    typedef HashMap<SimObjectId, SimObject*> typeObjectReferenceHash;

    typeObjectReferenceHash mObjectReferenceMap;

private:
    void resetParse( void );

    SimObject* parseElement( TiXmlElement* pXmlElement );
    void parseCustomElement( TiXmlElement* pXmlElement, TamlCollection& collection );
    void parseAttributes( TiXmlElement* pXmlElement, SimObject* pSimObject );

    U32 getTamlRefId( TiXmlElement* pXmlElement );
    U32 getTamlRefToId( TiXmlElement* pXmlElement );
    const char* getTamlObjectName( TiXmlElement* pXmlElement );   
    const char* getTamlRefField( TiXmlElement* pXmlElement );
};

#endif // _TAML_XMLREADER_H_