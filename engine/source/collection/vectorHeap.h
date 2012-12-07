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
// Based on demo Heap class by Ron Penton
//-------------------------------------------------------------------------------------

template <class T>
class Heap : public Vector<T>
{
   using Vector<T>::increment;
   using Vector<T>::decrement;
   using Vector<T>::back;
   using Vector<T>::mElementCount;
   using Vector<T>::mArray;
   
public:

    //---------------------------------------------------------
    
    Heap( U32 size, S32 ( *p_compare )( T, T ) )
        : Vector< T >( size + 1 )
    {
        m_compare = p_compare;
    }

    //---------------------------------------------------------
    
    void enqueue( T element )
    {
        increment( 1 );
        back() = element;
        walk_up( mElementCount );
    }

    //---------------------------------------------------------

    void dequeue()
    {
        if( mElementCount >= 1 )
        {
            mArray[1] = mArray[mElementCount]; // swap back to front
            walk_down( 1 );
            decrement( 1 );
        }
    }

    //---------------------------------------------------------

    T& item()
    {
        return mArray[1];
    }

    //---------------------------------------------------------

    void walk_up( U32 index )
    {
        // set up the parent and child indexes
        U32 parent = index / 2;
        U32 child = index;

        // store the item to walk up in temp buffer
        T temp = mArray[child];

        while( parent > 0 )
        {	// if the node to walk up is more than the parent, then swap
            // UNUSED: DAVID WYAND -> Node tempParent = mArray[parent];
            if( m_compare( temp, mArray[parent] ) > 0 )
            {
                // swap the parent and child, and go up a level
                mArray[child] = mArray[parent];
                child = parent;
                parent /= 2;
            }
            else 
            {
                // UNUSED: JOSEPH THOMAS -> int foo2 = 0;
                break;
            }
        }

        // put the temp variable (the one that was walked up) into the child index
        mArray[child] = temp;
    }

    //---------------------------------------------------------

    void walk_down( U32 index )
    {
        // calculate the parent and child indexes
        U32 parent = index;
        U32 child = index * 2;

        // store the data to walk down in a temp buffer
        T temp = mArray[parent];

        // loop through, walking node down the heap until both children are smaller than the node
        while( child < mElementCount )
        {
            // if left child is not the last node in the tree, then
            // find out which of the current node's children is largest
            if( child < mElementCount - 1 )
            {
                if( m_compare( mArray[child], mArray[child + 1] ) < 0 )
                { // change the pointer to the right child, since it is larger
                    child++;
                }
            }
            // if the node to walk down is lower than the highest value child.
            // move the child up one level
            if( m_compare( temp, mArray[child] ) < 0 )
            {
                mArray[parent] = mArray[child];
                parent = child;
                child *= 2;
            }
            else
                break;
        }
        mArray[parent] = temp;
    }

    //---------------------------------------------------------

    S32 ( *m_compare )( T, T );

};

#endif