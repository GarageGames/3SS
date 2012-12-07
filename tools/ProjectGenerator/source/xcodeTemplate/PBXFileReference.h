//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXFILEREFERENCE_H
#define _PBXFILEREFERENCE_H

#include "xcodeTemplate/PBXBase.h"
#include <string>
#include <vector>

using namespace std;

class PBXFileReference : public PBXBase
{
protected:
    bool m_WriteFileEncoding;

public:
    S32 m_FileEncoding;

    string m_ExplicitFileType;

    string m_LastKnownFileType;

    string m_Path;

    PBXSourceTree m_SourceTree;

public:
    PBXFileReference();
    virtual ~PBXFileReference();

    void ResolveFilePath();
    void ResolveFileType();

    bool GetWriteFileEncoding() { return m_WriteFileEncoding; }
    void SetWriteFileEncoding(bool state) { m_WriteFileEncoding = state; }

    DEFINE_ISA(PBXFileReference)
    DEFINE_GLOBAL_LIST(PBXFileReference)
    DEFINE_WRITE_GLOBAL_LIST(PBXFileReference)
};

#endif  // _PBXFILEREFERENCE_H
