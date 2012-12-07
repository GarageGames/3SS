//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AISEEKCOMPONENT_H_
#define _AISEEKCOMPONENT_H_

#include "component/dynamicConsoleMethodComponent.h"
#include "AIMovementComponent.h"
#include "IAIMovement.h"

class AISeekComponent : public DynamicConsoleMethodComponent, public IAIMovement
{
	typedef DynamicConsoleMethodComponent Parent;

protected:

   AIMovementComponent* mOwner;

   SimObjectPtr<SceneObject> mSeekTargetObject;

public:

	DECLARE_CONOBJECT(AISeekComponent);

	AISeekComponent();
   virtual ~AISeekComponent();

	bool onAdd();
	void onRemove();

	static void initPersistFields();

	//
	virtual bool onComponentAdd(SimComponent *target);
	virtual void onComponentRemove(SimComponent *target);

   //
   virtual Vector2 calculateForce(AIMovementComponent::MovementParameters& params);

   SceneObject* getSeekTargetObject() { return mSeekTargetObject; }
   void setSeekTargetObject(SceneObject* object);
};

#endif   // _AISEEKCOMPONENT_H_
