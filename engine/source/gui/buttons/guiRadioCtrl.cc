//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/console.h"
#include "graphics/dgl.h"
#include "gui/guiCanvas.h"
#include "gui/buttons/guiRadioCtrl.h"
#include "console/consoleTypes.h"

//---------------------------------------------------------------------------
IMPLEMENT_CONOBJECT(GuiRadioCtrl);

GuiRadioCtrl::GuiRadioCtrl()
{
   mButtonType = ButtonTypeRadio;
}
