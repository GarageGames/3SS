//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _BEHAVIORCOMPONENT_RAISEEVENT_H_
#define _BEHAVIORCOMPONENT_RAISEEVENT_H_

#ifndef _BEHAVIOR_COMPONENT_H_
#include "behaviorComponent.h"
#endif

//-----------------------------------------------------------------------------

class BehaviorComponentRaiseEvent : public SimEvent
{
public:
    BehaviorComponentRaiseEvent( BehaviorInstance* pOutputBehavior, StringTableEntry pOutputName )
    {
        // Sanity!
        AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
        AssertFatal( pOutputBehavior->isProperlyAdded(), "Output behavior is not registered." );
        AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );

        mpOutputBehavior = pOutputBehavior;
        mpOutputName = pOutputName;
    }
    virtual  ~BehaviorComponentRaiseEvent() {}

    virtual void process(SimObject *object)
    {
        // Fetch behavior component.
        BehaviorComponent* pBehaviorComponent = dynamic_cast<BehaviorComponent*>( object );

        // Sanity!
        AssertFatal( pBehaviorComponent, "BehaviorComponentRaiseEvent() - Could not process scheduled signal raise as the event was not raised on a behavior component." ); 

        // Is the output behavior still around?
        if ( !mpOutputBehavior )
        {
            // No, so warn.
            Con::warnf( "BehaviorComponentRaiseEvent() - Could not raise output '%s' on behavior as the behavior is not longer present.", mpOutputName );
            return;
        }

        // Raise output signal.
        pBehaviorComponent->raise( mpOutputBehavior, mpOutputName );
    }

private:
    SimObjectPtr<BehaviorInstance>  mpOutputBehavior;
    StringTableEntry                mpOutputName;
};

#endif // _BEHAVIORCOMPONENT_RAISEEVENT_H_