//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "collection/vector.h"
#include "console/console.h"
#include "platform/platform.h"

static char* s_pStagingPath = (char*)"^DocumentsFileLocation/3SSStaging/";
static char* s_pSynchronizePath = (char*)"^CacheFileLocation/3SSStaging";
static char* s_pSynchronizeModulesPath = (char*)"^CacheFileLocation/3SSStaging/modules";

ConsoleFunctionGroupBegin(WebServices, "WebServices support functions");

ConsoleFunction(CleanUpdateManifestStaging, const char*, 1, 1,  "() Clean up the staging area used for building the update manifest\n"
                                                                "@return The path to the staging area\n")
{
    // Get the full path to the staging area
    char path[1024];
    bool result = Con::expandPath(path, 1024, s_pStagingPath, true);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("CleanUpdateManifestStaging(): Could not expand staging path");
        return "";
    }

    // Make sure the path exists
    bool doesExist = Platform::isDirectory(path);
    if(!doesExist)
    {
        result = Platform::createPath(path);
        if(!result)
        {
            // Could not create the staging path
            Con::warnf("CleanUpdateManifestStaging(): Could not create staging path");
            return "";
        }
    }

    // Get the list of files in the staging area
    Vector<Platform::FileInfo> files;
    Platform::dumpPath(path, files, 0);

    // Delete each file
    char filePath[1024];
    for (S32 i = 0; i < files.size(); i++)
    {
        Platform::makeFullPathName(files[i].pFileName, filePath, 1024, path);
        Platform::fileDelete(filePath);
    }

    // Return the path
    char *buf = Con::getReturnBuffer(dStrlen(path)+1);
    dStrcpy(buf, path);
    return buf;
}

ConsoleFunction(PrepareSynchronizeStaging, const char*, 1, 1,   "() Prepare the staging area used for downloading files\n"
                                                                "@return The path to the staging area\n")
{
    // Get the full path to the staging area
    char path[1024];
    bool result = Con::expandPath(path, 1024, s_pSynchronizePath, true);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("PrepareSynchronizeStaging(): Could not expand staging path");
        return "";
    }

    // Make sure the path exists
    result = Platform::createPath(path);
    if(!result)
    {
        // Could not create the staging path
        Con::warnf("PrepareSynchronizeStaging(): Could not create staging path");
        return "";
    }

    // Make the module directory
    char modulesPath[1024];
    result = Con::expandPath(modulesPath, 1024, s_pSynchronizeModulesPath, true);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("PrepareSynchronizeStaging(): Could not expand modules staging path");
        return "";
    }

    result = Platform::createPath(modulesPath);
    if(!result)
    {
        // Could not create the staging path
        Con::warnf("PrepareSynchronizeStaging(): Could not create modules staging path");
        return "";
    }

    // Remove the trailing slash from the modules path
    modulesPath[dStrlen(modulesPath)-1] = '\0';

    // Return the paths
    S32 size = dStrlen(path)+1 + dStrlen(modulesPath)+1;
    char *buf = Con::getReturnBuffer(size);
    dSprintf(buf, size, "%s\n%s", path, modulesPath);
    return buf;
}

ConsoleFunction(CleanSynchronizeStaging, void, 1, 1,    "() Clean up the staging area used for downloading files\n")
{
    // Get the full path to the staging area
    char path[1024];
    bool result = Con::expandPath(path, 1024, s_pSynchronizePath, false);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("CleanSynchronizeStaging(): Could not expand staging path");
        return;
    }

    // If the path doesn't exist then there is nothing to clean
    if(!Platform::isDirectory(path))
        return;

    // Get the list of files in the staging area
    Vector<Platform::FileInfo> files;
    Platform::dumpPath(path, files, 0);

    // Delete each file
    char filePath[1024];
    for (S32 i = 0; i < files.size(); i++)
    {
        Platform::makeFullPathName(files[i].pFileName, filePath, 1024, path);
        Platform::fileDelete(filePath);
    }

    // Clean up the modules directory
    char modulesPath[1024];
    result = Con::expandPath(modulesPath, 1024, s_pSynchronizeModulesPath, false);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("CleanSynchronizeStaging(): Could not expand modules staging path");
        return;
    }
    if(!Platform::isDirectory(modulesPath))
        return;

    // Delete all contents
    Platform::deleteDirectory(modulesPath);
}

ConsoleFunction(CreateModuleStagingPath, const char*, 3, 3, "(moduleId, version) Create the directory for the module in the staging area.\n"
                                                            "@return The path to where the module will be stored\n")
{
    char *buf = Con::getReturnBuffer(1024);
    dStrcpy(buf, "");

    char pathBuf[1024];
    char dirBuf[1024];

    // Build up the module path in the staging area
    bool result = Con::expandPath(pathBuf, 1024, s_pSynchronizeModulesPath, true);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("CreateModulePath(): Could not expand module staging path");
        return buf;
    }

    dStrcpy(buf, pathBuf);

    // Add the module and version to the path
    dSprintf(dirBuf, 1024, "%s/", argv[1]);
    Platform::makeFullPathName(dirBuf, pathBuf, 1024, buf);
    dStrcpy(buf, pathBuf);
    dSprintf(dirBuf, 1024, "%s/", argv[2]);
    Platform::makeFullPathName(dirBuf, pathBuf, 1024, buf);
    dStrcpy(buf, pathBuf);

    // Is this an existing directory?
    if(Platform::isDirectory(buf))
    {
        return buf;
    }

    // Create the new directory
    result = Platform::createPath(buf);
    if(!result)
    {
        // Could not create the path
        Con::warnf("CreateModulePath(): Could not create the module path");
        dStrcpy(buf, "");
        return buf;
    }

    // Module path has been created
    return buf;
}

ConsoleFunction(GetModuleStagingPath, const char*, 3, 3,    "(moduleId, version) Get the directory for the module in the staging area.\n"
                                                            "@return The path to where the module will be stored\n")
{
    char *buf = Con::getReturnBuffer(1024);
    dStrcpy(buf, "");

    char pathBuf[1024];
    char dirBuf[1024];

    // Build up the module path in the staging area
    bool result = Con::expandPath(pathBuf, 1024, s_pSynchronizeModulesPath, true);
    if(!result)
    {
        // Could not expand the path
        Con::warnf("CreateModulePath(): Could not expand module staging path");
        return buf;
    }

    dStrcpy(buf, pathBuf);

    // Add the module and version to the path
    dSprintf(dirBuf, 1024, "%s/", argv[1]);
    Platform::makeFullPathName(dirBuf, pathBuf, 1024, buf);
    dStrcpy(buf, pathBuf);
    dSprintf(dirBuf, 1024, "%s/", argv[2]);
    Platform::makeFullPathName(dirBuf, pathBuf, 1024, buf);
    dStrcpy(buf, pathBuf);

    // Module path
    return buf;
}

ConsoleFunctionGroupEnd(WebServices);
