//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _STRINGFUNCTIONS_H
#define _STRINGFUNCTIONS_H

#include <string>

using namespace std;

namespace Platform
{
    bool StringCompare(const string& left, const string& right);

    bool StringCompareNoCase(const string& left, const string& right);
    
    int Stricmp(const char *str1, const char *str2);
};

inline bool Platform::StringCompare(const string& left, const string& right)
{
    return (strcmp(left.c_str(), right.c_str()) == 0);
}

inline bool Platform::StringCompareNoCase(const string& left, const string& right)
{
    return (Platform::Stricmp(left.c_str(), right.c_str()) == 0);
}

#endif  // _STRINGFUNCTIONS_H
