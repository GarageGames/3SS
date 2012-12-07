//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "io/stream.h"
#include "string/stringUnit.h"
#include "memory/frameAllocator.h"

#include "component/behaviors/behaviorComponent.h"
#include "component/behaviors/behaviorTemplate.h"

#ifndef _ASSET_FIELD_TYPES_H
#include "assets/assetFieldTypes.h"
#endif

#ifndef _TAML_COLLECTION_H_
#include "persistence/taml/tamlCollection.h"
#endif

// Script bindings.
#include "behaviorComponent_ScriptBinding.h"

//-----------------------------------------------------------------------------

#define BEHAVIOR_FIELDNAME              "Behavior"
#define BEHAVIOR_CONNECTION_FIELDNAME   "BehaviorConnection"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( BehaviorComponent );

//-----------------------------------------------------------------------------

BehaviorComponent::BehaviorComponent() :
    mMasterBehaviorId( 1 ),
    mpBehaviorFieldNames( NULL )
{
    SIMSET_SET_ASSOCIATION( mBehaviors );
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::onAdd()
{
    if( !Parent::onAdd() )
        return false;

    // Read behaviors.
    readBehaviors();
   
    return true;
}

//-----------------------------------------------------------------------------

void BehaviorComponent::onRemove()
{
    // Remove all behaviors and notify.
    clearBehaviors();

    // Call parent.
    Parent::onRemove();
}

//-----------------------------------------------------------------------------

void BehaviorComponent::onDeleteNotify( SimObject *object )
{
    // Cast to a behavior instance.
    BehaviorInstance* pInstance = dynamic_cast<BehaviorInstance*>( object );

    // Ignore if not appropriate.
    if ( pInstance == NULL )
        return;

    // Is the behavior instance owned by this component?
    if ( pInstance->getBehaviorOwner() == this )
    {
        // Yes, so remove.
        removeBehavior( pInstance, false );
    }

    // Destroy any input connections to the instance.
    destroyBehaviorInputConnections( pInstance );
}

//-----------------------------------------------------------------------------

void BehaviorComponent::copyTo(SimObject* obj)
{
    // Call parent.
    Parent::copyTo(obj);

    // Fetch object.
    BehaviorComponent* pObject = dynamic_cast<BehaviorComponent*>(obj);

    // Sanity!
    AssertFatal(pObject != NULL, "BehaviorComponent::copyTo() - Object is not the correct type.");

    // Clear behaviors.
    pObject->clearBehaviors();

    // Behaviors
    U32 behaviorCount = getBehaviorCount();

    // Finish if no behaviors.
    if ( behaviorCount == 0 )
        return;

    // Initialize a clone map.
    typedef HashMap<BehaviorInstance*,BehaviorInstance*> typeBehaviorCloneHash;
    typeBehaviorCloneHash behaviorInstanceCloneMap;

    // Iterate behaviors.
    for ( U32 index = 0; index < behaviorCount; ++index )
    {
        // Clone the behavior instance.
        BehaviorInstance* pFromInstance = getBehavior(index);
        BehaviorTemplate* pFromTemplate = pFromInstance->getTemplate();
        BehaviorInstance* pToInstance = pFromTemplate->createInstance();

        // Assign dynamic fields from behavior instance.
        pToInstance->assignDynamicFieldsFrom( pFromInstance );

        // Add the behavior instance.
        pObject->addBehavior(pToInstance);

        // Add to the clone map.
        behaviorInstanceCloneMap.insert( pFromInstance, pToInstance );
    }

    // Iterate instance connections.
    for( typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.begin(); instanceItr != mBehaviorConnections.end(); ++instanceItr )
    {
        // Fetch output name connection(s).
        typeOutputNameConnectionHash* pOutputNameConnection = instanceItr->value;

        // Iterate output name connections.
        for( typeOutputNameConnectionHash::iterator outputItr = pOutputNameConnection->begin(); outputItr != pOutputNameConnection->end(); ++outputItr )
        {
            // Fetch port connection(s).
            typePortConnectionVector* pPortConnections = outputItr->value;

            // Iterate input connections.
            for( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
            {
                // Fetch connection.
                BehaviorPortConnection* pConnection = connectionItr;

                // Find behavior instance mappings.
                typeBehaviorCloneHash::iterator toOutputItr = behaviorInstanceCloneMap.find( pConnection->mOutputInstance );
                typeBehaviorCloneHash::iterator toInputItr = behaviorInstanceCloneMap.find( pConnection->mInputInstance );

                // Sanity!
                AssertFatal( toOutputItr != behaviorInstanceCloneMap.end(), "Failed to find output behavior instance mapping during copy." );
                AssertFatal( toInputItr != behaviorInstanceCloneMap.end(), "Failed to find input behavior instance mapping during copy." );

                // Fetch behavior instance mappings.
                BehaviorInstance* pToInstanceOutput = toOutputItr->value;
                BehaviorInstance* pToInstanceInput = toInputItr->value;

                // Make cloned connection.
                pObject->connect( pToInstanceOutput, pToInstanceInput, pConnection->mOutputName, pConnection->mInputName );
            }
        }
    }
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::addBehavior( BehaviorInstance* bi )
{
    if( bi == NULL || !bi->isProperlyAdded() )
        return false;

    // Store behavior.
    mBehaviors.pushObject( bi );

    // Notify if the behavior instance is destroyed.
    deleteNotify( bi );

    // Set the behavior owner.
    bi->setBehaviorOwner( this );

    // Allocate a behavior Id.
    bi->setBehaviorId( mMasterBehaviorId++ );

    if( bi->isMethod("onBehaviorAdd") )
        Con::executef( bi , 1, "onBehaviorAdd" );

    return true;
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::removeBehavior( BehaviorInstance *bi, bool deleteBehavior )
{
    for( SimSet::iterator itr = mBehaviors.begin(); itr != mBehaviors.end(); ++itr )
    {
        if( *itr == bi )
        {
            mBehaviors.removeObject( *itr );

            // Perform callback if allowed.
            if( bi->isProperlyAdded() && bi->isMethod("onBehaviorRemove") )
                Con::executef( bi , 1, "onBehaviorRemove" );

            // Destroy any output connections.
            destroyBehaviorOutputConnections( bi );

            if ( deleteBehavior && bi->isProperlyAdded() )
            {
                bi->deleteObject();
            }
            else
            {
                bi->setBehaviorOwner( NULL );
                bi->setBehaviorId( 0 );

                // Remove delete notification.
                clearNotify( bi );
            }

            return true;
        }
    }

    return false;
}

//-----------------------------------------------------------------------------

void BehaviorComponent::clearBehaviors()
{
    while( mBehaviors.size() > 0 )
    {
        BehaviorInstance *bi = dynamic_cast<BehaviorInstance *>( mBehaviors.first() );
        removeBehavior( bi );
    }
}

//-----------------------------------------------------------------------------

BehaviorInstance *BehaviorComponent::getBehavior( StringTableEntry behaviorTemplateName )
{
    for( SimSet::iterator itr = mBehaviors.begin(); itr != mBehaviors.end(); ++itr )
    {
        // Fetch behavior.
        BehaviorInstance* pBehaviorInstance = dynamic_cast<BehaviorInstance*>(*itr);

        if ( !pBehaviorInstance || pBehaviorInstance->getTemplateName() != behaviorTemplateName )
            continue;

        return pBehaviorInstance;
    }

    return NULL;
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::reOrder( BehaviorInstance *obj, U32 desiredIndex /* = 0 */ )
{
    if( desiredIndex > (U32)mBehaviors.size() )
        return false;

    SimObject *target = mBehaviors.at( desiredIndex );
    return mBehaviors.reOrder( obj, target );
}

//-----------------------------------------------------------------------------

void BehaviorComponent::destroyBehaviorOutputConnections( BehaviorInstance* pOutputBehavior )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );

    // Is the output behavior owned by this behavior component?
    if ( pOutputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not destroy output behavior connection for behavior '%s' as the output behavior is not owned by this component.",
            pOutputBehavior->getTemplateName()
            );
        return;
    }

    // Find behavior instance connections.
    typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.find( pOutputBehavior->getId() );

    // Finish if there are no outbound connections for this output behavior.
    if ( instanceItr == mBehaviorConnections.end() )
        return;

    // Fetch output name hash.
    typeOutputNameConnectionHash* pOutputNameHash = instanceItr->value;

    // Iterate all outputs.
    for ( typeOutputNameConnectionHash::iterator outputItr = pOutputNameHash->begin(); outputItr != pOutputNameHash->end(); ++outputItr )
    {
        // Fetch port connection(s).
        typePortConnectionVector* pPortConnections = outputItr->value;

        // Destroy port connections.
        delete pPortConnections;
    }

    // Destroy outputs.
    delete pOutputNameHash;

    // Remove connection.
    mBehaviorConnections.erase( pOutputBehavior->getId() );
}

//-----------------------------------------------------------------------------

void BehaviorComponent::destroyBehaviorInputConnections( BehaviorInstance* pInputBehavior )
{
    // Sanity!
    AssertFatal( pInputBehavior != NULL, "Input behavior cannot be NULL." );

    // Iterate connections.
    for ( typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.begin(); instanceItr != mBehaviorConnections.end(); ++instanceItr )
    {
        // Fetch output name hash.
        typeOutputNameConnectionHash* pOutputNameHash = instanceItr->value;

        // Iterate all outputs.
        for ( typeOutputNameConnectionHash::iterator outputItr = pOutputNameHash->begin(); outputItr != pOutputNameHash->end(); ++outputItr )
        {
            // Fetch port connection(s).
            typePortConnectionVector* pPortConnections = outputItr->value;

            bool connectionFound;
            do
            {
                // Flag connection as 'not found' initially.
                connectionFound = false;

                // Look for an existing connection to the specified input instance.
                for ( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
                {
                    // Is this the input behavior?
                    if ( connectionItr->mInputInstance == pInputBehavior )
                    {
                        // Yes, so destroy it.
                        pPortConnections->erase_fast( connectionItr );

                        // Flag connection as 'found'.
                        connectionFound = true;
                        break;
                    }
                }

            } while (connectionFound);
        }
    }
}

//-----------------------------------------------------------------------------

BehaviorInstance* BehaviorComponent::getBehaviorByInstanceId( const U32 behaviorId )
{
    for( SimSet::iterator instanceItr = mBehaviors.begin(); instanceItr != mBehaviors.end(); ++instanceItr )
    {
        // Fetch behavior instance.
        BehaviorInstance* pInstance = static_cast<BehaviorInstance*>( *instanceItr );

        // Return instance if it has the same behavior Id.
        if ( pInstance->getBehaviorId() == behaviorId )
            return pInstance;
    }

    // Not found.
    return NULL;
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::connect( BehaviorInstance* pOutputBehavior, BehaviorInstance* pInputBehavior, StringTableEntry pOutputName, StringTableEntry pInputName )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
    AssertFatal( pInputBehavior != NULL, "Input behavior cannot be NULL." );
    AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );
    AssertFatal( pInputName != NULL, "Input name cannot be NULL." );

    // Is the output behavior owned by this behavior component?
    if ( pOutputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not connect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the output behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Is the input behavior owned by this behavior component?
    if ( pInputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not connect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the input behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Does the output behavior have the specified output?
    if ( !pOutputBehavior->getTemplate()->hasBehaviorOutput( pOutputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not connect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the output behavior does not have such an output.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Does the input behavior have the specified input?
    if ( !pInputBehavior->getTemplate()->hasBehaviorInput( pInputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not connect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the input behavior does not have such an input.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Find behavior instance connections.
    typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.find( pOutputBehavior->getId() );

    // Are there currently any outbound connections for this output instance?
    if ( instanceItr == mBehaviorConnections.end() )
    {
        // No, so create an entry for this instance.
        typeOutputNameConnectionHash* pOutputHash = new typeOutputNameConnectionHash();

        // Insert new output hash.
        instanceItr = mBehaviorConnections.insert( pOutputBehavior->getId(), pOutputHash );
    }

    // Fetch output name hash.
    typeOutputNameConnectionHash* pOutputNameHash = instanceItr->value;

    // Find instance output connection.
    typeOutputNameConnectionHash::iterator outputItr = pOutputNameHash->find( pOutputName );

    // Are there currently any outbound connections for this specific output?
    if ( outputItr == pOutputNameHash->end() )
    {
        // No, so create an entry for this output.
        typePortConnectionVector* pPortConnections = new typePortConnectionVector();

        // Insert new port connections.
        outputItr = pOutputNameHash->insert( pOutputName, pPortConnections );
    }

    // Fetch port connection(s).
    typePortConnectionVector* pPortConnections = outputItr->value;

    // Look for an identical connection.
    for ( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
    {
        // Is this an identical connection?
        if ( connectionItr->mInputInstance == pInputBehavior && connectionItr->mInputName == pInputName )
        {
            // Yes, so warn.
            Con::warnf(
                "Could not connect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the connection already exists.",
                pOutputName,
                pOutputBehavior->getTemplateName(),
                pInputName,
                pInputBehavior->getTemplateName() 
                );

            return false;
        }
    }
    
    // Populate port connection.
    BehaviorPortConnection portConnection( pOutputBehavior, pInputBehavior, pOutputName, pInputName );

    // Add port connection.
    pPortConnections->push_back( portConnection );

    // Notify if the input behavior instance is destroyed.
    deleteNotify( pInputBehavior );

    return true;
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::disconnect( BehaviorInstance* pOutputBehavior, BehaviorInstance* pInputBehavior, StringTableEntry pOutputName, StringTableEntry pInputName )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
    AssertFatal( pInputBehavior != NULL, "Input behavior cannot be NULL." );
    AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );
    AssertFatal( pInputName != NULL, "Input name cannot be NULL." );

    // Is the output behavior owned by this behavior component?
    if ( pOutputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not disconnect output '%s' on behavior '%s' from input '%s' on behavior '%s' as the output behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Is the input behavior owned by this behavior component?
    if ( pInputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not disconnect output '%s' on behavior '%s' from input '%s' on behavior '%s' as the input behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Does the output behavior have the specified output?
    if ( !pOutputBehavior->getTemplate()->hasBehaviorOutput( pOutputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not disconnect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the output behavior does not have such an output.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Does the input behavior have the specified input?
    if ( !pInputBehavior->getTemplate()->hasBehaviorInput( pInputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not disconnect output '%s' on behavior '%s' to input '%s' on behavior '%s' as the input behavior does not have such an input.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Find behavior instance connections.
    typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.find( pOutputBehavior->getId() );

    // Are there currently any outbound connections for this output instance?
    if ( instanceItr == mBehaviorConnections.end() )
    {
        // No, so warn.
        Con::warnf(
            "Could not disconnect output '%s' on behavior '%s' from input '%s' on behavior '%s' as the behavior does not have any connections.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Fetch output name hash.
    typeOutputNameConnectionHash* pOutputNameHash = instanceItr->value;

    // Find instance output connection.
    typeOutputNameConnectionHash::iterator outputItr = pOutputNameHash->find( pOutputName );

    // Are there currently any outbound connections for this specific output?
    if ( outputItr == pOutputNameHash->end() )
    {
        // No, so warn.
        Con::warnf(
            "Could not disconnect output '%s' on behavior '%s' from input '%s' on behavior '%s' as the specified output does not have any connections.",
            pOutputName,
            pOutputBehavior->getTemplateName(),
            pInputName,
            pInputBehavior->getTemplateName() 
            );
        return false;
    }

    // Fetch port connection(s).
    typePortConnectionVector* pPortConnections = outputItr->value;

    // Look for an existing connection to the specified input.
    for ( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
    {
        // Is this the requested disconnection?
        if ( connectionItr->mInputInstance == pInputBehavior && connectionItr->mInputName == pInputName )
        {
            // Yes, so remove connection.
            pPortConnections->erase_fast( connectionItr );

            return true;
        }
    }

    // Not found so warn.
    Con::warnf(
        "Could not disconnect output '%s' on behavior '%s' from input '%s' on behavior '%s' as the connection does not exist.",
        pOutputName,
        pOutputBehavior->getTemplateName(),
        pInputName,
        pInputBehavior->getTemplateName() 
        );
    
    return false;
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::raise( BehaviorInstance* pOutputBehavior, StringTableEntry pOutputName )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
    AssertFatal( pOutputBehavior->isProperlyAdded(), "Output behavior is not registered." );
    AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );

    // Is the output behavior owned by this behavior component?
    if ( pOutputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not raise output '%s' on behavior '%s' as the output behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName()
            );
        return false;
    }

    // Does the behavior have the specified output?
    if ( !pOutputBehavior->getTemplate()->hasBehaviorOutput( pOutputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not raise output '%s' on behavior '%s' as the behavior does not have such an output.",
            pOutputName,
            pOutputBehavior->getTemplateName()
            );
        return false;
    }

    // Execute a callback for the output.
    // NOTE: This callback should not delete behaviors otherwise strange things can happen!
    Con::executef( this, 2, pOutputName, pOutputBehavior->getIdString() );

    // Find behavior instance connections.
    typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.find( pOutputBehavior->getId() );

    // Finish if there are no outbound connections for this output behavior.
    if ( instanceItr == mBehaviorConnections.end() )
        return true;

    // Find instance output connection.
    typeOutputNameConnectionHash::iterator outputItr = instanceItr->value->find( pOutputName );

    // Finish if there are no outbound connections for this output.
    if ( outputItr == instanceItr->value->end() )
        return true;

    // Fetch port connection(s).
    typePortConnectionVector* pPortConnections = outputItr->value;

    // Process output connection(s).
    for ( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
    {
        // Fetch input behavior.
        BehaviorInstance* pInputBehavior = connectionItr->mInputInstance;

        // Fetch input name.
        StringTableEntry pInputName = connectionItr->mInputName;

#ifdef TORQUE_DEBUG

        // Sanity!
        AssertFatal( pInputBehavior->isProperlyAdded(), "Input behavior is not registered." );

        // Does the behavior have the specified input?
        if ( !pInputBehavior->getTemplate()->hasBehaviorInput( pInputName ) )
        {
            // No, so warn.
            Con::warnf(
                "Could not raise input '%s' on behavior '%s' as the behavior does not have such an input.",
                pInputName,
                pInputBehavior->getTemplateName()
                );
            return false;
        }
#endif
        // Execute a callback for the input.
        // NOTE: This callback should not delete behaviors otherwise strange things can happen!
        Con::executef( pInputBehavior, 3, pInputName, pOutputBehavior->getIdString(), pOutputName );
    }

    return true;
}

//-----------------------------------------------------------------------------

U32 BehaviorComponent::getBehaviorConnectionCount( BehaviorInstance* pOutputBehavior, StringTableEntry pOutputName )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
    AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );

    // Is the output behavior owned by this behavior component?
    if ( pOutputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not get behavior connection count on output '%s' on behavior '%s' as the output behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName()
            );
        return 0;
    }

    // Does the behavior have the specified output?
    if ( !pOutputBehavior->getTemplate()->hasBehaviorOutput( pOutputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not get behavior connection count for output '%s' on behavior '%s' as the behavior does not have such an output.",
            pOutputName,
            pOutputBehavior->getTemplateName()
            );
        return 0;
    }

    // Find behavior instance connections.
    typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.find( pOutputBehavior->getId() );

    // Finish if there are no outbound connections for this output behavior.
    if ( instanceItr == mBehaviorConnections.end() )
        return 0;

    // Find instance output connection.
    typeOutputNameConnectionHash::iterator outputItr = instanceItr->value->find( pOutputName );

    // Finish if there are no outbound connections for this output.
    if ( outputItr == instanceItr->value->end() )
        return 0;

    // Fetch port connection(s).
    typePortConnectionVector* pPortConnections = outputItr->value;

    // Return number of connections.
    return pPortConnections->size();
}

//-----------------------------------------------------------------------------

const BehaviorComponent::BehaviorPortConnection* BehaviorComponent::getBehaviorConnection( BehaviorInstance* pOutputBehavior, StringTableEntry pOutputName, const U32 connectionIndex  )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
    AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );

    // Fetch behavior connection count.
    const U32 behaviorConnectionCount = getBehaviorConnectionCount( pOutputBehavior, pOutputName );

    // Finish if there are no connections.
    if ( behaviorConnectionCount == 0 )
        return NULL;

    // Is the connection index valid?
    if ( connectionIndex >= behaviorConnectionCount )
    {
        // No, so warn.
        Con::warnf(
            "Could not get behavior the behavior connection index '%d' on output '%s' on behavior '%s' as the output behavior only has '%d' connections",
            connectionIndex,
            pOutputName,
            pOutputBehavior->getTemplateName(),
            behaviorConnectionCount
            );
        return NULL;
    }

    // Fetch behavior connections.
    const typePortConnectionVector* pConnections = getBehaviorConnections( pOutputBehavior, pOutputName );

    // Fetch behavior connection.
    const BehaviorComponent::BehaviorPortConnection* pBehaviorConnection = &((*pConnections)[connectionIndex]);

    // Return behavior connection.
    return pBehaviorConnection;
}

//-----------------------------------------------------------------------------

const BehaviorComponent::typePortConnectionVector* BehaviorComponent::getBehaviorConnections( BehaviorInstance* pOutputBehavior, StringTableEntry pOutputName )
{
    // Sanity!
    AssertFatal( pOutputBehavior != NULL, "Output behavior cannot be NULL." );
    AssertFatal( pOutputName != NULL, "Output name cannot be NULL." );

    // Is the output behavior owned by this behavior component?
    if ( pOutputBehavior->getBehaviorOwner() != this )
    {
        // No, so warn.
        Con::warnf(
            "Could not get behavior connections on output '%s' on behavior '%s' as the output behavior is not owned by this component.",
            pOutputName,
            pOutputBehavior->getTemplateName()
            );
        return NULL;
    }

    // Does the behavior have the specified output?
    if ( !pOutputBehavior->getTemplate()->hasBehaviorOutput( pOutputName ) )
    {
        // No, so warn.
        Con::warnf(
            "Could not get behavior connections for output '%s' on behavior '%s' as the behavior does not have such an output.",
            pOutputName,
            pOutputBehavior->getTemplateName()
            );
        return NULL;
    }

    // Find behavior instance connections.
    typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.find( pOutputBehavior->getId() );

    // Finish if there are no outbound connections for this output behavior.
    if ( instanceItr == mBehaviorConnections.end() )
        return NULL;

    // Find instance output connection.
    typeOutputNameConnectionHash::iterator outputItr = instanceItr->value->find( pOutputName );

    // Finish if there are no outbound connections for this output.
    if ( outputItr == instanceItr->value->end() )
        return NULL;

    // Fetch port connection(s).
    typePortConnectionVector* pPortConnections = outputItr->value;

    // Return number of connections.
    return pPortConnections;
}

//-----------------------------------------------------------------------------

void BehaviorComponent::onTamlPreWrite( void )
{
    // Call parent.
    Parent::onTamlPreWrite();

#if 0
    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Sanity!
    AssertFatal( mpBehaviorFieldNames == NULL, "Invalid behavior field name vector" );

    // Fetch behavior count.
    const U32 behaviorCount = mBehaviors.size();

    // Ignore if no behaviors.
    if( behaviorCount == 0 )
        return;

    // Create behavior fields.
    mpBehaviorFieldNames = new Vector<StringTableEntry>( behaviorCount );
    char buffer[4096];

    // Set buffer limits.
    char* pValueBuffer;
    S32 bufferLeft;

    // Reset field index.
    U32 fieldIndex = 0;

    // Iterate behaviors.
    for( SimSet::iterator behaviorItr = mBehaviors.begin(); behaviorItr != mBehaviors.end(); ++behaviorItr )
    {
        // Fetch fieldname.
        StringTableEntry fieldName = StringTable->insert( avar( "%s%d", BEHAVIOR_FIELDNAME, fieldIndex ) );

        // Note field name,
        mpBehaviorFieldNames->push_back( fieldName );

        // Fetch behavior.
        BehaviorInstance* pBehaviorInstance = dynamic_cast<BehaviorInstance*>( *behaviorItr );

        // Reset buffer.
        pValueBuffer = buffer;
        bufferLeft = sizeof( buffer );

        // Format field value.
        U32 used = dSprintf( pValueBuffer, bufferLeft, "%s=%d", pBehaviorInstance->getTemplateName(), pBehaviorInstance->getBehaviorId() );
        pValueBuffer += used;
        bufferLeft -= used;
            
        // Sanity.
        AssertFatal( bufferLeft > 0, "Cannot write behavior as we ran out of buffer." );

        // Fetch template.
        BehaviorTemplate* pBehaviorTemplate = pBehaviorInstance->getTemplate();

        // Fetch field count,
        const U32 behaviorFieldCount = pBehaviorTemplate->getBehaviorFieldCount();

        // Write out the fields which the behavior template knows about.
        for( U32 fieldIndex = 0; fieldIndex < behaviorFieldCount; ++fieldIndex )
        {
            // Fetch field.
            BehaviorTemplate::BehaviorField* pField = pBehaviorTemplate->getBehaviorField( fieldIndex );
                
            // Fetch field value.
            const char* pFieldValue = pBehaviorInstance->getDataField( pField->mName, NULL );

            // If the field holds the same value as the template's default value than it
            // will get initialized by the template, and so it won't be included just
            // to try to keep the object files looking as non-horrible as possible.
            if( dStrcmp( pField->mDefaultValue, pFieldValue ) != 0 )
            {
                // Write out a field/value pair
                used = dSprintf( pValueBuffer, bufferLeft, ",%s=%s", pField->mName, pFieldValue );

                pValueBuffer += used;
                bufferLeft -= used;
            
                // Sanity.
                AssertFatal( bufferLeft > 0, "Cannot write behavior field as we ran out of buffer." );
            }
        }

        // Set field.
        setDataField( fieldName, NULL, buffer ); 

        // Next field.
        fieldIndex++;
    }

    // Reset field index.
    fieldIndex = 0;

    // Iterate instance connections.
    for( typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.begin(); instanceItr != mBehaviorConnections.end(); ++instanceItr )
    {
        // Fetch output name connection(s).
        typeOutputNameConnectionHash* pOutputNameConnection = instanceItr->value;

        // Iterate output name connections.
        for( typeOutputNameConnectionHash::iterator outputItr = pOutputNameConnection->begin(); outputItr != pOutputNameConnection->end(); ++outputItr )
        {
            // Fetch port connection(s).
            typePortConnectionVector* pPortConnections = outputItr->value;

            // Iterate input connections.
            for( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
            {
                // Fetch connection.
                BehaviorPortConnection* pConnection = connectionItr;

                // Fetch fieldname.
                StringTableEntry fieldName = StringTable->insert( avar( "%s%d", BEHAVIOR_CONNECTION_FIELDNAME, fieldIndex ) );

                // Note field name,
                mpBehaviorFieldNames->push_back( fieldName );

                // Set buffer limits.
                pValueBuffer = buffer;
                bufferLeft = sizeof( buffer );

                // Format field value.
                const U32 used = dSprintf( pValueBuffer, bufferLeft, "%s=%d,%s=%d",
                    pConnection->mOutputName,
                    pConnection->mOutputInstance->getBehaviorId(),
                    pConnection->mInputName,
                    pConnection->mInputInstance->getBehaviorId() );

                pValueBuffer += used;
                bufferLeft -= used;

                // Sanity.
                AssertFatal( bufferLeft > 0, "Cannot write behavior connection as we ran out of buffer." );

                // Set field.
                setDataField( fieldName, NULL, buffer ); 

                // Next field.
                fieldIndex++;
            }
        }
    }
#endif
}

//-----------------------------------------------------------------------------

void BehaviorComponent::onTamlPostWrite( void )
{
    // Call parent.
    Parent::onTamlPostWrite();

#if 0
    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Finish if no behavior fields to reset.
    if ( mpBehaviorFieldNames == NULL )
        return;

    // Clear temporary fields.
    for ( Vector<StringTableEntry>::iterator fieldItr = mpBehaviorFieldNames->begin(); fieldItr != mpBehaviorFieldNames->end(); ++fieldItr )
    {
        // Clear field.
        setDataField( *fieldItr, NULL, "" );
    }

    // Remove behavior field name vector.
    delete mpBehaviorFieldNames;
    mpBehaviorFieldNames = NULL;
#endif
}

//-----------------------------------------------------------------------------

void BehaviorComponent::onTamlCustomWrite( TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomWrite( customCollection );

    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Fetch behavior count.
    const U32 behaviorCount = (U32)mBehaviors.size();

    // Finish if no behaviors.
    if( behaviorCount == 0 )
        return;

    // Fetch behavior template asset field type.
    StringTableEntry behaviorTemplateAssetFieldType = StringTable->insert( BEHAVIORTEMPLATE_ASSET_FIELDTYPE );

    // Add behavior property.
    TamlCollectionProperty* pBehaviorProperty = customCollection.addCollectionProperty( BEHAVIOR_COLLECTION_NAME );

    // Iterate behaviors.
    for( SimSet::iterator behaviorItr = mBehaviors.begin(); behaviorItr != mBehaviors.end(); ++behaviorItr )
    {
        // Fetch behavior.
        BehaviorInstance* pBehaviorInstance = dynamic_cast<BehaviorInstance*>( *behaviorItr );

        // Fetch template.
        BehaviorTemplate* pBehaviorTemplate = pBehaviorInstance->getTemplate();

        // Add behavior type alias.
        TamlPropertyTypeAlias* pBehaviorTypeAlias = pBehaviorProperty->addTypeAlias( pBehaviorInstance->getTemplateName() );

        // Add behavior Id field.
        pBehaviorTypeAlias->addPropertyField( BEHAVIOR_ID_FIELD_NAME, pBehaviorInstance->getBehaviorId() );

        // Fetch field count,
        const U32 behaviorFieldCount = pBehaviorTemplate->getBehaviorFieldCount();

        // Write out the fields which the behavior template knows about.
        for( U32 fieldIndex = 0; fieldIndex < behaviorFieldCount; ++fieldIndex )
        {
            // Fetch field.
            BehaviorTemplate::BehaviorField* pBehaviorField = pBehaviorTemplate->getBehaviorField( fieldIndex );
                
            // Set default field type.
            S32 fieldType = -1;

            // Is this an asset field type?
            if ( pBehaviorField != NULL && pBehaviorField->mType == behaviorTemplateAssetFieldType )
            {
                // Yes, so update field type.
                fieldType = TypeAssetId;
            }

            // Fetch field value.
            const char* pFieldValue = pBehaviorInstance->getPrefixedDynamicDataField( pBehaviorField->mName, NULL, fieldType );

            // Add behavior field.
            pBehaviorTypeAlias->addPropertyField( pBehaviorField->mName, pFieldValue );
        }
    }

    // Fetch behavior connection count.
    const U32 behaviorConnectionCount = (U32)mBehaviorConnections.size();

    // Finish if no behavior connections.
    if ( behaviorConnectionCount == 0 )
        return;

    // Add behavior connection property.
    TamlCollectionProperty* pConnectionProperty = customCollection.addCollectionProperty( BEHAVIOR_CONNECTION_COLLECTION_NAME );
    
    // Iterate instance connections.
    for( typeInstanceConnectionHash::iterator instanceItr = mBehaviorConnections.begin(); instanceItr != mBehaviorConnections.end(); ++instanceItr )
    {
        // Fetch output name connection(s).
        typeOutputNameConnectionHash* pOutputNameConnection = instanceItr->value;

        // Iterate output name connections.
        for( typeOutputNameConnectionHash::iterator outputItr = pOutputNameConnection->begin(); outputItr != pOutputNameConnection->end(); ++outputItr )
        {
            // Fetch port connection(s).
            typePortConnectionVector* pPortConnections = outputItr->value;

            // Iterate input connections.
            for( typePortConnectionVector::iterator connectionItr = pPortConnections->begin(); connectionItr != pPortConnections->end(); ++connectionItr )
            {
                // Fetch connection.
                BehaviorPortConnection* pConnection = connectionItr;

                // Add connection type alias.
                TamlPropertyTypeAlias* pConnectionTypeAlias = pConnectionProperty->addTypeAlias( BEHAVIOR_CONNECTION_TYPE_NAME );

                // Add behavior field.
                pConnectionTypeAlias->addPropertyField( pConnection->mOutputName, pConnection->mOutputInstance->getBehaviorId() );
                pConnectionTypeAlias->addPropertyField( pConnection->mInputName, pConnection->mInputInstance->getBehaviorId() );
            }
        }
    }
}

//-----------------------------------------------------------------------------

void BehaviorComponent::onTamlCustomRead( const TamlCollection& customCollection )
{
    // Call parent.
    Parent::onTamlCustomRead( customCollection );

    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Find behavior collection name.
    const TamlCollectionProperty* pCollectionProperty = customCollection.findProperty( BEHAVIOR_COLLECTION_NAME );

    // Do we have the property?
    if ( pCollectionProperty != NULL )
    {
        // Yes, so reset maximum behavior Id.
        S32 maximumBehaviorId = 0;

        // Fetch behavior Id field name.
        StringTableEntry behaviorFieldIdName = StringTable->insert( BEHAVIOR_ID_FIELD_NAME );

        // Fetch behavior template asset field type.
        StringTableEntry behaviorTemplateAssetFieldType = StringTable->insert( BEHAVIORTEMPLATE_ASSET_FIELDTYPE );

        // Iterate property type alias.
        for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pCollectionProperty->begin(); propertyTypeAliasItr != pCollectionProperty->end(); ++propertyTypeAliasItr )
        {
            // Fetch property type alias.
            TamlPropertyTypeAlias* pPropertyTypeAlias = *propertyTypeAliasItr;

            // Fetch template.
            BehaviorTemplate* pTemplate = dynamic_cast<BehaviorTemplate *>( Sim::findObject( pPropertyTypeAlias->mAliasName ) );

            // Find template?
            if( pTemplate == NULL )
            {
                // No, so warn appropriately.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Missing Behavior '%s'", pPropertyTypeAlias->mAliasName );

                if( isMethod( "onBehaviorMissing" ) )
                    Con::executef( this, 2, "onBehaviorMissing", pPropertyTypeAlias->mAliasName );

                // Skip it.
                continue;
            }

            // Create an instance of the template.
            BehaviorInstance* pBehaviorInstance = pTemplate->createInstance();

            // Did we create a behavior instance?
            if ( pBehaviorInstance == NULL )
            {
                // No, so warn appropriately.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Found behavior could not create an instance '%s'", pPropertyTypeAlias->mAliasName );

                if( isMethod( "onBehaviorMissing" ) )
                    Con::executef( this, 2, "onBehaviorMissing", pPropertyTypeAlias->mAliasName );

                // Skip it.
                continue;
            }

            S32 behaviorId = 0;

            // Iterate property fields.
            for ( TamlPropertyTypeAlias::const_iterator propertyFieldItr = pPropertyTypeAlias->begin(); propertyFieldItr != pPropertyTypeAlias->end(); ++propertyFieldItr )
            {
                // Fetch property field.
                TamlPropertyField* pPropertyField = *propertyFieldItr;

                // Fetch field name.
                const char* pFieldName = pPropertyField->getFieldName();

                // Fetch field value.
                const char* pFieldValue = pPropertyField->getFieldValue();

                // Is this the behavior field Id name?
                if ( pFieldName == behaviorFieldIdName )
                {
                    // Yes, so assign it.
                    behaviorId = dAtoi( pFieldValue );

                    // Is the behavior Id valid?
                    if ( behaviorId < 1 )
                    {
                        // No, so warn.
                        Con::warnf( "BehaviorComponent::onTamlCustomRead() - Encountered an invalid behavior Id of '%d' on behavior '%s'.",
                            behaviorId,
                            pPropertyTypeAlias->mAliasName );
                    }

                    // Update maximum behavior Id found.
                    if ( behaviorId > maximumBehaviorId )
                        maximumBehaviorId = behaviorId;

                    /// Skip it.
                    continue;
                }

                // Fetch behavior field.
                BehaviorTemplate::BehaviorField* pBehaviorField = pTemplate->getBehaviorField( pFieldName );

                // Set default field type.
                S32 fieldType = -1;

                // Is this an asset field type?
                if ( pBehaviorField != NULL && pBehaviorField->mType == behaviorTemplateAssetFieldType )
                {
                    // Yes, so update field type.
                    fieldType = TypeAssetId;
                }

                // Set field.
                pBehaviorInstance->setPrefixedDynamicDataField( pPropertyField->getFieldName(), NULL, pPropertyField->getFieldValue(), fieldType );
            }

            // Add behavior.
            addBehavior( pBehaviorInstance );

            // Override the automatically allocated behavior Id when adding the behavior.
            // NOTE: This must be done after adding the behavior.
            pBehaviorInstance->setBehaviorId( behaviorId );
        }

        // Set master as next behavior id.
        mMasterBehaviorId = (U32)maximumBehaviorId+1;
    }

    // Find behavior connections collection name.
    const TamlCollectionProperty* pConnectionProperty = customCollection.findProperty( BEHAVIOR_CONNECTION_COLLECTION_NAME );

    // Do we have the property?
    if ( pConnectionProperty != NULL )
    {
        // Yes, so insert connection type alias.
        StringTableEntry connectionTypeAlias = StringTable->insert( BEHAVIOR_CONNECTION_TYPE_NAME );

        // Iterate property type alias.
        for( TamlCollectionProperty::const_iterator propertyTypeAliasItr = pConnectionProperty->begin(); propertyTypeAliasItr != pConnectionProperty->end(); ++propertyTypeAliasItr )
        {
            // Fetch property type alias.
            TamlPropertyTypeAlias* pPropertyTypeAlias = *propertyTypeAliasItr;

            // Skip if the type alias isn't a connection.
            if ( pPropertyTypeAlias->mAliasName != connectionTypeAlias )
                continue;

            // Are there two properties?
            if ( pPropertyTypeAlias->size() != 2 )
            {
                // No, so warn.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Encountered a behavior connection with more than two connection fields." );
                continue;
            }

            // Fetch property field #1.
            TamlPropertyField* pPropertyField1 = *pPropertyTypeAlias->begin();
            TamlPropertyField* pPropertyField2 = *(pPropertyTypeAlias->begin()+1);
           
            // Fetch behavior instances #1.
            BehaviorInstance* pBehaviorInstance1 = getBehaviorByInstanceId( dAtoi( pPropertyField1->getFieldValue() ) );

            // Did we find the behavior?
            if ( pBehaviorInstance1 == NULL )
            {
                // No, so warn.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Could not find a behavior instance Id '%s=%s'.",
                    pPropertyField1->getFieldName(), pPropertyField1->getFieldValue() );
                continue;
            }

            // Fetch behavior instances #2.
            BehaviorInstance* pBehaviorInstance2 = getBehaviorByInstanceId( dAtoi( pPropertyField2->getFieldValue() ) );

            // Did we find the behavior?
            if ( pBehaviorInstance2 == NULL )
            {
                // No, so warn.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Could not find a behavior instance Id '%s=%s'.",
                    pPropertyField2->getFieldName(), pPropertyField2->getFieldValue() );
                continue;
            }

            // Fetch behavior fields.
            StringTableEntry behaviorFieldName1 = pPropertyField1->getFieldName();
            StringTableEntry behaviorFieldName2 = pPropertyField2->getFieldName();

            BehaviorInstance* pOutputInstance;
            BehaviorInstance* pInputInstance;
            StringTableEntry outputName;
            StringTableEntry inputName;

            // Is the output on behavior instance #1?
            if ( pBehaviorInstance1->getTemplate()->hasBehaviorOutput( behaviorFieldName1 ) )
            {
                // Yes, so has behavior instance #2 got the input?
                if ( !pBehaviorInstance2->getTemplate()->hasBehaviorInput( behaviorFieldName2 ) )
                {
                    // No, so warn.
                    Con::warnf( "BehaviorComponent::onTamlCustomRead() - Could not find a behavior input '%s' on behavior '%s'.",
                        behaviorFieldName2, pBehaviorInstance2->getTemplateName() );
                    continue;
                }

                // Assign output/input appropriately.
                pOutputInstance = pBehaviorInstance1;
                pInputInstance = pBehaviorInstance2;
                outputName = behaviorFieldName1;
                inputName = behaviorFieldName2;
            }
            // Is the output on behavior instance #2?
            else if ( pBehaviorInstance2->getTemplate()->hasBehaviorOutput( behaviorFieldName2 ) )
            {
                // Yes, so has behavior instance #1 got the input?
                if ( !pBehaviorInstance1->getTemplate()->hasBehaviorInput( behaviorFieldName1 ) )
                {
                    // No, so warn.
                    Con::warnf( "BehaviorComponent::onTamlCustomRead() - Could not find a behavior input '%s' on behavior '%s'.",
                        behaviorFieldName1, pBehaviorInstance1->getTemplateName() );
                    continue;
                }

                // Assign output/input appropriately.
                pOutputInstance = pBehaviorInstance2;
                pInputInstance = pBehaviorInstance1;
                outputName = behaviorFieldName2;
                inputName = behaviorFieldName1;
            }
            else
            {
                // Warn.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Invalid output/input on behavior connection '%s=%s' & '%s=%s''.",
                    behaviorFieldName1, pBehaviorInstance1->getTemplateName(),
                    behaviorFieldName2, pBehaviorInstance2->getTemplateName() );
                continue;
            }

            // Can we connect?
            if ( !connect( pOutputInstance, pInputInstance, outputName, inputName ) )
            {
                // No, so warn.
                Con::warnf( "BehaviorComponent::onTamlCustomRead() - Failed to connect behaviors '%s=%s' & '%s=%s''.",
                    behaviorFieldName1, pBehaviorInstance1->getTemplateName(),
                    behaviorFieldName2, pBehaviorInstance2->getTemplateName() );
                continue;
            }
        }
    }
}

//-----------------------------------------------------------------------------

void BehaviorComponent::write( Stream &stream, U32 tabStop, U32 flags /* = 0 */ )
{
    // Export selected only?
    if( ( flags & SelectedOnly ) && !isSelected() )
    {
        return;
    }

    if( mBehaviors.size() == 0 )
    {
        Parent::write( stream, tabStop, flags );
        return;
    }

    // The work we want to perform here is in the Taml callback.
    onTamlPreWrite();

    // Write object.
    Parent::write( stream, tabStop, flags );

    // The work we want to perform here is in the Taml callback.
    onTamlPostWrite();
}

//-----------------------------------------------------------------------------

void BehaviorComponent::readBehaviors()
{
    // Finish if we have a prefab assigned.
    if ( hasPrefab() )
        return;

    // Sanity!
    AssertFatal( mBehaviors.size() == 0, "BehaviorComponent::readBehaviors() already called!" );

    char fieldValue[4096];
    char slotUnit[256];
    char slotName[256];
    char slotValue[256];

    // Reset maximum behavior Id.
    U32 maximumBehaviorId = 0;

    // Iterate all behaviors.
    for ( U32 behaviorIndex = 0;; ++behaviorIndex )
    {
        // Fetch fieldname.
        StringTableEntry pFieldName = StringTable->insert( avar( "%s%d", BEHAVIOR_FIELDNAME, behaviorIndex ) );

        // Fetch field value.
        const U32 valueLength = dSprintf( fieldValue, 4096, "%s", getDataField( pFieldName, NULL ) );

        // Finish if no field exists.
        // Note: A skip in contiguous indexes will result in subsequent behaviors being ignored.
        if ( valueLength == 0 )
            break;

        // Reset field.
        setDataField( pFieldName, NULL, StringTable->EmptyString );

        // Fetch slot.
        dStrcpy( slotUnit, StringUnit::getUnit( fieldValue, 0, "," ) );
        
        // Fetch slot name and value.
        dStrcpy( slotName, StringUnit::getUnit( slotUnit, 0, "=" ) );
        dStrcpy( slotValue, StringUnit::getUnit( slotUnit, 1, "=" ) );

        // Fetch template name.
        StringTableEntry pTemplateName = StringTable->insert( slotName );

        // Fetch behavior Id.
        const U32 behaviorId = dAtoi( slotValue );

        // Valid behavior id?
        if ( behaviorId == 0 )
        {
            Con::warnf("BehaviorComponent::readBehaviors() - Found invalid behavior Id of '%d' for behavior '%s'", slotValue, pTemplateName );
            continue;
        }

        // Fetch template.
        BehaviorTemplate* pTemplate = dynamic_cast<BehaviorTemplate *>( Sim::findObject( pTemplateName ) );

        // Find template?
        if( pTemplate == NULL )
        {
            // No, so warn appropriately.
            Con::warnf("BehaviorComponent::readBehaviors() - Missing Behavior '%s'", pTemplateName );

            if( isMethod( "onBehaviorMissing" ) )
                Con::executef( this, 2, "onBehaviorMissing", pTemplateName );

            // Skip it.
            continue;
        }

        // Update maximum behavior Id found.
        if ( behaviorId > maximumBehaviorId )
            maximumBehaviorId = behaviorId;

        // Create an instance of the template.
        BehaviorInstance* pBehaviorInstance = pTemplate->createInstance();

        // Skip if behavior was not created.
        if ( pBehaviorInstance == NULL )
            break;

        // Fetch field word count.
        const U32 fieldWordCount = StringUnit::getUnitCount( fieldValue, "," );

        // Set the fields that have been written out.
        for ( U32 fieldIndex = 1; fieldIndex < fieldWordCount; ++fieldIndex )
        {
            // Fetch slot.
            dStrcpy( slotUnit, StringUnit::getUnit( fieldValue, fieldIndex, "," ) );
        
            // Fetch slot name and value.
            dStrcpy( slotName, StringUnit::getUnit( slotUnit, 0, "=" ) );
            dStrcpy( slotValue, StringUnit::getUnit( slotUnit, 1, "=" ) );

            // Fetch slot name.
            StringTableEntry pSlotName = StringTable->insert( slotName );

            // Set field.
            pBehaviorInstance->setDataField( pSlotName, NULL, slotValue );
        }

        // Add behavior.
        addBehavior( pBehaviorInstance );

        // Override the automatically allocated behavior Id when adding the behavior.
        // NOTE: This must be done after adding the behavior.
        pBehaviorInstance->setBehaviorId( behaviorId );
    }

    // Set master as next behavior id.
    mMasterBehaviorId = maximumBehaviorId+1;

    // Iterate all behavior connections.
    for ( U32 connectionIndex = 0;; ++connectionIndex )
    {
        // Fetch fieldname.
        StringTableEntry pFieldName = StringTable->insert( avar( "%s%d", BEHAVIOR_CONNECTION_FIELDNAME, connectionIndex ) );

        // Fetch field value.
        const U32 valueLength = dSprintf( fieldValue, 4096, "%s", getDataField( pFieldName, NULL ) );

        // Finish if no field exists.
        // Note: A skip in contiguous indexes will result in subsequent behavior connections being ignored.
        if ( valueLength == 0 )
            break;

        // Reset field.
        setDataField( pFieldName, NULL, StringTable->EmptyString );

        // Fetch slot #0.
        dStrcpy( slotUnit, StringUnit::getUnit( fieldValue, 0, "," ) );
        
        // Fetch slot name and value.
        dStrcpy( slotName, StringUnit::getUnit( slotUnit, 0, "=" ) );
        dStrcpy( slotValue, StringUnit::getUnit( slotUnit, 1, "=" ) );

        // Fetch output behavior.
        BehaviorInstance* pOutputInstance = getBehaviorByInstanceId( dAtoi( slotValue ) );

        // Was the output instance found?
        if ( !pOutputInstance )
        {
            // No, so warn.
            Con::warnf( "BehaviorComponent::readBehaviors() - Could not find behavior output Id for behavior connection '%s'.", slotUnit );
            continue;
        }

        // Fetch output name.
        StringTableEntry pOutputName = StringTable->insert( slotName );

        // Fetch slot #1.
        dStrcpy( slotUnit, StringUnit::getUnit( fieldValue, 1, "," ) );
        
        // Fetch slot name and value.
        dStrcpy( slotName, StringUnit::getUnit( slotUnit, 0, "=" ) );
        dStrcpy( slotValue, StringUnit::getUnit( slotUnit, 1, "=" ) );

        // Fetch input behavior.
        BehaviorInstance* pInputInstance = getBehaviorByInstanceId( dAtoi( slotValue ) );

        // Was the input instance found?
        if ( !pInputInstance )
        {
            // No, so warn.
            Con::warnf( "BehaviorComponent::readBehaviors() - Could not find behavior input Id for behavior connection '%s'.", slotUnit );
            continue;
        }

        // Fetch input name.
        StringTableEntry pInputName = StringTable->insert( slotName );

        // Connect behaviors.
        connect( pOutputInstance, pInputInstance, pOutputName, pInputName );
    }
}

//-----------------------------------------------------------------------------

bool BehaviorComponent::handlesConsoleMethod( const char *fname, S32 *routingId )
{

   // CodeReview [6/25/2007 justind]
   // If we're deleting the BehaviorComponent, don't forward the call to the
   // behaviors, the parent onRemove will handle freeing them
   // This should really be handled better, and is in the Parent implementation
   // but behaviors are a special case because they always want to be called BEFORE
   // the parent to act.
   if( dStricmp( fname, "delete" ) == 0 )
      return Parent::handlesConsoleMethod( fname, routingId );

   if( dStricmp( fname, "setPrefab" ) == 0 )
      return Parent::handlesConsoleMethod( fname, routingId );

   for( SimSet::iterator nItr = mBehaviors.begin(); nItr != mBehaviors.end(); nItr++ )
   {
      SimObject *pComponent = dynamic_cast<SimObject *>(*nItr);
      if( pComponent != NULL && pComponent->isMethod( fname ) )
      {
         *routingId = -2; // -2 denotes method on component
         return true;
      }
   }

   // Let parent handle it
   return Parent::handlesConsoleMethod( fname, routingId );
}

//-----------------------------------------------------------------------------

// Needed to be able to directly call execute on a Namespace::Entry
extern ExprEvalState gEvalState;

const char *BehaviorComponent::callOnBehaviors( U32 argc, const char *argv[] )
{   
    if( mBehaviors.empty() )   
        return Parent::callOnBehaviors( argc, argv );
   
    const char *cbName = StringTable->insert(argv[0]);
   
    // Copy the arguments to avoid weird clobbery situations.
    FrameTemp<char *> argPtrs (argc);
   
    U32 strdupWatermark = FrameAllocator::getWaterMark();
    for( U32 i = 0; i < argc; i++ )
    {
        argPtrs[i] = reinterpret_cast<char *>( FrameAllocator::alloc( dStrlen( argv[i] ) + 1 ) );
        dStrcpy( argPtrs[i], argv[i] );
    }

    // Walk backwards through the list just as with components
    const char* result = "";
    bool handled = false;
    for( SimSet::iterator i = (mBehaviors.end()-1); i >= mBehaviors.begin(); i-- )
    {
        BehaviorInstance *pBehavior = dynamic_cast<BehaviorInstance *>( *i );
        AssertFatal( pBehavior, "BehaviorComponent::callOnBehaviors - Bad behavior instance in list." );
        AssertFatal( pBehavior->getId() > 0, "Invalid id for behavior component" );

        // Use the BehaviorInstance's namespace
        Namespace *pNamespace = pBehavior->getNamespace();
        if(!pNamespace)
            continue;

        // Lookup the Callback Namespace entry and then splice callback
        Namespace::Entry *pNSEntry = pNamespace->lookup(cbName);
        if( pNSEntry )
        {
            // Set %this to our BehaviorInstance's Object ID
            argPtrs[1] = const_cast<char *>( pBehavior->getIdString() );

            // Change the Current Console object, execute, restore Object
            SimObject *save = gEvalState.thisObject;
            gEvalState.thisObject = pBehavior;

            result = pNSEntry->execute(argc, const_cast<const char **>( ~argPtrs ), &gEvalState);

            gEvalState.thisObject = save;
            handled = true;
            break;
        }
    }

    // If this wasn't handled by a behavior above then pass along to the parent DynamicConsoleMethodComponent
    // to deal with it.  If the parent cannot handle the message it will return an error string.
    if (!handled)
    {
        result = Parent::callOnBehaviors( argc, argv );
    }

    // Clean up.
    FrameAllocator::setWaterMark( strdupWatermark );

    return result;
}

//-----------------------------------------------------------------------------

const char *BehaviorComponent::_callMethod( U32 argc, const char *argv[], bool callThis /* = true  */ )
{   
    if( mBehaviors.empty() )   
        return Parent::_callMethod( argc, argv, callThis );
   
    const char *cbName = StringTable->insert(argv[0]);
   
    // Copy the arguments to avoid weird clobbery situations.
    FrameTemp<char *> argPtrs (argc);
   
    U32 strdupWatermark = FrameAllocator::getWaterMark();
    for( U32 i = 0; i < argc; i++ )
    {
        argPtrs[i] = reinterpret_cast<char *>( FrameAllocator::alloc( dStrlen( argv[i] ) + 1 ) );
        dStrcpy( argPtrs[i], argv[i] );
    }

    for( SimSet::iterator i = mBehaviors.begin(); i != mBehaviors.end(); i++ )
    {
        BehaviorInstance *pBehavior = dynamic_cast<BehaviorInstance *>( *i );
        AssertFatal( pBehavior, "BehaviorComponent::_callMethod - Bad behavior instance in list." );
        AssertFatal( pBehavior->getId() > 0, "Invalid id for behavior component" );

        // Use the BehaviorInstance's namespace
        Namespace *pNamespace = pBehavior->getNamespace();
        if(!pNamespace)
            continue;

        // Lookup the Callback Namespace entry and then splice callback
        Namespace::Entry *pNSEntry = pNamespace->lookup(cbName);
        if( pNSEntry )
        {
            // Set %this to our BehaviorInstance's Object ID
            argPtrs[1] = const_cast<char *>( pBehavior->getIdString() );

            // Change the Current Console object, execute, restore Object
            SimObject *save = gEvalState.thisObject;
            gEvalState.thisObject = pBehavior;

            pNSEntry->execute(argc, const_cast<const char **>( ~argPtrs ), &gEvalState);

            gEvalState.thisObject = save;
        }
    }

    // Pass this up to the parent since a BehaviorComponent is still a DynamicConsoleMethodComponent
    // it needs to be able to contain other components and behave properly
    const char* fnRet = Parent::_callMethod( argc, argv, callThis );

    // Clean up.
    FrameAllocator::setWaterMark( strdupWatermark );

    return fnRet;
}
