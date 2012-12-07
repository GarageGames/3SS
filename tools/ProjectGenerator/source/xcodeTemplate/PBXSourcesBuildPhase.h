//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXSOURCESBUILDPHASE_H
#define _PBXSOURCESBUILDPHASE_H

#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include <string>
#include <vector>

using namespace std;

class PBXBuildFile;

class PBXSourcesBuildPhase : public PBXBaseBuildPhase
{
public:
    vector<PBXBuildFile*> m_Files;

public:
    PBXSourcesBuildPhase();
    virtual ~PBXSourcesBuildPhase();

    DEFINE_ISA(PBXSourcesBuildPhase)
    DEFINE_GLOBAL_LIST(PBXSourcesBuildPhase)
    DEFINE_WRITE_GLOBAL_LIST(PBXSourcesBuildPhase)
};

#endif  // _PBXSOURCESBUILDPHASE_H
