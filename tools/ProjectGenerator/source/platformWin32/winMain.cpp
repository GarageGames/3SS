//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "platform/types.h"
#include "application/Application.h"

S32 main(S32 argc, const char **argv)
{
//   winState.appInstance = GetModuleHandle(NULL);
//   return run(argc, argv);

    new Application();
    Application::singleton()->Begin(argc, argv);

    return 0;
}
