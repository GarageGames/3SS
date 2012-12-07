//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#define _LEVELBUILDERBASEEDITTOOL_H_

#ifndef _LEVELBUILDERBASETOOL_H_
#include "editor/levelBuilderBaseTool.h"
#endif

#ifndef _LEVELBUILDERSCENEEDIT_H_
#include "editor/levelBuilderSceneEdit.h"
#endif

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------
// LevelBuilderBaseEditTool
// This tool provides common editing functionality like scaling or rotating.
//-----------------------------------------------------------------------------
class LevelBuilderBaseEditTool : public LevelBuilderBaseTool
{
protected:
   typedef LevelBuilderBaseTool Parent;

   // Sizing States
   enum objectSizingModes
   {
      SizingNone = 0,
      SizingLeft = 1,
      SizingRight = 2,
      SizingTop = 4,
      SizingBottom = 8
   };

   U32 mSizingState;
   
   virtual void nudge(Vector2 pos, S32 directionX, S32 directionY, bool fast, Vector2& newPos);
   virtual void move(LevelBuilderSceneEdit* sceneEdit, Vector2 size, Vector2 mousePoint2D, Vector2& finalPosition);
   virtual void rotate(LevelBuilderSceneEdit* sceneEdit, F32 rotation, Vector2 rotationVector, Vector2 newVector, F32& newRotation);
   virtual void scale(LevelBuilderSceneEdit* sceneEdit, Vector2 size, Vector2 pos, Vector2 mousePoint2D, bool uniform, bool maintainAR, F32 ar,
                      Vector2& newSize, Vector2& newPosition, bool& flipX, bool& flipY);
  
public:
   LevelBuilderBaseEditTool();
   virtual ~LevelBuilderBaseEditTool();

   virtual void drawSizingNuts(LevelBuilderSceneWindow* sceneWindow, const RectF& rect);
   virtual S32  getSizingState(LevelBuilderSceneWindow* sceneWindow, const Point2I &pt, const RectF &rect);

   // Declare our Console Object
   DECLARE_CONOBJECT(LevelBuilderBaseEditTool);
};

#endif

#endif // TORQUE_TOOLS
