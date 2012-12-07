//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SIM_FIELD_DICTIONARY_H_
#define _SIM_FIELD_DICTIONARY_H_

#ifndef _STREAM_H_
#include "io/stream.h"
#endif

//-----------------------------------------------------------------------------

class SimObject;

//-----------------------------------------------------------------------------

/// Dictionary to keep track of dynamic fields on SimObject.

class SimFieldDictionary
{
   friend class SimFieldDictionaryIterator;

  public:
   struct Entry
   {
      StringTableEntry slotName;
      char *value;
      Entry *next;
   };
   enum
   {
      HashTableSize = 19
   };
   Entry *mHashTable[HashTableSize];
  private:

   static Entry *mFreeList;
   static void freeEntry(Entry *entry);
   static Entry *allocEntry();

   /// In order to efficiently detect when a dynamic field has been
   /// added or deleted, we increment this every time we add or
   /// remove a field.
   U32 mVersion;

public:
   const U32 getVersion() const { return mVersion; }

   SimFieldDictionary();
   ~SimFieldDictionary();
   void setFieldValue(StringTableEntry slotName, const char *value);
   const char *getFieldValue(StringTableEntry slotName);
   void writeFields(SimObject *obj, Stream &strem, U32 tabStop);
   void printFields(SimObject *obj);
   void assignFrom(SimFieldDictionary *dict);
};

//-----------------------------------------------------------------------------

class SimFieldDictionaryIterator
{
   SimFieldDictionary *          mDictionary;
   S32                           mHashIndex;
   SimFieldDictionary::Entry *   mEntry;

  public:
   SimFieldDictionaryIterator(SimFieldDictionary*);
   SimFieldDictionary::Entry* operator++();
   SimFieldDictionary::Entry* operator*();
};

#endif // _SIM_FIELD_DICTIONARY_H_