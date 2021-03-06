//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "console/consoleTypes.h"
#include "console/console.h"
#include "graphics/color.h"
#include "gui/guiConsoleTextCtrl.h"
#include "graphics/dgl.h"

IMPLEMENT_CONOBJECT(GuiConsoleTextCtrl);

// -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- //

GuiConsoleTextCtrl::GuiConsoleTextCtrl()
{
   //default fonts
   mConsoleExpression = NULL;
   mResult = NULL;
}

void GuiConsoleTextCtrl::initPersistFields()
{
   Parent::initPersistFields();
   addGroup("GuiConsoleTextCtrl");		
   addField("expression",  TypeCaseString,  Offset(mConsoleExpression, GuiConsoleTextCtrl));
   endGroup("GuiConsoleTextCtrl");		
}

// -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- //

bool GuiConsoleTextCtrl::onWake()
{
   if (! Parent::onWake())
      return false;

   mFont = mProfile->mFont;
   return true;
}

void GuiConsoleTextCtrl::onSleep()
{
   Parent::onSleep();
   mFont = NULL;
}

// -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- //

void GuiConsoleTextCtrl::setText(const char *txt)
{
   //make sure we don't call this before onAdd();
   AssertFatal(mProfile, "Can't call setText() until setProfile() has been called.");

   if (txt)
      mConsoleExpression = StringTable->insert(txt);
   else
      mConsoleExpression = NULL;

   //Make sure we have a font
   mProfile->incRefCount();
   mFont = mProfile->mFont;

   setUpdate();

   //decrement the profile referrence
   mProfile->decRefCount();
}

void GuiConsoleTextCtrl::calcResize()
{
   if (!mResult)
      return;

   //resize
   if (mProfile->mAutoSizeWidth)
   {
      if (mProfile->mAutoSizeHeight)
         resize(mBounds.point, Point2I(mFont->getStrWidth((const UTF8 *)mResult) + 4, mFont->getHeight() + 4));
      else
         resize(mBounds.point, Point2I(mFont->getStrWidth((const UTF8 *)mResult) + 4, mBounds.extent.y));
   }
   else if (mProfile->mAutoSizeHeight)
   {
      resize(mBounds.point, Point2I(mBounds.extent.x, mFont->getHeight() + 4));
   }
}


void GuiConsoleTextCtrl::onPreRender()
{
   if (mConsoleExpression)
      mResult = Con::evaluatef("$temp = %s;", mConsoleExpression);
   calcResize();
}


// -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- //

void GuiConsoleTextCtrl::onRender(Point2I offset, const RectI &updateRect)
{
   // draw the background rectangle
   RectI r(offset, mBounds.extent);
   dglDrawRectFill(r, ColorI(255,255,255));

   // draw the border
   r.extent += r.point;
   glColor4ub(0, 0, 0, 0);

#ifdef TORQUE_OS_IOS
   Point2I topleft(r.point.x,  r.point.y);
   Point2I bottomRight(r.extent.x-1, r.extent.y-1);
	//this was the same drawing as dglDrawRect
	dglDrawRect( topleft, bottomRight, ColorI(0, 0, 0, 0) );// PUAP -Mat untested
#else
   glBegin(GL_LINE_LOOP);
   glVertex2i(r.point.x,  r.point.y);
   glVertex2i(r.extent.x-1, r.point.y);
   glVertex2i(r.extent.x-1, r.extent.y-1);
   glVertex2i(r.point.x,  r.extent.y-1);
   glEnd();
#endif

   if (mResult)
   {
      S32 txt_w = mFont->getStrWidth((const UTF8 *)mResult);
      Point2I localStart;
      switch (mProfile->mAlignment)
      {
         case GuiControlProfile::RightJustify:
            localStart.set(mBounds.extent.x - txt_w-2, 0);
            break;
         case GuiControlProfile::CenterJustify:
            localStart.set((mBounds.extent.x - txt_w) / 2, 0);
            break;
         default:
            // GuiControlProfile::LeftJustify
            localStart.set(2,0);
            break;
      }

      Point2I globalStart = localToGlobalCoord(localStart);

      //draw the text
      dglSetBitmapModulation(mProfile->mFontColor);
      dglDrawText(mFont, globalStart, mResult, mProfile->mFontColors);
   }

   //render the child controls
   renderChildControls(offset, updateRect);
}

// -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- //

const char *GuiConsoleTextCtrl::getScriptValue()
{
   return getText();
}

void GuiConsoleTextCtrl::setScriptValue(const char *val)
{
   setText(val);
}

// -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=- //
// EOF //
