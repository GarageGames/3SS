//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "AIFleeComponent.h"

IMPLEMENT_CO_NETOBJECT_V1(AIFleeComponent);

AIFleeComponent::AIFleeComponent()
{
   mOwner = NULL;
}

AIFleeComponent::~AIFleeComponent()
{
}

bool AIFleeComponent::onAdd()
{
    if (!Parent::onAdd())
        return false;
    
    return true;
}

void AIFleeComponent::onRemove()
{
    Parent::onRemove();
}

void AIFleeComponent::initPersistFields()
{
    Parent::initPersistFields();

    addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, AIFleeComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );

    addField("weight", TypeF32, Offset(mWeight, AIFleeComponent), 
      "Relative weight amongst all AI movement components." );
}

//-----------------------------------------------------------------------------

bool AIFleeComponent::onComponentAdd(SimComponent *target)
{
    if (!Parent::onComponentAdd(target))
        return false;
  
    //
    AIMovementComponent *owner = dynamic_cast<AIMovementComponent*>(target);
    if (!owner)
    {
        Con::warnf("AIFleeComponent::onComponentAdd - Must be added to an AIMovementComponent.");
        return false;
    }

    // Store our owner
    mOwner = owner;

    return true;
}

void AIFleeComponent::onComponentRemove(SimComponent *target)
{
    Parent::onComponentRemove(target);
}

//-----------------------------------------------------------------------------

Vector2 AIFleeComponent::calculateForce(AIMovementComponent::MovementParameters& params)
{
   // Should we use our specific flee target, or the one provided?
   Point2F target = params.target;
   if (!mFleeTargetObject.isNull())
   {
      target = mFleeTargetObject->getPosition();
   }

   Vector2 direction = params.ownerPosition - target;
   direction.Normalize();

   Vector2 desiredVelocity = direction * params.ownerMaxSpeed;

   return desiredVelocity - params.ownerVelocity;
}

//-----------------------------------------------------------------------------

void AIFleeComponent::setFleeTargetObject(SceneObject* object)
{
   mFleeTargetObject = object;
}

ConsoleMethod(AIFleeComponent, getFleeTargetObject, S32, 2, 2, 
   "Gives the ID of the targeted object.\n"
   "This is the target object set on this component using setFleeTargetObject() and not from "
   "the parent AIMovementComponent.\n"
   "@return The SimObjectID of the targetd object, or 0 if there is none.")
{
   SceneObject* target = object->getFleeTargetObject();
   if (target == NULL)
      return 0;
   else
      return target->getId();
}

ConsoleMethod(AIFleeComponent, setFleeTargetObject, bool, 3, 3, 
   "(SimObejctID id) - Sets the object to target with the AI component.\n"
   "This overrides any target object set by the parent AIMovementComponent.\n"
   "@param id The SimObjectID of the object to target, or 0 to clear the target.\n"
   "@return True if the object has been made the target.  False if there was a problem.")
{
   SceneObject* target = dynamic_cast<SceneObject *>(Sim::findObject( argv[2] ));
   if (target != NULL)
   {
      object->setFleeTargetObject(target);
      return true;
   }

   object->setFleeTargetObject(NULL);

   return false;
}
