//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUIDYNAMICCTRLARRAYCTRL_H_
#define _GUIDYNAMICCTRLARRAYCTRL_H_

#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif

#include "graphics/dgl.h"
#include "console/console.h"
#include "console/consoleTypes.h"

class GuiDynamicCtrlArrayControl : public GuiControl
{
private:
   typedef GuiControl Parent;

   S32 mCols;
   S32 mRowSize;
   S32 mColSize;
   S32 mRowSpacing;
   S32 mColSpacing;
   bool mResizing;

public:
   GuiDynamicCtrlArrayControl();
   virtual ~GuiDynamicCtrlArrayControl();

   void updateChildControls();
   void resize(const Point2I &newPosition, const Point2I &newExtent);

   void addObject(SimObject *obj);

   void childResized(GuiControl *child);

   void inspectPostApply();

   static void initPersistFields();
   DECLARE_CONOBJECT(GuiDynamicCtrlArrayControl);
};

#endif // _GUIDYNAMICCTRLARRAYCTRL_H_