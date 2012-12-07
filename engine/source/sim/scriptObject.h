//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCRIPT_OBJECT_H_
#define _SCRIPT_OBJECT_H_

#ifndef _CONSOLEINTERNAL_H_
#include "console/consoleInternal.h"
#endif

//-----------------------------------------------------------------------------

class ScriptObject : public SimObject
{
   typedef SimObject Parent;

public:
   ScriptObject();
   bool onAdd();
   void onRemove();

   DECLARE_CONOBJECT(ScriptObject);
};

#endif // _SCRIPT_OBJECT_H_