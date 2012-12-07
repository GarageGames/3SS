//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/PBXBase.h"
#include "platform/stringFunctions.h"

PBXBase::PBXBase()
{
    static U32 sNextReference = 1;

    // Set up the UUID
    m_UUID.Set(sNextReference++, 0, 0);
}

PBXBase::~PBXBase()
{
}

const char* PBXBase::SourceTreeName(PBXSourceTree value)
{
    switch(value)
    {
        case ST_AbsolutePath:
            return "\"<absolute>\"";

        case ST_RelativeToGroup:
            return "\"<group>\"";

        case ST_RelativeToProject:
            return "SOURCE_ROOT";

        case ST_RelativeToBuildProducts:
            return "BUILT_PRODUCTS_DIR";

        case ST_RelativeToDeveloperDirectory:
            return "DEVELOPER_DIR";

        case ST_RelativeToSDK:
            return "SDKROOT";
    }

    return "UNKNOWN";
}

PBXBase::PBXSourceTree PBXBase::SourceTreeValue(const char* name)
{
    if(Platform::StringCompareNoCase(name, "\"<absolute>\""))
    {
        return ST_AbsolutePath;
    }
    else if(Platform::StringCompareNoCase(name, "\"<group>\""))
    {
        return ST_RelativeToGroup;
    }
    else if(Platform::StringCompareNoCase(name, "SOURCE_ROOT"))
    {
        return ST_RelativeToProject;
    }
    else if(Platform::StringCompareNoCase(name, "BUILT_PRODUCTS_DIR"))
    {
        return ST_RelativeToBuildProducts;
    }
    else if(Platform::StringCompareNoCase(name, "DEVELOPER_DIR"))
    {
        return ST_RelativeToDeveloperDirectory;
    }
    else if(Platform::StringCompareNoCase(name, "SDKROOT"))
    {
        return ST_RelativeToSDK;
    }

    return ST_UNKNOWN;
}

PBXBase::PBXSourceTree PBXBase::FrameworkSourceToSourceTree(const char* name)
{
    if(Platform::StringCompareNoCase(name, "AbsolutePath"))
    {
        return ST_AbsolutePath;
    }
    else if(Platform::StringCompareNoCase(name, "RelativeToGroup"))
    {
        return ST_RelativeToGroup;
    }
    else if(Platform::StringCompareNoCase(name, "RelativeToProject"))
    {
        return ST_RelativeToProject;
    }
    else if(Platform::StringCompareNoCase(name, "RelativeToBuildProducts"))
    {
        return ST_RelativeToBuildProducts;
    }
    else if(Platform::StringCompareNoCase(name, "RelativeToDeveloperDirectory"))
    {
        return ST_RelativeToDeveloperDirectory;
    }
    else if(Platform::StringCompareNoCase(name, "RelativeToSDK"))
    {
        return ST_RelativeToSDK;
    }
    else if(Platform::StringCompareNoCase(name, "SOURCE_ROOT"))
    {
        return ST_RelativeToProject;
    }
    else if(Platform::StringCompareNoCase(name, "BUILT_PRODUCTS_DIR"))
    {
        return ST_RelativeToBuildProducts;
    }
    else if(Platform::StringCompareNoCase(name, "DEVELOPER_DIR"))
    {
        return ST_RelativeToDeveloperDirectory;
    }
    else if(Platform::StringCompareNoCase(name, "SDKROOT"))
    {
        return ST_RelativeToSDK;
    }
    // Indicates that we should force an absolute path based on the given relative path
    else if(Platform::StringCompareNoCase(name, "CalculateAbsolute"))
    {
        return ST_CalculateAbsolutePath;
    }
    
    return ST_UNKNOWN;
}
