//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXFrameworksBuildPhase.h"
#include "xcodeTemplate/PBXBuildFile.h"

IMPLEMENT_GLOBAL_LIST(PBXFrameworksBuildPhase)

PBXFrameworksBuildPhase::PBXFrameworksBuildPhase()
{
    SetName("Frameworks");

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXFrameworksBuildPhase::~PBXFrameworksBuildPhase()
{
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXFrameworksBuildPhase)
{
    stream << endl << "/* Begin PBXFrameworksBuildPhase section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXFrameworksBuildPhase* phase = m_List[i];

        stream << "\t\t";
        phase->GetUUID().Write(stream);
        if(!phase->GetName().empty())
        {
            stream << " /* " << phase->GetName() << " */";
        }
        stream << " = {" << endl;

        stream << "\t\t\tisa = " << phase->GetISA() << ";" << endl;
        stream << "\t\t\tbuildActionMask = " << phase->m_BuildActionMask << ";" << endl;

        stream << "\t\t\tfiles = (" << endl;
        for(U32 j=0; j<phase->m_Files.size(); ++j)
        {
            stream << "\t\t\t\t";
            phase->m_Files[j]->GetUUID().Write(stream);
            if(!phase->m_Files[j]->GetName().empty())
            {
                stream << " /* " << phase->m_Files[j]->GetName() << " in " << phase->GetName() << " */";
            }
            stream << "," << endl;
        }
        stream << "\t\t\t);" << endl;

        stream << "\t\t\trunOnlyForDeploymentPostprocessing = " << phase->m_RunOnlyForDeploymentPostprocessing << ";" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXFrameworksBuildPhase section */" << endl;

    return true;
}
