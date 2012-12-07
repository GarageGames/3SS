//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PATH_H_
#define _PATH_H_

#include "2d/sceneobject/SceneObject.h"

//---------------------------------------------------------------------------------------------

class Path;

//---------------------------------------------------------------------------------------------

enum ePathMode
{
   PATH_WRAP,                      // Loop.
   PATH_REVERSE,                   // Reverse directions at the end node.
   PATH_RESTART                    // Warp to the start node and restart.
};

//---------------------------------------------------------------------------------------------

enum eFollowMethod
{
   FOLLOW_LINEAR,
   FOLLOW_BEZIER,
   FOLLOW_CATMULL,
   FOLLOW_CUSTOM
};

//---------------------------------------------------------------------------------------------

class Path : public SceneObject
{
   typedef SceneObject Parent;

public:
    class PathedObject
    {
    public:
       PathedObject()
       {
           mObject = NULL;
           mPath = NULL;
           mSourceNode = 0;
           mDestinationNode = 0;
           mStartNode = 0;
           mEndNode = 0;
           mSpeed = 10.0f;
           mStartDirection = 1;
           mDirection = 1;
           mOrientToPath = true;
           mLoopCounter = 0;
           mTotalLoops = 0;
           mPathMode = PATH_WRAP;
           mTime = 0.0f;
       }
       virtual ~PathedObject() {};

       // Set Attributes
       void setSpeed(F32 speed, bool reset = false) { mSpeed = speed; if (reset) resetObject(); };
       void setDirection(S32 direction, bool reset = false) { mDirection = direction; if (reset) resetObject(); };
       void setStartDirection(S32 direction, bool reset = false) { mStartDirection = direction; if (reset) resetObject(); };
       void setLoop(S32 loop, bool reset = false) { mLoopCounter = loop; if (reset) resetObject(); };
       void setTotalLoops(S32 loops, bool reset = false) { mTotalLoops = loops; if (reset) resetObject(); };
       void setOrientToPath(bool orientToPath, bool reset = false) { mOrientToPath = orientToPath; if (reset) resetObject(); };
       void setPathMode(ePathMode pathMode, bool reset = false) { mPathMode = pathMode; if (reset) resetObject(); };
       void setAngleOffset(F32 radians, bool reset = false) { mAngleOffset = radians; if (reset) resetObject(); };
       inline void resetObject();
       inline bool sendToNode(S32 index);
       inline void setStartNode(S32 startNode, bool reset = false);
       inline void setEndNode(S32 endNode, bool reset = false);

       // Get attribute methods.
       inline F32 getSpeed() const { return mSpeed; };
       inline S32 getDirection() const { return mDirection; };
       inline S32 getStartNode() const { return mStartNode; };
       inline S32 getEndNode() const { return mEndNode; };
       inline S32 getCurrentNode() const { return mSourceNode; };
       inline S32 getDestinationNode() const { return mDestinationNode; };
       inline S32 getLoop() const { return mLoopCounter; };
       inline S32 getTotalLoops() const { return mTotalLoops; };
       inline bool getOrientToPath() const { return mOrientToPath; };
       inline ePathMode getPathMode() const { return mPathMode; };
       inline F32 getAngleOffset() const { return mAngleOffset; };

    private:
       friend class Path;

       void setCurrentNode(S32 node);
       void setDestinationNode(S32 node);

       Path* mPath;                       // The path this is attached to.
       SimObjectPtr<SceneObject> mObject; // The object this is wrapping.
       S32 mSourceNode;                      // The most recently passed node.
       S32 mDestinationNode;                 // The node being headed toward.
       S32 mStartNode;                       // The node at the beginning of the path.
       S32 mEndNode;                         // The node at the end of the path.
       F32 mSpeed;                           // The speed to move the object at.
       S32 mStartDirection;                  // The direction the object is initially traveling.
       S32 mDirection;                       // The direction the object is traveling (-1 or 1).
       bool mOrientToPath;                   // Follow the orientation of the path.
       S32 mLoopCounter;                     // The current loop the object is on.
       S32 mTotalLoops;                      // The number of loops to take around the path.
       F32 mTime;                            // The parametric time of the location on the path.
       ePathMode mPathMode;                  // The action to take upon path completion.
       F32 mAngleOffset;					     // The rotation offset of the object when using orient to path.

       //[neo, 5/22/2007 - #3139]
       // Used as a marker so we can map the object to this instance
       // By the time onDeleteNotify is called the mObject ref has already
       // been zeroed by the notification code so we can't use that.
       SimObjectId mObjectId;                       
    };

   struct PathNode
   {
      // Constructor.
      PathNode(Vector2 _position, F32 _rotation, F32 _weight)
      {
         position = _position;
         rotation = _rotation;
         weight = _weight;
         bezierLength = 0.0f;
         catmullLength = 0.0f;
      };

      Vector2 position;                 // The position of the node.
      F32 rotation;                       // The rotation of the node.
      F32 weight;                         // The weight of the node.
      F32 bezierLength;                   // The bezier length from this node to the next.
      F32 catmullLength;                  // The catmull length from this node to the next.
   };

   // Constructor/Destructor.
   Path();
   virtual ~Path();

   virtual void onDeleteNotify( SimObject* object );

   static void initPersistFields();

   virtual void onRemove();

   // Handles the path following.
   virtual void preIntegrate(const F32 totalTime, const F32 elapsedTime, DebugStats *pDebugStats);
   // For editor support. Resets pathed objects.
   virtual void integrateObject(const F32 totalTime, const F32 elapsedTime, DebugStats* pDebugStats);

   // Renders the path when debug modes are set.
   virtual void sceneRenderOverlay(const RectF& viewPort);

   virtual void setPosition(const Vector2& position);
   virtual void setSize(const Vector2& size);

   // Update the size and position to encompass all nodes.
   void updateSize();

   // Node management.
   S32 addNode(Vector2 position, F32 rotation, F32 weight, S32 location = -1);
   // Returns the node index or -1 if no matches
   S32 findNode( Vector2 position, F32 rotation, F32 weight );
   S32 removeNode(U32 index);
   void clear();
   S32 getNodeCount() const { return mNodes.size(); };

   void setPathType(eFollowMethod pathType) { mPathType = pathType; };
   eFollowMethod getPathType() const        { return mPathType; };

   void setNodeRenderSize(F32 size) { mNodeRenderSize = size; };
   F32 getNodeRenderSize() const { return mNodeRenderSize; };

   // Add and remove objects from the path.
   void attachObject(SceneObject* object, F32 speed, S32 direction, bool orientToPath,
                     S32 startNode, S32 endNode, ePathMode pathMode, S32 loops, bool sendToStart);
   void detachObject(SceneObject* object);

   S32 getPathedObjectCount()
   {
      return mObjects.size();
   };

   SceneObject* getPathedObject( U32 index )
   {
      if( index < (U32)mObjects.size() )
         return mObjects[index]->mObject;

      return NULL;
   };

   // Get the pathed object that represents a scene object.
   inline PathedObject* getPathedObject(const SceneObject* obj)
   {
      Vector<PathedObject*>::iterator i;
      for (i = mObjects.begin(); i != mObjects.end(); ++i)
         if ((*i)->mObject == obj) return *i;

      return NULL;
   };

   // Set Properties on Pathed Objects.
   void setStartNode(SceneObject* object, S32 node)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setStartNode(node, true);
   }
   void setEndNode(SceneObject* object, S32 node)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setEndNode(node, true);
   }
   void setSpeed(SceneObject* object, F32 speed, bool resetObject)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setSpeed(speed, resetObject);
   }
   void setMoveForward(SceneObject* object, bool forward, bool resetObject)
   {
      setDirection(object, forward ? 1 : -1, resetObject);
   }
   void setDirection(SceneObject* object, S32 direction, bool resetObject)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setStartDirection(direction, resetObject);
   }
   void setOrient(SceneObject* object, bool orient, bool resetObject)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setOrientToPath(orient, resetObject);
   }
   void setAngleOffset(SceneObject* object, F32 offset, bool resetObject)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setAngleOffset(offset, resetObject);
   }
   void setLoops(SceneObject* object, S32 loops, bool resetObject)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setTotalLoops(loops, resetObject);
   }
   void setFollowMode(SceneObject* object, ePathMode followMode, bool resetObject)
   {
      PathedObject* pathedObject = getPathedObject(object);
      if (pathedObject) pathedObject->setPathMode(followMode, resetObject);
   }

   // Get Properties on Pathed Objects.
   S32 getStartNode(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getStartNode() : -1;
   }
   S32 getEndNode(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getEndNode() : -1;
   }
   F32 getSpeed(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getSpeed() : 0.0f;
   }
   bool getMoveForward(SceneObject* object)
   {
      return getDirection(object) == 1 ? true : false;
   }
   S32 getDirection(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getDirection() : 0;
   }
   bool getOrient(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getOrientToPath() : false;
   }
   F32 getAngleOffset(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getAngleOffset() : false;
   }
   S32 getLoops(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getTotalLoops() : -1;
   }
   ePathMode getFollowMode(SceneObject* object)
   {
      PathedObject* pathedObject = getPathedObject(object);
      return pathedObject ? pathedObject->getPathMode() : PATH_WRAP;
   }

   inline PathNode& getNode(S32 index)
   {
      if (isValidNode(index)) return mNodes[index];
      return mNodes[0];
   };

   inline bool isValidNode(S32 index)
   {
      if (mNodes.empty()) return false;
      if ((index >= 0) && (index < mNodes.size())) return true;
      return false;
   };

   DECLARE_CONOBJECT(Path);

   virtual void writeFields(Stream &stream, U32 tabStop);

private:
   // Make sure all the PathedObjects still reference valid nodes.
   void checkObjectNodes();

   // Traversal methods.
   void linear(PathedObject& object);
   void bezier(PathedObject& object, F32 dt);
   void catmull(PathedObject& object, F32 dt);
   void custom(PathedObject& object);

   // Calculate the length from one node to the next based on the interpolation type.
   void calculateBezierLength(S32 node);
   void calculateCatmullLength(S32 node);

   // The type of path.
   eFollowMethod mPathType;
   bool mNodesLoaded;
   Vector<S32> mObjectsLoaded;
   S32 mMountOffset;

   bool mFinished;

   // The size to render the path nodes at.
   F32 mNodeRenderSize;

   // The objects and nodes.
   // [neo, 5/22/2007 - #3139]
   // PathedObject::mObject is a SimObjectPtr and calling erase_fast() etc or any reallocation
   // will invalidate the reference stored with the simobject it points to and so the object
   // will have a reference to garbage memory. We could just manually copy it in the specified
   // cases but this way we wont miss anything by accident.
   //Vector<PathedObject> mObjects;
   Vector<PathedObject*> mObjects;
   Vector<PathNode>      mNodes;

protected:
    virtual void onTamlPostRead( const TamlCollection& customCollection );

protected:
    static bool setPathType(void* obj, const char* data);
    static bool writePathType( void* obj, StringTableEntry pFieldName ) { PREFAB_WRITE_CHECK(Path); return pCastObject->mPathType != FOLLOW_LINEAR; }

};

inline void Path::PathedObject::resetObject()
{
   sendToNode(mStartNode);

   mTime = 0.0f;
   mLoopCounter = 0;
   mSourceNode = mStartNode;
   mDestinationNode = mStartNode;
   mDirection = mStartDirection;
}

inline bool Path::PathedObject::sendToNode(S32 index)
{
   Path::PathNode node = mPath->getNode(index);

   mObject->setPosition(node.position);
   if (mOrientToPath)
      mObject->setAngle(node.rotation);

   return true;
}

inline void Path::PathedObject::setCurrentNode(S32 node)
{
   if (mPath->isValidNode(node))
      mSourceNode = node;
}

inline void Path::PathedObject::setDestinationNode(S32 node)
{
   if (mPath->isValidNode(node))
      mDestinationNode = node;
}

inline void Path::PathedObject::setStartNode(S32 startNode, bool reset)
{
   if (!mPath->isValidNode(startNode))
      return;

   mStartNode = startNode;
   if (reset) resetObject();
};

inline void Path::PathedObject::setEndNode(S32 endNode, bool reset)
{
   if (!mPath->isValidNode(endNode))
      return;

   mEndNode = endNode;
   if (reset) resetObject();
}

//---------------------------------------------------------------------------------------------

extern ePathMode getPathMode(const char* label);
extern const char* getPathModeDescription(const ePathMode pathMode);
extern eFollowMethod getFollowMethod(const char* label);
extern const char* getFollowMethodDescription(const eFollowMethod follow);

#endif
