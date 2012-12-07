//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "frameAllocator.h"
#include "console/console.h"

U8*   FrameAllocator::smBuffer = NULL;
U32   FrameAllocator::smWaterMark = 0;
U32   FrameAllocator::smHighWaterMark = 0;

#if defined(TORQUE_DEBUG)

ConsoleFunction(getMaxFrameAllocation, S32, 1,1, "getMaxFrameAllocation();")
{
   return sgMaxFrameAllocation;
}

#endif
