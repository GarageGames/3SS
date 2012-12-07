//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _AIMOVEMENTCOMPONENT_H_
#define _AIMOVEMENTCOMPONENT_H_

#include "component/dynamicConsoleMethodComponent.h"
#include "2d/sceneobject/SceneObject.h"

class AIMovementComponent : public DynamicConsoleMethodComponent
{
    typedef DynamicConsoleMethodComponent Parent;

public:

   enum
   {
      MAX_AGENTS = 2,
   };

   enum
   {
      PATH_UNKNOWN,
      PATH_T2DPATH,
      PATH_ASTAR2D,  // Depreciated
   };

   struct MovementParameters
   {
      F32         elapsedTime;

      Point2F     ownerPosition;
      F32         ownerAngle;
      F32         ownerMotionAngle;
      Vector2   ownerVelocity;
      F32         ownerMaxSpeed;
      F32         ownerMaxForce;
      F32         ownerArriveDecelerationRate;

      Point2F target;

      SceneObject* agent[MAX_AGENTS];

      SceneObject*   path;
      S32               pathType;
      F32               pathWayPointDistance;
      bool              pathLoop;
      Vector2         pathOffset;
   };

protected:

    //
    SimObjectPtr<SceneObject> mOwner;

   /// Is this component active and allowed to perform its calculations?
   bool mActive;

   /// Defines the object to send callbacks to.  This is often set to the
   /// script behavior this component is part of, but it could also be
   /// the component's owner.  If not set then callbacks go to this component.
   SimObjectPtr<SimObject> mCallbackDestination;

   F32 mLastTime;

   /// The last known rotation angle based on the owner's motion
   /// (linear velocity)
   F32 mLastMotionBasedRotation;

   /// Max speed allowed by the owner due to AI movement.
   F32 mMaxSpeed;

   /// Max force allowed to be applied by AI movement.
   F32 mMaxForce;

   /// After all steering forces have been calculated but before the mMaxForce
   /// is tested, this multiplier is applied.
   F32 mForceMultiplier;

   /// Rate at which to decelerate when using the Arrive steering behavior.  A value
   /// of 0 is no deceleration (becomes a Seek).
   F32 mArriveDecelerationRate;

   /// Should the owner be rotated into the motion produced by AI movement?
   bool mRotateIntoMotion;

   Point2F     mTarget;
   const char* mTargetName;
   SimObjectPtr<SceneObject> mTargetObject;

   const char* mAgentName[MAX_AGENTS];
   SimObjectPtr<SceneObject> mAgent[MAX_AGENTS];

   const char* mPathName;
   SimObjectPtr<SceneObject> mPathObject;
   S32 mPathType;
   F32 mWayPointDistance;
   bool mPathLoop;
   Point2F mPathOffset;

   MovementParameters   mParameters;

   Vector2 calculateWeightedSum();

public:

    DECLARE_CONOBJECT(AIMovementComponent);

    AIMovementComponent();
   virtual ~AIMovementComponent();

    bool onAdd();
    void onRemove();

    static void initPersistFields();

   //
   virtual bool addComponent( SimComponent *component );

    //
    virtual bool onComponentAdd(SimComponent *target);
    virtual void onComponentRemove(SimComponent *target);

    //
    virtual void onAddToScene();
    virtual void onUpdate();

   void setAIActive(bool state) { mActive = state; }
   bool isAIActive() { return mActive; }

   //
   SimObject* getCallbackDestination();
   void setCallbackDestination(SimObject* obj) { mCallbackDestination = obj; }

   //
   void updateMovement();

   // Seek steering is very common
   static Vector2 calculateSeek(Point2F& target, Point2F& origin, Vector2& velocity, F32 maxSpeed);

   // Arrive steering is handy
   static Vector2 calculateArrive(Point2F& target, Point2F& origin, Vector2& velocity, F32 maxSpeed, F32 decelerationRate);

   SceneObject* getTargetObject() { return mTargetObject; }
   void setTargetObject(SceneObject* object);

   static bool setPathField( void* obj, const char* data );
   SceneObject* getPathObject() { return mPathObject; }
   void setPathObject(SceneObject* object);
   S32 getPathType(SceneObject* path);

   F32 getLastMotionBasedRotation() { return mLastMotionBasedRotation; }
};

#endif   // _AIMOVEMENTCOMPONENT_H_
