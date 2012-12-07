//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PHYSICS_PROXY_H_
#define _PHYSICS_PROXY_H_

//-----------------------------------------------------------------------------

class PhysicsProxy
{
public:
    PhysicsProxy() {}
    virtual ~PhysicsProxy() {}

    //// Specifies the proxy type.
    enum ePhysicsProxyType
    {
        PHYSIC_PROXY_INVALID,

        PHYSIC_PROXY_CUSTOM,

        PHYSIC_PROXY_SCENEOBJECT,

        PHYSIC_PROXY_GROUNDBODY,
    };

    //// Get the body proxy type.
    virtual ePhysicsProxyType getPhysicsProxyType() const = 0;
};

#endif // _PHYSICS_PROXY_H_
