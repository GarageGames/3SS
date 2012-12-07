//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#ifndef _LEVELBUILDERBASETOOL_H_
#define _LEVELBUILDERBASETOOL_H_

#ifndef _SIMBASE_H_
#include "sim/simBase.h"
#endif

#ifndef _SCENE_H_
#include "2d/scene/Scene.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _LEVELBUILDERSCENEWINDOW_H_
#include "editor/levelBuilderSceneWindow.h"
#endif

#ifndef _UNDO_H_
#include "collection/undo.h"
#endif

#ifndef _PROFILER_H_
#include "debug/profiler.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderBaseTool
//-----------------------------------------------------------------------------
class LevelBuilderBaseTool : public SimObject
{
   typedef SimObject Parent;

protected:
   // Tool Status
   bool mActive;
   StringTableEntry mToolName;
   StringTableEntry mTextureName;

   // Nut Properties
   Vector<Point2F>   mNutList;
   ColorI            mNutColor;
   ColorI            mNutOutlineColor;
   S32               mNutSize;
   
   UndoManager mUndoManager;

   // Nut Handling
   bool inNut(Point2I pt, S32 x, S32 y);
   void drawNut(Point2I position);
   void drawArrowNut(Point2I position);

public:
   LevelBuilderBaseTool();
   ~LevelBuilderBaseTool(); 
  
   virtual bool hasUndoManager() { return false; };
   const UndoManager& getUndoManager() { return mUndoManager; };

   /// Acquired Object Callbacks
   /// 
   /// onAcquireObject is called when an object is acquired by the SGEC that owns this tool.
   /// There is a required return of true or false indicating whether our not the tool wants
   /// the object and acquired it properly.
   virtual bool onAcquireObject(SceneObject* object);

   /// onRelinquishObject is called when the edit window that owns this tool loses acquisition of
   /// an object.
   virtual void onRelinquishObject(SceneObject* object);
   
	/// Acquired Object Mouse events
   ///
   /// When this tool is active the edit window that owns it will dispatch mouse events to it.
   /// Returning true from these functions indicates that the event was used and should
   /// not be passed along to the containing window.
   virtual bool onMouseMove(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)          { return false; };
   virtual bool onMouseDown(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)          { return false; };
   virtual bool onMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)            { return false; };
   virtual bool onMouseDragged(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)       { return false; };
   virtual bool onRightMouseDown(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)     { return false; };
   virtual bool onRightMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)       { return false; };
   virtual bool onRightMouseDragged(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)  { return false; };
   virtual bool onMiddleMouseDown(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)    { return false; };
   virtual bool onMiddleMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)      { return false; };
   virtual bool onMiddleMouseDragged(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus) { return false; };
   virtual bool onMouseWheelUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)       { return false; };
   virtual bool onMouseWheelDown(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)     { return false; };

   /// Acquired Object Key Events
   /// By default these functions return false which indicates that they were not handled by this tool
   /// and should be propogated to another handler for handling [11/4/2006 justind]
   virtual bool onKeyDown(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event)   { return false; };
   virtual bool onKeyUp(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event)     { return false; };
   virtual bool onKeyRepeat(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event) { return false; };

   /// onRenderScene is called by the edit window when this tool is active.
   virtual void onRenderScene( LevelBuilderSceneWindow* sceneWindow ) { };

   /// onActivate is called when a tool is selected as the active tool for an edit window. After
   /// this is called, the tool will begin to receive input events and will be given the
   /// opportunity to do some rendering.
   virtual bool onActivate(LevelBuilderSceneWindow* sceneWindow);

   /// onDeactivate is called when a tool is no longer selected as the active editing tool.
   virtual void onDeactivate();

   /// isActive returns true if our tool is active or false if it is not.
   inline bool isActive() const { return mActive; };

   /// Returns the name of this tool.
   inline StringTableEntry getToolName() const { return mToolName; };

   /// Returns the texture name (no path) of this tool's icon texture.
   inline StringTableEntry getToolTexture() const { return mTextureName; };

   /// Sets the name of this tool.
   inline void setToolName( StringTableEntry toolName ) { if( toolName != NULL ) mToolName = StringTable->insert(toolName); };

   /// Sets the texture name (no path) of this tool's icon texture.
   inline void setToolTexture( StringTableEntry toolTexture ) { if( toolTexture != NULL ) mTextureName = StringTable->insert(toolTexture); };

   // Parent Overrides
   bool onAdd();

   virtual bool undo() { return false; }
   virtual bool redo() { return false; }

   // Declare Console Object
   DECLARE_CONOBJECT(LevelBuilderBaseTool);
};

#endif


#endif // TORQUE_TOOLS
