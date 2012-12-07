//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _GUITEXTCTRL_H_
#define _GUITEXTCTRL_H_

#ifndef _GFONT_H_
#include "graphics/gFont.h"
#endif
#ifndef _GUITYPES_H_
#include "gui/guiTypes.h"
#endif
#ifndef _GUICONTROL_H_
#include "gui/guiControl.h"
#endif
#ifndef _STRINGBUFFER_H_
#include "string/stringBuffer.h"
#endif

class GuiTextCtrl : public GuiControl
{
private:
   typedef GuiControl Parent;

public:
   enum Constants { MAX_STRING_LENGTH = 1024 };


protected:
   StringTableEntry mInitialText;
   StringTableEntry mInitialTextID;
   UTF8 mText[MAX_STRING_LENGTH + 1];
   S32 mMaxStrLen;   // max string len, must be less then or equal to 255
   Resource<GFont> mFont;
   bool     mTruncateWhenUnfocused;

   S32 textBufferWidth(StringBuffer buffer);
   StringBuffer truncate(StringBuffer buffer, StringBuffer terminationString, S32 width);

public:

   //creation methods
   DECLARE_CONOBJECT(GuiTextCtrl);
   GuiTextCtrl();
   static void initPersistFields();

   //Parental methods
   bool onAdd();
   virtual bool onWake();
   virtual void onSleep();

   //text methods
   virtual void setText(const char *txt = NULL);
   virtual void setTextID(S32 id);
   virtual void setTextID(const char *id);
   const char *getText() { return (const char*)mText; }

   // Text Property Accessors
   static bool setText(void* obj, const char* data) { static_cast<GuiTextCtrl*>(obj)->setText(data); return true; }
   static const char* getTextProperty(void* obj, const char* data) { return static_cast<GuiTextCtrl*>(obj)->getText(); }


   void inspectPostApply();
   //rendering methods
   void onPreRender();
   void onRender(Point2I offset, const RectI &updateRect);
   void displayText( S32 xOffset, S32 yOffset );

   //Console methods
   const char *getScriptValue();
   void setScriptValue(const char *value);
};

#endif //_GUI_TEXT_CONTROL_H_
