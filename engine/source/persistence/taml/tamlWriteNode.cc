//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlWriteNode.h"

#ifndef _TAML_COLLECTION_H_
#include "tamlCollection.h"
#endif

//-----------------------------------------------------------------------------

void TamlWriteNode::resetNode( void )
{
    // Clear fields.
    for( Vector<TamlWriteNode::FieldValuePair*>::iterator itr = mFields.begin(); itr != mFields.end(); ++itr )
    {
        delete (*itr)->mpValue;
    }
    mFields.clear();

    // Clear children.
    if ( mChildren != NULL )
    {
        for( Vector<TamlWriteNode*>::iterator itr = mChildren->begin(); itr != mChildren->end(); ++itr )
        {
            (*itr)->resetNode();
        }

        mChildren->clear();
        delete mChildren;
        mChildren = NULL;
    }

    mRefId = 0;
    mRefToNode = NULL;
    mChildren = NULL;
    mpObjectName = NULL;
    mpSimObject = NULL;

    // Reset callbacks.
    mpTamlCallbacks = NULL;

    // Reset custom collection.
    mCustomCollection.resetState();
}