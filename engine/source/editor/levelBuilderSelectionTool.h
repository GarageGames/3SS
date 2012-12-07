//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef TORQUE_PLAYER

#ifndef _LEVELBUILDERSELECTIONTOOL_H_
#define _LEVELBUILDERSELECTIONTOOL_H_

#ifndef _SCENE_WINDOW_H_
#include "2d/gui/SceneWindow.h"
#endif

#ifndef _SCENE_OBJECT_H_
#include "2d/sceneobject/SceneObject.h"
#endif

#ifndef _LEVELBUILDERBASEEDITTOOL_H_
#include "editor/levelBuilderBaseEditTool.h"
#endif

//-----------------------------------------------------------------------------
// SelectionToolWidget
//-----------------------------------------------------------------------------
class SelectionToolWidget : public SimObject
{
    typedef SimObject Parent;

public:
   enum eDisplayRules
   {
      UnPathedOnly = 1,
      PathedOnly = 2,

      NoDisplayRules = 0
   };

   // The texture to draw for this widget.
   TextureHandle mTexture;

   // The tooltip to render when this widget is hovered over.
   StringTableEntry mToolTip;

   // Since different classes may have more than 4 widgets, the four with the highest
   // priority will be displayed.
   S32 mPriority;

   // This is the position this widget prefers to be drawn at (0 - upper left, 1 - upper right,
   // 2 - lower left, 3 - lower right, -1 - no preference).
   S32 mPosition;

   // These are the classes this widget should be or should not be shown for.
   Vector<StringTableEntry> mClasses;

   // Determines whether the previous list contains classes to show, or not show.
   bool mShowClasses;

   // Display rule flags.
   U32 mDisplayRules;

   // The script method to call when this is pressed.
   StringTableEntry mCallback;

   SelectionToolWidget();

   static void initPersistFields();

   // Checks if an object (based on its class) should show this widget.
   bool isDisplayed(const SceneObject* object)
   {
      if ((object->getAttachedToPath() && (mDisplayRules & UnPathedOnly)) ||
          (!object->getAttachedToPath() && (mDisplayRules & PathedOnly)))
      {
         return false;
      }

      StringTableEntry className = object->getClassName();
      for (S32 i = 0; i < mClasses.size(); i++)
      {
         if (dStricmp(className, mClasses[i]) == 0)
            return mShowClasses;
      }
      return !mShowClasses;
   };

   void addClass(const char* className)
   {
      mClasses.push_back(StringTable->insert(className));
   };

   void setTexture(const char* textureName)
   {
      mTexture = TextureHandle(textureName, TextureHandle::BitmapTexture);
   };

   void setDisplayRule(eDisplayRules displayRule)
   {
      mDisplayRules |= displayRule;
   };

   // Calls the script function defined in 'mCallback'.
   void doCallback(LevelBuilderSceneEdit* sceneEdit, const SceneObject* object)
   {
      Con::executef(sceneEdit, 2, mCallback, Con::getIntArg(object->getId()));
   };

    DECLARE_CONOBJECT( SelectionToolWidget );
};

//-----------------------------------------------------------------------------
// LevelBuilderSelectionTool
//-----------------------------------------------------------------------------
class LevelBuilderSelectionTool : public LevelBuilderBaseEditTool
{
    typedef LevelBuilderBaseEditTool Parent;

private:
   UndoAction* mCurrentUndo;
   bool        mAddUndo;

   // True when the left mouse button is down
   bool        mMouseDown;
   F32         mMouseDownAR;
   // True if trying to drag a mounted object without its mount.
   bool        mCantMove;
   bool        mForcingUniform;
   bool        mForcingShift;
   Point2I     mTooltipPosition;

   TextureHandle mTexture;
   Point2I     mWidgetSize;
   Point2I     mWidgetBufferSize;
   S32         mWidgetSelectBuffer;

   Vector<SelectionToolWidget*> mWidgets;
   SelectionToolWidget *mHoverWidget;

   // True to Allow Sizing of the acquired object
   bool m_AllowSizing;

   // True to Allow Selection of Multiple Objects
   bool m_AllowMultipleSelection;

protected:
   // Mouse States.
   enum objectMouseModes
   {
      Selecting,
      DragSelecting,
      MovingSelection,
      SizingSelection,
      RotatingSelection,
      SelectingWidget
   };
   
   U32                      mMouseState;

   // The hover object is the object that would be selected if the mouse were
   // clicked at its current point.
    const SceneObject*    mHoverObj;

   // The selected hover object is the object for which the widgets are being shown.
   const SceneObject*    mSelectedHoverObj;

   static const S32 mMaxWidgets = 8;
   // These are the widgets to be rendered for the current selected hover obj.
   SelectionToolWidget* mCurrentWidgets[mMaxWidgets];
   S32 mNumberOfWidgets;

   // The colors and width with which to draw the hover object rect.
    ColorI                   mHoverOutlineColor;
    float					 m_HoverOutlineWidth;
    ColorI                   mHoverFillColor;

   // The window space rectangle of the current drag rect.
   RectI                    mDragRect;

   // Properties
   bool                     mFullContainSelect;
   bool                     mUndoSelections;

   // This is the offset from an object's center that the mouse was clicked.
   Vector2 mMouseOffset;
   // These store info about the angle and mouse status at the start angle.
   F32 mStartAngle;
   Vector2 mAngleVector;

   // When scaling, always use orig pos/size while dragging (so we don't accum error)
   // Bug fix #TGB-138: remove/restore rotations during sizing to avoid the matrix blowout
   Vector2 mScaleOrigPos;
   Vector2 mScaleOrigSize;
   F32 *mAngleList;

   // This function finds the selected hover object based on a mouse status.
   void checkSelectedHoverObj(LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus& mouseStatus);
   void setSelectedHoverObj(const SceneObject* object);

public:
    LevelBuilderSelectionTool();
    ~LevelBuilderSelectionTool();

   void refreshSelectedHoverObj();

    // Acquired Mouse Events
   virtual bool onMouseMove( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseDown( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseDragged( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onMouseUp( LevelBuilderSceneWindow* sceneWindow, const EditMouseStatus &mouseStatus );
    virtual bool onKeyDown(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event);
    virtual bool onKeyRepeat(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event);
   virtual bool onKeyUp(LevelBuilderSceneWindow* sceneWindow, const GuiEvent &event);

   // Add Widgets
   void addWidget(SelectionToolWidget* widget);

    // Tool Rendering.
    void onRenderScene(LevelBuilderSceneWindow* sceneWindow);

   // Property Accessors
   inline void setFullContainSelect(bool set) { mFullContainSelect = set; };
   inline bool getFullContainSelect() const   { return mFullContainSelect; };
   
   inline void setUndoSelections(bool set) { mUndoSelections = set; };
   inline bool getUndoSelections() const   { return mUndoSelections; };

   inline void setHoverOutlineColor(ColorI hoverOutlineColor)			{ mHoverOutlineColor = hoverOutlineColor; }
   inline void setHoverOutlineWidth(float hoverOutlineWidth)			{ m_HoverOutlineWidth = hoverOutlineWidth; }
   inline void setAllowSizing(bool allowSizing)							{ m_AllowSizing = allowSizing; };
   inline void setAllowMultipleSelection(bool allowMultipleSelection)	{ m_AllowMultipleSelection = allowMultipleSelection; };

    // Declare our Console Object
    DECLARE_CONOBJECT( LevelBuilderSelectionTool );
};

// Undo Action Types
class UndoMoveAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   struct UndoObject
   {
      UndoObject(SceneObject* _object, Vector2 _oldPosition) { object = _object; oldPosition = _oldPosition; };
      SceneObject* object;
      Vector2 oldPosition;
      Vector2 newPosition;
   };

   Vector<UndoObject> mObjects;

   // We need this so we can send notifications of objects changing.
   LevelBuilderSceneEdit* mSceneEdit;

public:
   UndoMoveAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* name) : UndoAction(name) { mSceneEdit = sceneEdit; };

   void addObject(SceneObject* object)
   {
      mObjects.push_back(UndoObject(object, object->getPosition()));
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if (itr->object == object)
         {
            mObjects.erase_fast(itr);
            break;
         }
      }

      if (mObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   }

   void setNewPosition(SceneObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if ((*itr).object == object)
         {
            (*itr).newPosition = object->getPosition();
            return;
         }
      }
   };

   virtual void undo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         if (!mObjects[i].object) continue;
         SceneObject* object = mObjects[i].object;
         object->setPosition(mObjects[i].oldPosition);
         mSceneEdit->onObjectSpatialChanged(object);
      }
   };

   virtual void redo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         if (!mObjects[i].object) continue;
         SceneObject* object = mObjects[i].object;
         object->setPosition(mObjects[i].newPosition);
         mSceneEdit->onObjectSpatialChanged(object);
      }
   }
};

// Undo Action Types
class UndoScaleAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   struct UndoObject
   {
      UndoObject(SceneObject* _object, Vector2 _oldSize, Vector2 _oldPosition, bool _oldFlipX, bool _oldFlipY)
      {
          object = _object;
          oldSize = _oldSize;
          oldPosition = _oldPosition;
          oldFlipX = _oldFlipX;
          oldFlipY = _oldFlipY;
      };
      SceneObject* object;
      Vector2 oldSize;
      Vector2 oldPosition;
      bool oldFlipX;
      bool oldFlipY;
      Vector2 newSize;
      Vector2 newPosition;
      bool newFlipX;
      bool newFlipY;
   };

   Vector<UndoObject> mObjects;

   // We need this so we can send notifications of objects changing.
   LevelBuilderSceneEdit* mSceneEdit;

public:
   UndoScaleAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* name) : UndoAction(name) { mSceneEdit = sceneEdit; };

   void addObject(SceneObject* object)
   {
      mObjects.push_back(UndoObject(object, object->getSize(), object->getPosition(), object->getFlipX(), object->getFlipY()));
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if (itr->object == object)
         {
            mObjects.erase_fast(itr);
            break;
         }
      }

      if (mObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   }

   void setNewSize(SceneObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if ((*itr).object == object)
         {
            (*itr).newSize = object->getSize();
            (*itr).newPosition = object->getPosition();
            (*itr).newFlipX = object->getFlipX();
            (*itr).newFlipY = object->getFlipY();
            return;
         }
      }
   };

   virtual void undo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         if (!mObjects[i].object) continue;
         SceneObject* object = mObjects[i].object;
         object->setSize(mObjects[i].oldSize);
         object->setPosition(mObjects[i].oldPosition);
         object->setFlipX(mObjects[i].oldFlipX);
         object->setFlipY(mObjects[i].oldFlipY);
         mSceneEdit->onObjectSpatialChanged(object);
      }
   };

   virtual void redo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         if (!mObjects[i].object) continue;
         SceneObject* object = mObjects[i].object;
         object->setSize(mObjects[i].newSize);
         object->setPosition(mObjects[i].newPosition);
         object->setFlipX(mObjects[i].newFlipX);
         object->setFlipY(mObjects[i].newFlipY);
         mSceneEdit->onObjectSpatialChanged(object);
      }
   }
};

// Undo Action Types
class UndoSelectAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   Vector<SceneObject*> mOldObjects;
   Vector<SceneObject*> mNewObjects;

   // We need this so we can send notifications of objects changing.
   LevelBuilderSceneEdit* mSceneEdit;

public:
   UndoSelectAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* name) : UndoAction(name) { mSceneEdit = sceneEdit; };

   void addOldObject(SceneObject* object)
   {
      mOldObjects.push_back(object);
      deleteNotify(object);
   };
   void addNewObject(SceneObject* object)
   {
      mNewObjects.push_back(object);
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<SceneObject*>::iterator itr;
      for (itr = mOldObjects.begin(); itr != mOldObjects.end(); itr++)
      {
         if (*itr == object)
         {
            mOldObjects.erase_fast(itr);
            break;
         }
      }

      Vector<SceneObject*>::iterator itr2;
      for (itr2 = mNewObjects.begin(); itr2 != mNewObjects.end(); itr2++)
      {
         if (*itr2 == object)
         {
            mNewObjects.erase_fast(itr2);
            break;
         }
      }

      if (mOldObjects.empty() && mNewObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   }

   virtual void undo()
   {
      mSceneEdit->clearAcquisition();
      for (S32 i = 0; i < mOldObjects.size(); i++)
      {
         if (!mOldObjects[i]) continue;
         mSceneEdit->acquireObject(mOldObjects[i]);
      }
   };

   virtual void redo()
   {
      mSceneEdit->clearAcquisition();
      for (S32 i = 0; i < mNewObjects.size(); i++)
      {
         SimObjectPtr<SceneObject> object = mNewObjects[i];
         if (!object) continue;
         mSceneEdit->acquireObject(mNewObjects[i]);
      }
   }
};

// Undo Action Types
class UndoRotateAction : public UndoAction
{
   typedef UndoAction Parent;

private:
   struct UndoObject
   {
      UndoObject(SceneObject* _object, F32 _oldAngle) { object = _object; oldAngle = _oldAngle; };
      SceneObject* object;
      F32 oldAngle;
      F32 newAngle;
   };

   Vector<UndoObject> mObjects;

   // We need this so we can send notifications of objects changing.
   LevelBuilderSceneEdit* mSceneEdit;

public:
   UndoRotateAction(LevelBuilderSceneEdit* sceneEdit, const UTF8* name) : UndoAction(name) { mSceneEdit = sceneEdit; };

   void addObject(SceneObject* object)
   {
      mObjects.push_back(UndoObject(object, object->getAngle()));
      deleteNotify(object);
   };

   virtual void onDeleteNotify(SimObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if (itr->object == object)
         {
            mObjects.erase_fast(itr);
            break;
         }
      }

      if (mObjects.empty() && mUndoManager)
         mUndoManager->removeAction(this);
   };

   void setNewRotation(SceneObject* object)
   {
      Vector<UndoObject>::iterator itr;
      for (itr = mObjects.begin(); itr != mObjects.end(); itr++)
      {
         if ((*itr).object == object)
         {
            (*itr).newAngle = object->getAngle();
            return;
         }
      }
   };

   virtual void undo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         if (!mObjects[i].object) continue;
         SceneObject* object = mObjects[i].object;
         object->setAngle(mObjects[i].oldAngle);
         mSceneEdit->onObjectSpatialChanged(object);
      }
   };

   virtual void redo()
   {
      for (S32 i = 0; i < mObjects.size(); i++)
      {
         if (!mObjects[i].object) continue;
         SceneObject* object = mObjects[i].object;
         object->setAngle(mObjects[i].newAngle);
         mSceneEdit->onObjectSpatialChanged(object);
      }
   }
};

#endif


#endif // TORQUE_TOOLS
