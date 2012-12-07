//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "winVideo.h"


bool WinDisplayDevice::smCanSwitchBitDepth = false;
bool WinDisplayDevice::smCanDo16Bit        = false;
bool WinDisplayDevice::smCanDo32Bit        = false;


WinDisplayDevice::WinDisplayDevice()
{
   mRestoreGamma      = false;
}

void WinDisplayDevice::enumerateBitDepths()
{
   // Enumerate all available resolutions:
   DEVMODE devMode;
   U32 modeNum = 0;
   U32 foundDisplayMode = true;

   while ( foundDisplayMode )
   {
      // Clear out our display mode settings structure.
      dMemset( &devMode, 0, sizeof( devMode ) );
      devMode.dmSize = sizeof( devMode );

      // Check for display mode settings
      foundDisplayMode = EnumDisplaySettings( NULL, modeNum++, &devMode );

	  //MIN_RESOLUTION defined in platformWin32/platformGL.h
      if ( devMode.dmPelsWidth >= MIN_RESOLUTION_X && devMode.dmPelsHeight >= MIN_RESOLUTION_Y && ( devMode.dmBitsPerPel == 16 || devMode.dmBitsPerPel == 32 ) )
      {
         // Note BPP for can switch bit depth.
         if( devMode.dmBitsPerPel == 16 )
            smCanDo16Bit = true;
         else if( devMode.dmBitsPerPel == 32 )
            smCanDo32Bit = true;
      }
   }

   // Set can switch bit depth based on found resolutions.
   if( smCanDo16Bit && smCanDo32Bit )
      smCanSwitchBitDepth = true;
   else
      smCanSwitchBitDepth = false;
}

void WinDisplayDevice::initDevice()
{
   // Set some initial conditions:
   mResolutionList.clear();

   // Enumerate all available resolutions:
   DEVMODE devMode;
   U32 modeNum = 0;
   U32 foundDisplayMode = true;

   while ( foundDisplayMode )
   {
      // Clear out our display mode settings structure.
      dMemset( &devMode, 0, sizeof( devMode ) );
      devMode.dmSize = sizeof( devMode );

      // Check for display mode settings
      foundDisplayMode = EnumDisplaySettings( NULL, modeNum++, &devMode );

      if ( devMode.dmPelsWidth >= MIN_RESOLUTION_X && devMode.dmPelsHeight >= MIN_RESOLUTION_Y && ( devMode.dmBitsPerPel == 16 || devMode.dmBitsPerPel == 32 ) )
      {
         // Only add this resolution if it is not already in the list:
         bool alreadyInList = false;
         for ( U32 i = 0; i < (U32)mResolutionList.size(); i++ )
         {
            if ( devMode.dmPelsWidth == mResolutionList[i].w
               && devMode.dmPelsHeight == mResolutionList[i].h
               && devMode.dmBitsPerPel == mResolutionList[i].bpp )
            {
               alreadyInList = true;
               break;
            }
         }

         // If we've not already added this resolution, add it to our resolution list.
         if ( !alreadyInList )
         {
            Resolution newRes( devMode.dmPelsWidth, devMode.dmPelsHeight, devMode.dmBitsPerPel );
            mResolutionList.push_back( newRes );
         }
      }
   }
}


U32 WinDisplayDevice::getOSVersion()
{
   // Initialize OSVERSIONINFO Structure
   OSVERSIONINFO OSVersionInfo;
   dMemset( &OSVersionInfo, 0, sizeof( OSVERSIONINFO ) );
   OSVersionInfo.dwOSVersionInfoSize = sizeof( OSVERSIONINFO );

   // Get OS Version
   if( !GetVersionEx( &OSVersionInfo ) )
      return OS_ERROR;
   else if ( OSVersionInfo.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS )
      return OS_9X;
   else if (OSVersionInfo.dwPlatformId == VER_PLATFORM_WIN32_NT && OSVersionInfo.dwMajorVersion >= 4 )
      return OS_NT;
   else
      return OS_UNKNOWN;

}