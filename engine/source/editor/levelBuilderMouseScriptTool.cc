//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "graphics/dgl.h"
#include "editor/levelBuilderMouseScriptTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderMouseScriptTool);

LevelBuilderMouseScriptTool::LevelBuilderMouseScriptTool() : LevelBuilderBaseEditTool(),
                                                         mSceneWindow(NULL)
{
   // Set our tool name
   mToolName = StringTable->insert("Tower Defense Terrain Tool");
}

LevelBuilderMouseScriptTool::~LevelBuilderMouseScriptTool()
{
}

bool LevelBuilderMouseScriptTool::onAdd()
{
   if (!Parent::onAdd())
      return false;

   return true;
}

void LevelBuilderMouseScriptTool::onRemove()
{
   Parent::onRemove();
}

bool LevelBuilderMouseScriptTool::onActivate(LevelBuilderSceneWindow* sceneWindow)
{
   if (!Parent::onActivate(sceneWindow))
      return false;

   mSceneWindow = sceneWindow;

   return true;
}

void LevelBuilderMouseScriptTool::onDeactivate()
{
   mSceneWindow = NULL;
   Parent::onDeactivate();
}

bool LevelBuilderMouseScriptTool::onAcquireObject(SceneObject* object)
{
   if(!mSceneWindow)
      return false;

   if (!Parent::onAcquireObject(object))
      return false;

   return true;
}

void LevelBuilderMouseScriptTool::onRelinquishObject(SceneObject* object)
{
   if(!mSceneWindow)
      return Parent::onRelinquishObject(object);

   // Do parent cleanup
   Parent::onRelinquishObject(object);
}

bool LevelBuilderMouseScriptTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (sceneWindow != mSceneWindow)
      return Parent::onMouseDragged(sceneWindow, mouseStatus);

   Con::executef(this, 3, "onMouseDown", Con::getFloatArg(mouseStatus.mousePoint2D.x), Con::getFloatArg(mouseStatus.mousePoint2D.y));

   return true;
}

bool LevelBuilderMouseScriptTool::onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (sceneWindow != mSceneWindow)
      return Parent::onMouseDragged(sceneWindow, mouseStatus);

   Con::executef(this, 3, "onMouseDragged", Con::getFloatArg(mouseStatus.mousePoint2D.x), Con::getFloatArg(mouseStatus.mousePoint2D.y));

   return true;
}

bool LevelBuilderMouseScriptTool::onMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus& mouseStatus)
{
   if (sceneWindow != mSceneWindow)
      return Parent::onMouseUp(sceneWindow, mouseStatus);

   Con::executef(this, 3, "onMouseUp", Con::getFloatArg(mouseStatus.mousePoint2D.x), Con::getFloatArg(mouseStatus.mousePoint2D.y));

   return false;
}

void LevelBuilderMouseScriptTool::onRenderScene(LevelBuilderSceneWindow* sceneWindow)
{ 
   // Render Parent first
   Parent::onRenderScene( sceneWindow );

   if (sceneWindow != mSceneWindow)
      return;
}

void LevelBuilderMouseScriptTool::endTool()
{
   if (!mSceneWindow)
      return;

   LevelBuilderSceneEdit* owner = mSceneWindow->getSceneEdit();
   AssertFatal( owner, "LevelBuilderTDTerrainTool::endTool - Scene Window does not have an owner." );

   owner->setDefaultToolActive();
}

ConsoleMethod(LevelBuilderMouseScriptTool, endTool, void, 2, 2, "End this tool and go back to the default tool.")
{
   object->endTool();
}

#endif // TORQUE_TOOLS
