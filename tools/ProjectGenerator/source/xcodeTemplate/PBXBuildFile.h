//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXBUILDFILE_H
#define _PBXBUILDFILE_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class PBXFileReference;

class PBXBuildFile : public PBXBase
{
protected:
    // The name of the build phase this file belongs to.
    // Used in comments when writing to project file.
    string m_BuildPhase;
public:
    PBXBase* m_FileReference;

public:
    PBXBuildFile();
    virtual ~PBXBuildFile();

    const string& GetBuildPhase() { return m_BuildPhase; }
    void SetBuildPhase(const string& phase) { m_BuildPhase = phase; }

    DEFINE_ISA(PBXBuildFile)
    DEFINE_GLOBAL_LIST(PBXBuildFile)
    DEFINE_WRITE_GLOBAL_LIST(PBXBuildFile)
};

#endif  // _PBXBUILDFILE_H
