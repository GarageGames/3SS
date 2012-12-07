//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _STRINGUNIT_H_
#define _STRINGUNIT_H_

#include "platform/types.h"

namespace StringUnit
{
   const char *getUnit(const char *string, U32 index, const char *set);
   const char *getUnits(const char *string, S32 startIndex, S32 endIndex, const char *set);
   U32 getUnitCount(const char *string, const char *set);
   const char* setUnit(const char *string, U32 index, const char *replace, const char *set);
   const char* removeUnit(const char *string, U32 index, const char *set);
};

#endif