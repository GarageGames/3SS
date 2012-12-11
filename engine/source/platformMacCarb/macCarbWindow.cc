//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include <unistd.h>

#include "platformMacCarb/platformMacCarb.h"
#include "platform/platformVideo.h"
#include "platformMacCarb/macCarbOGLVideo.h"
#include "platformMacCarb/macCarbConsole.h"
#include "platform/platformInput.h"
#include "game/gameInterface.h"
#include "console/consoleTypes.h"
#include "console/console.h"
#include "platformMacCarb/macCarbEvents.h"
#include "platform/threads/thread.h"
#include "platformMacCarb/cocoaUtils.h"


//------------------------------------------------------------------------------
#pragma mark ---- PlatState ----
MacCarbPlatState platState;

MacCarbPlatState::MacCarbPlatState()
{
   appWindow      = NULL;
   
   captureDisplay = true;
   fadeWindows    = true;
   backgrounded   = false;
   minimized      = false;
   
   quit           = false;
   
   ctx            = NULL;
   
   // start with something reasonable.
   desktopBitsPixel  = 32;
   desktopWidth      = 1024;
   desktopHeight     = 768;
   
   osVersion = 0;
   
   dStrcpy(appWindowTitle, "Mac Torque Game Engine");
   
   // Semaphore for alerts. We put the app in a modal state by blocking the main 
   // Torque thread until the RAEL thread  allows it to continue. 
   alertSemaphore = Semaphore::createSemaphore(0);
   alertDlg = NULL;
   
   // directory that contains main.cs . This will help us detect whether we are 
   // running with the scripts in the bundle or not.
   mainDotCsDir = NULL;
   
   mainLoopTimer = NULL;
}

#pragma mark ---- Window stuff ----
const U32 kTFullscreenWindowAttrs =  kWindowNoShadowAttribute | kWindowStandardHandlerAttribute;
const U32 kTDefaultWindowAttrs = kWindowStandardDocumentAttributes | kWindowStandardHandlerAttribute;
//------------------------------------------------------------------------------
WindowPtr MacCarbCreateOpenGLWindow( CGDirectDisplayID hDevice, U32 width, U32 height, bool fullScreen )
{
   WindowPtr w = NULL;
   
   Rect   rect;
   CGRect dRect;
   
   // device bounds, eg: [top,left , bottom,right] = [0,0 , 768,1024]
   
   dRect = CGDisplayBounds(hDevice);
   // center the window's rect in the display's rect.
   rect.top = 50; //dRect.origin.y + (dRect.size.height - height) / 2;
   rect.left = 50;//dRect.origin.x + (dRect.size.width - width) / 2;
   rect.right = rect.left + width;
   rect.bottom = rect.top + height;
   
   OSStatus err;
   WindowAttributes windAttr = 0L;
   WindowClass windClass = kDocumentWindowClass;
   
   if (fullScreen)
   {
      windAttr = kTFullscreenWindowAttrs;
      windClass = kAltPlainWindowClass;
      //    Overlay windows can be used to make transperant windows,
      //    which are good for performing all kinds of cool tricks.
      //      windClass = kOverlayWindowClass;
      //      windAttr |= kWindowOpaqueForEventsAttribute;
   }
   else
   {
      windAttr = kTDefaultWindowAttrs;
   }
   
   err = CreateNewWindow(windClass, windAttr, &rect, &w);
   HISize maxSize;
   maxSize.width = 1024;
   maxSize.height = 768;
   
   SetWindowResizeLimits(w, &maxSize, &maxSize);
   
   PlatStateMac::get().windowRef2NSWindow(w);
   
   AssertISV( err == noErr && w != NULL, "Failed to create a new window.");
   
   // in windowed-fullscreen mode, we set the window's level to be
   // in front of the blanking window
   if (fullScreen)
   { 
      // create a new group if ours doesn't already exist
      if (platState.torqueWindowGroup==NULL)
         CreateWindowGroup(NULL, &platState.torqueWindowGroup);
      
      // place window in group
      SetWindowGroup(w, platState.torqueWindowGroup);
      
      // set window group level to one higher than blanking window.
      SetWindowGroupLevel(platState.torqueWindowGroup, kTFullscreenWindowLevel);
   }
   
   platState.minimized = false;
   
   RGBColor black;
   dMemset(&black, 0, sizeof(RGBColor));
   SetWindowContentColor( w, &black);
   
   return(w);
}

//------------------------------------------------------------------------------
// Fade a window in, asynchronously.
void MacCarbFadeInWindow( WindowPtr window )
{
   if(!IsValidWindowPtr(window))
      return;
   
   // bump this to the main thread if we're not on the main thread.
   if(! ThreadManager::isCurrentThread(platState.firstThreadId) && !platState.quit)
   {
      MacCarbSendTorqueEventToMain( kEventTorqueFadeInWindow, window );
      return;
   }
   
   // set state on menubar & mouse cursor. 
   if(Video::isFullScreen())
   {
      HideMenuBar();
      PlatStateMac::get().setHideCursor(true);
   }
   else
   {
      ShowMenuBar();
   }
   
   SelectWindow(window);
   
   if(platState.fadeWindows)
   {
      TransitionWindowOptions t;
      dMemset(&t, 0, sizeof(t));
      TransitionWindowWithOptions( window, kWindowFadeTransitionEffect, 
                                  kWindowShowTransitionAction, NULL, true, &t);
   }
   else
   {
      ShowWindow(window);
   }
}

//------------------------------------------------------------------------------
// Fade a window out, asynchronously. It will be released when the transition finishes.
void MacCarbFadeAndReleaseWindow( WindowPtr window )
{
   if(!IsValidWindowPtr(window))
      return;
   
   if(! ThreadManager::isCurrentThread(platState.firstThreadId) && !platState.quit)
   {
      MacCarbSendTorqueEventToMain( kEventTorqueFadeOutWindow, window );
      return;
   }
   
   if(platState.fadeWindows)
   {
      TransitionWindowOptions t;
      dMemset(&t, 0, sizeof(t));
      TransitionWindowWithOptions( window, kWindowFadeTransitionEffect, 
                                  kWindowHideTransitionAction, NULL, false, &t);
   }
   else
   {
      MacCarbSendTorqueEventToMain(kEventTorqueReleaseWindow, window);
   }
}

//------------------------------------------------------------------------------
// Hide or show the menu bar.
void MacCarbShowMenuBar(bool show)
{
   if(! ThreadManager::isCurrentThread(platState.firstThreadId) && !platState.quit)
   {
      MacCarbSendTorqueEventToMain( kEventTorqueShowMenuBar, (void*)show );
      return;
   }
   
   if(show)
      ShowMenuBar();
   else
      HideMenuBar();
}

//------------------------------------------------------------------------------
// DGL, the Gui, and TS use this for various purposes.
const Point2I &Platform::getWindowSize()
{
   return platState.windowSize;
}


//------------------------------------------------------------------------------
// save the window size, for DGL's use
void Platform::setWindowSize( U32 newWidth, U32 newHeight )
{
   platState.windowSize.set( newWidth, newHeight );
   char tempBuf[32];
   dSprintf(tempBuf, sizeof(char) * 32, "%d %d %d", newWidth, newHeight, 16);
   Con::setVariable("$pref::Video::windowedRes", tempBuf);
}


//------------------------------------------------------------------------------
// Issue a minimize event. The standard handler will handle it.
void Platform::minimizeWindow()
{
   CollapseWindow(platState.appWindow, true);
}

void Platform::restoreWindow()
{
   CollapseWindow(platState.appWindow, false);
}

//------------------------------------------------------------------------------
void Platform::setWindowTitle(const char* title )
{
   if(!platState.appWindow)
      return;
   
   // set app window's title
   CFStringRef cfsTitle = CFStringCreateWithCString(NULL, title, kCFStringEncodingUTF8);
   SetWindowTitleWithCFString(platState.appWindow, cfsTitle);
   CFRelease(cfsTitle);
   
   // save title in platstate
   dStrncpy(platState.appWindowTitle, title, getMin((U32)dStrlen(title), sizeof(platState.appWindowTitle)));
}


#pragma mark ---- Init funcs  ----
//------------------------------------------------------------------------------
void Platform::init()
{
   // Set the platform variable for the scripts
   Con::setVariable( "$platform", "macos" );
   
   MacConsole::create();
   //if ( !MacConsole::isEnabled() )
   Input::init();
   
   // allow users to specify whether to capture the display or not when going fullscreen
   Con::addVariable("pref::mac::captureDisplay", TypeBool, &platState.captureDisplay);
   Con::addVariable("pref::mac::fadeWindows", TypeBool, &platState.fadeWindows);
   
   // create the opengl display device
   DisplayDevice *dev = NULL;
   Con::printSeparator();
   Con::printf( "Video Initialization:" );
   Video::init();
   dev = OpenGLDevice::create();
   if(dev)
      Con::printf( "   Accelerated OpenGL display device detected." );
   else
      Con::printf( "   Accelerated OpenGL display device not detected." );
   
   // and now we can install the device.
   Video::installDevice(dev);
   Con::printf( "" );
}

//------------------------------------------------------------------------------
void Platform::shutdown()
{
   setWindowLocked( false );
   Video::destroy();
   Input::destroy();
   MacConsole::destroy();
}

//------------------------------------------------------------------------------
// Get the video settings from the prefs.
static void _MacCarbGetInitialRes(U32 &width, U32 &height, U32 &bpp, bool &fullScreen)
{
   const char* resString;
   char *tempBuf;
   
   // cache the desktop size of the selected screen in platState
   Video::getDesktopResolution();
   
   // load pref variables, properly choose windowed / fullscreen
#ifndef TORQUE_TOOLS
   fullScreen = Con::getBoolVariable( "$pref::Video::fullScreen" );
#else
   fullScreen = Con::getBoolVariable( "$pref::T2D::fullScreen" );
#endif
   
   if (fullScreen)
      resString = Con::getVariable( "$pref::Video::resolution" );
   else
      resString = Con::getVariable( "$pref::Video::windowedRes" );
   
   // dStrtok is destructive, work on a copy...
   tempBuf = new char[dStrlen( resString ) + 1];
   dStrcpy( tempBuf, resString );
   
   // set window size
   //DAW: Added min size checks for windowSize
   width = dAtoi( dStrtok( tempBuf, " x\0" ) );
   if( ! width > 0 ) width = platState.windowSize.x;
   
   height = dAtoi( dStrtok( NULL, " x\0") );
   if( ! height > 0 ) height = platState.windowSize.y;
   
   // bit depth
   if (fullScreen)
   {
      dAtoi( dStrtok( NULL, "\0" ) );
      if( ! bpp > 0 ) bpp = 16;
   }
   else
      bpp = platState.desktopBitsPixel;
   
   delete [] tempBuf;
}

//------------------------------------------------------------------------------
void Platform::initWindow(const Point2I &initialSize, const char *name)
{
   dSprintf(platState.appWindowTitle, sizeof(platState.appWindowTitle), name);
   
   // init the default window size
   platState.windowSize = initialSize;
   if( ! platState.windowSize.x > 0 ) platState.windowSize.x = 640;
   if( ! platState.windowSize.y > 0 ) platState.windowSize.y = 480;
   
   DisplayDevice::init();
   
   bool fullScreen;
   U32 width, height, bpp;
   _MacCarbGetInitialRes(width, height, bpp, fullScreen);
   
   // this will create a rendering context & window
   bool ok = Video::setDevice( "OpenGL", width, height, bpp, fullScreen );
   if ( ! ok )
   {
      AssertFatal( false, "Could not find a compatible display device!" );
   }
   
   if (platState.appWindow)
   {
      // install handlers to the given window.
      EventTargetRef winTarg = GetWindowEventTarget(platState.appWindow);
   }
   MacCarbInstallCarbonEventHandlers();
}

#pragma mark ---- Platform utility funcs ----

char * getFullPath( const char * name )
{
   static char pathstr[1024];
   static char rtnval[1024];
   
   char * p;
   char * path;
   size_t ln, lp;
   
	/* "" is not a valid filename; check this before traversing PATH. */
	if ((name == NULL) || (name[0] == '\0')) {
		errno = ENOENT;
		return NULL;
	}
	/* If it's an absolute or relative path name, it's easy. */
	if (strchr(name, '/')) {
		strcpy( rtnval, name );
		return rtnval;
	}
   
   // look in current working directory first
   if ((path = getenv("PWD")))
      strcpy( rtnval, path );
   else
      rtnval[0] = '\0';
   strcat( rtnval, "/" );
   strcat( rtnval, name );
   if (access( rtnval, X_OK )==0) {
      return rtnval;
   }
   
	/* Get the path we're searching. */
   if (!(path = getenv("PATH"))) {
      strcpy( pathstr, "." );
   } else {
      strcpy( pathstr, path );
   }
   path = pathstr;
   
	ln = strlen(name);
	do {
		/* Find the end of this path element. */
		for (p = path; *path != 0 && *path != ':'; path++)
			continue;
		/*
		 * It's a SHELL path -- double, leading and trailing colons
		 * mean the current directory.
		 */
		if (p == path) {
			p = (char*)".";
			lp = 1;
		} else
			lp = path - p;
      
      path++;
      
		/*
		 * If the path is too long complain.  This is a possible
		 * security issue; given a way to make the path too long
		 * the user may execute the wrong program.
		 */
		if (lp + ln + 2 > strlen(rtnval)) {
         //			(void)write(STDERR_FILENO, "execvp: ", 8);
         //			(void)write(STDERR_FILENO, p, lp);
         //			(void)write(STDERR_FILENO, ": path too long\n", 16);
			return NULL;
		}
		memcpy(rtnval, p, lp);
		rtnval[lp] = '/';
		memcpy(rtnval + lp + 1, name, ln);
		rtnval[lp + ln + 1] = '\0';
      
      if (access( rtnval, X_OK )==0) {
         return rtnval;
      }
      
   } while (*path != 0);
   
   return NULL;
}

extern char **environ;
// run other apps, return 0 if failed, otherwise return the UNIX process ID
// if the executable is 'sh', then launch the script directly and let the system locate the interpreter
// output of the script goes to the debugging console output, unless the script redirects it somewhere else
S32 Platform::runBatchFile( const char* appPathName, const char* cmdLine, bool blocking )
{
   static char appname[1024];
   static char argsline[1024];
   char * app;
   bool isSH = false;
   if ( strcmp( appPathName, "sh" ) == 0 ) {
      isSH = true;
   } else if (strrchr( appPathName, '/' ) != NULL) {
      if ( strcmp( strrchr( appPathName, '/' )+1, "sh" ) == 0 ) {
         isSH = true;
      }
   }
   char * pptr = argsline;
   char * arg;
   int argc = 0;
   char * argv[16];
   strcpy( argsline, cmdLine );
   
   if (isSH) 
   {
      strcpy( appname, "/bin/sh" ); // call the sh interp directly...this allows the scripts to be common text files, without executable mode
   } 
   else 
   {
      strcpy( appname, getFullPath( appPathName ));
      
      if(!appname)
         return 0;
   }
   argc = 1;
   argv[0]  = appname;
    
   for ( ; argc<16; argc++) {
      argv[argc] = strsep( &pptr, ";" );
      if (argv[argc] == NULL) {
         break;
      }
   }
   if (argc >= 16) argv[10] = NULL; // gurantee a null terminated array
   
   // now fork to the app
   pid_t pid = vfork();
   if (pid == 0) {
      // we are now in the child process, so exec the application
      (void) execve(argv[0], argv, environ);
      _exit(EXIT_FAILURE);   
   }
   // if got here, we are still in the parent process, so just return the pid
   if (pid < 0) {
      // was error, so return 0
      return 0;
   }
   
   if (blocking) {
      pid_t waitResult;
      int status;
      do {
         waitResult = waitpid(pid, &status, 0);
      } while ( (waitResult == -1) && (errno == EINTR) );      
      return 0;
   }
   
   return pid;
}

// if can convert the Unix process ID back into a process serial number, is still running
bool Platform::isBatchFileDone( int appid )
{
   ProcessSerialNumber PSN = {0,0};
   OSStatus err = GetProcessForPID( appid, &PSN);
   return ((err < 0) || ((PSN.highLongOfPSN==0) && (PSN.lowLongOfPSN==0)));
}

//--------------------------------------
// Web browser function:
//--------------------------------------
bool Platform::openWebBrowser( const char* webAddress )
{
   OSStatus err;
   CFURLRef url = CFURLCreateWithBytes(NULL,(UInt8*)webAddress,dStrlen(webAddress),kCFStringEncodingASCII,NULL);
   err = LSOpenCFURLRef(url,NULL);
   CFRelease(url);
   
   // kick out of fullscreen mode, so we can *see* the webpage!
   if(Video::isFullScreen())
      Video::toggleFullScreen();
   
   return(err==noErr);
}

#pragma mark -
#pragma mark ---- Tests ----

ConsoleFunction(testWindowLevels,void,1,2,"testWindowLevels([lev to set]);")
{
   SInt32 lev;
   Con::printf(" Sheilding window level is %x",CGShieldingWindowLevel());   
   GetWindowGroupLevel(GetWindowGroupOfClass(kUtilityWindowClass),&lev);
   Con::printf("   Utility window level is %x", lev);
   GetWindowGroupLevel(GetWindowGroupOfClass(kUtilityWindowClass),&lev);
   Con::printf("  Floating window level is %x", lev);
   GetWindowGroupLevel(GetWindowGroupOfClass(kAlertWindowClass),&lev);
   Con::printf("     Alert window level is %x", lev);
   
   lev=1;
   if(argc==2)
      lev=dAtoi(argv[1]);
   SetWindowGroupLevel( GetWindowGroupOfClass(kUtilityWindowClass), lev);
   SetWindowGroupLevel( GetWindowGroupOfClass(kFloatingWindowClass), lev);   
   SetWindowGroupLevel( GetWindowGroupOfClass(kAlertWindowClass), lev);
}


ConsoleFunction( testSetWindowTitle, void, 2,4, "")
{
   Platform::setWindowTitle(argv[1]);
}

ConsoleFunction( invertScreenColor, void, 1,1, "")
{
   static bool inverted = false;
   
   CGGammaValue reds[1024];
   CGGammaValue greens[1024];
   CGGammaValue blues[1024];
   U32 numTableEntries;
   
   CGGetDisplayTransferByTable( CGMainDisplayID(), 1024, reds, greens, blues, &numTableEntries);
   
   CGGammaValue newReds[numTableEntries];
   CGGammaValue newGreens[numTableEntries];
   CGGammaValue newBlues[numTableEntries];
   
   for(int i=0; i< numTableEntries; i++)
   {
      newReds[i] = reds[numTableEntries-1-i];
      newGreens[i] = greens[numTableEntries-1-i];
      newBlues[i] = blues[numTableEntries-1-i];      
   }
   
   CGSetDisplayTransferByTable(CGMainDisplayID(), numTableEntries, newReds, newGreens, newBlues);
   
}

ConsoleFunction(testAsserts, void, 1,1,"")
{
   AssertFatal(false,"Monsters in my OATMEAL.");
   AssertWarn(false,"Oh sweet mercy, the PAIN... THE PAIN!");
   AssertISV(false,"AAaaah! *GARGLE* *SPUTTER* *WET THUD*");
}



