//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _iOSUTIL_H_
#define _iOSUTIL_H_


#include "network/tcpObject.h"



//Luma:	Orientation support
/*enum iOSOrientation
{
	iOSOrientationUnkown,				//All applications start in this state
	iOSOrientationLandscapeLeft,			//The home button is on the RIGHT
	iOSOrientationLandscapeRight,		//The home button is on the LEFT
	iOSOrientationPortrait,				//The home button is on the bottom
	iOSOrientationPortraitUpsideDown		//The home button is on the top
};*/

int _iOSGameGetCurrentOrientation();	
void _iOSGameSetCurrentOrientation(int iOrientation);	


//Luma: Ability to get the Local IP (Internal IP) for an iOS as opposed to it's External one
void _iOSGetLocalIP(unsigned char *pcIPString);

//Luma: Make sure that the iOS Radio is on before connection via TCP... NOTE: sometimes the Radio wont be ready for immediate use after this is processed... need to see why
void OpeniOSNetworkingAndConnectToTCPObject(TCPObject *psTCPObject, const char *pcAddress);

//Luma: ability to quickly tell if this is an iOS or an iTouch
bool IsDeviceiPhone(void);


#endif // _iOSUTIL_H_
