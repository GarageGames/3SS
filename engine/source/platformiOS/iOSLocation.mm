//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifdef TORQUE_ALLOW_LOCATION

#import "iOSLocation.h"

#import <CoreLocation/CoreLocation.h>

//Torque Stuff
#include "console/console.h"

bool createLocationManager();
void destroyLocationManager();

@interface iOSLocation : NSObject <CLLocationManagerDelegate> {
	
	CLLocationManager *locationMan;
	BOOL compassUpdating;
	BOOL locationUpdating;
	BOOL hasCompass;
	BOOL hasLocation;
	
}

@property (nonatomic, retain) CLLocationManager *locationMan;
@property (nonatomic) BOOL compassUpdating;
@property (nonatomic) BOOL locationUpdating;
@property (nonatomic) BOOL hasCompass;
@property (nonatomic) BOOL hasLocation;

//Returns false if the device is lacking support, or something is wrong.
-(BOOL) findCompass;
-(BOOL) findLocation;

//For toggling the fetching of compass data.
-(BOOL) startUpdatingCompass;
-(void) stopUpdatingCompass;

//For toggling the fetching of location data (convenience, use getiOSLocation rather)
-(BOOL) startUpdatingLocation; 
-(void) stopUpdatingLocation;

@end

static iOSLocation* iT2dLocationManager;
static bool iOSLocationManagerIsActive = false;

bool createLocationManager()
{
	iT2dLocationManager = [[[iOSLocation alloc] init] retain]; 
	iT2dLocationManager.locationMan = [[[CLLocationManager alloc] init] autorelease];
	iT2dLocationManager.locationMan.delegate = iT2dLocationManager;
	
	iT2dLocationManager.hasCompass = true;
	iT2dLocationManager.hasLocation = true;
	
	if([iT2dLocationManager findCompass] == NO)
	{
		Con::printf("Location Manager :: Cannot find compass support");
		iT2dLocationManager.hasCompass = false;
	}
	
	if([iT2dLocationManager findLocation] == NO)
	{
		Con::printf("Location Manager :: Cannot find location support, might be disabled.");
		iT2dLocationManager.hasLocation = false;
	}
	
	if(!iT2dLocationManager.hasCompass && !iT2dLocationManager.hasLocation)
	{
		//Let GC take this one
		destroyLocationManager();
		
		Con::printf("Location Manager :: No Location services available, no location manager will be created.");
		
		return false;
	}
	
    iOSLocationManagerIsActive = true;
	return true;
}

void destroyLocationManager()
{
    iT2dLocationManager.locationMan = nil;
    [iT2dLocationManager release];
    iT2dLocationManager = nil;
    
    iOSLocationManagerIsActive = false;
}

@implementation iOSLocation

@synthesize locationMan;
@synthesize compassUpdating;
@synthesize locationUpdating;
@synthesize hasCompass;
@synthesize hasLocation;

-(BOOL) findCompass
{
	if (locationMan.headingAvailable == NO) 
	{
		
		//No compass support
		return false;
	}
	else 
	{
		//Set up parameters
		locationMan.headingFilter = kCLHeadingFilterNone;
		return true;
	}

}

-(BOOL) findLocation
{
	if(locationMan.locationServicesEnabled)
	{
		//We can use locations
		return true;
	}
	else 
	{
		//These are disabled
		return false;
	}

}

//Dont want it to be polling and running all the time.

//Start the updates on the compass values
-(BOOL) startUpdatingCompass 
{
	if(locationMan != nil && hasCompass)
	{
		[locationMan startUpdatingHeading];
		return YES;
	}
	else
	{
		return NO;
	}
}

//Stop the updates on the compass values.
-(void) stopUpdatingCompass
{
	if(locationMan != nil && hasCompass)
		[locationMan stopUpdatingHeading];

}

//Start the location polling updates..
-(BOOL) startUpdatingLocation
{
	if(locationMan != nil && hasLocation)
	{
	 	[locationMan startUpdatingLocation];
		return YES;
	}
	else 
	{
		return NO;
	}

}

//Start the location polling updates.
-(void) stopUpdatingLocation
{
	if(locationMan != nil && hasLocation)
		[locationMan stopUpdatingLocation];
}

//Delegate methods for the Location manager and the compass
// This delegate method is invoked when the location manager has heading data.
- (void)locationManager:(CLLocationManager *)manager didUpdateHeading:(CLHeading *)heading {
    

	//From the apple example , Teslameter
    // Compute and display the magnitude (size or strength) of the vector.
	//      magnitude = sqrt(x^2 + y^2 + z^2)
	CGFloat magnitude = sqrt(heading.x*heading.x + heading.y*heading.y + heading.z*heading.z);

    //Format (separated by spaces in the string):
    // raw heading x
    // raw heading y
    // raw heading z
    // raw heading magnitude
    // magnetic heading
    // true heading
    // heading accuracy
	NSString* output = [NSString stringWithFormat:@"%.1f %.1f %.1f %.1f %.5f %.5f %.5f", heading.x, heading.y, heading.z, magnitude, heading.magneticHeading, heading.trueHeading, heading.headingAccuracy];
    
	//Hand it off to the compass variable in torque.
	const char* tOutput = [output UTF8String];
	
	Con::setVariable("$iOSCompassHeading", tOutput );
}

- (void)locationManager:(CLLocationManager *)manager didUpdateToLocation:(CLLocation *)location fromLocation:(CLLocation* )oldLocation {
    
    //Format (separated by spaces in the string):
    // latitude
    // longitude
    // altitude
    // horizontal accuracy
    // vertical accuracy
    // speed
    // course
	NSString* output = [NSString stringWithFormat:@"%.5f %.5f %.5f %.5f %.5f %.5f %.5f", location.coordinate.latitude, location.coordinate.longitude, location.altitude, location.horizontalAccuracy, location.verticalAccuracy, location.speed, location.course];
	
	//Hand it off to the compass variable in torque.
	const char* tOutput = [output UTF8String];
	
	Con::setVariable("$iOSLocationLocation", tOutput );
}

// This delegate method is invoked when the location managed encounters an error condition.
- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error {
    if ([error code] == kCLErrorDenied) {
        // This error indicates that the user has denied the application's request to use location services.
		Con::printf("Location Manager was denied access to use the user location.");
		Con::executef(1, "oniOSLocationDenied");
		//Disable any features that were trying to be run
		if(self.compassUpdating)
			[manager stopUpdatingHeading];
		if(self.locationUpdating)
			[manager stopUpdatingLocation];
		
    } else if ([error code] == kCLErrorHeadingFailure) {
        // This error indicates that the heading could not be determined, most likely because of strong magnetic interference.
		Con::printf("Compass had a heading failure. This could be a result of strong magnetic interference.");
    }
}

@end

ConsoleFunction(iOSCreateLocationManager, bool, 1, 1, "Call this to initialise and create the location manager.")
{
	return createLocationManager();
}

ConsoleFunction(iOSDestroyLocationManager, void, 1, 1, "Call this to clean up the location manager.")
{
	destroyLocationManager();
}

//Torque script accessible
ConsoleFunction(iOSStartCompass, bool, 1, 1, "iOSStartCompass() Returns True if the compass can be used, false otherwise")
{
	//This will tell the compass to start polling the data.
	//Returns false if its unable to (ie , wrong device type)
	if([iT2dLocationManager startUpdatingCompass] == NO)
	{
		Con::printf("Location Manager cannot start the compass. Does your device support it?");
		return false;
	}
	else
	{
		Con::printf("Location Manager :: Compass has been activated. Use $iOSCompassHeading to get the data.");
		return true;
	}

}

ConsoleFunction(iOSStopCompass, void, 1, 1, "iOSStopCompass() Stops the compass polling")
{
	//This will tell the compass to start polling the data.
	[iT2dLocationManager stopUpdatingCompass];
}

ConsoleFunction(iOSStartUpdatingLocation, bool, 1, 1, "iOSStartUpdatingLocation()")
{
    int accuracyParam = Con::getIntVariable("$iOSLocationAccuracy");

    switch(accuracyParam)
    {
        case(0):
        {
            iT2dLocationManager.locationMan.desiredAccuracy = kCLLocationAccuracyBest;
            break;
        }
        case(1):
        {
            iT2dLocationManager.locationMan.desiredAccuracy = kCLLocationAccuracyNearestTenMeters;
            break;
        }
        case(2):
        {
            iT2dLocationManager.locationMan.desiredAccuracy = kCLLocationAccuracyHundredMeters;
            break;
        }
        case(3):
        {
            iT2dLocationManager.locationMan.desiredAccuracy = kCLLocationAccuracyKilometer;
            break;
        }
        case(4):
        {
            iT2dLocationManager.locationMan.desiredAccuracy = kCLLocationAccuracyThreeKilometers;
            break;
        }
        default:
        {
            iT2dLocationManager.locationMan.desiredAccuracy = kCLLocationAccuracyThreeKilometers;
        }
    }
    
    
    if([iT2dLocationManager startUpdatingLocation] == NO)
	{
		Con::printf("Location Manager cannot start updating its location. Does your device support it?");
		return false;
	}
	else
	{
		Con::printf("Location Manager :: Location updating has been activated. Use $iOSLocation to get the data.");
		return true;
	}
}

ConsoleFunction(iOSStopUpdatingLocation, void, 1, 1, "iOSStopUpdatingLocation()")
{
    [iT2dLocationManager stopUpdatingLocation];
}

/*
ConsoleFunction(getiOSCompassHeading, const char*, 1, 1, "getiOSCompassHeading() Gets the current compass heading of the iOS")
{
    char* result = Con::getReturnBuffer();
    return result;
}

ConsoleFunction(getiOSCurrentLocation, const char*, 5, 5, "getiOSCurrentLocation( accuracy, timoutInSeconds, useHorizontalAccuracy, useVerticalAcuracy ) Gets the current GPS location of the iOS")
{
    //char* getiOSCurrentLocation(int accuracy, int timeoutSeconds, bool useHorizontalAcc = true, bool useVerticalAccuracy = false);
    char* result = Con::getReturnBuffer();
    return result;
}
*/

#endif //TORQUE_ALLOW_LOCATION