//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "platformWin32/platformWin32.h" 
#include "console/console.h"
#include "string/unicode.h"

#include "platform/nativeDialogs/msgBox.h"

struct _FlagMap
{
   S32 num;
   U32 flag;
};

static _FlagMap sgButtonMap[] =
{
   { MBOk,                 MB_OK },
   { MBOkCancel,           MB_OKCANCEL },
   { MBRetryCancel,        MB_RETRYCANCEL },
   { MBSaveDontSave,       MB_YESNO },
   { MBSaveDontSaveCancel, MB_YESNOCANCEL },
   { 0xffffffff,           0xffffffff }
};

static _FlagMap sgIconMap[] =
{
   { MIWarning,            MB_ICONWARNING },
   { MIInformation,        MB_ICONINFORMATION },
   { MIQuestion,           MB_ICONQUESTION },
   { MIStop,               MB_ICONSTOP },
   { 0xffffffff,           0xffffffff }
};

static _FlagMap sgMsgBoxRetMap[] =
{
   { IDCANCEL,             MRCancel },
   { IDNO,                 MRDontSave },
   { IDOK,                 MROk},
   { IDRETRY,              MRRetry },
   { IDYES,                MROk },
   { 0xffffffff,           0xffffffff }
};

//////////////////////////////////////////////////////////////////////////

static U32 getMaskFromID(_FlagMap *map, S32 id)
{
   for(S32 i = 0;map[i].num != 0xffffffff && map[i].flag != 0xffffffff;++i)
   {
      if(map[i].num == id)
         return map[i].flag;
   }

   return 0;
}

//////////////////////////////////////////////////////////////////////////

S32 Platform::messageBox(const UTF8 *title, const UTF8 *message, MBButtons buttons, MBIcons icon)
{

   // Get us rendering while we're blocking.
   winState.renderThreadBlocked = true;

#ifdef UNICODE
   const UTF16 *msg = convertUTF8toUTF16(message);
   const UTF16 *t = convertUTF8toUTF16(title);
#else
   const UTF8 *msg = message;
   const UTF8 *t = title;
#endif
   S32 ret = ::MessageBox(winState.appWindow, msg, t, getMaskFromID(sgButtonMap, buttons) | getMaskFromID(sgIconMap, icon));

#ifdef UNICODE
   delete [] msg;
   delete [] t;
#endif

   // Dialog is gone.
   winState.renderThreadBlocked = false;

   return getMaskFromID(sgMsgBoxRetMap, ret);
}
