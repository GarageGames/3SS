//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERPATHEDITTOOL_H_
#define _LEVELBUILDERPATHDITTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _PATH_H_
#include "2d/sceneobject/Path.h"
#endif

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#include "editor/levelBuilderBaseEditTool.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderPathEditTool
// Provides path editing functionality.
//-----------------------------------------------------------------------------
class LevelBuilderPathEditTool : public LevelBuilderBaseEditTool
{
private:
   bool mAddUndo;
   UndoAction* mUndoAction;

protected:
   typedef LevelBuilderBaseEditTool Parent;

   LevelBuilderSceneWindow* mSceneWindow;
   Path*                 mPath;
   S32                      mDraggingNode;
   S32                      mDraggingHandle;
   F32                      mStartRotation;
   Vector2                mRotationVector;
  
public:
   LevelBuilderPathEditTool();
   ~LevelBuilderPathEditTool();
   
   virtual bool onActivate(LevelBuilderSceneWindow* sceneWindow);
   virtual void onDeactivate();
   virtual void showObject();

    virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onRightMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   // Object Editing
   void editObject(Path* object);
   // This cancels an edit, applying changes.
   void finishEdit();

   S32 findPathNode(Path* path, Vector2 position);
   S32 findClosestNode(Path* path, Vector2 position);
   S32 findBezierHandle(Path::PathNode& node, Vector2 position);

   void onRenderScene( LevelBuilderSceneWindow* sceneWindow );

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderPathEditTool);
};

class UndoPathNodeAddAction : public UndoAction
{
private:
   LevelBuilderSceneEdit* mSceneEdit;
   Path* mObject;
   U32 mNode;
   Vector2 mPosition;
   F32 mWeight;
   F32 mRotation;

public:
   UndoPathNodeAddAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* actionName) : UndoAction(actionName) { mSceneEdit = sceneEdit; };

   void setNode(Path* object, U32 node, Vector2 position, F32 rotation, F32 weight)
   {
      mObject = object;
      mNode = node;
      mPosition = position;
      mRotation = rotation;
      mWeight = weight;
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      if ((object == mObject) && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      mObject->removeNode(mNode);
      mSceneEdit->onObjectSpatialChanged(mObject);
   };
   virtual void redo()
   {
      mObject->addNode(mPosition, mRotation, mWeight, mNode);
      mSceneEdit->onObjectSpatialChanged(mObject);
   };
};

class UndoPathNodeRemoveAction : public UndoAction
{
private:
   LevelBuilderSceneEdit* mSceneEdit;
   Path* mObject;
   U32 mNode;
   Vector2 mPosition;
   F32 mWeight;
   F32 mRotation;

public:
   UndoPathNodeRemoveAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* actionName) : UndoAction(actionName) { mSceneEdit = sceneEdit; };

   void setNode(Path* object, U32 node, Vector2 position, F32 rotation, F32 weight)
   {
      mObject = object;
      mNode = node;
      mPosition = position;
      mRotation = rotation;
      mWeight = weight;
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      if ((object == mObject) && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void redo()
   {
      mObject->removeNode(mNode);
      mSceneEdit->onObjectSpatialChanged(mObject);
   };
   virtual void undo()
   {
      mObject->addNode(mPosition, mRotation, mWeight, mNode);
      mSceneEdit->onObjectSpatialChanged(mObject);
   };
};

class UndoPathNodeMoveAction : public UndoAction
{
private:
   LevelBuilderSceneEdit* mSceneEdit;
   Path* mObject;
   
   U32 mNode;
   Vector2 mStartPosition;
   Vector2 mEndPosition;

public:
   UndoPathNodeMoveAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* actionName) : UndoAction(actionName) { mSceneEdit = sceneEdit; };

   void setStartPosition(Path* object, U32 node, Vector2 position) { mObject = object; mNode = node; mStartPosition = position; deleteNotify(object); };
   void setEndPosition(Vector2 position) { mEndPosition = position; };

   virtual void onDeleteNotify(SimObject* object)
   {
      if ((object == mObject) && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      if (mObject->isValidNode(mNode))
      {
         mObject->getNode(mNode).position = mStartPosition;
         mSceneEdit->onObjectSpatialChanged(mObject);
      }
   };
   virtual void redo()
   {
      if (mObject->isValidNode(mNode))
      {
         mObject->getNode(mNode).position = mEndPosition;
         mSceneEdit->onObjectSpatialChanged(mObject);
      }
   };
};

class UndoPathNodeRotateAction : public UndoAction
{
private:
   LevelBuilderSceneEdit* mSceneEdit;
   Path* mObject;
   
   U32 mNode;
   F32 mStartRotation;
   F32 mStartWeight;
   F32 mEndRotation;
   F32 mEndWeight;

public:
   UndoPathNodeRotateAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* actionName) : UndoAction(actionName) { mSceneEdit = sceneEdit; };

   void setStartRotation(Path* object, U32 node, F32 rotation, F32 weight) { mObject = object; mNode = node; mStartRotation = rotation; mStartWeight = weight; deleteNotify(object); };
   void setEndRotation(F32 rotation, F32 weight) { mEndRotation = rotation; mEndWeight = weight; };

   virtual void onDeleteNotify(SimObject* object)
   {
      if ((object == mObject) && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      if (mObject->isValidNode(mNode))
      {
         Path::PathNode& node = mObject->getNode(mNode);
         node.rotation = mStartRotation;
         node.weight = mStartWeight;
         mSceneEdit->onObjectSpatialChanged(mObject);
      }
   };
   virtual void redo()
   {
      if (mObject->isValidNode(mNode))
      {
         Path::PathNode& node = mObject->getNode(mNode);
         node.rotation = mEndRotation;
         node.weight = mEndWeight;
         mSceneEdit->onObjectSpatialChanged(mObject);
      }
   };
};

#endif


#endif // TORQUE_TOOLS
