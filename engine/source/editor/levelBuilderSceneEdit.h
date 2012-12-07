//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERSCENEEDIT_H_
#define _LEVELBUILDERSCENEEDIT_H_

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _SCENE_OBJECT_SET_H_
#include "2d/sceneobject/SceneObjectSet.h"
#endif

#ifndef _SCENE_OBJECT_GROUP_H_
#include "2d/sceneobject/SceneObjectGroup.h"
#endif

#ifndef _LEVELBUILDERBASETOOL_H_
#include "editor/levelBuilderBaseTool.h"
#endif

#ifndef _LEVELBUILDERSCENEWINDOW_H_
#include "editor/levelBuilderSceneWindow.h"
#endif

#ifndef _UNDO_H_
#include "collection/undo.h"
#endif

typedef SimObjectPtr<LevelBuilderBaseTool> ToolPtr;

//-----------------------------------------------------------------------------
// LevelBuilderSceneEdit
//-----------------------------------------------------------------------------
class LevelBuilderSceneEdit : public SimObject
{
   typedef SimObject Parent;

private:
   // Tool Info
   ToolPtr mActiveTool;
   ToolPtr mDefaultTool;
   SimSet mTools;

   // Properties
   F32                     mGridSnapX;
   F32                     mGridSnapY;
   bool                    mGridVisible;
   bool                    mSnapToGridX;
   bool                    mSnapToGridY;
   ColorF                  mGridColor;
   ColorF                  mFillColor;
   F32                     mSnapThreshold;
   bool                    mRotationSnap;
   F32                     mRotationSnapThreshold;
   F32                     mRotationSnapAngle;
   bool                    mGuidesVisible;
   bool                    mCameraVisible;

   // Guides
   Vector<F32>             mXGuides;
   Vector<F32>             mYGuides;

   Point2I                 mDesignResolution;

   // State Info
   Vector2               mCameraPosition;
   Vector2               mMousePosition;
   F32                     mCameraZoom;

   // The Undo Manager
   UndoManager             mUndoManager;

   // This holds all deleted objects so the deletion can be undone.
   struct RecycledObject
   {
      RecycledObject(SimObject* _object, SimSet* _group) { object = _object; group = _group; }
      SimSet* group;
      SimObject* object;
   };

protected:
   // Acquired Objects
   SceneObjectSet       mAcquiredObjects;
   SceneObjectGroup*    mAcquiredGroup;

   // The Last Window Events Were Received From
   LevelBuilderSceneWindow* mLastWindow;
  
public:
   LevelBuilderSceneEdit();
   virtual ~LevelBuilderSceneEdit();

   Vector<RecycledObject>  mRecycleBin;

   // SimObject Overrides
   virtual bool onAdd();
   virtual void onRemove();

   // Scene Window Management
   void setLastWindow(LevelBuilderSceneWindow* sceneWindow) { mLastWindow = sceneWindow; };
   inline LevelBuilderSceneWindow* getLastWindow() { return mLastWindow; };

   // Undo Manager
   UndoManager& getUndoManager() { return mUndoManager; };
   void moveToRecycleBin(SimObject* object);
   void moveFromRecycleBin(SimObject* object);
   void onDeleteNotify( SimObject* object );
   bool isRecycled(SimObject* object);
   void undo();
   void redo();

   // Tool Management
   ToolPtr getActiveTool() const { return mActiveTool; };
   bool    setActiveTool(ToolPtr tool);
   void	   clearActiveTool(void);
   bool    addTool(ToolPtr tool, bool setDefault = false);
   bool    removeTool(ToolPtr tool);

   // Default Tool
   ToolPtr getDefaultTool() const { return mDefaultTool; };
   void    setDefaultTool(ToolPtr tool);
   bool    setDefaultToolActive();

   // Object Acquisition
   void acquireObject(SceneObject* object);
   void acquireObject(SceneObjectGroup* pGroup);
   void requestAcquisition(SceneObject* obj);
   void requestAcquisition(SceneObjectGroup* pGroup);
   void clearAcquisition(SceneObject* object = NULL);

   // Acquired Object Management
   inline SceneObject*      getAcquiredObject(S32 index = 0) const { return (SceneObject*)mAcquiredObjects.at(index); };
   inline SceneObjectSet&   getAcquiredObjects()                   { return mAcquiredObjects; }
   inline SceneObjectGroup* getAcquiredGroup() const               { return mAcquiredGroup; };
   inline S32                  getAcquiredObjectCount() const         { return mAcquiredObjects.size(); }
   inline bool                 hasAcquiredObjects() const             { return mAcquiredObjects.size() > 0; }
   bool                        isAcquired(const SceneObject* object) const;
   bool                        isAcquired(const SceneObjectGroup* group) const;
   bool                        isOnlyAcquired(const SceneObjectGroup* group) const;
   bool                        containsAllAcquiredObjects(const SceneObjectGroup* group) const;
   void                        deleteAcquiredObjects();
   void                        groupAcquiredObjects();
   void                        addObjectsToGroup(SceneObjectGroup* checkGroup, Vector<SimObject*>& objectGroup);
   void                        breakApartAcquiredObjects();
   void                        updateAcquiredObjects();
    void                        onObjectChanged();
    void                        onObjectSpatialChanged();
   void                        onObjectChanged(SceneObject* object);
   void                        onObjectSpatialChanged(SceneObject* object);

   // Scene Window Events
   virtual bool onMouseEvent(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus& mouseStatus);
   virtual bool onKeyDown(LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event);
   virtual bool onKeyRepeat(LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event);
   virtual bool onKeyUp(LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event);
   virtual void onRenderBackground(LevelBuilderSceneWindow* sceneWindow);
   virtual void onRenderForeground(LevelBuilderSceneWindow* sceneWindow);

   // Design Resolution
   inline void    setDesignResolution(Point2I res) { mDesignResolution = res; };
   inline Point2I getDesignResolution()            { return mDesignResolution; };

   // Property Accessors
   inline ColorI getGridColor() const             { return mGridColor; };
   inline ColorI getFillColor() const             { return mFillColor; };
   inline bool   getGridVisibility() const        { return mGridVisible; }
   inline bool   getSnapToGridX() const           { return mSnapToGridX; };
   inline bool   getSnapToGridY() const           { return mSnapToGridY; };
   inline bool   isGridSnapX() const              { return (mGridSnapX != 0.0f); }
   inline bool   isGridSnapY() const              { return (mGridSnapY != 0.0f); }
   inline F32    getGridSnapX() const             { return mGridSnapX; }
   inline F32    getGridSnapY() const             { return mGridSnapY; }
   inline F32    getSnapThreshold() const         { return mSnapThreshold; };
   inline F32    getRotationSnapThreshold() const { return mRotationSnapThreshold; };
   inline bool   getRotationSnap() const          { return mRotationSnap; };
   inline F32    getRotationSnapAngle() const     { return mRotationSnapAngle; };
   inline bool   getCameraVisibility() const      { return mCameraVisible; };
   inline bool   getGuidesVisibility() const      { return mGuidesVisible; };

   // Property Setting
   inline void setGridColor(ColorI gridColor)      { mGridColor = gridColor; };
   inline void setFillColor(ColorI fillColor)      { mFillColor = fillColor; };
   inline void setGridVisibility(bool bVisible)    { mGridVisible = bVisible; }
   inline void setSnapToGridX(bool bSnap)          { mSnapToGridX = bSnap; };
   inline void setSnapToGridY(bool bSnap)          { mSnapToGridY = bSnap; };
   inline void setGridSnapX(F32 fSnap)             { mGridSnapX = fSnap;}
   inline void setGridSnapY(F32 fSnap)             { mGridSnapY = fSnap;}
   inline void setSnapThreshold(F32 value)         { mSnapThreshold = value; };
   inline void setRotationSnap(bool value)         { mRotationSnap = value; };
   inline void setRotationSnapAngle(F32 value)     { mRotationSnapAngle = value; };
   inline void setRotationSnapThreshold(F32 value) { mRotationSnapThreshold = value; };
   inline void setCameraVisibility(bool visible)   { mCameraVisible = visible; };
   inline void setGuidesVisibility(bool visible)   { mGuidesVisible = visible; };

   // Guide Management
   void addXGuide( F32 x ) { mXGuides.push_back( x ); };
   void addYGuide( F32 y ) { mYGuides.push_back( y ); };
   void removeXGuide( F32 x );
   void removeYGuide( F32 y );
   bool hasXGuides() { return !mXGuides.empty(); };
   bool hasYGuides() { return !mYGuides.empty(); };
   F32 getClosestXGuide( F32 x );
   F32 getClosestYGuide( F32 y );

   // State Accessors
   inline Vector2 getMousePosition()          { return mMousePosition; };
   inline Vector2 getCameraPosition()         { return mCameraPosition; };
   inline F32       getCameraZoom()             { return mCameraZoom; };

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderSceneEdit);
};

// Undo Action Types
class UndoDeleteAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   struct UndoObject
   {
      UndoObject(SceneObject* _object, bool _wasAcquired) { object = _object; wasAcquired = _wasAcquired; group = _object->getSceneObjectGroup(); };
      SceneObject* object;
      SceneObjectGroup* group;
      bool wasAcquired;
   };

   Vector<UndoObject> mObjects;

   // We need this so we can send notifications of objects changing.
   LevelBuilderSceneEdit* mSceneEdit;

public:
   UndoDeleteAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* name) : UndoAction(name) { mSceneEdit = sceneEdit; };

   void addObject(SceneObject* object, bool wasAcquired) { mObjects.push_back(UndoObject(object, wasAcquired)); deleteNotify(object); };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if (itr->object == object)
         {
            mObjects.erase_fast(itr);
            break;
         }
      }

      if (mObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         SceneObject* object = mObjects[i].object;
         if (mSceneEdit->isRecycled(mObjects[i].group))
            mSceneEdit->moveFromRecycleBin(mObjects[i].group);

         mSceneEdit->moveFromRecycleBin(object);
         if (mObjects[i].wasAcquired)
            mSceneEdit->acquireObject(object);
      }

      Con::executef(1, "refreshTreeView");

      // Call undo on quiet sub actions [KNM | 08/10/11 | ITGB-152]
      UndoAction::undo();
   };

   virtual void redo()
   {
      // Call redo on quiet sub actions [KNM | 08/10/11 | ITGB-152]
      UndoAction::redo();

      for (S32 i = 0; i < mObjects.size(); i++)
      {
         SceneObject* object = mObjects[i].object;
         if (mSceneEdit->isAcquired(object))
            mSceneEdit->clearAcquisition(object);

         mSceneEdit->moveToRecycleBin(object);

         if (mObjects[i].group && mObjects[i].group->empty())
            mSceneEdit->moveToRecycleBin(mObjects[i].group);
      }

      Con::executef(1, "refreshTreeView");
   }
};

class UndoGroupAction : public UndoAction
{
   struct GroupedObject
   {
      GroupedObject(SimSet* _oldGroup, SimObject* _object) { oldGroup = _oldGroup; object = _object; };
      SimSet* oldGroup;
      SimObject* object;
   };

   Vector<GroupedObject> mObjects;
   SimSet* mOldGroup;
   SceneObjectGroup* mGroup;

   LevelBuilderSceneEdit* mSceneEdit;

public:

   UndoGroupAction(LevelBuilderSceneEdit* sceneEdit, SceneObjectGroup* group, const UTF8* actionName) : UndoAction(actionName)
   {
      mSceneEdit = sceneEdit;
      mGroup = group;
   };

   void addObject(SimObject* object)
   {
      SimSet* set = SceneObjectGroup::getSceneObjectGroup(object);
      if (!set) set = SceneObjectGroup::getScene(object);
      mObjects.push_back(GroupedObject(set, object));

      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<GroupedObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if (itr->object == object)
         {
            mObjects.erase_fast(itr);
            break;
         }
      }

      if (mObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         mGroup->removeObject(mObjects[i].object);
         mObjects[i].oldGroup->addObject(mObjects[i].object);
      }
      mSceneEdit->moveToRecycleBin(mGroup);

      Con::executef(1, "refreshTreeView");
   };

   virtual void redo()
   {
      mSceneEdit->moveFromRecycleBin(mGroup);
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         mObjects[i].oldGroup->removeObject(mObjects[i].object);
         mGroup->addObject(mObjects[i].object);
      }

      Con::executef(1, "refreshTreeView");
   };
};

class UndoBreakApartAction : public UndoAction
{
   struct GroupedObject
   {
      GroupedObject(SimSet* _oldGroup, SimSet* _newGroup, SimObject* _object) { oldGroup = _oldGroup; newGroup = _newGroup; object = _object; };
      SimSet* oldGroup;
      SimSet* newGroup;
      SimObject* object;
   };

   Vector<GroupedObject> mObjects;
   Vector<SceneObjectGroup*> mRecycledGroups;
   LevelBuilderSceneEdit* mSceneEdit;

public:

   UndoBreakApartAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* actionName) : UndoAction(actionName)
   {
      mSceneEdit = sceneEdit;
   };

   void addObject(SimObject* object, SimSet* oldGroup, SimSet* newGroup)
   {
      mObjects.push_back(GroupedObject(oldGroup, newGroup, object));
      deleteNotify(object);
   };

   void addRecycledGroup(SceneObjectGroup* group)
   {
      mRecycledGroups.push_back(group);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<GroupedObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if (itr->object == object)
         {
            mObjects.erase_fast(itr);
            break;
         }
      }

      if (mObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      for (S32 i = 0; i < mRecycledGroups.size(); i++)
         mSceneEdit->moveFromRecycleBin(mRecycledGroups[i]);

      for (S32 i = 0; i < mObjects.size(); i++)
      {
         mObjects[i].newGroup->removeObject(mObjects[i].object);
         mObjects[i].oldGroup->addObject(mObjects[i].object);
      }

      Con::executef(1, "refreshTreeView");
   };

   virtual void redo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         mObjects[i].oldGroup->removeObject(mObjects[i].object);
         mObjects[i].newGroup->addObject(mObjects[i].object);
      }

      for (S32 i = 0; i < mRecycledGroups.size(); i++)
         mSceneEdit->moveToRecycleBin(mRecycledGroups[i]);

      Con::executef(1, "refreshTreeView");
   };
};

#endif


#endif // TORQUE_TOOLS
