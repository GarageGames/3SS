//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _TORQUEGAME_H_
#define _TORQUEGAME_H_

#ifndef _GAMEINTERFACE_H_
#include "game/gameInterface.h"
#endif

//-----------------------------------------------------------------------------

class DefaultGame : public GameInterface
{
public:
   void textureKill();
   void textureResurrect();
   void refreshWindow();

   //virtual int  main(int argc, const char **argv);
   virtual bool mainInit(int argc, const char **argv);
   virtual void mainLoop();
   virtual void mainShutdown();
      
   void processPacketReceiveEvent(PacketReceiveEvent *event);
   void processMouseMoveEvent(MouseMoveEvent *event);
   void processInputEvent(InputEvent *event);
   void processScreenTouchEvent(ScreenTouchEvent *event);
   void processQuitEvent();
   void processTimeEvent(TimeEvent *event);
   void processConsoleEvent(ConsoleEvent *event);
   void processConnectedAcceptEvent(ConnectedAcceptEvent *event);
   void processConnectedReceiveEvent(ConnectedReceiveEvent *event);
   void processConnectedNotifyEvent(ConnectedNotifyEvent *event);
};

bool clientProcess(U32 timeDelta);


//Added by Matthew Langley Start
bool serverProcess(U32 timeDelta);

#define TickShift   5
#define TickMs      (1 << TickShift)
#define TickSec     (F32(TickMs) / 1000.0f)
#define TickMask    (TickMs - 1)


class ProcessList
{
  // GameBase head;
   U32 mCurrentTag;
   SimTime mLastTick;
   SimTime mLastTime;
   SimTime mLastDelta;
   bool mIsServer;
   bool mDirty;
   static bool mDebugControlSync;

public:
   SimTime getLastTime() { return mLastTime; }
   ProcessList(bool isServer);
   void markDirty()  { mDirty = true; }
   bool isDirty()  { return mDirty; }
   F32 getLastInterpDelta() { return mLastDelta / F32(TickMs); }

   /// @name Advancing Time
   /// The advance time functions return true if a tick was processed.
   ///
   /// These functions go through either gServerProcessList or gClientProcessList and
   /// call each GameBase's processTick().
   /// @{

   bool advanceServerTime(SimTime timeDelta);

   /// @}
};

extern ProcessList gServerProcessList;
//Added by Matthew Langley End


#endif
