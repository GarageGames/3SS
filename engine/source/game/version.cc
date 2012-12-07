//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platform/platform.h"
#include "game/version.h"
#include "console/console.h"

static const U32 csgVersionNumber = TORQUE_GAME_ENGINE;

U32 getVersionNumber()
{
   return csgVersionNumber;
}

const char* getVersionString()
{
   return TORQUE_GAME_VERSION_STRING;
}

const char* getCompileTimeString()
{
   return __DATE__ " at " __TIME__;
}
//----------------------------------------------------------------

ConsoleFunction(isDebugBuild, bool, 1, 1, "() Use the isDebugBuild function to determine if this is a debug build.\n"
                                                                "@return Returns true if this is a debug build, otherwise false.\n"
                                                                "@sa getBuildString, getCompileTimeString, getVersionNumber, getVersionString")
{
#ifdef TORQUE_DEBUG
   return true;
#else
   return false;
#endif
}

ConsoleFunction( getVersionNumber, S32, 1, 1, "() Use the getVersionNumber function to get the version number of the currently executing engine.\n"
                                                                "@return Returns an integer representing the engine's version number.\n"
                                                                "@sa getBuildString, getCompileTimeString, getVersionString, isDebugBuild")
{
   return getVersionNumber();
}

ConsoleFunction( getVersionString, const char*, 1, 1, "() Use the getVersionString function to get the version name and number for the currently executing engine.\n"
                                                                "@return Returns a string containing a name and an integer representing the engine's version type and version number.\n"
                                                                "@sa getBuildString, getCompileTimeString, getVersionNumber, isDebugBuild")
{
   return getVersionString();
}

ConsoleFunction( getCompileTimeString, const char*, 1, 1, "() Use the getCompileTimeString function to determine when the currently running engine was built.\n"
                                                                "@return Returns a string containing \"Month Day Year at Hour:Minute:Second\" showing when this executable was built.\n"
                                                                "@sa getBuildString, getVersionNumber, getVersionString, isDebugBuild")
{
   return getCompileTimeString();
}

ConsoleFunction( getBuildString, const char*, 1, 1, "() Use the getBuildString function to determine if this build is a \"Debug\" release, or a \"Release\" build.\n"
                                                                "@return Returns a string, either \"Debug\" for a debug build, or \"Release\" for a release build.\n"
                                                                "@sa getCompileTimeString, getVersionNumber, getVersionString, isDebugBuild")
{
#ifdef TORQUE_DEBUG
   return "Debug";
#else
   return "Release";
#endif
}

//-----------------------------------------------------------------------------

ConsoleFunction( getEngineVersion, const char*, 1, 1, "() - Gets the engine version.")
{
    return T2D_ENGINE_VERSION;
}

//-----------------------------------------------------------------------------

ConsoleFunction( getiPhoneToolsVersion, const char*, 1, 1, "Returns iPhone Tools Version")
{
    return T2D_IPHONETOOLS_VERSION;
}
