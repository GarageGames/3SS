//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXVARIANTGROUP_H
#define _PBXVARIANTGROUP_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class PBXVariantGroup : public PBXBase
{
public:
    vector<PBXBase*> m_ChildList;

    PBXSourceTree m_SourceTree;

public:
    PBXVariantGroup();
    virtual ~PBXVariantGroup();

    DEFINE_ISA(PBXVariantGroup)
    DEFINE_GLOBAL_LIST(PBXVariantGroup)
    DEFINE_WRITE_GLOBAL_LIST(PBXVariantGroup)
};

#endif  // _PBXVARIANTGROUP_H
