//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXNativeTarget.h"
#include "xcodeTemplate/PBXProject.h"
#include "xcodeTemplate/PBXFileReference.h"
#include "xcodeTemplate/PBXBaseBuildPhase.h"
#include "xcodeTemplate/PBXHeadersBuildPhase.h"
#include "xcodeTemplate/PBXResourcesBuildPhase.h"
#include "xcodeTemplate/PBXSourcesBuildPhase.h"
#include "xcodeTemplate/PBXFrameworksBuildPhase.h"
#include "xcodeTemplate/PBXRezBuildPhase.h"
#include "xcodeTemplate/PBXCopyFilesBuildPhase.h"
#include "xcodeTemplate/XCConfigurationList.h"
#include "xcodeTemplate/XCBuildConfiguration.h"
#include "xcodeTemplate/PBXBuildFile.h"
#include "xcodeTemplate/PBXGroup.h"
#include "xcodeTemplate/PBXContainerItemProxy.h"
#include "xcodeTemplate/PBXTargetDependency.h"
#include "platform/stringFunctions.h"

IMPLEMENT_GLOBAL_LIST(PBXNativeTarget)

PBXNativeTarget::PBXNativeTarget()
{
    m_BuildConfigurationList = NULL;

    m_ProductReference = NULL;

    m_ParentProject = NULL;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXNativeTarget::~PBXNativeTarget()
{
}

bool PBXNativeTarget::ProcessXMLFile(const string& filePath, PBXProject* parent)
{
    m_ParentProject = parent;

    // Read the given file
    TiXmlDocument xml;
    bool result = xml.LoadFile(filePath);
    if(!result)
    {
        printf("PBXNativeTarget::ProcessXMLFile(): Cannot load target XML file '%s'.\n", filePath.c_str());
        return false;
    }

    // Process the target entry
    TiXmlElement* root = xml.FirstChildElement("Target");
    if(!root)
    {
        printf("PBXNativeTarget::ProcessXMLFile(): Target XML file '%s' is missing root element\n", filePath.c_str());
        return false;
    }

    string name = root->Attribute("name");
    SetName(name);

    m_ProductName = root->Attribute("productName");

    m_ProductType = root->Attribute("productType");

    // Process build phases
    TiXmlElement* buildPhases = root->FirstChildElement("BuildPhases");
    if(!buildPhases)
    {
        printf("PBXNativeTarget::ProcessXMLFile(): Target XML file '%s' is missing build phases\n", filePath.c_str());
        return false;
    }

    TiXmlElement* buildPhase = buildPhases->FirstChildElement("BuildPhase");
    while(buildPhase)
    {
        string type = buildPhase->Attribute("type");
        string buildActionMask = buildPhase->Attribute("buildActionMask");
        string runOnlyForDeploymentPostprocessing = buildPhase->Attribute("runOnlyForDeploymentPostprocessing");

        // Create the buildPhase
        PBXBaseBuildPhase* phase = NULL;
        if(Platform::StringCompareNoCase(type, "Headers"))
        {
            phase = new PBXHeadersBuildPhase();
            phase->m_BuildActionMask = buildActionMask;
            phase->m_RunOnlyForDeploymentPostprocessing = runOnlyForDeploymentPostprocessing;
            phase->m_ParentTarget = this;
        }
        else if(Platform::StringCompareNoCase(type, "Resources"))
        {
            phase = new PBXResourcesBuildPhase();
            phase->m_BuildActionMask = buildActionMask;
            phase->m_RunOnlyForDeploymentPostprocessing = runOnlyForDeploymentPostprocessing;
            phase->m_ParentTarget = this;
        }
        else if(Platform::StringCompareNoCase(type, "Sources"))
        {
            phase = new PBXSourcesBuildPhase();
            phase->m_BuildActionMask = buildActionMask;
            phase->m_RunOnlyForDeploymentPostprocessing = runOnlyForDeploymentPostprocessing;
            phase->m_ParentTarget = this;
        }
        else if(Platform::StringCompareNoCase(type, "Frameworks"))
        {
            phase = new PBXFrameworksBuildPhase();
            phase->m_BuildActionMask = buildActionMask;
            phase->m_RunOnlyForDeploymentPostprocessing = runOnlyForDeploymentPostprocessing;
            phase->m_ParentTarget = this;
        }
        else if(Platform::StringCompareNoCase(type, "Rez"))
        {
            phase = new PBXRezBuildPhase();
            phase->m_BuildActionMask = buildActionMask;
            phase->m_RunOnlyForDeploymentPostprocessing = runOnlyForDeploymentPostprocessing;
            phase->m_ParentTarget = this;
        }
        else if(Platform::StringCompareNoCase(type, "CopyFiles"))
        {
            PBXCopyFilesBuildPhase* copyFiles = new PBXCopyFilesBuildPhase();
            copyFiles->m_BuildActionMask = buildActionMask;
            copyFiles->m_RunOnlyForDeploymentPostprocessing = runOnlyForDeploymentPostprocessing;
            copyFiles->m_DstPath = buildPhase->Attribute("dstPath");
            copyFiles->m_DstSubfolderSpec = buildPhase->Attribute("dstSubfolderSpec");
            copyFiles->m_ParentTarget = this;

            // This build phase may not be completely supported at this time.  There is nothing in place
            // to manually add files to copy.

            phase = copyFiles;
        }
        else if(Platform::StringCompareNoCase(type, "ShellScript"))
        {
            // Not supported at this time
        }

        if(phase)
        {
            m_BuildPhases.push_back(phase);
        }

        buildPhase = buildPhase->NextSiblingElement("BuildPhase");
    }

    // Process build configurations
    TiXmlElement* buildConfigs = root->FirstChildElement("BuildConfigs");
    if(!buildConfigs)
    {
        printf("PBXNativeTarget::ProcessXMLFile(): Target XML file '%s' is missing build configurations\n", filePath.c_str());
        return false;
    }

    m_BuildConfigurationList = new XCConfigurationList();
    m_BuildConfigurationList->SetOwner(this);
    m_BuildConfigurationList->m_DefaultConfigurationIsVisible = buildConfigs->Attribute("defaultConfigurationIsVisible");
    m_BuildConfigurationList->m_DefaultConfigurationName = buildConfigs->Attribute("default");

    // A config file may be referenced for the build config
    PBXFileReference* configFile = NULL;
    const char* configReference = buildConfigs->Attribute("baseConfigurationReference");
    if(configReference && configReference[0])
    {
        m_BuildConfigurationList->SetBaseConfigurationReferencePath(configReference);
    }

    m_BuildConfigurationList->ProcessBuildConfigXML(*buildConfigs);

    // Process the product
    TiXmlElement* productFile = root->FirstChildElement("ProductFile");
    if(!buildConfigs)
    {
        printf("PBXNativeTarget::ProcessXMLFile(): Target XML file '%s' is missing product file.\n", filePath.c_str());
        return false;
    }
    m_ProductReference = new PBXFileReference();
    m_ProductReference->SetName(productFile->Attribute("name"));
    m_ProductReference->m_Path = productFile->Attribute("path");
    string sourceTree = productFile->Attribute("sourceTree");
    m_ProductReference->m_SourceTree = PBXBase::SourceTreeValue(sourceTree.c_str());
    m_ProductReference->ResolveFilePath();
    m_ProductReference->ResolveFileType();
    m_ProductReference->SetWriteFileEncoding(false);

    // Add the product to the group
    m_ParentProject->m_ProductRefGroup->m_ChildList.push_back(m_ProductReference);

    // Do we need to build a proxy?
    S32 buildProxy = 0;
    root->Attribute("buildProxy", &buildProxy);
    if((bool)buildProxy)
    {
        // Create the PBXContainerItemProxy
        PBXContainerItemProxy* proxy = new PBXContainerItemProxy();
        proxy->m_RemoteGlobalIDString = this;
        proxy->m_ContainerPortal = m_ParentProject;
        proxy->m_RemoteInfo = GetName();

        // Create the PBXTargetDependency
        PBXTargetDependency* td = new PBXTargetDependency();
        td->m_Target = this;
        td->m_TargetProxy = proxy;
    }

    return true;
}

bool PBXNativeTarget::DoesBuildPhaseExist(const string& buildPhase)
{
    if(Platform::StringCompareNoCase(buildPhase, "Headers"))
    {
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXHeadersBuildPhase* phase = dynamic_cast<PBXHeadersBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                return true;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Resources"))
    {
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXResourcesBuildPhase* phase = dynamic_cast<PBXResourcesBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                return true;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Sources"))
    {
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXSourcesBuildPhase* phase = dynamic_cast<PBXSourcesBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                return true;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Frameworks"))
    {
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXFrameworksBuildPhase* phase = dynamic_cast<PBXFrameworksBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                return true;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Rez"))
    {
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXRezBuildPhase* phase = dynamic_cast<PBXRezBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                return true;
            }
        }
    }

    return false;
}

void PBXNativeTarget::AddBuildFileToBuildPhase(PBXBuildFile* buildFile)
{
    const string& buildPhase = buildFile->GetBuildPhase();

    if(Platform::StringCompareNoCase(buildPhase, "Headers"))
    {
        // Add to the first Headers build phase
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXHeadersBuildPhase* phase = dynamic_cast<PBXHeadersBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                phase->m_Files.push_back(buildFile);
                return;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Resources"))
    {
        // Add to the first Headers build phase
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXResourcesBuildPhase* phase = dynamic_cast<PBXResourcesBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                phase->m_Files.push_back(buildFile);
                return;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Sources"))
    {
        // Add to the first Headers build phase
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXSourcesBuildPhase* phase = dynamic_cast<PBXSourcesBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                phase->m_Files.push_back(buildFile);
                return;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Frameworks"))
    {
        // Add to the first Headers build phase
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXFrameworksBuildPhase* phase = dynamic_cast<PBXFrameworksBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                phase->m_Files.push_back(buildFile);
                return;
            }
        }
    }
    else if(Platform::StringCompareNoCase(buildPhase, "Rez"))
    {
        // Add to the first Headers build phase
        for(U32 i=0; i<m_BuildPhases.size(); ++i)
        {
            PBXRezBuildPhase* phase = dynamic_cast<PBXRezBuildPhase*>(m_BuildPhases[i]);
            if(phase)
            {
                phase->m_Files.push_back(buildFile);
                return;
            }
        }
    }
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXNativeTarget)
{
    stream << endl << "/* Begin PBXNativeTarget section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXNativeTarget* target = m_List[i];

        stream << "\t\t";
        target->GetUUID().Write(stream);
        if(!target->GetName().empty())
        {
            stream << " /* " << target->GetName() << " */";
        }
        stream << " = {" << endl;

        stream << "\t\t\tisa = " << target->GetISA() << ";" << endl;

        stream << "\t\t\tbuildConfigurationList = ";
        target->m_BuildConfigurationList->GetUUID().Write(stream);
        stream << " /* Build configuration list for PBXNativeTarget \""<< target->GetName() << "\" */;" << endl;

        stream << "\t\t\tbuildPhases = (" << endl;
        for(U32 j=0; j<target->m_BuildPhases.size(); ++j)
        {
            stream << "\t\t\t\t";
            target->m_BuildPhases[j]->GetUUID().Write(stream);
            if(!target->m_BuildPhases[j]->GetName().empty())
            {
                stream << " /* " << target->m_BuildPhases[j]->GetName() << " */";
            }
            stream << "," << endl;
        }
        stream << "\t\t\t);" << endl;

        // Build rules are not supported
        stream << "\t\t\tbuildRules = (" << endl;
        stream << "\t\t\t);" << endl;

        // Dependencies are not supported
        stream << "\t\t\tdependencies = (" << endl;
        stream << "\t\t\t);" << endl;

        stream << "\t\t\tname = " << target->m_Name << ";" << endl;
        stream << "\t\t\tproductName = \"" << target->m_ProductName << "\";" << endl;

        stream << "\t\t\tproductReference = ";
        target->m_ProductReference->GetUUID().Write(stream);
        stream << " /* "<< target->m_ProductReference->GetName() << " */;" << endl;

        stream << "\t\t\tproductType = \"" << target->m_ProductType << "\";" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXNativeTarget section */" << endl;

    return true;
}
