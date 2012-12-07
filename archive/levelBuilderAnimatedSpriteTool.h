//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERANIMATEDSPRITETOOL_H_
#define _LEVELBUILDERANIMATEDSPRITETOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2D/SceneWindow.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderAnimatedSpriteTool
//-----------------------------------------------------------------------------
class LevelBuilderAnimatedSpriteTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry mAnimationName;

protected:
   virtual SceneObject* createObject();
   virtual Point2I getPixelSize();
  
public:
   LevelBuilderAnimatedSpriteTool();
   ~LevelBuilderAnimatedSpriteTool();

   void setAnimationName(const char* name) { mAnimationName = StringTable->insert(name); };

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderAnimatedSpriteTool);
};

#endif

#endif // TORQUE_TOOLS
