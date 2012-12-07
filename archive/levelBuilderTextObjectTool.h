//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERTEXTOBJECTTOOL_H_
#define _LEVELBUILDERTEXTOBJECTTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/SceneObject.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#include "2d/TextObject.h"

//-----------------------------------------------------------------------------
// LevelBuilderTextObjectTool
//-----------------------------------------------------------------------------
class LevelBuilderTextObjectTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry mDefaultFont;
   S32 mDefaultSize;
   S32 mDefaultHeight;
   TextObject::TextAlign mDefaultAlignment;
   bool mDefaultWordWrap;
   bool mDefaultClipText;
   bool mDefaultAutoSize;
   F32 mDefaultAspectRatio;
   F32 mDefaultLineSpacing;
   F32 mDefaultCharacterSpacing;
   StringBuffer mDefaultText;

protected:
   virtual SceneObject* createObject();
   virtual void showObject();
   virtual Vector2 getDefaultSize(LevelBuilderSceneWindow* sceneWindow);
  
public:
   LevelBuilderTextObjectTool();
   ~LevelBuilderTextObjectTool();

   virtual void onObjectCreated();

   void setFontDBName( const char* name);

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderTextObjectTool);
};

#endif


#endif // TORQUE_TOOLS
