//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _VECTOREXT_H
#define _VECTOREXT_H

#ifndef _VECTOR_H_
#include "vector.h"
#endif

//-------------------------------------------------------------------------------------
// Simple wrapper for vector so it works like the Heap class
// adds:
// enqueue( T )	- adds item to back
// dequeue()	- removes item from front
// item()		- gets item from front
//-------------------------------------------------------------------------------------

template <class T>
class Queue : public Vector<T>
{
   using Vector<T>::mElementCount;
   using Vector<T>::pop_front;
   using Vector<T>::front;
   
public:

    //---------------------------------------------------------
    
    Queue()
    {
    }

    //---------------------------------------------------------
    
    void enqueue( T element )
    {
        push_back( element );
    }

    //---------------------------------------------------------
    
    void dequeue()
    {
        if( mElementCount >= 0 )
        {
            pop_front();
        }
    }

    //---------------------------------------------------------
    T& item()
    {
        return front();
    }
};

#endif