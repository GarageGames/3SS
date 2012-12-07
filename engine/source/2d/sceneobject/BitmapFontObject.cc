//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _DGL_H_
#include "graphics/dgl.h"
#endif

#include "console/consoleTypes.h"

#include "io/bitStream.h"

#include "string/stringBuffer.h"

#include "BitmapFontObject.h"

// Script bindings.
#include "BitmapFontObject_ScriptBinding.h"

//------------------------------------------------------------------------------

static EnumTable::Enums textAlignmentEnums[] = 
{
    { BitmapFontObject::ALIGN_LEFT,      "Left" },
    { BitmapFontObject::ALIGN_CENTER,    "Center" },
    { BitmapFontObject::ALIGN_RIGHT,     "Right" },
};

static EnumTable gTextAlignmentTable(3, &textAlignmentEnums[0]); 

//-----------------------------------------------------------------------------

BitmapFontObject::TextAlignment getTextAlignmentEnum(const char* label)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(textAlignmentEnums) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(textAlignmentEnums[i].label, label) == 0)
            return (BitmapFontObject::TextAlignment)textAlignmentEnums[i].index;
    }

    // Warn!
    Con::warnf("BitmapFontObject::getTextAlignmentEnum() - Invalid text alignment of '%s'", label );

    // Bah!
    return BitmapFontObject::ALIGN_CENTER;
}

//-----------------------------------------------------------------------------

const char* getTextAlignmentDescription(const BitmapFontObject::TextAlignment alignment)
{
    // Search for Mnemonic.
    for (U32 i = 0; i < (sizeof(textAlignmentEnums) / sizeof(EnumTable::Enums)); i++)
    {
        if( textAlignmentEnums[i].index == alignment )
            return textAlignmentEnums[i].label;
    }

    // Fatal!
    AssertFatal(false, "BitmapFontObject::getTextAlignmentDescription() - Invalid text alignment.");

    // Bah!
    return StringTable->insert(textAlignmentEnums[1].label);
}

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(BitmapFontObject);

//-----------------------------------------------------------------------------

BitmapFontObject::BitmapFontObject() :
    mTextAlignment( BitmapFontObject::ALIGN_CENTER ),
    mCharacterSize( 1.0f, 1.0f ),
    mCharacterPadding( 0 ),
    mConsoleBuffer(StringTable->EmptyString)
{
   // Use a static body by default.
   mBodyDefinition.type = b2_staticBody;
}

//-----------------------------------------------------------------------------

BitmapFontObject::~BitmapFontObject()
{
}

//-----------------------------------------------------------------------------

void BitmapFontObject::initPersistFields()
{    
    // Call parent.
    Parent::initPersistFields();

    addProtectedField("imageMap", TypeImageMapAssetPtr, Offset(mImageAsset, BitmapFontObject), &setImageMap, &getImageMap, &writeImageMap, "");
    addProtectedField("text", TypeString, Offset( mConsoleBuffer, BitmapFontObject ), setText, getText, &writeText, "The text to be displayed." );  
    addProtectedField("textAlignment", TypeEnum, Offset(mTextAlignment, BitmapFontObject), &setTextAlignment, &defaultProtectedGetFn, &writeTextAlignment, 1, &gTextAlignmentTable, "");
    addProtectedField("characterSize", TypeT2DVector, Offset(mCharacterSize, BitmapFontObject), &setCharacterSize, &defaultProtectedGetFn,&writeCharacterSize, "" );
    addProtectedField("characterPadding", TypeF32, Offset(mCharacterPadding, BitmapFontObject), &setCharacterPadding, &defaultProtectedGetFn, &writeCharacterPadding, "" );
}

//-----------------------------------------------------------------------------

bool BitmapFontObject::onAdd()
{
    // Call Parent.
    if(!Parent::onAdd())
        return false;
    
    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------

void BitmapFontObject::onRemove()
{
    // Call Parent.
    Parent::onRemove();
}

//------------------------------------------------------------------------------

void BitmapFontObject::copyTo(SimObject* object)
{
    // Fetch font object.
    BitmapFontObject* pFontObject = dynamic_cast<BitmapFontObject*>(object);

    // Sanity.
    AssertFatal(pFontObject != NULL, "BitmapFontObject::copyTo() - Object is not the correct type.");

    // Call parent.
    Parent::copyTo(object);

    // Copy.
    pFontObject->setImageMap( getImageMap() );
    pFontObject->setText( getText() );
    pFontObject->setTextAlignment( getTextAlignment() );
    pFontObject->setCharacterSize( getCharacterSize() );
    pFontObject->setCharacterPadding( getCharacterPadding() );
}

//------------------------------------------------------------------------------

void BitmapFontObject::sceneRender( const SceneRenderState* pSceneRenderState, const SceneRenderRequest* pSceneRenderRequest, BatchRender* pBatchRenderer )
{
    // Finish if no imagemap asset.
    if ( mImageAsset.isNull() )
        return;

    // Fetch number of characters to render.
    const U32 renderCharacters = mText.length();

    // Ignore if no text to render.
    if( renderCharacters == 0 )
        return;

    // Fetch render OOBB.
    const Vector2& renderOOBB0 = mRenderOOBB[0];
    const Vector2& renderOOBB1 = mRenderOOBB[1];
    const Vector2& renderOOBB3 = mRenderOOBB[3];

    Vector2 characterOOBB0;
    Vector2 characterOOBB1;
    Vector2 characterOOBB2;
    Vector2 characterOOBB3;

    // Calculate the starting render position based upon text alignment.
    switch( mTextAlignment )
    {
        case ALIGN_LEFT:
            {
                // Size is twice the padded text width as we're aligning to the left from the position expanding rightwards.
                characterOOBB0.Set( (renderOOBB0.x + renderOOBB1.x)*0.5f, renderOOBB0.y );
            }
            break;

        case ALIGN_RIGHT:
            {
                // Size is twice the padded text width as we're aligning to the right from the position expanding leftwards.
                characterOOBB0 = renderOOBB0;
            }
            break;

        default:
            {
                // Warn.
                Con::warnf("BitmapFontObject() - Unknown text alignment!");
            }
        case ALIGN_CENTER:
            {
                // Size is the total padded text size as we're simply centered on the position.
                characterOOBB0 = renderOOBB0;
            }
            break;
    }

    // Calculate character width stride.
    Vector2 characterWidthStride = (renderOOBB1 - renderOOBB0);
    characterWidthStride.Normalize( mCharacterSize.x + mCharacterPadding );

    // Calculate character height stride.
    Vector2 characterHeightStride = (renderOOBB3 - renderOOBB0);
    characterHeightStride.Normalize( mCharacterSize.y );

    // Complete character OOBB.
    characterOOBB1 = characterOOBB0 + characterWidthStride;
    characterOOBB2 = characterOOBB1 + characterHeightStride;
    characterOOBB3 = characterOOBB2 - characterWidthStride;

    // Render all the characters.    
    for( U32 characterIndex = 0; characterIndex < renderCharacters; ++characterIndex )
    {
        // Fetch character.
        U32 character = mText.getChar( characterIndex );

        // Set character to "space" if it's out of bounds.
        if ( character < 32 || character > 128 )
            character = 32;

        // Calculate character frame index.
        const U32 characterFrameIndex = character - 32;

        // Fetch current frame area.
        const ImageAsset::FrameArea::TexelArea& texelFrameArea = mImageAsset->getImageFrameArea( characterFrameIndex ).mTexelArea;

        // Fetch lower/upper texture coordinates.
        const Vector2& texLower = texelFrameArea.mTexelLower;
        const Vector2& texUpper = texelFrameArea.mTexelUpper;

        // Submit batched quad.
        pBatchRenderer->SubmitQuad(
            characterOOBB0,
            characterOOBB1,
            characterOOBB2,
            characterOOBB3,
            Vector2( texLower.x, texUpper.y ),
            Vector2( texUpper.x, texUpper.y ),
            Vector2( texUpper.x, texLower.y ),
            Vector2( texLower.x, texLower.y ),
            mImageAsset->getImageTexture() );

        // Translate character OOBB.
        characterOOBB0 += characterWidthStride;
        characterOOBB1 += characterWidthStride;
        characterOOBB2 += characterWidthStride;
        characterOOBB3 += characterWidthStride;
    }
}


//-----------------------------------------------------------------------------

bool BitmapFontObject::setImageMap( const char* pImageMapAssetId )
{
    // Set asset.
    mImageAsset = pImageMapAssetId;

    // Finish if no imagemap asset.
    if ( mImageAsset.isNull() )
        return false;

    // We need a minimum of 96 frames here.
    if ( mImageAsset->getFrameCount() < 96 )
    {
        // Warn.
        Con::warnf("BitmapFontObject::setImageMap() - The image-map needs to have at least 96 frames to be used as a font! (%s)", mImageAsset.getAssetId() );
        mImageAsset.clear();
        return false;
    }
    
    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------

void BitmapFontObject::setText( const StringBuffer& text )
{
    // Set text.
    mText.set( &text );
    calculateSpatials();   
}

//-----------------------------------------------------------------------------

void BitmapFontObject::setTextAlignment( const TextAlignment alignment )
{
    mTextAlignment = alignment;
    calculateSpatials();
}

//-----------------------------------------------------------------------------

void BitmapFontObject::setCharacterSize( const Vector2& size )
{
    mCharacterSize = size;
    mCharacterSize.clampZero();
    calculateSpatials();
}

//-----------------------------------------------------------------------------

void BitmapFontObject::setCharacterPadding( const U32 padding )
{
    mCharacterPadding = padding;
    calculateSpatials();
}

//-----------------------------------------------------------------------------

void BitmapFontObject::calculateSpatials( void )
{
    // Fetch number of characters to render.
    const U32 renderCharacters = mText.length();

    // Set size as a single character if no text.
    if ( renderCharacters == 0 )
    {
        setSize( mCharacterSize );
        return;
    }

    // Calculate total character padding.
    const U32 totalCharacterPadding = (renderCharacters * mCharacterPadding) - mCharacterPadding;

    // Calculate total character size.
    const Vector2 totalCharacterSize( renderCharacters * mCharacterSize.x, mCharacterSize.y );

    // Calculate total padded text size.
    const Vector2 totalPaddedTextSize( totalCharacterSize.x + totalCharacterPadding, totalCharacterSize.y );

    // Calculate size (AABB) including alignment relative to position.
    // NOTE:    For left/right alignment we have to double the size width as clipping is based upon size and
    //          we're aligning to the position here.  We cannot align to the size AABB itself as that changes
    //          as the text length changes therefore it is not a stable point from which to align from.
    switch( mTextAlignment )
    {
        case ALIGN_LEFT:
            {
                // Size is twice the padded text width as we're aligning to the left from the position expanding rightwards.
                setSize( totalPaddedTextSize * 2.0f );
            }
            break;

        case ALIGN_RIGHT:
            {
                // Size is twice the padded text width as we're aligning to the right from the position expanding leftwards.
                setSize( totalPaddedTextSize * 2.0f );
            }
            break;

        case ALIGN_CENTER:
            {
                // Size is the total padded text size as we're simply centered on the position.
                setSize( totalPaddedTextSize );
            }
            break;

        default:
            {
                // Warn.
                Con::warnf("BitmapFontObject() - Unknown text alignment!");
            }
    }
}

//-----------------------------------------------------------------------------

bool BitmapFontObject::setTextAlignment( void* obj, const char* data )
{
    static_cast<BitmapFontObject*>( obj )->setTextAlignment( getTextAlignmentEnum(data) ); return false;
}
