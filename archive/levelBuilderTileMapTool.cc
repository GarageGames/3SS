//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "editor/levelBuilderTileMapTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderTileMapTool);

LevelBuilderTileMapTool::LevelBuilderTileMapTool() : LevelBuilderCreateTool(),
                                                     mTileLayerFile(NULL),
                                                     mTileMap(NULL)
{
   // Set our tool name
   mToolName = StringTable->insert("Tile Map Tool");
}

LevelBuilderTileMapTool::~LevelBuilderTileMapTool()
{
}

SceneObject* LevelBuilderTileMapTool::createObject()
{
   if (!mTileMap)
   {
      Con::warnf("LevelBuilderTileMapTool::createObject - No tile map for creating tile layers.");
      return NULL;
   }

   S32 id = mTileMap->createTileLayer(1, 1, 1, 1);
   TileLayer* tileLayer = dynamic_cast<TileLayer*>(Sim::findObject(id));
   if (tileLayer)
   {
      if (tileLayer->loadTileLayer(mTileLayerFile))
         return tileLayer;
   }

   return NULL;
}

Vector2 LevelBuilderTileMapTool::getDefaultSize( LevelBuilderSceneWindow *window )
{
   TileLayer* tileMap = dynamic_cast<TileLayer*>(mCreatedObject);

   if (tileMap)
   {
      return Vector2(tileMap->getTileCountX() * tileMap->getTileSizeX(),
                       tileMap->getTileCountY() * tileMap->getTileSizeY());
   }
   return Vector2(10.0f, 10.0f);
}

ConsoleMethod(LevelBuilderTileMapTool, setTileLayerFile, void, 3, 3, "Sets the tile layer file for the created tile layers.")
{
   if (Platform::isFile(argv[2]))
      object->setTileLayerFile(argv[2]);
   else
      Con::warnf("LevelBuilderTileMapTool::setTileLayer - Invalid tile layer file: %s", argv[2]);
}

ConsoleMethod(LevelBuilderTileMapTool, setTileMap, void, 3, 3, "Sets the tile map to place created layers in")
{
   TileMap* tileMap = dynamic_cast<TileMap*>(Sim::findObject(argv[2]));
   if (tileMap)
      object->setTileMap(tileMap);
}


#endif // TORQUE_TOOLS
