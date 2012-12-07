//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERTILEMAPTOOL_H_
#define _LEVELBUILDERTILEMAPTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/SceneWindow.h"
#endif

#ifndef _TILE_MAP_H_
#include "2d/TileMap.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderTileMapTool
//-----------------------------------------------------------------------------
class LevelBuilderTileMapTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry mTileLayerFile;
   TileMap* mTileMap;

protected:
   virtual SceneObject* createObject();
   virtual Vector2 getDefaultSize( LevelBuilderSceneWindow *window );
  
public:
   LevelBuilderTileMapTool();
   ~LevelBuilderTileMapTool();

   void setTileMap(TileMap* map) { mTileMap = map; };
   void setTileLayerFile(const char* file) { mTileLayerFile = StringTable->insert(file); };

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderTileMapTool);
};

#endif


#endif // TORQUE_TOOLS
