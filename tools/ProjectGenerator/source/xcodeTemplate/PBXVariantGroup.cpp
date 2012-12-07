//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXVariantGroup.h"

IMPLEMENT_GLOBAL_LIST(PBXVariantGroup)

PBXVariantGroup::PBXVariantGroup()
{
    m_SourceTree = ST_RelativeToGroup;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXVariantGroup::~PBXVariantGroup()
{
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXVariantGroup)
{
    stream << endl << "/* Begin PBXVariantGroup section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXVariantGroup* group = m_List[i];

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

        stream << "\t\t\tsourceTree = " << PBXBase::SourceTreeName(group->m_SourceTree) << ";" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXVariantGroup section */" << endl;

    return true;
}
