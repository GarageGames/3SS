//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#ifndef _PBXBASE_H
#define _PBXBASE_H

#include "platform/types.h"
#include "xcodeTemplate/ElementReference.h"

// Sets the ISA value
#define DEFINE_ISA(className) \
    virtual const char* GetISA() { return #className; }

// Work with a list to hold all items of a particular class
#define DEFINE_GLOBAL_LIST(className) \
    static vector<className*> m_List;
#define IMPLEMENT_GLOBAL_LIST(className) \
    vector<className*> className::m_List;
#define ADD_TO_GLOBAL_LIST() \
    m_List.push_back(this);
#define DEFINE_WRITE_GLOBAL_LIST(className) \
    static bool WriteGlobalList(ofstream& stream);
#define IMPLEMENT_WRITE_GLOBAL_LIST(className) \
    bool className::WriteGlobalList(ofstream& stream)

// Not part of the actual Xcode project file, but forms the base for all
// other Xcode project files.
class PBXBase
{
public:
    enum PBXSourceTree {
        ST_UNKNOWN,
        ST_AbsolutePath,
        ST_RelativeToGroup,
        ST_RelativeToProject,
        ST_RelativeToBuildProducts,
        ST_RelativeToDeveloperDirectory,
        ST_RelativeToSDK,
        ST_CalculateAbsolutePath,
    };

protected:
    ElementReference m_UUID;

    string m_Name;
public:

    PBXBase();
    virtual ~PBXBase();

    virtual const char* GetISA() = 0;

    virtual ElementReference& GetUUID() { return m_UUID; }

    virtual string& GetName() { return m_Name; }
    virtual void SetName(const string& name) { m_Name = name; }

    static const char* SourceTreeName(PBXSourceTree value);
    static PBXSourceTree SourceTreeValue(const char* name);
    static PBXSourceTree FrameworkSourceToSourceTree(const char* name);
};

#endif  // _PBXBASE_H
