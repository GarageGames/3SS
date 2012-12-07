//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformWin32/platformWin32.h"
#include "time.h"

void Platform::sleep(U32 ms)
{
   Sleep(ms);
}

//--------------------------------------
void Platform::getLocalTime(LocalTime &lt)
{
   struct tm *systime;
   time_t long_time;

   time( &long_time );                // Get time as long integer.
   systime = localtime( &long_time ); // Convert to local time.

   lt.sec      = systime->tm_sec;
   lt.min      = systime->tm_min;
   lt.hour     = systime->tm_hour;
   lt.month    = systime->tm_mon;
   lt.monthday = systime->tm_mday;
   lt.weekday  = systime->tm_wday;
   lt.year     = systime->tm_year;
   lt.yearday  = systime->tm_yday;
   lt.isdst    = systime->tm_isdst;
}

U32 Platform::getTime()
{
   time_t long_time;
   time( &long_time );
   return (U32)long_time;
}

void Platform::fileToLocalTime(const FileTime & ft, LocalTime * lt)
{
   if(!lt)
      return;

   dMemset(lt, 0, sizeof(LocalTime));

   FILETIME winFileTime;
   winFileTime.dwLowDateTime = ft.v1;
   winFileTime.dwHighDateTime = ft.v2;

   SYSTEMTIME winSystemTime;

   // convert the filetime to local time
   FILETIME convertedFileTime;
   if(::FileTimeToLocalFileTime(&winFileTime, &convertedFileTime))
   {
      // get the time into system time struct
      if(::FileTimeToSystemTime((const FILETIME *)&convertedFileTime, &winSystemTime))
      {
         SYSTEMTIME * time = &winSystemTime;

         // fill it in...
         lt->sec      = (U8)time->wSecond;
         lt->min      = (U8)time->wMinute;
         lt->hour     = (U8)time->wHour;
         lt->month    = (U8)time->wMonth;
         lt->monthday = (U8)time->wDay;
         lt->weekday  = (U8)time->wDayOfWeek;
         lt->year     = (U8)((time->wYear < 1900) ? 1900 : (time->wYear - 1900));

         // not calculated
         lt->yearday = 0;
         lt->isdst = false;
      }
   }
}

U32 Platform::getRealMilliseconds()
{
   return GetTickCount();
}

U32 Platform::getVirtualMilliseconds()
{
   return winState.currentTime;
}

void Platform::advanceTime(U32 delta)
{
   winState.currentTime += delta;
}

