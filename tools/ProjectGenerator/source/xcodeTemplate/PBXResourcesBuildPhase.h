//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXRESOURCESBUILDPHASE_H
#define _PBXRESOURCESBUILDPHASE_H

#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include <string>
#include <vector>

using namespace std;

class PBXBuildFile;

class PBXResourcesBuildPhase : public PBXBaseBuildPhase
{
public:
    vector<PBXBuildFile*> m_Files;

public:
    PBXResourcesBuildPhase();
    virtual ~PBXResourcesBuildPhase();

    DEFINE_ISA(PBXResourcesBuildPhase)
    DEFINE_GLOBAL_LIST(PBXResourcesBuildPhase)
    DEFINE_WRITE_GLOBAL_LIST(PBXResourcesBuildPhase)
};

#endif  // _PBXRESOURCESBUILDPHASE_H
