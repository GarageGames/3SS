//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#include "console/console.h"
#include "graphics/dgl.h"
#include "editor/levelBuilderSortPointTool.h"
#include "editor/levelBuilderSceneEdit.h"

IMPLEMENT_CONOBJECT(LevelBuilderSortPointTool);

LevelBuilderSortPointTool::LevelBuilderSortPointTool() : LevelBuilderBaseTool(),
                                                         mCameraArea(0.0f, 0.0f, 0.0f, 0.0f),
                                                         mAngle(0.0f),
                                                         mSceneWindow(NULL),
                                                         mSceneObject(NULL)
{
   mNutSize = 8;
   // Set our tool name
   mToolName = StringTable->insert("Sort Point Tool");
}

LevelBuilderSortPointTool::~LevelBuilderSortPointTool()
{
}

bool LevelBuilderSortPointTool::onActivate(LevelBuilderSceneWindow* sceneWindow)
{
   if (!Parent::onActivate(sceneWindow))
      return false;

   mSceneObject = NULL;
   mSceneWindow = sceneWindow;

   return true;
}

void LevelBuilderSortPointTool::onDeactivate()
{
   finishEdit();

   mSceneObject = NULL;
   mSceneWindow = NULL;
   Parent::onDeactivate();
}

bool LevelBuilderSortPointTool::onAcquireObject( SceneObject *object )
{
   if(!isEditable(object) || !mSceneWindow)
      return false;

   // Parent handling 
   if(!Parent::onAcquireObject(object)) 
      return false;
   
   if (!mSceneObject || (mSceneWindow->getToolOverride() == this))
   {
      finishEdit();
      editObject(object);
   }
   
   return true;
}

void LevelBuilderSortPointTool::onRelinquishObject( SceneObject *object )
{
   if(!mSceneWindow || !mSceneObject)
      return Parent::onRelinquishObject(object);

   if (object == mSceneObject)
   {
      finishEdit();

      if (mSceneWindow->getToolOverride() == this)
      {
         bool foundNewObject = false;
         // Since we're a tool override, we should try to edit any object we can.
         for (S32 i = 0; i < mSceneWindow->getSceneEdit()->getAcquiredObjectCount(); i++)
         {
            SceneObject* newObject = mSceneWindow->getSceneEdit()->getAcquiredObject(i);
            if ((newObject != mSceneObject) && isEditable(newObject))
            {
               foundNewObject = true;
               editObject(newObject);
               break;
            }
         }

         if (!foundNewObject)
         {
            // Grab the size and position of the camera from the scene.
            Vector2 cameraPosition = Vector2(0.0f, 0.0f);
            Vector2 cameraSize = Vector2(100.0f, 75.0f);
            if (mSceneWindow->getScene())
            {
               const char* pos = mSceneWindow->getScene()->getDataField(StringTable->insert("cameraPosition"), NULL);
               if (Utility::mGetStringElementCount(pos) == 2)
                  cameraPosition = Utility::mGetStringElementVector(pos);
               
               const char* size = mSceneWindow->getScene()->getDataField(StringTable->insert("cameraSize"), NULL);
               if (Utility::mGetStringElementCount(size) == 2)
                  cameraSize = Utility::mGetStringElementVector(size);
            }

            // And update the camera.
            mSceneWindow->setTargetCameraZoom( 1.0f );
            mSceneWindow->setTargetCameraPosition(cameraPosition, cameraSize.x, cameraSize.y);
            mSceneWindow->startCameraMove( 0.5f );
            mSceneObject = NULL;
         }
      }
   }

   // Do parent cleanup
   Parent::onRelinquishObject(object);
}

void LevelBuilderSortPointTool::editObject(SceneObject* object)
{
   if (!mSceneWindow || !isEditable(object))
      return;

   mSceneObject = object;

   // We're going to modify some things so we can get a better view on this object
   // for poly creation stuff, so let's back up their current settings
   mCameraZoom = mSceneWindow->getCurrentCameraZoom();
   mCameraArea = mSceneWindow->getCurrentCameraArea();
   RectF newArea = object->getAABBRectangle();
   newArea.inset(-1, -1);
   mSceneWindow->setTargetCameraZoom(1.0f);
   mSceneWindow->setTargetCameraArea(newArea);
   mSceneWindow->startCameraMove(0.5f);

   mAngle = object->getAngle();
   object->setAngle(0.0f);
}

ConsoleMethod(LevelBuilderSortPointTool, editObject, void, 3, 3, "Selects an object for editing.")
{
   SceneObject* obj = dynamic_cast<SceneObject*>(Sim::findObject(argv[2]));
   if (obj)
      object->editObject(obj);
   else
      Con::warnf("Invalid object passed to LevelBuilderSortPointTool::editObject");
}

void LevelBuilderSortPointTool::finishEdit()
{
   if (!mSceneObject || !mSceneWindow)
      return;

   mSceneObject->setAngle(mAngle);
   mSceneWindow->getSceneEdit()->onObjectChanged(mSceneObject);

   // Reset the camera.
   mSceneWindow->setTargetCameraZoom( mCameraZoom );
   mSceneWindow->setTargetCameraArea( mCameraArea );
   mSceneWindow->startCameraMove( 0.5f );

   mSceneObject = NULL;
}

ConsoleMethod(LevelBuilderSortPointTool, finishEdit, void, 2, 2, "Applies changes and ends editing of an object.")
{
   object->finishEdit();
}

bool LevelBuilderSortPointTool::isEditable(SceneObject* obj)
{
   return true;
}

void LevelBuilderSortPointTool::onRenderScene(LevelBuilderSceneWindow* sceneWindow )
{
   Parent::onRenderScene( sceneWindow );

   if ((mSceneWindow != sceneWindow) || !mSceneObject)
      return;

   // Draw the bounding rect.
   RectI bounds = mSceneWindow->getObjectBoundsWindow(mSceneObject);
   bounds.point = mSceneWindow->localToGlobalCoord(bounds.point);
   dglDrawRect(bounds, ColorI(255, 255, 255));

   Point2I pt = getMountPointWorld( mSceneWindow, mSceneObject, mSceneObject->getSortPoint().ToPoint2F() );
   S32 size = 8;
   drawNut( pt );

   // If this is a window's tool override, we need the window to follow the object
   // being edited.
   if ((mSceneWindow->getToolOverride() == this) && !mSceneWindow->isCameraMoving())
   {
      RectF newArea = mSceneObject->getAABBRectangle();
      newArea.inset(-1, -1);
      mSceneWindow->setCurrentCameraArea(newArea);
   }
}

bool LevelBuilderSortPointTool::onMouseMove( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return Parent::onMouseMove(sceneWindow, mouseStatus);

   return true;
}

bool LevelBuilderSortPointTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   mAddUndo = false;
   mUndoAction = NULL;

   if (!mSceneWindow || !mSceneWindow->getSceneEdit())
      return false;

   // Acquire Object
   if (!mSceneObject)
   {
      if (mouseStatus.pickList.size() == 0)
         return Parent::onMouseDown(sceneWindow, mouseStatus);

      SceneObject* pObj = mouseStatus.pickList[0].mpSceneObject;

      if ((mouseStatus.event.mouseClickCount >= 2) && isEditable(pObj))
         sceneWindow->getSceneEdit()->requestAcquisition(pObj);

      return true;
   }

   mUndoAction = new UndoSortPointMoveAction(sceneWindow->getSceneEdit(), (UTF8*)"Moved Sort Point");
   mUndoAction->setStartPosition(mSceneObject, mSceneObject->getSortPoint());
   mAddUndo = true;

   Vector2 position = getMountPointObject( mSceneWindow, mSceneObject, mSceneWindow->localToGlobalCoord(mouseStatus.event.mousePoint));
   mSceneObject->setSortPoint( position );
   
   mSceneWindow->getSceneEdit()->onObjectSpatialChanged(mSceneObject);

   return true;
}

bool LevelBuilderSortPointTool::onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (!mSceneObject || (mSceneWindow != sceneWindow) || !mSceneWindow->getSceneEdit())
      return false;

   RectI bounds = sceneWindow->getObjectBoundsWindow(mSceneObject);

   if (bounds.pointInRect(mouseStatus.event.mousePoint))
   {
      Vector2 position = getMountPointObject( mSceneWindow, mSceneObject, mSceneWindow->localToGlobalCoord(mouseStatus.event.mousePoint));
      mSceneObject->setSortPoint( position );
      
      mSceneWindow->getSceneEdit()->onObjectSpatialChanged(mSceneObject);
   }

   return true;
}

bool LevelBuilderSortPointTool::onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (!mSceneObject || (mSceneWindow != sceneWindow) || !mSceneWindow->getSceneEdit())
      return false;

   if (mAddUndo && mUndoAction->isChanged())
   {
      mUndoAction->setEndPosition(mSceneObject->getSortPoint());
      mUndoAction->addToManager( &mSceneWindow->getSceneEdit()->getUndoManager() );
   }
   else if (mUndoAction)
      delete mUndoAction;

   mAddUndo = false;
   mUndoAction = NULL;

   return true;
}

Point2F LevelBuilderSortPointTool::getMountPointObject(LevelBuilderSceneWindow* sceneWindow, const SceneObject* obj, const Point2I& worldPoint) const 
{
   Point2I localPoint = sceneWindow->globalToLocalCoord( worldPoint );
   // Get our object's bounds window
   RectI objRect = sceneWindow->getObjectBoundsWindow( obj );

   F32 nWidthInverse  = 1.0f / (F32)objRect.extent.x;
   F32 nHeightInverse = 1.0f / (F32)objRect.extent.y;

   S32 positionY = localPoint.y - objRect.point.y;
   if( positionY < 0 || positionY > objRect.extent.y  )
   {
      return Point2F(0,0);
   }

   S32 positionX = localPoint.x - objRect.point.x;
   if( positionX < 0 || positionX > objRect.extent.x ) 
   {
      return Point2F(0,0);
   }

   return Point2F( ( (F32)positionX * nWidthInverse  * 2.0f - 1.0f ), 
                   ( (F32)positionY * nHeightInverse * 2.0f - 1.0f) );
}

Point2I LevelBuilderSortPointTool::getMountPointWorld(LevelBuilderSceneWindow* sceneWindow, const SceneObject *obj, Point2F oneToOnePoint) const 
{
   // Get our object's bounds window
   RectI objRect = sceneWindow->getObjectBoundsWindow(obj);

   F32 nWidth  = (F32)objRect.extent.x;
   F32 nHeight = (F32)objRect.extent.y;

   // Validate Y
   if( oneToOnePoint.y < -1.0f || oneToOnePoint.y > 1.0f  )
   {
      return Point2I( 0, 0 );
   }

   // Validate X
   if( oneToOnePoint.x < -1.0f || oneToOnePoint.x > 1.0f ) 
   {
      return Point2I( 0, 0 );
   }

   // Calculate Local Point
   Point2I localPoint = Point2I( S32( ( ( oneToOnePoint.x + 1.0f ) * 0.5f ) * nWidth ),
                                 S32( ( ( oneToOnePoint.y + 1.0f ) * 0.5f ) * nHeight ) );

   // Have to make sure we're lined up with the object in world coordinates
   localPoint += objRect.point;

   // Convert to global and return
   return sceneWindow->localToGlobalCoord( localPoint );
}


#endif // TORQUE_TOOLS
