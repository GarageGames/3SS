//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERTILEMAPEDITTOOL_H_
#define _LEVELBUILDERTILEMAPEDITTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/SceneWindow.h"
#endif

#ifndef _TILE_MAP_H_
#include "2d/TileMap.h"
#endif

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#include "editor/levelBuilderBaseEditTool.h"
#endif

class UndoTileEditAction;


///-----------------------------------------------------------------------------
/// Structures.
///-----------------------------------------------------------------------------
struct tLayerAlphaHandle
{
   SimObjectId     mObjectId;
   F32             mAlphaValue;
};

///-----------------------------------------------------------------------------
/// Types.
///-----------------------------------------------------------------------------
typedef Vector<tLayerAlphaHandle> typeLayerAlphaList;


//-----------------------------------------------------------------------------
// LevelBuilderTileMapEditTool
// Provides tilemap editing functionality.
//-----------------------------------------------------------------------------
class LevelBuilderTileMapEditTool : public LevelBuilderBaseEditTool
{
public:
   enum ToolType
   {
      SELECT_TOOL,
      PAINT_TOOL,
      EYE_TOOL,
      FLOOD_TOOL,
      ERASER_TOOL,

      INVALID_TOOL
   };

private:
   ToolType mTool;

   bool mMouseDown;

   StringTableEntry mBitmapName;
   TextureHandle mTextureHandle;

   bool mAddUndo;
   UndoTileEditAction* mUndoAction;

   StringTableEntry mImageMap;
   StringTableEntry mTileScript;
   StringTableEntry mCustomData;
   U32 mFrame;
   bool mFlipX;
   bool mFlipY;

   bool mDragSelect;
   RectI mDragRect;

   bool mUpdateImageMap;
   bool mUpdateTileScript;
   bool mUpdateCustomData;
   bool mUpdateFlipX;
   bool mUpdateFlipY;

   typeLayerAlphaList mLayerList;
   bool           mGridWasActive;

   void eraseSelected( UndoTileEditAction* undo );

   Vector<Point2I> mSelectedTiles;
   void selectTile(Point2I tilePosition);
   void deselectTile(Point2I tilePosition);
   void toggleTileSelection(Point2I tilePosition);
   void clearSelections();
   bool isSelected(Point2I tilePosition);
   void floodSelect(Point2I tilePosition, bool start = 0);

   void selectRect(RectF rect);
   void paintRect(RectF rect, UndoTileEditAction* undo);
   void eraseRect(RectF rect, UndoTileEditAction* undo);

   void drawTile(Point2I tilePosition, UndoTileEditAction* undo);
   void floodFill(Point2I tilePosition, UndoTileEditAction* undo, bool start = 0);
   void eraseTile(Point2I tilePosition, UndoTileEditAction* undo);
   void addUndoData(Point2I tilePosition, UndoTileEditAction* undo, bool old);

protected:
   typedef LevelBuilderBaseEditTool Parent;

   LevelBuilderSceneWindow* mSceneWindow;
   TileLayer*            mTileLayer;
  
public:
   LevelBuilderTileMapEditTool();
   ~LevelBuilderTileMapEditTool();
   
   virtual bool onActivate(LevelBuilderSceneWindow* sceneWindow);
   virtual void onDeactivate();

   void setTool(ToolType tool);
   Vector2 getTileSize();
   void applyToSelection();

   virtual bool onKeyDown( LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event );
    virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   // Object Editing
   void editObject(TileLayer* object);
   // This cancels an edit, applying changes.
   void finishEdit();

   void setIconBitmap( const char* bitmapName );

   void onRenderScene( LevelBuilderSceneWindow* sceneWindow );

   void setImageMap(const char* imageMap, bool useImageMap);
   void setFrame(U32 frame);
   void setTileScript(const char* script, bool useScript);
   void setCustomData(const char* data, bool useData);
   void setFlipX(bool flip, bool useFlip);
   void setFlipY(bool flip, bool useFlip);

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderTileMapEditTool);
};

class UndoTileEditAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   TileLayer* mLayer;

   struct TileChange
   {
      Point2I tile;
      StringTableEntry image;
      U32 frame;
      StringTableEntry customData;
      StringTableEntry tileScript;
      bool flipX;
      bool flipY;
      
      StringTableEntry oldImage;
      U32 oldFrame;
      StringTableEntry oldCustomData;
      StringTableEntry oldTileScript;
      bool oldFlipX;
      bool oldFlipY;
   };

   Vector<TileChange> mTiles;

public:
   UndoTileEditAction(TileLayer* layer, const UTF8* actionName) : UndoAction(actionName) { mLayer = layer; deleteNotify(layer); };

   virtual void onDeleteNotify(SimObject* object)
   {
      if (mUndoManager)
         mUndoManager->removeAction(this);
   };

   bool hasChanged()
   {
      for (S32 i = 0; i < mTiles.size(); i++)
      {
         TileChange& tile = mTiles[i];
         if ((tile.image != tile.oldImage) ||
             (tile.frame != tile.oldFrame) ||
             (tile.customData != tile.oldCustomData) ||
             (tile.tileScript != tile.oldTileScript) ||
             (tile.flipX != tile.oldFlipX) ||
             (tile.flipY != tile.oldFlipY) )
            return true;
      }

      return false;
   };

   S32 findTile(Point2I tilePosition)
   {
      for (U32 i = 0; i < (U32)mTiles.size(); i++)
      {
         if (mTiles[i].tile == tilePosition)
            return i;
      }
      return -1;
   }

   void addOldTile(Point2I tilePosition, StringTableEntry image, U32 frame, StringTableEntry customData,
                   StringTableEntry tileScript, bool flipX, bool flipY)
   {
      S32 tileIndex = findTile(tilePosition);
      if (tileIndex != -1)
         return;

      mTiles.increment();
      TileChange& tile = mTiles.last();
      constructInPlace(&tile);
      tile.tile = tilePosition;
      tile.image = image;
      tile.frame = frame;
      tile.customData = customData;
      tile.tileScript = tileScript;
      tile.flipX = flipX;
      tile.flipY = flipY;
   }

   void addNewTile(Point2I tilePosition, StringTableEntry image, U32 frame, StringTableEntry customData,
                   StringTableEntry tileScript, bool flipX, bool flipY)
   {
      S32 tileIndex = findTile(tilePosition);
      if (tileIndex == -1)
         return;

      TileChange& tile = mTiles[tileIndex];
      tile.oldImage = image;
      tile.oldFrame = frame;
      tile.oldCustomData = customData;
      tile.oldTileScript = tileScript;
      tile.oldFlipX = flipX;
      tile.oldFlipY = flipY;
   }

   virtual void redo()
   {
      for (U32 i = 0; i < (U32)mTiles.size(); i++)
      {
         TileChange& tile = mTiles[i];
         S32 x = tile.tile.x;
         S32 y = tile.tile.y;

         if (!tile.oldImage || !*tile.oldImage)
            mLayer->clearTile(x, y);

         else
         {
            ImageAsset* imageMap = dynamic_cast<ImageAsset*>(Sim::findObject(tile.oldImage));
            AnimationAsset* animation = dynamic_cast<AnimationAsset*>(Sim::findObject(tile.oldImage));
            if (imageMap)
               mLayer->setStaticTile(x, y, tile.oldImage, tile.oldFrame);
            else if (animation)
               mLayer->setAnimatedTile(x, y, tile.oldImage, false);
            else
               return;
            mLayer->setTileCustomData(x, y, tile.oldCustomData);
            mLayer->setTileScript(x, y, tile.oldTileScript);
            mLayer->setTileFlip(x, y, tile.oldFlipX, tile.oldFlipY);
         }
      }

      Con::executef( mLayer, 1, "onChanged" );
   };
   virtual void undo()
   {
      for (U32 i = 0; i < (U32)mTiles.size(); i++)
      {
         TileChange& tile = mTiles[i];
         S32 x = tile.tile.x;
         S32 y = tile.tile.y;

         if (!tile.image || !*tile.image)
            mLayer->clearTile(x, y);

         else
         {
            ImageAsset* imageMap = dynamic_cast<ImageAsset*>(Sim::findObject(tile.image));
            AnimationAsset* animation = dynamic_cast<AnimationAsset*>(Sim::findObject(tile.image));
            if (imageMap)
               mLayer->setStaticTile(x, y, tile.image, tile.frame);
            else if (animation)
               mLayer->setAnimatedTile(x, y, tile.image, false);
            else
               return;

            mLayer->setTileCustomData(x, y, tile.customData);
            mLayer->setTileScript(x, y, tile.tileScript);
            mLayer->setTileFlip(x, y, tile.flipX, tile.flipY);
         }
      }

      Con::executef( mLayer, 1, "onChanged" );
   };
};

#endif


#endif // TORQUE_TOOLS
