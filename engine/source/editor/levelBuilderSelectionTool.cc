//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER


#include "console/console.h"
#include "console/consoleTypes.h"
#include "graphics/dgl.h"
#include "gui/guiDefaultControlRender.h"
#include "editor/levelBuilderSelectionTool.h"


// Implement Our Console Object
IMPLEMENT_CONOBJECT( SelectionToolWidget );

//-----------------------------------------------------------------------------
// World Limit Table.
//-----------------------------------------------------------------------------
static EnumTable::Enums displayRuleLookup[] =
                {
                { SelectionToolWidget::UnPathedOnly,    "Unpathed" },
                { SelectionToolWidget::PathedOnly,      "Pathed" },
                { SelectionToolWidget::NoDisplayRules,  "None" }
                };

static EnumTable displayRuleTable(sizeof(displayRuleLookup) /  sizeof(EnumTable::Enums), &displayRuleLookup[0]);

//-----------------------------------------------------------------------------
// World Limit Script-Enumerator.
//-----------------------------------------------------------------------------
static SelectionToolWidget::eDisplayRules getDisplayRule(const char* label)
{
    // Search for Mnemonic.
    for(U32 i = 0; i < (sizeof(displayRuleLookup) / sizeof(EnumTable::Enums)); i++)
        if( dStricmp(displayRuleLookup[i].label, label) == 0)
            return((SelectionToolWidget::eDisplayRules)displayRuleLookup[i].index);

    return SelectionToolWidget::NoDisplayRules;
}

SelectionToolWidget::SelectionToolWidget()
{
   mPriority = 1;
   mPosition = -1;
   mShowClasses = true;
   mCallback = StringTable->EmptyString;
   mToolTip = StringTable->EmptyString;
   mTexture = NULL;
   mDisplayRules = NoDisplayRules;
}

void SelectionToolWidget::initPersistFields()
{
   Parent::initPersistFields();
   addField("priority", TypeS32, Offset(mPriority, SelectionToolWidget));
   addField("position", TypeS32, Offset(mPosition, SelectionToolWidget));
   addField("showClasses", TypeBool, Offset(mShowClasses, SelectionToolWidget));
   addField("callback", TypeString, Offset(mCallback, SelectionToolWidget));
   addField("tooltip", TypeCaseString, Offset(mToolTip, SelectionToolWidget));
}

ConsoleMethod(SelectionToolWidget, addClass, void, 3, 3, "%widget.addClass(className)")
{
   object->addClass(argv[2]);
}

ConsoleMethod(SelectionToolWidget, setTexture, void, 3, 3, "%widget.setTexture(textureName)")
{
   object->setTexture(argv[2]);
}

ConsoleMethod(SelectionToolWidget, setDisplayRule, void, 3, 3, "%widget.setDisplayRule(\"Rule\")")
{
   object->setDisplayRule(getDisplayRule(argv[2]));
}

IMPLEMENT_CONOBJECT( LevelBuilderSelectionTool );

LevelBuilderSelectionTool::LevelBuilderSelectionTool() : LevelBuilderBaseEditTool(),
                                                         mHoverObj( NULL ),
                                                         mHoverOutlineColor( 255, 255, 255, 200 ),
                                                         mHoverFillColor( 128, 128, 128, 200),
                                                         mMouseState( 0 ),
                                                         mDragRect(0, 0, -1, -1),
                                                         mFullContainSelect(false),
                                                         mMouseDown(false),
                                                         mAddUndo(false),
                                                         mCurrentUndo(NULL),
                                                         mSelectedHoverObj(NULL),
                                                         mTexture(NULL),
                                                         mWidgetSize(16, 16),
                                                         mWidgetBufferSize(20, 20),
                                                         mUndoSelections(false),
                                                         mNumberOfWidgets(0),
                                                         mWidgetSelectBuffer(4),
                                                         mCantMove(false),
                                                         mForcingUniform(false),
                                                         mForcingShift(false),
                                                         mTooltipPosition(0, 0),
                                                         mMouseDownAR(1.0f),
                                                         mScaleOrigPos(0,0),
                                                         mScaleOrigSize(0,0),
                                                         mAngleList( NULL ),
														 m_AllowSizing(true),
														 m_AllowMultipleSelection(true),
														 m_HoverOutlineWidth(1.0f)
{
   // Set our tool name
   mToolName = StringTable->insert("Selection Tool");
}

LevelBuilderSelectionTool::~LevelBuilderSelectionTool()
{
   mHoverObj         = NULL;
   mHoverWidget      = NULL;
   mSelectedHoverObj = NULL;
   mCurrentUndo      = NULL;
}

//---------------------------------------------------------------------------------------------
//	onKeyDown
// Arrow keys nudge the object in the direction of the arrow (holding shift nudges them
// farther. Delete deletes all acquired objects.
//---------------------------------------------------------------------------------------------
bool LevelBuilderSelectionTool::onKeyDown(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event)
{
   // Don't do anything if the mouse is down.
   if (mMouseDown)
      return false;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   Vector2 newPos;

   mAddUndo = false;

   // Create the undo object.
   UndoMoveAction* undoMove = new UndoMoveAction(mOwner, (UTF8*)"Nudge Objects");
   mCurrentUndo = (UndoAction*)undoMove;

   switch(event.keyCode)
   {
   case KEY_LEFT:
      nudge(mOwner->getAcquiredObjects().getPosition(), -1, 0, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();

      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undoMove->addObject(mOwner->getAcquiredObject(i));

      mAddUndo = true;
      return true;
   case KEY_RIGHT:
      nudge(mOwner->getAcquiredObjects().getPosition(), 1, 0, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();

      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undoMove->addObject(mOwner->getAcquiredObject(i));

      mAddUndo = true;
      return true;
   case KEY_UP:
      nudge(mOwner->getAcquiredObjects().getPosition(), 0, -1, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();

      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undoMove->addObject(mOwner->getAcquiredObject(i));

      mAddUndo = true;
      return true;
   case KEY_DOWN:
      nudge(mOwner->getAcquiredObjects().getPosition(), 0, 1, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();

      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undoMove->addObject(mOwner->getAcquiredObject(i));

      mAddUndo = true;
      return true;
   case KEY_DELETE:
      mOwner->deleteAcquiredObjects();
      mHoverObj = NULL;
      return true;
   }

   return false;
}

bool LevelBuilderSelectionTool::onKeyRepeat(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event)
{
   // Don't do anything if the mouse is down.
   if (mMouseDown)
      return false;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   Vector2 newPos;
   switch(event.keyCode)
   {
   case KEY_LEFT:
      nudge(mOwner->getAcquiredObjects().getPosition(), -1, 0, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();
      return true;
   case KEY_RIGHT:
      nudge(mOwner->getAcquiredObjects().getPosition(), 1, 0, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();
      return true;
   case KEY_UP:
      nudge(mOwner->getAcquiredObjects().getPosition(), 0, -1, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();
      return true;
   case KEY_DOWN:
      nudge(mOwner->getAcquiredObjects().getPosition(), 0, 1, event.modifier & SI_SHIFT, newPos);
      mOwner->getAcquiredObjects().setPosition(newPos);
      mOwner->onObjectSpatialChanged();
      return true;
   }

   return false;
}

bool LevelBuilderSelectionTool::onKeyUp(LevelBuilderSceneWindow* sceneWindow, const GuiEvent& event)
{
   // Don't do anything if the mouse is down.
   if (mMouseDown)
      return false;

   switch(event.keyCode)
   {
   case KEY_LEFT:
   case KEY_RIGHT:
   case KEY_UP:
   case KEY_DOWN:
      if (mAddUndo && sceneWindow->getSceneEdit()->hasAcquiredObjects())
      {
         UndoMoveAction* undoMove = (UndoMoveAction*)mCurrentUndo;
         if(!undoMove)
            return false;

         for (S32 i = 0; i < sceneWindow->getSceneEdit()->getAcquiredObjectCount(); i++)
            undoMove->setNewPosition(sceneWindow->getSceneEdit()->getAcquiredObject(i));

         undoMove->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());
      }
      else if (mCurrentUndo)
         delete mCurrentUndo;

      mCurrentUndo = NULL;
   }
   return false;
}

//---------------------------------------------------------------------------------------------
// onMouseMove
//---------------------------------------------------------------------------------------------
bool LevelBuilderSelectionTool::onMouseMove( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   checkSelectedHoverObj(sceneWindow, mouseStatus);

      // Clear hover widget
   mHoverWidget = NULL;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   // Save any object that's under the mouse so it can have a nice pulsing box rendered on it.
   if(mouseStatus.firstPickPoint == NULL)
   {
      mHoverObj = NULL;
   }
   else
   {
      for (S32 i = 0; i < mouseStatus.pickList.size(); i++)
      {
          if (mOwner->isAcquired(mouseStatus.pickList[i].mpSceneObject))
         {
            if (i == (mouseStatus.pickList.size() - 1))
                mHoverObj = mouseStatus.pickList[0].mpSceneObject;
            else
                mHoverObj = mouseStatus.pickList[i + 1].mpSceneObject;

            return Parent::onMouseMove( sceneWindow, mouseStatus );
         }
      }
      // Getting here means no objects in the pick list are acquired.
      mHoverObj = mouseStatus.pickList[0].mpSceneObject;
   }

   // Check to see if a widget is being hovered over.
   if (mSelectedHoverObj && mOwner->isAcquired(mSelectedHoverObj))
   {
      // Grab the object's bounds.
      Point2I widgetSize = mWidgetBufferSize;
      RectI objRect = sceneWindow->getObjectBoundsWindow( mSelectedHoverObj );

      RectI widgetRect;
      Point2I startPoint = objRect.point - Point2I(mWidgetSelectBuffer, widgetSize.y + mWidgetSelectBuffer);
      for (S32 i = 0; i < mNumberOfWidgets; i++)
      {
         widgetRect.point = startPoint;
         widgetRect.extent = widgetSize;
         if ((i == 0) || (i == (mNumberOfWidgets - 1)))
            widgetRect.extent += Point2I(mWidgetSelectBuffer, mWidgetSelectBuffer);
         
         if (widgetRect.pointInRect(mouseStatus.event.mousePoint) && mCurrentWidgets[i] != NULL)
         {
            mHoverWidget = mCurrentWidgets[i];
            break;
         }

         startPoint.x += widgetRect.extent.x;
      }
   }


   // Can't forget to let our parent do it's work!
   return Parent::onMouseMove( sceneWindow, mouseStatus );
}

//---------------------------------------------------------------------------------------------
// onMouseDown
//---------------------------------------------------------------------------------------------
bool LevelBuilderSelectionTool::onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   mAddUndo = false;
   mMouseDown = true;
   mTooltipPosition = mouseStatus.event.mousePoint;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   // If nothing is acquired, the only thing to do is select.
   if( !mOwner->getAcquiredObject() )
   {
      mMouseState = Selecting;
      return Parent::onMouseDown( sceneWindow, mouseStatus );
   }

   mMouseOffset = mOwner->getAcquiredObjects().getPosition() - mouseStatus.mousePoint2D;
   Vector2 size = mOwner->getAcquiredObjects().getSize();
   mMouseDownAR = size.x / size.y;

   // Check to see if a sizing nob was hit - in which case all acquired objects will be scaled.
   if (mOwner->getAcquiredObjectCount() && m_AllowSizing)
   {
      mSizingState = getSizingState( sceneWindow, mouseStatus.event.mousePoint, mOwner->getAcquiredObjects().getBoundingRect() );
      if( mSizingState != SizingNone )
      {
         // Alt turns this into a rotation tool. Otherwise it's scaling.
         if (mouseStatus.event.modifier & SI_ALT)
         {
            mMouseState = RotatingSelection;
            mStartAngle = mOwner->getAcquiredObjects().getAngle();
            mAngleVector = mouseStatus.mousePoint2D - mOwner->getAcquiredObjects().getPosition();

            // Create the undo object.
            UndoRotateAction* undoMove = new UndoRotateAction(mOwner, "Rotate Objects");
            for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
               undoMove->addObject(mOwner->getAcquiredObject(i));

            mCurrentUndo = (UndoAction*)undoMove;
         }
         else
         {
            mMouseState = SizingSelection;

            // create angle list for selected objects
            mAngleList = new F32[mOwner->getAcquiredObjectCount()];

            // Create the undo object.
            UndoScaleAction* undoMove = new UndoScaleAction(mOwner, "Scale Objects");
            for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
            {
               SceneObject *object = mOwner->getAcquiredObject(i);

               // add to angle list
               mAngleList[i] = object->getAngle();

               // add to undo 
               undoMove->addObject(object);
            }

            mCurrentUndo = (UndoAction*)undoMove;

            // store orig click pos and size
            mScaleOrigSize = mOwner->getAcquiredObjects().getSize(); 
            mScaleOrigPos = mOwner->getAcquiredObjects().getPosition();
         }

         return Parent::onMouseDown( sceneWindow, mouseStatus );
      }
   }

   // Check if one of the widget buttons was pressed. This needs to be done after the sizing knob check.
   if (mSelectedHoverObj && mOwner->isAcquired(mSelectedHoverObj))
   {
      // Grab the object's bounds.
      Point2I widgetSize = mWidgetBufferSize;
      RectI objRect = sceneWindow->getObjectBoundsWindow( mSelectedHoverObj );

      RectI widgetRect;
      Point2I startPoint = objRect.point - Point2I(mWidgetSelectBuffer, widgetSize.y + mWidgetSelectBuffer);
      for (S32 i = 0; i < mNumberOfWidgets; i++)
      {
         widgetRect.point = startPoint;
         widgetRect.extent = widgetSize;
         if ((i == 0) || (i == (mNumberOfWidgets - 1)))
            widgetRect.extent += Point2I(mWidgetSelectBuffer, mWidgetSelectBuffer);
         
         if (widgetRect.pointInRect(mouseStatus.event.mousePoint) && mCurrentWidgets[i] != NULL)
         {
            mHoverWidget = mCurrentWidgets[i];
            mMouseState = SelectingWidget;
            return Parent::onMouseDown(sceneWindow,  mouseStatus );
         }

         startPoint.x += widgetRect.extent.x;
      }
   }

   // Check to see if an object was hit - in which case all acquired objects will be moved.
   if (mouseStatus.pickList.size())
   {
      for (U32 i = 0; i < (U32)mouseStatus.pickList.size(); i++)
      {
          if (mOwner->isAcquired(mouseStatus.pickList[i].mpSceneObject))
         {
            mMouseState = MovingSelection;

            // Create the undo object.
            UndoMoveAction* undoMove = new UndoMoveAction(mOwner, "Move Objects");
            for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
               undoMove->addObject(mOwner->getAcquiredObject(i));

            mCurrentUndo = (UndoAction*)undoMove;

            return Parent::onMouseDown(sceneWindow,  mouseStatus );
         }
      }
   }

   // If we get here, then we must be selecting.
   mMouseState = Selecting;
   return Parent::onMouseDown( sceneWindow, mouseStatus );
}


//---------------------------------------------------------------------------------------------
// onMouseDragged
//---------------------------------------------------------------------------------------------
bool LevelBuilderSelectionTool::onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   if (m_AllowMultipleSelection && ((mMouseState == Selecting) || (mMouseState == SelectingWidget)) && (mouseStatus.dragRectNormal.extent.len() > 1))
   {
      mMouseState = DragSelecting;
      mDragRect = mouseStatus.dragRectNormal;
   }

   else if (mMouseState == DragSelecting)
   {
      mDragRect = mouseStatus.dragRectNormal;
   }

   else if (mMouseState == MovingSelection)
   {
      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
      {
         if ((mOwner->getAcquiredObject(i)->getAttachedToPath()) &&
             (!mOwner->isAcquired(mOwner->getAcquiredObject(i)->getAttachedToPath())))
         {
            mCantMove = true;
            return false;
         }
      }

      mCantMove = false;
      Vector2 finalPosition;
      move(mOwner, mOwner->getAcquiredObjects().getSize(), mouseStatus.mousePoint2D + mMouseOffset, finalPosition);

      mAddUndo = true;
      mOwner->getAcquiredObjects().setPosition(finalPosition);
      mOwner->onObjectSpatialChanged();
   }

   else if (mMouseState == SizingSelection)
   {
      Vector2 newSize, newPosition;
      bool flipX, flipY;

      // Alt scales uniformly.
      bool uniform = false;

      // Mounted or pathed objects force uniform scaling.
      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
      {
         if ( mOwner->getAcquiredObject(i)->getAttachedToPath() )
            uniform = true;
      }

      if (uniform)
         mForcingUniform = true;
      else
      {
         mForcingUniform = false;
         uniform = mouseStatus.event.modifier & SI_CTRL;
      }

      // Shift scales with maintained aspect ratio.
      bool shiftScale = false;

      // Rotated objects force shift scaling.
      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
      {
         SceneObject *object = mOwner->getAcquiredObject(i);
         F32 angle = mRadToDeg(object->getAngle());

         if (mNotZero(angle) &&
             mNotEqual(angle, 90.0f) &&
             mNotEqual(angle, 180.0f) &&
             mNotEqual(angle, 270.0f) &&
             mNotEqual(angle, 360.0f))
         {
            shiftScale = true;
         }

         // Clear angle.
         object->setAngle(0.0f);
      }

      if (shiftScale)
         mForcingShift = true;
      else
      {
         mForcingShift = false;
         shiftScale = mouseStatus.event.modifier & SI_SHIFT;
      }

      scale(mOwner, mScaleOrigSize, mScaleOrigPos, mouseStatus.mousePoint2D,
            uniform, shiftScale, mMouseDownAR, newSize, newPosition, flipX, flipY);

      mAddUndo = true;

      mOwner->getAcquiredObjects().setSize(newSize);
      if (!uniform) mOwner->getAcquiredObjects().setPosition(newPosition);
      if (flipX) mOwner->getAcquiredObjects().flipX();
      if (flipY) mOwner->getAcquiredObjects().flipY();

      // restore angles before callback.
      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
      {
         mOwner->getAcquiredObject(i)->setAngle(mAngleList[i]);
      }

      mOwner->onObjectSpatialChanged();
   }

   else if (mMouseState == RotatingSelection)
   {
      mAddUndo = true;
      F32 oldRotation = mOwner->getAcquiredObjects().getAngle();
      F32 newRotation;
      rotate(mOwner, mStartAngle, mAngleVector, mouseStatus.mousePoint2D - mOwner->getAcquiredObjects().getPosition(), newRotation);
      mOwner->getAcquiredObjects().rotate(newRotation - oldRotation);
      mOwner->onObjectSpatialChanged();
   }

   // Call Parent.
   return true;  
}

bool LevelBuilderSelectionTool::onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus )
{
   mMouseDown = false;

   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   bool shift = mouseStatus.event.modifier & SI_SHIFT;
   bool ctrl = mouseStatus.event.modifier & SI_CTRL;

   // If a selected object was clicked on, but the mouse never moved (or didn't move much),
   // then cycle the selection through the pick list. If control is down, then deselect.
   if ((mMouseState == MovingSelection) && !shift && (mouseStatus.pickList.size() > 0) && (mouseStatus.dragRectNormal.extent.len() < 1))
   {
      if (ctrl)
         mOwner->clearAcquisition(mouseStatus.firstPickPoint);

      else
      {
         if (mouseStatus.pickList.size() == 1)
         {
            if (mouseStatus.event.mouseClickCount == 2)
               Con::executef(mOwner, 2, "onObjectDoubleClicked", Con::getIntArg(mouseStatus.firstPickPoint->getId()));
            return Parent::onMouseUp( sceneWindow, mouseStatus );
         }

         for (S32 i = 0; i < mouseStatus.pickList.size(); i++)
         {
             if (mOwner->isAcquired(mouseStatus.pickList[i].mpSceneObject))
            {
               mOwner->clearAcquisition();
               if (i == (mouseStatus.pickList.size() - 1))
                   mOwner->requestAcquisition(mouseStatus.pickList[0].mpSceneObject);
               else
                   mOwner->requestAcquisition(mouseStatus.pickList[i + 1].mpSceneObject);

               break;
            }
         }
      }

      mMouseState = Selecting;
      return Parent::onMouseUp( sceneWindow, mouseStatus );
   }

   // Moving
   else if (mMouseState == MovingSelection)
   {
      if ((mAddUndo) && (mCurrentUndo))
      {
         UndoMoveAction* undoMove = (UndoMoveAction*)mCurrentUndo;
         for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
            undoMove->setNewPosition(mOwner->getAcquiredObject(i));

         mCurrentUndo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());
         mCurrentUndo = NULL;
      }
      else if (mCurrentUndo)
      {
         delete mCurrentUndo;
         mCurrentUndo = NULL;
      }
   }

   // Sizing 
   else if (mMouseState == SizingSelection)
   {
      mMouseState = Selecting;
      if ((mAddUndo) && (mCurrentUndo))
      {
         UndoScaleAction* undoScale = (UndoScaleAction*)mCurrentUndo;
         for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
            undoScale->setNewSize(mOwner->getAcquiredObject(i));

         mCurrentUndo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());
         mCurrentUndo = NULL;
      }
      else if (mCurrentUndo)
      {
         delete mCurrentUndo;
         mCurrentUndo = NULL;
      }

      // remove angle list
      delete [] mAngleList;
      mAngleList = NULL;

      // force bounds update
      mOwner->getAcquiredObjects().calculateObjectRect();

      return Parent::onMouseUp( sceneWindow, mouseStatus );
   }

   // Rotating
   else if (mMouseState == RotatingSelection)
   {
      mMouseState = Selecting;
      if ((mAddUndo) && (mCurrentUndo))
      {
         UndoRotateAction* undoScale = (UndoRotateAction*)mCurrentUndo;
         for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
            undoScale->setNewRotation(mOwner->getAcquiredObject(i));

         mCurrentUndo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());
         mCurrentUndo = NULL;
      }
      else if (mCurrentUndo)
      {
         delete mCurrentUndo;
         mCurrentUndo = NULL;
      }
      return Parent::onMouseUp( sceneWindow, mouseStatus );
   }

   // Drag selecting.
   else if (mMouseState == DragSelecting)
   {
      UndoSelectAction* undo = new UndoSelectAction(mOwner, "Select Objects");
      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undo->addOldObject(mOwner->getAcquiredObject(i));

      bool selectionChanged = false;

      if (!ctrl && !shift)
      {
         if (mOwner->hasAcquiredObjects())
            selectionChanged = true;
         mOwner->clearAcquisition();
      }

      for (S32 i = 0; i < mouseStatus.dragPickList.size(); i++)
      {
         if (getFullContainSelect())
         {
             RectF objAABB = mouseStatus.dragPickList[i].mpSceneObject->getAABBRectangle();
            Vector2 upperLeft = Vector2( objAABB.point.x, objAABB.point.y + objAABB.extent.y );
            Vector2 lowerRight = Vector2( objAABB.point.x + objAABB.extent.x, objAABB.point.y );
            Vector2 dragUpperLeft = mouseStatus.dragRectNormal2D.point;
            Vector2 dragLowerRight = mouseStatus.dragRectNormal2D.point + mouseStatus.dragRectNormal2D.extent;

            if (!((upperLeft.x > dragUpperLeft.x) && (upperLeft.y > dragUpperLeft.y) &&
                (lowerRight.x < dragLowerRight.x) && (lowerRight.y < dragLowerRight.y)))
               continue;
         }

         if (!mOwner->isAcquired(mouseStatus.dragPickList[i].mpSceneObject))
         {
             mOwner->requestAcquisition(mouseStatus.dragPickList[i].mpSceneObject);
            selectionChanged = true;
         }
         else if (ctrl && !shift)
         {
             mOwner->clearAcquisition(mouseStatus.dragPickList[i].mpSceneObject);
            selectionChanged = true;
         }
      }

      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undo->addNewObject(mOwner->getAcquiredObject(i));

      if (selectionChanged && mUndoSelections)
         undo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());
      else
         delete undo;

      mMouseState = Selecting;
      return Parent::onMouseUp( sceneWindow, mouseStatus );
   }

   // Mouse has not been dragged (or at least, not very much).
   else if (mMouseState == Selecting)
   {
      UndoSelectAction* undo = new UndoSelectAction(mOwner, "Select Objects");
      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undo->addOldObject(mOwner->getAcquiredObject(i));

      bool selectionChanged = false;

      if (!m_AllowMultipleSelection || (!shift && !ctrl))
      {
         if (mOwner->hasAcquiredObjects())
            selectionChanged = true;
         mOwner->clearAcquisition();
      }

      if (mouseStatus.firstPickPoint)
      {
         if (!mOwner->isAcquired(mouseStatus.firstPickPoint))
         {
            mOwner->requestAcquisition(mouseStatus.firstPickPoint);
            selectionChanged = true;
         }
      }

      for (S32 i = 0; i < mOwner->getAcquiredObjectCount(); i++)
         undo->addNewObject(mOwner->getAcquiredObject(i));

      if (selectionChanged && mUndoSelections)
         undo->addToManager(&sceneWindow->getSceneEdit()->getUndoManager());
      else
         delete undo;

      mMouseState = Selecting;
      return Parent::onMouseUp( sceneWindow, mouseStatus );
   }

   // Selecting a Widget
   else if (mMouseState == SelectingWidget)
   {
      if (mSelectedHoverObj && mOwner->isAcquired(mSelectedHoverObj))
      {
         // Grab the object's bounds.
         Point2I widgetSize = mWidgetBufferSize;
         RectI objRect = sceneWindow->getObjectBoundsWindow( mSelectedHoverObj );

         RectI widgetRect;
         Point2I startPoint = objRect.point - Point2I(mWidgetSelectBuffer, widgetSize.y + mWidgetSelectBuffer);
         for (S32 i = 0; i < mNumberOfWidgets; i++)
         {
            widgetRect.point = startPoint;
            widgetRect.extent = widgetSize;
            if ((i == 0) || (i == (mNumberOfWidgets - 1)))
               widgetRect.extent += Point2I(mWidgetSelectBuffer, mWidgetSelectBuffer);
            
            if (widgetRect.pointInRect(mouseStatus.event.mousePoint) && mCurrentWidgets[i] != NULL)
            {
               mCurrentWidgets[i]->doCallback(mOwner, mSelectedHoverObj);
               break;
            }

            startPoint.x += widgetRect.extent.x;
         }
      }
   }

   checkSelectedHoverObj(sceneWindow, mouseStatus);

   mCantMove = false;
   mForcingUniform = false;
   mForcingShift = false;
   mCurrentUndo = NULL;
   return Parent::onMouseUp( sceneWindow, mouseStatus );
}

// This handling needs to be improved, but it works pretty well in most cases.
// If multiple objects are selected and overlapping it can be slightly non-intuitive.
void LevelBuilderSelectionTool::checkSelectedHoverObj(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus& mouseStatus)
{
   // If we already have a selected hover object, we don't want to relinquish it unless it is both not being hovered
   // over and its widgets (plus a buffer) are not being hovered over.
   if (mSelectedHoverObj && sceneWindow->getSceneEdit()->isAcquired(mSelectedHoverObj))
   {
      Point2I widgetSize = mWidgetBufferSize;

      // Grab the object's bounds.
      RectI objRect = sceneWindow->getObjectBoundsWindow( mSelectedHoverObj );
      // And the window bounds.
      RectI widgetBounds = RectI(Point2I(objRect.point.x - mWidgetSelectBuffer, objRect.point.y - (widgetSize.y + mWidgetSelectBuffer)),
                                 Point2I(widgetSize.x * mNumberOfWidgets + (mWidgetSelectBuffer * 2), widgetSize.y + mWidgetSelectBuffer));

      if (objRect.pointInRect(mouseStatus.event.mousePoint) || widgetBounds.pointInRect(mouseStatus.event.mousePoint))
      {
         // Don't change!
         return;
      }
   }

   if (mouseStatus.pickList.size() < 1)
   {
      setSelectedHoverObj(NULL);
      return;
   }

   else
   {
      for (S32 i = 0; i < mouseStatus.pickList.size(); i++)
      {
          if (sceneWindow->getSceneEdit()->isAcquired(mouseStatus.pickList[i].mpSceneObject))
         {
             setSelectedHoverObj(mouseStatus.pickList[i].mpSceneObject);
            return;
         }
      }
   }

   setSelectedHoverObj(NULL);
}

void LevelBuilderSelectionTool::refreshSelectedHoverObj()
{
   setSelectedHoverObj(mSelectedHoverObj);
}

ConsoleMethod(LevelBuilderSelectionTool, RefreshWidgets, void, 2, 2, "%tool.refreshWidgets")
{
   object->refreshSelectedHoverObj();
}

void LevelBuilderSelectionTool::setSelectedHoverObj(const SceneObject* object)
{
   mSelectedHoverObj = object;
   mHoverWidget = NULL;
   mNumberOfWidgets = 0;

   for (S32 i = 0; i < mMaxWidgets; i++)
      mCurrentWidgets[i] = NULL;

   if (object)
   {
      S32 index = 0;
      for (S32 i = 0; i < mWidgets.size(); i++)
      {
         if (mWidgets[i]->isDisplayed(mSelectedHoverObj))
         {
            // If we haven't filled it yet, just add to the array.
            if (index < mMaxWidgets)
               mCurrentWidgets[index++] = mWidgets[i];
         }
      }
      mNumberOfWidgets = index;
   }
}

void LevelBuilderSelectionTool::onRenderScene(LevelBuilderSceneWindow* sceneWindow)
{
   LevelBuilderSceneEdit* mOwner = sceneWindow->getSceneEdit();
   // Render Parent first
   Parent::onRenderScene( sceneWindow );

   // Draw Hover Outline
   if( mHoverObj && !mOwner->isAcquired(mHoverObj))
   {
      // Grab the object's bounds.
      RectI objRect = sceneWindow->getObjectBoundsWindow( mHoverObj );
      objRect.point = sceneWindow->localToGlobalCoord(objRect.point);

      // Render the bounding box.
      dglDrawRect( objRect, mHoverFillColor );
   }

   // Draw selected hover object's widgets only in the current window.
   if (mSelectedHoverObj && mOwner->isAcquired(mSelectedHoverObj))
   {
      // Grab the object's bounds.
      Point2I widgetSize = mWidgetBufferSize;
      RectI objRect = sceneWindow->getObjectBoundsWindow( mSelectedHoverObj );
      objRect.point = sceneWindow->localToGlobalCoord(objRect.point);

      RectI widgetRect;
      Point2I startPoint = objRect.point - Point2I(0, widgetSize.y);
      for (S32 i = 0; i < mNumberOfWidgets; i++)
      {
         widgetRect.point = startPoint;
         widgetRect.extent = widgetSize;
         
         dglSetBitmapModulation(ColorF(1.0f, 1.0f, 1.0f, 1.0f));
         if (mCurrentWidgets[i] && mCurrentWidgets[i]->mTexture)
         {
            // Draw Widget Background?
            if( sceneWindow->mProfile != NULL && sceneWindow->mProfile->mBorder == -2 )
               renderBorder( widgetRect, sceneWindow->mProfile );

            // Draw Widget Icon
            RectI iconRect = widgetRect;
            iconRect.inset((mWidgetBufferSize.x - mWidgetSize.x) >> 1, (mWidgetBufferSize.y - mWidgetSize.y) >> 1);
            dglDrawBitmapStretch( mCurrentWidgets[i]->mTexture, iconRect);

            if( mCurrentWidgets[i] == mHoverWidget )
            {
               if( dStrlen( mCurrentWidgets[i]->mToolTip ) > 0 )
                  sceneWindow->renderTooltip( widgetRect.point, mCurrentWidgets[i]->mToolTip );
            }
         }

         startPoint.x += widgetSize.x;
      }
   }

   // Draw the drag selection.
   if( mMouseState == DragSelecting )
   {
      dglDrawRect( mDragRect, ColorI( mHoverOutlineColor ));
   }

   else if ((mMouseState == MovingSelection) && mCantMove)
   {
      sceneWindow->renderTooltip( mTooltipPosition, "Cannot move mounted or pathed objects separate from their mount." );
   }

   else if ((mMouseState == SizingSelection) && (mForcingUniform || mForcingShift))
   {
      if (mForcingUniform)
         sceneWindow->renderTooltip( mTooltipPosition, "Mounted or pathed objects must be scaled without moving." );
      else if (mForcingShift)
         sceneWindow->renderTooltip( mTooltipPosition, "Objects rotated at non 90 degree increments must be scaled uniformly." );
   }

   // Render a bounding box around each acquired object and a scaling nut at the 4 corners and
   // edges around the selection rect.
   if( mOwner->hasAcquiredObjects() )
   {
      // Draw the object bounding boxes.
      for (U32 i = 0; i < (U32)mOwner->getAcquiredObjectCount(); i++)
      {
         SceneObject *obj = mOwner->getAcquiredObject(i);

         // Render bounding box
         RectI objRect = sceneWindow->getObjectBoundsWindow( obj );
         objRect.point = sceneWindow->localToGlobalCoord(objRect.point);

         // Draw Bounding Box
         dglDrawRect(objRect, mHoverOutlineColor, m_HoverOutlineWidth);
      }

      // Draw the 8 scaling nuts.
	  if (m_AllowSizing)
		drawSizingNuts(sceneWindow, mOwner->getAcquiredObjects().getBoundingRect());
   }
}

ConsoleMethod(LevelBuilderSelectionTool, addWidget, void, 3, 3, "%tool.addWidget(widget)")
{
   SelectionToolWidget* widget = dynamic_cast<SelectionToolWidget*>(Sim::findObject(argv[2]));
   if (widget)
      object->addWidget(widget);
   else
      Con::warnf("Invalid widget past to LevelBuilderSelectionTool::addWidget");
}

void LevelBuilderSelectionTool::addWidget(SelectionToolWidget* widget)
{
   mWidgets.push_back(widget);
}

//---------------------------------------------------------------------------------------------
// Console Functionality
//---------------------------------------------------------------------------------------------
ConsoleMethod( LevelBuilderSelectionTool, setFullContainSelect, void, 3, 3, "%tool.setFullContainSelect(true/false)")
{
   bool fullSelect = dAtob( argv[2] );
   object->setFullContainSelect( fullSelect );
}

ConsoleMethod( LevelBuilderSelectionTool, getFullContainSelect, bool, 2, 2, "%tool.getFullContainSelect()")
{
   return object->getFullContainSelect();
}

ConsoleMethod( LevelBuilderSelectionTool, setUndoSelections, void, 3, 3, "%tool.setUndoSelections(true/false)")
{
   bool undoSelections = dAtob( argv[2] );
   object->setUndoSelections( undoSelections );
}

ConsoleMethod( LevelBuilderSelectionTool, getUndoSelections, bool, 2, 2, "%tool.getUndoSelections()")
{
   return object->getUndoSelections();
}

ConsoleMethod(LevelBuilderSelectionTool, setHoverOutlineColor, void, 6, 6, "%tool.setHoverOutlineColor()")
{
	object->setHoverOutlineColor(ColorI(dAtoi(argv[2]), dAtoi(argv[3]), dAtoi(argv[4]), dAtoi(argv[5])));
}

ConsoleMethod(LevelBuilderSelectionTool, setHoverOutlineWidth, void, 3, 3, "%tool.setHoverOutlineWidth()")
{
	object->setHoverOutlineWidth(dAtof(argv[2]));
}

ConsoleMethod(LevelBuilderSelectionTool, setAllowSizing, void, 3, 3, "%tool.setAllowSizing()")
{
	object->setAllowSizing(dAtob(argv[2]));
}

ConsoleMethod(LevelBuilderSelectionTool, setAllowMultipleSelection, void, 3, 3, "%tool.setAllowMultipleSelection()")
{
	object->setAllowMultipleSelection(dAtob(argv[2]));
}


#endif // TORQUE_TOOLS
