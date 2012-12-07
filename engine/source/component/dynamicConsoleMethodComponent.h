//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _DYNAMIC_CONSOLEMETHOD_COMPONENT_H_
#define _DYNAMIC_CONSOLEMETHOD_COMPONENT_H_

#include "component/simComponent.h"
#include "console/consoleInternal.h"

class DynamicConsoleMethodComponent : public SimComponent
{
   typedef SimComponent Parent;

protected:
   /// Internal callMethod : Actually does component notification and script method execution
   ///  @attention This method does some magic to the argc argv to make Con::execute act properly
   ///   as such it's internal and should not be exposed or used except by this class
   virtual const char* _callMethod( U32 argc, const char *argv[], bool callThis = true );

public:

   /// Call Method format string
   const char* callMethod( S32 argc, const char* methodName, ... );

   /// Call Method
   virtual const char* callMethodArgList( U32 argc, const char *argv[], bool callThis = true );

   // query for console method data
   virtual bool handlesConsoleMethod(const char * fname, S32 * routingId);

   ///
   virtual const char* callOnBehaviors( U32 argc, const char *argv[] );

   DECLARE_CONOBJECT(DynamicConsoleMethodComponent);
};

#endif