//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXGROUP_H
#define _PBXGROUP_H

#include "xcodeTemplate/PBXBase.h"
#include "tinyXML/tinyxml.h"
#include <string>
#include <vector>

using namespace std;

class PBXProject;

class PBXGroup : public PBXBase
{
protected:
    PBXProject* m_ParentProject;

public:
    vector<PBXBase*> m_ChildList;

    // Optionally points to the (relative) file path for the whole group.
    // If defined, may want to use the SOURCE_ROOT source tree to be relative
    // from the project.
    // Should not end with slash.
    string m_Path;

    PBXSourceTree m_SourceTree;

public:
    PBXGroup();
    virtual ~PBXGroup();

    PBXProject* GetParentProject() { return m_ParentProject; }
    void SetParentProject(PBXProject* project) { m_ParentProject = project; }

    // Process the Group tag
    bool ProcessGroupXML(PBXGroup* parent, TiXmlElement& xml);

    bool ProcessGroupElement(TiXmlElement& xml, bool topLevel);

    bool ProcessEngineSourceDirectory(TiXmlElement& xml, bool topLevel, const string& parentPath);

    bool ProcessEngineLibDirectory(TiXmlElement& xml, bool topLevel, const string& parentPath);

    bool ProcessNamedGroup(TiXmlElement& xml, bool topLevel);

    bool ProcessFrameworkDirectory(TiXmlElement& xml, bool topLevel);

    bool AddFilesToGroup(string& path, bool includeChildren);

    bool AddFrameworkToGroup(TiXmlElement& xml);

    bool AddFileToGroup(const string& name, const string& path, const string& source, const string& localSetting);

    bool AddFolderReferenceToGroup(const string& name, const string& path, const string& source, const string& buildPhase);

    DEFINE_ISA(PBXGroup)
    DEFINE_GLOBAL_LIST(PBXGroup)
    DEFINE_WRITE_GLOBAL_LIST(PBXGroup)
};

#endif  // _PBXGROUP_H
