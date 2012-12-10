//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLE_H_
#include "console/console.h"
#endif

#ifndef _CONSOLEINTERNAL_H_
#include "console/consoleInternal.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _PLATFORM_H_
#include "platform/platform.h"
#endif

#ifndef _GBITMAP_H_
#include "graphics/gBitmap.h"
#endif

#ifndef _UTILITY_H_
#include "2d/core/Utility.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

// Script bindings.
#include "ImageAsset_ScriptBinding.h"

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(ImageAsset);

//------------------------------------------------------------------------------

static EnumTable::Enums textureFilterLookup[] =
                {
                { ImageAsset::FILTER_NEAREST,     "NONE"      },
                { ImageAsset::FILTER_BILINEAR,    "SMOOTH"    },
                };

EnumTable textureFilterTable(sizeof(textureFilterLookup) / sizeof(EnumTable::Enums), &textureFilterLookup[0]);

//------------------------------------------------------------------------------

ImageAsset::TextureFilterMode getFilterModeEnum(const char* label)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(textureFilterLookup) / sizeof(EnumTable::Enums)); i++)
        if( dStricmp(textureFilterLookup[i].label, label) == 0)
            return((ImageAsset::TextureFilterMode)textureFilterLookup[i].index);

    // Bah!
    return ImageAsset::FILTER_INVALID;
}

//------------------------------------------------------------------------------

const char* getFilterModeDescription( ImageAsset::TextureFilterMode filterMode )
{
    // Search for Mode.
    for(U32 i = 0; i < (sizeof(textureFilterLookup) / sizeof(EnumTable::Enums)); i++)
        if( textureFilterLookup[i].index == filterMode )
            return textureFilterLookup[i].label;

    // Bah!
    return "Undefined ImageMap Error; please report this problem!";
}

//------------------------------------------------------------------------------

ImageAsset::ImageAsset() :  mImageFile(StringTable->EmptyString),
                            mForce16Bit(false),
                            mLocalFilterMode(FILTER_INVALID),

                            mCellRowOrder(true),
                            mCellOffsetX(0),
                            mCellOffsetY(0),
                            mCellStrideX(0),
                            mCellStrideY(0),
                            mCellCountX(0),
                            mCellCountY(0),
                            mCellWidth(0),
                            mCellHeight(0),

                            mImageTextureHandle(NULL)
{
    // Set Vector Associations.
    VECTOR_SET_ASSOCIATION( mFrames );

    // Set filter mode.    
    setFilterMode( FILTER_NEAREST );
}

//------------------------------------------------------------------------------

ImageAsset::~ImageAsset()
{
}

//------------------------------------------------------------------------------

void ImageAsset::initPersistFields()
{
    // Call parent.
    Parent::initPersistFields();

    // Fields.
    addProtectedField("ImageFile", TypeAssetLooseFilePath, Offset(mImageFile, ImageAsset), &setImageFile, &getImageFile, &defaultProtectedWriteFn, "");
    addProtectedField("Force16bit", TypeBool, Offset(mForce16Bit, ImageAsset), &setForce16Bit, &defaultProtectedGetFn, &writeForce16Bit, "");
    addProtectedField("FilterMode", TypeEnum, Offset(mLocalFilterMode, ImageAsset), &setFilterMode, &defaultProtectedGetFn, &writeFilterMode, 1, &textureFilterTable);
   
    addProtectedField("CellRowOrder", TypeBool, Offset(mCellRowOrder, ImageAsset), &setCellRowOrder, &defaultProtectedGetFn, &writeCellRowOrder, "");
    addProtectedField("CellOffsetX", TypeS32, Offset(mCellOffsetX, ImageAsset), &setCellOffsetX, &defaultProtectedGetFn, &writeCellOffsetX, "");
    addProtectedField("CellOffsetY", TypeS32, Offset(mCellOffsetY, ImageAsset), &setCellOffsetY, &defaultProtectedGetFn, &writeCellOffsetY, "");
    addProtectedField("CellStrideX", TypeS32, Offset(mCellStrideX, ImageAsset), &setCellStrideX, &defaultProtectedGetFn, &writeCellStrideX, "");
    addProtectedField("CellStrideY", TypeS32, Offset(mCellStrideY, ImageAsset), &setCellStrideY, &defaultProtectedGetFn, &writeCellStrideY, "");
    addProtectedField("CellCountX", TypeS32, Offset(mCellCountX, ImageAsset), &setCellCountX, &defaultProtectedGetFn, &writeCellCountX, "");
    addProtectedField("CellCountY", TypeS32, Offset(mCellCountY, ImageAsset), &setCellCountY, &defaultProtectedGetFn, &writeCellCountY, "");
    addProtectedField("CellWidth", TypeS32, Offset(mCellWidth, ImageAsset), &setCellWidth, &defaultProtectedGetFn, &writeCellWidth, "");
    addProtectedField("CellHeight", TypeS32, Offset(mCellHeight, ImageAsset), &setCellHeight, &defaultProtectedGetFn, &writeCellHeight, "");
}

//------------------------------------------------------------------------------

bool ImageAsset::onAdd()
{
    // Call Parent.
    if (!Parent::onAdd())
       return false;

    return true;
}

//------------------------------------------------------------------------------

void ImageAsset::onRemove()
{
    // Call Parent.
    Parent::onRemove();
}

//------------------------------------------------------------------------------

void ImageAsset::setImageFile( const char* pImageFile )
{
    // Sanity!
    AssertFatal( pImageFile != NULL, "Cannot use a NULL image file." );

    // Fetch image file.
    pImageFile = StringTable->insert( pImageFile );

    // Ignore no change,
    if ( pImageFile == mImageFile )
        return;

    // Update.
    mImageFile = getOwned() ? expandAssetFilePath( pImageFile ) : StringTable->insert( pImageFile );

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setForce16Bit( const bool force16Bit )
{
    // Ignore no change,
    if ( force16Bit == mForce16Bit )
        return;

    // Update.
    mForce16Bit = force16Bit;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setFilterMode( const ImageAsset::TextureFilterMode filterMode )
{
    // Ignore no change,
    if ( filterMode == mLocalFilterMode )
        return;

    // Invalid filter mode?
    if ( filterMode == FILTER_INVALID )
    {
        // Yes, so warn.
        Con::warnf( "Cannot set an invalid filter mode." );
        return;
    }

    // Update.
    mLocalFilterMode = filterMode;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellRowOrder( const bool cellRowOrder )
{
    // Ignore no change.
    if ( cellRowOrder == mCellRowOrder )
        return;

    // Update.
    mCellRowOrder = cellRowOrder;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellOffsetX( const S32 cellOffsetX )
{
    // Ignore no change.
    if ( cellOffsetX == mCellOffsetX )
        return;

    // Valid?
    if ( cellOffsetX < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL offset X '%d'.", cellOffsetX );
        return;
    }

    // Update.
    mCellOffsetX = cellOffsetX;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellOffsetY( const S32 cellOffsetY )
{
    // Ignore no change.
    if ( cellOffsetY == mCellOffsetY )
        return;

    // Valid?
    if ( cellOffsetY < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL offset Y '%d'.", cellOffsetY );
        return;
    }

    // Update.
    mCellOffsetY = cellOffsetY;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellStrideX( const S32 cellStrideX )
{
    // Ignore no change.
    if ( cellStrideX == mCellStrideX )
        return;

    // Valid?
    if ( cellStrideX < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL stride X '%d'.", cellStrideX );
        return;
    }

    // Update.
    mCellStrideX = cellStrideX;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellStrideY( const S32 cellStrideY )
{
    // Ignore no change.
    if ( cellStrideY == mCellStrideY )
        return;

    // Valid?
    if ( cellStrideY < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL stride Y '%d'.", cellStrideY );
        return;
    }

    // Update.
    mCellStrideY = cellStrideY;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellCountX( const S32 cellCountX )
{
    // Ignore no change.
    if ( cellCountX == mCellCountX )
        return;

    // Valid?
    if ( cellCountX < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL count X '%d'.", cellCountX );
        return;
    }

    // Update.
    mCellCountX = cellCountX;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellCountY( const S32 cellCountY )
{
    // Ignore no change.
    if ( cellCountY == mCellCountY )
        return;

    // Valid?
    if ( cellCountY < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL count Y '%d'.", cellCountY );
        return;
    }

    // Update.
    mCellCountY = cellCountY;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellWidth( const S32 cellWidth )
{
    // Ignore no change.
    if ( cellWidth == mCellWidth )
        return;

    // Valid?
    if ( cellWidth < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL width '%d'.", cellWidth );
        return;
    }

    // Update.
    mCellWidth = cellWidth;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setCellHeight( const S32 cellheight )
{
    // Ignore no change.
    if ( cellheight == mCellHeight )
        return;

    // Valid?
    if ( cellheight < 0 )
    {
        // No, so warn.
        Con::warnf( "Invalid CELL height '%d'.", cellheight );
        return;
    }

    // Update.
    mCellHeight = cellheight;

    // Refresh the asset.
    refreshAsset();
}

//------------------------------------------------------------------------------

void ImageAsset::setTextureFilter( const TextureFilterMode filterMode )
{
    // Finish if no texture.
    if ( mImageTextureHandle.IsNull() )
        return;

    // Select Hardware Filter Mode.
    GLint glFilterMode;

    switch( filterMode )
    {
        // Nearest ("none").
        case FILTER_NEAREST:
        {
            glFilterMode = GL_NEAREST;

        } break;

        // Bilinear ("smooth").
        case FILTER_BILINEAR:
        {
            glFilterMode = GL_LINEAR;

        } break;

        // Huh?
        default:
            // Oh well...
            glFilterMode = GL_LINEAR;
    };

    // Set the texture objects filter mode.
    mImageTextureHandle.setFilter( glFilterMode );
}

//------------------------------------------------------------------------------

void ImageAsset::initializeAsset( void )
{
    // Call parent.
    Parent::initializeAsset();

    // Ensure the image-file is expanded.
    mImageFile = expandAssetFilePath( mImageFile );

    // Calculate the image-map.
    calculateImageMap();
}

//------------------------------------------------------------------------------

void ImageAsset::onAssetReload( void ) 
{
	// Ignore if not yet added to the sim.
    if ( !isProperlyAdded() )
        return;

    // Call parent.
    Parent::onAssetReload();
    
    if( !mImageTextureHandle.IsNull() )
	{
		mImageTextureHandle.reload();
	}
}

//------------------------------------------------------------------------------

void ImageAsset::onAssetRefresh( void ) 
{
    // Ignore if not yet added to the sim.
    if ( !isProperlyAdded() )
        return;

    // Call parent.
    Parent::onAssetRefresh();
    
    // Compile image-map.
    calculateImageMap();
}

//-----------------------------------------------------------------------------

void ImageAsset::onTamlPreWrite( void )
{
    // Call parent.
    Parent::onTamlPreWrite();

    // Ensure the image-file is collapsed.
    mImageFile = collapseAssetFilePath( mImageFile );
}

//-----------------------------------------------------------------------------

void ImageAsset::onTamlPostWrite( void )
{
    // Call parent.
    Parent::onTamlPostWrite();

    // Ensure the image-file is expanded.
    mImageFile = expandAssetFilePath( mImageFile );
}

//------------------------------------------------------------------------------

void ImageAsset::calculateImageMap( void )
{
    // Clear frames.
    mFrames.clear();

    // Get image texture.
    mImageTextureHandle.set( mImageFile, TextureHandle::BitmapTexture, true, getForce16Bit() );

    // Is the texture valid?
    if ( mImageTextureHandle.IsNull() )
    {
        // No, so warn.
        Con::warnf( "ImageMap '%s' could not load texture '%s'.", getAssetId(), mImageFile );
        return;
    }

    // Fetch global filter.
    const char* pGlobalFilter = Con::getVariable( "$pref::T2D::imageAssetGlobalFilterMode" );

    // Fetch global filter mode.
    TextureFilterMode filterMode = getFilterModeEnum( pGlobalFilter );

    // If global filter mode is invalid then use local filter mode.
    if ( filterMode == FILTER_INVALID )
        filterMode = mLocalFilterMode;

    // Set filter mode.
    if ( filterMode != FILTER_INVALID )
    {
        // Set filter mode if valid.
        setTextureFilter( filterMode );
    }
    else
    {
        // Set to nearest if invalid.
        setTextureFilter( FILTER_NEAREST );
    }

    // Fetch the texture object.
    TextureObject* pTextureObject = ((TextureObject*)mImageTextureHandle);

    // Calculate texel scales.
    const F32 texelWidthScale = 1.0f / (F32)pTextureObject->getTextureWidth();
    const F32 texelHeightScale = 1.0f / (F32)pTextureObject->getTextureHeight();

    // Fetch the original image dimensions.
    const S32 imageWidth = getImageWidth();
    const S32 imageHeight = getImageHeight();

    // Set full-frame as default.
    FrameArea frameArea( 0, 0, imageWidth, imageHeight, texelWidthScale, texelHeightScale );
    mFrames.push_back( frameArea );

    // Finish if no cell counts are specified.  This is how we default to full-frame mode.
    if ( mCellCountX < 1 || mCellCountY < 1 )
        return;

    // The cell width needs to be at maximum the image width!
    if ( mCellWidth < 1 || mCellWidth >imageWidth )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell Width of %d.", mCellWidth );
        return;
    }

    // The cell height needs to be at maximum the image height!
    if ( mCellHeight < 1 || mCellHeight > imageWidth )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell Height of %d.", mCellHeight );
        return;
    }

    // The Cell Offset X needs to be within the image.
    if ( mCellOffsetX < 0 || mCellOffsetX >=imageWidth )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell OffsetX of %d.", mCellOffsetX );
        return;
    }

    // The Cell Offset Y needs to be within the image.
    if ( mCellOffsetY < 0 || mCellOffsetY >= imageWidth )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell OffsetY of %d.", mCellOffsetY );
        return;
    }

    // Are we using Cell-StrideX?
    S32 cellStepX;
    if ( mCellStrideX != 0 )
    {
        // Yes, so set stepX to be StrideX.
        cellStepX = mCellStrideX;
    }
    else
    {
        // No, so set stepY to be Cell Width.
        cellStepX = mCellWidth;
    }

    // Are we using Cell-StrideY?
    S32 cellStepY;
    if ( mCellStrideY != 0 )
    {
        // Yes, so set stepY to be StrideY.
        cellStepY = mCellStrideY;
    }
    else
    {
        // No, so set stepY to be Cell Height.
        cellStepY = mCellHeight;
    }

    // Calculate Final Cell Position X.
    S32 cellFinalPositionX = mCellOffsetX + ((mCellCountX-((cellStepX<0)?1:0))*cellStepX);
    // Off Left?
    if ( cellFinalPositionX < 0 )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell OffsetX(%d)/Width(%d)/CountX(%d); off image left-hand-side.", mCellOffsetX, mCellWidth, mCellCountX );
        return;
    }
            // Off Right?
    else if ( cellFinalPositionX > imageWidth )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell OffsetX(%d)/Width(%d)/CountX(%d); off image right-hand-side.", mCellOffsetX, mCellWidth, mCellCountX );
        return;
    }

    // Calculate Final Cell Position Y.
    S32 cellFinalPositionY = mCellOffsetY + ((mCellCountY-((cellStepY<0)?1:0))*cellStepY);
    // Off Top?
    if ( cellFinalPositionY < 0 )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell OffsetY(%d)/Height(%d)/CountY(%d); off image top-side.", mCellOffsetY, mCellHeight, mCellCountY );
        return;
    }
            // Off Bottom?
    else if ( cellFinalPositionY > imageHeight )
    {
        // Warn.
        Con::warnf("ImageAsset::calculateImageMap() - Invalid Cell OffsetY(%d)/Height(%d)/CountY(%d); off image bottom-side.", mCellOffsetY, mCellHeight, mCellCountY );
        return;
    }

    // Clear default frame.
    mFrames.clear();

    // Cell Row Order?
    if ( mCellRowOrder )
    {
        // Yes, so RowRow Order.
        for ( S32 y = 0, cellPositionY = mCellOffsetY; y < mCellCountY; y++, cellPositionY+=cellStepY )
        {
            for ( S32 x = 0, cellPositionX = mCellOffsetX; x < mCellCountX; x++, cellPositionX+=cellStepX )
            {
                // Set frame area.
                frameArea.setArea( cellPositionX, cellPositionY, mCellWidth, mCellHeight, texelWidthScale, texelHeightScale );

                // Store fame.
                mFrames.push_back( frameArea );
            }
        }

        return;
    }

    // No, so Column Order.
    for ( S32 x = 0, cellPositionX = mCellOffsetX; x < mCellCountX; x++, cellPositionX+=cellStepX )
    {
        for ( S32 y = 0, cellPositionY = mCellOffsetY; y < mCellCountY; y++, cellPositionY+=cellStepY )
        {
            // Set frame area.
            frameArea.setArea( cellPositionX, cellPositionY, mCellWidth, mCellHeight, texelWidthScale, texelHeightScale );

            // Store fame.
            mFrames.push_back( frameArea );
        }
    }
}

//------------------------------------------------------------------------------

bool ImageAsset::setFilterMode( void* obj, const char* data )
{
    static_cast<ImageAsset*>(obj)->setFilterMode(getFilterModeEnum(data));
    return false;
}
