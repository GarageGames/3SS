//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
 
#include "console/consoleTypes.h"
#include "console/console.h"
#include "graphics/dgl.h"
#include "gui/guiCanvas.h"
#include "gui/guiConsoleEditCtrl.h"
#include "memory/frameAllocator.h"

IMPLEMENT_CONOBJECT(GuiConsoleEditCtrl);

GuiConsoleEditCtrl::GuiConsoleEditCtrl()
{
   mSinkAllKeyEvents = true;
   mSiblingScroller = NULL;
   mUseSiblingScroller = true;
}

void GuiConsoleEditCtrl::initPersistFields()
{
   Parent::initPersistFields();

   addGroup("GuiConsoleEditCtrl");
   addField("useSiblingScroller", TypeBool, Offset(mUseSiblingScroller, GuiConsoleEditCtrl));
   endGroup("GuiConsoleEditCtrl");
}

bool GuiConsoleEditCtrl::onKeyDown(const GuiEvent &event)
{
   // UNUSED: JOSEPH THOMAS -> S32 stringLen = dStrlen(mText);
   setUpdate();

   if (event.keyCode == KEY_TAB) 
   {
      // Get a buffer that can hold the completed text...
      FrameTemp<UTF8> tmpBuff(GuiTextCtrl::MAX_STRING_LENGTH);
      // And copy the text to be completed into it.
      mTextBuffer.getCopy8(tmpBuff, GuiTextCtrl::MAX_STRING_LENGTH);

      // perform the completion
      bool forward = event.modifier & SI_SHIFT;
      mCursorPos = Con::tabComplete(tmpBuff, mCursorPos, GuiTextCtrl::MAX_STRING_LENGTH, forward);

      // place results in our buffer.
      mTextBuffer.set(tmpBuff);
      return true;
   }
   else if ((event.keyCode == KEY_PAGE_UP) || (event.keyCode == KEY_PAGE_DOWN)) 
   {
      // See if there's some other widget that can scroll the console history.
      if (mUseSiblingScroller) 
      {
         if (mSiblingScroller) 
         {
            return mSiblingScroller->onKeyDown(event);
         }
         else 
         {
            // Let's see if we can find it...
            SimGroup* pGroup = getGroup();
            if (pGroup) 
            {
               // Find the first scroll control in the same group as us.
               for (SimSetIterator itr(pGroup); *itr; ++itr) 
               {
                  if ((mSiblingScroller = dynamic_cast<GuiScrollCtrl*>(*itr))) 
                  {
                     return mSiblingScroller->onKeyDown(event);
                  }
               }
            }

            // No luck... so don't try, next time.
            mUseSiblingScroller = false;
         }
      }
   }

   return Parent::onKeyDown(event);
}

