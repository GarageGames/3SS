//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifdef TORQUE_ALLOW_LOCATION

//Call this first if you wanna use the location manager at all.
bool createLocationManager();
void destroyLocationManager();

//Fetch the heading of the compass, from the device. What this returns is a string in the format : 
// "x y z Magnitude", this is for the scripts but is exposed to code as well.
char* getiOSCompassHeading();

//Fetch the current location, using the desired accuracy. 
//Using both can be expensive, but might be more accurate. Default 
//samples use horizontal accuracy to find the location roughly, where
//you can decide whether to use both or not.

//For accuracy, specify one of the following : 

// 0 - kCLLocationAccuracyBest
// 1 - kCLLocationAccuracyNearestTenMeters
// 2 - kCLLocationAccuracyHundredMeters
// 3 - kCLLocationAccuracyKilometer
// 4 - kCLLocationAccuracyThreeKilometers

//For the timoutSeconds, any time in seconds to attempt to get the values

//Returns something like "lat long alt", or 0 where any of these dont apply
char* getiOSCurrentLocation(int accuracy, int timeoutSeconds, bool useHorizontalAcc = true, bool useVerticalAccuracy = false);

#endif //TORQUE_ALLOW_LOCATION