//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PLATFORM_THREAD_SEMAPHORE_H_
#define _PLATFORM_THREAD_SEMAPHORE_H_

#ifndef _TORQUE_TYPES_H_
#include "platform/types.h"
#endif

// Forward ref used by platform code
class PlatformSemaphore;

class Semaphore
{
protected:
   PlatformSemaphore *mData;
public:
   /// Create a semaphore. initialCount defaults to 1.
   Semaphore(S32 initialCount = 1);
   /// Delete a semaphore, ignoring it's count.
   ~Semaphore();

   /// Acquire the semaphore, decrementing its count.
   /// if the initial count is less than 1, block until it goes above 1, then acquire.
   /// Returns true if the semaphore was acquired, false if the semaphore could
   /// not be acquired and block was false.
   bool acquire(bool block = true, S32 timeoutMS = -1);
   
   /// Release the semaphore, incrementing its count.
   /// Never blocks.
   void release();
   
   // Old API so that we don't have to change a load of code
   static void* createSemaphore(U32 initialCount = 1)
   {
      return new Semaphore(initialCount);
   }

   static void destroySemaphore(void * semaphore)
   {
      Semaphore* realSem = reinterpret_cast<Semaphore*>(semaphore);
      delete realSem;
   }

   static bool acquireSemaphore(void * semaphore, bool block = true)
   {
      Semaphore* realSem = reinterpret_cast<Semaphore*>(semaphore);
      return realSem->acquire(block);
   }

   static void releaseSemaphore(void * semaphore)
   {
      Semaphore* realSem = reinterpret_cast<Semaphore*>(semaphore);
      realSem->release();
   }

   //inline static bool P(void * semaphore, bool block = true) {return(acquireSemaphore(semaphore, block));}
   //inline static void V(void * semaphore) {releaseSemaphore(semaphore);}
};

#endif
