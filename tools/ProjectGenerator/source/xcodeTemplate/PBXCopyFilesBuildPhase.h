//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXCOPYFILESBUILDPHASE_H
#define _PBXCOPYFILESBUILDPHASE_H

#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include <string>
#include <vector>

using namespace std;

class PBXBuildFile;

class PBXCopyFilesBuildPhase : public PBXBaseBuildPhase
{
public:
    vector<PBXBuildFile*> m_Files;

    string m_DstPath;

    string m_DstSubfolderSpec;

public:
    PBXCopyFilesBuildPhase();
    virtual ~PBXCopyFilesBuildPhase();

    DEFINE_ISA(PBXCopyFilesBuildPhase)
    DEFINE_GLOBAL_LIST(PBXCopyFilesBuildPhase)
    DEFINE_WRITE_GLOBAL_LIST(PBXCopyFilesBuildPhase)
};

#endif  // _PBXCOPYFILESBUILDPHASE_H
