//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PLATFORM_MATH_H_
#define _PLATFORM_MATH_H_

#ifndef _TORQUE_TYPES_H_
#include "platform/types.h"
#endif

//------------------------------------------------------------------------------

struct Math
{
   static void init( U32 properties = 0 );   // 0 == detect available hardware
};

#endif // _PLATFORM_MATH_H_
