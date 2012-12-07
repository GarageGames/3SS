//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERCAMERATOOL_H_
#define _LEVELBUILDERCAMERATOOL_H_

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#include "editor/levelBuilderBaseEditTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

class UndoCameraAction;
class UndoFullCameraAction;

//-----------------------------------------------------------------------------
// LevelBuilderCameraTool
// Provides editing functionality for the scene camera area.
//-----------------------------------------------------------------------------
class LevelBuilderCameraTool : public LevelBuilderBaseEditTool
{
private:
   UndoFullCameraAction* mUndoFullAction;
   UndoCameraAction* mUndoAction;

   F32 mMouseDownAR;

   Vector2 mStartPos;
   Vector2 mStartSize;

protected:
   // Whether or not the camera is being moved.
   bool mMoving;

   typedef LevelBuilderBaseEditTool Parent;

   // Current size and position of the camera being edited.
   Vector2 mCameraPosition;
   Vector2 mCameraSize;
   Vector2 mOffset;

   ColorI mCameraOutlineColor;
   ColorI mCameraFillColor;

   // The camera area of the editor scene window prior to editing.
   RectF mCameraArea;
   F32 mCameraZoom;

   // The scene window on which this tool is being used.
   LevelBuilderSceneWindow* mSceneWindow;
  
public:
   LevelBuilderCameraTool();
   ~LevelBuilderCameraTool();
   
   virtual bool onAdd();
   virtual void onRemove();

   virtual bool hasUndoManager() { return true; };

    virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   virtual bool onActivate(LevelBuilderSceneWindow* sceneWindow);
   virtual void onDeactivate();

   virtual bool undo() { mUndoManager.undo(); return true; };
   virtual bool redo() { mUndoManager.redo(); return true; };

   void onRenderScene( LevelBuilderSceneWindow* sceneWindow );

   // State Info Accessors
   Vector2 getCameraPosition() { return mCameraPosition; };
   Vector2 getCameraSize() { return mCameraSize; };
   void setCameraPosition(Vector2 pos) { mCameraPosition = pos; };
   void setCameraSize(Vector2 size) { mCameraSize = size; };

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderCameraTool);
};

// This undoes the entire camera change.
class UndoFullCameraAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   LevelBuilderCameraTool* mCameraTool;

   Vector2 mStartPosition;
   Vector2 mStartSize;
   Vector2 mFinishPosition;
   Vector2 mFinishSize;

   SimObjectPtr<Scene> mpScene;

public:
   UndoFullCameraAction(LevelBuilderCameraTool* camera, Scene* pScene, const UTF8* actionName) : UndoAction(actionName) { mpScene = pScene; mCameraTool = camera; };

   void setStartBounds(Vector2 pos, Vector2 size) { mStartPosition = pos; mStartSize = size; };
   void setFinishBounds(Vector2 pos, Vector2 size) { mFinishPosition = pos; mFinishSize = size; };

   bool hasChanged() { return !((mStartPosition == mFinishPosition) && (mStartSize == mFinishSize)); };

   virtual void undo()
   {
      if (!mpScene) return;
      char pos[32];
      dSprintf(pos, 32, "%g %g", mStartPosition.x, mStartPosition.y);
      mpScene->setDataField(StringTable->insert("cameraPosition"), NULL, pos);

      char size[32];
      dSprintf(size, 32, "%g %g", mStartSize.x, mStartSize.y);
      mpScene->setDataField(StringTable->insert("cameraSize"), NULL, size);

      Con::executef(mCameraTool, 3, "onCameraChanged", pos, size);
   };

   virtual void redo()
   {
      if (!mpScene) return;
      char pos[32];
      dSprintf(pos, 32, "%g %g", mFinishPosition.x, mFinishPosition.y);
      mpScene->setDataField(StringTable->insert("cameraPosition"), NULL, pos);

      char size[32];
      dSprintf(size, 32, "%g %g", mFinishSize.x, mFinishSize.y);
      mpScene->setDataField(StringTable->insert("cameraSize"), NULL, size);

      Con::executef(mCameraTool, 3, "onCameraChanged", pos, size);
   };
};

// This undoes each incremental change by the camera tool.
class UndoCameraAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   LevelBuilderCameraTool* mCameraTool;
   
   Vector2 mStartPosition;
   Vector2 mStartSize;
   Vector2 mFinishPosition;
   Vector2 mFinishSize;

public:
   UndoCameraAction(LevelBuilderCameraTool* camera, const UTF8* actionName) : UndoAction(actionName) { mCameraTool = camera; };

   bool hasChanged() { return !((mStartPosition == mFinishPosition) && (mStartSize == mFinishSize)); };

   void setStartBounds(Vector2 pos, Vector2 size) { mStartPosition = pos; mStartSize = size; };
   void setFinishBounds(Vector2 pos, Vector2 size) { mFinishPosition = pos; mFinishSize = size; };

   virtual void undo()
   {
      mCameraTool->setCameraPosition(mStartPosition);
      mCameraTool->setCameraSize(mStartSize);

      char position[64];
      dSprintf(position, 64, "%g %g", mStartPosition.x, mStartPosition.y);
      char size[64];
      dSprintf(size, 64, "%g %g", mStartSize.x, mStartSize.y);
      Con::executef(mCameraTool, 3, "onCameraChanged", position, size);
   };

   virtual void redo()
   {
      mCameraTool->setCameraPosition(mFinishPosition);
      mCameraTool->setCameraSize(mFinishSize);

      char position[64];
      dSprintf(position, 64, "%g %g", mFinishPosition.x, mFinishPosition.y);
      char size[64];
      dSprintf(size, 64, "%g %g", mFinishSize.x, mFinishSize.y);
      Con::executef(mCameraTool, 3, "onCameraChanged", position, size);
   };
};

#endif


#endif // TORQUE_TOOLS
