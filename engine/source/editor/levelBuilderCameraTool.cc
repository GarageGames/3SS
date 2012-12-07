//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "graphics/dgl.h"
#include "editor/levelBuilderCameraTool.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderCameraTool);

LevelBuilderCameraTool::LevelBuilderCameraTool() : LevelBuilderBaseEditTool(),
                                                   mCameraPosition(0.0f, 0.0f),
                                                   mCameraSize(100.0f, 75.0f),
                                                   mCameraOutlineColor( 255, 255, 255, 200 ),
                                                   mCameraFillColor( 40, 40, 40, 150),
                                                   mSceneWindow(NULL),
                                                   mUndoFullAction(NULL),
                                                   mUndoAction(NULL),
                                                   mMouseDownAR( 1.0f )
{
   // Set our tool name
   mToolName = StringTable->insert("Camera Tool");
}

LevelBuilderCameraTool::~LevelBuilderCameraTool()
{
}

bool LevelBuilderCameraTool::onAdd()
{
   if (!Parent::onAdd())
      return false;

   if (!mUndoManager.registerObject())
      return false;

   mUndoManager.setModDynamicFields(true);
   mUndoManager.setModStaticFields(true);

   return true;
}

void LevelBuilderCameraTool::onRemove()
{
   mUndoManager.unregisterObject();

   Parent::onRemove();
}

bool LevelBuilderCameraTool::onActivate(LevelBuilderSceneWindow* sceneWindow)
{
   if (!Parent::onActivate(sceneWindow))
      return false;

   mSceneWindow = sceneWindow;

   if (sceneWindow->getScene())
   {
      const char* pos = sceneWindow->getScene()->getDataField(StringTable->insert("cameraPosition"), NULL);
      if (Utility::mGetStringElementCount(pos) == 2)
         mCameraPosition = Utility::mGetStringElementVector(pos);
      
      const char* size = sceneWindow->getScene()->getDataField(StringTable->insert("cameraSize"), NULL);
      if (Utility::mGetStringElementCount(size) == 2)
         mCameraSize = Utility::mGetStringElementVector(size);
   }

   mUndoFullAction = new UndoFullCameraAction(this, sceneWindow->getScene(), "Camera Change");
   mUndoFullAction->setStartBounds(mCameraPosition, mCameraSize);

   mStartPos = mCameraPosition;
   mStartSize = mCameraSize;

   mCameraZoom = sceneWindow->getCurrentCameraZoom();
   mCameraArea = sceneWindow->getCurrentCameraArea();

   sceneWindow->setTargetCameraZoom(1.0f);
   sceneWindow->setTargetCameraPosition(mCameraPosition, mCameraSize.x * 2.0f, mCameraSize.y * 2.0f);
   sceneWindow->startCameraMove( 0.5f );

   return true;
}

void LevelBuilderCameraTool::onDeactivate()
{
   if (!mSceneWindow)
      return Parent::onDeactivate();

   if (mSceneWindow->getScene())
   {
      char pos[32];
      dSprintf(pos, 32, "%g %g", mCameraPosition.x, mCameraPosition.y);
      mSceneWindow->getScene()->setDataField(StringTable->insert("cameraPosition"), NULL, pos);

      char size[32];
      dSprintf(size, 32, "%g %g", mCameraSize.x, mCameraSize.y);
      mSceneWindow->getScene()->setDataField(StringTable->insert("cameraSize"), NULL, size);

      if( mSceneWindow->getSceneEdit() )
      {
         char oldPos[32];
         dSprintf(oldPos, 32, "%g %g", mStartPos.x, mStartPos.y);

         char oldSize[32];
         dSprintf(oldSize, 32, "%g %g", mStartSize.x, mStartSize.y);

         Con::executef( mSceneWindow->getSceneEdit(), 5, "onCameraChanged", oldPos, oldSize, pos, size );
      }
   }
   
   if (mUndoFullAction)
   {
      mUndoFullAction->setFinishBounds(mCameraPosition, mCameraSize);
      if (mUndoFullAction->hasChanged())
         mUndoFullAction->addToManager(&mSceneWindow->getSceneEdit()->getUndoManager());
      else
         delete mUndoFullAction;
   }
   
   mSceneWindow->setTargetCameraZoom( mCameraZoom );
   mSceneWindow->setTargetCameraPosition( mCameraPosition, mCameraSize.x, mCameraSize.y );
   mSceneWindow->startCameraMove( 0.5f );

   Parent::onDeactivate();

   mSceneWindow = NULL;

   mUndoManager.clearAll();

   mUndoAction = NULL;
   mUndoFullAction = NULL;
}

bool LevelBuilderCameraTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (sceneWindow != mSceneWindow)
      return Parent::onMouseDragged(sceneWindow, mouseStatus);

   Vector2 upperLeft = mCameraPosition - (mCameraSize * 0.5);
   Vector2 lowerRight = mCameraPosition + (mCameraSize * 0.5);

   mSizingState = getSizingState( sceneWindow, mouseStatus.event.mousePoint, RectF(upperLeft, mCameraSize));
   mMouseDownAR = mCameraSize.x / mCameraSize.y;

   mMoving = false;
   if (!mSizingState)
   {
      if ((mouseStatus.mousePoint2D.x > upperLeft.x) && (mouseStatus.mousePoint2D.x < lowerRight.x) &&
          (mouseStatus.mousePoint2D.y > upperLeft.y) && (mouseStatus.mousePoint2D.y < lowerRight.y))
      {
          mMoving = true;
          mOffset = mouseStatus.mousePoint2D - (upperLeft + ((lowerRight - upperLeft) * 0.5));
      }
   }

   mUndoAction = new UndoCameraAction(this, "Camera Change");
   mUndoAction->setStartBounds(mCameraPosition, mCameraSize);

   return true;
}

bool LevelBuilderCameraTool::onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if ((sceneWindow != mSceneWindow) || !sceneWindow->getSceneEdit())
      return Parent::onMouseDragged(sceneWindow, mouseStatus);

   bool flipX, flipY;
   if (mSizingState)
      scale(sceneWindow->getSceneEdit(), mCameraSize, mCameraPosition, mouseStatus.mousePoint2D, !(mouseStatus.event.modifier & SI_CTRL),
            !(mouseStatus.event.modifier & SI_SHIFT), mMouseDownAR, mCameraSize, mCameraPosition, flipX, flipY);
   else if (mMoving)
      move(sceneWindow->getSceneEdit(), mCameraSize, mouseStatus.mousePoint2D - mOffset, mCameraPosition);

   char position[64];
   dSprintf(position, 64, "%g %g", mCameraPosition.x, mCameraPosition.y);
   char size[64];
   dSprintf(size, 64, "%g %g", mCameraSize.x, mCameraSize.y);
   Con::executef(this, 3, "onCameraChanged", position, size);
   return true;
}

bool LevelBuilderCameraTool::onMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus& mouseStatus)
{
   if (mUndoAction)
   {
      mUndoAction->setFinishBounds(mCameraPosition, mCameraSize);
      if (mUndoAction->hasChanged())
         mUndoAction->addToManager(&mUndoManager);
      else
         delete mUndoAction;
   }

   mUndoAction = NULL;

   return false;
}

void LevelBuilderCameraTool::onRenderScene(LevelBuilderSceneWindow* sceneWindow)
{
   // Render Parent first
   Parent::onRenderScene( sceneWindow );

   if (sceneWindow != mSceneWindow)
      return;

   Vector2 upperLeft = mCameraPosition - (mCameraSize * 0.5);
   Vector2 lowerRight = mCameraPosition + (mCameraSize * 0.5);
   Vector2 windowUpperLeft, windowLowerRight;
   mSceneWindow->sceneToWindowPoint(upperLeft, windowUpperLeft);
   mSceneWindow->sceneToWindowPoint(lowerRight, windowLowerRight);

   Point2I offsetUpperLeft = mSceneWindow->localToGlobalCoord(Point2I(S32(windowUpperLeft.x), S32(windowUpperLeft.y)));
   Point2I offsetLowerRight = mSceneWindow->localToGlobalCoord(Point2I(S32(windowLowerRight.x), S32(windowLowerRight.y)));

   RectI cameraRect = RectI(offsetUpperLeft, offsetLowerRight - offsetUpperLeft);

   dglDrawRect( cameraRect, mCameraOutlineColor );
   dglDrawRectFill( cameraRect, mCameraFillColor );

   drawSizingNuts(mSceneWindow, RectF(upperLeft, mCameraSize));
}

ConsoleMethod(LevelBuilderCameraTool, getCameraPosition, const char*, 2, 2, "() Get the current camera position\n"
              "@return Returns the cameras position formatted as \"x y\"")
{
   char* ret = Con::getReturnBuffer(32);
   dSprintf(ret, 32, "%s %s", object->getCameraPosition().x, object->getCameraPosition().y);
   return ret;
}

ConsoleMethod(LevelBuilderCameraTool, getCameraSize, const char*, 2, 2, "() Get the current camera size\n"
              "@return Returns the cameras size formatted as \"x y\"")
{
   char* ret = Con::getReturnBuffer(32);
   dSprintf(ret, 32, "%s %s", object->getCameraSize().x, object->getCameraSize().y);
   return ret;
}

ConsoleMethod(LevelBuilderCameraTool, setCameraPosition, void, 3, 3, "(position) Set the current camera position\n"
              "@param position Coordinates formatted as \"x y\""
              "@return No return value.")
{
    object->setCameraPosition(Utility::mGetStringElementVector(argv[2]));
}

ConsoleMethod(LevelBuilderCameraTool, setCameraSize, void, 3, 3, "(size) Set the current camera size\n"
              "@param size Coordinates formatted as \"x y\""
              "@return No return value.")
{
    object->setCameraSize(Utility::mGetStringElementVector(argv[2]));
}

#endif // TORQUE_TOOLS
