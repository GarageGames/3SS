//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXPROJECT_H
#define _PBXPROJECT_H

#include "xcodeTemplate/PBXBase.h"
#include "tinyXML/tinyxml.h"
#include <string>
#include <vector>
#include <fstream>

using namespace std;

class PBXGroup;
class XCConfigurationList;
class PBXNativeTarget;
class PBXBuildFile;

class PBXProject : public PBXBase
{
protected:
    // Stores the path to engine source.  Becomes $TSS_SOURCE_PATH
    string m_EngineSourcePath;

    // Stores the path to the engine libraries.  Becomes $TSS_LIB_PATH
    string m_EngineLibPath;

public:
    XCConfigurationList* m_BuildConfigurationList;

    string m_CompatibilityVersion;

    string m_DevelopmentRegion;

    S32 m_HasScannedForEncodings;

    vector<string> m_KnownRegions;

    PBXGroup* m_MainGroup;

    PBXGroup* m_ProductRefGroup;

    string m_ProjectDirPath;

    vector<PBXBase*> m_ProjectReferences; // This is the wrong type, but maybe should be calculated

    string m_ProjectRoot;

    vector<string> m_AttributeName;
    vector<string> m_AttributeValue;

    // Native or Aggregate targets
    vector<PBXBase*> m_Targets;

public:
    PBXProject();
    virtual ~PBXProject();

    const string& GetEngineSourcePath() { return m_EngineSourcePath; }
    const string& GetEngineLibPath() { return m_EngineLibPath; }

    // Process the Project tag of a multi-project XML file
    bool ProcessProjectXML(TiXmlElement& xml);

    bool DoesBuildPhaseExist(const string& buildPhase);

    void AddBuildFileToTarget(PBXBuildFile* buildFile);

    DEFINE_ISA(PBXProject)
    DEFINE_GLOBAL_LIST(PBXProject)
    DEFINE_WRITE_GLOBAL_LIST(PBXProject)
};

#endif  // _PBXPROJECT_H
