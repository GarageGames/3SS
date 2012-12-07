//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_BINARYREADER_H_
#define _TAML_BINARYREADER_H_

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _TAML_H_
#include "taml.h"
#endif

//-----------------------------------------------------------------------------

class TamlBinaryReader
{
public:
    TamlBinaryReader( Taml* pTaml ) :
        mpTaml( pTaml )
    {
    }

    virtual ~TamlBinaryReader() {}

    /// Read.
    SimObject* read( FileStream& stream );

private:
    Taml*               mpTaml;
    StringTableEntry    mTamlObjectName;

    typedef HashMap<SimObjectId, SimObject*> typeObjectReferenceHash;

    typeObjectReferenceHash mObjectReferenceMap;

private:
    void resetParse( void );

    SimObject* parseElement( Stream& stream, const U32 versionId );
    void parseCustomElement( Stream& stream, TamlCallbacks* pCallbacks, TamlCollection& customCollection, const U32 versionId );
    void parseAttributes( Stream& stream, SimObject* pSimObject, const U32 versionId );
    void parseChildren( Stream& stream, TamlCallbacks* pCallbacks, SimObject* pSimObject, const U32 versionId );
};

#endif // _TAML_BINARYREADER_H_