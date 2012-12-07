//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONTACT_FILTER_H_
#define _CONTACT_FILTER_H_

#ifndef BOX2D_H
#include "box2d/Box2D.h"
#endif

//-----------------------------------------------------------------------------

class ContactFilter : public b2ContactFilter
{
    virtual bool ShouldCollide(b2Fixture* fixtureA, b2Fixture* fixtureB);
};

#endif //_CONTACT_FILTER_H_
