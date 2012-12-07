//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#include "console/console.h"
#include "console/consoleTypes.h"
#include "dgl/dgl.h"
#include "editor/levelBuilderPolyTool.h"
#include "2D/TileMap.h"
#include "2D/ParticleEmitter.h"
#include "editor/levelBuilderSceneEdit.h"

IMPLEMENT_CONOBJECT( LevelBuilderPolyTool );

LevelBuilderPolyTool::LevelBuilderPolyTool() : LevelBuilderBaseTool(),
                                               mSceneWindow(NULL),
                                               mSceneObject(NULL),
                                               mAngle(0.0f),
                                               mCameraArea(0.0f, 0.0f, -1.0f, -1.0f),
                                               mDragVertex(-1),
                                               mUndoAction(NULL)
{
   mToolName = StringTable->insert("Collision Polygon Tool");
}

LevelBuilderPolyTool::~LevelBuilderPolyTool()
{
}

bool LevelBuilderPolyTool::onAdd()
{
   if (!Parent::onAdd())
      return false;

   if (!mUndoManager.registerObject())
      return false;

   mUndoManager.setModDynamicFields(true);
   mUndoManager.setModStaticFields(true);

   return true;
}

void LevelBuilderPolyTool::onRemove()
{
   mUndoManager.unregisterObject();

   Parent::onRemove();
}

bool LevelBuilderPolyTool::onActivate(LevelBuilderSceneWindow* sceneWindow)
{
   if (!Parent::onActivate(sceneWindow))
      return false;

   mSceneWindow = sceneWindow;
   mSceneObject = NULL;

   return true;
}

void LevelBuilderPolyTool::onDeactivate()
{
   finishEdit();

   mUndoFullAction = NULL;
   mUndoAction = NULL;

   mSceneObject = NULL;
   mSceneWindow = NULL;
   Parent::onDeactivate();
}

bool LevelBuilderPolyTool::onAcquireObject(SceneObject *object)
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

void LevelBuilderPolyTool::onRelinquishObject(SceneObject* object)
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
            // Grab the size and position of the camera from the scenegraph.
            t2dVector cameraPosition = t2dVector(0.0f, 0.0f);
            t2dVector cameraSize = t2dVector(100.0f, 75.0f);
            if (mSceneWindow->getSceneGraph())
            {
               const char* pos = mSceneWindow->getSceneGraph()->getDataField(StringTable->insert("cameraPosition"), NULL);
               if (mGetStringElementCount(pos) == 2)
                  cameraPosition = mGetStringElementVector(pos);
               
               const char* size = mSceneWindow->getSceneGraph()->getDataField(StringTable->insert("cameraSize"), NULL);
               if (mGetStringElementCount(size) == 2)
                  cameraSize = mGetStringElementVector(size);
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

void LevelBuilderPolyTool::editObject(SceneObject* object)
{
   if (!mSceneWindow || !isEditable(object))
      return;

   mSceneObject = object;

   clearCollisionPoly();
   acquireCollisionPoly(object);

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

   // Save Flip Settings
   mFlipSettings[0] = object->getFlipX();
   mFlipSettings[1] = object->getFlipY();
   object->setFlip(false,false);

   object->setCollisionPolyScale(t2dVector(1.0f,1.0f));

   mUndoFullAction = new UndoFullPolyAction(mSceneObject, "Collision Poly");
   mUndoFullAction->setOldPoints(mSceneObject->getCollisionPolyCount(), mSceneObject->getCollisionPolyArray());
}

ConsoleMethod(LevelBuilderPolyTool, editObject, void, 3, 3, "Selects an object for editing.")
{
   SceneObject* obj = dynamic_cast<SceneObject*>(Sim::findObject(argv[2]));
   if (obj)
      object->editObject(obj);
   else
      Con::warnf("Invalid object past to LevelBuilderPolyTool::editObject");
}

void LevelBuilderPolyTool::cancelEdit()
{
   if (!mSceneObject || !mSceneWindow)
      return;

   mSceneObject->setAngle(mAngle);

   // Restore Flip Settings
   mSceneObject->setFlip( mFlipSettings[0], mFlipSettings[1] );

   // Reset the camera.
   mSceneWindow->setTargetCameraZoom( mCameraZoom );
   mSceneWindow->setTargetCameraArea( mCameraArea );
   mSceneWindow->startCameraMove( 0.5f );

   // Cancel the undo.
   if (mUndoFullAction)
   {
      delete mUndoFullAction;
      mUndoFullAction = NULL;
   }
   
   mUndoManager.clearAll();

   clearCollisionPoly();
   mSceneObject = NULL;
}

ConsoleMethod(LevelBuilderPolyTool, cancelEdit, void, 2, 2, "Cancels editing of an object.")
{
   object->cancelEdit();
}

void LevelBuilderPolyTool::finishEdit()
{
   if (!mSceneObject || !mSceneWindow)
      return;

   setCollisionPoly(mSceneObject);
   mSceneObject->setAngle(mAngle);

   // Restore Flip Settings
   mSceneObject->setFlip( mFlipSettings[0], mFlipSettings[1] );

   // Reset the camera.
   mSceneWindow->setTargetCameraZoom( mCameraZoom );
   mSceneWindow->setTargetCameraArea( mCameraArea );
   mSceneWindow->startCameraMove( 0.5f );

   // Set the undo.
   if (mUndoFullAction)
   {
      mUndoFullAction->setNewPoints(mSceneObject->getCollisionPolyCount(), mSceneObject->getCollisionPolyArray());
      if (mUndoFullAction->hasChanged())
         mUndoFullAction->addToManager(&mSceneWindow->getSceneEdit()->getUndoManager());
      else
         delete mUndoFullAction;

      mUndoFullAction = NULL;
   }

   mUndoManager.clearAll();

   clearCollisionPoly();
   mSceneObject = NULL;
}

ConsoleMethod(LevelBuilderPolyTool, finishEdit, void, 2, 2, "Applies changes and ends editing of an object.")
{
   object->finishEdit();
}

void LevelBuilderPolyTool::clearCollisionPoly()
{
   mNutList.clear();
}

bool LevelBuilderPolyTool::isEditable(SceneObject* obj)
{
   if (dynamic_cast<TileMap*>(obj) ||
       dynamic_cast<ParticleEmitter*>(obj))
   {
      return false;
   }
   return true;
}

bool LevelBuilderPolyTool::onMouseMove( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return Parent::onMouseMove(sceneWindow, mouseStatus);

   mLocalMousePosition = getCollisionPointObject(sceneWindow, mSceneObject, mouseStatus.event.mousePoint);

   return true;
}

bool LevelBuilderPolyTool::onRightMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return Parent::onRightMouseDown(sceneWindow, mouseStatus);

   S32 hitVertex = findCollisionVertex(mouseStatus.event.mousePoint);
   if (hitVertex != -1)
   {
      UndoPolyRemoveVertexAction* undo = new UndoPolyRemoveVertexAction(this, "Remove Vertex");
      undo->addIndex(mNutList[hitVertex], hitVertex);
      undo->addToManager(&mUndoManager);
      mNutList.erase(hitVertex);
   }

   return true;
}

bool LevelBuilderPolyTool::onRightMouseDragged(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)
{
   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return Parent::onRightMouseDown(sceneWindow, mouseStatus);

   return true;
}

bool LevelBuilderPolyTool::onRightMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)
{
   // Somewhere along the lines this broke. So... I'm killing it for now.
   if (!mSceneObject || (sceneWindow != mSceneWindow) || mNutList.empty())
      return false;

   //UndoPolyRemoveVertexAction* undo = new UndoPolyRemoveVertexAction(this, "Remove Vertices");
   //bool add = false;
   //RectI eraseRect = mouseStatus.dragRectNormal;
   //S32 i = 0;
   //for (Vector<Point2F>::iterator j = mNutList.begin(); j != mNutList.end(); j++, i++)
   //{
   //   if (eraseRect.pointInRect(sceneWindow->globalToLocalCoord(getCollisionPointWorld(sceneWindow, mSceneObject, (*j)))))
   //   {
   //      undo->addIndex(mNutList[i], i);
   //      add = true;
   //      mNutList.erase(j);
   //   }
   //}
   //if (add)
   //   mUndoManager.addAction(undo);
   //else
   //   delete undo;

   return true;
}

bool LevelBuilderPolyTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   if (mSceneWindow != sceneWindow)
      return false;

   // Acquire Object
   if (!mSceneObject)
   {
      if (mouseStatus.pickList.size() == 0)
         return Parent::onMouseDown(sceneWindow, mouseStatus);

      SceneObject* pObj = mouseStatus.pickList[0];

      if ((mouseStatus.event.mouseClickCount >= 2) && isEditable(pObj))
         sceneWindow->getSceneEdit()->requestAcquisition(pObj);

      return true;
   }

   mAddUndo = false;

   RectI bounds = sceneWindow->getObjectBoundsWindow(mSceneObject);
   if (bounds.pointInRect(mouseStatus.event.mousePoint))
   {
      mDragVertex = findCollisionVertex(mouseStatus.event.mousePoint);
      if (mDragVertex == -1)
      {
         // We clicked inside the bounds of our object, make sure the new point keeps the poly convex.
         S32 newIndex = checkNewPointConvexAddition(mSceneWindow->localToGlobalCoord(mouseStatus.event.mousePoint));
         if (newIndex != -1)
         {
            mAddUndo = true;
            mNutList.insert(newIndex);
            mNutList[newIndex] = getCollisionPointObject(mSceneWindow, mSceneObject, mSceneWindow->localToGlobalCoord(mouseStatus.event.mousePoint));
            mDragVertex = newIndex;
            UndoPolyAddVertexAction* undo = new UndoPolyAddVertexAction(this, "Add Vertex");
            undo->setIndex(mNutList[newIndex], newIndex);
            mUndoAction = (UndoAction*)undo;
         }
      }
      else
      {
         UndoPolyMoveVertexAction* undo = new UndoPolyMoveVertexAction(this, "Move Vertex");
         undo->setStartPosition(mNutList[mDragVertex], mDragVertex);
         mUndoAction = (UndoAction*)undo;
      }
   }

   else if ((mouseStatus.event.mouseClickCount >= 2) && mSceneObject)
      finishEdit();

   return true;
}

bool LevelBuilderPolyTool::onMouseDragged(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)
{
   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return false;

   // Drag vertex
   if (mDragVertex != -1)
   {      
      RectI bounds = sceneWindow->getObjectBoundsWindow(mSceneObject);
      if (bounds.pointInRect(mouseStatus.event.mousePoint))
      {
         if (checkDragPoint(mNutList, mDragVertex, mSceneWindow->localToGlobalCoord(mouseStatus.event.mousePoint)))
         {
            mNutList[mDragVertex] = getCollisionPointObject(mSceneWindow, mSceneObject, mSceneWindow->localToGlobalCoord(mouseStatus.event.mousePoint));
            mAddUndo = true;
         }
      }
   }
   return true;
}

bool LevelBuilderPolyTool::onMouseUp(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus)
{
   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return false;

   if (mAddUndo)
   {
      UndoPolyMoveVertexAction* undoMove = dynamic_cast<UndoPolyMoveVertexAction*>(mUndoAction);
      UndoPolyAddVertexAction* undoAdd = dynamic_cast<UndoPolyAddVertexAction*>(mUndoAction);
      if (undoMove)
      {
         undoMove->setEndPosition(mNutList[mDragVertex]);
         undoMove->addToManager(&mUndoManager);
      }
      else if (undoAdd)
      {
         undoAdd->setIndex(mNutList[mDragVertex], mDragVertex);
         undoAdd->addToManager(&mUndoManager);
      }
      else if (mUndoAction)
         delete mUndoAction;
   }
   else if (mUndoAction)
      delete mUndoAction;

   mUndoAction = NULL;

   mDragVertex = -1;
   return true;
}

void LevelBuilderPolyTool::onRenderGraph(LevelBuilderSceneWindow* sceneWindow)
{
   Parent::onRenderGraph(sceneWindow);

   if (!mSceneObject || (sceneWindow != mSceneWindow))
      return;

   // Draw the bounding rect.
   RectI bounds = mSceneWindow->getObjectBoundsWindow(mSceneObject);
   bounds.point = mSceneWindow->localToGlobalCoord(bounds.point);
   dglDrawRect(bounds, ColorI(255, 255, 255));
   
   if (!mNutList.empty())
   {
      // Generate Draw Points 
      static Vector<Point2I> drawPoints;
      drawPoints.clear();
      drawPoints.reserve(mNutList.size() + 1);

      for (Vector<Point2F>::iterator j = mNutList.begin(); j != mNutList.end(); j++)
         drawPoints.push_back(getCollisionPointWorld(mSceneWindow, mSceneObject, (*j)));

      // Render the collision poly.
      Vector<Point2I>::iterator i;
      for (i = drawPoints.begin(); i != drawPoints.end(); i++)
      {
         Point2I pt = (*i);

         i++;
         if (i != drawPoints.end())
         {
            Point2I ptNext = (*i);
            dglDrawLine(pt, (*i), mNutColor);
         }
         i--;

         drawNut(pt);
      }

      // Connect the end of the poly.
      if (drawPoints.size() > 2)
         dglDrawLine(*(i - 1), (*drawPoints.begin()), mNutColor);
   }

   // If this is a window's tool override, we need the window to follow the object
   // being edited.
   if ((mSceneWindow->getToolOverride() == this) && !mSceneWindow->isCameraMoving())
   {
      RectF newArea = mSceneObject->getAABBRectangle();
      newArea.inset(-1, -1);
      mSceneWindow->setCurrentCameraArea(newArea);
   }
}

void LevelBuilderPolyTool::insertVertex(t2dVector position, S32 index)
{
   mNutList.insert(index);
   mNutList[index] = position;
}

void LevelBuilderPolyTool::removeVertex(S32 index)
{
   mNutList.erase(index);
}

void LevelBuilderPolyTool::moveVertex(S32 index, t2dVector position)
{
   mNutList[index] = position;
}

Point2F LevelBuilderPolyTool::getCollisionPointObject(LevelBuilderSceneWindow* sceneWindow, const SceneObject* obj, const Point2I& worldPoint) const 
{
   Point2I localPoint = sceneWindow->globalToLocalCoord( worldPoint );
   // Get our object's bounds window
   RectI objRect = sceneWindow->getObjectBoundsWindow( obj );

   F32 nWidthInverse  = 1.0f / (F32)objRect.extent.x;
   F32 nHeightInverse = 1.0f / (F32)objRect.extent.y;

   S32 positionY = localPoint.y - objRect.point.y;
   if( positionY < 0 || positionY > objRect.extent.y  )
      return Point2F(0,0);

   S32 positionX = localPoint.x - objRect.point.x;
   if( positionX < 0 || positionX > objRect.extent.x )
      return Point2F(0,0);

   return Point2F( ( (F32)positionX * nWidthInverse  * 2.0f - 1.0f ), 
                   ( (F32)positionY * nHeightInverse * 2.0f - 1.0f) );
}

Point2I LevelBuilderPolyTool::getCollisionPointWorld(LevelBuilderSceneWindow* sceneWindow, const SceneObject *obj, Point2F oneToOnePoint) const 
{
   // Get our object's bounds window
   RectI objRect = sceneWindow->getObjectBoundsWindow(obj);

   F32 nWidth  = (F32)objRect.extent.x;
   F32 nHeight = (F32)objRect.extent.y;

   // Validate Y
   if( oneToOnePoint.y < -1.0f || oneToOnePoint.y > 1.0f  )
   {
      Con::warnf("GuiSceneToolBase::getObjectPointWorld - invalid y point!");
      return Point2I( 0, 0 );
   }

   // Validate X
   if( oneToOnePoint.x < -1.0f || oneToOnePoint.x > 1.0f ) 
   {
      Con::warnf("GuiSceneToolBase::getObjectPointWorld - invalid x point!");
      return Point2I( 0, 0 );
   }

   oneToOnePoint.x *= obj->getCollisionPolyScale().x;
   oneToOnePoint.y *= obj->getCollisionPolyScale().x;

   // Calculate Local Point
   Point2I localPoint = Point2I( S32( ( ( oneToOnePoint.x + 1.0f ) * 0.5f ) * nWidth ),
                                 S32( ( ( oneToOnePoint.y + 1.0f ) * 0.5f ) * nHeight ) );

   // Have to make sure we're lined up with the object in world coordinates
   localPoint += objRect.point;

   // Convert to global and return
   return sceneWindow->localToGlobalCoord( localPoint );
}

bool LevelBuilderPolyTool::acquireCollisionPoly(SceneObject *obj)
{
   // Fetch Collision Details.
   U32 polyCount = obj->getParentPhysics().getCollisionPolyCount();
   const t2dVector* pPolyBasis = obj->getParentPhysics().getCollisionPolyBasis();

   if (polyCount <= 3) 
      return false;

   // Check to see if it's a default collision poly.  If it is, ignore it.
   if( (t2dVector)pPolyBasis[0] == t2dVector(-1.f,-1.f) && (t2dVector)pPolyBasis[1] == t2dVector(1.f,-1.f) &&
       (t2dVector)pPolyBasis[2] == t2dVector(1.f,1.f)   && (t2dVector)pPolyBasis[3] == t2dVector(-1.f,1.f) )
       return false;

   // Clear our poly basis list
   mNutList.clear();

   // Generate our poly basis list
   for ( U32 n = 0; n < polyCount; n++ )
      mNutList.push_back( Point2F( pPolyBasis[n].x, pPolyBasis[n].y ) );

   // Success
   return true;
}

bool LevelBuilderPolyTool::setCollisionPoly( SceneObject *obj )
{
   if (!mSceneWindow)
      return false;

   // When we relinquish control we set the new collision polygon
   if( mNutList.size() >= 3 )
   {
      t2dVector poly[b2_maxPolygonVertices];

      // Generate our poly basis list
      Vector<Point2F>::iterator iter = mNutList.begin();
      for (U32 i = 0 ; iter != mNutList.end(); i++, iter++)
         poly[i] = t2dVector((*iter).x, (*iter).y);

      // Set the poly to our object
      obj->setCollisionPolyCustom(mNutList.size(), poly);

      mSceneWindow->getSceneEdit()->onObjectChanged();
      return true;
   }

   return false;
}

S32 LevelBuilderPolyTool::checkNewPointConvexAddition( const Point2I &newPoint  )
{
   // We can't act unless we've got an object lock and an object
   if( !mSceneObject || (mNutList.size() >= 64) || !mSceneWindow)
      return -1;

   // If it's the first point of course it's ok
   if( mNutList.empty() || mNutList.size() < 2 )
      return mNutList.size();

   // Get the pointer to our acquired object
   SceneObject *obj = mSceneObject;

   // Setup poly basis list
   Vector<t2dVector> PolyBasisList;
   PolyBasisList.clear();
   PolyBasisList.reserve( mNutList.size() + 2 );

   // Generate our poly basis list
   Vector<Point2F>::iterator i = mNutList.begin();
   for( ; i != mNutList.end(); i++ ) 
      PolyBasisList.push_back( t2dVector( (*i).x, (*i).y ) );

   // Append the new point to check 
   Point2F fPoint = getCollisionPointObject( mSceneWindow, obj, newPoint );
   t2dVector vPoint = t2dVector( fPoint.x, fPoint.y );

   for( S32 j = PolyBasisList.size() ; j > 0; j-- )
   {
      PolyBasisList.insert( j );
      PolyBasisList[j] = vPoint;
      if ( checkConvexPoly( PolyBasisList ) )
         return j;
      PolyBasisList.erase( j );
   }

   return -1;
}

bool LevelBuilderPolyTool::checkDragPoint( Vector<Point2F> &list, S32 index, Point2I dragPoint )
{
   // If it's the first point of course it's ok
   if( mNutList.empty() || (mNutList.size() < index) || !mSceneWindow )
      return false;

   // Get the pointer to our acquired object
   SceneObject *obj = mSceneObject;

   // Setup poly basis list
   Vector<t2dVector> PolyBasisList;
   PolyBasisList.clear();
   PolyBasisList.reserve( list.size() + 2 );

   // Generate our poly basis list
   Vector<Point2F>::iterator i = list.begin();
   for(S32 j = 0 ; i != list.end(); i++, j++ ) 
   {
      if( j == index )
      {
         Point2F pt = getCollisionPointObject( mSceneWindow, obj, dragPoint );
         PolyBasisList.push_back(  t2dVector( pt.x, pt.y ) );
      }
      else
         PolyBasisList.push_back( t2dVector( (*i).x, (*i).y ) );
   }

   if ( checkConvexPoly( PolyBasisList ) )
        return true;

   return false;
}

void LevelBuilderPolyTool::setPolyPrimitive( U32 polyVertexCount )
{
   // Check for Maximum Polygon Edges.
   if ( polyVertexCount > b2_maxPolygonVertices )
   {
      Con::warnf("LevelBuilderPolyTool::setPolyPrimitive() - Cannot generate a %d edged collision polygon.  Maximum is %d!", polyVertexCount, b2_maxPolygonVertices);
      return;
   }

   if( polyVertexCount < 3 )
   {
      Con::warnf("LevelBuilderPolyTool::setPolyPrimitive() - Cannot generate a %d edged collision polygon.  Minimum is 3!", polyVertexCount );
      return;
   }

   // Clear Polygon Basis List.
   mNutList.clear(); 
   mNutList.setSize( polyVertexCount );

   // Special-Case Quad?
   if ( polyVertexCount == 4 )
   {
      // Yes, so set Quad.
      mNutList[0].set(-1.0f, -1.0f);
      mNutList[1].set(+1.0f, -1.0f);
      mNutList[2].set(+1.0f, +1.0f);
      mNutList[3].set(-1.0f, +1.0f);
   }
   else
   {
      // No, so calculate Regular (Primitive) Polygon Stepping.
      //
      // NOTE:- The polygon sits on an ellipse the scribes the interior
      //            of the collision box.
      F32 angle = M_PI_F / polyVertexCount;
      F32 angleStep = M_2PI_F / polyVertexCount;

      // Calculate Polygon.
      for ( U32 n = 0; n < polyVertexCount; n++ )
      {
         // Calculate Angle.
         angle += angleStep;
         // Store Polygon Vertex.
         mNutList[n].set(mCos(angle), mSin(angle));
      }
   }
}

ConsoleMethod(LevelBuilderPolyTool, checkConvexPoly, bool, 3, 3, "")
{
   Vector<t2dVector> poly;

   for (S32 i = 0; i < mGetStringElementCount(argv[2]); i += 2)
      poly.push_back(t2dVector(mGetStringElementVector(argv[2], i)));

   return object->checkConvexPoly(poly);
}

bool LevelBuilderPolyTool::checkConvexPoly( Vector<t2dVector> &list )
{
   // Reset Sign Flag.
   bool sign = false;
   // Assume the polygon is convex!
   bool convex = true;

   // Poly vertex count
   S32 polyVertexCount = list.size();

   // Check that the polygon is convex.
   // NOTE:-    We'll iterate the polygon and check that each consecutive edge-pair
   //           maintains the same perp-dot-product sign; if not then it's not convex.
   for ( U32 n0 = 0; n0 < polyVertexCount; n0++ )
   {
      // Calculate next two vertex indices.
      U32 n1 = (n0+1)%polyVertexCount;
      U32 n2 = (n0+2)%polyVertexCount;
      // Calculate Edges.
      const t2dVector e0 = list[n1] - list[n0];
      const t2dVector e1 = list[n2] - list[n1];
      // Calculate Perpendicular Dot-Product for edges.
      F32 perpDotEdge = e0.getPerp().dot( e1 );
      // Have we processed the first vertex?
      if ( n0 > 0 )
      {
         // Yes, so is the PDE the same sign?
         if ( sign != ( perpDotEdge > 0.0f ) )
         {
            // No, so polygon is *not* convex!
            convex = false;
            break;
         }
      }
      // No, so fetch sign.
      else 
      {
         // Calculate sign flag.
         sign = ( perpDotEdge > 0.0f );
      }
   }

   // Convex?
   if ( !convex )
      return false;

   // Convex!
   return true;
}

S32 LevelBuilderPolyTool::findCollisionVertex( Point2I hitPoint )
{
   if ( mNutList.empty() || !mSceneWindow)
      return -1;

   // Generate Draw Points 
   static Vector<Point2I> drawPoints;
   drawPoints.clear();
   drawPoints.reserve( mNutList.size() + 1 );

   Vector<Point2F>::iterator j = mNutList.begin();
   for ( ; j != mNutList.end(); j++ )
      drawPoints.push_back( mSceneWindow->globalToLocalCoord(getCollisionPointWorld( mSceneWindow, mSceneObject, (*j) ) ) );

   for (S32 i = 0 ; i < drawPoints.size(); i++ )
      if( inNut (drawPoints[i], hitPoint.x, hitPoint.y) )
         return i;

   return -1;
}

ConsoleMethod( LevelBuilderPolyTool, getVertexCount, S32,2,2,"tool.getVertexCount()")
{
   return object->getVertexCount();
}

ConsoleMethod( LevelBuilderPolyTool, getScript, const char*,2,2,"tool.getScript()")
{
   return object->getCollisionPolyScript();
}

StringTableEntry LevelBuilderPolyTool::getCollisionPolyScript()
{
   if ( mNutList.empty() || mNutList.size() < 3 )
      return "";

   char *returnBuffer = Con::getReturnBuffer( mNutList.size() * 8);

   dMemset( returnBuffer, 0, mNutList.size() * 8 );

   if( !returnBuffer )
   {
      // Warn.
      Con::printf("LevelBuilderPolyTool::getCollisionPolyScript() - Unable to allocate buffer!");

      // Exit.
      return "";
   }

   // Interate Polygon.
   for ( U32 i = 0; i < mNutList.size(); i++ )
   {
      char szPoly[256];
      dMemset( szPoly, 0, 256 );
      dSprintf( szPoly, 256, "%0.3f %0.3f ", mNutList[i].x, mNutList[i].y );
      dStrcat( returnBuffer, szPoly);
   }

   return returnBuffer;

}

ConsoleMethod( LevelBuilderPolyTool, setPolyPrimitive, void, 3, 3, "setPolyPrimitive(%numEdges)" )
{
   object->setPolyPrimitive( dAtoi(argv[2]) );
}

ConsoleMethod( LevelBuilderPolyTool, getLocalMousePosition, const char*, 2, 2, "")
{
   char* ret = Con::getReturnBuffer(32);
   dSprintf(ret, 32, "%s %s", object->getLocalMousePosition().x, object->getLocalMousePosition().y);
   return ret;
}


#endif // TORQUE_TOOLS
