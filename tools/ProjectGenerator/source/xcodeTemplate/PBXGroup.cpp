//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXGroup.h"
#include "xcodeTemplate/PBXProject.h"
#include "xcodeTemplate/PBXFileReference.h"
#include "xcodeTemplate/PBXBuildFile.h"
#include "xcodeTemplate/FileTypeDatabase.h"
#include "xcodeTemplate/PBXVariantGroup.h"
#include "platform/stringFunctions.h"
#include "platform/fileIO.h"

IMPLEMENT_GLOBAL_LIST(PBXGroup)

PBXGroup::PBXGroup()
{
    m_ParentProject = NULL;

    // Default Source
    m_SourceTree = ST_AbsolutePath;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXGroup::~PBXGroup()
{
}

bool PBXGroup::ProcessGroupXML(PBXGroup* parent, TiXmlElement& xml)
{
    string type = xml.Attribute("type");
    if(Platform::StringCompareNoCase(type, string("files")))
    {
        string filePath = xml.Attribute("file");
        if(filePath.empty())
        {
            printf("PBXGroup::ProcessGroupXML(): Missing file path for group.\n");
            return false;
        }

        // Load up the group file
        TiXmlDocument xml;
        bool result = xml.LoadFile(filePath);
        if(!result)
        {
            printf("PBXGroup::ProcessGroupXML(): Cannot load group XML file '%s'.\n", filePath.c_str());
            return false;
        }

        TiXmlElement* groupTag = xml.FirstChildElement("Group");
        if(!groupTag)
        {
            printf("PBXGroup::ProcessGroupXML(): Group file '%s' is missing group tab.\n", filePath.c_str());
            return false;
        }

        // Process elements
        TiXmlElement* e = groupTag->FirstChildElement();
        while(e)
        {
            ProcessGroupElement(*e, true);

            e = e->NextSiblingElement();
        }
    }

    return true;
}

bool PBXGroup::ProcessGroupElement(TiXmlElement& xml, bool topLevel)
{
    // Is this an engine source directory?
    string type = xml.ValueStr();
    if(Platform::StringCompareNoCase(type, "EngineSourceDirectory"))
    {
        if(topLevel)
        {
            return ProcessEngineSourceDirectory(xml, topLevel, m_ParentProject->GetEngineSourcePath());
        }
        else
        {
            // Cannot process a child EngineSourceDirectory from here.
            printf("PBXGroup::ProcessGroupElements(): Cannot process a child EngineSourceDirectory.\n");
            return false;
        }
    }
    else if(Platform::StringCompareNoCase(type, "EngineLibDirectory"))
    {
        if(topLevel)
        {
            return ProcessEngineLibDirectory(xml, topLevel, m_ParentProject->GetEngineLibPath());
        }
        else
        {
            // Cannot process a child EngineLibDirectory from here.
            printf("PBXGroup::ProcessGroupElements(): Cannot process a child EngineLibDirectory.\n");
            return false;
        }
    }
    else if(Platform::StringCompareNoCase(type, "NamedGroup"))
    {
        return ProcessNamedGroup(xml, topLevel);
    }
    else if(Platform::StringCompareNoCase(type, "FrameworkDirectory"))
    {
        return ProcessFrameworkDirectory(xml, topLevel);
    }

    // Not processed
    printf("PBXGroup::ProcessGroupElements(): Unknown directory type of '%s'.\n", type.c_str());
    return false;
}

bool PBXGroup::ProcessEngineSourceDirectory(TiXmlElement& xml, bool topLevel, const string& parentPath)
{
    // Extract the name for this group
    m_Name = xml.Attribute("name");

    string sourcePath;
    string groupPath;
    if(topLevel)
    {
		Platform::MakeFullPathName(parentPath, sourcePath);
        m_Path = "\"" + sourcePath + m_Name + "\"";
        m_SourceTree = PBXBase::ST_AbsolutePath;
        
        Platform::MakeFullPathName(m_Name, groupPath, &sourcePath);
    }
    else
    {
        m_Path = m_Name;
        m_SourceTree = PBXBase::ST_RelativeToGroup;
        Platform::MakeFullPathName(m_Path, sourcePath, &parentPath);
        groupPath = sourcePath;
    }

    S32 includeChildren = 0;
    xml.Attribute("includeChildren", &includeChildren);

    // Work on any child XML elements.  NOTE: Must be the same type as this element
    TiXmlElement* e = xml.FirstChildElement("EngineSourceDirectory");
    while(e)
    {
        PBXGroup* group = new PBXGroup();
        group->SetParentProject(m_ParentProject);
        bool result = group->ProcessEngineSourceDirectory(*e, false, groupPath);
        if(!result)
            return false;

        m_ChildList.push_back(group);

        e = e->NextSiblingElement("EngineSourceDirectory");
    }

    // Add files to the group
    AddFilesToGroup(groupPath, (bool)includeChildren);

    return true;
}

bool PBXGroup::ProcessEngineLibDirectory(TiXmlElement& xml, bool topLevel, const string& parentPath)
{
    // Extract the name for this group
    m_Name = xml.Attribute("name");

    string sourcePath;
    string groupPath;
    if(topLevel)
    {
        m_Path = "\"$(TSS_LIB_PATH)/" + m_Name + "\"";
        m_SourceTree = PBXBase::ST_AbsolutePath;
        Platform::MakeFullPathName(parentPath, sourcePath);
        Platform::MakeFullPathName(m_Name, groupPath, &sourcePath);
    }
    else
    {
        m_Path = m_Name;
        m_SourceTree = PBXBase::ST_RelativeToGroup;
        Platform::MakeFullPathName(m_Path, sourcePath, &parentPath);
        groupPath = sourcePath;
    }

    S32 includeChildren = 0;
    xml.Attribute("includeChildren", &includeChildren);

    // Work on any child XML elements.  NOTE: Must be the same type as this element
    TiXmlElement* e = xml.FirstChildElement("EngineLibDirectory");
    while(e)
    {
        PBXGroup* group = new PBXGroup();
        group->SetParentProject(m_ParentProject);
        bool result = group->ProcessEngineLibDirectory(*e, false, groupPath);
        if(!result)
            return false;

        m_ChildList.push_back(group);

        e = e->NextSiblingElement("EngineLibDirectory");
    }

    // Add files to the group
    AddFilesToGroup(groupPath, (bool)includeChildren);

    return true;
}

bool PBXGroup::ProcessNamedGroup(TiXmlElement& xml, bool topLevel)
{
    // Extract the name for this group
    string name = xml.Attribute("name");
    m_Name = "\"" + name + "\"";

    // Optional path
    const string* path = xml.Attribute(string("path"));
    if(path && !path->empty())
    {
        m_Path = *path;
    }

    // Optional source tree
    const string* source = xml.Attribute(string("source"));
    if(source && !source->empty())
    {
        m_SourceTree = PBXBase::FrameworkSourceToSourceTree(source->c_str());
    }
    else
    {
        // Make relative to group
        m_SourceTree = PBXBase::ST_RelativeToGroup;
    }

    // Work on any child XML elements
    TiXmlElement* e = xml.FirstChildElement();
    while(e)
    {
        // Check if this is a lone file
        string value = e->Value();
        if(Platform::StringCompareNoCase(value, "File"))
        {
            string name = e->Attribute("name");
            string path = e->Attribute("path");
            string source = e->Attribute("source");

            // Check if this file has bee localised
            string localSetting;
            const char* local = e->Attribute("local");
            if(local && local[0])
            {
                localSetting = local;
            }

            AddFileToGroup(name, path, source, localSetting);
        }
        // Check if this is a folder reference
        else if(Platform::StringCompareNoCase(value, "Folder"))
        {
            string name = e->Attribute("name");
            string path = e->Attribute("path");
            string source = e->Attribute("source");
            string buildPhase = e->Attribute("buildPhase");

            AddFolderReferenceToGroup(name, path, source, buildPhase);
        }
        else
        {
            // Handle as a new group
            PBXGroup* group = new PBXGroup();
            group->SetParentProject(m_ParentProject);
            // Named groups pass along their topLevel-ness
            bool result = group->ProcessGroupElement(*e, topLevel);
            if(!result)
                return false;

            m_ChildList.push_back(group);
        }

        e = e->NextSiblingElement();
    }

    return true;
}

bool PBXGroup::ProcessFrameworkDirectory(TiXmlElement& xml, bool topLevel)
{
    // Extract the name for this group
    string name = xml.Attribute("name");
    m_Name = "\"" + name + "\"";

    // Make relative to group
    m_SourceTree = PBXBase::ST_RelativeToGroup;

    // Work on any child XML elements
    TiXmlElement* e = xml.FirstChildElement();
    while(e)
    {
        string value = e->Value();
        if(Platform::StringCompareNoCase(value, "Framework"))
        {
            AddFrameworkToGroup(*e);
        }
        else
        {
            // Assume this is another group element
            PBXGroup* group = new PBXGroup();
            group->SetParentProject(m_ParentProject);
            bool result = group->ProcessGroupElement(*e, false);
            if(!result)
                return false;

            m_ChildList.push_back(group);
        }

        e = e->NextSiblingElement();
    }

    return true;
}

bool PBXGroup::AddFilesToGroup(string& path, bool includeChildren)
{
    if(includeChildren)
    {
        // Get a list of all directories
        vector<string> dirList;
        Platform::DumpDirectories(path, dirList);

        // Process each directory as a group
        for(U32 i=0; i<dirList.size(); ++i)
        {
            string file = dirList[i];

            // TODO: Create group and process based in this directory name.  Needs to be <group> source type
            PBXGroup* group = new PBXGroup();
            group->SetName(file);
            group->SetParentProject(m_ParentProject);
            group->m_SourceTree = PBXBase::ST_RelativeToGroup;
            group->m_Path = file;

            string fullPath;
            Platform::MakeFullPathName(file, fullPath, &path);
            group->AddFilesToGroup(fullPath, includeChildren);

            m_ChildList.push_back(group);
        }
    }

    // Get a list of all files
    vector<string> fileList;
    Platform::DumpFiles(path, fileList);

    // Add the files to the group
    for(U32 i=0; i<fileList.size(); ++i)
    {
        string file = fileList[i];
        string localSetting;

        AddFileToGroup(file, file, "RelativeToGroup", localSetting);
    }

    return true;
}

bool PBXGroup::AddFrameworkToGroup(TiXmlElement& xml)
{
    string name = xml.Attribute("name");
    string path = xml.Attribute("path");
    string source = xml.Attribute("source");

    string localSetting;
    AddFileToGroup(name, path, source, localSetting);

    return true;
}

bool PBXGroup::AddFileToGroup(const string& name, const string& path, const string& source, const string& localSetting)
{
    // Check if the file exists in the extension table.  If not then don't include
    // it in project.
    string extension;
    Platform::GetFileExtension(name, extension);
    if(!FileTypeDatabase::singleton()->HasFileType(extension))
    {
        printf("PBXGroup::AddFileToGroup(): File not found in type database: '%s'\n", name.c_str());
        return false;
    }
    
    // Build the PBXFileReference
    PBXFileReference* ref = new PBXFileReference();
    ref->SetName(name);
    ref->m_Path = path;
    ref->m_SourceTree = PBXBase::FrameworkSourceToSourceTree(source.c_str());
    ref->ResolveFilePath();
    ref->ResolveFileType();
    
    // If this file has not been localised then just add it to the group
    PBXVariantGroup* vg = NULL;
    if(localSetting.empty())
    {
        // Not localised to add the file reference directly to the group
        m_ChildList.push_back(ref);
    }
    else
    {
        // File has been localised.  Maybe there is a variant group already made?
        for(U32 i=0; i<PBXVariantGroup::m_List.size(); ++i)
        {
            if(Platform::StringCompare(PBXVariantGroup::m_List[i]->GetName(), name))
            {
                // Found a group
                vg = PBXVariantGroup::m_List[i];
                break;
            }
        }

        if(!vg)
        {
            // Need to create a new variant group
            vg = new PBXVariantGroup();
            vg->SetName(name);
        }

        // Put the file reference on the variant group
        vg->m_ChildList.push_back(ref);

        // Push the variant group on to this group instead of the file reference
        m_ChildList.push_back(vg);
    }

    // Build the PBXBuildFile if necessary
    string buildPhase = FileTypeDatabase::singleton()->GetFileBuildPhase(extension);
    if(buildPhase.empty())
    {
        printf("PBXGroup::AddFileToGroup(): Unknown build phase for '%s'\n", name.c_str());
    }
    else if(!Platform::StringCompareNoCase(buildPhase, "NONE") && m_ParentProject->DoesBuildPhaseExist(buildPhase))
    {
        PBXBuildFile* buildFile = new PBXBuildFile();
        buildFile->SetName(name);
        buildFile->SetBuildPhase(buildPhase);

        if(vg)
        {
            // Point to the variance group rather than the file reference
            buildFile->m_FileReference = vg;
        }
        else
        {
            // No variant group so use the file reference
            buildFile->m_FileReference = ref;
        }

        // Add to the project's target
        m_ParentProject->AddBuildFileToTarget(buildFile);
    }

    return true;
}

bool PBXGroup::AddFolderReferenceToGroup(const string& name, const string& path, const string& source, const string& buildPhase)
{
    // Build the PBXFileReference
    PBXFileReference* ref = new PBXFileReference();
    ref->SetName(name);
    ref->m_Path = path;
    ref->m_SourceTree = PBXBase::FrameworkSourceToSourceTree(source.c_str());
    ref->m_LastKnownFileType = "folder";

    // Add PBXFileReference to the group
    m_ChildList.push_back(ref);

    // Build the PBXBuildFile if necessary
    if(!Platform::StringCompareNoCase(buildPhase, "NONE") && m_ParentProject->DoesBuildPhaseExist(buildPhase))
    {
        PBXBuildFile* buildFile = new PBXBuildFile();
        buildFile->SetName(name);
        buildFile->SetBuildPhase(buildPhase);
        buildFile->m_FileReference = ref;

        // Add to the project's target
        m_ParentProject->AddBuildFileToTarget(buildFile);
    }

    return true;
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXGroup)
{
    stream << endl << "/* Begin PBXGroup section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXGroup* group = m_List[i];

        stream << "\t\t";
        group->GetUUID().Write(stream);
        if(!group->GetName().empty())
        {
            stream << " /* " << group->GetName() << " */";
        }
        stream << " = {" << endl;

        stream << "\t\t\tisa = " << group->GetISA() << ";" << endl;

        if(group->m_ChildList.size() > 0)
        {
            stream << "\t\t\tchildren = (" << endl;

            for(U32 j=0; j<group->m_ChildList.size(); ++j)
            {
                stream << "\t\t\t\t";
                group->m_ChildList[j]->GetUUID().Write(stream);
                if(!group->m_ChildList[j]->GetName().empty())
                {
                    stream << " /* " << group->m_ChildList[j]->GetName() << " */";
                }
                stream << "," << endl;
            }

            stream << "\t\t\t);" << endl;
        }

        if(!group->m_Name.empty())
        {
            stream << "\t\t\tname = " << group->m_Name << ";" << endl;
        }

        if(!group->m_Path.empty())
        {
            stream << "\t\t\tpath = " << group->m_Path << ";" << endl;
        }

        stream << "\t\t\tsourceTree = " << PBXBase::SourceTreeName(group->m_SourceTree) << ";" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXGroup section */" << endl;

    return true;
}
