//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//These correspond to the docs over on Apples site : 
#ifdef TORQUE_ALLOW_MOVIEPLAYER

#import <MediaPlayer/MPMoviePlayerController.h>
#import <UIKit/UIDevice.h>
bool playiOSMovie(const char* fileName, const char* extension, MPMovieScalingMode scalingMode, MPMovieControlStyle controlStyle);
#endif//TORQUE_ALLOW_MOVIEPLAYER