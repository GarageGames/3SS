//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _ENGINE_VERSION_H_
#define _ENGINE_VERSION_H_

// Engine Version.
#define T2D_ENGINE_VERSION      		"v1.7.5"        ///< Engine Version String.
#define T2D_IPHONETOOLS_VERSION      	"v1.5"          ///< Engine Version String for iPhone tools. Changing this will allow a fresh AppData folder to avoid conflicts with other builds existing on the system.

/// Gets the specified version number.  The version number is specified as a global in version.cc
U32 getVersionNumber();

/// Gets the version number in string form
const char* getVersionString();

/// Gets the compile date and time
const char* getCompileTimeString();

#endif // _ENGINE_VERSION_H_
