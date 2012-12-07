//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXTargetDependency.h"
#include "xcodeTemplate/PBXNativeTarget.h"
#include "xcodeTemplate/PBXContainerItemProxy.h"

IMPLEMENT_GLOBAL_LIST(PBXTargetDependency)

PBXTargetDependency::PBXTargetDependency()
{
    m_Target = NULL;
    m_TargetProxy = NULL;

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXTargetDependency::~PBXTargetDependency()
{
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXTargetDependency)
{
    stream << endl << "/* Begin PBXTargetDependency section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXTargetDependency* target = m_List[i];

        stream << "\t\t";
        target->GetUUID().Write(stream);
        stream << " /* " << target->GetISA() << " */";
        stream << " = {" << endl;

        stream << "\t\t\tisa = " << target->GetISA() << ";" << endl;

        stream << "\t\t\ttarget = ";
        target->m_Target->GetUUID().Write(stream);
        stream << " /* " << target->m_Target->GetName() << " */;" << endl;
        
        stream << "\t\t\ttargetProxy = ";
        target->m_TargetProxy->GetUUID().Write(stream);
        stream << " /* PBXContainerItemProxy */;" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXTargetDependency section */" << endl;

    return true;
}
