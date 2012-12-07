//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "AIWanderComponent.h"

// Returns a random number between -1 and 1
#define RNDCLAMPED (gRandGen.randF() - gRandGen.randF())

IMPLEMENT_CO_NETOBJECT_V1(AIWanderComponent);

AIWanderComponent::AIWanderComponent()
{
   mOwner = NULL;

   mWanderJitter = 120.0;
   mWanderRadius = 10.0f;
   mWanderDistance = 50.0f;

   F32 angle = (F32)(gRandGen.randF() * M_2PI);
   mWanderTarget.Set(mWanderRadius*cos(angle), mWanderRadius*sin(angle));
}

AIWanderComponent::~AIWanderComponent()
{
}

bool AIWanderComponent::onAdd()
{
	if (!Parent::onAdd())
		return false;
	
	return true;
}

void AIWanderComponent::onRemove()
{
	Parent::onRemove();
}

void AIWanderComponent::initPersistFields()
{
	Parent::initPersistFields();

	addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, AIWanderComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );

	addField("weight", TypeF32, Offset(mWeight, AIWanderComponent), 
      "Relative weight amongst all AI movement components." );

	addField("wanderJitter", TypeF32, Offset(mWanderJitter, AIWanderComponent), 
      "Amount of jitter to apply to the wandering target per second." );

	addField("wanderRadius", TypeF32, Offset(mWanderRadius, AIWanderComponent), 
      "Radius of the wandering target's circle." );

	addField("wanderDistance", TypeF32, Offset(mWanderDistance, AIWanderComponent), 
      "The distance the wandering circle is projected in front." );
}

//-----------------------------------------------------------------------------

bool AIWanderComponent::onComponentAdd(SimComponent *target)
{
	if (!Parent::onComponentAdd(target))
		return false;
  
	//
	AIMovementComponent *owner = dynamic_cast<AIMovementComponent*>(target);
	if (!owner)
	{
		Con::warnf("AIWanderComponent::onComponentAdd - Must be added to an AIMovementComponent.");
		return false;
	}

	// Store our owner
	mOwner = owner;

	return true;
}

void AIWanderComponent::onComponentRemove(SimComponent *target)
{
	Parent::onComponentRemove(target);
}

//-----------------------------------------------------------------------------

Vector2 AIWanderComponent::calculateForce(AIMovementComponent::MovementParameters& params)
{
   F32 jitter = mWanderJitter * params.elapsedTime;

   // Add a small random vector to the target
   mWanderTarget += Vector2( RNDCLAMPED * jitter, RNDCLAMPED * jitter);

   // Place target onto our wandering circle
   mWanderTarget.Normalize();
   mWanderTarget *= mWanderRadius;

   // Put the target out in front of the owner
   Vector2 target = mWanderTarget + Vector2(0.0f, -mWanderDistance);

   // Rotate the target into the world
   target.rotate(params.ownerMotionAngle);

   // Steer towards it
   //return target;
   target.Normalize();
   Vector2 desiredVelocity = target * params.ownerMaxSpeed;
   return desiredVelocity;
}
