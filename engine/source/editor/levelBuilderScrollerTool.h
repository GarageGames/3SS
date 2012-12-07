//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERSCROLLERTOOL_H_
#define _LEVELBUILDERSCROLLERTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _SCROLLER_H_
#include "2d/sceneobject/Scroller.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderScrollerTool
//-----------------------------------------------------------------------------
class LevelBuilderScrollerTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry mImageMapName;

protected:
   virtual SceneObject* createObject();
   virtual Point2I getPixelSize();
  
public:
   LevelBuilderScrollerTool();
   ~LevelBuilderScrollerTool();

   void setImageMapName(const char* name) { mImageMapName = StringTable->insert(name); };

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderScrollerTool);
};

#endif


#endif // TORQUE_TOOLS
