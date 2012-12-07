//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_ASSET_REFERENCED_UPDATE_VISITOR_H_
#define _TAML_ASSET_REFERENCED_UPDATE_VISITOR_H_

#ifndef _TAML_XMLPARSER_H_
#include "persistence//taml/tamlXmlParser.h"
#endif

#ifndef _STRINGUNIT_H_
#include "string/stringUnit.h"
#endif

#ifndef _STRINGTABLE_H_
#include "string/stringTable.h"
#endif

#ifndef _ASSET_FIELD_TYPES_H
#include "assets/assetFieldTypes.h"
#endif

//-----------------------------------------------------------------------------

class TamlAssetReferencedUpdateVisitor : public TamlXmlVisitor
{
protected:
    virtual bool visit( TiXmlElement* pXmlElement, TamlXmlParser& xmlParser ) { return true; }

    virtual bool visit( TiXmlAttribute* pAttribute, TamlXmlParser& xmlParser )
    {
        // Fetch attribute value.
        const char* pAttributeValue = pAttribute->Value();

        // Fetch attribute value word count.
        const U32 valueWordCount = StringUnit::getUnitCount( pAttributeValue, ASSET_ASSIGNMENT_SEPARATOR );

        // Finish if not two words.
        if ( valueWordCount != 2 )
            return true;

        // Skip if this is not an asset signature.
        if ( dStricmp( StringUnit::getUnit( pAttributeValue, 0, ASSET_ASSIGNMENT_SEPARATOR), ASSET_ID_SIGNATURE ) != 0 )
            return true;

        // Get the asset value.
        const char* pAssetValue = StringUnit::getUnit( pAttributeValue, 1, ASSET_ASSIGNMENT_SEPARATOR );

        // Finish if not the asset Id we're looking for.
        if ( dStricmp( pAssetValue, mAssetIdFrom ) != 0 )
            return true;

        // Is the target asset empty?
        if ( mAssetIdTo == StringTable->EmptyString )
        {
            // Yes, so set the attribute as empty.
            pAttribute->SetValue( StringTable->EmptyString );
            return true;
        }

        // Format asset.
        char assetBuffer[1024];
        dSprintf( assetBuffer, sizeof(assetBuffer), "%s%s%s", ASSET_ID_SIGNATURE, ASSET_ASSIGNMENT_SEPARATOR, mAssetIdTo );

        // Assign new value.
        pAttribute->SetValue( assetBuffer );

        return true;
    }

public:
    TamlAssetReferencedUpdateVisitor() {}
    virtual ~TamlAssetReferencedUpdateVisitor() {}

    bool parse( const char* pFilename )
    {
        TamlXmlParser parser;
        return parser.parse( pFilename, *this, true );
    }

    void setAssetIdFrom( const char* pAssetIdFrom )
    {
        // Sanity!
        AssertFatal( pAssetIdFrom != NULL, "Asset Id from cannot be NULL." );

        mAssetIdFrom = StringTable->insert( pAssetIdFrom );
    }
    StringTableEntry getAssetIdFrom( void ) const { return mAssetIdFrom; }

    void setAssetIdTo( const char* pAssetIdTo )
    {
        // Sanity!
        AssertFatal( pAssetIdTo != NULL, "Asset Id to cannot be NULL." );

        mAssetIdTo = StringTable->insert( pAssetIdTo );
    }
    const char* getAssetIdTo( void ) const { return mAssetIdTo; }

private:    
    StringTableEntry mAssetIdFrom;
    StringTableEntry mAssetIdTo;
};

#endif // _TAML_ASSET_REFERENCED_UPDATE_VISITOR_H_
