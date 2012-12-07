//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// The intent of this object is that it will allow cocoa features to be used
// by various parts of the torque mac platform, without having to include
// the <Cocoa/Cocoa.h> header(s), or any extra Torque headers.

typedef void* NSAutoReleasePoolPtr;
typedef void* NSWindowPtr;
typedef void* NSMutableDictionaryPtr;
#ifndef WindowRef
typedef void* MacWindowRef;
#endif
 
class PlatStateMac
{
   NSAutoReleasePoolPtr globalPool;
   bool torqueCursorHidden;
   bool platformCursorHidden;
   NSMutableDictionaryPtr nsWindows;

   PlatStateMac();
   ~PlatStateMac() { /*releaseAutoReleasePool(globalPool);*/ }
public:
   static PlatStateMac& get(void)
   {
      static PlatStateMac singleton;
      return singleton;
   }
   
   /// class methods that manage things we don't store on a PlatStateMac object.
   static NSAutoReleasePoolPtr createAutoReleasePool();
   static void releaseAutoReleasePool(NSAutoReleasePoolPtr);

   void ensureMultithreaded();
  
   /// Sets the native cursor hidden state.
   /// @param hide Whether to hide or show the native cursor
   /// @param track If true, will save the value of @p hide, for use by @c restoreHideCursor() .
   /// @see restoreHideCursor()
   void setHideCursor(bool hide, bool track = true);
   /// Restores the cursor state to the last state tracked by @c setHideCursor()
   void restoreHideCursor();
   
   NSWindowPtr windowRef2NSWindow(MacWindowRef w);
   
};

// todo: paxorr: threaded shellExecute
//    move ExecuteCleanupEvent to common platform
//    consolefunction( shellexecute ) { executeThread::shellexecute()/*blocks*/; sendCleanupEvent(); }
// todo: paxorr: game binary
