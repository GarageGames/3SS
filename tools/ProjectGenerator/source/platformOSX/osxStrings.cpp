//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "platform/stringFunctions.h"
#include <strings.h>

int Platform::Stricmp(const char *str1, const char *str2)
{
    char c1, c2;
    while (1)
    {
        c1 = tolower(*str1++);
        c2 = tolower(*str2++);
        if (c1 < c2) return -1;
        if (c1 > c2) return 1;
        if (c1 == 0) return 0;
    }
    
}