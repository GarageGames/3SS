//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef _BEHAVIORINSTANCE_H_
#define _BEHAVIORINSTANCE_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _HASHTABLE_H
#include "collection/hashTable.h"
#endif

#ifndef _SIMCOMPONENT_H_
#include "component/simComponent.h"
#endif

//-----------------------------------------------------------------------------

class BehaviorTemplate;
class BehaviorComponent;

//-----------------------------------------------------------------------------

class BehaviorInstance : public SimObject
{
    typedef SimObject Parent;

public:
    BehaviorInstance( BehaviorTemplate* pTemplate = NULL );
    virtual ~BehaviorInstance() {}

    virtual bool onAdd();
    virtual void onRemove();
    static void initPersistFields();

    /// Template name.
    inline BehaviorTemplate* getTemplate( void ) { return mTemplate; }
    const char* getTemplateName( void );

    /// Owner.
    inline void setBehaviorOwner( BehaviorComponent* pOwner ) { mBehaviorOwner = pOwner; }
    inline BehaviorComponent* getBehaviorOwner( void ) const { return mBehaviorOwner ? mBehaviorOwner : NULL; }

    /// Behavior Id.
    inline void setBehaviorId( const U32 id ) { mBehaviorId = id; }
    inline U32 getBehaviorId( void ) const { return mBehaviorId; }

    DECLARE_CONOBJECT(BehaviorInstance);

protected:
    BehaviorTemplate*   mTemplate;
    BehaviorComponent*  mBehaviorOwner;
    U32                 mBehaviorId;

    // Set "Owner" via the field does nothing.
    static bool setOwner( void* obj, const char* data ) { return true; }

    // Get template.
    static const char* getTemplate(void* obj, const char* data);
};

#endif // _BEHAVIORINSTANCE_H_
