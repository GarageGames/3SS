//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#import <Cocoa/Cocoa.h>
#import "cocoaUtils.h"
#include "platform/platformInput.h"

PlatStateMac::PlatStateMac()
{
   globalPool = createAutoReleasePool(); 
   torqueCursorHidden = false;
   platformCursorHidden = false;
   NSApplicationLoad();
   [NSBundle loadNibNamed:@"mainMenu" owner:NSApp];
   nsWindows = [NSMutableDictionary dictionaryWithCapacity:10];
   // and finally create a bogus nswindow, just to make sure cocoa is awake & paying attention.
   NSWindow* tmp = [[NSWindow alloc] init];
   [tmp release];
}

NSAutoReleasePoolPtr PlatStateMac::createAutoReleasePool()
{
   return [[NSAutoreleasePool alloc] init];
}

void PlatStateMac::releaseAutoReleasePool(NSAutoReleasePoolPtr pool)
{
   [(NSAutoreleasePool*)pool release];
}

// dummy class for ensureMultithreaded()
   @interface DummyThreadSelector : NSObject { }
      + (void)ThreadRoutine:(id)param;
   @end
   
   @implementation DummyThreadSelector
   + (void)ThreadRoutine:(id)param
   {
      NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
      [pool release];
   } 
   @end

// spawn off 1 NSThread.
// this will make sure cocoa goes multithreaded, so that we can use cocoa from 
// any thread.
void PlatStateMac::ensureMultithreaded()
{
   [NSThread detachNewThreadSelector:@selector(ThreadRoutine:) toTarget:[DummyThreadSelector class] withObject:nil];
}

// Sets the native cursor hidden state.
void PlatStateMac::setHideCursor(bool hide, bool track)
{
   if(hide != platformCursorHidden)
   {
      if(hide)
         [NSCursor hide];
      else
         [NSCursor unhide];
      platformCursorHidden = hide;
   }
   
   if(track)
      torqueCursorHidden = hide;
}

// Restores the cursor state to the last state tracked by @c setHideCursor()
void PlatStateMac::restoreHideCursor()
{
   setHideCursor(torqueCursorHidden, false);
}

NSWindowPtr PlatStateMac::windowRef2NSWindow(MacWindowRef w)
{
   NSMutableDictionary *dict = (NSMutableDictionary*)nsWindows;
   NSString *key = [NSString stringWithFormat:@"%i", w];
   NSWindow *win = (NSWindow*)[dict objectForKey:key];
   if(win == nil)
   {
      win = [[NSWindow alloc] initWithWindowRef:w];
      [dict setObject:win forKey:key];
   }
   [key release];
   return win;
}
