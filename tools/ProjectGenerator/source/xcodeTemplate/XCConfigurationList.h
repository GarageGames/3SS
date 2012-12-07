//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _XCCONFIGURATIONLIST_H
#define _XCCONFIGURATIONLIST_H

#include "xcodeTemplate/PBXBase.h"
#include "tinyXML/tinyxml.h"
#include <string>
#include <vector>

using namespace std;

class XCBuildConfiguration;

class XCConfigurationList : public PBXBase
{
protected:
    PBXBase* m_Owner;

    // Holds a reference to a config file that will be resolved later
    // by the created XCBuildConfiguration
    string m_BaseConfigurationReferencePath;

public:
    vector<XCBuildConfiguration*> m_BuildConfigurations;

    string m_DefaultConfigurationIsVisible;

    string m_DefaultConfigurationName;

public:
    XCConfigurationList();
    virtual ~XCConfigurationList();

    PBXBase* GetOwner() { return m_Owner; }
    void SetOwner(PBXBase* owner) { m_Owner = owner; }

    void SetBaseConfigurationReferencePath(const string& path) { m_BaseConfigurationReferencePath = path; }

    bool ProcessBuildConfigXML(TiXmlElement& buildConfigs);

    DEFINE_ISA(XCConfigurationList)
    DEFINE_GLOBAL_LIST(XCConfigurationList)
    DEFINE_WRITE_GLOBAL_LIST(XCConfigurationList)
};

#endif  // _XCCONFIGURATIONLIST_H
