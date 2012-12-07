//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXProject.h"
#include "xcodeTemplate/XCConfigurationList.h"
#include "xcodeTemplate/PBXTargetDependency.h"
#include "xcodeTemplate/PBXGroup.h"
#include "xcodeTemplate/PBXNativeTarget.h"
#include "xcodeTemplate/PBXBuildFile.h"
#include "xcodeTemplate/PBXAggregateTarget.h"
#include "xcodeTemplate/XCBuildConfiguration.h"
#include "platform/fileIO.h"
#include "tinyXML/tinyxml.h"

IMPLEMENT_GLOBAL_LIST(PBXProject)

PBXProject::PBXProject()
{
    m_BuildConfigurationList = NULL;
    m_MainGroup = NULL;
    m_ProductRefGroup = NULL;

    m_CompatibilityVersion = "Xcode 3.2";
    m_DevelopmentRegion = "English";

    m_ProjectDirPath = "";
    m_ProjectRoot = "";

    m_HasScannedForEncodings = 1;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXProject::~PBXProject()
{
}

bool PBXProject::ProcessProjectXML(TiXmlElement& xml)
{
    m_Name = xml.Attribute("name");
    if(m_Name.empty())
    {
        printf("PBXProject::ProcessProjectXML(): Missing project name in XML file.\n");
        return false;
    }

    m_EngineSourcePath = xml.Attribute("engineSourceDirectory");
    m_EngineLibPath = xml.Attribute("engineLibDirectory");
    if(m_EngineSourcePath.empty() || m_EngineLibPath.empty())
    {
        printf("PBXProject::ProcessProjectXML(): Cannot find engine source or library path.\n");
        return false;
    }

    // Process the excluded directories for the project
    Platform::CleanExcludedDirectories();
    TiXmlElement* excludedDirectories = xml.FirstChildElement("ExcludeDirectories");
    if(excludedDirectories)
    {
        TiXmlElement* dir = excludedDirectories->FirstChildElement("Directory");
        while(dir)
        {
            string dirName = dir->Attribute("name");
            Platform::AddExcludedDirectory(dirName);

            dir = dir->NextSiblingElement("Directory");
        }
    }

    // Process the regions
    TiXmlElement* regions = xml.FirstChildElement("Regions");
    if(!regions)
    {
        printf("PBXProject::ProcessProjectXML(): Cannot find any regions in the project XML file.\n");
        return false;
    }
    m_DevelopmentRegion = regions->Attribute("developmentRegion");
    TiXmlElement* region = regions->FirstChildElement("Region");
    while(region)
    {
        string text = region->Attribute("name");
        m_KnownRegions.push_back(text);

        region = region->NextSiblingElement("Region");
    }

    // Create the main group for the project
    m_MainGroup = new PBXGroup();
    m_MainGroup->SetParentProject(this);
    m_MainGroup->m_SourceTree = PBXBase::ST_RelativeToGroup;

    // Create the Product group
    m_ProductRefGroup = new PBXGroup();
    m_ProductRefGroup->SetParentProject(this);
    m_ProductRefGroup->SetName("Products");
    m_ProductRefGroup->m_SourceTree = PBXBase::ST_RelativeToGroup;
    m_MainGroup->m_ChildList.push_back(m_ProductRefGroup);

    // Create the native target for the project
    TiXmlElement* targets = xml.FirstChildElement("Targets");
    if(!targets)
    {
        printf("PBXProject::ProcessProjectXML(): Cannot find any targets in the project XML file.\n");
        return false;
    }
    TiXmlElement* target = targets->FirstChildElement("Target");
    while(target)
    {
        string targetFile = target->Attribute("file");
        if(!targetFile.empty())
        {
            PBXNativeTarget* nativeTarget = new PBXNativeTarget();
            bool result = nativeTarget->ProcessXMLFile(targetFile, this);
            if(result)
            {
                m_Targets.push_back(nativeTarget);
            }
            else
            {
                delete nativeTarget;
                printf("PBXProject::ProcessProjectXML(): Problem creating target from file '%s'.\n", targetFile.c_str());
                return false;
            }
        }

        target = target->NextSiblingElement("Target");
    }

    // Now the All target
    TiXmlElement* targetAll = targets->FirstChildElement("TargetAll");
    if(targetAll)
    {
        string targetFile = targetAll->Attribute("file");
        if(!targetFile.empty())
        {
            PBXAggregateTarget* at = new PBXAggregateTarget();
            bool result = at->ProcessXMLFile(targetFile, this);
            if(!result)
            {
                delete at;
                printf("PBXProject::ProcessProjectXML(): Problem creating All target from file '%s'.\n", targetFile.c_str());
                return false;
            }
        }
    }

    // Process all groups
    TiXmlElement* groups = xml.FirstChildElement("Groups");
    if(groups)
    {
        TiXmlElement* group = groups->FirstChildElement("Group");
        while(group)
        {
            PBXGroup* g = new PBXGroup();
            g->SetParentProject(this);
            bool result = g->ProcessGroupXML(m_MainGroup, *group);
            if(!result)
            {
                delete g;
            }
            else
            {
                m_MainGroup->m_ChildList.push_back(g);
            }

            group = group->NextSiblingElement("Group");
        }
    }

    // Process all top level files
    TiXmlElement* topLevelFiles = xml.FirstChildElement("TopLevelFiles");
    if(topLevelFiles)
    {
        TiXmlElement* file = topLevelFiles->FirstChildElement("File");
        while(file)
        {
            string name = file->Attribute("name");
            string path = file->Attribute("path");
            string source = file->Attribute("source");

            // Check if this file has bee localised
            string localSetting;
            const char* local = file->Attribute("local");
            if(local && local[0])
            {
                localSetting = local;
            }

            m_MainGroup->AddFileToGroup(name, path, source, localSetting);

            file = file->NextSiblingElement("File");
        }
    }

    // Process the build configs
    TiXmlElement* buildConfigs = xml.FirstChildElement("BuildConfigs");
    if(!buildConfigs)
    {
        printf("PBXProject::ProcessProjectXML(): Project missing build configs\n");
        return false;
    }

    m_BuildConfigurationList = new XCConfigurationList();
    m_BuildConfigurationList->SetOwner(this);
    m_BuildConfigurationList->m_DefaultConfigurationIsVisible = buildConfigs->Attribute("defaultConfigurationIsVisible");
    m_BuildConfigurationList->m_DefaultConfigurationName = buildConfigs->Attribute("default");
    m_BuildConfigurationList->ProcessBuildConfigXML(*buildConfigs);

    // Process the attributes
    TiXmlElement* projectAttributes = xml.FirstChildElement("Attributes");
    if(!projectAttributes)
    {
        printf("PBXProject::ProcessProjectXML(): Project missing attributes\n");
        return false;
    }
    TiXmlElement* projectAttribute = projectAttributes->FirstChildElement();
    while(projectAttribute)
    {
        string name = projectAttribute->Value();
        string value = projectAttribute->GetText();

        m_AttributeName.push_back(name);
        m_AttributeValue.push_back(value);

        projectAttribute = projectAttribute->NextSiblingElement();
    }

    // Resolve all config file references in XCBuildConfiguration
    for(U32 i=0; i<XCBuildConfiguration::m_List.size(); ++i)
    {
        XCBuildConfiguration::m_List[i]->ResolveConfigFileReference();
    }

    return true;
}

bool PBXProject::DoesBuildPhaseExist(const string& buildPhase)
{
    for(U32 i=0; i<m_Targets.size(); ++i)
    {
        // Only check native targets (not aggregate)
        PBXNativeTarget* native = dynamic_cast<PBXNativeTarget*>(m_Targets[i]);
        if(native)
        {
            return native->DoesBuildPhaseExist(buildPhase);
        }
    }

    return false;
}

void PBXProject::AddBuildFileToTarget(PBXBuildFile* buildFile)
{
    for(U32 i=0; i<m_Targets.size(); ++i)
    {
        // Only add files to native targets (not aggregate)
        PBXNativeTarget* native = dynamic_cast<PBXNativeTarget*>(m_Targets[i]);
        if(native)
        {
            native->AddBuildFileToBuildPhase(buildFile);
        }
    }
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXProject)
{
    stream << endl << "/* Begin PBXProject section */" << endl;

    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXProject* project = m_List[i];

        stream << "\t\t";
        project->GetUUID().Write(stream);
        stream << " /* Project object */ = {" << endl;

        stream << "\t\t\tisa = " << project->GetISA() << ";" << endl;

        // Add a default attributes section.  What does this do?
        stream << "\t\t\tattributes = {" << endl;
        for(U32 j=0; j<project->m_AttributeName.size(); ++j)
        {
            stream << "\t\t\t\t" << project->m_AttributeName[j] << " = " << project->m_AttributeValue[j] << ";" << endl;
        }
        stream << "\t\t\t};" << endl;

        stream << "\t\t\tbuildConfigurationList = ";
        project->m_BuildConfigurationList->GetUUID().Write(stream);
        stream << " /* Build configuration list for PBXProject \"" << project->GetName() << "\" */;" << endl;

        stream << "\t\t\tcompatibilityVersion = \"" << project->m_CompatibilityVersion << "\";" << endl;

        stream << "\t\t\tdevelopmentRegion = " << project->m_DevelopmentRegion << ";" << endl;

        stream << "\t\t\thasScannedForEncodings = " << project->m_HasScannedForEncodings << ";" << endl;

        stream << "\t\t\tknownRegions = (" << endl;
        for(U32 j=0; j<project->m_KnownRegions.size(); ++j)
        {
            stream << "\t\t\t\t" << project->m_KnownRegions[j] << "," << endl;
        }
        stream << "\t\t\t);" << endl;

        stream << "\t\t\tmainGroup = ";
        project->m_MainGroup->GetUUID().Write(stream);
        stream << ";" << endl;

        stream << "\t\t\tproductRefGroup = ";
        project->m_ProductRefGroup->GetUUID().Write(stream);
        stream << " /* " << project->m_ProductRefGroup->GetName() << " */;" << endl;

        stream << "\t\t\tprojectDirPath = \"\";" << endl;

        stream << "\t\t\tprojectRoot = \"\";" << endl;

        stream << "\t\t\ttargets = (" << endl;
        for(U32 j=0; j<project->m_Targets.size(); ++j)
        {
            stream << "\t\t\t\t";
            project->m_Targets[j]->GetUUID().Write(stream);
            stream << " /* " << project->m_Targets[j]->GetName() << " */," << endl;
        }
        stream << "\t\t\t);" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXProject section */" << endl;

    return true;
}
