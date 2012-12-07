//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TEXTURE_OBJECT_H_
#define _TEXTURE_OBJECT_H_

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

#ifndef _PLATFORMGL_H_
#include "platform/platformAssert.h"
#include "platform/platformGL.h"
#endif

#ifndef _TEXTURE_HANDLE_H_
#include "graphics/TextureHandle.h"
#endif

#ifndef _GBITMAP_H_
#include "graphics/gBitmap.h"
#endif

//-----------------------------------------------------------------------------

class GBitmap;

//------------------------------------------------------------------------------

class TextureObject
{
    friend class TextureManager;
    friend class TextureDictionary;
    friend class TextureHandle;

private:
    TextureObject*  next;
    TextureObject*  prev;
    TextureObject*  hashNext;

    S32                 mTextureResidentWasteSize;
    S32                 mTextureResidentSize;
    S32                 mBitmapResidentSize;
    S32                 mRefCount;

    StringTableEntry    mTextureKey;
    GLuint              mGLTextureName;
    GBitmap*            mpBitmap;
    U32                 mTextureWidth;
    U32                 mTextureHeight;
    U32                 mBitmapWidth;
    U32                 mBitmapHeight;
    GLuint              mFilter;
    bool                mClamp;

    TextureHandle::TextureHandleType mHandleType;

public:
    TextureObject() :
        next( NULL ), prev( NULL ), hashNext( NULL ),
        mTextureResidentWasteSize( 0 ),
        mTextureResidentSize( 0 ),
        mBitmapResidentSize( 0 ),
        mRefCount( 0 ),
        mTextureKey( NULL ),
        mGLTextureName( 0 ),
        mpBitmap( NULL ),
        mTextureWidth( 0 ),
        mTextureHeight( 0 ),
        mBitmapWidth( 0 ),
        mBitmapHeight( 0 ),
        mFilter( GL_NEAREST ),
        mClamp( false ),
        mHandleType( TextureHandle::InvalidTexture )
    {
    }

    inline StringTableEntry getTextureKey( void ) { return mTextureKey; }
    inline GLuint getGLTextureName( void ) { return mGLTextureName; }
    inline const GBitmap* getBitmap( void ) { return mpBitmap; }
    inline U32 getTextureWidth( void ) { return mTextureWidth; }
    inline U32 getTextureHeight( void ) { return mTextureHeight; }
    inline U32 getBitmapWidth( void ) { return mBitmapWidth; }
    inline U32 getBitmapHeight( void ) { return mBitmapHeight; }
    inline GLuint getFilter( void ) { return mFilter; }
    inline bool getClamp( void ) { return mClamp; }
    
    inline S32 getTextureResidentSize( void ) const { return mTextureResidentSize; }
    inline S32 getBitmapResidentSize( void ) const { return mBitmapResidentSize; }
    inline TextureHandle::TextureHandleType getHandleType( void ) { return mHandleType; }
};

#endif // _TEXTURE_OBJECT_H_
