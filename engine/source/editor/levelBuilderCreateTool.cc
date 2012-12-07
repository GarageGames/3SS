//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "graphics/dgl.h"
#include "editor/levelBuilderCreateTool.h"
#include "2d/sceneobject/SceneObject.h"

// Implement Our Console Object
IMPLEMENT_CONOBJECT(LevelBuilderCreateTool);

LevelBuilderCreateTool::LevelBuilderCreateTool() : LevelBuilderBaseEditTool(), 
                                                   mOutlineColor( 255, 255, 255 ),
                                                   mCreatedObject( NULL ),
                                                   mDragStart(0.0f, 0.0f),
                                                   mObjectHidden(false),
                                                   mAcquireCreatedObjects(true),
                                                   mMouseDownAR(1.0f)
{
   // Set our tool name
   mToolName            = StringTable->insert("Create Tool");
   mScriptClassName     = StringTable->EmptyString;
   mScriptSuperClassName= StringTable->EmptyString;
}

LevelBuilderCreateTool::~LevelBuilderCreateTool()
{
}

ConsoleMethod(LevelBuilderCreateTool, createObject, S32, 4, 4, "(sceneWindow, position) Creates a new object at given position"
              "@param sceneWindow The destination sceneWindow.\n"
              "@param position The desired position for the object.\n"
              "@return Returns the new objects ID or NULL in given sceneWindow is invalid")
{
   LevelBuilderSceneWindow* sceneWindow = dynamic_cast<LevelBuilderSceneWindow*>(Sim::findObject(argv[2]));
   if (sceneWindow)
   {
      SceneObject* sceneObject = object->createFull(sceneWindow, Utility::mGetStringElementVector(argv[3]));
      if (sceneObject)
         return sceneObject->getId();
   }

   return NULL;
}

ConsoleMethod(LevelBuilderCreateTool, startCreate, void, 4, 4, "(sceneWindow, position) ")
{
   LevelBuilderSceneWindow* sceneWindow = dynamic_cast<LevelBuilderSceneWindow*>(Sim::findObject(argv[2]));
   if (sceneWindow)
   {
      EditMouseStatus mouseStatus;
      mouseStatus.mousePoint2D = Utility::mGetStringElementVector( argv[3] );
      object->onMouseDown( sceneWindow, mouseStatus );
   }
}

bool LevelBuilderCreateTool::create(LevelBuilderSceneWindow* sceneWindow)
{
   if (!sceneWindow->getScene())
      return false;

   // Create the object.
   mCreatedObject = createObject();

   if (mCreatedObject == NULL)
      return false;

   // Link Class Namespace if specified.
   if( mScriptClassName != StringTable->EmptyString )
      mCreatedObject->setClassNamespace( StringTable->insert( mScriptClassName ) );

   // Link Class Namespace if specified.
   if( mScriptSuperClassName != StringTable->EmptyString )
      mCreatedObject->setSuperClassNamespace( StringTable->insert( mScriptSuperClassName ) );

   if( !mCreatedObject->isProperlyAdded() && !mCreatedObject->registerObject())
   {
      delete mCreatedObject;
      return false;
   }

   mCreatedObject->setModStaticFields(true);
   mCreatedObject->setModDynamicFields(true);

   sceneWindow->getScene()->addToScene(mCreatedObject);
   mCreatedObject->setVisible(false);
   mObjectHidden = true;

   return true;
}

SceneObject* LevelBuilderCreateTool::createFull(LevelBuilderSceneWindow* sceneWindow, Vector2 position)
{
   if (!create(sceneWindow))
      return NULL;

   if (!mCreatedObject || !sceneWindow->getSceneEdit())
      return NULL;

   showObject();
   mCreatedObject->setPosition(position);
   mCreatedObject->setSize(getDefaultSize(sceneWindow));

   if (mAcquireCreatedObjects)
   {
      sceneWindow->getSceneEdit()->clearAcquisition();
      sceneWindow->getSceneEdit()->requestAcquisition(mCreatedObject);
   }
   
   onObjectCreated();
   sceneWindow->getSceneEdit()->onObjectChanged(mCreatedObject);
   sceneWindow->getSceneEdit()->getAcquiredObjects().calculateObjectRect();

   UndoCreateAction* undo = new UndoCreateAction(sceneWindow->getSceneEdit(), "Create Object");
   undo->addObject(mCreatedObject);
   undo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());

   SceneObject* obj = mCreatedObject;
   mCreatedObject = NULL;

   return obj;
}

Vector2 LevelBuilderCreateTool::getDefaultSize(LevelBuilderSceneWindow* sceneWindow)
{
   Point2I pixelSize = getPixelSize();
   Point2I designSize = sceneWindow->getSceneEdit()->getDesignResolution();
   Vector2 worldSize = Vector2(100.0f, 75.0f);

   Scene* pScene = sceneWindow->getScene();
   if (pScene)
   {
      const char* cameraSize = pScene->getDataField(StringTable->insert("cameraSize"), NULL);
      if (cameraSize && cameraSize[0])
         worldSize = Utility::mGetStringElementVector(cameraSize);
   }

   Vector2 objectSize;
   objectSize.x = ((F32)worldSize.x / designSize.x) * pixelSize.x;
   objectSize.y = ((F32)worldSize.y / designSize.y) * pixelSize.y;

   return objectSize;
}

bool LevelBuilderCreateTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (!sceneWindow->getSceneEdit() || !create(sceneWindow))
      return Parent::onMouseDown(sceneWindow, mouseStatus);

   if (!mCreatedObject)
      return Parent::onMouseDown(sceneWindow, mouseStatus);

   mCreatedObject->setSize(mouseStatus.mousePoint2D - mDragStart);
   mCreatedObject->setPosition(mDragStart + (mouseStatus.mousePoint2D - mDragStart));

   bool flipX, flipY;
   bool actualFlipX = false;
   bool actualFlipY = false;
   Vector2 newSize, newPosition;
   // Snap the mouse position to the grid.
   move(sceneWindow->getSceneEdit(), Vector2(0.0f, 0.0f), mouseStatus.mousePoint2D, mDragStart);

   // Setup the sizing state.
   mSizingState = 0;
   if (mouseStatus.mousePoint2D.x < mDragStart.x)
   {
      actualFlipX = true;
      mSizingState |= SizingLeft;
   }
   else
      mSizingState |= SizingRight;

   if (mouseStatus.mousePoint2D.y < mDragStart.y)
   {
      actualFlipY = true;
      mSizingState |= SizingTop;
   }
   else
      mSizingState |= SizingBottom;

   Vector2 size = getDefaultSize( sceneWindow );
   mMouseDownAR = size.x / size.y;

   scale(sceneWindow->getSceneEdit(), Vector2(0.0f, 0.0f), mDragStart, mouseStatus.mousePoint2D, mouseStatus.event.modifier & SI_CTRL,
         mouseStatus.event.modifier & SI_SHIFT, mMouseDownAR, newSize, newPosition, flipX, flipY);

   mCreatedObject->setSize(newSize);
   mCreatedObject->setPosition(newPosition);
   mCreatedObject->setFlip(actualFlipX, actualFlipY);

   if (mAcquireCreatedObjects)
   {
      sceneWindow->getSceneEdit()->clearAcquisition();
      sceneWindow->getSceneEdit()->requestAcquisition(mCreatedObject);
   }

   return true;
}

bool LevelBuilderCreateTool::onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   // Maintain a small buffer zone so minor drags aren't actually interpreted as drags.
   if ( mouseStatus.dragRectNormal2D.extent.len() < 1.0f || mouseStatus.dragRectNormal.extent.len() < 4.0f)
      return Parent::onMouseDragged(sceneWindow, mouseStatus);

   if (!mCreatedObject || !sceneWindow->getSceneEdit())
      return Parent::onMouseDragged( sceneWindow, mouseStatus );

   // Show the object if it's not shown already.
   if (mObjectHidden)
   {
      showObject();
      mObjectHidden = false;
   }

   Vector2 newSize, newPosition;
   bool flipX, flipY;
   scale(sceneWindow->getSceneEdit(), mCreatedObject->getSize(), mCreatedObject->getPosition(), mouseStatus.mousePoint2D,
         mouseStatus.event.modifier & SI_CTRL, mouseStatus.event.modifier & SI_SHIFT, mMouseDownAR, newSize, newPosition, flipX, flipY);

   bool flipXValue = (mCreatedObject->getFlipX() || flipX) && !(mCreatedObject->getFlipX() && flipX);
   bool flipYValue = (mCreatedObject->getFlipY() || flipY) && !(mCreatedObject->getFlipY() && flipY);

   mCreatedObject->setFlip(flipXValue, flipYValue);
   mCreatedObject->setPosition(newPosition);
   mCreatedObject->setSize(newSize);

   sceneWindow->getSceneEdit()->onObjectSpatialChanged();

   return true;
}

bool LevelBuilderCreateTool::onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   // Nothing to do if nothing was created.
   if (!mCreatedObject || !sceneWindow->getSceneEdit())
      return Parent::onMouseUp( sceneWindow, mouseStatus );

   // Show the object if it's not shown already.
   if (mObjectHidden)
   {
      showObject();
      mObjectHidden = false;
   }

   if ( mouseStatus.dragRectNormal2D.extent.len() < 1.0f || mouseStatus.dragRectNormal.extent.len() < 4.0f)
   {
      Vector2 size = getDefaultSize( sceneWindow );

      mCreatedObject->setSize(size);
      mCreatedObject->setPosition(mouseStatus.mousePoint2D);
      mCreatedObject->setFlip(false, false);
   }

   onObjectCreated();
   sceneWindow->getSceneEdit()->onObjectChanged();
   sceneWindow->getSceneEdit()->getAcquiredObjects().calculateObjectRect();

   UndoCreateAction* undo = new UndoCreateAction(sceneWindow->getSceneEdit(), "Create Object");
   undo->addObject(mCreatedObject);
   undo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());

   mCreatedObject = NULL;

   return true;
}

void LevelBuilderCreateTool::onObjectCreated()
{
   Con::executef(this, 4, "onObjectCreated", Con::getIntArg(mCreatedObject->getId()), mCreatedObject->getClassName() );
}

void LevelBuilderCreateTool::onRenderScene( LevelBuilderSceneWindow* sceneWindow )
{
   // Render Parent
   Parent::onRenderScene( sceneWindow );

   // Draw Object Outline
   if( mCreatedObject && !mObjectHidden )
   {
      RectI objRect = sceneWindow->getObjectBoundsWindow( mCreatedObject );
      objRect.point = sceneWindow->localToGlobalCoord(objRect.point);
      dglDrawRect( objRect, mOutlineColor );
   }
}

ConsoleMethod(LevelBuilderCreateTool, setAcquireCreatedObjects, void, 3, 3, "")
{
   object->setAcquireCreatedObjects(dAtob(argv[2]));
}

ConsoleMethod(LevelBuilderCreateTool, getAcquireCreatedObjects, bool, 2, 2, "")
{
   return object->getAcquireCreatedObjects();
}


//-----------------------------------------------------------------------------
// Creation Configuration (Config Datablock/Script Class/SuperClass Namespaces)
//-----------------------------------------------------------------------------
ConsoleMethod(LevelBuilderCreateTool, setClassName, void, 3, 3, "Sets the script class namespace to link the created object to.")
{
   object->setClassNamespace( argv[2] );
}
void LevelBuilderCreateTool::setClassNamespace( const char* classNamespace )
{
   mScriptClassName = StringTable->insert( classNamespace );
}

ConsoleMethod(LevelBuilderCreateTool, setSuperClassName, void, 3, 3, "Sets the script super class namespace to link the created object to.")
{
   object->setSuperClassNamespace( argv[2] );
}
void LevelBuilderCreateTool::setSuperClassNamespace( const char* superClassNamespace )
{
   mScriptSuperClassName = StringTable->insert( superClassNamespace );
}


#endif // TORQUE_TOOLS
