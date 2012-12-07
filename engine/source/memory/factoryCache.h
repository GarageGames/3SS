//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _FACTORY_CACHE_H_
#define _FACTORY_CACHE_H_

#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif

//-----------------------------------------------------------------------------

class IFactoryObjectReset
{
public:
    virtual void resetState( void ) = 0;
};

//-----------------------------------------------------------------------------

template<class T>
class FactoryCache : private Vector<T*>
{
public:
    FactoryCache()
    {
    }

    virtual ~FactoryCache()
    {
        purgeCache();
    }

    T* createObject( void )
    {
        // Create a new object if cache is empty.
        if ( this->size() == 0 )
            return new T();

        // Return a cached object.
        T* pObject = this->back();
        this->pop_back();
        return pObject;
    }

    void cacheObject( T* pObject )
    {
        // Cache object.
        this->push_back( pObject );

        // Reset object state if available.
        IFactoryObjectReset* pResetStateObject = dynamic_cast<IFactoryObjectReset*>( pObject );
        if ( pResetStateObject != NULL )
            pResetStateObject->resetState();
    }

    void purgeCache( void )
    {
        while( this->size() > 0 )
        {
            delete this->back();
            this->pop_back();
        }
    }
};

#endif // _FACTORY_CACHE_H_