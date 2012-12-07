//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AIPURSUITCOMPONENT_H_
#define _AIPURSUITCOMPONENT_H_

#include "component/dynamicConsoleMethodComponent.h"
#include "AIMovementComponent.h"
#include "IAIMovement.h"

class AIPursuitComponent : public DynamicConsoleMethodComponent, public IAIMovement
{
	typedef DynamicConsoleMethodComponent Parent;

protected:

   AIMovementComponent* mOwner;

   SimObjectPtr<SceneObject> mPursuitTargetObject;

public:

	DECLARE_CONOBJECT(AIPursuitComponent);

	AIPursuitComponent();
   virtual ~AIPursuitComponent();

	bool onAdd();
	void onRemove();

	static void initPersistFields();

	//
	virtual bool onComponentAdd(SimComponent *target);
	virtual void onComponentRemove(SimComponent *target);

   //
   virtual Vector2 calculateForce(AIMovementComponent::MovementParameters& params);

   SceneObject* getPursuitTargetObject() { return mPursuitTargetObject; }
   void setPursuitTargetObject(SceneObject* object);
};

#endif   // _AIPURSUITCOMPONENT_H_
