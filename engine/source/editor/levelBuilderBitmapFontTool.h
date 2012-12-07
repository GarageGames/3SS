//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LevelBuilderBitmapFontTool_H_
#define _LevelBuilderBitmapFontTool_H_

#include "string/stringBuffer.h"

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _LEVELBUILDERCREATETOOL_H_
#include "editor/levelBuilderCreateTool.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

#include "2d/sceneobject/BitmapFontObject.h"

//-----------------------------------------------------------------------------
// LevelBuilderBitmapFontTool
//-----------------------------------------------------------------------------
class LevelBuilderBitmapFontTool : public LevelBuilderCreateTool
{
   typedef LevelBuilderCreateTool Parent;

private:
   StringTableEntry  mImageMapName;
   StringBuffer mText;

protected:
   virtual SceneObject* createObject();
   virtual Point2I getPixelSize();
  
public:
   LevelBuilderBitmapFontTool();
   ~LevelBuilderBitmapFontTool();

   void setImageMapName( const char* name );
   void setText( const char* text );

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderBitmapFontTool);
};

#endif


#endif // TORQUE_TOOLS
