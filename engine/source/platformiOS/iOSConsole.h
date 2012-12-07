//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _iOSCONSOLE_H_
#define _iOSCONSOLE_H_

#ifndef _CONSOLE_H_
#include "console/console.h"
#endif
#ifndef _EVENT_H_
#include "Platform/event.h"
#endif


class iOSConsole
{
private:
   bool consoleEnabled;
	bool debugOutputEnabled;
   
   U32   inBufPos;
   char  inBuf[MaxConsoleLineSize];
   ConsoleEvent postEvent;
   
   void clearInBuf();
   
public:
   static void create();
   static void destroy();
   static bool isEnabled();

   iOSConsole();
   ~iOSConsole();
   void enable(bool);
	//%PUAP%
	void enableDebugOutput( bool );

   void processConsoleLine(const char *consoleLine);
   
   void  inputLoop();

};

extern iOSConsole *gConsole;

#endif
