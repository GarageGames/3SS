//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/console.h"
#include "3ss/3ssVersion.h"

//-----------------------------------------------------------------------------

ConsoleFunction( getThreeStepStudioVersion, const char*, 1, 1, "Returns 3 Step Studio Version")
{
    return THREE_STEP_STUDIO_VERSION;
}

