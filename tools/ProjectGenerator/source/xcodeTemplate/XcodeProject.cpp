//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/XcodeProject.h"
#include "xcodeTemplate/PBXProject.h"
#include "xcodeTemplate/PBXGroup.h"
#include "xcodeTemplate/PBXFileReference.h"
#include "xcodeTemplate/PBXBuildFile.h"
#include "xcodeTemplate/PBXCopyFilesBuildPhase.h"
#include "xcodeTemplate/PBXFrameworksBuildPhase.h"
#include "xcodeTemplate/PBXHeadersBuildPhase.h"
#include "xcodeTemplate/PBXResourcesBuildPhase.h"
#include "xcodeTemplate/PBXRezBuildPhase.h"
#include "xcodeTemplate/PBXSourcesBuildPhase.h"
#include "xcodeTemplate/XCConfigurationList.h"
#include "xcodeTemplate/PBXNativeTarget.h"
#include "xcodeTemplate/PBXTargetDependency.h"
#include "xcodeTemplate/PBXContainerItemProxy.h"
#include "xcodeTemplate/XCBuildConfiguration.h"
#include "xcodeTemplate/PBXAggregateTarget.h"
#include "xcodeTemplate/PBXVariantGroup.h"
#include "tinyXML/tinyxml.h"
#include "platform/fileIO.h"

XcodeProject::XcodeProject()
{
}

XcodeProject::~XcodeProject()
{
}

bool XcodeProject::CreateProjectFile(const string& filePath, const string& xcodePath)
{
    // Read the given file
    TiXmlDocument xml;
    bool result = xml.LoadFile(filePath);
    if(!result)
    {
        printf("XcodeProject::CreateProjectFile(): Cannot load project XML file.\n");
        return false;
    }

    // Process the project entry
    TiXmlElement* root = xml.FirstChildElement("Project");
    if(!root)
    {
        printf("XcodeProject::CreateProjectFile(): Project XML file is missing root element\n");
        return false;
    }

    PBXProject* project = new PBXProject();
    if(!project->ProcessProjectXML(*root))
    {
        return false;
    }

    // Turn the passed in project file to a directory
    string xcodePathAsDir = xcodePath + "/";

    // Make sure the project path exists
    string xcodeProjPath;
    Platform::MakeFullPathName(xcodePathAsDir, xcodeProjPath);
    Platform::CreatePath(xcodeProjPath.c_str());

    // Now attempt to open the .pbxproj file within the project
    string pbxProjPath;
    Platform::MakeFullPathName("project.pbxproj", pbxProjPath, &xcodeProjPath);
    m_File.open(pbxProjPath.c_str(), ios_base::out);
    if(m_File.fail())
    {
        // Could not open file
        printf("XcodeProject::CreateProjectFile(): Could not open output file.\n");
        return false;
    }

    // Write the file header
    WriteFileHeader();

    // Write the various sections
    PBXAggregateTarget::WriteGlobalList(m_File);
    PBXBuildFile::WriteGlobalList(m_File);
    PBXContainerItemProxy::WriteGlobalList(m_File);
    PBXCopyFilesBuildPhase::WriteGlobalList(m_File);
    PBXFileReference::WriteGlobalList(m_File);
    PBXFrameworksBuildPhase::WriteGlobalList(m_File);
    PBXGroup::WriteGlobalList(m_File);
    PBXHeadersBuildPhase::WriteGlobalList(m_File);
    PBXNativeTarget::WriteGlobalList(m_File);
    PBXProject::WriteGlobalList(m_File);
    PBXResourcesBuildPhase::WriteGlobalList(m_File);
    PBXRezBuildPhase::WriteGlobalList(m_File);
    // TODO: Shell script writing goes here
    PBXSourcesBuildPhase::WriteGlobalList(m_File);
    PBXTargetDependency::WriteGlobalList(m_File);
    PBXVariantGroup::WriteGlobalList(m_File);
    XCBuildConfiguration::WriteGlobalList(m_File);
    XCConfigurationList::WriteGlobalList(m_File);


    // Write the file footer
    WriteFileFooter(project);

    // Close the output file
    m_File.close();

    return true;
}

void XcodeProject::WriteFileHeader()
{
    m_File << "// !$*UTF8*$!" << endl;
    m_File << "{" << endl;
    m_File << "\tarchiveVersion = 1;" << endl;
    m_File << "\tclasses = {" << endl;
    m_File << "\t};" << endl;
    m_File << "\tobjectVersion = 46;" << endl;
    m_File << "\tobjects = {" << endl;
}

void XcodeProject::WriteFileFooter(PBXProject* project)
{
    m_File << "\t};" << endl;
    m_File << "\trootObject = ";
    project->GetUUID().Write(m_File);
    m_File << " /* Project object */;" << endl;
    m_File << "}" << endl;
}
