//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERSCENEOBJECTTOOL_H_
#define _LEVELBUILDERSCENEOBJECTTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderSceneObjectTool
//-----------------------------------------------------------------------------
class LevelBuilderSceneObjectTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

protected:
   virtual SceneObject* createObject();
   virtual void showObject();
  
public:
   LevelBuilderSceneObjectTool();
   ~LevelBuilderSceneObjectTool();

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderSceneObjectTool);
};

#endif


#endif // TORQUE_TOOLS
