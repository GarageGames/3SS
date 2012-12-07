//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformiOS/platformiOS.h"
#include "console/console.h"
#include "string/stringTable.h"
#include <math.h>




void Processor::init()
{
	
   Con::printf("System & Processor Information:");

   Con::printf("   iOS version: %0.0f", platState.osVersion );

	// -Mat FIXME: USE SYSTEM FUNCTION to get version number for 
	//just use OS version for now
   Con::printf("   CarbonLib version: %0.0f", platState.osVersion );
   
   Con::printf("   Physical RAM: %dMB", 128);

   Con::printf("   Logical RAM: %dMB", 128);

   PlatformSystemInfo.processor.mhz = 412;

   //PlatformSystemInfo.processor.type =  ARM_1176;
   PlatformSystemInfo.processor.name = StringTable->insert("ARM 1176");

   PlatformSystemInfo.processor.properties = CPU_PROP_PPCMIN;

	Con::printf("   %s, %d Mhz", PlatformSystemInfo.processor.name, PlatformSystemInfo.processor.mhz);
   if (PlatformSystemInfo.processor.properties & CPU_PROP_PPCMIN)
      Con::printf("   FPU detected");
   if (PlatformSystemInfo.processor.properties & CPU_PROP_ALTIVEC)
      Con::printf("   AltiVec detected");

   Con::printf(" ");
}

