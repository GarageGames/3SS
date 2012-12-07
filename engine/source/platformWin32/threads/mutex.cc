//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/threads/mutex.h"
#include "platformWin32/platformWin32.h"
#include "memory/safeDelete.h"

//////////////////////////////////////////////////////////////////////////
// Mutex Data
//////////////////////////////////////////////////////////////////////////

struct PlatformMutexData
{
   HANDLE mMutex;

   PlatformMutexData()
   {
      mMutex = NULL;
   }
};

//////////////////////////////////////////////////////////////////////////
// Constructor/Destructor
//////////////////////////////////////////////////////////////////////////

Mutex::Mutex()
{
   mData = new PlatformMutexData;

   mData->mMutex = CreateMutex(NULL, FALSE, NULL);
}

Mutex::~Mutex()
{
   if(mData && mData->mMutex)
      CloseHandle(mData->mMutex);
   
   SAFE_DELETE(mData);
}

//////////////////////////////////////////////////////////////////////////
// Public Methods
//////////////////////////////////////////////////////////////////////////

bool Mutex::lock(bool block /* = true */)
{
   if(mData == NULL || mData->mMutex == NULL)
      return false;

   return (bool)WaitForSingleObject(mData->mMutex, block ? INFINITE : 0) == WAIT_OBJECT_0;
}

void Mutex::unlock()
{
   if(mData == NULL || mData->mMutex == NULL)
      return;

   ReleaseMutex(mData->mMutex);
}

//void Mutex::set( void*data )
//{
//   if(mData && mData->mMutex)
//      CloseHandle(mData->mMutex);
//
//   if( mData == NULL )
//      mData = new PlatformMutexData;
//
//   mData->mMutex = (HANDLE)data;
//
//}