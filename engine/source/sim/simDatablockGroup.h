//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _SIM_DATABLOCK_GROUP_H_
#define _SIM_DATABLOCK_GROUP_H_

#include "platform/platform.h"

#ifndef _SIMSET_H_
#include "sim/simSet.h"
#endif

//---------------------------------------------------------------------------

class SimDataBlockGroup : public SimGroup
{
  private:
   S32 mLastModifiedKey;

  public:
   static S32 QSORT_CALLBACK compareModifiedKey(const void* a,const void* b);
   void sort();
   SimDataBlockGroup();
};

#endif // _SIM_DATABLOCK_GROUP_H_