//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _DYNAMIC_TEXTURE_H_
#define _DYNAMIC_TEXTURE_H_

#ifndef _TEXTURE_HANDLE_H_
#include "graphics/TextureHandle.h"
#endif

#ifndef _TEXTURE_HANDLE_H_
#include "graphics/textureHandle.h"
#endif

#ifndef _SIM_OBJECT_H_
#include "sim/simObject.h"
#endif

//-----------------------------------------------------------------------------

class DynamicTexture : SimObject
{
    typedef SimObject Parent;

private:
    StringTableEntry    mTextureKey;
    TextureHandle       mTextureHandle;
    GBitmap*            mpBitmap;

public:
    DynamicTexture();
    ~DynamicTexture();

    void setSize( const U32 texelWidth, const U32 texelHeight );
};

#endif // _DYNAMIC_TEXTURE_H_
