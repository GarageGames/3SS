//
//  AppDelegate.m
//  iTorque2D
//
//  Created by QA Mac1 on 2/2/12.
//  Copyright (c) 2012 GarageGames. All rights reserved.
//

#import "AppDelegate.h"

#import "ViewController.h"

#include "platform/platformInput.h"
#include "platformiOS/iOSUtil.h"
#include "console/console.h"

extern int _iOSRunTorqueMain( id appID,  UIView *Window, UIApplication *app );
extern void _iOSGameInnerLoop();
extern void _iOSGameResignActive();
extern void _iOSGameBecomeActive();
extern void _iOSGameWillTerminate();

// Store current orientation for easy access
extern void _iOSGameChangeOrientation(S32 newOrientation);
UIDeviceOrientation currentOrientation;

bool _iOSTorqueFatalError = false;


@implementation AppDelegate

@synthesize window = _window;
@synthesize viewController = _viewController;

- (void)dealloc 
{
    [_window release];
    [_viewController release];
    [super dealloc];
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    self.window = [[[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]] autorelease];
    
    // Override point for customization after application launch.
    if ([[UIDevice currentDevice] userInterfaceIdiom] == UIUserInterfaceIdiomPhone)
    {
        self.viewController = [[[ViewController alloc] initWithNibName:@"ViewController_iPhone" bundle:nil] autorelease];
    } 
    else 
    {
        self.viewController = [[[ViewController alloc] initWithNibName:@"ViewController_iPad" bundle:nil] autorelease];
    }
    
    self.window.rootViewController = self.viewController;
    
    [self.window makeKeyAndVisible];
    
    [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
    
	// Also we set the currentRotation up so its not invalid
	currentOrientation = [UIDevice currentDevice].orientation;
    
	// So we make a selector to handle that, called didRotate (lower down in the code)
	[[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didRotate:)
                                                 name:UIDeviceOrientationDidChangeNotification
                                               object:nil];	
    _iOSTorqueFatalError = false;
    
	if(!_iOSRunTorqueMain( self, _window, application ))
	{
		_iOSTorqueFatalError = true;
        
		return NO;
	}
    
    return YES;
}

- (void)applicationWillResignActive:(UIApplication *)application
{
    _iOSGameResignActive();
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    /*
     Use this method to release shared resources, save user data, invalidate timers, and store enough application state information to restore your application to its current state in case it is terminated later. 
     If your application supports background execution, this method is called instead of applicationWillTerminate: when the user quits.
     */
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    /*
     Called as part of the transition from the background to the inactive state; here you can undo many of the changes made on entering the background.
     */
}

- (void)applicationDidBecomeActive:(UIApplication *)application
{
    if(!_iOSTorqueFatalError)
        _iOSGameBecomeActive();
}

- (void)applicationWillTerminate:(UIApplication *)application
{
    _iOSGameWillTerminate();
	
	[[UIDevice currentDevice] endGeneratingDeviceOrientationNotifications];
}

- (void)didRotate:(NSNotification *)notification
{	
	//Default to landscape left
	UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
	if(currentOrientation != orientation)
	{
		//Change the orientation
		currentOrientation = orientation;
		//Tell the rest of the engine
		_iOSGameChangeOrientation(currentOrientation);
	}
}

- (void) runMainLoop
{
	_iOSGameInnerLoop();
}

@end