//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXFRAMEWORKSBUILDPHASE_H
#define _PBXFRAMEWORKSBUILDPHASE_H

#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include <string>
#include <vector>

using namespace std;

class PBXBuildFile;

class PBXFrameworksBuildPhase : public PBXBaseBuildPhase
{
public:
    vector<PBXBuildFile*> m_Files;

public:
    PBXFrameworksBuildPhase();
    virtual ~PBXFrameworksBuildPhase();

    DEFINE_ISA(PBXFrameworksBuildPhase)
    DEFINE_GLOBAL_LIST(PBXFrameworksBuildPhase)
    DEFINE_WRITE_GLOBAL_LIST(PBXFrameworksBuildPhase)
};

#endif  // _PBXFRAMEWORKSBUILDPHASE_H
