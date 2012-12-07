//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/dgl.h"
#include "console/consoleTypes.h"
#include "io/bitStream.h"
#include "Trigger.h"

// Script bindings.
#include "Trigger_ScriptBinding.h"

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
#include "debug/profiler.h"
#endif

//------------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(Trigger);

//-----------------------------------------------------------------------------

Trigger::Trigger()
{
    // Setup some debug vector associations.
    VECTOR_SET_ASSOCIATION(mEnterColliders);
    VECTOR_SET_ASSOCIATION(mLeaveColliders);

    // Set default callbacks.
    mEnterCallback = true;
    mStayCallback = false;
    mLeaveCallback = true;

    // Use a static body by default.
    mBodyDefinition.type = b2_staticBody;

    // Gather contacts.
    mGatherContacts = true;
}

//-----------------------------------------------------------------------------

bool Trigger::onAdd()
{
    // Eventually, we'll need to deal with Server/Client functionality!

    // Call Parent.
    if(!Parent::onAdd())
        return false;

    // Return Okay.
    return true;
}

//-----------------------------------------------------------------------------

void Trigger::initPersistFields()
{
   addProtectedField("EnterCallback", TypeBool, Offset(mEnterCallback, Trigger), &setEnterCallback, &defaultProtectedGetFn, &writeEnterCallback,"");
   addProtectedField("StayCallback", TypeBool, Offset(mStayCallback, Trigger), &setStayCallback, &defaultProtectedGetFn, &writeStayCallback, "");
   addProtectedField("LeaveCallback", TypeBool, Offset(mLeaveCallback, Trigger), &setLeaveCallback, &defaultProtectedGetFn, &writeLeaveCallback, "");

   Parent::initPersistFields();
}

//-----------------------------------------------------------------------------

void Trigger::preIntegrate(const F32 totalTime, const F32 elapsedTime, DebugStats *pDebugStats)
{
    // Call Parent.
    Parent::preIntegrate(totalTime, elapsedTime, pDebugStats);

    // Clear Collider Callback Lists.
    mEnterColliders.clear();
    mLeaveColliders.clear();
}

//-----------------------------------------------------------------------------

void Trigger::integrateObject( const F32 totalTime, const F32 elapsedTime, DebugStats *pDebugStats )
{
    // Call Parent.
    Parent::integrateObject(totalTime, elapsedTime, pDebugStats);

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Trigger_IntegrateObject);
#endif

    // Perform "OnEnter" callback.
    if ( mEnterCallback && mEnterColliders.size() > 0 )
    {
// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Trigger_OnEnterCallback);
#endif

    for ( collideCallbackType::iterator contactItr = mEnterColliders.begin(); contactItr != mEnterColliders.end(); ++contactItr )
    {
        Con::executef(this, 2, "onEnter", (*contactItr)->getIdString());
    }

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Trigger_OnEnterCallback
#endif
    }

    // Fetch current contacts.
    const typeContactVector* pCurrentContacts = getCurrentContacts();

    // Sanity!
    AssertFatal( pCurrentContacts != NULL, "Trigger::integrateObject() - Contacts not initialized correctly." );

    // Perform "OnStay" callback.
    if ( mStayCallback && pCurrentContacts->size() > 0 )
    {
// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Trigger_OnStayCallback);
#endif

    for ( typeContactVector::const_iterator contactItr = pCurrentContacts->begin(); contactItr != pCurrentContacts->end(); ++contactItr )
    {
        // Fetch colliding object.
        SceneObject* pCollideWidth = contactItr->getCollideWith( this );

        Con::executef(this, 2, "onStay", pCollideWidth->getIdString());
    }

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Trigger_OnStayCallback
#endif
    }

    // Perform "OnLeave" callback.
    if ( mLeaveCallback && mLeaveColliders.size() > 0 )
    {
// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_START(Trigger_OnLeaveCallback);
#endif

    for ( collideCallbackType::iterator contactItr = mLeaveColliders.begin(); contactItr != mLeaveColliders.end(); ++contactItr )
    {
        Con::executef(this, 2, "onLeave", (*contactItr)->getIdString());
    }

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Trigger_OnLeaveCallback
#endif
    }

// Debug Profiling.
#ifdef TORQUE_ENABLE_PROFILER
    PROFILE_END();   // Trigger_IntegrateObject
#endif
}

//-----------------------------------------------------------------------------

void Trigger::onBeginCollision( const TickContact& tickContact )
{
    // Call parent.
    Parent::onBeginCollision( tickContact );

    // Add to enter colliders.
    mEnterColliders.push_back( tickContact.getCollideWith( this ) );
}

//-----------------------------------------------------------------------------

void Trigger::onEndCollision( const TickContact& tickContact )
{
    // Call parent.
    Parent::onEndCollision( tickContact );

    // Add to leave colliders.
    mLeaveColliders.push_back( tickContact.getCollideWith( this ) );
}

//-----------------------------------------------------------------------------

void Trigger::copyTo(SimObject* object)
{
   Parent::copyTo(object);

   AssertFatal(dynamic_cast<Trigger*>(object), "Trigger::copyTo() - Object is not the correct type.");
   Trigger* trigger = static_cast<Trigger*>(object);

   trigger->mEnterCallback = mEnterCallback;
   trigger->mStayCallback = mStayCallback;
   trigger->mLeaveCallback = mLeaveCallback;
}
