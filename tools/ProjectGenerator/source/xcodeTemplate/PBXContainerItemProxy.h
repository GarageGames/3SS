//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXCONTAINERITEMPROXY_H
#define _PBXCONTAINERITEMPROXY_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class PBXNativeTarget;
class PBXProject;

class PBXContainerItemProxy : public PBXBase
{
public:
    PBXProject* m_ContainerPortal;

    string m_ProxyType;

    PBXNativeTarget* m_RemoteGlobalIDString;

    string m_RemoteInfo;

public:
    PBXContainerItemProxy();
    virtual ~PBXContainerItemProxy();

    DEFINE_ISA(PBXContainerItemProxy)
    DEFINE_GLOBAL_LIST(PBXContainerItemProxy)
    DEFINE_WRITE_GLOBAL_LIST(PBXContainerItemProxy)
};

#endif  // _PBXCONTAINERITEMPROXY_H
