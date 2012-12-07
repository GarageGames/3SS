//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#include "platform/platform.h"
#include <windows.h>
#include <winreg.h>
#include <shlwapi.h>
#include "memory/frameAllocator.h"
#include "string/stringTable.h"
#include "string/unicode.h"
#include "winRegistryAccess.h"

RegistryObject::RegistryObject()
{
}

RegistryObject::~RegistryObject()
{
}

void RegistryObject::setRoot( HKEY hKeyRoot )
{
   m_hKey = hKeyRoot;
}

bool RegistryObject::valueExists( const char* pszSubKey, const char* pszValue )
{
   HKEY  hKey;
   BYTE pBuffer[512];
   DWORD dwType;
   DWORD dwSize = 512;
   LONG  lRes   = 0;

#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];
   UTF16 pszSafeValue[MAX_PATH];
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
   convertUTF8toUTF16(pszValue, pszSafeValue, sizeof(pszSafeValue));
#else
   UTF8 pszSafeSubKey[MAX_PATH];
   UTF8 pszSafeValue[MAX_PATH];
   dStrcpy( pszSafeSubKey, pszSubKey );
   dStrcpy( pszSafeValue, pszValue );
#endif

   ZeroMemory(pBuffer,512);

   lRes = ::RegOpenKeyEx(m_hKey, pszSafeSubKey, 0, KEY_READ, &hKey);

   if( lRes != ERROR_SUCCESS )
   {
      SetLastError((DWORD)lRes);
      return false;
   }

   lRes = ::RegQueryValueEx(hKey, pszSafeValue, 0, &dwType, (LPBYTE)&pBuffer, &dwSize);

   RegCloseKey(hKey);

   if(lRes != ERROR_SUCCESS )
      return false;

   return true;
}
bool RegistryObject::keyExists( const char* pszSubKey )
{
   HKEY  hKey;
   LONG  lRes   = 0;

#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];
   //UTF16 pszSafeValue[MAX_PATH];
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
#else
   UTF8 pszSafeSubKey[MAX_PATH];
   dStrcpy( pszSafeSubKey, pszSubKey );
#endif

   lRes = ::RegOpenKeyEx(m_hKey, pszSafeSubKey, 0, KEY_READ, &hKey);

   if( lRes != ERROR_SUCCESS) 
      return false;

   return true;
}

bool RegistryObject::deleteValue( const char* pszSubKey, const char* pszValue )
{
   HKEY hKey;
   LONG lRes;
#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];
   UTF16 pszSafeValue[MAX_PATH];
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
   convertUTF8toUTF16(pszValue, pszSafeValue, sizeof(pszSafeValue));
#else
   UTF8 pszSafeSubKey[MAX_PATH];
   UTF8 pszSafeValue[MAX_PATH];
   dStrcpy( pszSafeSubKey, pszSubKey );
   dStrcpy( pszSafeValue, pszValue );
#endif

   lRes = ::RegOpenKeyEx( m_hKey, pszSafeSubKey, 0, KEY_SET_VALUE, &hKey);

   if( lRes != ERROR_SUCCESS )
   {
      SetLastError((DWORD)lRes);
      return false;
   }

   lRes = ::RegDeleteValue(hKey, pszSafeValue);

   RegCloseKey(hKey);

   if( lRes != ERROR_SUCCESS )
      return false;

   return true;

}
bool RegistryObject::setDWORD(const char* pszSubKey, const char* pszValue, DWORD dwValue)
{
   HKEY	hKey;
   LONG	lRes;
#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];
   UTF16 pszSafeValue[MAX_PATH];
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
   convertUTF8toUTF16(pszValue, pszSafeValue, sizeof(pszSafeValue));
#else
   UTF8 pszSafeSubKey[MAX_PATH];
   UTF8 pszSafeValue[MAX_PATH];
   dStrcpy( pszSafeSubKey, pszSubKey );
   dStrcpy( pszSafeValue, pszValue );
#endif

   createKey( pszSubKey );

   lRes = ::RegOpenKeyEx(m_hKey, pszSafeSubKey, 0, KEY_WRITE, &hKey);

   if( lRes != ERROR_SUCCESS )
      return false;

   lRes = ::RegSetValueEx(hKey, pszSafeValue,0,REG_DWORD,reinterpret_cast<BYTE*>(&dwValue),sizeof(DWORD));

   ::RegCloseKey(hKey);

   if( lRes != ERROR_SUCCESS )
      return false;

   return true;
}


bool RegistryObject::setString(const char* pszSubKey, const char* pszValue, const char* pszString)
{
   HKEY	hKey;
   LONG	lRes;
#ifdef UNICODE
   DWORD	dwSize = dStrlen(pszString) * sizeof(UTF16);
   
   UTF16 pszSafeSubKey[1024];
   UTF16 pszSafeValue[1024];
   UTF16 pszSafeString[1024];
   
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
   convertUTF8toUTF16(pszValue, pszSafeValue, sizeof(pszSafeValue));
   convertUTF8toUTF16(pszString, pszSafeString, sizeof(pszSafeString));
#else
   DWORD	dwSize = dStrlen(pszString) * sizeof(UTF8);
   
   UTF8 pszSafeSubKey[1024];
   UTF8 pszSafeValue[1024];
   UTF8 pszSafeString[1024];
   
   dStrcpy( pszSafeSubKey, pszSubKey );
   dStrcpy( pszSafeValue, pszValue );
   dStrcpy( pszSafeString, pszString );
#endif

   createKey( pszSubKey );

   lRes = ::RegOpenKeyEx(m_hKey, pszSafeSubKey, 0, KEY_WRITE, &hKey);

   if( lRes != ERROR_SUCCESS) 
      return false;

   lRes = ::RegSetValueEx(hKey, pszSafeValue, 0, REG_SZ, reinterpret_cast<const BYTE*>(pszSafeString), dwSize);

   ::RegCloseKey(hKey);

   if( lRes != ERROR_SUCCESS )
      return false;

   return true;
}
StringTableEntry RegistryObject::getString(const char* pszSubKey, const char* pszValue)
{

   HKEY	hKey;
   LONG	lRes;
#ifdef UNICODE
   DWORD	dwSize = 1024 * sizeof(UTF16);
   UTF16 pszSafeSubKey[1024];
   UTF16 pszSafeValue[1024];
   UTF16 pszSafeString[1024];
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
   convertUTF8toUTF16(pszValue, pszSafeValue, sizeof(pszSafeValue));
#else
   DWORD	dwSize = 1024 * sizeof(UTF8);
   UTF8 pszSafeSubKey[1024];
   UTF8 pszSafeValue[1024];
   UTF8 pszSafeString[1024];
   dStrcpy( pszSafeSubKey, pszSubKey );
   dStrcpy( pszSafeValue, pszValue );
#endif

   lRes = ::RegOpenKeyEx(m_hKey, pszSafeSubKey, 0, KEY_READ, &hKey);

   if( lRes != ERROR_SUCCESS )
      return StringTable->EmptyString;

   lRes = ::RegQueryValueEx(hKey, pszSafeValue, NULL, NULL, (BYTE*)pszSafeString, &dwSize);

   ::RegCloseKey(hKey);

   if( lRes != ERROR_SUCCESS )
      return StringTable->EmptyString;

   char pszResultString[1024];
#ifdef UNICODE
   convertUTF16toUTF8(pszSafeString, pszResultString, sizeof(pszResultString));
#else
   dStrcpy( pszResultString, pszSafeString );
#endif

   return StringTable->insert(pszResultString);

}
bool RegistryObject::setInt(const char* pszSubKey, const char* pszValue, int iValue)
{
   return setDWORD(pszSubKey,pszValue,static_cast<DWORD>(iValue));
}

DWORD RegistryObject::getDWORD(const char* pszSubKey, const char* pszValue)
{
   HKEY	hKey;
   DWORD	dwType  = REG_DWORD;
   DWORD	dwSize  = sizeof(DWORD);
   DWORD	dwValue = 0;
   LONG	lRes;
#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];
   UTF16 pszSafeValue[MAX_PATH];
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
   convertUTF8toUTF16(pszValue, pszSafeValue, sizeof(pszSafeValue));
#else
   UTF8 pszSafeSubKey[MAX_PATH];
   UTF8 pszSafeValue[MAX_PATH];
   dStrcpy( pszSafeSubKey, pszSubKey );
   dStrcpy( pszSafeValue, pszValue );
#endif

   lRes = ::RegOpenKeyEx(m_hKey, pszSafeSubKey, 0, KEY_READ, &hKey);

   if( lRes != ERROR_SUCCESS )
      return static_cast<DWORD>(-1);

   lRes = ::RegQueryValueEx(hKey, pszSafeValue, 0, &dwType, (LPBYTE)&dwValue, &dwSize);

   ::RegCloseKey(hKey);

   if(lRes!=ERROR_SUCCESS)
      return static_cast<DWORD>(-1);

   return dwValue;
}
int RegistryObject::getInt(const char* pszSubKey, const char* pszValue)
{
   return static_cast<int>(getDWORD(pszSubKey,pszValue));
}
bool RegistryObject::createKey(const char* pszSubKey)
{
   HKEY hKey;
   DWORD dwFunc;
   LONG  lRet;

#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];

   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));

   lRet = ::RegCreateKeyEx( m_hKey,	
                            pszSafeSubKey,	
                            0, 
                            (UTF16*)NULL, 
                            REG_OPTION_NON_VOLATILE, 
                            KEY_WRITE, 
                           (LPSECURITY_ATTRIBUTES)NULL, &hKey, &dwFunc);
#else
   UTF8 pszSafeSubKey[MAX_PATH];

   dStrcpy( pszSafeSubKey, pszSubKey );

   lRet = ::RegCreateKeyEx(  m_hKey, 
                             pszSafeSubKey, 
                             0, 
                             (UTF8*)NULL, 
                             REG_OPTION_NON_VOLATILE, 
                             KEY_WRITE, 
                             (LPSECURITY_ATTRIBUTES)NULL, &hKey, &dwFunc);
#endif

   if( lRet == ERROR_SUCCESS )
   {
      ::RegCloseKey(hKey);

      hKey = (HKEY)NULL;

      return true;
   }

   ::SetLastError((DWORD)lRet);

   return false;
}
 
bool RegistryObject::deleteKey( const char* pszSubKey )
{
   LONG  lRet;
#ifdef UNICODE
   UTF16 pszSafeSubKey[MAX_PATH];
 
   convertUTF8toUTF16(pszSubKey, pszSafeSubKey, sizeof(pszSafeSubKey));
#else
   UTF8 pszSafeSubKey[MAX_PATH];

   dStrcpy( pszSafeSubKey, pszSubKey );
#endif

   lRet = ::SHDeleteKey( m_hKey, pszSafeSubKey );

   if( lRet == ERROR_SUCCESS )
      return true;

   SetLastError((DWORD)lRet);

   return false;

}