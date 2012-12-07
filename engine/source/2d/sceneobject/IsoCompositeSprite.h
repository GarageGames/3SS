//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ISO_COMPOSITE_SPRITE_H_
#define _ISO_COMPOSITE_SPRITE_H_

#ifndef _COMPOSITE_SPRITE_H_
#include "2d/sceneobject/CompositeSprite.h"
#endif

//------------------------------------------------------------------------------  

class IsoCompositeSprite : public CompositeSprite
{
private:
    typedef CompositeSprite Parent;

public:
    virtual SpriteBatchItem* createSprite( const LogicalPosition& logicalPosition );

    /// Declare Console Object.
    DECLARE_CONOBJECT( IsoCompositeSprite );
};

#endif // _ISO_COMPOSITE_SPRITE_H_
