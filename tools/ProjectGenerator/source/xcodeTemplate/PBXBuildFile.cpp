//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXBuildFile.h"
#include "xcodeTemplate/PBXFileReference.h"

IMPLEMENT_GLOBAL_LIST(PBXBuildFile)

PBXBuildFile::PBXBuildFile()
{
    m_FileReference = NULL;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXBuildFile::~PBXBuildFile()
{
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXBuildFile)
{
    stream << endl << "/* Begin PBXBuildFile section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXBuildFile* file = m_List[i];

        stream << "\t\t";
        file->GetUUID().Write(stream);
        if(!file->GetName().empty())
        {
            stream << " /* " << file->GetName() << " in " << file->GetBuildPhase() << " */";
        }
        stream << " = {";

        stream << "isa = " << file->GetISA() << ";";

        stream << " fileRef = ";
        file->m_FileReference->GetUUID().Write(stream);
        stream << " /* " << file->m_FileReference->GetName() << " */;";

        stream << " };" << endl;
    }

    stream << "/* End PBXBuildFile section */" << endl;

    return true;
}
