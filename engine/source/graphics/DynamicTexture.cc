//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/DynamicTexture.h"
#include "graphics/TextureManager.h"

//-----------------------------------------------------------------------------

DynamicTexture::DynamicTexture() :
    mpBitmap( NULL )
{
    // Generate a texture key.
    mTextureKey = TextureManager::getUniqueTextureKey();
}

//-----------------------------------------------------------------------------

DynamicTexture::~DynamicTexture()
{
}

//-----------------------------------------------------------------------------

void DynamicTexture::setSize( const U32 texelWidth, const U32 texelHeight )
{
    // Finish if any existing bitmap is adequate.
    if ( mpBitmap != NULL && texelWidth == mpBitmap->getWidth() && texelHeight == mpBitmap->getHeight() )
        return;

    // Generate new bitmap.
    // NOTE: Any previous bitmap would be allocated to the texture handle therefore destroyed
    // when the texture handle is modified.
    mpBitmap = new GBitmap( texelWidth, texelHeight, false, GBitmap::RGBA );

    // Set texture against bitmap.
    mTextureHandle.set( mTextureKey, mpBitmap, TextureHandle::BitmapKeepTexture );
}