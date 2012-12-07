//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _XCBUILDCONFIGURATION_H
#define _XCBUILDCONFIGURATION_H

#include "xcodeTemplate/PBXBase.h"
#include "tinyXML/tinyxml.h"
#include <string>
#include <vector>

using namespace std;

class PBXFileReference;

class XCBuildConfiguration : public PBXBase
{
protected:
    // The file reference path to search for
    string m_BaseConfigurationReferencePath;

public:
    enum BuildSettingType {
        BST_Single,
        BST_List,
    };

    vector<string> m_BuildSettingName;

    vector< vector<string> > m_BuildSettingValues;

    vector<S32> m_BuildSettingType;

    PBXFileReference* m_BaseConfigurationReference;

public:
    XCBuildConfiguration();
    virtual ~XCBuildConfiguration();

    void SetBaseConfigurationReferencePath(const string& path) { m_BaseConfigurationReferencePath = path; }

    void ResolveConfigFileReference();

    bool ProcessConfigXML(TiXmlElement& xml);

    DEFINE_ISA(XCBuildConfiguration)
    DEFINE_GLOBAL_LIST(XCBuildConfiguration)
    DEFINE_WRITE_GLOBAL_LIST(PBXTargetDependency)
};

#endif  // _XCBUILDCONFIGURATION_H
