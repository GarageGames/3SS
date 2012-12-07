//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SIM_CONSOLE_EVENT_H_
#define _SIM_CONSOLE_EVENT_H_

#ifndef _SIM_EVENT_H_
#include "sim/simEvent.h"
#endif

//-----------------------------------------------------------------------------

/// Implementation of schedule() function.
///
/// This allows you to set a console function to be
/// called at some point in the future.

class SimConsoleEvent : public SimEvent
{
protected:
   S32 mArgc;
   char **mArgv;
   bool mOnObject;
  public:

   /// Constructor
   ///
   /// Pass the arguments of a function call, optionally on an object.
   ///
   /// The object for the call to be executed on is specified by setting
   /// onObject and storing a reference to the object in destObject. If
   /// onObject is false, you don't need to store anything into destObject.
   ///
   /// The parameters here are passed unmodified to Con::execute() at the
   /// time of the event.
   ///
   /// @see Con::execute(S32 argc, const char *argv[])
   /// @see Con::execute(SimObject *object, S32 argc, const char *argv[])
   SimConsoleEvent(S32 argc, const char **argv, bool onObject);

   ~SimConsoleEvent();
   virtual void process(SimObject *object);
};

#endif // _SIM_CONSOLE_EVENT_H_