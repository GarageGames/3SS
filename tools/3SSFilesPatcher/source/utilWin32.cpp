//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "util.h"

#include "windows.h"
#include "Psapi.h"

namespace Platform
{
    bool Has3SSClosed(int pid)
    {
        DWORD processIds[1024];
        DWORD bytesReturned;

        // Get all processes on the system
        BOOL result = EnumProcesses(processIds, 1024, &bytesReturned);
        if(!result)
        {
            return false;
        }

        DWORD numProcesses = bytesReturned / sizeof(DWORD);

        // Check if 3SS is in the list
        bool found = false;
        for(unsigned int i=0; i<numProcesses; ++i)
        {
            if(processIds[i] == pid)
            {
                found = true;
                break;
            }
        }

        return !found;
    }
}
