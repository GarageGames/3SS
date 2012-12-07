//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "graphics/gBitmap.h"
#include "platform/types.h"
#include "platform/event.h"
#include "gui/guiCanvas.h"
#include "console/console.h"
#include "game/defaultGame.h"
#include "platform/Tickable.h"

ProcessList gServerProcessList(true);

const U32 AudioUpdatePeriod = 125;  ///< milliseconds between audio updates.

//--------------------------------------------------------------------------
ConsoleFunctionGroupBegin( GameFunctions, "General game functionality.");

ConsoleFunction(screenShot, void, 3, 3, "(string file, string format)"
                "Take a screenshot.\n\n"
                "@param format One of JPEG or PNG.")
{
#ifndef TORQUE_OS_IOS
// PUAP -Mat no screenshots on iPhone can do it from Xcode
    FileStream fStream;
   if(!fStream.open(argv[1], FileStream::Write))
   {   
      Con::printf("Failed to open file '%s'.", argv[1]);
      return;
   }
    
   glReadBuffer(GL_FRONT);
   
   Point2I extent = Canvas->getExtent();
   U8 * pixels = new U8[extent.x * extent.y * 4];
   glReadPixels(0, 0, extent.x, extent.y, GL_RGB, GL_UNSIGNED_BYTE, pixels);
   
   GBitmap * bitmap = new GBitmap;
   bitmap->allocateBitmap(U32(extent.x), U32(extent.y));
   
   // flip the rows
   for(U32 y = 0; y < (U32)extent.y; y++)
      dMemcpy(bitmap->getAddress(0, extent.y - y - 1), pixels + y * extent.x * 3, U32(extent.x * 3));

   if ( dStrcmp( argv[2], "JPEG" ) == 0 )
      bitmap->writeJPEG(fStream);
   else if( dStrcmp( argv[2], "PNG" ) == 0)
      bitmap->writePNG(fStream);
   else
      bitmap->writePNG(fStream);

   fStream.close();
   delete [] pixels;
   delete bitmap;
#endif
}

ConsoleFunctionGroupEnd( GameFunctions );

bool clientProcess(U32 timeDelta)
{
   Tickable::advanceTime(timeDelta);	

    // This is based on PW's stuff
#ifndef NO_AUDIO_SUPPORT
   // alxUpdate is somewhat expensive and does not need to be updated constantly,
   // though it does need to be updated in real time
   static U32 lastAudioUpdate = 0;
   U32 realTime = Platform::getRealMilliseconds();
   if((realTime - lastAudioUpdate) >= AudioUpdatePeriod)
   {
      alxUpdate();
      lastAudioUpdate = realTime;
   }
#endif

   return true;
}

//Added by Matthew Langley Start

ProcessList::ProcessList(bool isServer)
{
   mDirty = false;
   mCurrentTag = 0;
   mLastTick = 0;
   mLastTime = 0;
   mLastDelta = 0;
   mIsServer = isServer;
//   Con::addVariable("debugControlSync",TypeBool, &mDebugControlSync);
}


bool ProcessList::advanceServerTime(SimTime timeDelta)
{
   //PROFILE_START(AdvanceServerTime);

//   if (mDirty) orderList();

   SimTime targetTime = mLastTime + timeDelta;
   SimTime targetTick = targetTime & ~TickMask;
   // UNUSED: JOSEPH THOMAS -> SimTime tickCount = (targetTick - mLastTick) >> TickShift;

   bool ret = mLastTick != targetTick;

   mLastTime = targetTime;
  // PROFILE_END();
   return ret;
}

bool serverProcess(U32 timeDelta)
{
   return gServerProcessList.advanceServerTime(timeDelta);
}
//Added by Matthew Langley End