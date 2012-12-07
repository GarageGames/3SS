//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "console/console.h"

#include "platform/nativeDialogs/msgBox.h"

// these are the return values for message box dialog buttons
void initMessageBoxVars()
{
   Con::setIntVariable("$MROk",        MROk);
   Con::setIntVariable("$MRCancel",    MRCancel);
   Con::setIntVariable("$MRRetry",     MRRetry);
   Con::setIntVariable("$MRDontSave",  MRDontSave);
}

//////////////////////////////////////////////////////////////////////////

static EnumTable::Enums sgButtonEnums[] =
{
   { MBOk,                 "Ok" },
   { MBOkCancel,           "OkCancel" },
   { MBRetryCancel,        "RetryCancel" },
   { MBSaveDontSave,       "SaveDontSave" }, // maps to yes/no on win, to save/discard on mac.
   { MBSaveDontSaveCancel, "SaveDontSaveCancel" }, // maps to yes/no/cancel on win, to save/cancel/don'tsave on mac.
   { 0, NULL }
};

static EnumTable::Enums sgIconEnums[] =
{
   { MIInformation,        "Information" },// win: blue i, mac: app icon or talking head
   { MIWarning,            "Warning" },    // win & mac: yellow triangle with exclamation pt
   { MIStop,               "Stop" },       // win: red x, mac: app icon or stop icon, depending on version
   { MIQuestion,           "Question" },   // win: blue ?, mac: app icon
   { 0, NULL }
};

//////////////////////////////////////////////////////////////////////////

static S32 getIDFromName(EnumTable::Enums *table, const char *name, S32 def = -1)
{
   for(S32 i = 0;table[i].label != NULL;++i)
   {
      if(dStricmp(table[i].label, name) == 0)
         return table[i].index;
   }
   AssertWarn(false,"getIDFromName(): didn't find that name" );
   return def;
}

//////////////////////////////////////////////////////////////////////////

ConsoleFunction(messageBox, S32, 3, 5, "(title, message[, buttons[, icon]]) Pops up a message box\n"
				"@param title The message box's title to display\n"
				"@param message The message to display in the box\n"
				"@param buttons The buttons to include on box (default MBOkCancel)\n"
				"@param icon The displayed icon (default MIInformation)\n"
				"@return Returns the ID of the box")
{
   S32 btns = MBOkCancel;
   S32 icns = MIInformation;
   
   if(argc > 3)
      btns = getIDFromName(sgButtonEnums, argv[3], btns);
   if(argc > 4)
      icns = getIDFromName(sgIconEnums, argv[4], icns);

   return Platform::messageBox(argv[1], argv[2], (MBButtons)btns, (MBIcons)icns);
}
