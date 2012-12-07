//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _VECTOR2D_H
#define _VECTOR2D_H

#ifndef _VECTOR_H_
#include "vector.h"
#endif

//-------------------------------------------------------------------------------------
// Adds accessors for using vector as a 2d structure
// Vector2d class inheriting from vector
//-------------------------------------------------------------------------------------

template <class T>
class Vector2d : public Vector<T>
{
protected:
    U32	mWidth;
    U32 mHeight;
   using Vector<T>::mArray;
   using Vector<T>::mElementCount;
   using Vector<T>::mArraySize;
   using Vector<T>::reserve;

public:
    Vector2d( const U32 initialWidth = 0, const U32 initialHeight = 0 )
    {
        mArray        = 0;
        mElementCount = 0;
        mArraySize    = 0;
        mWidth = initialWidth;
        mHeight = initialHeight;

        if(initialWidth && initialHeight)
            reserve( initialWidth * initialHeight );
    }

    //---------------------------------------------------------

    U32 width()
    {
        return mWidth;
    }

    //---------------------------------------------------------

    U32 height()
    {
        return mHeight;
    }

    //---------------------------------------------------------

    bool resize( const U32 width, const U32 height )
    {
        reserve( width * height );
        mWidth = width;
        mHeight = height;
        return true;
    }

    //---------------------------------------------------------

    T& get( const U32 indexX, const U32 indexY )
    {
        U32 index = ( indexY * mWidth ) + indexX;
        return mArray[index];
    }

    //---------------------------------------------------------

    T& get( const Vector2 v )
    {
        U32 index = ( v.mY * mWidth ) + v.mX;
        return mArray[index];
    }
};

#endif