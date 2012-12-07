//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "sim/simBase.h"
#include "console/consoleTypes.h"
#include "sim/scriptGroup.h"
#include "sim/simBase.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT(ScriptGroup);

//-----------------------------------------------------------------------------

ScriptGroup::ScriptGroup()
{
   mNSLinkMask = LinkSuperClassName | LinkClassName;
}

//-----------------------------------------------------------------------------

bool ScriptGroup::onAdd()
{
   if (!Parent::onAdd())
      return false;

   // Call onAdd in script!
   Con::executef(this, 2, "onAdd", Con::getIntArg(getId()));
   return true;
}

//-----------------------------------------------------------------------------

void ScriptGroup::onRemove()
{
   // Call onRemove in script!
   Con::executef(this, 2, "onRemove", Con::getIntArg(getId()));

   // Call parent.
   Parent::onRemove();
}
