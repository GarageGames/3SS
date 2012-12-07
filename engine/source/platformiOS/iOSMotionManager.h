//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLEINTERNAL_H_
#include "console/consoleInternal.h"
#endif

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>
#import <CoreMotion/CoreMotion.h>

@interface iOSMotionManager: NSObject
{
    // The sole CMMotionManager reference
    CMMotionManager* motionManager;
    
    // The starting attitude reference DeviceMotion will use
    CMAttitude* referenceAttitude;
    
    bool accelerometerEnabled;
    bool gyroscopeEnabled;
}

@property (readwrite, assign) bool accelerometerEnabled;
@property (readwrite, assign) bool gyroscopeEnabled;

@property (retain) CMAttitude* referenceAttitude;

// Accelerometer related functions
- (void) enableAccelerometer;
- (void) disableAccelerometer;
- (bool) isAccelerometerActive;

// Gyroscope related functions
- (bool) enableGyroscope;
- (bool) disableGyroscope;
- (bool) isGyroAvailable;
- (bool) isGyroActive;

// Motion device related functions
- (bool) startDeviceMotion;
- (bool) stopDeviceMotion;
- (bool) resetDeviceMotionReference;
- (bool) isDeviceMotionAvailable;
- (bool) isDeviceMotionActive;

@end

static iOSMotionManager* gMotionManager;