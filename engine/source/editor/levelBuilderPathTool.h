//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERPATHTOOL_H_
#define _LEVELBUILDERPATHTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _PATH_H_
#include "2d/sceneobject/Path.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderPathTool
//-----------------------------------------------------------------------------
class LevelBuilderPathTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

protected:
   virtual SceneObject* createObject();
   virtual void showObject();
   virtual Vector2 getDefaultSize( LevelBuilderSceneWindow *window );
  
public:
   LevelBuilderPathTool();
   ~LevelBuilderPathTool();

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderPathTool);
};

#endif


#endif // TORQUE_TOOLS
