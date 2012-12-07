//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERMOUSESCRIPTTOOL_H_
#define _LEVELBUILDERMOUSESCRIPTTOOL_H_

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#include "editor/levelBuilderBaseEditTool.h"
#endif
#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif
#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderTDTerrainTool
//-----------------------------------------------------------------------------
class LevelBuilderMouseScriptTool : public LevelBuilderBaseEditTool
{
   typedef LevelBuilderBaseEditTool Parent;
private:
   F32 mMouseDownAR;

protected:
   bool mMoving;

   LevelBuilderSceneWindow* mSceneWindow;

   bool isEditable(SceneObject* object) { return true; };

public:
   LevelBuilderMouseScriptTool();
   ~LevelBuilderMouseScriptTool();
   
   virtual bool onAdd();
   virtual void onRemove();

	virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
	virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
   virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );

   // Base Tool Overrides
   virtual bool onActivate(LevelBuilderSceneWindow* sceneWindow);
   virtual void onDeactivate();
   virtual bool onAcquireObject(SceneObject* object);
   virtual void onRelinquishObject(SceneObject* object);

   void onRenderScene( LevelBuilderSceneWindow* sceneWindow );

   /// End this tool and go back to the default tool
   void endTool();

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderMouseScriptTool);
};

#endif


#endif // TORQUE_TOOLS
