//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AIWANDERCOMPONENT_H_
#define _AIWANDERCOMPONENT_H_

#include "component/dynamicConsoleMethodComponent.h"
#include "AIMovementComponent.h"
#include "IAIMovement.h"

class AIWanderComponent : public DynamicConsoleMethodComponent, public IAIMovement
{
	typedef DynamicConsoleMethodComponent Parent;

protected:

   AIMovementComponent* mOwner;

   // Radius of the wandering target about the owner
   F32 mWanderRadius;

   // The distance the wandering circle is projected in front
   F32 mWanderDistance;

   // The calculated target we point to
   Vector2 mWanderTarget;

   // Amount of jitter per second
   F32 mWanderJitter;

public:

	DECLARE_CONOBJECT(AIWanderComponent);

	AIWanderComponent();
   virtual ~AIWanderComponent();

	bool onAdd();
	void onRemove();

	static void initPersistFields();

	//
	virtual bool onComponentAdd(SimComponent *target);
	virtual void onComponentRemove(SimComponent *target);

   //
   virtual Vector2 calculateForce(AIMovementComponent::MovementParameters& params);
};

#endif   // _AISEEKCOMPONENT_H_
