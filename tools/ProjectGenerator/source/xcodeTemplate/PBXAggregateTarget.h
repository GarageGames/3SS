//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXAGGREGATETARGET_H
#define _PBXAGGREGATETARGET_H

#include "xcodeTemplate/PBXBase.h"

#include <string>
#include <vector>

using namespace std;

class XCConfigurationList;
class PBXTargetDependency;
class PBXProject;

class PBXAggregateTarget : public PBXBase
{
protected:
    PBXProject* m_ParentProject;

public:
    vector<string> m_BuildPhases;

    XCConfigurationList* m_BuildConfigurationList;

    vector<PBXTargetDependency*> m_Dependencies;

    string m_ProductName;

public:
    PBXAggregateTarget();
    virtual ~PBXAggregateTarget();

    bool ProcessXMLFile(const string& filePath, PBXProject* parent);

    DEFINE_ISA(PBXAggregateTarget)
    DEFINE_GLOBAL_LIST(PBXAggregateTarget)
    DEFINE_WRITE_GLOBAL_LIST(PBXAggregateTarget)
};

#endif  // _PBXAGGREGATETARGET_H
