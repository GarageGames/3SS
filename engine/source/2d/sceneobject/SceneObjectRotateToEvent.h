//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_OBJECT_ROTATE_TO_EVENT_H_
#define _SCENE_OBJECT_ROTATE_TO_EVENT_H_

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

//-----------------------------------------------------------------------------

class SceneObjectRotateToEvent : public SimEvent
{
public:
    SceneObjectRotateToEvent( const F32 targetAngle, const bool autoStop ) :
      mAutoStop( autoStop ),
      mTargetAngle( targetAngle ) {}
    virtual ~SceneObjectRotateToEvent() {};

    virtual void process(SimObject *object)
    {
        // Fetch scene object.
        SceneObject* pSceneObject = (dynamic_cast<SceneObject*>(object));
        if (pSceneObject == NULL )
            return;

        // Are we auto stopping?
        if ( mAutoStop )
        {
            // Yes, so angular velocity.
            pSceneObject->setAngularVelocity( 0.0f );
        }

        // Reset event Id.
        pSceneObject->mRotateToEventId = 0;

        // Script callback.
        Con::executef( object, 2, "onRotateToComplete", Con::getFloatArg((mRadToDeg(mTargetAngle))) );
    }

private:
    F32         mTargetAngle;
    bool        mAutoStop;
};

#endif // _SCENE_OBJECT_ROTATE_TO_EVENT_H_