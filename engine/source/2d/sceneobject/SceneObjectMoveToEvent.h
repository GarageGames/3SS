//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_OBJECT_MOVE_TO_EVENT_H_
#define _SCENE_OBJECT_MOVE_TO_EVENT_H_

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

//-----------------------------------------------------------------------------

class SceneObjectMoveToEvent : public SimEvent
{
public:
    SceneObjectMoveToEvent( const Vector2& targetWorldPoint, const bool autoStop ) :
        mAutoStop( autoStop ),
        mTargetWorldPoint( targetWorldPoint ) {}
    virtual ~SceneObjectMoveToEvent() {}

    virtual void process(SimObject *object)
    {
        // Fetch scene object.
        SceneObject* pSceneObject = (dynamic_cast<SceneObject*>(object));
        if (pSceneObject == NULL )
            return;

        // Are we auto stopping?
        if ( mAutoStop )
        {
            // Yes, so reset linear velocity.
            pSceneObject->setLinearVelocity( Vector2::getZero() );
        }

        // Reset event Id.
        pSceneObject->mMoveToEventId = 0;

        // Script callback.
        Con::executef( object, 2, "onMoveToComplete", mTargetWorldPoint.scriptThis() );
    }

private:
    Vector2   mTargetWorldPoint;
    bool        mAutoStop;
};

#endif // _SCENE_OBJECT_MOVE_TO_EVENT_H_