//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERPOLYTOOL_H_
#define _LEVELBUILDERPOLYTOOL_H_

#ifndef _LEVELBUILDERBASETOOL_H_
#include "editor/levelBuilderBaseTool.h"
#endif

#ifndef _UNDO_H_
#include "util/undo.h"
#endif

class UndoPolyAction;
class UndoFullPolyAction;

//-----------------------------------------------------------------------------
// LevelBuilderPolyTool
//-----------------------------------------------------------------------------
class LevelBuilderPolyTool : public LevelBuilderBaseTool
{
   typedef LevelBuilderBaseTool Parent;

private:
   S32 mDragVertex;

   // This undoes the entire change from the main undo manager.
   UndoFullPolyAction* mUndoFullAction;
   // This undoes each incremental change while using this tool.
   UndoAction* mUndoAction;
   bool mAddUndo;

protected:
   F32   mAngle;
   bool  mFlipSettings[2];
   RectF mCameraArea;
   F32 mCameraZoom;

   LevelBuilderSceneWindow* mSceneWindow;
   SceneObject*          mSceneObject;
   
   // Helper Functions
   S32     findCollisionVertex(Point2I hitPoint);
   Point2I getCollisionPointWorld(LevelBuilderSceneWindow* sceneWindow, const SceneObject *obj, Point2F oneToOnePoint) const;
   Point2F getCollisionPointObject(LevelBuilderSceneWindow* sceneWindow, const SceneObject *obj, const Point2I &worldPoint) const;

   // Object Editing
   bool acquireCollisionPoly(SceneObject* object);
   bool setCollisionPoly(SceneObject* object);
   void clearCollisionPoly();
   bool isEditable(SceneObject* obj);

   // State Info
   t2dVector mLocalMousePosition;
   
public:
   LevelBuilderPolyTool();
   virtual ~LevelBuilderPolyTool();
   
   virtual bool onAdd();
   virtual void onRemove();

   virtual bool hasUndoManager() { return true; };
   
   // Convex Checking
   bool checkConvexPoly(Vector<t2dVector> &list);
   bool checkDragPoint(Vector<Point2F> &list, S32 index, Point2I dragPoint);
   S32  checkNewPointConvexAddition(const Point2I &newPoint);

   // Input Events
   virtual bool onMouseMove( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onRightMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onRightMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onRightMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   // Base Tool Overrides
   bool onActivate(LevelBuilderSceneWindow* sceneWindow);
   void onDeactivate();
   bool onAcquireObject(SceneObject* object);
   void onRelinquishObject(SceneObject* object);

   // Object Editing
   void editObject(SceneObject* object);
   // This cancels an edit, not applying any changes.
   void cancelEdit();
   // This cancels an edit, applying changes.
   void finishEdit();

   virtual bool undo() { mUndoManager.undo(); return true; };
   virtual bool redo() { mUndoManager.redo(); return true; };

   void moveVertex(S32 index, t2dVector position);
   void insertVertex(t2dVector position, S32 index);
   void removeVertex(S32 index);

   void onRenderGraph(LevelBuilderSceneWindow* sceneWindow);

   inline S32       getVertexCount() { return mNutList.size(); };
   StringTableEntry getCollisionPolyScript();
   void             setPolyPrimitive(U32 polyVertexCount);

   // State Accessors
   t2dVector getLocalMousePosition() { return mLocalMousePosition; };

   DECLARE_CONOBJECT(LevelBuilderPolyTool);
};

// Undoes an entire poly edit for the global undo manager.
class UndoFullPolyAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   SceneObject* mSceneObject;
   Vector<t2dVector> mOldPoints;
   Vector<t2dVector> mNewPoints;

public:
   UndoFullPolyAction(SceneObject* object, UTF8* actionName) : UndoAction(actionName) { mSceneObject = object; deleteNotify(object); };

   virtual void onDeleteNotify(SimObject* object)
   {
      if ((object == mSceneObject) && mUndoManager)
         mUndoManager->removeAction(this);
   };

   void setOldPoints(S32 count, const t2dVector* oldPoints)
   {
      mOldPoints.clear();
      for (S32 i = 0; i < count; i++)
         mOldPoints.push_back(oldPoints[i]);
   };

   void setNewPoints(S32 count, const t2dVector* newPoints)
   {
      mNewPoints.clear();
      for (S32 i = 0; i < count; i++)
         mNewPoints.push_back(newPoints[i]);
   };

   bool hasChanged()
   {
      if (mOldPoints.size() != mNewPoints.size())
         return true;

      for (S32 i = 0; i < mOldPoints.size(); i++)
      {
         if (mOldPoints[i] != mNewPoints[i])
            return true;
      }

      return false;
   };

   virtual void undo()
   {
      mSceneObject->setCollisionPolyCustom(mOldPoints.size(), mOldPoints.address());
   };

   virtual void redo()
   {
      mSceneObject->setCollisionPolyCustom(mNewPoints.size(), mNewPoints.address());
   };
};

class UndoPolyAddVertexAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   LevelBuilderPolyTool* mPolyTool;
   
   S32 mIndex;
   t2dVector mPosition;

public:
   UndoPolyAddVertexAction(LevelBuilderPolyTool* tool, UTF8* actionName) : UndoAction(actionName) { mPolyTool = tool; };

   void setIndex(t2dVector position, S32 index) { mIndex = index, mPosition = position; };

   virtual void undo() { mPolyTool->removeVertex(mIndex); };
   virtual void redo() { mPolyTool->insertVertex(mPosition, mIndex); };
};

class UndoPolyRemoveVertexAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   LevelBuilderPolyTool* mPolyTool;
   
   Vector<S32> mIndex;
   Vector<t2dVector> mPosition;

public:
   UndoPolyRemoveVertexAction(LevelBuilderPolyTool* tool, UTF8* actionName) : UndoAction(actionName) { mPolyTool = tool; };

   void addIndex(t2dVector position, S32 index) { mIndex.push_back(index), mPosition.push_back(position); };

   virtual void undo() { for (S32 i = 0; i < mIndex.size(); i++) mPolyTool->insertVertex(mPosition[i], mIndex[i]); };
   virtual void redo() { for (S32 i = 0; i < mIndex.size(); i++) mPolyTool->removeVertex(mIndex[i]); };
};

class UndoPolyMoveVertexAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   LevelBuilderPolyTool* mPolyTool;
   
   S32 mIndex;
   t2dVector mStartPosition;
   t2dVector mEndPosition;

public:
   UndoPolyMoveVertexAction(LevelBuilderPolyTool* tool, UTF8* actionName) : UndoAction(actionName) { mPolyTool = tool; };

   void setStartPosition(t2dVector position, S32 index) { mIndex = index, mStartPosition = position; };
   void setEndPosition(t2dVector position) { mEndPosition = position; };

   virtual void undo() { mPolyTool->moveVertex(mIndex, mStartPosition); };
   virtual void redo() { mPolyTool->moveVertex(mIndex, mEndPosition); };
};

#endif


#endif // TORQUE_TOOLS
