//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERSTATICSPRITETOOL_H_
#define _LEVELBUILDERSTATICSPRITETOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2D/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2D/sceneObject.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderStaticSpriteTool
//-----------------------------------------------------------------------------
class LevelBuilderStaticSpriteTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry  mImageMapName;
   U32               mImageMapFrame;

protected:
   virtual SceneObject* createObject();
   virtual Point2I getPixelSize();
  
public:
   LevelBuilderStaticSpriteTool();
   ~LevelBuilderStaticSpriteTool();

   void setImageMapName( const char* name, S32 frame );

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderStaticSpriteTool);
};

#endif


#endif // TORQUE_TOOLS
