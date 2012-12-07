//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TAML_WRITE_NODE_H_
#define _TAML_WRITE_NODE_H_

#ifndef _TAML_COLLECTION_H_
#include "tamlCollection.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

//-----------------------------------------------------------------------------

class TamlCallbacks;

//-----------------------------------------------------------------------------

class TamlWriteNode
{
public:
    class FieldValuePair
    {
    public:        
        FieldValuePair( StringTableEntry name, const char* pValue )
        {
            // Set the field name.
            mName = name;

            // Allocate and copy the value.
            mpValue = new char[ dStrlen(pValue)+1 ];
            dStrcpy( (char *)mpValue, pValue );
        }
        

        StringTableEntry    mName;
        const char*         mpValue;
    };

public:
    TamlWriteNode()
    {
        // NOTE: This MUST be done before the state is reset otherwise we'll be touching uninitialized stuff.
        mRefToNode = NULL;
        mRefField = StringTable->EmptyString;
        mChildren = NULL;
        mpSimObject = NULL;
        mpTamlCallbacks = NULL;
        mpObjectName = NULL;
        mChildren = NULL;

        resetNode();
    }

    void set( SimObject* pSimObject )
    {
        // Sanity!
        AssertFatal( pSimObject != NULL, "Cannot set a write node with a NULL sim object." );

        // Reset the node.
        resetNode();

        // Set sim object.
        mpSimObject = pSimObject;

        // Fetch name.
        const char* pObjectName = pSimObject->getName();

        // Assign name usage.
        if ( pObjectName != NULL &&
            pObjectName != StringTable->EmptyString &&
            dStrlen( pObjectName ) > 0 )
        {
            mpObjectName = pObjectName;
        }

        // Find Taml callbacks.
        mpTamlCallbacks = dynamic_cast<TamlCallbacks*>( mpSimObject );
    }

    void resetNode( void );

    U32                         mRefId;
    TamlWriteNode*              mRefToNode;
    StringTableEntry            mRefField;
    SimObject*                  mpSimObject;
    TamlCallbacks*              mpTamlCallbacks;
    const char*                 mpObjectName;
    Vector<TamlWriteNode::FieldValuePair*> mFields;
    Vector<TamlWriteNode*>*     mChildren;
    TamlCollection              mCustomCollection;
};

#endif // _TAML_WRITE_NODE_H_