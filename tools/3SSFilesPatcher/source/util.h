//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _UTIL_H
#define _UTIL_H

#define MANIFEST_FILE "filesmanifest.txt"
#define ROOT_PATH "../../../"
#define MAX_BYTES_TO_COPY 4096

#define CHECK_3SS_INTERVAL 1000
#define EVENT_LOOP_INTERVAL 0
#define TIME_TO_CLOSE 4000

namespace Platform
{
    bool Has3SSClosed(int pid);
};

#endif  // _UTIL_H
