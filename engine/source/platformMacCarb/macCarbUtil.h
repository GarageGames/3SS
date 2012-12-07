//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _MACCARBUTIL_H_
#define _MACCARBUTIL_H_

#include <ApplicationServices/ApplicationServices.h>
#include <CoreFoundation/CoreFoundation.h>

/// Looks in the app's Frameworks folder for a framework and loads it if it finds it.
bool LoadFrameworkBundle(CFStringRef framework, CFBundleRef *bundlePtr);
/// Looks for a framework first in the app then the system, and loads it if it finds it.
bool LoadPrivateFrameworkBundle(CFStringRef framework, CFBundleRef *bundlePtr);

// DWB: 8-2-11: After porting some device stuff to support OSX 10.7, this fcn not needed anymore
/// Converts a QuickDraw displayID to a Core Graphics displayID.
/// Different Mac APIs need different displayID types. The conversion is trivial
/// on 10.3+, but ugly on 10.2, so we wrap it here.
//CGDirectDisplayID MacCarbGetCGDisplayFromQDDisplay(CGDirectDisplayID hDisplay);

CGPoint MacCarbTorqueToNativeCoords(int x, int y);

#endif // _MACCARBUTIL_H_
