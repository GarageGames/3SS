//
//  main.m
//  CleanView
//
//  Created by QA Mac1 on 1/25/12.
//  Copyright (c) 2012 GarageGames. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "AppDelegate.h"

#include "platformiOS/platformiOS.h"
#include "platformiOS/iOSEvents.h"
#include "platformiOS/iOSUtil.h"
#include "platform/threads/thread.h"
#include "game/gameInterface.h"
#include "io/fileio.h"

extern void clearPendingMultitouchEvents( void );

static BOOL CAdisplayLinkSupported = NO;
id displayLink = nil;

S32 gLastStart = 0;

bool appIsRunning = true;

int _iOSRunTorqueMain( id appID, UIView *Window, UIApplication *app )
{
	platState.appID = appID;	
	platState.firstThreadId = ThreadManager::getCurrentThreadId();
	platState.Window = Window;
	platState.application = app;
	
	// Hidden by default.
	platState.application.statusBarHidden = YES;
	
#if !defined(TORQUE_MULTITHREAD)
    
	printf("performing mainInit()\n");
    
	platState.lastTimeTick = Platform::getRealMilliseconds();
    
	if(!Game->mainInit(platState.argc, platState.argv))
	{
		return 0;
	}
	
	// CADisplayLink main loop support
	// Currently needs to move to app delegate or platstate.
	NSString *reqSysVer = @"3.1";
    
	NSString *currSysVer = [[UIDevice currentDevice] systemVersion];
    
	if ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending)
		CAdisplayLinkSupported = YES;
    
	// start the game going
    if(!CAdisplayLinkSupported)
    {
        iOSRunEventLoopTimer(sgTimeManagerProcessInterval);
    }
    else
    {
        /* 
         We can use the CADisplayLink to update the game now. The magic number 1 below is because of what the docs say.
         
		 The default value is 1, which results in your application being notified at the refresh rate of the display.
		 If the value is set to a value larger than 1, the display link notifies your application at a fraction of the
		 native refresh rate. For example, setting the interval to 2 causes the display link to fire every other frame, 
		 providing half the frame rate.
         
		 Setting this value to less than 1 results in undefined behavior and is a programmer error.
		 */
        
        NSInteger updateDelay = 1;
		
		displayLink = [NSClassFromString(@"CADisplayLink") displayLinkWithTarget:appID selector:@selector(runMainLoop)];
        
		[displayLink setFrameInterval:updateDelay];
        
		[displayLink addToRunLoop:[NSRunLoop currentRunLoop] forMode:NSDefaultRunLoopMode];
    }
	
	// Need a true or it will think its crashed, and fail.
	return true;
	
#else
	
#endif
}

void _iOSGameInnerLoop()
{
    if(!appIsRunning)
    {
        return;
    }
    else if(Game->isRunning())
    {
		S32 start = Platform::getRealMilliseconds();
		
        Game->mainLoop();
		
        S32 time = sgTimeManagerProcessInterval - (start - gLastStart);
		
        gLastStart = start;
        
        if(!CAdisplayLinkSupported)
        {
            iOSRunEventLoopTimer(time);
        }
	}
	else
	{
		Game->mainShutdown();
        
		// Need to actually exit the application now
		exit(0);
	}
}

void _iOSGameResignActive()
{
	Con::executef( 1, "oniOSResignActive" );
    appIsRunning = false;
}

void _iOSGameBecomeActive()
{
	clearPendingMultitouchEvents( );
    
	Con::executef( 1, "oniOSBecomeActive" );
    
    appIsRunning = true;
}

void _iOSGameWillTerminate()
{
	Con::executef( 1, "oniOSWillTerminate" );
    
	Con::executef( 1, "onExit" );
    
	Game->mainShutdown();
}

// Store current orientation for easy access
void _iOSGameChangeOrientation(S32 newOrientation)
{
	_iOSGameSetCurrentOrientation(newOrientation);
    
    // The rotation matching the project orientation must be allowed for any to occur
    if (Con::getBoolVariable("$pref::iOS::EnableOrientationRotation"))
    {
        // Start "collecting animations" 
        [UIView beginAnimations: nil context: nil];
        
        //  If the project is designed for landscape or it allows landscape rotation
        if ((Con::getIntVariable("$pref::iOS::ScreenOrientation") == 0) || Con::getBoolVariable("$pref::iOS::EnableOtherOrientationRotation"))
        {
            if (newOrientation == UIDeviceOrientationLandscapeLeft)
            {
                platState.Window.transform = CGAffineTransformMakeRotation(mDegToRad(0.0f));
                Con::executef(1, "oniOSOrientationToLandscapeLeft");
                //  Show animations
                [UIView commitAnimations];
                
                return;
            }
            
            if (newOrientation == UIDeviceOrientationLandscapeRight)
            {
                platState.Window.transform = CGAffineTransformMakeRotation(mDegToRad(180.0f));
                Con::executef(1, "oniOSOrientationToLandscapeRight");
                //  Show animations
                [UIView commitAnimations];
                
                return;
            }
        }
        
        // If the project is designed for portrait or it allows portrait rotation
        if ((Con::getIntVariable("$pref::iOS::ScreenOrientation") == 1) || Con::getBoolVariable("$pref::iOS::EnableOtherOrientationRotation"))
        {
            if (newOrientation == UIDeviceOrientationPortrait)
            {
                platState.Window.transform = CGAffineTransformMakeRotation(mDegToRad(270.0f));
                Con::executef(1, "oniOSOrientationToPortrait");     
                //  Show animations
                [UIView commitAnimations];
                
                return;
            }
            
            if (newOrientation == UIDeviceOrientationPortraitUpsideDown)
            {
                platState.Window.transform = CGAffineTransformMakeRotation(mDegToRad(90.0f));	
                Con::executef(1, "oniOSOrientationToPortraitUpsideDown");
                //  Show animations
                [UIView commitAnimations];
                
                return;
            }
        }
        
        // Show animations
        [UIView commitAnimations];
    }
}

void iOSRunEventLoopTimer(S32 intervalMs)
{
	
	//Luma: We only want to support NSTimer method, if below 3.1 OS.
	if(!CAdisplayLinkSupported)
	{
		if( intervalMs < 4 )
            intervalMs = 4;
        
        // EventTimerInterval is a double.
		NSTimeInterval interval = intervalMs / 1000.0; 
        
		platState.mainLoopTimer = [NSTimer scheduledTimerWithTimeInterval:interval target:platState.appID selector:@selector(runMainLoop) userInfo:nil repeats:NO];
        
		platState.sleepTicks = intervalMs;
	}
}

static void _iOSGetTxtFileArgs(int &argc, char** argv, int maxargc)
{
    argc = 0;
    
    const U32 kMaxTextLen = 2048;
    
    U32 textLen;
    
    char* text = new char[kMaxTextLen];   
    
    // Open the file, kick out if we can't
    File cmdfile;
    
    File::Status err = cmdfile.open("iOSCmdLine.txt", cmdfile.Read);
    
    // Re-organise function to handle memory deletion better
    if(err == File::Ok)
    {
        // read in the first kMaxTextLen bytes, kick out if we get errors or no data
        err = cmdfile.read(kMaxTextLen-1, text, &textLen);
        
        if(((err == File::Ok || err == File::EOS) || textLen > 0))
        {
            // Null terminate
            text[textLen++] = '\0';
            
            // Truncate to the 1st line of the file
            for(int i = 0; i < textLen; i++)
            {
                if( text[i] == '\n' || text[i] == '\r' )
                {
                    text[i] = '\0';
                    textLen = i+1;
                    break;
                }
            }
            
            // Tokenize the args with nulls, save them in argv, count them in argc
            char* tok;
            
            for(tok = dStrtok(text, " "); tok && argc < maxargc; tok = dStrtok(NULL, " "))
                argv[argc++] = tok;
		}
	}
	
	// Close file and delete memory before returning
    cmdfile.close();
    
	delete[] text;
    
	text = NULL;
}

int main(int argc, char *argv[])
{
    @autoreleasepool
    {
        int kMaxCmdlineArgs = 32; //Arbitrary
        
        printf("Initial Command Line\n");
        
        for(int i = 0; i < argc; i++)
            printf("%i : %s", i, argv[i]);
        
        NSString *nsStrVersion = [UIDevice currentDevice ].systemVersion;
        
        const char *strVersion = [nsStrVersion UTF8String ];
        
        platState.osVersion = dAtof( strVersion); 
        
        // Find Main.cs .
        const char* cwd = Platform::getMainDotCsDir();
        
        // Change to the directory that contains main.cs
        Platform::setCurrentDirectory(cwd);
        
        // Get the actual command line args
        S32 newArgc = argc;
        
        const char* newArgv[kMaxCmdlineArgs];
        
        for(int i=0; i < argc && i < kMaxCmdlineArgs; i++)
            newArgv[i] = argv[i];
        
        // Get the text file args
        S32 textArgc;
        
        char* textArgv[kMaxCmdlineArgs];
        
        _iOSGetTxtFileArgs(textArgc, textArgv, kMaxCmdlineArgs);
        
        // Merge them
        int i = 0;
        
        while(i < textArgc && newArgc < kMaxCmdlineArgs)
            newArgv[newArgc++] = textArgv[i++];
        
        // store them in platState
        platState.argc = newArgc;
        platState.argv = newArgv;
        
        printf("\nMerged Command Line\n");
        
        for( int i = 0; i < platState.argc; i++ )
            printf("%i : %s", i, platState.argv[i] );
        
        printf("\n");
        
        printf("exiting...\n");
        
        platState.appReturn = UIApplicationMain(argc, argv, nil, NSStringFromClass([AppDelegate class]));
        
        return platState.appReturn;
    }
}
