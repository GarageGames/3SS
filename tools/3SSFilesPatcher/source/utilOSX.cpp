//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "util.h"

#include <errno.h>
#include <stdlib.h>
#include <stdio.h>
#include <sys/sysctl.h>

namespace Platform
{
    bool Has3SSClosed(int pid)
    {
        int         error = 0;
        kinfo_proc* result = NULL;
        bool        done = false;
        size_t      length = 0;
        static const int name[] = {CTL_KERN, KERN_PROC, KERN_PROC_ALL, 0};

        size_t numProcesses = 0;
        
        do {
            // Call sysctl with a NULL buffer
            length = 0;
            error = sysctl((int*)name, (sizeof(name)/sizeof(*name))-1, NULL, &length, NULL, 0);
            if(error == -1)
            {
                error = errno;
            }
            
            // Allocate an appropriately sized buffer based on the results
            if(error == 0)
            {
                result = (kinfo_proc*)malloc(length);
                if(result == NULL)
                    error = ENOMEM;
            }
            
            // Call sysctl again with the new buffer
            if(error == 0)
            {
                error = sysctl((int*)name, (sizeof(name)/sizeof(*name))-1, result, &length, NULL, 0);
                if(error == -1)
                {
                    error = errno;
                }
                
                if(error == 0)
                {
                    done = true;
                }
                else if(error == ENOMEM)
                {
                    free(result);
                    result = NULL;
                    error = 0;
                }
            }
        } while(error == 0 && !done);

        bool found = false;
        if(error == 0 && result != NULL)
        {
            // Check if 3SS is in the list
            numProcesses = length / sizeof(kinfo_proc);
            
            for(unsigned int i=0; i<numProcesses; ++i)
            {
                if(result[i].kp_proc.p_pid == pid)
                {
                    found = true;
                    break;
                }
            }
        }
        else if(result != NULL)
        {
            free(result);
            result = NULL;
        }

        return !found;
    }
}
