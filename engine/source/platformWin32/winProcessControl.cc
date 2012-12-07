//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformWin32/platformWin32.h"

void Platform::postQuitMessage(const U32 in_quitVal)
{
   PostQuitMessage(in_quitVal);
}

void Platform::debugBreak()
{
    DebugBreak();
}

void Platform::forceShutdown(S32 returnValue)
{
   ExitProcess(returnValue);
}

void Platform::outputDebugString( const char *string )
{
   OutputDebugStringA( string );
   OutputDebugStringA( "\n" );
}
