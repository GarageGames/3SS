//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "AISeekComponent.h"

IMPLEMENT_CO_NETOBJECT_V1(AISeekComponent);

AISeekComponent::AISeekComponent()
{
   mOwner = NULL;
}

AISeekComponent::~AISeekComponent()
{
}

bool AISeekComponent::onAdd()
{
	if (!Parent::onAdd())
		return false;
	
	return true;
}

void AISeekComponent::onRemove()
{
	Parent::onRemove();
}

void AISeekComponent::initPersistFields()
{
	Parent::initPersistFields();

	addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, AISeekComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );

	addField("weight", TypeF32, Offset(mWeight, AISeekComponent), 
      "Relative weight amongst all AI movement components." );
}

//-----------------------------------------------------------------------------

bool AISeekComponent::onComponentAdd(SimComponent *target)
{
	if (!Parent::onComponentAdd(target))
		return false;
  
	//
	AIMovementComponent *owner = dynamic_cast<AIMovementComponent*>(target);
	if (!owner)
	{
		Con::warnf("AISeekComponent::onComponentAdd - Must be added to an AIMovementComponent.");
		return false;
	}

	// Store our owner
	mOwner = owner;

	return true;
}

void AISeekComponent::onComponentRemove(SimComponent *target)
{
	Parent::onComponentRemove(target);
}

//-----------------------------------------------------------------------------

Vector2 AISeekComponent::calculateForce(AIMovementComponent::MovementParameters& params)
{
   // Should we use our specific seek target, or the one provided?
   Point2F target = params.target;
   if (!mSeekTargetObject.isNull())
   {
      target = mSeekTargetObject->getPosition();
   }

   return AIMovementComponent::calculateSeek(target, params.ownerPosition, params.ownerVelocity, params.ownerMaxSpeed);
}

//-----------------------------------------------------------------------------

void AISeekComponent::setSeekTargetObject(SceneObject* object)
{
   mSeekTargetObject = object;
}

ConsoleMethod(AISeekComponent, getSeekTargetObject, S32, 2, 2, 
   "Gives the ID of the targeted object.\n"
   "This is the target object set on this component using setSeekTargetObject() and not from "
   "the parent AIMovementComponent.\n"
   "@return The SimObjectID of the targetd object, or 0 if there is none.")
{
   SceneObject* target = object->getSeekTargetObject();
   if (target == NULL)
      return 0;
   else
      return target->getId();
}

ConsoleMethod(AISeekComponent, setSeekTargetObject, bool, 3, 3, 
   "(SimObejctID id) - Sets the object to target with the AI component.\n"
   "This overrides any target object set by the parent AIMovementComponent.\n"
   "@param id The SimObjectID of the object to target, or 0 to clear the target.\n"
   "@return True if the object has been made the target.  False if there was a problem.")
{
   SceneObject* target = dynamic_cast<SceneObject *>(Sim::findObject( argv[2] ));
   if (target != NULL)
   {
      object->setSeekTargetObject(target);
      return true;
   }

   object->setSeekTargetObject(NULL);

   return false;
}
