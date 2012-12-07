//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "ContactFilter.h"
#include "2d/sceneobject/SceneObject.h"

//-----------------------------------------------------------------------------

bool ContactFilter::ShouldCollide(b2Fixture* fixtureA, b2Fixture* fixtureB)
{
    PhysicsProxy* pPhysicsProxyA = static_cast<PhysicsProxy*>(fixtureA->GetBody()->GetUserData());
    PhysicsProxy* pPhysicsProxyB = static_cast<PhysicsProxy*>(fixtureB->GetBody()->GetUserData());

    // If not scene objects then cannot collide.
    if ( pPhysicsProxyA->getPhysicsProxyType() != PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT ||
         pPhysicsProxyB->getPhysicsProxyType() != PhysicsProxy::PHYSIC_PROXY_SCENEOBJECT )
         return false;

    const SceneObject* pSceneObjectA = static_cast<SceneObject*>(pPhysicsProxyA);
    const SceneObject* pSceneObjectB = static_cast<SceneObject*>(pPhysicsProxyB);

    // No contact if either objects are suppressing collision.
    if ( pSceneObjectA->mCollisionSuppress || pSceneObjectB->mCollisionSuppress )
        return false;

    // Check group/layer masks.
    return
        (pSceneObjectA->mCollisionGroupMask & pSceneObjectB->mSceneGroupMask) != 0 &&
        (pSceneObjectB->mCollisionGroupMask & pSceneObjectA->mSceneGroupMask) != 0 &&
        (pSceneObjectA->mCollisionLayerMask & pSceneObjectB->mSceneLayerMask) != 0 &&
        (pSceneObjectB->mCollisionLayerMask & pSceneObjectA->mSceneLayerMask) != 0;
}
