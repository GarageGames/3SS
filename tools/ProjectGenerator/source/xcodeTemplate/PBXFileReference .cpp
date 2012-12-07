//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXFileReference.h"
#include "xcodeTemplate/FileTypeDatabase.h"
#include "platform/fileIO.h"
#include "platform/stringFunctions.h"

IMPLEMENT_GLOBAL_LIST(PBXFileReference)

PBXFileReference::PBXFileReference() : PBXBase()
{
    // File encoding values:
    // 4  - UTF-8
    // 30 - Western
    // We will assume a file encoding of 4 for UTF-8
    m_FileEncoding = 4;

    m_WriteFileEncoding = true;

    // Default Source
    m_SourceTree = ST_AbsolutePath;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXFileReference::~PBXFileReference()
{
}

void PBXFileReference::ResolveFilePath()
{
    if(m_SourceTree == PBXBase::ST_CalculateAbsolutePath)
    {
        string absolutePath;
        Platform::MakeFullPathName(m_Path, absolutePath);
        m_Path = absolutePath;
        
        m_SourceTree = PBXBase::ST_AbsolutePath;
    }
}

void PBXFileReference::ResolveFileType()
{
    string extension;
    Platform::GetFileExtension(GetName(), extension);
    string fileType = FileTypeDatabase::singleton()->GetFileType(extension);
    if(fileType.empty())
    {
        printf("PBXFileReference::ResolveFileType(): Unknown file type for '%s'\n", GetName().c_str());
    }
    else if(FileTypeDatabase::singleton()->FileTypeIsExplicit(extension))
    {
        m_ExplicitFileType = fileType;
    }
    else
    {
        m_LastKnownFileType = fileType;
    }
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXFileReference)
{
    stream << endl << "/* Begin PBXFileReference section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXFileReference* file = m_List[i];

        stream << "\t\t";
        file->GetUUID().Write(stream);
        if(!file->GetName().empty())
        {
            stream << " /* " << file->GetName() << " */";
        }
        stream << " = {";

        stream << "isa = " << file->GetISA() << ";";

        if(file->GetWriteFileEncoding())
        {
            stream << " fileEncoding = " << file->m_FileEncoding << ";";
        }

        if(!file->m_LastKnownFileType.empty())
        {
            stream << " lastKnownFileType = " << file->m_LastKnownFileType << ";";
        }
        if(!file->m_ExplicitFileType.empty())
        {
            stream << " explicitFileType = " << file->m_ExplicitFileType << ";";
        }

        // If name and path are the same, only write the path
        if(Platform::StringCompare(file->GetName(), file->m_Path))
        {
            stream << " path = \"" << file->m_Path << "\";";
        }
        else
        {
            stream << " name = \"" << file->GetName() << "\";";
            stream << " path = \"" << file->m_Path << "\";";
        }

        stream << " sourceTree = " << PBXBase::SourceTreeName(file->m_SourceTree) << ";";

        stream << " };" << endl;
    }

    stream << "/* End PBXFileReference section */" << endl;

    return true;
}
