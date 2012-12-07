//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AIPATHFOLLOWCOMPONENT_H_
#define _AIPATHFOLLOWCOMPONENT_H_

#include "component/dynamicConsoleMethodComponent.h"
#include "AIMovementComponent.h"
#include "IAIMovement.h"

class AIPathFollowComponent : public DynamicConsoleMethodComponent, public IAIMovement
{
	typedef DynamicConsoleMethodComponent Parent;

protected:

   AIMovementComponent* mOwner;

   SimObjectPtr<SceneObject> mPathObject;
   S32 mPathType;
   F32 mArriveDecelerationRate;
   F32 mWayPointDistance;
   bool mPathLoop;
   Point2F mPathOffset;


   S32 mDestinationNode;

   bool getCurrentWayPoint(SceneObject* path, S32 pathType, Point2F& destinationPosition);
   void setNextWayPoint(SceneObject* path, S32 pathType, bool loop);
   bool headingToLastNode(SceneObject* path, S32 pathType, bool loop);

public:

	DECLARE_CONOBJECT(AIPathFollowComponent);

	AIPathFollowComponent();
   virtual ~AIPathFollowComponent();

	bool onAdd();
	void onRemove();

	static void initPersistFields();

	//
	virtual bool onComponentAdd(SimComponent *target);
	virtual void onComponentRemove(SimComponent *target);

   //
   virtual Vector2 calculateForce(AIMovementComponent::MovementParameters& params);

   SceneObject* getPathObject() { return mPathObject; }
   void setPathObject(SceneObject* object);

   S32 getPathDestination() { return mDestinationNode; }
   void setPathDestination(S32 index);

   void setPathLoop(bool loop) { mPathLoop = loop; }
   void setPathDecelerationRate(F32 rate) { mArriveDecelerationRate = rate; }
   void setPathOffset(Point2F offset) { mPathOffset = offset; }
   void setPathWayPointDistance(F32 distance) { mWayPointDistance = distance; }

   F32 getPathDistanceToEnd(SceneObject* obj);
};

#endif   // _AIPATHFOLLOWCOMPONENT_H_
