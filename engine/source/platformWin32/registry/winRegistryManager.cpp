//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef TORQUE_PLAYER 

#include "winRegistryManager.h"
#include "sim/simBase.h"

//#pragma message("This class assumes that the user has access to read/write from the registry, this may need to be changed later [10/30/2006 justin]") 

IMPLEMENT_CONOBJECT(TGBRegistryManager);
//-----------------------------------------------------------------------------
static void backslash(char *str)
{
   while(*str)
   {
      if(*str == '/')
         *str = '\\';
      str++;
   }
}
//-----------------------------------------------------------------------------
TGBRegistryManager::TGBRegistryManager()
{
}
//-----------------------------------------------------------------------------
TGBRegistryManager::~TGBRegistryManager()
{
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::fileAssociationExists()
{
   // Need Classes Root
   mRegistryObj.setRoot( HKEY_CLASSES_ROOT );

   // Check if all of the required keys exist
   return ( mRegistryObj.keyExists( TSS_PROJECT_EXTENSION ) 
         && mRegistryObj.keyExists( TSS_PROJECT_KEYNAME ) );
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::setFileAssociations()
{
   // Need Classes Root
   mRegistryObj.setRoot( HKEY_CLASSES_ROOT );

   // .t2dproj Association
   if( !registerExtension( TSS_PROJECT_EXTENSION, TSS_PROJECT_KEYNAME, TSS_PROJECT_DESCRIPTION, TSS_PROJECT_FILEICON ) )
   {
      removeFileAssociations();

      return false;
   }
   
   return true;
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::removeFileAssociations()
{
   // Need Classes Root
   mRegistryObj.setRoot( HKEY_CLASSES_ROOT );

   // Remove all keys
   bool key1Del = mRegistryObj.deleteKey( TSS_PROJECT_EXTENSION );
   bool key2Del = mRegistryObj.deleteKey( TSS_PROJECT_KEYNAME );
   
   return ( key1Del && key2Del ); 
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::executablePathsAreRegistered()
{
   return toolExecutablePathIsRegistered() && gameExecutablePathIsRegistered();
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::toolExecutablePathIsRegistered()
{
   // Need HKLM
   mRegistryObj.setRoot( HKEY_LOCAL_MACHINE );

   return mRegistryObj.keyExists( TSS_TOOL_APPPATH_KEY );
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::gameExecutablePathIsRegistered()
{
   // Need HKLM
   mRegistryObj.setRoot( HKEY_LOCAL_MACHINE );

   return mRegistryObj.keyExists( TSS_GAME_APPPATH_KEY );
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::registerExecutablePaths()
{
   const char *basePath = Platform::getExecutablePath();

   char gamePath[ 1024 ];

   dSprintf( gamePath, 1024, "%s/%s", basePath, TSS_GAME_RPATH );

   return setExecutablePath( TSS_TOOL_APPPATH_KEY, basePath, TSS_TOOL_BINARY_NAME )
      &&  setExecutablePath( TSS_GAME_APPPATH_KEY, gamePath, TSS_GAME_BINARY_NAME );
}

bool TGBRegistryManager::unregisterExecutablePaths()
{
   // Need HKLM
   mRegistryObj.setRoot( HKEY_LOCAL_MACHINE );

   return ( mRegistryObj.deleteKey( TSS_TOOL_APPPATH_KEY ) && mRegistryObj.deleteKey( TSS_GAME_APPPATH_KEY ) );
}

//-----------------------------------------------------------------------------
bool TGBRegistryManager::setExecutablePath( const char *key, const char *path, const char *exename )
{   
   // Need Classes Root
   mRegistryObj.setRoot( HKEY_LOCAL_MACHINE );
   
   // Note, we always refresh it without checking if it already exists just in case.
   if( mRegistryObj.createKey( key ) )
   {   
      char szBuffer[1024];

      dSprintf( szBuffer, 1024, "%s/%s", path, exename );

      backslash( szBuffer );

      return mRegistryObj.setString( key, "", szBuffer );
   }

   return false;
}
//-----------------------------------------------------------------------------
bool TGBRegistryManager::registerExtension( char *fileExtension, char *associationKeyName, char *extensionDescription, S32 nIconIndex  )
{
   // Setup TSS_PROJECT_EXTENSION key and accompanying TSS_PROJECT_KEYNAME key
   // Link extension key to KeyName key
   if( mRegistryObj.createKey( fileExtension ) 
    && mRegistryObj.createKey( associationKeyName ) 
    && mRegistryObj.setString( fileExtension, "", associationKeyName ) 
    && mRegistryObj.setString( associationKeyName, "", extensionDescription) )
   {               
      // Time for a bit of string dancing! [10/30/2006 justin]
      char szFullExePath[1024];

      dSprintf( szFullExePath, 1024, "%s/%s", Platform::getExecutablePath(), TSS_TOOL_BINARY_NAME );

      backslash( szFullExePath );

      char szFullMainCSPath[1024];
      dSprintf( szFullMainCSPath, 1024, "%s/%s", Platform::getExecutablePath(), "main.cs" );
      backslash( szFullMainCSPath );

      char szBuffer[1024];
      char szBuffer2[1024];

      // Shell Execute Command Setup
      dSprintf( szBuffer, 1024, "%s\\shell\\open\\command", associationKeyName );

      // Set Default Icon Key
      if( mRegistryObj.createKey( szBuffer ) )
      {
         // "fullpathto.exe", "fullpathtomain.cs" -project "%1"
         // We add the -project so that the tool will not try and use it as the tool main.cs 
         // and just pass it off to the script!
         dSprintf( szBuffer2, 1024, "\"%s\" \"%s\" -project \"%%1\"", szFullExePath, szFullMainCSPath );

         backslash( szBuffer2 );

         if( mRegistryObj.setString( szBuffer, "", szBuffer2 ) )
         {            
            // Default Icon
            dSprintf( szBuffer, 1024, "%s\\DefaultIcon", associationKeyName );

            // Set Default Icon Key
            if( mRegistryObj.createKey( szBuffer ) )
            {      
               dSprintf( szBuffer2, 1024, "\"%s\", %d", szFullExePath, nIconIndex );

               backslash( szBuffer2 );

               if( mRegistryObj.setString( szBuffer, "", szBuffer2 ) )
                  return true;
            }
         }
      }
   }
   
   // If we fall through we've failed so clean up...
   removeFileAssociations();

   return false;
}
//=============================================================================
//
// CONSOLE METHODS
//
//=============================================================================


//-----------------------------------------------------------------------------
ConsoleMethod( TGBRegistryManager, fileAssociationExists, bool, 2, 2, "%obj.fileAssociationExists()" )
{
   return object->fileAssociationExists();
}
//-----------------------------------------------------------------------------
ConsoleMethod( TGBRegistryManager, setFileAssociations, bool, 2, 2, "%obj.setFileAssociations()" )
{
   return object->setFileAssociations();
}
//-----------------------------------------------------------------------------
ConsoleMethod( TGBRegistryManager, removeFileAssociations, bool, 2, 2, "%obj.removeFileAssociations()" )
{
   return object->removeFileAssociations();
}
//-----------------------------------------------------------------------------
ConsoleMethod( TGBRegistryManager, executablePathsAreRegistered, bool, 2, 2, "%obj.executablePathIsRegistered()" )
{
   return object->executablePathsAreRegistered();
}
//-----------------------------------------------------------------------------
ConsoleMethod( TGBRegistryManager, registerExecutablePaths, bool, 2, 2, "%obj.registerExecutablePath()" )
{
   return object->registerExecutablePaths();
}
//-----------------------------------------------------------------------------
ConsoleMethod( TGBRegistryManager, unregisterExecutablePaths, bool, 2, 2, "%obj.unregisterExecutablePath()" )
{
   return object->unregisterExecutablePaths();
}

#endif // #ifndef TORQUE_PLAYER