//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "component/behaviors/behaviorInstance.h"
#include "component/behaviors/behaviorTemplate.h"
#include "console/consoleTypes.h"
#include "console/consoleInternal.h"
#include "io/stream.h"

// Script bindings.
#include "behaviorInstance_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(BehaviorInstance);

//-----------------------------------------------------------------------------

BehaviorInstance::BehaviorInstance( BehaviorTemplate* pTemplate ) :
    mTemplate( pTemplate ),
    mBehaviorOwner( NULL ),
    mBehaviorId( 0 )
{
    if ( pTemplate != NULL )
    {
        // Fetch field prototype count.
        const U32 fieldCount = pTemplate->getBehaviorFieldCount();

        // Set field prototypes.
        for( U32 index = 0; index < fieldCount; ++index )
        {        
            // Fetch fields.
            BehaviorTemplate::BehaviorField* pField = pTemplate->getBehaviorField( index );

            // Set cloned field.
            setDataField( pField->mName, NULL, pField->mDefaultValue );
        }
    }
}

//-----------------------------------------------------------------------------

bool BehaviorInstance::onAdd()
{
   if(! Parent::onAdd())
      return false;

   // Store this object's namespace
   mNameSpace = Namespace::global()->find( getTemplateName() );

   return true;
}

//-----------------------------------------------------------------------------

void BehaviorInstance::onRemove()
{
   Parent::onRemove();
}

//-----------------------------------------------------------------------------

void BehaviorInstance::initPersistFields()
{
   addGroup("Behavior");
      addField("template", TypeSimObjectName, Offset(mTemplate, BehaviorInstance), "Template this instance was created from.");
      addProtectedField( "Owner", TypeSimObjectPtr, Offset(mBehaviorOwner, BehaviorInstance), &setOwner, &defaultProtectedGetFn, "Behavior component owner." );
      addProtectedField( "Template", TypeSimObjectPtr,0, &defaultProtectedNotSetFn, &getTemplate, "The behavior instances template." );
   endGroup("Behavior");

   Parent::initPersistFields();
}

//-----------------------------------------------------------------------------

const char* BehaviorInstance::getTemplateName( void )
{
    return mTemplate ? mTemplate->getName() : NULL;
}

// Get template.
const char* BehaviorInstance::getTemplate(void* obj, const char* data)
{
    return static_cast<BehaviorInstance*>(obj)->getTemplate()->getIdString();
}
