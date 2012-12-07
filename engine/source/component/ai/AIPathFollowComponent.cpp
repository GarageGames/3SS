//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "AIPathFollowComponent.h"
#include "2d/sceneobject/Path.h"

IMPLEMENT_CO_NETOBJECT_V1(AIPathFollowComponent);

AIPathFollowComponent::AIPathFollowComponent()
{
   mOwner = NULL;
   mPathType = AIMovementComponent::PATH_UNKNOWN;
   mDestinationNode = 0;

   mArriveDecelerationRate = 0.0f;
   mWayPointDistance = 32.0f;
   mPathLoop = false;
   mPathOffset.set(0.0f, 0.0f);
}

AIPathFollowComponent::~AIPathFollowComponent()
{
}

bool AIPathFollowComponent::onAdd()
{
    if (!Parent::onAdd())
        return false;
    
    return true;
}

void AIPathFollowComponent::onRemove()
{
    Parent::onRemove();
}

void AIPathFollowComponent::initPersistFields()
{
    Parent::initPersistFields();

    addProtectedField("Owner", TypeSimObjectPtr, Offset(mOwner, AIPathFollowComponent), &defaultProtectedSetFn, &defaultProtectedGetFn, "" );

    addField("weight", TypeF32, Offset(mWeight, AIPathFollowComponent), 
      "Relative weight amongst all AI movement components." );
}

//-----------------------------------------------------------------------------

bool AIPathFollowComponent::onComponentAdd(SimComponent *target)
{
    if (!Parent::onComponentAdd(target))
        return false;
  
    //
    AIMovementComponent *owner = dynamic_cast<AIMovementComponent*>(target);
    if (!owner)
    {
        Con::warnf("AIPathFollowComponent::onComponentAdd - Must be added to an AIMovementComponent.");
        return false;
    }

    // Store our owner
    mOwner = owner;

    return true;
}

void AIPathFollowComponent::onComponentRemove(SimComponent *target)
{
    Parent::onComponentRemove(target);
}

//-----------------------------------------------------------------------------

Vector2 AIPathFollowComponent::calculateForce(AIMovementComponent::MovementParameters& params)
{
   SceneObject* path;
   S32 pathType;
   F32 decelerationRate;
   F32 wayPointDistance;
   Point2F pathOffset;
   bool pathLoop;
   if (mPathObject)
   {
      path = mPathObject;
      pathType = mPathType;
      decelerationRate = mArriveDecelerationRate;
      wayPointDistance = mWayPointDistance;
      pathLoop = mPathLoop;
      pathOffset = mPathOffset;
   }
   else if(params.path)
   {
      path = params.path;
      pathType = params.pathType;
      decelerationRate = params.ownerArriveDecelerationRate;
      wayPointDistance = params.pathWayPointDistance;
      pathLoop = params.pathLoop;
      pathOffset = params.pathOffset;
   }
   else
   {
      return Vector2::getZero();
   }

   // Check if we can deal with the given path
   if (!path || pathType == AIMovementComponent::PATH_UNKNOWN)
      return Vector2::getZero();

   F32 wpDistanceSq = wayPointDistance * wayPointDistance;

   Point2F destinationPosition;
   bool check = getCurrentWayPoint(path, pathType, destinationPosition);
   if (!check)
      return Vector2::getZero();

   // Apply an offset to the destination point
   destinationPosition += pathOffset;

   // Are we within range of the destination way point
   Vector2 distance = destinationPosition - params.ownerPosition;
   if (distance.LengthSquared() <= wpDistanceSq)
   {
      bool lastNode = headingToLastNode(path, pathType, pathLoop);

      // Hit node callback
      Con::executef(mOwner->getCallbackDestination(), 4, "onPathNodeReached", Con::getIntArg(this->getId()), Con::getIntArg(mDestinationNode), Con::getIntArg(lastNode));

      setNextWayPoint(path, pathType, pathLoop);
      bool check = getCurrentWayPoint(path, pathType, destinationPosition);

      if (lastNode || !check)
         return Vector2::getZero();

      // Apply an offset to the destination point
      destinationPosition += pathOffset;
   }

   if (!headingToLastNode(path, pathType, pathLoop))
   {
      return AIMovementComponent::calculateSeek(destinationPosition, params.ownerPosition, params.ownerVelocity, params.ownerMaxSpeed);
   }
   else
   {
      return AIMovementComponent::calculateArrive(destinationPosition, params.ownerPosition, params.ownerVelocity, params.ownerMaxSpeed, decelerationRate);
   }
}

bool AIPathFollowComponent::getCurrentWayPoint(SceneObject* path, S32 pathType, Point2F& destinationPosition)
{
   if (pathType == AIMovementComponent::PATH_T2DPATH)
   {
      Path* p = static_cast<Path*>(path);

      // Check if the requested node index is even on the path
      if (!p->isValidNode(mDestinationNode))
      {
         return false;
      }

      // Get the node's position
      Path::PathNode node = p->getNode(mDestinationNode);
      destinationPosition = node.position;

      return true;
   }

   return false;
}

void AIPathFollowComponent::setNextWayPoint(SceneObject* path, S32 pathType, bool loop)
{
   if (pathType == AIMovementComponent::PATH_T2DPATH)
   {
      Path* p = static_cast<Path*>(path);

      S32 nodeCount = p->getNodeCount();
      if (mDestinationNode >= (nodeCount-1) && loop)
      {
         // Loop back to the start
         mDestinationNode = 0;
      }
      else
      {
         // On to the next node
         mDestinationNode++;
      }
   }
}

bool AIPathFollowComponent::headingToLastNode(SceneObject* path, S32 pathType, bool loop)
{
   // If we're looping then we'll never reach the last node
   if (loop)
      return false;

   if (pathType == AIMovementComponent::PATH_T2DPATH)
   {
      Path* p = static_cast<Path*>(path);

      S32 nodeCount = p->getNodeCount();
      if (mDestinationNode >= (nodeCount-1))
         return true;
   }

   return false;
}

//-----------------------------------------------------------------------------

void AIPathFollowComponent::setPathObject(SceneObject* object)
{
   mPathObject = object;
   mPathType = mOwner->getPathType(mPathObject);
}

void AIPathFollowComponent::setPathDestination(S32 index)
{
   mDestinationNode = index;
}

F32 AIPathFollowComponent::getPathDistanceToEnd(SceneObject* obj)
{
    Path* path = dynamic_cast<Path*>(getPathObject());

    if (!path)
    {
        Con::warnf("AIPathFollowComponent::getPathDistanceToEnd - No path found for object.");
        return false;
    }

    // Assume the end node is the last node in the path
    S32 endNode = path->getNodeCount() - 1;

    // Create a temporay variable to store the currenct position
    Point2F currentPosition = obj->getPosition();

    // Calculate the total distance along the path from pos to the end node
    F32 distance = 0;
    for (S32 i = mDestinationNode + 1; i <= endNode; i++)
    {
        Path::PathNode node = path->getNode(i);
        distance += static_cast<Vector2>(currentPosition - node.position).Length();
    }

    return distance;
}

//-----------------------------------------------------------------------------

ConsoleMethod(AIPathFollowComponent, getPathObject, S32, 2, 2, 
   "Gives the ID of the local path object.\n"
   "This is the path object set on this component using setPathObject() and not from "
   "the parent AIMovementComponent.\n"
   "@return The SimObjectID of the targetd object, or 0 if there is none.")
{
   SceneObject* target = object->getPathObject();
   if (!target)
      return 0;
   else
      return target->getId();
}

ConsoleMethod(AIPathFollowComponent, setPathObject, bool, 3, 3, 
   "(SimObejctID id) - Sets the local path for the AI component.\n"
   "This overrides any path set by the parent AIMovementComponent.\n"
   "@param id The SimObjectID of the path, or 0 to clear the target.\n"
   "@return True if the path has been assigned.  False if there was a problem.")
{
   SceneObject* path = dynamic_cast<SceneObject *>(Sim::findObject( argv[2] ));
   if (path)
   {
      object->setPathObject(path);
      return true;
   }

   object->setPathObject(NULL);

   return false;
}

ConsoleMethod(AIPathFollowComponent, getPathDestination, S32, 2, 2, 
   "Get the current path destination node.\n")
{
   return object->getPathDestination();
}

ConsoleMethod(AIPathFollowComponent, setPathDestination, void, 3, 3, 
   "(S32 index) - Sets the current path destination node.\n"
   "@param index The node index.")
{
   object->setPathDestination(dAtoi(argv[2]));
}

ConsoleMethod(AIPathFollowComponent, setPathLoop, void, 3, 3, 
   "(bool loop) - Sets the local path to loop.\n"
   "@param loop Set to true to loop.")
{
   object->setPathLoop(dAtob(argv[2]));
}

ConsoleMethod(AIPathFollowComponent, setPathDecelerationRate, void, 3, 3, 
   "(F32 rate) - Sets the local path's deceleration rate when approaching the last node.\n"
   "Set to zero to not slow down for the last path node.\n"
   "@param rate The deceleration rate.")
{
   object->setPathDecelerationRate(dAtof(argv[2]));
}

ConsoleMethod(AIPathFollowComponent, setPathOffset, void, 3, 3, 
   "(Point2F offset) - Offset added to path way points in world coordinates.\n"
   "@param offset The applied offset in the form of 'x y'.")
{
   F32 x=0.0f, y=0.0f;
   dSscanf(argv[2], "%g %g", &x, &y);

   object->setPathOffset(Point2F(x,y));
}

ConsoleMethod(AIPathFollowComponent, setPathWayPointDistance, void, 3, 3, 
   "(F32 distance) - Sets the local path's radius around path way points that defines their touch zone.\n"
   "@param distance Distance from path node.")
{
   object->setPathWayPointDistance(dAtof(argv[2]));
}

ConsoleMethod(AIPathFollowComponent, getPathDistanceToEnd, F32, 3, 3,
    "(SceneObject* object) Gets the total distance along the path from the specified object on the path.\n"
    "@param object Object for which to calculate distance to end of path.\n"
    "@return The total distance to the end of the path.")
{
    SceneObject* objectParam = dynamic_cast<SceneObject*>(Sim::findObject( argv[2] ));

    if (objectParam == NULL)
        return 0.0f;

    return object->getPathDistanceToEnd(objectParam);
}