//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "console/console.h"
#include "console/consoleTypes.h"
#include "platform/threads/mutex.h"

//Added for the cprintf below
#include <stdarg.h>
#include <stdio.h>

// The tools prefer to allow the CPU time to process
#ifdef TORQUE_OS_IOS
#ifndef TORQUE_TOOLS
S32 sgBackgroundProcessSleepTime = 16;
#else
S32 sgBackgroundProcessSleepTime = 200;
#endif
S32 sgTimeManagerProcessInterval = 16;
#else
#ifndef TORQUE_TOOLS
S32 sgBackgroundProcessSleepTime = 25;
#else
S32 sgBackgroundProcessSleepTime = 200;
#endif
S32 sgTimeManagerProcessInterval = 0;
#endif //TORQUE__IPHONE

void Platform::initConsole()
{
   Con::addVariable("Pref::backgroundSleepTime", TypeS32, &sgBackgroundProcessSleepTime);
   Con::addVariable("Pref::timeManagerProcessInterval", TypeS32, &sgTimeManagerProcessInterval);
}

S32 Platform::getBackgroundSleepTime()
{
   return sgBackgroundProcessSleepTime;
}

void Platform::cprintf( const char* str )
{
    printf( "%s \n", str );
}

bool Platform::hasExtension(const char* pFilename, const char* pExtension)
{
    // Sanity!
    AssertFatal( pFilename != NULL, "Filename cannot be NULL." );
    AssertFatal( pExtension != NULL, "Extension cannot be NULL." );

    // Find filename length.
    const U32 filenameLength = dStrlen( pFilename );

    // Find extension length.
    const U32 extensionLength = dStrlen( pExtension );

    // Skip if extension is longer than filename.
    if ( extensionLength >= filenameLength )
        return false;

    // Check if extension exists.
    return dStricmp( pFilename + filenameLength - extensionLength, pExtension ) == 0;
}

ConsoleFunction( createUUID, const char*, 1, 1, "() - Creates a UUID string." )
{
    return Platform::createUUID();
}
