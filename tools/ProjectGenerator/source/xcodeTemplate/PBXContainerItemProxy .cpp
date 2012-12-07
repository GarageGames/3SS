//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXContainerItemProxy.h"
#include "xcodeTemplate/PBXNativeTarget.h"
#include "xcodeTemplate/PBXProject.h"

IMPLEMENT_GLOBAL_LIST(PBXContainerItemProxy)

PBXContainerItemProxy::PBXContainerItemProxy()
{
    m_ContainerPortal = NULL;
    m_RemoteGlobalIDString = NULL;

    m_ProxyType = "1";

    // Add ourselves to the global list
    ADD_TO_GLOBAL_LIST()
}

PBXContainerItemProxy::~PBXContainerItemProxy()
{
}

IMPLEMENT_WRITE_GLOBAL_LIST(PBXContainerItemProxy)
{
    stream << endl << "/* Begin PBXContainerItemProxy section */" << endl;
    U32 size = m_List.size();
    for(U32 i=0; i<size; ++i)
    {
        PBXContainerItemProxy* proxy = m_List[i];

        stream << "\t\t";
        proxy->GetUUID().Write(stream);
        stream << " /* " << proxy->GetISA() << " */";
        stream << " = {" << endl;

        stream << "\t\t\tisa = " << proxy->GetISA() << ";" << endl;

        stream << "\t\t\tcontainerPortal = ";
        proxy->m_ContainerPortal->GetUUID().Write(stream);
        stream << " /* Project object */;" << endl;

        stream << "\t\t\tproxyType = " << proxy->m_ProxyType << ";" << endl;

        stream << "\t\t\tremoteGlobalIDString = ";
        proxy->m_RemoteGlobalIDString->GetUUID().Write(stream);
        stream << ";" << endl;

        stream << "\t\t\tremoteInfo = " << proxy->m_RemoteInfo << ";" << endl;

        stream << "\t\t};" << endl;
    }

    stream << "/* End PBXContainerItemProxy section */" << endl;

    return true;
}
