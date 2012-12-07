//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------


#include "platformiOS/platformiOS.h"
#include "platform/platformSemaphore.h"
#include "platform/platformVideo.h"
#include "platform/threads/thread.h"
#include "console/console.h"
#include "platformiOS/iOSEvents.h"
#include "platform/nativeDialogs/msgBox.h"

#include "platformiOS/iOSAlerts.h"


@implementation iOSAlertDelegate

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex {
	buttonNumber = buttonIndex;
}

//Luma: Added delegate for dismissed call by system
- (void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex {
	buttonNumber = buttonIndex;
}


- (void)didPresentAlertView:(UIAlertView *)alertView {
	
}

@end



bool iOSButtonBox(const char *windowTitle, const char *message, int numButtons = 0, NSString *buttons[] = nil, iOSAlertDelegate *delegate = nil) 
{

	UIAlertView *Alert =  [[UIAlertView alloc] initWithTitle: [NSString stringWithUTF8String: windowTitle]
													 message: [NSString stringWithUTF8String: message] 
													delegate: delegate 
										   cancelButtonTitle: nil
										   otherButtonTitles: nil ];
	
	if(numButtons > 0)
	{
		NSString *current = nil;
		for( int i = 1;  i < numButtons ; i++ ) 
		{
			current = buttons[i];
			[Alert addButtonWithTitle: current ];
		}
	}
	else 
	{
		[Alert addButtonWithTitle: @"OK" ];
	}

	
	[Alert show];
	
	// PUAP -Mat NOTE: NSRunLoop is not Thread-Safe, see documentation

	while (Alert.visible) 
	{
		[[NSRunLoop currentRunLoop] runMode: NSDefaultRunLoopMode beforeDate: [NSDate dateWithTimeIntervalSinceNow: 0.100]];
	}
	
	[Alert release];
	
	return true;
}



//-----------------------------------------------------------------------------
void Platform::AlertOK(const char *windowTitle, const char *message)
{
	iOSAlertDelegate *delegate = [[iOSAlertDelegate alloc] init];
	
	iOSButtonBox( windowTitle, message, 0, nil, delegate );
	
	[delegate release];
}
//-----------------------------------------------------------------------------
bool Platform::AlertOKCancel(const char *windowTitle, const char *message)
{	
	iOSAlertDelegate *delegate = [[iOSAlertDelegate alloc] init];
	

	NSString *buttons[] = { @"OK", @"Cancel" };
	
	//Luma:	Need to pass the delegate in as well
	iOSButtonBox( windowTitle, message, 2, buttons, delegate );	
	
	//Luma: Zero is NOT the cancel button index... it is based on the order of the buttons in the above array
	bool returnValue = (delegate->buttonNumber != 1 );
	[delegate release];
	return returnValue;
}

//-----------------------------------------------------------------------------
bool Platform::AlertRetry(const char *windowTitle, const char *message)
{//retry/cancel
	iOSAlertDelegate *delegate = [[iOSAlertDelegate alloc] init];
	
	//Luma:	Should be Retry / Cancel, not Cancel / Retry
	NSString *buttons[] = { @"Retry",
							@"Cancel",
						  };

	//Luma:	Need to pass the delegate in as well
	iOSButtonBox( windowTitle, message, 2, buttons, delegate );	
	
	//Luma: Zero is NOT the cancel button index... it is based on the order of the buttons in the above array
	bool returnValue = (delegate->buttonNumber != 1 );
	[delegate release];
	return returnValue;
}


bool Platform::AlertYesNo(const char *windowTitle, const char *message)
{	
	iOSAlertDelegate *delegate = [[iOSAlertDelegate alloc] init];
	
	NSString *buttons[] = { @"Yes", @"No" };
	
	iOSButtonBox( windowTitle, message, 2, buttons, delegate );	
	bool returnValue = (delegate->buttonNumber != 1 );
	[delegate release];
	
	return returnValue;
}
