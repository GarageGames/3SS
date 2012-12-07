//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/TextureHandle.h"
#include "graphics/TextureManager.h"
#include "platform/platformAssert.h"

//-----------------------------------------------------------------------------

TextureHandle::TextureHandle( const char* pTextureKey, TextureHandleType type, bool clampToEdge, bool force16Bit )
{
    // Sanity!
    AssertISV( type != TextureHandle::InvalidTexture, "Invalid texture type." );

    object = TextureManager::loadTexture(pTextureKey, type, clampToEdge, false, force16Bit );
    lock();
}

//-----------------------------------------------------------------------------

TextureHandle::TextureHandle( const char* pTextureKey, GBitmap* bmp, TextureHandleType type, bool clampToEdge ) 
{
    // Sanity!
    AssertISV( type != TextureHandle::InvalidTexture, "Invalid texture type." );

    object = TextureManager::registerTexture(pTextureKey, bmp, type, clampToEdge);
    lock();
}

//-----------------------------------------------------------------------------

bool TextureHandle::set( const char* pTextureKey, TextureHandleType type, bool clampToEdge, bool force16Bit ) 
{
    // Sanity!
    AssertISV( type != TextureHandle::InvalidTexture, "Invalid texture type." );

    TextureObject* newObject = TextureManager::loadTexture(pTextureKey, type, clampToEdge, false, force16Bit );
    if (newObject != object)
    {
        unlock();
        object = newObject;
        lock();
    }
    return (object != NULL);
}

//-----------------------------------------------------------------------------

bool TextureHandle::set( const char* pTextureKey, GBitmap *bmp, TextureHandleType type, bool clampToEdge ) 
{
    // Sanity!
    AssertISV( type != TextureHandle::InvalidTexture, "Invalid texture type." );

    TextureObject* newObject = TextureManager::registerTexture(pTextureKey, bmp, type, clampToEdge );
    if (newObject != object)
    {
        unlock();
        object = newObject;
        lock();
    }
    return (object != NULL);
}

//-----------------------------------------------------------------------------

void TextureHandle::reload( void )
{
    TextureManager::reload(object);
}

//-----------------------------------------------------------------------------

void TextureHandle::refresh( void )
{
    TextureManager::refresh(object);
}

//-----------------------------------------------------------------------------

const char* TextureHandle::getTextureKey( void ) const
{
    return (object ? object->mTextureKey : NULL);
}

//-----------------------------------------------------------------------------

U32 TextureHandle::getWidth( void ) const
{
    return (object ? object->mBitmapWidth : 0);
}

//-----------------------------------------------------------------------------

U32 TextureHandle::getHeight( void ) const
{
    return (object ? object->mBitmapHeight : 0);
}

//-----------------------------------------------------------------------------

GBitmap* TextureHandle::getBitmap( void )
{
    return (object ? object->mpBitmap : NULL);
}

//-----------------------------------------------------------------------------

const GBitmap* TextureHandle::getBitmap( void ) const
{
    return (object ? object->mpBitmap: NULL);
}

//-----------------------------------------------------------------------------

U32 TextureHandle::getGLName( void ) const
{
    return object == NULL ? 0 : object->mGLTextureName;
}

//-----------------------------------------------------------------------------

void TextureHandle::setFilter( const GLuint filter )
{
    // Finish if no object.
    if (object == NULL  )
        return;

    // Set filter.
    object->mFilter = filter;

    // Finish if no GL texture name.
    if ( object->mGLTextureName == 0 )
        return;

    // Set texture state.
    glBindTexture( GL_TEXTURE_2D, object->mGLTextureName );
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, filter );
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, filter );
}

//-----------------------------------------------------------------------------

void TextureHandle::setClamp( const bool clamp )
{
    // Finish if no object.
    if (object == NULL  )
        return;

    // Set clamp.
    object->mClamp = clamp;

    // Finish if no GL texture name.
    if ( object->mGLTextureName == 0 )
        return;

    // Set texture state.
    glBindTexture(GL_TEXTURE_2D, object->mGLTextureName);
    GLenum glClamp;
    if ( clamp )
        glClamp = dglDoesSupportEdgeClamp() ? GL_CLAMP_TO_EDGE : GL_CLAMP;
    else
        glClamp = GL_REPEAT;

    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, glClamp );
    glTexParameteri( GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, glClamp );
}

//-----------------------------------------------------------------------------

void TextureHandle::lock( void )
{
    if ( object == NULL )
        return;

    object->mRefCount++;
}  

//-----------------------------------------------------------------------------

void TextureHandle::unlock( void )
{
    // Finish if the texture manager is not active or the handle does not represent an object.
    // Do nothing if the manager isn't active or we do not have an object    
    if ( IsNull() || TextureManager::getManagerState() == TextureManager::NotInitialized )
        return;

    if( --object->mRefCount == 0 )
    {
        TextureManager::freeTexture(object);
    }

    object = NULL;
}
