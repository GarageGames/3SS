//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "gui/guiPopUpCtrl.h"
#include "gui/guiCanvas.h"
#include "gui/guiInputCtrl.h"

class GuiControlListPopUp : public GuiPopUpMenuCtrl
{
   typedef GuiPopUpMenuCtrl Parent;
public:
   bool onAdd();

   DECLARE_CONOBJECT(GuiControlListPopUp);
};

IMPLEMENT_CONOBJECT(GuiControlListPopUp);

bool GuiControlListPopUp::onAdd()
{
   if(!Parent::onAdd())
      return false;
   clear();

   AbstractClassRep *guiCtrlRep = GuiControl::getStaticClassRep();
   AbstractClassRep *guiCanvasRep = GuiCanvas::getStaticClassRep();
   AbstractClassRep *guiInputRep = GuiInputCtrl::getStaticClassRep();

   for(AbstractClassRep *rep = AbstractClassRep::getClassList(); rep; rep = rep->getNextClass())
   {
      if( rep->isClass(guiCtrlRep))
      {
         if(!rep->isClass(guiCanvasRep) && !rep->isClass(guiInputRep))
            addEntry(rep->getClassName(), 0);
      }
   }

   // We want to be alphabetical!
   sort();

   return true;
}
