//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXHEADERSBUILDPHASE_H
#define _PBXHEADERSBUILDPHASE_H

#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include <string>
#include <vector>

using namespace std;

class PBXBuildFile;

class PBXHeadersBuildPhase : public PBXBaseBuildPhase
{
public:
    vector<PBXBuildFile*> m_Files;

public:
    PBXHeadersBuildPhase();
    virtual ~PBXHeadersBuildPhase();

    DEFINE_ISA(PBXHeadersBuildPhase)
    DEFINE_GLOBAL_LIST(PBXHeadersBuildPhase)
    DEFINE_WRITE_GLOBAL_LIST(PBXHeadersBuildPhase)
};

#endif  // _PBXHEADERSBUILDPHASE_H
