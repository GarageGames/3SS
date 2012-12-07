//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TEXTURE_MANAGER_H_
#define _TEXTURE_MANAGER_H_

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

#ifndef _PLATFORMGL_H_
#include "platform/platformAssert.h"
#include "platform/platformGL.h"
#endif

#ifndef _TEXTURE_OBJECT_H_
#include "graphics/TextureObject.h"
#endif

#ifndef _TEXTURE_DICTIONARY_H_
#include "graphics/TextureDictionary.h"
#endif

//-----------------------------------------------------------------------------

#define MaximumProductSupportedTextureWidth 2048
#define MaximumProductSupportedTextureHeight MaximumProductSupportedTextureWidth

class TextureManager
{
   friend class TextureHandle;
   friend class TextureDictionary;

public:
    /// Texture manager event codes.
    enum TextureEventCode
    {
        BeginZombification,
        BeginResurrection,
        EndResurrection
    };

    typedef void (*TextureEventCallback)(const TextureEventCode eventCode, void *userData);

    /// Textrue manager state.
    enum ManagerState
    {
        NotInitialized = 0,
        Alive,
        Dead,
        Resurrecting,
    };

private:
    static S32 mMasterTextureKeyIndex;
    static ManagerState mManagerState;
    static S32 mTextureResidentWasteSize;
    static S32 mTextureResidentSize;
    static S32 mTextureResidentCount;
    static S32 mBitmapResidentSize;
    static bool mForce16BitTexture;
    static bool mAllowTextureCompression;
    static bool mDisableTextureSubImageUpdates;

public:
    static bool mDGLRender;
    static GLenum mTextureCompressionHint;

public:
    static void create();
    static void destroy();
    static ManagerState getManagerState( void ) { return mManagerState; }

    static void killManager();
    static void resurrectManager();
    static void flush();
    static S32 getBitmapResidentSize( void ) { return mBitmapResidentSize; }
    static S32 getTextureResidentSize( void ) { return mTextureResidentSize; }
    static S32 getTextureResidentWasteSize( void ) { return mTextureResidentWasteSize; }
    static S32 getTextureResidentCount( void ) { return mTextureResidentCount; }

    static U32  registerEventCallback(TextureEventCallback, void *userData);
    static void unregisterEventCallback(const U32 callbackKey);

    static StringTableEntry getUniqueTextureKey( void );

    static void dumpMetrics( void );

private:
    static void postTextureEvent(const TextureEventCode eventCode);

    static void createGLName( TextureObject* pTextureObject );
    static TextureObject* registerTexture(const char *textureName, GBitmap* pNewBitmap, TextureHandle::TextureHandleType type, bool clampToEdge);
    static TextureObject* loadTexture(const char *textureName, TextureHandle::TextureHandleType type, bool clampToEdge, bool checkOnly = false, bool force16Bit = false );
    static void freeTexture( TextureObject* pTextureObject );
	static void reload(TextureObject* pTextureObject);
    static void refresh(TextureObject* pTextureObject);

    static GBitmap* loadBitmap(const char *textureName, bool recurse = true, bool nocompression = false);
    static GBitmap* createPowerOfTwoBitmap( GBitmap* pBitmap );
    static U16* create16BitBitmap( GBitmap *pDL, U8 *in_source8, GBitmap::BitmapFormat alpha_info, GLint *GLformat, GLint *GLdata_type, U32 width, U32 height );
    static void getSourceDestByteFormat(GBitmap *pBitmap, U32 *sourceFormat, U32 *destFormat, U32 *byteFormat, U32* texelSize);
    static F32 getResidentFraction( void );
};

#endif // _TEXTURE_MANAGER_H_