//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_BINARYWRITER_H_
#define _TAML_BINARYWRITER_H_

#ifndef _TAML_H_
#include "taml.h"
#endif

//-----------------------------------------------------------------------------

class TamlBinaryWriter
{
public:
    TamlBinaryWriter( Taml* pTaml ) :
        mpTaml( pTaml ),
        mVersionId(1)
    {
    }
    virtual ~TamlBinaryWriter() {}

    /// Write.
    bool write( FileStream& stream, const TamlWriteNode* pTamlWriteNode, const bool compressed );

private:
    Taml* mpTaml;
    const U32 mVersionId;

private:
    void writeElement( Stream& stream, const TamlWriteNode* pTamlWriteNode );
    void writeCustomElements( Stream& stream, const TamlWriteNode* pTamlWriteNode );
    void writeAttributes( Stream& stream, const TamlWriteNode* pTamlWriteNode );
    void writeChildren( Stream& stream, const TamlWriteNode* pTamlWriteNode );
};

#endif // _TAML_BINARYWRITER_H_