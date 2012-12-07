//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _MACCARBCONSOLE_H_
#define _MACCARBCONSOLE_H_

#ifndef _CONSOLE_H_
#include "console/console.h"
#endif
#ifndef _EVENT_H_
#include "platform/event.h"
#endif


class MacConsole
{
private:
   bool consoleEnabled;
   
   U32   inBufPos;
   char  inBuf[MaxConsoleLineSize];
   ConsoleEvent postEvent;
   
   void clearInBuf();
   
public:
   static void create();
   static void destroy();
   static bool isEnabled();

   MacConsole();
   ~MacConsole();
   void enable(bool);

   void processConsoleLine(const char *consoleLine);
   
   void  inputLoop();

};

extern MacConsole *gConsole;

#endif
