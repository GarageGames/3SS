//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef _WINREGISTRY_H_
#define _WINREGISTRY_H_

#ifndef _WINDOWS_
   #include <windows.h>
#endif

#ifndef _WINREG_
   #include <winreg.h>
#endif

#ifndef _PLATFORM_H_
   #include "platform/platform.h"
#endif

/// Registered Windows Application Path Registry
///
/// We register TGB with windows paths registry so that we may
/// allow addons and external apps that need to know where TGB.exe is to either
/// a) simply reference our location by doing "TGB.exe yourArgs" if using TGB as 
///    a command line tool
/// b) Query the registry key below for the 'Path' value which will contains
///    TGB location on the system
///
/// HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\TGBGame.exe
class RegistryObject 
{
private:
   //typedef SimObject Parent;
   HKEY              m_hKey;
public:   
   RegistryObject();
   ~RegistryObject();

   /// Set Root for setting storage. HKLM,HKCU,etc
   void setRoot( HKEY hKeyRoot );
   /// See if a value exists in the settings hive i.e. "Config","MyValue"
   bool valueExists( const char* pszSubKey, const char* pszValue );
   /// Check to see if a key exists in the hive i.e. "Software//Microsoft//Windows//CurrentVersion"
   bool keyExists( const char* pszSubKey );
   /// Delete a setting
   bool deleteValue( const char* pszSubKey, const char* pszValue );
   /// Set a DWORD setting
   bool setDWORD(const char* pszSubKey, const char* pszValue, DWORD dwValue);
   /// Set a string setting
   bool setString(const char* pszSubKey, const char* pszValue, const char* pszString);
   /// Set an integer setting
   bool setInt(const char* pszSubKey, const char* pszValue, int iValue);
   /// Get a DWORD setting
   DWORD getDWORD(const char* pszSubKey, const char* pszValue);
   /// Get a string setting
   StringTableEntry getString(const char* pszSubKey, const char* pszValue);
   /// Get a string setting
   bool getString(const char* pszValue, char* pszBuffer, int nMax = 512);
   /// Get an integer setting
   int getInt(const char* pszSubKey, const char* pszValue);
   /// Create a Key in the current Root Hive
   bool createKey(const char* pszSubKey);
   /// Delete a Key in the current Root Hive
   bool deleteKey(const char* pszSubKey);
};

#endif