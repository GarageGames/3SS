//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUIFORMCTRL_H_
#define _GUIFORMCTRL_H_

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif

#ifndef _GUICANVAS_H_
#include "gui/guiCanvas.h"
#endif

#include "graphics/dgl.h"
#include "console/console.h"
#include "console/consoleTypes.h"

class GuiMenuBar;

/// Collapsable pane control.
///
/// This class wraps a single child control and displays a header with caption
/// above it. If you click the header it will collapse or expand. The control
/// resizes itself based on its collapsed/expanded size.
///
/// In the GUI editor, if you just want the header you can make collapsable 
/// false. The caption field lets you set the caption. It expects a bitmap
/// (from the GuiControlProfile) that contains two images - the first is
/// displayed when the control is expanded and the second is displayed when
/// it is collapsed. The header is sized based off of the first image.
class GuiFormCtrl : public GuiControl
{
private:
   typedef GuiControl Parent;

   Resource<GFont>  mFont;
   StringTableEntry mCaption;
   bool             mCanMove;
   bool             mUseSmallCaption;
   StringTableEntry mSmallCaption;
   StringTableEntry mContentLibrary;
   StringTableEntry mContent;

   Point2I          mThumbSize;
   bool             mHasMenu;

   GuiMenuBar*      mMenuBar;

   bool mMouseMovingWin;

   Point2I mMouseDownPosition;
   RectI mOrigBounds;
   RectI mStandardBounds;

   RectI mCloseButton;
   RectI mMinimizeButton;
   RectI mMaximizeButton;

   bool mMouseOver;
   bool mDepressed;

public:
   GuiFormCtrl();
   virtual ~GuiFormCtrl();

   void setCaption(const char* caption);

   void resize(const Point2I &newPosition, const Point2I &newExtent);
   void onRender(Point2I offset, const RectI &updateRect);

   // DAW: Called when the GUI theme changes and a bitmap arrary may need updating
  // void onThemeChange();

   U32  getMenuBarID();

   bool onAdd();
   bool onWake();
   void onSleep();

   virtual void addObject(SimObject *newObj );

   void onMouseDragged(const GuiEvent &event);
   void onMouseDown(const GuiEvent &event);
   void onMouseUp(const GuiEvent &event);
   void onMouseMove(const GuiEvent &event);
   void onMouseLeave(const GuiEvent &event);
   void onMouseEnter(const GuiEvent &event);

   static void initPersistFields();
   DECLARE_CONOBJECT(GuiFormCtrl);
};

#endif