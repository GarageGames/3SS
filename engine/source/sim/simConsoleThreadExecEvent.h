//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SIM_CONSOLE_THREAD_EXEC_EVENT_H_
#define _SIM_CONSOLE_THREAD_EXEC_EVENT_H_

#ifndef _SIM_CONSOLE_EVENT_H_
#include "sim/simConsoleEvent.h"
#endif

//-----------------------------------------------------------------------------

/// Used by Con::threadSafeExecute()
struct SimConsoleThreadExecCallback
{
   void *sem;
   const char *retVal;

   SimConsoleThreadExecCallback();
   ~SimConsoleThreadExecCallback();

   void handleCallback(const char *ret);
   const char *waitForResult();
};

class SimConsoleThreadExecEvent : public SimConsoleEvent
{
   SimConsoleThreadExecCallback *cb;

public:
   SimConsoleThreadExecEvent(S32 argc, const char **argv, bool onObject, SimConsoleThreadExecCallback *callback);

   virtual void process(SimObject *object);
};

#endif // _SIM_CONSOLE_THREAD_EXEC_EVENT_H_