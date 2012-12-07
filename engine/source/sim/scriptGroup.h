//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SCRIPT_GROUP_H_
#define _SCRIPT_GROUP_H_

#ifndef _CONSOLEINTERNAL_H_
#include "console/consoleInternal.h"
#endif

//-----------------------------------------------------------------------------

class ScriptGroup : public SimGroup
{
   typedef SimGroup Parent;
   
public:
   ScriptGroup();
   bool onAdd();
   void onRemove();

   DECLARE_CONOBJECT(ScriptGroup);
};

#endif // _SCRIPT_GROUP_H_