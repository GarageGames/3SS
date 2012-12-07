//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "platform/types.h"
#include "application/Application.h"

int main(int argc, const char * argv[])
{
    new Application();
    Application::singleton()->Begin(argc, argv);
    
    return 0;
}

