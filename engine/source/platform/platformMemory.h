//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PLATFORM_MEMORY_H_
#define _PLATFORM_MEMORY_H_

#ifndef _TORQUE_TYPES_H_
#include "platform/types.h"
#endif

//------------------------------------------------------------------------------

#define placenew(x) new(x)
#define dMalloc(x) dMalloc_r(x, __FILE__, __LINE__)
#define dRealloc(x, y) dRealloc_r(x, y, __FILE__, __LINE__)

//------------------------------------------------------------------------------

namespace Memory
{
   void flagCurrentAllocs();
   void dumpUnflaggedAllocs(const char *file);
   S32 countUnflaggedAllocs(const char *file, S32 *outUnflaggedRealloc = NULL);
   dsize_t getMemoryUsed();
   dsize_t getMemoryAllocated();
   void validate();
}

//------------------------------------------------------------------------------

#if defined(TORQUE_OS_WIN32)
extern void* FN_CDECL operator new(dsize_t size, void* ptr);
#endif

//------------------------------------------------------------------------------

template <class T> inline T* constructInPlace(T* p)
{
   return new(p) T;
}

//------------------------------------------------------------------------------

template <class T> inline T* constructInPlace(T* p, const T* copy)
{
   return new(p) T(*copy);
}

//------------------------------------------------------------------------------

template <class T> inline void destructInPlace(T* p)
{
   p->~T();
}

//------------------------------------------------------------------------------

extern void  setBreakAlloc(dsize_t);
extern void  setMinimumAllocUnit(U32);
extern void* dMalloc_r(dsize_t in_size, const char*, const dsize_t);
extern void  dFree(void* in_pFree);
extern void* dRealloc_r(void* in_pResize, dsize_t in_size, const char*, const dsize_t);
extern void* dRealMalloc(dsize_t);
extern void  dRealFree(void*);

extern void* dMemcpy(void *dst, const void *src, dsize_t size);
extern void* dMemmove(void *dst, const void *src, dsize_t size);
extern void* dMemset(void *dst, int c, dsize_t size);
extern int   dMemcmp(const void *ptr1, const void *ptr2, dsize_t size);

#endif // _PLATFORM_MEMORY_H_
