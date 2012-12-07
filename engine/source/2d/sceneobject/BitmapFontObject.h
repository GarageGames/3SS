//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _BITMAP_FONT_OBJECT_H_
#define _BITMAP_FONT_OBJECT_H_

#ifndef _STRINGBUFFER_H_
#include "string/stringBuffer.h"
#endif

#ifndef _IMAGE_ASSET_H_
#include "2d/assets/ImageAsset.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _ASSET_PTR_H_
#include "assets/assetPtr.h"
#endif

#ifndef _UTILITY_H_
#include "2d/core/utility.h"
#endif

//-----------------------------------------------------------------------------

class BitmapFontObject : public SceneObject
{
    typedef SceneObject          Parent;

public:
    enum TextAlignment
    {
        ALIGN_LEFT = 0,
        ALIGN_CENTER,
        ALIGN_RIGHT };

private:
    AssetPtr<ImageAsset> mImageAsset;
    StringBuffer        mText;
    U32                 mCharacterPadding;
    Vector2           mCharacterSize;
    TextAlignment       mTextAlignment;

    struct TextCell
    {
        char CharValue;
        U32 FrameCell;

        TextCell(char charValue, U32 frameCell)
        {
            CharValue = charValue;
            FrameCell = frameCell;
        }
    };

    const char*         mConsoleBuffer;
    Vector<TextCell>    mTextCells;

private:
    void calculateSpatials( void );

public:
    BitmapFontObject();
    ~BitmapFontObject();

    static void initPersistFields();

    bool onAdd();
    void onRemove();
    void copyTo(SimObject* object);

    virtual void sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer );
    virtual bool canRender( void ) const { return mImageAsset.notNull() && mText.length() > 0; }

    bool setImageMap( const char* pImageMapAssetId );
    const char* getImageMap( void ) const                           { return mImageAsset.getAssetId(); };
    void setText( const StringBuffer& text );
    inline StringBuffer& getText( void )                                { return mText; }
    void setTextAlignment( const TextAlignment alignment );
    inline TextAlignment getTextAlignment( void ) const                 { return mTextAlignment; }
    void setCharacterSize( const Vector2& size );
    inline Vector2 getCharacterSize( void ) const                     { return mCharacterSize; }
    void setCharacterPadding( const U32 padding );
    inline U32 getCharacterPadding( void ) const                        { return mCharacterPadding; }

    // Declare Console Object.
    DECLARE_CONOBJECT(BitmapFontObject);

protected:
    static bool setImageMap(void* obj, const char* data)                { static_cast<BitmapFontObject*>(obj)->setImageMap( data ); return false; }
    static const char* getImageMap(void* obj, const char* data)         { return static_cast<BitmapFontObject*>(obj)->getImageMap(); }
    static bool writeImageMap( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(BitmapFontObject); return pCastObject->mImageAsset.notNull(); }
    static bool setText( void* obj, const char* data )                  { static_cast<BitmapFontObject*>( obj )->setText( data ); return false; }
    static const char* getText( void* obj, const char* data )           { return static_cast<BitmapFontObject*>( obj )->getText().getPtr8(); }
    static bool writeText( void* obj, StringTableEntry pFieldName )     { PREFAB_WRITE_CHECK(BitmapFontObject); return pCastObject->mText.length() != 0; }
    static bool setTextAlignment( void* obj, const char* data );
    static bool writeTextAlignment( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(BitmapFontObject); return pCastObject->getTextAlignment() != BitmapFontObject::ALIGN_CENTER; }
    static bool setCharacterSize( void* obj, const char* data )         { static_cast<BitmapFontObject*>( obj )->setCharacterSize( Utility::mGetStringElementVector(data) ); return false; }
    static bool writeCharacterSize( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(BitmapFontObject); return pCastObject->getCharacterSize().isEqual(Vector2::getOne()); }
    static bool setCharacterPadding( void* obj, const char* data )      { static_cast<BitmapFontObject*>( obj )->setCharacterPadding( dAtoi(data) ); return false; }
    static bool writeCharacterPadding( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(BitmapFontObject); return pCastObject->getCharacterPadding() != 0; }
};

//-----------------------------------------------------------------------------

extern BitmapFontObject::TextAlignment getTextAlignmentEnum(const char* label);
extern const char* getTextAlignmentDescription(const BitmapFontObject::TextAlignment alignment);

#endif // _BITMAP_FONT_OBJECT_H_
