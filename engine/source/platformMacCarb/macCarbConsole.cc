//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformMacCarb/platformMacCarb.h"
#include "platformMacCarb/macCarbConsole.h"
#include "platform/event.h"
#include "game/gameInterface.h"
#include "platform/threads/thread.h"

#include <stdio.h>

// TODO: convert this to use ncurses.

MacConsole *gConsole = NULL;

ConsoleFunction(enableWinConsole, void, 2, 2, "(bool enable)")
{
   if (gConsole)
      gConsole->enable(dAtob(argv[1]));
}

static void macConsoleConsumer(ConsoleLogEntry::Level, const char *line)
{
   if (gConsole)
      gConsole->processConsoleLine(line);
}

static void macConsoleInputLoopThread(S32 *arg)
{
   if(!gConsole)
      return;
   gConsole->inputLoop();
}

void MacConsole::create()
{
   gConsole = new MacConsole();
}

void MacConsole::destroy()
{
   if (gConsole)
      delete gConsole;
   gConsole = NULL;
}

void MacConsole::enable(bool enabled)
{
   if (gConsole == NULL) return;
   
   consoleEnabled = enabled;
   if(consoleEnabled)
   {
      printf("Initializing Console...\n");
      new Thread((ThreadRunFunction)macConsoleInputLoopThread,0,true);
      printf("Console Initialized.\n");

      printf("%s", Con::getVariable("Con::Prompt"));
   }
   else
   {
      printf("Deactivating Console.");
   }
}

bool MacConsole::isEnabled()
{
   if ( !gConsole )
      return false;

   return gConsole->consoleEnabled;
}


MacConsole::MacConsole()
{
   consoleEnabled = false;
   clearInBuf();
   
   Con::addConsumer(macConsoleConsumer);
}

MacConsole::~MacConsole()
{
   Con::removeConsumer(macConsoleConsumer);
}

void MacConsole::processConsoleLine(const char *consoleLine)
{
   if(consoleEnabled)
   {
         printf("%s\n", consoleLine);
   }
}

void MacConsole::clearInBuf()
{
   dMemset(inBuf, 0, MaxConsoleLineSize);
   inBufPos=0;
}

void MacConsole::inputLoop()
{
   Con::printf("Console Input Thread Started");
   unsigned char c;
   while(consoleEnabled)
   {
      c = fgetc(stdin);
      if(c == '\n')
      {
         // exec the line
         dStrcpy(postEvent.data, inBuf);
         postEvent.size = ConsoleEventHeaderSize + dStrlen(inBuf) + 1;
         Con::printf("=> %s",postEvent.data);
         Game->postEvent(postEvent);
         // clear the buffer
         clearInBuf();
         // display the prompt. Note that we're using real printf, not Con::printf...
         printf("=> ");
      }
      else
      {
         // add it to the buffer.
         inBuf[inBufPos++] = c;
         // if we're full, clear & warn.
         if(inBufPos >= MaxConsoleLineSize-1)
         {
            clearInBuf();
            Con::warnf("Line too long, discarding the last 512 bytes...");
         }
      }
   }
   Con::printf("Console Input Thread Stopped");
}
