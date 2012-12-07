//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXNATIVETARGET_H
#define _PBXNATIVETARGET_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class XCConfigurationList;
class PBXFileReference;
class PBXProject;
class PBXBaseBuildPhase;
class PBXBuildFile;

class PBXNativeTarget : public PBXBase
{
protected:
    PBXProject* m_ParentProject;

public:
    XCConfigurationList* m_BuildConfigurationList;

    vector<PBXBaseBuildPhase*> m_BuildPhases;

    string m_ProductName;

    PBXFileReference* m_ProductReference;

    string m_ProductType;

public:
    PBXNativeTarget();
    virtual ~PBXNativeTarget();

    bool ProcessXMLFile(const string& filePath, PBXProject* parent);

    bool DoesBuildPhaseExist(const string& buildPhase);

    void AddBuildFileToBuildPhase(PBXBuildFile* buildFile);

    DEFINE_ISA(PBXNativeTarget)
    DEFINE_GLOBAL_LIST(PBXNativeTarget)
    DEFINE_WRITE_GLOBAL_LIST(PBXNativeTarget)
};

#endif  // _PBXNATIVETARGET_H
