//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TEXTURE_DICTIONARY_H_
#define _TEXTURE_DICTIONARY_H_

#ifndef _TEXTURE_OBJECT_H_
#include "graphics/TextureObject.h"
#endif

//-----------------------------------------------------------------------------

class TextureDictionary
{
public:
    static TextureObject **smTable;
    static TextureObject *TextureObjectChain;
    static U32 smHashTableSize;

    static void create();
    static void destroy();

    static void insert(TextureObject *object);
    static TextureObject *find(StringTableEntry textureKey, TextureHandle::TextureHandleType type, bool clamp);
    static void remove(TextureObject *object);
};

#endif // _TEXTURE_DICTIONARY_H_