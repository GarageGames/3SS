//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUIEDITCTRL_H_
#define _GUIEDITCTRL_H_

#include "gui/guiControl.h"
#include "collection/undo.h"

class GuiEditCtrl : public GuiControl
{
   typedef GuiControl Parent;

   Vector<GuiControl *> mSelectedControls;
   GuiControl*          mCurrentAddSet;
   GuiControl*          mContentControl;
   Point2I              mLastMousePos;
   Point2I              mSelectionAnchor;
   Point2I              mGridSnap;
   Point2I				   mDragBeginPoint;
   Vector<Point2I>		mDragBeginPoints;

   // Undo
   UndoManager          mUndoManager;
   SimGroup             mTrash;
   SimSet               mSelectedSet;
   
   // Sizing Cursors
   GuiCursor*        mDefaultCursor;
   GuiCursor*        mLeftRightCursor;
   GuiCursor*        mUpDownCursor;
   GuiCursor*        mNWSECursor;
   GuiCursor*        mNESWCursor;
   GuiCursor*        mMoveCursor;

   enum mouseModes { Selecting, MovingSelection, SizingSelection, DragSelecting };
   enum sizingModes { sizingNone = 0, sizingLeft = 1, sizingRight = 2, sizingTop = 4, sizingBottom = 8 };

   mouseModes             mMouseDownMode;
   sizingModes             mSizingMode;

   // private methods
   void updateSelectedSet();

  public:
   GuiEditCtrl();
   DECLARE_CONOBJECT(GuiEditCtrl);

   bool onWake();
   void onSleep();

   void select(GuiControl *ctrl);
   void setRoot(GuiControl *ctrl);
   void setEditMode(bool value);
   S32 getSizingHitKnobs(const Point2I &pt, const RectI &box);
   void getDragRect(RectI &b);
   void drawNut(const Point2I &nut, ColorI &outlineColor, ColorI &nutColor);
   void drawNuts(RectI &box, ColorI &outlineColor, ColorI &nutColor);
   void onPreRender();
   void onRender(Point2I offset, const RectI &updateRect);
   void addNewControl(GuiControl *ctrl);
   bool selectionContains(GuiControl *ctrl);
   void setCurrentAddSet(GuiControl *ctrl, bool clearSelection = true);
   const GuiControl* getCurrentAddSet() const;
   void setSelection(GuiControl *ctrl, bool inclusive = false);

   // Undo Access
   void undo();
   void redo();
   UndoManager& getUndoManager() { return mUndoManager; }

   // When a control is changed by the inspector
   void controlInspectPreApply(GuiControl* object);
   void controlInspectPostApply(GuiControl* object);

   // Sizing Cursors
   bool initCursors();
   void getCursor(GuiCursor *&cursor, bool &showCursor, const GuiEvent &lastGuiEvent);


   const Vector<GuiControl *> *getSelected() const { return &mSelectedControls; }
   const SimSet& getSelectedSet() { updateSelectedSet(); return mSelectedSet; }
   const SimGroup& getTrash() { return mTrash; }
   const GuiControl *getAddSet() const { return mCurrentAddSet; }; //JDD

   bool onKeyDown(const GuiEvent &event);
   void onMouseDown(const GuiEvent &event);
   void onMouseUp(const GuiEvent &event);
   void onMouseDragged(const GuiEvent &event);
   void onRightMouseDown(const GuiEvent &event);

   virtual bool onAdd();
   virtual void onRemove();

   enum Justification {
      JUSTIFY_LEFT,
      JUSTIFY_CENTER,
      JUSTIFY_RIGHT,
      JUSTIFY_TOP,
      JUSTIFY_BOTTOM,
      SPACING_VERTICAL,
      SPACING_HORIZONTAL
   };

   void justifySelection( Justification j);
   void moveSelection(const Point2I &delta);
   void moveAndSnapSelection(const Point2I &delta);
   void saveSelection(const char *filename);
   void loadSelection(const char *filename);
   void addSelection(S32 id);
   void removeSelection(S32 id);
   void deleteSelection();
   void clearSelection();
   void selectAll();
   void bringToFront();
   void pushToBack();
   void setSnapToGrid(U32 gridsize);
   void moveSelectionToCtrl(GuiControl*);
};

#endif //_GUI_EDIT_CTRL_H
