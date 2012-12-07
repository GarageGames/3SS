//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IHTTPSERVICESPROVIDER_H
#define _IHTTPSERVICESPROVIDER_H

#include "collection/hashTable.h"

class IHttpServicesProvider
{
public:
    typedef HashTable<StringTableEntry, StringTableEntry> PostDictionary;

    virtual S32 HttpPost(S32 callback, const char* url, PostDictionary* postArray) = 0;
};

#endif // _IHTTPSERVICESPROVIDER_H
