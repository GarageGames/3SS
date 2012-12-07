//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERSHAPEVECTORTOOL_H_
#define _LEVELBUILDERSHAPEVECTORTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _SHAPE_VECTOR_H_
#include "2d/sceneobject/ShapeVector.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderShapeVectorTool
//-----------------------------------------------------------------------------
class LevelBuilderShapeVectorTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

protected:
   virtual ShapeVector* createObject();
   virtual void showObject();
  
public:
   LevelBuilderShapeVectorTool();
   ~LevelBuilderShapeVectorTool();

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderShapeVectorTool);
};

#endif


#endif // TORQUE_TOOLS
