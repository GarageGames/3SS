//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "sim/simBase.h"
#include "console/consoleTypes.h"
#include "sim/scriptObject.h"
#include "sim/simBase.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(ScriptObject);

//-----------------------------------------------------------------------------

ScriptObject::ScriptObject()
{
   mNSLinkMask = LinkSuperClassName | LinkClassName;
}

//-----------------------------------------------------------------------------

bool ScriptObject::onAdd()
{
   if (!Parent::onAdd())
      return false;

   // Call onAdd in script!
   Con::executef(this, 2, "onAdd", Con::getIntArg(getId()));
   return true;
}

//-----------------------------------------------------------------------------

void ScriptObject::onRemove()
{
   // Call onRemove in script!
   Con::executef(this, 2, "onRemove", Con::getIntArg(getId()));

   // Call parent.
   Parent::onRemove();
}
