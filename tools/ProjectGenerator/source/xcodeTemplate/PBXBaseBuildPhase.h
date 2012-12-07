//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXBASEBUILDPHASE_H
#define _PBXBASEBUILDPHASE_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class PBXNativeTarget;

class PBXBaseBuildPhase : public PBXBase
{
public:
    string m_BuildActionMask;

    string m_RunOnlyForDeploymentPostprocessing;

    PBXNativeTarget* m_ParentTarget;

public:
    PBXBaseBuildPhase();
    virtual ~PBXBaseBuildPhase();
};

#endif  // _PBXBASEBUILDPHASE_H
