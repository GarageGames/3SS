//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "AIPursuitComponent.h"

IMPLEMENT_CO_NETOBJECT_V1(AIPursuitComponent);

AIPursuitComponent::AIPursuitComponent()
{
   mOwner = NULL;
}

AIPursuitComponent::~AIPursuitComponent()
{
}

bool AIPursuitComponent::onAdd()
{
	if (!Parent::onAdd())
		return false;
	
	return true;
}

void AIPursuitComponent::onRemove()
{
	Parent::onRemove();
}

void AIPursuitComponent::initPersistFields()
{
	Parent::initPersistFields();

	addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, AIPursuitComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );

	addField("weight", TypeF32, Offset(mWeight, AIPursuitComponent), 
      "Relative weight amongst all AI movement components." );
}

//-----------------------------------------------------------------------------

bool AIPursuitComponent::onComponentAdd(SimComponent *target)
{
	if (!Parent::onComponentAdd(target))
		return false;
  
	//
	AIMovementComponent *owner = dynamic_cast<AIMovementComponent*>(target);
	if (!owner)
	{
		Con::warnf("AIPursuitComponent::onComponentAdd - Must be added to an AIMovementComponent.");
		return false;
	}

	// Store our owner
	mOwner = owner;

	return true;
}

void AIPursuitComponent::onComponentRemove(SimComponent *target)
{
	Parent::onComponentRemove(target);
}

//-----------------------------------------------------------------------------

Vector2 AIPursuitComponent::calculateForce(AIMovementComponent::MovementParameters& params)
{
   // We pursue agent[0] or our given agent target
   if (!params.agent[0] && mPursuitTargetObject.isNull())
   {
      return Vector2(0.0f, 0.0f);
   }

   SceneObject* agent = params.agent[0];
   if (!mPursuitTargetObject.isNull())
   {
      agent = mPursuitTargetObject;
   }

   Vector2 toAgent = agent->getPosition() - params.ownerPosition;

   // If the agent is in front and heading towards us then just seek to it.
   Vector2 ownerHeading;
   ownerHeading.setAngle(params.ownerMotionAngle);
   Vector2 agentHeading = agent->getLinearVelocity();
   agentHeading.Normalize();
   if (agentHeading.LengthSquared() < 0.0001)
   {
      agentHeading.setAngle(agent->getAngle());
   }

   F32 relativeHeading = ownerHeading.dot(agentHeading);
   if (toAgent.dot(ownerHeading) > 0 && relativeHeading < -0.965) // 0.965 = 15 degrees
   {
      Point2F agentPosition = agent->getPosition();
      return AIMovementComponent::calculateSeek(agentPosition, params.ownerPosition, params.ownerVelocity, params.ownerMaxSpeed);
   }

   // Not ahead so anticipate where the agent is heading.
   Vector2 agentVelocity = agent->getLinearVelocity();
   F32 lookAheadTime = toAgent.Length() / (params.ownerMaxSpeed + agentVelocity.Length());

   Point2F targetPosition = agent->getPosition() + agentVelocity * lookAheadTime;

   return AIMovementComponent::calculateSeek(targetPosition, params.ownerPosition, params.ownerVelocity, params.ownerMaxSpeed);
}

//-----------------------------------------------------------------------------

void AIPursuitComponent::setPursuitTargetObject(SceneObject* object)
{
   mPursuitTargetObject = object;
}

ConsoleMethod(AIPursuitComponent, getPursuitTargetObject, S32, 2, 2, 
   "Gives the ID of the targeted object.\n"
   "This is the target object set on this component using setPursuitTargetObject() and not from "
   "the parent AIMovementComponent.\n"
   "@return The SimObjectID of the targetd object, or 0 if there is none.")
{
   SceneObject* target = object->getPursuitTargetObject();
   if (target == NULL)
      return 0;
   else
      return target->getId();
}

ConsoleMethod(AIPursuitComponent, setPursuitTargetObject, bool, 3, 3, 
   "(SimObejctID id) - Sets the object to target with the AI component.\n"
   "This overrides any target object set by the parent AIMovementComponent.\n"
   "@param id The SimObjectID of the object to target, or 0 to clear the target.\n"
   "@return True if the object has been made the target.  False if there was a problem.")
{
   SceneObject* target = dynamic_cast<SceneObject *>(Sim::findObject( argv[2] ));
   if (target != NULL)
   {
      object->setPursuitTargetObject(target);
      return true;
   }

   object->setPursuitTargetObject(NULL);

   return false;
}
