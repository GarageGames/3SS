//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PLATFORMUNICODE_H
#define _PLATFORMUNICODE_H

#include "platform/types.h"

namespace Platform
{
    const U32 convertUTF8toUTF16(const UTF8 *unistring, UTF16 *outbuffer, U32 len);
    const U32 convertUTF16toUTF8( const UTF16 *unistring, UTF8  *outbuffer, U32 len);

    const UTF32  oneUTF8toUTF32( const UTF8 *codepoint,  U32 *unitsWalked = NULL);
    const U32    oneUTF32toUTF8( const UTF32 codepoint, UTF8 *threeByteCodeunitBuf);

    const UTF16  oneUTF32toUTF16(const UTF32 codepoint);
    const UTF32  oneUTF16toUTF32(const UTF16 *codepoint, U32 *unitsWalked = NULL);
}

#endif  // _PLATFORMUNICODE_H
