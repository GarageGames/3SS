//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef _TGB_REGMANAGER_H_
#define _TGB_REGMANAGER_H_

#ifndef _WINREGISTRY_H_
   #include "winRegistryAccess.h"
#endif

#include "sim/simBase.h"

/// Registered Windows Application Path Registry
///
/// We register TGB for with windows paths registry so that we may
/// allow add ons and external apps that need to know where TGB.exe is to either
/// a) simply reference our location by doing "TGB.exe yourArgs" if using TGB as 
///    a command line tool
/// b) Query the registry key below for the 'Path' value which will contains
///    TGB location on the system
///
/// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\TGB.exe

#define TSS_TOOL_BINARY_NAME "3StepStudio.exe"
#define TSS_TOOL_APPPATH_KEY "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\" TSS_TOOL_BINARY_NAME

#define TSS_GAME_RPATH       "templates/projectFiles"
#define TSS_GAME_BINARY_NAME "3StepStudioGame.exe"
#define TSS_GAME_APPPATH_KEY "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\" TSS_GAME_BINARY_NAME

// .tssproj Association
#define TSS_PROJECT_EXTENSION    ".tssproj"
#define TSS_PROJECT_DESCRIPTION  "3 Step Studio Project"
#define TSS_PROJECT_KEYNAME      "TSSProject"
#define TSS_PROJECT_FILEICON     0

#define TSS_SCENEFILEICON 0

class TGBRegistryManager : public SimObject
{
private:
   typedef SimObject Parent;

   RegistryObject mRegistryObj;
public:
   DECLARE_CONOBJECT(TGBRegistryManager);
   TGBRegistryManager();
   ~TGBRegistryManager();

   // File Association Methods
   bool fileAssociationExists();
   bool setFileAssociations();

   bool registerExtension( char *fileExtension, char *associationKeyName, char *extensionDescription, S32 nIconIndex );
   bool removeFileAssociations();

   // Registered Executable Path Methods
   bool toolExecutablePathIsRegistered();
   bool gameExecutablePathIsRegistered();
   bool executablePathsAreRegistered();
   bool registerExecutablePaths();
   bool unregisterExecutablePaths();
   bool setExecutablePath( const char *key, const char *path, const char *exename );
};

#endif