//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "AIMovementComponent.h"
#include "IAIMovement.h"
#include "2d/sceneobject/SceneObject.h"
#include "2d/sceneobject/Path.h"
#include "debug/profiler.h"

IMPLEMENT_CO_NETOBJECT_V1(AIMovementComponent);

AIMovementComponent::AIMovementComponent()
{
   mActive = true;

   mMaxSpeed = 100.0f;
   mMaxForce = 10.0f;
   mForceMultiplier = 1.0f;
   mArriveDecelerationRate = 0.0f;

   mRotateIntoMotion = false;

   mTargetName = "";
   mTargetObject = NULL;
   mTarget.set(0.0f, 0.0f);

   for (S32 i=0; i<MAX_AGENTS; ++i)
   {
      mAgentName[i] = "";
   }

   mPathName = "";
   mPathType = PATH_UNKNOWN;
   mPathLoop = false;
   mPathOffset.set(0.0f, 0.0f);

   mWayPointDistance = 32.0f;

   mLastTime = 0.0f;
   mLastMotionBasedRotation = 0.0f;
}

AIMovementComponent::~AIMovementComponent()
{
}

bool AIMovementComponent::onAdd()
{
    if (!Parent::onAdd())
        return false;

    return true;
}

void AIMovementComponent::onRemove()
{
    Parent::onRemove();
}

void AIMovementComponent::initPersistFields()
{
    Parent::initPersistFields();

    addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, AIMovementComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );

    addField("targetObject", TypeString, Offset( mTargetName, AIMovementComponent ), 
      "Name of object to target with AI components.  If empty then the 'target' field will be used instead." );

    addField("target", TypePoint2F, Offset( mTarget, AIMovementComponent ), 
      "Target position for AI components.  If 'targetObject' is set, that object's position will be used instead." );

    addField("agentName", TypeString, Offset( mAgentName, AIMovementComponent ), MAX_AGENTS, 0,
      "Name of object referenced by AI components.  If empty then the 'agent' field will be used instead." );

    addField("agent", TypeSimObjectPtr, Offset( mAgent, AIMovementComponent ), MAX_AGENTS, 0,
      "SimObjectID of object referenced by AI components.  Zero if not set." );

    addField("pathName", TypeString, Offset( mPathName, AIMovementComponent ), 
      "Name of path to use with AI components.  If empty then the 'path' field will be used instead." );

    addProtectedField("Path", TypeSimObjectPtr, Offset(mPathObject, AIMovementComponent), &setPathField, &defaultProtectedGetFn, "" );

    addField("pathLoop", TypeBool, Offset( mPathLoop, AIMovementComponent ), 
      "When the path end node is reached does the owner return to the start path node." );

    addField("pathOffset", TypePoint2F, Offset( mPathOffset, AIMovementComponent ), 
      "Offset added to path way points in world coordinates." );

    addField("wayPointDistance", TypeF32, Offset( mWayPointDistance, AIMovementComponent ), 
      "Radius around path way points that defines their touch zone." );

    addField("maxSpeed", TypeF32, Offset( mMaxSpeed, AIMovementComponent ), 
      "Maximum speed of owner object." );

    addField("maxForce", TypeF32, Offset( mMaxForce, AIMovementComponent ), 
      "Maximum forces applied due to steering." );

    addField("forceMultiplier", TypeF32, Offset( mForceMultiplier, AIMovementComponent ), 
      "Modifies the AI steering applied force on the object." );

    addField("arriveDecelerationRate", TypeF32, Offset( mArriveDecelerationRate, AIMovementComponent ), 
      "Deceleration rate used for arrive steering." );

    addField("rotateIntoMotion", TypeBool, Offset( mRotateIntoMotion, AIMovementComponent ), 
      "Should the owner automatically rotate to face the direction of motion?." );
}

//-----------------------------------------------------------------------------

bool AIMovementComponent::addComponent( SimComponent *component )
{
   if (!dynamic_cast<IAIMovement*>(component))
   {
      Con::errorf("AIMovementComponent::addComponent(): Cannot add a non-AI movement component");
      return false;
   }

   return Parent::addComponent(component);
}

bool AIMovementComponent::onComponentAdd(SimComponent *target)
{
    if (!Parent::onComponentAdd(target))
        return false;
  
    //
    SceneObject *owner = dynamic_cast<SceneObject*>(target);
    if (!owner)
    {
        Con::warnf("AIMovementComponent::onComponentAdd - Must be added to a SceneObject.");
        return false;
    }

    // Store our owner and current time
    mOwner = owner;
   mLastTime = mOwner->getSceneTime();

    return true;
}

void AIMovementComponent::onComponentRemove(SimComponent *target)
{
    Parent::onComponentRemove(target);
}

//-----------------------------------------------------------------------------

void AIMovementComponent::onAddToScene()
{
}

void AIMovementComponent::onUpdate()
{
   if (isAIActive())
   {
      updateMovement();
   }
}

//-----------------------------------------------------------------------------

SimObject* AIMovementComponent::getCallbackDestination()
{
   if (mCallbackDestination.isNull())
   {
      return this;
   }
   else
   {
      return mCallbackDestination;
   }
}

//-----------------------------------------------------------------------------

void AIMovementComponent::updateMovement()
{
   // Check that there is at least one AI movement component
   if (!hasComponents())
      return;

   Con::executef(getCallbackDestination(), 2, "onPreUpdateMovement", Con::getIntArg(this->getId()));

   // Set up parameters common to all AI components
   mParameters.ownerPosition = mOwner->getPosition();
   mParameters.ownerAngle = mOwner->getAngle();
   mParameters.ownerVelocity = mOwner->getLinearVelocity();
   mParameters.ownerMaxForce = mMaxForce;
   mParameters.ownerArriveDecelerationRate = mArriveDecelerationRate;
   mParameters.ownerMaxSpeed = mMaxSpeed;

   mParameters.ownerMotionAngle = mLastMotionBasedRotation;

   F32 time = mOwner->getSceneTime();
   mParameters.elapsedTime = time - mLastTime;
   mLastTime = time;

   mParameters.target = mTarget;
   if (mTargetName && mTargetName[0] && !mTargetObject)
   {
      mTargetObject = (SceneObject *)Sim::findObject(mTargetName);
   }
   if (mTargetObject)
   {
      mParameters.target = mTargetObject->getPosition();
   }

   for (S32 i=0; i<MAX_AGENTS; ++i)
   {
      if (mAgentName[i] && mAgentName[i][0] && !mAgent[i])
      {
         mAgent[i] = (SceneObject *)Sim::findObject(mAgentName[i]);
      }
      if (mAgent[i])
      {
         mParameters.agent[i] = mAgent[i];
      }
      else
      {
         mParameters.agent[i] = NULL;
      }
   }

   if (mPathName && mPathName[0] && !mPathObject)
   {
      mPathObject = (SceneObject *)Sim::findObject(mPathName);
      mPathType = getPathType(mPathObject);
   }
   mParameters.path = mPathObject;
   mParameters.pathType = mPathType;
   mParameters.pathLoop = mPathLoop;
   mParameters.pathOffset = mPathOffset;
   mParameters.pathWayPointDistance = mWayPointDistance;

   Vector2 forces = calculateWeightedSum() * mForceMultiplier;
   if (forces.LengthSquared() > (mMaxForce * mMaxForce))
   {
      forces.Normalize();
      forces *= mMaxForce;
   }

   mOwner->applyLinearImpulse(forces);

   Vector2 vel = mOwner->getLinearVelocity();
   if (vel.LengthSquared() > 0.0001)
   {
      vel.Normalize();
      F32 angle = vel.getAngle();

      // Update the last known rotation angle based on motion
      mLastMotionBasedRotation = angle;

      // Should the owner be rotated as well?
      if (mRotateIntoMotion)
      {
         mOwner->setAngle(angle);
      }
   }

   char* forcesBuffer = Con::getArgBuffer(128);
   dSprintf(forcesBuffer, 128, "%g %g", forces.x, forces.y);
   Con::executef(getCallbackDestination(), 3, "onPostUpdateMovement", Con::getIntArg(getId()), forcesBuffer);
}

Vector2 AIMovementComponent::calculateWeightedSum()
{
   // Run through each AI component
   Vector2 totalForces(0.0f, 0.0f);
   S32 count =getComponentCount();
   for (S32 i = 0; i < count; ++i)
   {
      SimComponent* component = getComponent(i);
      if (!component->isEnabled())
         continue;

      IAIMovement* movement = dynamic_cast<IAIMovement*>(getComponent(i));
      if (!movement)
         continue;

      F32 weight = movement->getWeight();
      Vector2 force = movement->calculateForce(mParameters);
      force *= weight;
      totalForces += force;
   }

   // Max sure we're within limits
   if (totalForces.LengthSquared() > (mMaxForce * mMaxForce))
   {
      totalForces.Normalize();
      totalForces *= mMaxForce;
   }

   return totalForces;
}

Vector2 AIMovementComponent::calculateSeek(Point2F& target, Point2F& origin, Vector2& velocity, F32 maxSpeed)
{
   Vector2 direction = target - origin;
   direction.Normalize();

   Vector2 desiredVelocity = direction * maxSpeed;

   return desiredVelocity - velocity;
}

Vector2 AIMovementComponent::calculateArrive(Point2F& target, Point2F& origin, Vector2& velocity, F32 maxSpeed, F32 decelerationRate)
{
   // If deceleration rate is zero then just do a seek
   if (mIsZero(decelerationRate))
   {
      return calculateSeek(target, origin, velocity, maxSpeed);
   }

   Vector2 direction = target - origin;
   F32 distance = direction.Length();

   if (distance > 0.0f)
   {
      F32 speed = distance / decelerationRate;

      // Make sure we're within the speed limit
      speed = getMin(speed, maxSpeed);

      Vector2 desiredVelocity = direction * speed / distance;

      return desiredVelocity - velocity;
   }

   return Vector2::getZero();
}

//-----------------------------------------------------------------------------

void AIMovementComponent::setTargetObject(SceneObject* object)
{
   mTargetObject = object;

   mTargetName = "";
}

bool AIMovementComponent::setPathField(void* obj, const char* data)
{
   AIMovementComponent *object = static_cast<AIMovementComponent *>(obj);
   
   SceneObject* path = dynamic_cast<SceneObject*>(Sim::findObject(data));
   object->setPathObject(path);

   // We return false since we don't want the console to mess with the data
   return false;
}

void AIMovementComponent::setPathObject(SceneObject* object)
{
   mPathObject = object;

   // Determine the path's type
   mPathType = getPathType(object);

   mPathName = "";
}

S32 AIMovementComponent::getPathType(SceneObject* path)
{
   if (dynamic_cast<Path*>(path))
   {
      return PATH_T2DPATH;
   }

   return PATH_UNKNOWN;
}

//-----------------------------------------------------------------------------

ConsoleMethod(AIMovementComponent, setAIActive, void, 3, 3, 
   "(bool state) - Sets if this component is active and allowed to perform its calculations.\n"
   "@param state True to make the component active, false to deactivate it.\n")
{
   object->setAIActive(dAtob(argv[2]));
}

ConsoleMethod(AIMovementComponent, isAIActive, bool, 3, 3, 
   "Is this component active and allowed to perform its calculations?\n"
   "@return True if this component is active and allowed to perform its calculations.\n")
{
   return object->isAIActive();
}

ConsoleMethod(AIMovementComponent, getCallbackDestination, S32, 2, 2, 
   "Gives the ID of the SimObject used for AI callbacks.\n"
   "@return The SimObjectID of the callback object, or 0 if there is none.")
{
   SimObject* target = object->getCallbackDestination();
   if (!target)
      return 0;
   else
      return target->getId();
}

ConsoleMethod(AIMovementComponent, setCallbackDestination, void, 3, 3, 
   "(SimObejctID id) - Sets the object used for AI callbacks.\n"
   "@param id The SimObjectID of the object to used for callbacks, or 0 to clear the target.\n"
   "@note If there is no callback destination object set, then this AIMovementComponet is used.")
{
   SimObject* target = dynamic_cast<SimObject *>(Sim::findObject( argv[2] ));
   if (target)
   {
      object->setCallbackDestination(target);
   }
   else
   {
      object->setCallbackDestination(NULL);
   }
}

ConsoleMethod(AIMovementComponent, getTargetObject, S32, 2, 2, 
   "Gives the ID of the targeted object.\n"
   "@return The SimObjectID of the targetd object, or 0 if there is none.")
{
   SceneObject* target = object->getTargetObject();
   if (!target)
      return 0;
   else
      return target->getId();
}

ConsoleMethod(AIMovementComponent, setTargetObject, bool, 3, 3, 
   "(SimObejctID id) - Sets the object to target with the AI components.\n"
   "@param id The SimObjectID of the object to target, or 0 to clear the target.\n"
   "@return True if the object has been made the target.  False if there was a problem.")
{
   SceneObject* target = dynamic_cast<SceneObject *>(Sim::findObject( argv[2] ));
   if (target)
   {
      object->setTargetObject(target);
      return true;
   }

   object->setTargetObject(NULL);

   return false;
}

ConsoleMethod(AIMovementComponent, getLastMotionBasedRotation, F32, 2, 2, 
   "Returns the last rotation angle based on motion." )
{
   return object->getLastMotionBasedRotation();
}
