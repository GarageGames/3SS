//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _ELEMENTREFERENCE_H
#define _ELEMENTREFERENCE_H

#include "platform/types.h"
#include <iostream>
#include <fstream>

using namespace std;

class ElementReference
{
protected:
    U32 m_Reference[3];

public:
    ElementReference()
    {
        Clear();
    }

    virtual ~ElementReference() {}

    void Clear()
    {
        m_Reference[0] = 0;
        m_Reference[1] = 0;
        m_Reference[2] = 0;
    }

    void Set(U32 a, U32 b, U32 c)
    {
        m_Reference[0] = a;
        m_Reference[1] = b;
        m_Reference[2] = c;
    }

    void Write(ofstream& file);
};

#endif  // _ELEMENTREFERENCE_H
