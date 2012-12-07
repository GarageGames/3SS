//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUIBUTTONCTRL_H_
#define _GUIBUTTONCTRL_H_

#ifndef _GUIBUTTONBASECTRL_H_
#include "gui/buttons/guiButtonBaseCtrl.h"
#endif

class GuiButtonCtrl : public GuiButtonBaseCtrl
{
   typedef GuiButtonBaseCtrl Parent;
protected:
   bool mHasTheme;
public:
   DECLARE_CONOBJECT(GuiButtonCtrl);
   GuiButtonCtrl();
   bool onWake();
   void onRender(Point2I offset, const RectI &updateRect);
};

#endif //_GUI_BUTTON_CTRL_H
