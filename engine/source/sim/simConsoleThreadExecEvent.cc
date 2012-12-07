//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "sim/simConsoleThreadExecEvent.h"
#include "platform/platform.h"
#include "console/consoleInternal.h"

//-----------------------------------------------------------------------------

extern ExprEvalState gEvalState;

//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------

SimConsoleThreadExecCallback::SimConsoleThreadExecCallback() : retVal(NULL)
{
   sem = Semaphore::createSemaphore(0);
}

SimConsoleThreadExecCallback::~SimConsoleThreadExecCallback()
{
   Semaphore::destroySemaphore(sem);
}

void SimConsoleThreadExecCallback::handleCallback(const char *ret)
{
   retVal = ret;
   Semaphore::releaseSemaphore(sem);
}

const char *SimConsoleThreadExecCallback::waitForResult()
{
   if(Semaphore::acquireSemaphore(sem, true))
   {
      return retVal;
   }

   return NULL;
}

//-----------------------------------------------------------------------------

SimConsoleThreadExecEvent::SimConsoleThreadExecEvent(S32 argc, const char **argv, bool onObject, SimConsoleThreadExecCallback *callback) : 
   SimConsoleEvent(argc, argv, onObject),
   cb(callback)
{
}

void SimConsoleThreadExecEvent::process(SimObject* object)
{
   const char *retVal;
   if(mOnObject)
      retVal = Con::execute(object, mArgc, const_cast<const char**>( mArgv ));
   else
      retVal = Con::execute(mArgc, const_cast<const char**>( mArgv ));

   if(cb)
      cb->handleCallback(retVal);
}
