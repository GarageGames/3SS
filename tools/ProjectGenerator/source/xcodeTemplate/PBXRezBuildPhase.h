//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXREZBUILDPHASE_H
#define _PBXREZBUILDPHASE_H

#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include <string>
#include <vector>

using namespace std;

class PBXBuildFile;

class PBXRezBuildPhase : public PBXBaseBuildPhase
{
public:
    vector<PBXBuildFile*> m_Files;

public:
    PBXRezBuildPhase();
    virtual ~PBXRezBuildPhase();

    DEFINE_ISA(PBXRezBuildPhase)
    DEFINE_GLOBAL_LIST(PBXRezBuildPhase)
    DEFINE_WRITE_GLOBAL_LIST(PBXRezBuildPhase)
};

#endif  // _PBXREZBUILDPHASE_H
