//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _WINOGLVIDEO_H_
#define _WINOGLVIDEO_H_

#ifndef _PLATFORMVIDEO_H_
#include "platform/platformVideo.h"
#endif

#ifndef _PLATFORMGL_H_
#include "platformWin32/platformGL.h"
#endif

#ifndef _PLATFORMWIN32_H_
#include "platformWin32/platformWin32.h"
#endif

#ifndef _WINDDISPLAYVIDEO_H_
#include "platformWin32/winVideo.h"
#endif

class OpenGLDevice : public WinDisplayDevice
{
private:
   typedef WinDisplayDevice Parent;
public:
   OpenGLDevice();

   virtual void initDevice();
   bool activate( U32 width, U32 height, U32 bpp, bool fullScreen );
   void shutdown();
   void destroy();
   bool setScreenMode( U32 width, U32 height, U32 bpp, bool fullScreen, bool forceIt = false, bool repaint = true );
   void swapBuffers();
   const char* getDriverInfo();
   bool getGammaCorrection(F32 &g);
   bool setGammaCorrection(F32 g);
   bool setVerticalSync( bool on );

   static DisplayDevice* create();
};

#endif // _H_WINOGLVIDEO
