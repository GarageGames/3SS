//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ISO_COMPOSITE_SPRITE_H_
#include "2d/sceneobject/IsoCompositeSprite.h"
#endif

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(IsoCompositeSprite);

//------------------------------------------------------------------------------

SpriteBatchItem* IsoCompositeSprite::createSprite( const LogicalPosition& logicalPosition )
{
    // Do we have a valid logical position?
    if ( logicalPosition.mArgCount != 2 )
    {
        // No, so warn.
        Con::warnf( "Invalid logical position specified for composite rectilinear sprite." );
        return NULL;
    }

    // Fetch sprite key.
    const StringTableEntry spriteKey = getSpriteKey( logicalPosition );

    // Does the sprite already exist?
    if ( findSpriteKey( spriteKey ) != NULL )
    {
        // Yes, so warn.
        Con::warnf( "Cannot add sprite at logical position '%s' as one already exists.", spriteKey );
        return NULL;
    }

    // Create the sprite.
    SpriteBatchItem* pSpriteBatchItem = SpriteBatch::createSprite();

    // Set sprite key.
    pSpriteBatchItem->setKey( spriteKey );

    // Fetch sprite stride.
    const Vector2 spriteStride = getDefaultSpriteStride();

    // Fetch logical coordinates.
    Vector2 position( dAtof(logicalPosition.mArgs[0]), dAtof(logicalPosition.mArgs[1]) );

    // Calculate position.
    position.Set( (position.x * spriteStride.x) + (position.y * spriteStride.x), (position.x * spriteStride.y) + (position.y * -spriteStride.y) );

    // Set the sprite default position.
    pSpriteBatchItem->setLocalPosition( position );

    // Set the sprite default size and angle.
    pSpriteBatchItem->setSize( getDefaultSpriteSize() );
    pSpriteBatchItem->setLocalAngle( SpriteBatch::getDefaultSpriteAngle() );

    return pSpriteBatchItem;
}
