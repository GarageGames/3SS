//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderShapeVectorTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderShapeVectorTool);

LevelBuilderShapeVectorTool::LevelBuilderShapeVectorTool() : LevelBuilderCreateTool()
{
   // Set our tool name
   mToolName = StringTable->insert("Shape Vector Tool");
}

LevelBuilderShapeVectorTool::~LevelBuilderShapeVectorTool()
{
}

ShapeVector* LevelBuilderShapeVectorTool::createObject()
{
   ShapeVector* shapeVector = dynamic_cast<ShapeVector*>(ConsoleObject::create("ShapeVector"));

   return shapeVector;
}

void LevelBuilderShapeVectorTool::showObject()
{
   mCreatedObject->setVisible(true);
}


#endif // TORQUE_TOOLS
