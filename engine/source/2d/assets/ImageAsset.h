//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IMAGE_ASSET_H_
#define _IMAGE_ASSET_H_

#ifndef _ASSET_BASE_H
#include "assets/assetBase.h"
#endif

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

#ifndef _VECTOR2_H_
#include "2d/core/Vector2.h"
#endif

#ifndef _TEXTURE_MANAGER_H_
#include "graphics/TextureManager.h"
#endif

//-----------------------------------------------------------------------------

class ImageAsset : public AssetBase
{
private:
    typedef AssetBase Parent;

public:
    /// Texture Filter Mode.
    enum TextureFilterMode
    {
        FILTER_NEAREST,
        FILTER_BILINEAR,

        FILTER_INVALID,
    };

    /// Frame area.
    class FrameArea
    {
    public:
        /// Frame Pixel Area.
        class PixelArea
        {
        public:
            inline void setArea( const S32 pixelFrameOffsetX, const S32 pixelFrameOffsetY, const U32 pixelFrameWidth, const U32 pixelFrameHeight )
            {
                mPixelOffset.set( pixelFrameOffsetX, pixelFrameOffsetY );
                mPixelWidth = pixelFrameWidth;
                mPixelHeight = pixelFrameHeight;
            };

            Point2I mPixelOffset;
            U32 mPixelWidth;
            U32 mPixelHeight;
        };


        /// Frame Texel Area.
        class TexelArea
        {
        public:
            void setArea( const PixelArea& pixelArea, const F32 texelWidthScale, const F32 texelHeightScale )
            {
                mTexelLower.Set( pixelArea.mPixelOffset.x * texelWidthScale, pixelArea.mPixelOffset.y * texelHeightScale );
                mTexelWidth = pixelArea.mPixelWidth * texelWidthScale;
                mTexelHeight = pixelArea.mPixelHeight * texelHeightScale;
                mTexelUpper.Set( mTexelLower.x + mTexelWidth, mTexelLower.y + mTexelHeight );
            }

            void setFlip( const bool flipX, const bool flipY )
            {
                if ( flipX ) mSwap( mTexelLower.x, mTexelUpper.x );
                if ( flipY ) mSwap( mTexelLower.y, mTexelUpper.y );
            }

            Vector2 mTexelLower;
            Vector2 mTexelUpper;
            F32 mTexelWidth;
            F32 mTexelHeight;
        };

    public:
        FrameArea( const S32 pixelFrameOffsetX, const S32 pixelFrameOffsetY, const U32 pixelFrameWidth, const U32 pixelFrameHeight, const F32 texelWidthScale, const F32 texelHeightScale )
        {
            setArea( pixelFrameOffsetX, pixelFrameOffsetY, pixelFrameWidth, pixelFrameHeight, texelWidthScale, texelHeightScale );
        }

        void setArea( const S32 pixelFrameOffsetX, const S32 pixelFrameOffsetY, const U32 pixelFrameWidth, const U32 pixelFrameHeight, const F32 texelWidthScale, const F32 texelHeightScale )
        {
            mPixelArea.setArea( pixelFrameOffsetX, pixelFrameOffsetY, pixelFrameWidth, pixelFrameHeight );
            mTexelArea.setArea( mPixelArea, texelWidthScale, texelHeightScale );
        }

        PixelArea mPixelArea;
        TexelArea mTexelArea;
    };

private:
    typedef Vector<FrameArea> typeFrameAreaVector;

    /// Configuration.
    StringTableEntry            mImageFile;
    bool						mForce16Bit;
    TextureFilterMode           mLocalFilterMode;
    bool                        mCellRowOrder;
    S32                         mCellOffsetX;
    S32                         mCellOffsetY;
    S32                         mCellStrideX;
    S32                         mCellStrideY;
    S32                         mCellWidth;
    S32                         mCellHeight;
    S32                         mCellCountX;
    S32                         mCellCountY;

    /// Imagery.
    typeFrameAreaVector         mFrames;
    TextureHandle               mImageTextureHandle;

public:
    ImageAsset();
    virtual ~ImageAsset();

    /// Core.
    static void initPersistFields();
    virtual bool onAdd();
    virtual void onRemove();

    void                    setImageFile( const char* pImageFile );
    inline StringTableEntry getImageFile( void ) const                      { return mImageFile; };

    void                    setForce16Bit( const bool force16Bit );
    inline bool             getForce16Bit( void ) const                     { return mForce16Bit; }

    void                    setFilterMode( const TextureFilterMode filterMode );
    TextureFilterMode       getFilterMode( void ) const                     { return mLocalFilterMode; }

    void                    setCellRowOrder( const bool cellRowOrder );
    inline bool             getCellRowOrder( void ) const                   { return mCellRowOrder; }

    void                    setCellOffsetX( const S32 cellOffsetX );
    inline S32              getCellOffsetX( void ) const                    { return mCellOffsetX; }

    void                    setCellOffsetY( const S32 cellOffsetY );
    inline S32              getCellOffsetY( void ) const                    { return mCellOffsetY; }

    void                    setCellStrideX( const S32 cellStrideX );
    inline S32              getCellStrideX( void ) const                    { return mCellStrideX; }

    void                    setCellStrideY( const S32 cellStrideY );
    inline S32              getCellStrideY( void ) const                    { return mCellStrideY; }

    void                    setCellCountX( const S32 cellCountX );
    inline S32              getCellCountX( void ) const                     { return mCellCountX; }

    void                    setCellCountY( const S32 cellCountY );
    inline S32              getCellCountY( void ) const                     { return mCellCountY; }

    void                    setCellWidth( const S32 cellWidth );
    inline S32              getCellWidth( void ) const						{ return mCellWidth; }

    void                    setCellHeight( const S32 cellheight );
    S32                     getCellHeight( void) const						{ return mCellHeight; }

    inline TextureHandle&   getImageTexture( void )                         { return mImageTextureHandle; }
    inline S32              getImageWidth( void ) const                     { return mImageTextureHandle.getWidth(); }
    inline S32              getImageHeight( void ) const                    { return mImageTextureHandle.getHeight(); }
    inline U32              getFrameCount( void ) const                     { return (U32)mFrames.size(); };

    inline const FrameArea& getImageFrameArea( U32 frame ) const            { clampFrame(frame); return mFrames[frame]; };
    inline const void       bindImageTexture( void)                         { glBindTexture( GL_TEXTURE_2D, getImageTexture().getGLName() ); };

    inline void             clampFrame( U32& frame ) const                  { const U32 totalFrames = getFrameCount(); if ( frame >= totalFrames ) frame = (totalFrames == 0 ? 0 : totalFrames-1 ); };
    void                    calculateImageMap( void );

    /// Declare Console Object.
    DECLARE_CONOBJECT(ImageAsset);

private:
    void setTextureFilter( const TextureFilterMode filterMode );

protected:
    virtual void initializeAsset( void );
	virtual void onAssetReload( void );
    virtual void onAssetRefresh( void );

    /// Taml callbacks.
    virtual void onTamlPreWrite( void );
    virtual void onTamlPostWrite( void );

protected:
    static void textureEventCallback( const U32 eventCode, void *userData );

    static bool setImageFile( void* obj, const char* data )                 { static_cast<ImageAsset*>(obj)->setImageFile(data); return false; }
    static const char* getImageFile(void* obj, const char* data)            { return static_cast<ImageAsset*>(obj)->getImageFile(); }
    static bool writeImageFile( void* obj, StringTableEntry pFieldName )    { return static_cast<ImageAsset*>(obj)->getImageFile() != StringTable->EmptyString; }

    static bool setForce16Bit( void* obj, const char* data )                { static_cast<ImageAsset*>(obj)->setForce16Bit(dAtob(data)); return false; }
    static bool writeForce16Bit( void* obj, StringTableEntry pFieldName )   { return static_cast<ImageAsset*>(obj)->getForce16Bit() == true; }

    static bool setFilterMode( void* obj, const char* data );
    static bool writeFilterMode( void* obj, StringTableEntry pFieldName )   { return static_cast<ImageAsset*>(obj)->getFilterMode() != FILTER_NEAREST; }

    static bool setCellRowOrder( void* obj, const char* data )              { static_cast<ImageAsset*>(obj)->setCellRowOrder(dAtob(data)); return false; }
    static bool writeCellRowOrder( void* obj, StringTableEntry pFieldName ) { return static_cast<ImageAsset*>(obj)->getCellRowOrder() == false; }

    static bool setCellOffsetX( void* obj, const char* data )               { static_cast<ImageAsset*>(obj)->setCellOffsetX(dAtoi(data)); return false; }
    static bool writeCellOffsetX( void* obj, StringTableEntry pFieldName )  { return static_cast<ImageAsset*>(obj)->getCellOffsetX() != 0; }

    static bool setCellOffsetY( void* obj, const char* data )               { static_cast<ImageAsset*>(obj)->setCellOffsetY(dAtoi(data)); return false; }
    static bool writeCellOffsetY( void* obj, StringTableEntry pFieldName )  { return static_cast<ImageAsset*>(obj)->getCellOffsetY() != 0; }

    static bool setCellStrideX( void* obj, const char* data )               { static_cast<ImageAsset*>(obj)->setCellStrideX(dAtoi(data)); return false; }
    static bool writeCellStrideX( void* obj, StringTableEntry pFieldName )  { return static_cast<ImageAsset*>(obj)->getCellStrideX() != 0; }

    static bool setCellStrideY( void* obj, const char* data )               { static_cast<ImageAsset*>(obj)->setCellStrideY(dAtoi(data)); return false; }
    static bool writeCellStrideY( void* obj, StringTableEntry pFieldName )  { return static_cast<ImageAsset*>(obj)->getCellStrideY() != 0; }

    static bool setCellCountX( void* obj, const char* data )                { static_cast<ImageAsset*>(obj)->setCellCountX(dAtoi(data)); return false; }
    static bool writeCellCountX( void* obj, StringTableEntry pFieldName )   { return static_cast<ImageAsset*>(obj)->getCellCountX() != 0; }

    static bool setCellCountY( void* obj, const char* data )                { static_cast<ImageAsset*>(obj)->setCellCountY(dAtoi(data)); return false; }
    static bool writeCellCountY( void* obj, StringTableEntry pFieldName )   { return static_cast<ImageAsset*>(obj)->getCellCountY() != 0; }

    static bool setCellWidth( void* obj, const char* data )                 { static_cast<ImageAsset*>(obj)->setCellWidth(dAtoi(data)); return false; }
    static bool writeCellWidth( void* obj, StringTableEntry pFieldName )    { return static_cast<ImageAsset*>(obj)->getCellWidth() != 0; }

    static bool setCellHeight( void* obj, const char* data )                { static_cast<ImageAsset*>(obj)->setCellHeight(dAtoi(data)); return false; }
    static bool writeCellHeight( void* obj, StringTableEntry pFieldName )   { return static_cast<ImageAsset*>(obj)->getCellHeight() != 0; }
};

// Image-Map Console Type.
DECLARE_CONSOLETYPE(ImageAsset)

//-----------------------------------------------------------------------------

extern ImageAsset::TextureFilterMode getFilterModeEnum(const char* label);
extern const char* getFilterModeDescription( ImageAsset::TextureFilterMode filterMode );

#endif // _IMAGE_ASSET_H_
