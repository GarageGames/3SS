//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXTARGETDEPENDENCY_H
#define _PBXTARGETDEPENDENCY_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class PBXNativeTarget;
class PBXContainerItemProxy;

class PBXTargetDependency : public PBXBase
{
public:
    PBXNativeTarget* m_Target;

    PBXContainerItemProxy* m_TargetProxy;

public:
    PBXTargetDependency();
    virtual ~PBXTargetDependency();

    DEFINE_ISA(PBXTargetDependency)
    DEFINE_GLOBAL_LIST(PBXTargetDependency)
    DEFINE_WRITE_GLOBAL_LIST(PBXTargetDependency)
};

#endif  // _PBXTARGETDEPENDENCY_H
