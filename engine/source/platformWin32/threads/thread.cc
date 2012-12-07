//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/threads/thread.h"
#include "platform/threads/semaphore.h"
#include "platformWin32/platformWin32.h"
#include "memory/safeDelete.h"

#include <process.h> // [tom, 4/20/2006] for _beginthread()

//////////////////////////////////////////////////////////////////////////
// Thread data
//////////////////////////////////////////////////////////////////////////

class PlatformThreadData
{
public:
   ThreadRunFunction       mRunFunc;
   void*                   mRunArg;
   Thread*                 mThread;
   HANDLE                  mThreadHnd;
   Semaphore               mGateway;
   U32                     mThreadID;

   PlatformThreadData()
   {
      mRunFunc    = NULL;
      mRunArg     = 0;
      mThread     = 0;
      mThreadHnd  = 0;
   };
};

//////////////////////////////////////////////////////////////////////////
// Static Functions/Methods
//////////////////////////////////////////////////////////////////////////


//-----------------------------------------------------------------------------
// Function:    ThreadRunHandler
// Summary:     Calls Thread::run() with the thread's specified run argument.
//               Neccesary because Thread::run() is provided as a non-threaded
//               way to execute the thread's run function. So we have to keep
//               track of the thread's lock here.
static void ThreadRunHandler(void * arg)
{
   PlatformThreadData* mData = reinterpret_cast<PlatformThreadData*>(arg);
   mData->mThreadID = ThreadManager::getCurrentThreadId();
   
   ThreadManager::addThread(mData->mThread);
   mData->mThread->run(mData->mRunArg);
   ThreadManager::removeThread(mData->mThread);

   // we could delete the Thread here, if it wants to be auto-deleted...
   mData->mGateway.release();
   // the end of this function is where the created win32 thread will die.

   mData->mThreadHnd = 0;
}

//////////////////////////////////////////////////////////////////////////
// Constructor/Destructor
//////////////////////////////////////////////////////////////////////////

Thread::Thread(ThreadRunFunction func /* = 0 */, void *arg /* = 0 */, bool start_thread /* = true */, bool autodelete /*= false*/)
{
   mData = new PlatformThreadData;
   mData->mRunFunc = func;
   mData->mRunArg = arg;
   mData->mThread = this;

   if(start_thread)
      start();
}

Thread::~Thread()
{
   stop();
   join();

   SAFE_DELETE(mData);
}

//////////////////////////////////////////////////////////////////////////
// Public Methods
//////////////////////////////////////////////////////////////////////////

void Thread::start()
{
   if(isAlive())
      return;

   // cause start to block out other pthreads from using this Thread, 
   // at least until ThreadRunHandler exits.
   mData->mGateway.acquire();
   
   // reset the shouldStop flag, so we'll know when someone asks us to stop.
   shouldStop = false;

   mData->mThreadHnd = (HANDLE)_beginthread(ThreadRunHandler, 0, mData);
}

bool Thread::join()
{
   if(!isAlive())
      return true;

   return WaitForSingleObject(mData->mThreadHnd, INFINITE) != WAIT_FAILED;
}

void Thread::run(void *arg /* = 0 */)
{
   if(mData->mRunFunc)
      mData->mRunFunc(arg);
}

bool Thread::isAlive()
{
   if(mData->mThreadHnd == 0)
      return false;

   DWORD res = WaitForSingleObject(mData->mThreadHnd, 0);
   return res != WAIT_OBJECT_0 && res != WAIT_FAILED;
}

U32 Thread::getId()
{
   return mData->mThreadID;
}

U32 ThreadManager::getCurrentThreadId()
{
   return GetCurrentThreadId();
}

bool ThreadManager::compare(U32 threadId_1, U32 threadId_2)
{
   return (threadId_1 == threadId_2);
}
