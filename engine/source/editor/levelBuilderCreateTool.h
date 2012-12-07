//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERCREATETOOL_H_
#define _LEVELBUILDERCREATETOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#include "editor/levelBuilderBaseEditTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderCreateTool
// Provides standard functionality for object creation.
//-----------------------------------------------------------------------------
class LevelBuilderCreateTool : public LevelBuilderBaseEditTool
{
private:
   bool              mObjectHidden;
   ColorI            mOutlineColor;

   // The mouse drag start position after snapping.
   Vector2 mDragStart;
   F32 mMouseDownAR;

protected:
   typedef LevelBuilderBaseEditTool Parent;
   
   SceneObject*   mCreatedObject;
   StringTableEntry  mScriptClassName;
   StringTableEntry  mScriptSuperClassName;

   // Must be defined by the derived class though they can't be pure virtual since this
   // is a console object.
   virtual SceneObject* createObject() { return NULL; };
   virtual void showObject() { if (mCreatedObject) mCreatedObject->setVisible(true); };
   virtual Vector2 getDefaultSize(LevelBuilderSceneWindow* sceneWindow);
   virtual Point2I getPixelSize() { return Point2I(128, 128); };

   // Properties
   bool mAcquireCreatedObjects;
  
public:
   LevelBuilderCreateTool();
   ~LevelBuilderCreateTool();
   
    virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   void onRenderScene( LevelBuilderSceneWindow* sceneWindow );

   bool create(LevelBuilderSceneWindow* sceneWindow);
   SceneObject* createFull(LevelBuilderSceneWindow* sceneWindow, Vector2 position);

   virtual void onObjectCreated();

   // Property Accessors
   void setAcquireCreatedObjects(bool val) { mAcquireCreatedObjects = val; };
   bool getAcquireCreatedObjects()         { return mAcquireCreatedObjects; };

   void setConfigDatablock( const char* datablockName );
   void setClassNamespace( const char* classNamespace );
   void setSuperClassNamespace( const char* superClassNamespace );

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderCreateTool);
};

// Undo Action
class UndoCreateAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   SceneObject* mObject;
   bool mWasAcquired;
   LevelBuilderSceneEdit* mSceneEdit;

public:
   UndoCreateAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* name) : UndoAction(name) { mSceneEdit = sceneEdit; };
   void addObject(SceneObject* object)
   {
      mObject = object;
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      if (mUndoManager)
         mUndoManager->removeAction(this);
   };

   virtual void undo()
   {
      if (!mObject) return;
      if (mSceneEdit->isAcquired(mObject))
      {
         mWasAcquired = true;
         mSceneEdit->clearAcquisition(mObject);
      }
      else
         mWasAcquired = false;

      mSceneEdit->moveToRecycleBin(mObject);
   }

   virtual void redo()
   {
      if (!mObject) return;
      mSceneEdit->moveFromRecycleBin(mObject);
      if (mWasAcquired)
         mSceneEdit->requestAcquisition(mObject);
   }
};

#endif


#endif // TORQUE_TOOLS
