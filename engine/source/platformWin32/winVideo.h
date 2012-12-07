//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WINDDISPLAYVIDEO_H_
#define _WINDDISPLAYVIDEO_H_

#ifndef _PLATFORMVIDEO_H_
#include "platform/platformVideo.h"
#endif

#ifndef _PLATFORMGL_H_
#include "platformWin32/platformGL.h"
#endif

#ifndef _PLATFORMWIN32_H_
#include "platformWin32/platformWin32.h"
#endif


class WinDisplayDevice : public DisplayDevice
{
protected:
   bool mRestoreGamma;
   U16  mOriginalRamp[256*3];
   static bool smCanSwitchBitDepth;
   static bool smCanDo16Bit;
   static bool smCanDo32Bit;
public:
   WinDisplayDevice();

   virtual void initDevice();

   static void enumerateBitDepths();

   enum OSVer
   {
      OS_NT = 0,
      OS_9X,
      OS_UNKNOWN,
      OS_ERROR
   };
   static U32 getOSVersion();
};

#endif // _H_WIND3DVIDEO


