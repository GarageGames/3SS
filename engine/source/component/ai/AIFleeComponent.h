//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AIFLEECOMPONENT_H_
#define _AIFLEECOMPONENT_H_

#include "component/dynamicConsoleMethodComponent.h"
#include "AIMovementComponent.h"
#include "IAIMovement.h"

class AIFleeComponent : public DynamicConsoleMethodComponent, public IAIMovement
{
	typedef DynamicConsoleMethodComponent Parent;

protected:

   AIMovementComponent* mOwner;

   SimObjectPtr<SceneObject> mFleeTargetObject;

public:

	DECLARE_CONOBJECT(AIFleeComponent);

	AIFleeComponent();
   virtual ~AIFleeComponent();

	bool onAdd();
	void onRemove();

	static void initPersistFields();

	//
	virtual bool onComponentAdd(SimComponent *target);
	virtual void onComponentRemove(SimComponent *target);

   //
   virtual Vector2 calculateForce(AIMovementComponent::MovementParameters& params);

   SceneObject* getFleeTargetObject() { return mFleeTargetObject; }
   void setFleeTargetObject(SceneObject* object);
};

#endif   // _AIFLEECOMPONENT_H_
