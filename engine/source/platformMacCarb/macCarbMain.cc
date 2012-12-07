//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#include "platformMacCarb/platformMacCarb.h"
#include "platformMacCarb/cocoaUtils.h"
#include "platformMacCarb/macCarbEvents.h"
#include "platform/threads/thread.h"
#include "game/gameInterface.h"
#include "io/fileio.h"

//-----------------------------------------------------------------------------
// _MacCarbRunTorqueMain() starts the Game.main() loop
//-----------------------------------------------------------------------------
static void _MacCarbRunTorqueMain()
{
   NSAutoReleasePoolPtr pool = PlatStateMac::createAutoReleasePool();
   platState.torqueThreadId = ThreadManager::getCurrentThreadId();
   platState.windowSize.set(0,0);
   platState.lastTimeTick = Platform::getRealMilliseconds();
   platState.appReturn = Game->main(platState.argc, platState.argv);
   PlatStateMac::releaseAutoReleasePool(pool);
   QuitApplicationEventLoop();
}

static void _MacCarbGameInnerLoop(EventLoopTimerRef theTimer, void *userData)
{
   if(Game->isRunning())
      Game->mainLoop();
   else
   {
      Game->mainShutdown();
      RemoveEventLoopTimer(platState.mainLoopTimer);
      QuitApplicationEventLoop();
   }
}

void MacCarbInstallEventLoopTimer(U32 intervalMs)
{
   if(platState.mainLoopTimer)
   {
      printf("removing old main loop timer\n");
      RemoveEventLoopTimer(platState.mainLoopTimer);
   }
   
   printf("installing main loop timer = %i\n", intervalMs);
   EventTimerInterval delay = 1.0 / 1000 / 1; // 1 nanosec initial delay
   EventTimerInterval interval = intervalMs / 1000.0; // EventTimerInterval is a double.
   if(interval == 0.0)
      interval = 1.0 / 1000 / 1000; // 1 nanosec minimum delay. 0 delay gives a 1-shot timer.

   InstallEventLoopTimer(GetCurrentEventLoop(), delay, interval,
      NewEventLoopTimerUPP(_MacCarbGameInnerLoop), NULL, &platState.mainLoopTimer);
   platState.sleepTicks = intervalMs;
}

//-----------------------------------------------------------------------------
// Handler stub for bsd signals.
//-----------------------------------------------------------------------------
static void _MacCarbSignalHandler(int )
{
   // we send the torque thread a SIGALRM to wake it up from usleep()
   // when transitioning from background - forground.
}

//-----------------------------------------------------------------------------
//  Thread subclass, for running Torque in multithreaded mode
//-----------------------------------------------------------------------------
class TorqueMainThread : public Thread
{
public:
   TorqueMainThread() : Thread(NULL,0,false) { }

   virtual void run(void* arg)
   {
      signal(SIGALRM, _MacCarbSignalHandler);
      _MacCarbRunTorqueMain();
   }
};

//-----------------------------------------------------------------------------
//  RunAppEventLoop Callback, for running Torque in single threaded mode
//-----------------------------------------------------------------------------
void _MacCarbRAELCallback(EventLoopTimerRef theTimer, void *userData)
{
   _MacCarbRunTorqueMain();
}

#pragma mark -

//-----------------------------------------------------------------------------
// command line arg processing
//-----------------------------------------------------------------------------
#define KEYISDOWN(key) ((((unsigned char *)currKeyState)[key>>3] >> (key & 7)) & 1)
static bool _MacCarbCheckProcessTxtFileArgs()
{
   // this is yucky, but the easiest way to ignore the cmd line args:
   KeyMap currKeyState;
   GetKeys(currKeyState);
   if (KEYISDOWN(0x38)) // check shift key -- actually LShift.
      return false;
   else
      return true;
}

//-----------------------------------------------------------------------------
static void _MacCarbGetTxtFileArgs(int &argc, char** argv, int maxargc)
{
   argc = 0;
   
   const U32 kMaxTextLen = 2048; // arbitrary
   U32 textLen;
   char* text = new char[kMaxTextLen];   

   // open the file, kick out if we can't
   File cmdfile;
   File::Status err = cmdfile.open("maccmdline.txt", cmdfile.Read);
   if(err != File::Ok)
      return;
   
   // read in the first kMaxTextLen bytes, kick out if we get errors or no data
   err = cmdfile.read(kMaxTextLen-1, text, &textLen);
   if(!((err == File::Ok || err == File::EOS) || textLen > 0))
   {
      cmdfile.close();
      return;
   }
   
   // null terminate
   text[textLen++] = '\0';
   // truncate to the 1st line of the file
   for(int i = 0; i < textLen; i++)
   {
      if( text[i] == '\n' || text[i] == '\r' )
      {
         text[i] = '\0';
         textLen = i+1;
         break;
      }
   }

   // tokenize the args with nulls, save them in argv, count them in argc
   char* tok;
   for(tok = dStrtok(text, " "); tok && argc < maxargc; tok = dStrtok(NULL, " "))
   {
      argv[argc++] = tok;
   }
}

//-----------------------------------------------------------------------------
static void _MacCarbFilterCmdLineArgs( int &argc, const char** argv)
{
   // MacOSX gui apps get at least 2 args: the full path to their binary, 
   // and a process serial number, formed something like: "-psn_0_123456".
   // Torque doesnt want to see the psn arg, so we strip it out.
   int j = 0;
   for(int i = 0; i < argc; i++)
   {
      if(dStrncmp(argv[i], "-psn", 4) != 0)
         argv[j++] = argv[i];
   }
   argc = j;
}

#pragma mark -

//-----------------------------------------------------------------------------
// main() - the real one - this is the actual program entry point.
//-----------------------------------------------------------------------------
S32 main(S32 argc, const char **argv)
{
   
   const int kMaxCmdlineArgs = 32; // arbitrary

   FlushEvents( everyEvent, 0 );
   SetEventMask(everyEvent);

   // push us to the front, just to be sure
   ProcessSerialNumber psn = { 0, kCurrentProcess };
   SetFrontProcess(&psn);

   // save away OS version info into platState.
   Gestalt(gestaltSystemVersion, (SInt32 *) &(platState.osVersion));

   // Find Main.cs .
   const char* cwd = Platform::getMainDotCsDir();
   // Change to the directory that contains main.cs
   Platform::setCurrentDirectory(cwd);
   
   // get the actual command line args
   S32   newArgc = argc;
   const char* newArgv[kMaxCmdlineArgs];
   for(int i=0; i < argc && i < kMaxCmdlineArgs; i++)
      newArgv[i] = argv[i];

   if( _MacCarbCheckProcessTxtFileArgs() )
   {
      // get the text file args
      S32 textArgc;
      char* textArgv[kMaxCmdlineArgs];
      _MacCarbGetTxtFileArgs(textArgc, textArgv, kMaxCmdlineArgs);
      
      // merge them
      int i=0;
      while(i < textArgc && newArgc < kMaxCmdlineArgs)
         newArgv[newArgc++] = textArgv[i++];
   }
   
   // filter them
   _MacCarbFilterCmdLineArgs( newArgc, newArgv);
   
   // store them in platState
   platState.argc = newArgc;
   platState.argv = newArgv;
    
   // now, we prepare to hand off execution to torque & macosx.
   platState.appReturn = 0;
   platState.firstThreadId = ThreadManager::getCurrentThreadId();

#if !defined(TORQUE_MULTITHREAD)
   // Install a one-shot timer to run the game, then call RAEL to install
   // the default application handler (which can't be called directly).
   EventLoopTimerRef timer;
   InstallEventLoopTimer(GetCurrentEventLoop(), 0, 0, 
                     NewEventLoopTimerUPP(_MacCarbRAELCallback), NULL, &timer);
   PlatStateMac &cocoaPlatState = PlatStateMac::get();
   cocoaPlatState.ensureMultithreaded();
   RunApplicationEventLoop();
#elif defined(OLD_TORQUE_MULTITHREAD)
   // Put the Torque application loop in one thread, and the event listener loop
   // in the other thread. The event loop must use the process's initial thread.

   // We need to cache a ref to the main event queue because GetMainEventQueue
   // is not thread safe pre 10.4 . 
   platState.mainEventQueue = GetMainEventQueue();

   // We need to install event handlers for interthread communication.
   // Events and some system calls must happen in the process's initial thread.
   MacCarbInstallTorqueCarbonEventHandlers();
   
   TorqueMainThread mainLoop;
   mainLoop.start();
   
   PlatStateMac &cocoaPlatState = PlatStateMac::get();
   cocoaPlatState.ensureMultithreaded();
   printf("starting RAEL\n");
   RunApplicationEventLoop();
   printf("trying to join main loop...\n");
   mainLoop.join();
   printf("main loop joined.\n");
#else
   signal(SIGALRM, _MacCarbSignalHandler);

   platState.mainEventQueue = GetMainEventQueue();
   MacCarbInstallTorqueCarbonEventHandlers();

   PlatStateMac &cocoaPlatState = PlatStateMac::get();
   cocoaPlatState.ensureMultithreaded();

   printf("performing mainInit()\n");
   platState.torqueThreadId = ThreadManager::getCurrentThreadId();
   platState.windowSize.set(0,0);
   platState.lastTimeTick = Platform::getRealMilliseconds();
   Game->mainInit(platState.argc, platState.argv);

   // start with foreground time.
   printf("installing main loop timer\n");
   MacCarbInstallEventLoopTimer(sgTimeManagerProcessInterval);

   printf("starting RAEL with timer\n");
   RunApplicationEventLoop();
   printf("main loop over\n");
#endif   
   
   printf("exiting...\n");   
   return(platState.appReturn);
   
}
