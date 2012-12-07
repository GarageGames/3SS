//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_ASSET_REFERENCED_VISITOR_H_
#define _TAML_ASSET_REFERENCED_VISITOR_H_

#ifndef _TAML_XMLPARSER_H_
#include "persistence//taml/tamlXmlParser.h"
#endif

#ifndef _ASSET_FIELD_TYPES_H
#include "assets/assetFieldTypes.h"
#endif

//-----------------------------------------------------------------------------

class TamlAssetReferencedVisitor : public TamlXmlVisitor
{
protected:
    virtual bool visit( TiXmlElement* pXmlElement, TamlXmlParser& xmlParser ) { return true; }

    virtual bool visit( TiXmlAttribute* pAttribute, TamlXmlParser& xmlParser )
    {
        // Fetch asset reference.
        const char* pAssetReference = pAttribute->Value();

        // Fetch field word count.
        const U32 fieldWordCount = StringUnit::getUnitCount( pAssetReference, ASSET_ASSIGNMENT_SEPARATOR );

        // Finish if there are not two words.
        if ( fieldWordCount != 2 )
            return true;

        // Finish if the first word is not an asset signature.
        if ( StringTable->insert( StringUnit::getUnit( pAssetReference, 0, ASSET_ASSIGNMENT_SEPARATOR ) ) != StringTable->insert(ASSET_ID_SIGNATURE) )
            return true;

        // Get asset Id.
        typeAssetId assetId = StringTable->insert( StringUnit::getUnit( pAssetReference, 1, ASSET_ASSIGNMENT_SEPARATOR ) );

        // Finish if we already have this asset Id.
        if ( mAssetReferenced.contains( assetId ) )
            return true;

        // Insert asset reference.
        mAssetReferenced.insert( assetId, StringTable->EmptyString );

        return true;
    }

public:
    TamlAssetReferencedVisitor() {}
    virtual ~TamlAssetReferencedVisitor() {}

    bool parse( const char* pFilename )
    {
        TamlXmlParser parser;
        return parser.parse( pFilename, *this, false );
    }

    typedef StringTableEntry typeAssetId;
    typedef HashMap<typeAssetId, StringTableEntry> typeAssetReferencedHash;

    const typeAssetReferencedHash& getAssetReferencedMap( void ) const { return mAssetReferenced; }

    void clear( void ) { mAssetReferenced.clear(); }

private:
    typeAssetReferencedHash mAssetReferenced;
};

#endif // _TAML_ASSET_REFERENCED_VISITOR_H_
