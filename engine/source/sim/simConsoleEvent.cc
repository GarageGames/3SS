//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "sim/simConsoleEvent.h"
#include "platform/platform.h"
#include "console/consoleInternal.h"

//-----------------------------------------------------------------------------

extern ExprEvalState gEvalState;

//-----------------------------------------------------------------------------

SimConsoleEvent::SimConsoleEvent(S32 argc, const char **argv, bool onObject)
{
   mOnObject = onObject;
   mArgc = argc;
   U32 totalSize = 0;
   S32 i;
   for(i = 0; i < argc; i++)
      totalSize += dStrlen(argv[i]) + 1;
   totalSize += sizeof(char *) * argc;

   mArgv = (char **) dMalloc(totalSize);
   char *argBase = (char *) &mArgv[argc];

   for(i = 0; i < argc; i++)
   {
      mArgv[i] = argBase;
      dStrcpy(mArgv[i], argv[i]);
      argBase += dStrlen(argv[i]) + 1;
   }
}

SimConsoleEvent::~SimConsoleEvent()
{
   dFree(mArgv);
}

void SimConsoleEvent::process(SimObject* object)
{
// #ifdef DEBUG
//    Con::printf("Executing schedule: %d", sequenceCount);
// #endif
    if(mOnObject)
      Con::execute(object, mArgc, const_cast<const char**>( mArgv ));
   else
   {
      // Grab the function name. If '::' doesn't exist, then the schedule is
      // on a global function.
      char* func = dStrstr( mArgv[0], (char*)"::" );
      if( func )
      {
         // Set the first colon to NULL, so we can reference the namespace.
         // This is okay because events are deleted immediately after
         // processing. Maybe a bad idea anyway?
         func[0] = '\0';

         // Move the pointer forward to the function name.
         func += 2;

         // Lookup the namespace and function entry.
         Namespace* ns = Namespace::find( StringTable->insert( mArgv[0] ) );
         if( ns )
         {
            Namespace::Entry* nse = ns->lookup( StringTable->insert( func ) );
            if( nse )
               // Execute.
               nse->execute( mArgc, (const char**)mArgv, &gEvalState );
         }
      }

      else
         Con::execute(mArgc, const_cast<const char**>( mArgv ));
   }
}
