//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformiOS/platformiOS.h"
#include "platformiOS/iOSUtil.h"
#include "platform/platformAL.h"
#import <AudioToolbox/AudioToolbox.h>  

ConsoleFunction(doDeviceVibrate, void, 1, 1, "Makes the device do a quick vibration. Only works on the iPhone line of devices - the iPod Touch line does not have vibration functionality.")  
{  
	if(IsDeviceiPhone())  
	{
		AudioServicesPlaySystemSound (kSystemSoundID_Vibrate);  
	}
}  

namespace Audio
{
	
	/*!  The MacOS X build links against the OpenAL framework.
     It can be built to use either an internal framework, or the system framework.
     Since OpenAL is weak-linked in at compile time, we don't need to init anything.
     Stub it out...
	 */
	bool OpenALDLLInit() {  return true; }
	
	/*!   Stubbed out, see the note on OpenALDLLInit().  
	 */
	void OpenALDLLShutdown() { }   
	
	
} // namespace Audio



// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
typedef ALvoid	AL_APIENTRY	(*alcMacOSXMixerOutputRateProcPtr) (const ALdouble value);
ALvoid  alcMacOSXMixerOutputRateProc(const ALdouble value)
{
	static	alcMacOSXMixerOutputRateProcPtr	proc = NULL;
    
    if (proc == NULL) {
        proc = (alcMacOSXMixerOutputRateProcPtr) alcGetProcAddress(NULL, (const ALCchar*) "alcMacOSXMixerOutputRate");
    }
    
    if (proc)
        proc(value);
	
    return;
}