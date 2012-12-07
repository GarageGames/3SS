//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/dgl.h"
#include "console/consoleTypes.h"
#include "Path.h"
#include "math/mMathFn.h"

// Script bindings.
#include "Path_ScriptBinding.h"

//---------------------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(Path);

//---------------------------------------------------------------------------------------------

static EnumTable::Enums pathModeLookup[] =
                {
                { PATH_WRAP, "WRAP" },
                { PATH_REVERSE, "REVERSE" },
                { PATH_RESTART, "RESTART" }
                };

//---------------------------------------------------------------------------------------------

static EnumTable pathModeTable(sizeof(pathModeLookup) /  sizeof(EnumTable::Enums),
                               &pathModeLookup[0]);

//---------------------------------------------------------------------------------------------

ePathMode getPathMode(const char* label)
{
   for(U32 i = 0; i < (sizeof(pathModeLookup) / sizeof(EnumTable::Enums)); i++)
   {
      if(dStricmp(pathModeLookup[i].label, label) == 0)
         return((ePathMode)pathModeLookup[i].index);
   }

   AssertFatal(false, "getPathMode() - Invalid Path Mode!");
   return PATH_WRAP;
}

//---------------------------------------------------------------------------------------------

const char* getPathModeDescription(const ePathMode pathMode)
{
   for(U32 i = 0; i < (sizeof(pathModeLookup) / sizeof(EnumTable::Enums)); i++)
   {
      if(pathModeLookup[i].index == pathMode)
         return pathModeLookup[i].label;
   }

   AssertFatal(false, "getPathModeDescription() - Invalid Path Mode!");
   return StringTable->EmptyString;
}

//---------------------------------------------------------------------------------------------

static EnumTable::Enums followMethodLookup[] =
                {
                { FOLLOW_LINEAR, "LINEAR" },
                { FOLLOW_BEZIER, "BEZIER" },
                { FOLLOW_CATMULL, "CATMULL" },
                { FOLLOW_CUSTOM, "CUSTOM" }
                };

//---------------------------------------------------------------------------------------------

static EnumTable followMethodTable(sizeof(followMethodLookup) /  sizeof(EnumTable::Enums),
                                      &followMethodLookup[0]);

//---------------------------------------------------------------------------------------------

eFollowMethod getFollowMethod(const char* label)
{
    for(U32 i = 0; i < (sizeof(followMethodLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( dStricmp(followMethodLookup[i].label, label) == 0)
           return((eFollowMethod)followMethodLookup[i].index);
    }

    AssertFatal(false, "getFollowMethod() - Invalid FollowMethod!");
    return FOLLOW_LINEAR;
}

//---------------------------------------------------------------------------------------------

const char* getFollowMethodDescription(const eFollowMethod follow)
{
    for(U32 i = 0; i < (sizeof(followMethodLookup) / sizeof(EnumTable::Enums)); i++)
    {
        if( followMethodLookup[i].index == follow )
            return followMethodLookup[i].label;
    }

    AssertFatal(false, "getFollowMethodDescription() - Invalid Follow Method!");
    return StringTable->EmptyString;
}

//---------------------------------------------------------------------------------------------

Path::Path()
{
   mPathType = FOLLOW_LINEAR;
   mNodeRenderSize = 0.5f;
   mNodesLoaded = false;
   mMountOffset = 0;
   mFinished = false;

   VECTOR_SET_ASSOCIATION(mObjects);
   VECTOR_SET_ASSOCIATION(mNodes);

   // Use a static body by default.
   mBodyDefinition.type = b2_staticBody;
}

//---------------------------------------------------------------------------------------------

Path::~Path()
{
   clear();
}

//---------------------------------------------------------------------------------------------

void Path::initPersistFields()
{
   addProtectedField("pathType", TypeEnum, Offset(mPathType, Path), &setPathType, &defaultProtectedGetFn, &writePathType, 1, &followMethodTable);

   Parent::initPersistFields();
}

//---------------------------------------------------------------------------------------------

void Path::onRemove()
{
   // [neo, 5/22/2007 - #3139]
   // Moved code to clear()
   clear();

   Parent::onRemove();
}


//---------------------------------------------------------------------------------------------

bool Path::setPathType(void* obj, const char* data)
{
   static_cast<Path*>(obj)->setPathType(getFollowMethod(data));
   return false;
} 

//---------------------------------------------------------------------------------------------

void Path::setPosition(const Vector2& position)
{
   Vector2 previousPosition = getPosition();
   Parent::setPosition(position);

   Vector2 difference = getPosition() - previousPosition;

   for (S32 i = 0; i < getNodeCount(); i++)
   {
      PathNode& node = getNode(i);
      node.position += difference;
   }
}

//---------------------------------------------------------------------------------------------

Vector2 validateSize( const Vector2 &size )
{
   return Vector2( getMax( size.x, 4.0f ), getMax( size.y, 4.0f ) );
}

//---------------------------------------------------------------------------------------------

void Path::setSize(const Vector2& size)
{
   Vector2 previousSize = getSize();

   // [neo, 7/6/2007 - #3206]
   // Make sure it never gets too small to select
   Parent::setSize( validateSize( size ) );

   Vector2 position = getPosition();
   Vector2 difference = getSize().div(previousSize);

   for (S32 i = 0; i < getNodeCount(); i++)
   {
      PathNode& node = getNode(i);
      Vector2 distance = node.position - position;
      distance.mult(difference);
      node.position = position + distance;
   }

   for (S32 i = 0; i < getNodeCount(); i++)
   {
      calculateBezierLength(i);
      calculateCatmullLength(i);
   }
}

//---------------------------------------------------------------------------------------------

void Path::checkObjectNodes()
{
   S32 lastNode = mNodes.size() - 1;

   // If there are no nodes, we can't support any objects.
   if (lastNode < 0)
   {
      Con::warnf("Path has no nodes. Detaching all objects.");
      //[neo, 5/22/2007 - #3139]
      //mObjects.clear();
      clear();
      
      return;
   }
   //[neo, 5/22/2007 - #3139]
   // mObjects is now a vector of pointers!
   Vector<PathedObject*>::iterator i;
   for (i = mObjects.begin(); i != mObjects.end(); i++)
   {
      if (!isValidNode((*i)->getStartNode()))
         (*i)->setStartNode(0);

      if (!isValidNode((*i)->getEndNode()))
         (*i)->setEndNode(lastNode);

      if (!isValidNode((*i)->getCurrentNode()))
         (*i)->setCurrentNode(lastNode);

      if (!isValidNode((*i)->getDestinationNode()))
         (*i)->setDestinationNode(lastNode);
   }
}

//---------------------------------------------------------------------------------------------

void Path::calculateBezierLength(S32 node)
{
   S32 i = node;
   S32 j = node + 1;
   if (j >= mNodes.size())
      j = 0;

   S32 dir = i % 2 ? -1 : 1;
   Vector2 a = mNodes[i].position;
   F32 rot = mNodes[i].rotation + (b2_pi*0.5f * dir);
   Vector2 b = a + (Vector2(mCos(rot), mSin(rot)) * mNodes[i].weight);
   Vector2 d = mNodes[j].position;
   if ((j == 0) && (mNodes.size() % 2)) dir = -dir;
   rot = mNodes[j].rotation + (b2_pi*0.5f * dir);
   Vector2 c = d + (Vector2(mCos(rot), mSin(rot)) * mNodes[j].weight);
   
   F32 length = 0;
   Vector2 pos1;
   Vector2 pos2 = a;
   for (F32 i = 0.0f; i < 1.001f; i += 0.001f)
   {
      F32 ii = 1.0f - i;
      pos1 = pos2;
      pos2 = (a * ii * ii * ii) + (3 * b * ii * ii * i) + (3 * c * ii * i * i) + (d * i * i * i);
      length += (pos2 - pos1).Length();
   }
   mNodes[node].bezierLength = length;
}

//---------------------------------------------------------------------------------------------

void Path::calculateCatmullLength(S32 node)
{
   F32 length = 0;
   Vector2 pos1;
   Vector2 pos2 = mNodes[node].position;

   S32 nodeCount = mNodes.size();

   S32 p0 = node - 1;
   if (p0 < 0) p0 = nodeCount - 1;
   S32 p1 = p0 + 1;
   S32 p2 = p0 + 2;
   S32 p3 = p0 + 3;

   if (p1 >= nodeCount) { p1 = 0; p2 = 1; p3 = 2; }
   if (p2 >= nodeCount) { p2 = 0; p3 = 1; }
   if (p3 >= nodeCount) { p3 = 0; }

   for (F32 t = 0.0f; t < 1.001f; t += 0.001f)
   {
      pos1 = pos2;
      pos2 = 0.5 * ((2 * mNodes[p1].position) + (-mNodes[p0].position + mNodes[p2].position) * t +
         (2 * mNodes[p0].position - 5 * mNodes[p1].position + 4 * mNodes[p2].position - mNodes[p3].position) * t * t +
         (-mNodes[p0].position + 3 * mNodes[p1].position - 3 * mNodes[p2].position + mNodes[p3].position) * t * t * t);

      length += (pos2 - pos1).Length();
   }

   mNodes[node].catmullLength = length;
}

//---------------------------------------------------------------------------------------------

void Path::attachObject(SceneObject* object, F32 speed, S32 direction, bool orientToPath,
                           S32 startNode, S32 endNode, ePathMode pathMode, S32 loops, bool sendToStart)
{
   if (sendToStart)
   {
      if ((startNode >= 0) && (startNode < mNodes.size()))
         object->setPosition(mNodes[startNode].position);
   }

   if (startNode == endNode)
   {
      // If the start is the same as the end, the reverse mode will switch the
      // direction right at the start, so we reverse it first here thus counteracting.
      if (pathMode == PATH_REVERSE)
         direction = -direction;
   }

   // Don't attach to two paths.
   if (object->getAttachedToPath())
      return;

   if (object == this)
   {
      Con::warnf("Path::attachObject - Can't attach to self!");
      return;
   }

   object->setAttachedToPath(this);
   object->setLinearVelocity( Vector2::getZero() );
   deleteNotify(object);

   // [neo, 5/22/2007 - #3139]
   // mObjects is now a vector of pointers!

   /*mObjects.increment();
   PathedObject& pathedObject = mObjects.last();
   constructInPlace(&mObjects.last());
   pathedObject.mPath = this;
   pathedObject.mObject = object;
   pathedObject.mSourceNode = startNode;
   pathedObject.mDestinationNode = startNode;
   pathedObject.mStartNode = startNode;
   pathedObject.mEndNode = endNode;
   pathedObject.mSpeed = speed;
   pathedObject.mStartDirection = direction;
   pathedObject.mDirection = direction;
   pathedObject.mOrientToPath = orientToPath;
   pathedObject.mLoopCounter = 0;
   pathedObject.mTotalLoops = loops;
   pathedObject.mPathMode = pathMode;
   pathedObject.mTime = 0.0f;
   pathedObject.mRotationOffset = 0.0f;*/

   PathedObject *pathedObject     = new PathedObject();
   pathedObject->mPath            = this;
   pathedObject->mObject          = object;
   pathedObject->mObjectId        = object->getId(); // used for lookup as mObject gets cleared before we get a notification!
   pathedObject->mSourceNode      = startNode;
   pathedObject->mDestinationNode = startNode;
   pathedObject->mStartNode       = startNode;
   pathedObject->mEndNode         = endNode;
   pathedObject->mSpeed           = speed;
   pathedObject->mStartDirection  = direction;
   pathedObject->mDirection       = direction;
   pathedObject->mOrientToPath    = orientToPath;
   pathedObject->mLoopCounter     = 0;
   pathedObject->mTotalLoops      = loops;
   pathedObject->mPathMode        = pathMode;
   pathedObject->mTime            = 0.0f;
   pathedObject->mAngleOffset  = 0.0f;

   mObjects.push_back( pathedObject );
}

//---------------------------------------------------------------------------------------------

void Path::detachObject(SceneObject* object)
{
   // [neo, 05/22/2007 - #3139]
   // Refactored checks and made sure all references were cleared etc
   // Note: mObjects is now a vector of pointers so need to dealloc elements!
   if( !object )
      return;

   Vector<PathedObject*>::iterator i;

   for( i = mObjects.begin(); i != mObjects.end(); i++ )
   {      
      PathedObject *pobj = (*i);

      if( object == pobj->mObject )
      {
         if( !pobj->mObject.isNull() )
         {
            // Stop object and detach
            pobj->mObject->setLinearVelocity(Vector2(0, 0));
            pobj->mObject->setAttachedToPath(NULL);
                        
            // Clean up any references so object does not try notify when deleting
            clearNotify( pobj->mObject);
            
            // Note: we don't have to explicitly clear mObject as
            // that will be done automatically when deleting below.
         }      

         // We allocate these dynamically so ta ta...
         delete pobj;

         mObjects.erase_fast(i);

         break;
      }
   }
}

//---------------------------------------------------------------------------------------------

void Path::onDeleteNotify( SimObject* object )
{
   // [neo, 5/22/2007 - #3139]
   // mObjects is now a vector of pointers!
   // We also look up by object id as the SimObjectPtr ref could have been cleared already
   // by processNotify()!
   Vector<PathedObject*>::iterator i;

   SimObjectId objId = object->getId();

   for( i = mObjects.begin(); i != mObjects.end(); i++ )
   {
      if( (*i)->mObjectId == objId )
      {
         // [neo, 5/22/2007 - #3139]
         // mObjects is now a vector of pointers so delete element
         delete (*i);

         mObjects.erase_fast( i );

         break;
      }
   }
}

//---------------------------------------------------------------------------------------------

S32 Path::addNode(Vector2 position, F32 rotation, F32 weight, S32 location)
{
    rotation = mDegToRad( rotation );

   // Bind the location;
   S32 nodeCount = mNodes.size();
   if ((location < 0) || (location >= nodeCount))
      location = nodeCount;

   // Add the node to the list.
   if (location >= nodeCount)
   {
      // Shortcut for adding to the end.
      mNodes.push_back(PathNode(position, rotation, weight));
   }
   else
   {
      Vector<PathNode>::iterator iter = mNodes.begin();
      for (S32 i = 0; i < location; i++) iter++;
      mNodes.insert(iter, PathNode(position, rotation, weight));
   }

   nodeCount = mNodes.size();
   S32 left = location - 1;
   if (left < 0)
      left = nodeCount - 1;
   S32 right = location + 1;
   if (right >= nodeCount)
      right = 0;
   S32 right2 = right + 1;
   if (right2 >= nodeCount)
      right2 = 0;

   calculateBezierLength(left);
   calculateBezierLength(location);
   calculateBezierLength(right);

   calculateCatmullLength(left);
   calculateCatmullLength(location);
   calculateCatmullLength(right);
   calculateCatmullLength(right2);

   updateSize();

   return nodeCount;
}

//---------------------------------------------------------------------------------------------

void Path::updateSize()
{
   Vector2 min, max;
   for (S32 i = 0; i < getNodeCount(); i++)
   {
      PathNode& node = getNode(i);
      if (i == 0)
      {
         min = node.position;
         max = node.position;
      }
      else
      {
         if (node.position.x < min.x)
            min.x = node.position.x;
         if (node.position.y < min.y)
            min.y = node.position.y;
         if (node.position.x > max.x)
            max.x = node.position.x;
         if (node.position.y > max.y)
            max.y = node.position.y;
      }
   }

   if (getNodeCount() == 1)
      Parent::setSize(Vector2(10.0f, 10.0f));
   else
      // [neo, 7/6/2007 - #3206]
      // Make sure it never gets too small to select
      Parent::setSize( validateSize( max - min ) );

   Parent::setPosition(min + ((max - min) * 0.5));
}

//---------------------------------------------------------------------------------------------

S32 Path::removeNode(U32 index)
{
   if (isValidNode(index))
      mNodes.erase(index);

   checkObjectNodes();

   if (mNodes.empty())
      return 0;

   S32 nodeCount = mNodes.size();
   S32 left = index - 1;
   if (left < 0)
      left = nodeCount - 1;
   S32 right = index + 1;
   if (right >= nodeCount)
      right = 0;
   S32 right2 = right + 1;
   if (right2 >= nodeCount)
      right2 = 0;

   calculateBezierLength(left);
   calculateBezierLength(index);
   calculateBezierLength(right);

   calculateCatmullLength(left);
   calculateCatmullLength(index);
   calculateCatmullLength(right);
   calculateCatmullLength(right2);

   updateSize();

   return nodeCount;
}

//---------------------------------------------------------------------------------------------

void Path::clear()
{
   // [neo, 5/22/2007 - #3139]
   // This will loop forever if and object was detached and cleared dynamically from
   // script as it will not find the node and so not reduce the size. We need to clear
   // out any back-references the PathedObject::mObject might have (SimObjectPtr) so we
   // don't get trailing references to non existent objects. Also mObjects is now a vector
   // of pointers so deallocate elements!

   //while(mObjects.size())
     // detachObject(mObjects.first().mObject);

   Vector<PathedObject*>::iterator i;

   for( i = mObjects.begin(); i != mObjects.end(); i++ )
   {
      if( !(*i)->mObject.isNull() )
      {
         clearNotify( (*i)->mObject );

         // [neo, 27/6/2007 - #3260]
         (*i)->mObject->setLinearVelocity( Vector2( 0, 0 ) );
         (*i)->mObject->setAttachedToPath(NULL);
      }

      delete (*i);
   }
   
   mObjects.clear();
   mNodes.clear();
}

//---------------------------------------------------------------------------------------------

void Path::linear(PathedObject& object)
{
   // Grab destination, position, and direction.
   Vector2 destination = mNodes[object.mDestinationNode].position;
   Vector2 position = object.mObject->getPosition();
   Vector2 direction = destination - position;
   direction.Normalize();

   // Set the velocity.
   object.mObject->setLinearVelocity(direction * object.mSpeed);

   if (object.getOrientToPath())
   {
      Vector2 direction = position - destination;
      F32 rotation = -mAtan(direction.x, direction.y);
      object.mObject->setAngle( rotation - object.getAngleOffset());
   }
}

//---------------------------------------------------------------------------------------------

void Path::bezier(PathedObject& object, F32 dt)
{
   S32 i = object.mSourceNode;
   S32 j = object.mDestinationNode;
   S32 dir = object.mSourceNode % 2 ? -object.mDirection : object.mDirection;
   Vector2 a = mNodes[i].position;
   F32 rot = mNodes[i].rotation + (b2_pi * 0.5f * dir);
   Vector2 b = a + (Vector2(mCos(rot), mSin(rot)) * mNodes[i].weight);
   Vector2 d = mNodes[j].position;
   if ((j == 0) && (mNodes.size() % 2)) dir = -dir;
   rot = mNodes[j].rotation + (b2_pi * 0.5f * dir);
   Vector2 c = d + (Vector2(mCos(rot), mSin(rot)) * mNodes[j].weight);
   
   F32 length = (object.getDirection() == 1) ? getNode(object.getCurrentNode()).bezierLength : getNode(object.getDestinationNode()).bezierLength;
   object.mTime += dt * object.mSpeed;

   F32 t = object.mTime / length;
   if (t > 1.0f) t = 1.0f;
   F32 it = 1.0f - t;
   Vector2 prevPos = object.mObject->getPosition();
   object.mObject->setPosition((a*it*it*it) + (3*b*it*it*t) + (3*c*it*t*t) + (d*t*t*t));
   Vector2 currPos = object.mObject->getPosition();

   if (object.getOrientToPath())
   {
      Vector2 direction = currPos - prevPos;
      F32 rotation = -mAtan(direction.x, direction.y);
      object.mObject->setAngle( rotation + b2_pi - object.getAngleOffset());
   }
}

//---------------------------------------------------------------------------------------------

void Path::catmull(PathedObject& object, F32 dt)
{
   S32 p0 = object.mSourceNode - object.mDirection;
   S32 p1 = object.mSourceNode;
   S32 p2 = object.mDestinationNode;
   S32 p3 = object.mDestinationNode + object.mDirection;

   F32 length = (object.getDirection() == 1) ? getNode(object.getCurrentNode()).catmullLength : getNode(object.getDestinationNode()).catmullLength;
   object.mTime += dt * object.mSpeed;

   S32 nodeCount = mNodes.size();
   if (p0 < 0) p0 = nodeCount - 1;
   else if (p0 >= nodeCount) p0 = 0;
   if (p3 >= nodeCount) p3 = 0;
   else if (p3 < 0) p3 = nodeCount - 1;

   F32 t = object.mTime / length;
   if (t > 1.0f) t = 1.0f;
   Vector2 pos = 0.5 * ((2 * mNodes[p1].position) + (-mNodes[p0].position + mNodes[p2].position) * t +
      (2 * mNodes[p0].position - 5 * mNodes[p1].position + 4 * mNodes[p2].position - mNodes[p3].position) * t * t +
      (-mNodes[p0].position + 3 * mNodes[p1].position - 3 * mNodes[p2].position + mNodes[p3].position) * t * t * t);

   Vector2 prevPos = object.mObject->getPosition();
   object.mObject->setPosition(pos);
   Vector2 currPos = pos;

   if (object.getOrientToPath())
   {
      Vector2 direction = currPos - prevPos;
      F32 rotation = -mAtan(direction.x, direction.y);
      object.mObject->setAngle(rotation + b2_pi - object.getAngleOffset());
   }
}

//---------------------------------------------------------------------------------------------

void Path::custom(PathedObject& object)
{
   char from[32];
   dSprintf(from, 32, "%g %g", getNode(object.getCurrentNode()).position.x, getNode(object.getCurrentNode()).position.x);
   char to[32];
   dSprintf(to, 32, "%g %g", getNode(object.getDestinationNode()).position.x, getNode(object.getDestinationNode()).position.x);
   Con::executef(this, 4, "onUpdatePath", Con::getIntArg(object.mObject->getId()), from, to);
}

//---------------------------------------------------------------------------------------------

void Path::preIntegrate(const F32 totalTime, const F32 elapsedTime, DebugStats *pDebugStats)
{
   Parent::preIntegrate(totalTime, elapsedTime, pDebugStats);

   Vector<PathedObject*>::iterator i;

   for (i = mObjects.begin(); i != mObjects.end(); i++)
   {
      bool stop = false;
      PathedObject &object = *(*i);

      Vector2 position = object.mObject->getPosition();
      Vector2 destination = mNodes[object.mDestinationNode].position;
      F32 distance = (destination - position).Length();

      // Basically, if the object is going to reach the node this frame, just
      // pass it off as having already reached it.
      if (distance < (object.mSpeed * elapsedTime))
      {
         // Reset time for bezier.
         object.mTime = 0.0f;

         // Check to see if we have arrived at the ultimate end node.
         S32 nodeCount = mNodes.size();
         S32 end = object.mEndNode;
         if (end == -1) end = nodeCount - 1;
         if (object.mDestinationNode == end)
         {
            object.mLoopCounter++;
            if ((object.getTotalLoops() > 0) && (object.getLoop() >= object.getTotalLoops()))
            {
               stop = true;
               if( !mFinished )
                  Con::executef( this, 3, "onPathFinished", Con::getIntArg( object.mObject->getId() ), Con::getIntArg( object.mDestinationNode ) );

               mFinished = true;
            }
            else
            {
               // Action depends on wrap mode and direction.
               S32 temp;
               switch (object.mPathMode)
               {
                  case PATH_WRAP:
                     object.mSourceNode = object.mDestinationNode;
                     object.mDestinationNode += object.mDirection;
                     break;
                  case PATH_REVERSE:
                     object.mDirection = -object.mDirection;
                     object.mSourceNode = object.mDestinationNode;
                     object.mDestinationNode += object.mDirection;
                     temp = object.mEndNode;
                     object.mEndNode = object.mStartNode;
                     object.mStartNode = temp;
                     break;
                  case PATH_RESTART:
                     object.sendToNode(object.mStartNode);
                     object.mSourceNode = object.mStartNode;
                     object.mDestinationNode = object.mSourceNode + object.mDirection;
                     break;
               }

               mFinished = false;
               Con::executef( this, 4, "onReachNode", Con::getIntArg( object.mObject->getId() ), Con::getIntArg( object.mSourceNode ), "1" );
            }
         }
         else
         {
            object.mSourceNode = object.mDestinationNode;
            object.mDestinationNode += object.mDirection;

            mFinished = false;
            Con::executef( this, 4, "onReachNode", Con::getIntArg( object.mObject->getId() ), Con::getIntArg( object.mSourceNode ), "0" );
         }

         // If the new node is out of our bounds, fix it.
         if (object.mDestinationNode >= nodeCount)
            object.mDestinationNode = 0;
         else if (object.mDestinationNode < 0)
            object.mDestinationNode = nodeCount - 1;
      }

      if (!stop)
      {
         switch(mPathType)
         {
            case FOLLOW_LINEAR:
               linear(object);
               break;
            case FOLLOW_BEZIER:
               bezier(object, elapsedTime);
               break;
            case FOLLOW_CATMULL:
               catmull(object, elapsedTime);
               break;
            case FOLLOW_CUSTOM:
               custom(object);
               break;
         }
      }
      else
         object.mObject->setLinearVelocity(Vector2(0, 0));
   }
}

//---------------------------------------------------------------------------------------------

void Path::integrateObject(const F32 totalTime, const F32 elapsedTime, DebugStats *pDebugStats)
{
    // Call Parent.
    Parent::integrateObject( totalTime, elapsedTime, pDebugStats );

   if (getScene()->getIsEditorScene())
   {
      for (S32 i = 0; i < mObjects.size(); i++)
         mObjects[i]->resetObject();
   }
}

//---------------------------------------------------------------------------------------------

#ifdef TORQUE_OS_IOS
void Path::sceneRenderOverlay(const RectF& viewPort )
{
   // Grab the scene/object debug mask.
   U32 debugMask = getDebugMask() | getScene()->getDebugMask();

    glDisableClientState(GL_COLOR_ARRAY);
    glDisableClientState(GL_POINT_SIZE_ARRAY_OES);
    glEnableClientState(GL_VERTEX_ARRAY);
    
    
    
   if (debugMask & Scene::SCENE_DEBUG_OOBB)
   {
      // Greenish-blue.
      glColor4f(0, 255, 255, 255);

      S32 nodeCount = mNodes.size();

      if (mPathType == FOLLOW_CATMULL)
      {
      
            for (S32 i = 0; i < nodeCount; i++)
            {
               S32 p0 = i;
               S32 p1 = i + 1;
               S32 p2 = i + 2;
               S32 p3 = i + 3;
               if (p1 >= nodeCount) { p1 = 0; p2 = 1; p3 = 2; }
               if (p2 >= nodeCount) { p2 = 0; p3 = 1; }
               if (p3 >= nodeCount) { p3 = 0; }

               Vector2 previousPos = mNodes[p1].position;
               for (F32 t = 0.0f; t < 1.01f; t += 0.01f)
               {
                  Vector2 pos = 0.5 * ((2 * mNodes[p1].position) + (-mNodes[p0].position + mNodes[p2].position) * t +
                  (2 * mNodes[p0].position - 5 * mNodes[p1].position + 4 * mNodes[p2].position - mNodes[p3].position) * t * t +
                  (-mNodes[p0].position + 3 * mNodes[p1].position - 3 * mNodes[p2].position + mNodes[p3].position) * t * t * t);

                   GLfloat verts[] = {
                       (GLfloat)previousPos.x, (GLfloat)previousPos.y,
                       (GLfloat)pos.x, (GLfloat)pos.y,
                   };

                  glVertexPointer(2, GL_FLOAT, 0, verts);
                  glDrawArrays(GL_LINES, 0, 2);
                  previousPos = pos;
               }
            }
      }
      else if (mPathType == FOLLOW_BEZIER)
      {
         S32 dir = -1;
            for (S32 i = 0; i < nodeCount; i++)
            {
               dir = -dir;

               S32 j = i + 1;
               if (j == nodeCount) j = 0;

               Vector2 a = mNodes[i].position;
               F32 rot = mNodes[i].rotation + (b2_pi * 0.5f * dir);
               Vector2 b = a + (Vector2(mCos(rot), mSin(rot)) * mNodes[i].weight);
               Vector2 d = mNodes[j].position;
               if ((j == 0) && (nodeCount % 2)) dir = -dir;
               rot = mNodes[j].rotation + (b2_pi * 0.5f * dir);
               Vector2 c = d + (Vector2(mCos(rot), mSin(rot)) * mNodes[j].weight);

               Vector2 previousPos = mNodes[i].position;
               for (F32 t = 0.0f; t < 1.01f; t += 0.01f)
               {
                  F32 it = 1.0f - t;
                  Vector2 pos = (a * it * it * it) + (3 * b * it * it * t) +
                                  (3 * c * it * t * t) + (d * t * t * t);

                   GLfloat verts[] = {
                       (GLfloat)previousPos.x, (GLfloat)previousPos.y,
                       (GLfloat)pos.x, (GLfloat)pos.y,
                   };
                   
                  glVertexPointer(2, GL_FLOAT, 0, verts);
                  glDrawArrays(GL_LINES, 0, 2);
                  previousPos = pos;
               }
            }
      }
      else if (mPathType == FOLLOW_LINEAR)
      {
            for (S32 i = 1; i <= nodeCount; i++)
            {
                GLfloat verts[] = {
                    (GLfloat)(mNodes[i - 1].position.x), (GLfloat)(mNodes[i - 1].position.y),
                    (GLfloat)(mNodes[i == nodeCount ? 0 : i].position.x), (GLfloat)(mNodes[i == nodeCount ? 0 : i].position.y),
                };
                glVertexPointer(2, GL_FLOAT, 0, verts);
                glDrawArrays(GL_LINES, 0, 2);
            }
      }

      // Draw boxes at each node. The size of the box depends on the weight.
      for (S32 i = 0; i < nodeCount; i++)
      {
         F32 xOffset = mNodeRenderSize;
         F32 yOffset = mNodeRenderSize;
         Vector2 pos0 = Vector2(-xOffset, -yOffset);
         Vector2 pos1 = Vector2(-xOffset, yOffset);
         Vector2 pos2 = Vector2(xOffset, yOffset);
         Vector2 pos3 = Vector2(xOffset, -yOffset);

         if (mPathType == FOLLOW_BEZIER)
         {
            pos0.rotate(mNodes[i].rotation);
            pos1.rotate(mNodes[i].rotation);
            pos2.rotate(mNodes[i].rotation);
            pos3.rotate(mNodes[i].rotation);
         }

         pos0 += mNodes[i].position;
         pos1 += mNodes[i].position;
         pos2 += mNodes[i].position;
         pos3 += mNodes[i].position;

          GLfloat verts[] = {
              (GLfloat)(pos0.x), (GLfloat)(pos0.y),
              (GLfloat)(pos1.x), (GLfloat)(pos1.y),
              (GLfloat)(pos2.x), (GLfloat)(pos2.y),
              (GLfloat)(pos3.x), (GLfloat)(pos3.y),
          };
          glVertexPointer(2, GL_FLOAT, 0, verts);
          glDrawArrays(GL_LINE_LOOP, 0, 4);
      }
   }
}

#else

void Path::sceneRenderOverlay(const RectF& viewPort )
{
   // Grab the scene/object debug mask.
   U32 debugMask = getDebugMask() | getScene()->getDebugMask();

   if (debugMask)
   {
      // Greenish-blue.
      glColor3f(0, 255, 255);

      S32 nodeCount = mNodes.size();

      if (mPathType == FOLLOW_CATMULL)
      {
         glBegin(GL_LINES);
            for (S32 i = 0; i < nodeCount; i++)
            {
               S32 p0 = i;
               S32 p1 = i + 1;
               S32 p2 = i + 2;
               S32 p3 = i + 3;
               if (p1 >= nodeCount) { p1 = 0; p2 = 1; p3 = 2; }
               if (p2 >= nodeCount) { p2 = 0; p3 = 1; }
               if (p3 >= nodeCount) { p3 = 0; }

               Vector2 previousPos = mNodes[p1].position;
               for (F32 t = 0.0f; t < 1.01f; t += 0.01f)
               {
                  Vector2 pos = 0.5 * ((2 * mNodes[p1].position) + (-mNodes[p0].position + mNodes[p2].position) * t +
                  (2 * mNodes[p0].position - 5 * mNodes[p1].position + 4 * mNodes[p2].position - mNodes[p3].position) * t * t +
                  (-mNodes[p0].position + 3 * mNodes[p1].position - 3 * mNodes[p2].position + mNodes[p3].position) * t * t * t);
                  glVertex2fv((GLfloat*)&previousPos);
                  glVertex2fv((GLfloat*)&pos);
                  previousPos = pos;
               }
            }
         glEnd();
      }

      else if (mPathType == FOLLOW_BEZIER)
      {
         S32 dir = -1;
         glBegin(GL_LINES);
            for (S32 i = 0; i < nodeCount; i++)
            {
               dir = -dir;

               S32 j = i + 1;
               if (j == nodeCount) j = 0;

               Vector2 a = mNodes[i].position;
               F32 rot = mNodes[i].rotation + (b2_pi * 0.5f * dir);
               Vector2 b = a + (Vector2(mCos(rot), mSin(rot)) * mNodes[i].weight);
               Vector2 d = mNodes[j].position;
               if ((j == 0) && (nodeCount % 2)) dir = -dir;
               rot = mNodes[j].rotation + (b2_pi * 0.5f * dir);
               Vector2 c = d + (Vector2(mCos(rot), mSin(rot)) * mNodes[j].weight);

               Vector2 previousPos = mNodes[i].position;
               for (F32 t = 0.0f; t < 1.01f; t += 0.01f)
               {
                  F32 it = 1.0f - t;
                  Vector2 pos = (a * it * it * it) + (3 * b * it * it * t) +
                                  (3 * c * it * t * t) + (d * t * t * t);
                  glVertex2fv((GLfloat*)&previousPos);
                  glVertex2fv((GLfloat*)&pos);
                  previousPos = pos;
               }
            }
         glEnd();
      }

      else if (mPathType == FOLLOW_LINEAR)
      {
         glBegin(GL_LINES);
            for (S32 i = 1; i <= nodeCount; i++)
            {
               glVertex2fv((GLfloat*)&(mNodes[i - 1].position));
               glVertex2fv((GLfloat*)&(mNodes[i == nodeCount ? 0 : i].position));
            }
         glEnd();
      }

      // Draw boxes at each node. The size of the box depends on the weight.
      for (S32 i = 0; i < nodeCount; i++)
      {
         F32 xOffset = mNodeRenderSize;
         F32 yOffset = mNodeRenderSize;
         Vector2 pos0 = Vector2(-xOffset, -yOffset);
         Vector2 pos1 = Vector2(-xOffset, yOffset);
         Vector2 pos2 = Vector2(xOffset, yOffset);
         Vector2 pos3 = Vector2(xOffset, -yOffset);

         if (mPathType == FOLLOW_BEZIER)
         {
            pos0.rotate(mNodes[i].rotation);
            pos1.rotate(mNodes[i].rotation);
            pos2.rotate(mNodes[i].rotation);
            pos3.rotate(mNodes[i].rotation);
         }

         pos0 += mNodes[i].position;
         pos1 += mNodes[i].position;
         pos2 += mNodes[i].position;
         pos3 += mNodes[i].position;

         glBegin(GL_LINE_LOOP);
            glVertex2fv((GLfloat*)&(pos0));
            glVertex2fv((GLfloat*)&(pos1));
            glVertex2fv((GLfloat*)&(pos2));
            glVertex2fv((GLfloat*)&(pos3));
         glEnd();
      }
   }
}
#endif

//---------------------------------------------------------------------------------------------

void Path::writeFields(Stream &stream, U32 tabStop)
{
   S32 nodeCount = getNodeCount();
   char countString[9];
   dSprintf(countString, 9, "%d", nodeCount);
   setDataField(StringTable->insert("nodeCount"), NULL, countString);
   for (S32 i = 0; i < nodeCount; i++)
   {
      Path::PathNode& node = getNode(i);
      char nodeData[128];
      char nodeIndex[4];
      dSprintf(nodeData, 128, "%g %g %g %g", node.position.x, node.position.y, mRadToDeg(node.rotation), node.weight);
      dSprintf(nodeIndex, 4, "%d", i);
      setDataField(StringTable->insert("node"), nodeIndex, nodeData);
   }
   
   S32 objectCount = mObjects.size();
   char objectString[8];
   dSprintf(objectString, 8, "%d", objectCount);
   setDataField(StringTable->insert("objectCount"), NULL, objectString);
   for (S32 i = 0; i < objectCount; i++)
   {
      PathedObject& object = *mObjects[i];
      if (object.mObject.isNull())
         continue;

      char objectData[512];
      char objectIndex[4];
      dSprintf(objectData, 512, "%s %d %d %g %d %d %g %d %s", object.mObject->getDataField(StringTable->insert("mountID"), NULL),
                                                           object.getStartNode(), object.getEndNode(), object.getSpeed(),
                                                           object.getDirection(), object.getOrientToPath(), mRadToDeg(object.getAngleOffset()),
                                                           object.getTotalLoops(), getPathModeDescription(object.getPathMode()));
      dSprintf(objectIndex, 4, "%d", i);
      setDataField(StringTable->insert("object"), objectIndex, objectData);
   }
   
   Parent::writeFields(stream, tabStop);

   // [neo, 7/6/2007 - #3207]
   // These fields are just used for saving, clear them out again so we don't
   // get another copy of ourselves by mistake when gets called.
   setDataField(StringTable->insert("nodeCount"), NULL, "");

   for( S32 i = 0; i < nodeCount; i++ )
   {
      char nodeIndex[ 4 ];
      
      dSprintf( nodeIndex, 4, "%d", i );

      setDataField( StringTable->insert( "node" ), nodeIndex, "" );
   }
}

//---------------------------------------------------------------------------------------------

void Path::onTamlPostRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlPostRead( customCollection );

   if (!getScene())
      return;

   S32 nodeCount = dAtoi(getDataField(StringTable->insert("nodeCount"), NULL));
   S32 objectCount = dAtoi(getDataField(StringTable->insert("objectCount"), NULL));
   setDataField(StringTable->insert("nodeCount"), NULL, "");
   setDataField(StringTable->insert("objectCount"), NULL, "");

   for (S32 i = 0; i < nodeCount; i++)
   {
      char nodeIndex[4];
      dSprintf(nodeIndex, 4, "%d", i);
      const char* nodeData = getDataField(StringTable->insert("node"), nodeIndex);

      Vector2 position;
      F32 rotation, weight;
      dSscanf(nodeData, "%g %g %g %g", &position.x, &position.y, &rotation, &weight);

      rotation = mDegToRad(rotation);

      // Only add if this node doesn't exist
      if( findNode( position, rotation, weight ) == -1 )
         addNode(position, rotation, weight);

      setDataField(StringTable->insert("node"), nodeIndex, "");
   }

   for (S32 i = 0; i < objectCount; i++)
   {
        char objectIndex[4];
        dSprintf(objectIndex, 4, "%d", i);
        const char* objectData = getDataField(StringTable->insert("object"), objectIndex);

        S32 mountID, start, end, direction, loops;
        F32 speed;
        S32 orient;
        F32 offset = 0.0f;
        char pathMode[32];

        // For backwards compatibility.
        S32 count =Utility::mGetStringElementCount(objectData);
        if (count == 8)
            dSscanf(objectData, "%d %d %d %g %d %d %d %s", &mountID, &start, &end, &speed, &direction, &orient, &loops, &pathMode);
        else
            dSscanf(objectData, "%d %d %d %g %d %d %g %d %s", &mountID, &start, &end, &speed, &direction, &orient, &offset, &loops, &pathMode);

        // Iterate scene objects.
        typeSceneObjectVectorConstRef sceneObjects = getScene()->getSceneObjects();
        for( S32 n = 0; n < sceneObjects.size(); ++n )
        {
            // Fetch scene object.
            SceneObject* pSceneObject = sceneObjects[n];

            if (dAtoi(pSceneObject->getDataField(StringTable->insert("mountID"), NULL)) == mountID)
            {
                attachObject(pSceneObject, speed, direction, orient, start, end, getPathMode(pathMode), loops, true);
                setAngleOffset(pSceneObject, offset, true);
                break;
            }
        }

        setDataField(StringTable->insert("object"), objectIndex, "");
   }
}

//---------------------------------------------------------------------------------------------

S32 Path::findNode( Vector2 position, F32 rotation, F32 weight )
{
   S32 nodeCount = mNodes.size();
   for( S32 nI = 0; nI < nodeCount; nI++ )
   {
      PathNode &pNode = mNodes[ nI ];

      // Check P/R/W 
      if( pNode.position == position && 
          pNode.rotation == rotation && 
          pNode.weight == weight )
         return nI;
   }

   // No match
   return -1;
}
