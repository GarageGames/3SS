//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCENE_OBJECT_TIMER_EVENT_H_
#define _SCENE_OBJECT_TIMER_EVENT_H_

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

//-----------------------------------------------------------------------------

class SceneObjectTimerEvent : public SimEvent
{
public:
    SceneObjectTimerEvent( U32 timerPeriod ) : mTimerPeriod(timerPeriod) {}
    virtual  ~SceneObjectTimerEvent() {}

    virtual void process(SimObject *object)
    {
        /// Create new Timer Event.
        SceneObjectTimerEvent* pEvent = new SceneObjectTimerEvent( mTimerPeriod );

        /// Post Event.
        (dynamic_cast<SceneObject*>(object))->setPeriodicTimerID( Sim::postEvent( object, pEvent, Sim::getCurrentTime() + mTimerPeriod ) );

        // Script callback.
        /// This *must* be done here in-case the user turns off the timer which would be the one above!
        Con::executef( object, 1, "onTimer" );
    }

private:
    U32 mTimerPeriod;
};

#endif // _SCENE_OBJECT_TIMER_EVENT_H_