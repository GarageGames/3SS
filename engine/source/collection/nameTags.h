//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _NAMETAGS_H_
#define _NAMETAGS_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

//-----------------------------------------------------------------------------

class NameTags : public SimSet
{
    typedef SimSet      Parent;

public:
    NameTags();
    virtual ~NameTags();
    virtual bool onAdd();

    /// Tag type-definitions.
    typedef U32                                 TagId;
    typedef U32                                 HashId;
    typedef HashMap<HashId, TagId>             hashTagMapType;
    typedef HashMap<TagId, StringTableEntry>   tagNameMapType;
    typedef HashMap<SimObjectId, SimObject*>   queryType;

    /// Tag accessor.
    TagId               createTag( const char* pTagName );
    TagId               renameTag( const TagId tagId, const char* pNewTagName );
    TagId               deleteTag( const TagId tagId );

    inline U32          getTagCount( void ) const { return mTagNameMap.size(); }
    StringTableEntry    getTagName( const TagId tagId );
    U32                 getTagId( const char* pTagName );

    /// Tagging.
    bool                tag( const SimObjectId objId, const TagId tagId );
    bool                untag( const SimObjectId objId, const TagId tagId );
    bool                hasTag( const SimObjectId objId, const TagId tagId ) const;

    /// Tag query.
    void                queryTags( const char* pTags );

    /// Tag format.
    S32                 formatTags( char* pBuffer, U32 bufferLength );

    virtual void        write( Stream &stream, U32 tabStop, U32 flags = 0 );
    virtual void        writeFields( Stream& stream, U32 tabStop );

    DECLARE_CONOBJECT( NameTags );

public:
    queryType           mIncludedQueryMap;
    queryType           mExcludedQueryMap;

private:
    hashTagMapType      mHashTagMap;
    tagNameMapType      mTagNameMap;

    TagId               mMasterTagId;
    StringTableEntry    mNameTagsFieldEntry;
};

#endif // _NAMETAGS_H_