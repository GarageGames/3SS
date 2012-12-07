//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderTriggerTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderTriggerTool);

LevelBuilderTriggerTool::LevelBuilderTriggerTool() : LevelBuilderCreateTool()
{
   // Set our tool name
   mToolName = StringTable->insert("Trigger Tool");
}

LevelBuilderTriggerTool::~LevelBuilderTriggerTool()
{
}

SceneObject* LevelBuilderTriggerTool::createObject()
{
   Trigger* trigger = dynamic_cast<Trigger*>(ConsoleObject::create("Trigger"));

   return trigger;
}

void LevelBuilderTriggerTool::showObject()
{
   mCreatedObject->setDebugOn(BIT(1));
   mCreatedObject->setVisible(true);
}


#endif // TORQUE_TOOLS
