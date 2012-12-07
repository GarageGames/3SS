//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformMacCarb/platformMacCarb.h"
#include "console/console.h"
#include "string/stringTable.h"
#include <math.h>

// The Gestalt() API will no longer be updated, so this code should migrate to
// the unix sysctl() and sysctlbyname() API. This is the current doctrine from Apple, dec 2005.

#define BASE_MHZ_SPEED      0

void Processor::init()
{
   Con::printSeparator();
   Con::printf("CPU initialization:");
   OSErr err;
   long raw, mhzSpeed = BASE_MHZ_SPEED;

   err = Gestalt(gestaltSystemVersion, &raw);
   Con::printf("   MacOS version: %x.%x.%x", (raw>>8), (raw&0xFF)>>4, (raw&0x0F));

   err = Gestalt(gestaltCarbonVersion, &raw);
   if (err)
      Con::printf("   No CarbonLib support.");
   else
      Con::printf("   CarbonLib version: %x.%x.%x", (raw>>8), (raw&0xFF)>>4, (raw&0x0F));
   
   Gestalt(gestaltPhysicalRAMSize, &raw);
   raw /= (1024*1024); // MB
   Con::printf("   Physical RAM: %dMB", raw);
   
   Gestalt(gestaltLogicalRAMSize, &raw);
   raw /= (1024*1024); // MB
   Con::printf("   Logical RAM: %dMB", raw);

   // this is known to have issues with some Processor Upgrade cards
   err = Gestalt(gestaltProcClkSpeed, &raw);
   if (err==noErr && raw)
   {
      mhzSpeed = (float)raw / 1000000.0f;
      if (mhzSpeed < BASE_MHZ_SPEED)
      { // something drastically wrong.
         mhzSpeed = BASE_MHZ_SPEED;
      }
   }
   PlatformSystemInfo.processor.mhz = mhzSpeed;

   PlatformSystemInfo.processor.type = CPU_PowerPC_Unknown;
   err = Gestalt(gestaltNativeCPUtype, &raw);
   switch(raw)
   {
      case gestaltCPU601:
         PlatformSystemInfo.processor.type = CPU_PowerPC_601;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC 601");
         break;
      case gestaltCPU603e:
      case gestaltCPU603ev:
         PlatformSystemInfo.processor.type = CPU_PowerPC_603e;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC 603e");
         break;
      case gestaltCPU603:
         PlatformSystemInfo.processor.type = CPU_PowerPC_603;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC 603");
         break;
      case gestaltCPU604e:
      case gestaltCPU604ev:
         PlatformSystemInfo.processor.type = CPU_PowerPC_604e;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC 604e");
         break;
      case gestaltCPU604:
         PlatformSystemInfo.processor.type = CPU_PowerPC_604;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC 604");
         break;
      case gestaltCPU750:     //G3
      case gestaltCPU750FX:   // from apple headers: "Sahara,G3 like thing"
         PlatformSystemInfo.processor.type = CPU_PowerPC_G3;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC G3");
         break;
      case gestaltCPUG4:
         PlatformSystemInfo.processor.type = CPU_PowerPC_G4;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC G4");
         break;
      case gestaltCPUG47450:  // Advanced late model G4s, often seen in laptops
         PlatformSystemInfo.processor.type = CPU_PowerPC_G4_7455;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC G4 7450");
         break;
      case gestaltCPUApollo:
         PlatformSystemInfo.processor.type = CPU_PowerPC_G4_7455;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC G4 7455");
         break;
      case gestaltCPUG47447:
         PlatformSystemInfo.processor.type = CPU_PowerPC_G4_7447;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC G4 7447");
         break;
      case gestaltCPU970:     // G5 flavors
      case gestaltCPU970FX:
         PlatformSystemInfo.processor.type = CPU_PowerPC_G5;
         PlatformSystemInfo.processor.name = StringTable->insert("PowerPC G5");
         break;
      case gestaltCPUX86:
         PlatformSystemInfo.processor.type = CPU_X86Compatible;
         PlatformSystemInfo.processor.name = StringTable->insert("x86 Compatible");
         break;
      case gestaltCPUPentium4:
         PlatformSystemInfo.processor.type = CPU_Intel_Pentium4;
         PlatformSystemInfo.processor.name = StringTable->insert("Intel Pentium 4");
         break;
      default:
         // explain why we can't get the processor type.
         Con::warnf("Unknown Getstalt value for processor type: 0x%x",raw);
         Con::warnf("platform layer should be changed to use sysctl() instead of Gestalt() .");
         // for now, identify it as an x86 processor, because Apple is moving to Intel chips...
         PlatformSystemInfo.processor.type = CPU_X86Compatible;
         PlatformSystemInfo.processor.name = StringTable->insert("Unknown Processor, assuming x86 Compatible");
         break;
   }

   PlatformSystemInfo.processor.properties = CPU_PROP_PPCMIN;
   err = Gestalt(gestaltPowerPCProcessorFeatures, &raw);
#if defined(__VEC__)
   if ((1 << gestaltPowerPCHasVectorInstructions) & (raw)) {
      PlatformSystemInfo.processor.properties |= CPU_PROP_ALTIVEC; // OR it in as they are flags...
   }
#endif
   Con::printf("   %s, %d Mhz", PlatformSystemInfo.processor.name, PlatformSystemInfo.processor.mhz);
   if (PlatformSystemInfo.processor.properties & CPU_PROP_PPCMIN)
      Con::printf("   FPU detected");
   if (PlatformSystemInfo.processor.properties & CPU_PROP_ALTIVEC)
      Con::printf("   AltiVec detected");

   Con::printf(" ");
}
