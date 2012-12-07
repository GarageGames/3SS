//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include <pthread.h>
#include "platform/threads/thread.h"
#include "platform/platformSemaphore.h"
#include "platform/threads/mutex.h"
#include "platform/platformTLS.h"
#include "memory/safeDelete.h"
#include "platformMacCarb/cocoaUtils.h"
#include <stdlib.h>

class PlatformThreadData
{
public:
   ThreadRunFunction       mRunFunc;
   void*                   mRunArg;
   Thread*                 mThread;
   Semaphore               mGateway; // default count is 1
   U32                     mThreadID;
};

//-----------------------------------------------------------------------------
// Function:    ThreadRunHandler
// Summary:     Calls Thread::run() with the thread's specified run argument.
//               Neccesary because Thread::run() is provided as a non-threaded
//               way to execute the thread's run function. So we have to keep
//               track of the thread's lock here.
static void *ThreadRunHandler(void * arg)
{
   PlatformThreadData *mData = reinterpret_cast<PlatformThreadData*>(arg);
   Thread *thread = mData->mThread;
   NSAutoReleasePoolPtr pool = PlatStateMac::createAutoReleasePool();
   mData->mThreadID = ThreadManager::getCurrentThreadId();
   ThreadManager::addThread(thread);
   thread->run(mData->mRunArg);
   PlatStateMac::releaseAutoReleasePool(pool);
   mData->mGateway.release();
   // we could delete the Thread here, if it wants to be auto-deleted...
   if(thread->autoDelete)
   {
      ThreadManager::removeThread(thread);
      delete thread;
   }
   // return value for pthread lib's benefit
   return NULL;
   // the end of this function is where the created pthread will die.
}
   
//-----------------------------------------------------------------------------
Thread::Thread(ThreadRunFunction func, void* arg, bool start_thread, bool autodelete)
{
   mData = new PlatformThreadData;
   mData->mRunFunc = func;
   mData->mRunArg = arg;
   mData->mThread = this;
   mData->mThreadID = 0;
   autoDelete = autodelete;
   
   if(start_thread)
      start();
}

Thread::~Thread()
{
   stop();
   join();

   SAFE_DELETE(mData);
}

void Thread::start()
{
   if(isAlive())
      return;

   // cause start to block out other pthreads from using this Thread, 
   // at least until ThreadRunHandler exits.
   mData->mGateway.acquire();

   // reset the shouldStop flag, so we'll know when someone asks us to stop.
   shouldStop = false;

   pthread_create((pthread_t*)(&mData->mThreadID), NULL, ThreadRunHandler, mData);
}

bool Thread::join()
{
   if(!isAlive())
      return true;

   // not using pthread_join here because pthread_join cannot deal
   // with multiple simultaneous calls.
   mData->mGateway.acquire();
   mData->mGateway.release();
   return true;
}

void Thread::run(void* arg)
{
   if(mData->mRunFunc)
      mData->mRunFunc(arg);
}

bool Thread::isAlive()
{
   if(mData->mThreadID == 0)
      return false;

   if( mData->mGateway.acquire(false) ) 
   {
     mData->mGateway.release();
     return false; // we got the lock, it aint alive.
   }
   else
     return true; // we could not get the lock, it must be alive.
}

U32 Thread::getId()
{
   return mData->mThreadID;
}

U32 ThreadManager::getCurrentThreadId()
{
   return (U32)pthread_self();
}

bool ThreadManager::compare(U32 threadId_1, U32 threadId_2)
{
   return (bool)pthread_equal((pthread_t)threadId_1, (pthread_t)threadId_2);
}

#if defined(TORQUE_MAC_THREAD_TESTS) && 0
static void *gMut1 = Mutex::createMutex();
// -- the following is a set of tests to check thread sync facilities.
//  the 2 different ways of starting threads, by subclassing Thread,
//  and by using a (ThreadRunFunc*)(), are both demonstrated here.
#include "gui/core/guiControl.h"

class TestThread : public Thread
{
public:
   volatile bool dienow;
   TestThread(ThreadRunFunction func, void* arg, bool start_thread) 
     : Thread(func,arg,start_thread)
   {
     dienow = false;
   }
   virtual void run(void* arg)
   {
     int r;
     U32 time = Platform::getRealMilliseconds();
     Con::printf("thread %i starting",arg);
     while(!this->dienow) // will be set true by mothership thread
     {
       Mutex::lockMutex(gMut1);
       GuiControl *ctrl = new GuiControl();
       r = ((float)rand() / RAND_MAX ) * 10;
       Platform::sleep(r);
       delete ctrl;
       Mutex::unlockMutex(gMut1);
     }
     time = Platform::getRealMilliseconds() - time;
     Con::printf("thread %i exec time: %i",arg,time);
   }
};



void mothership(S32 arg)
{
   Con::printf("mothership started with arg %i",arg);
   int r;
   U32 time = Platform::getRealMilliseconds();
   TestThread* thread[arg];
   // create some threads, randomly sleep or delete one.
   Mutex::lockMutex(gMut1);
   for(int i=0; i < arg; i++)
   {
   
     Con::printf("starting thread %i",i+1);
     thread[i] = new TestThread((ThreadRunFunction)NULL, i+1, true);
     r = ((float)rand() / RAND_MAX ) * 10;
     Platform::sleep(r);

   }
   Mutex::unlockMutex(gMut1);
     
   for(int i=0; i < arg; i++)
   {
     r = ((float)rand() / RAND_MAX ) * 10;
     Platform::sleep(r);
     thread[i]->dienow=true;
     delete thread[i];
   }
   time = Platform::getRealMilliseconds() - time;
   Con::printf("mothership exec time: %i",time);
}


ConsoleFunction(TestThreads,void,1,3,
     "TestThreads([number of motherships], [number of threads per mothership]);"
     " Launches threads, all competing for the same mutex.")
{
   int nThreads = 1;
   int nMotherships = 1;
   if(argc>=2) {
     nThreads = dAtoi(argv[1]);
   }

   if(argc>=3) {
      nMotherships = dAtoi(argv[2]);
   }

   bool semStateTest1, semStateTest2, semIndepTest;

   // check whether we can acquire a newly made semaphore
   semStateTest1 = true;
   void* sem = Semaphore::createSemaphore();
   void* sem2 = Semaphore::createSemaphore();
   if(Semaphore::acquireSemaphore(sem,false)) {
      semStateTest1 = true;
   } else 
      semStateTest1 = false;

   if(Semaphore::acquireSemaphore(sem2,false)) {
      semStateTest2 = true;
   } else 
      semStateTest2 = false;
   
   // if we failed to acquire new semaphores, 
   // test whether semaphores are independant.
   semIndepTest = true; 
   if(!semStateTest1 && !semStateTest2) 
   {
      // release one
      Semaphore::releaseSemaphore(sem);
      // try to acquire the other ( that we know we can't acquire yet )
      if(Semaphore::acquireSemaphore(sem2,false)) 
         // we really should not be able to get this semaphore
         semIndepTest = false;
      else 
         semIndepTest = true;
   }

   Con::errorf("-------------- Semaphore test Results ------------");
   if(!semStateTest1)
      Con::errorf("New Semaphore Aqcuire test 1 failed.");
   else
      Con::printf("New Semaphore Aqcuire test 1 passed.");

   if(!semStateTest2)
      Con::errorf("New Semaphore Aqcuire test 2 failed.");
   else
      Con::printf("New Semaphore Aqcuire test 2 passed.");

   if(!semIndepTest)
      Con::errorf("Semaphores are NOT INDEPENDANT!!! - This is bad.");
   else
      Con::errorf("Semaphore Independance test passed.");

   
   Con::printf("starting concurrent threads...");
   Mutex::lockMutex(gMut1);
   for(int i=0; i < nMotherships; i++) {
     // memory leak here: because we dont keeep refs to the mothership Threads,
     // we cannot delete them.
     Con::printf("starting a mothership");
     Thread *t = new Thread((ThreadRunFunction)mothership,nThreads,true);
   }
   Mutex::unlockMutex(gMut1);
}

#endif // TORQUE_MAC_THREAD_TESTS


class PlatformThreadStorage
{
public:
   pthread_key_t mThreadKey;
};

ThreadStorage::ThreadStorage()
{
   mThreadStorage = (PlatformThreadStorage *) mStorage;
   constructInPlace(mThreadStorage);

   pthread_key_create(&mThreadStorage->mThreadKey, NULL);
}

ThreadStorage::~ThreadStorage()
{
   pthread_key_delete(mThreadStorage->mThreadKey);
}

void *ThreadStorage::get()
{
   return pthread_getspecific(mThreadStorage->mThreadKey);
}

void ThreadStorage::set(void *value)
{
   pthread_setspecific(mThreadStorage->mThreadKey, value);
}

