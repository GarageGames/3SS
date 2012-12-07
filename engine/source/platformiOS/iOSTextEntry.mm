//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/*
 *  TextEntry.mm
 *  TGE
 *
 *  Created by James Touton on 11/19/08.
 *  Copyright 2008 Pick Up And Play. All rights reserved.
 *
 */

#include "platformiOS/iOSTextEntry.h"
#include "platformiOS/platformiOS.h"
#include "platformiOS/iOSTextEntryController.h"

extern void _iOSGameInnerLoop();

void iOSTextEntry::onUserTextFinished(bool cancelled, const char* &result)
{
	Con::executef(3,"oniOSKeyboardInputFinished",Con::getIntArg(0), Con::getIntArg(cancelled), Con::getReturnBuffer(result));
	Con::setVariable("$iOS::keyboardWasCancelled", Con::getIntArg(cancelled));
	
}

bool iOSTextEntry::getUserText(StringBuffer& text)
{
	
	iOSTextEntryController* controller = [[iOSTextEntryController alloc] initWithNibName: @"iOSTextEntry" bundle: nil];
	UIWindow* window = [UIApplication sharedApplication].keyWindow;
	[controller loadView];
	
	NSString* nsText = [[NSString alloc] initWithBytesNoCopy: const_cast<UTF16*>(text.getPtr()) length: text.length() * sizeof(UTF16) encoding: NSUTF16LittleEndianStringEncoding freeWhenDone: NO];
	controller.textField.text = nsText;
	[nsText release];
	
	controller.view.alpha = 0.0f;
	[window addSubview: controller.view];
	
	[UIView beginAnimations: nil context: nil];
	
	controller.view.center = CGPointMake(platState.windowSize.x / 2, platState.windowSize.y / 2);
	
	if(!platState.portrait) 
	{
		//Rotate the view according to landscape mode
		controller.view.transform = CGAffineTransformMakeRotation(mDegToRad(90.0f));
	}	
	
//	controller.view.transform = CGAffineTransformMakeTranslation(0, 0);
	if(!platState.portrait)
	{
		//set the frame to a landscape size
		controller.view.frame = CGRectMake(0.0f, 0.0f, platState.windowSize.y, platState.windowSize.x);
	}
	else
	{
		//set the frame to portrait mode
		controller.view.frame = CGRectMake(0.0f, 0.0f, platState.windowSize.x, platState.windowSize.y);
	}
	
	controller.view.alpha = 1.0f;

	[UIView commitAnimations];
	
	[controller.textField becomeFirstResponder];
	
	//Notify we want to listen for an event
	[[NSNotificationCenter defaultCenter] addObserver:controller
									selector:@selector(onFinished:)
										name:@"didFinishNotification" object:nil];
	
	/*[controller.view removeFromSuperview];
	
	bool userCanceled = controller.userCanceled;
	if (!userCanceled) {
		text.set(reinterpret_cast<const UTF16*>([controller.textField.text cStringUsingEncoding: NSUTF16LittleEndianStringEncoding]));
	}
	
	[controller release];
	Con::setVariable("$iOS::keyboardWasCancelled", Con::getIntArg(userCanceled));
	
	return !userCanceled;
	*/
	
	return true;
	
	/*
	iOSTextEntryController* controller = [[iOSTextEntryController alloc] initWithNibName: @"TextEntry" bundle: nil];
	UIWindow* window = [UIApplication sharedApplication].keyWindow;
   [controller loadView];

	NSString* nsText = [[NSString alloc] initWithBytesNoCopy: const_cast<UTF16*>(text.getPtr()) length: text.length() * sizeof(UTF16) encoding: NSUTF16LittleEndianStringEncoding freeWhenDone: NO];
	controller.textField.text = nsText;
	[nsText release];

	controller.view.alpha = 0.0f;
	[window addSubview: controller.view];

	// For some reason, autorotation doesn't always happen -- check for this here:
	UIApplication* app = [UIApplication sharedApplication];
	CGAffineTransform viewTransform = controller.view.transform;
	if ((app.statusBarOrientation != UIInterfaceOrientationPortrait) && (memcmp(&viewTransform, &CGAffineTransformIdentity, sizeof(CGAffineTransform)) == 0)) {
		CGRect bounds = controller.view.bounds;
		switch (app.statusBarOrientation) {
			case UIInterfaceOrientationLandscapeLeft:
				controller.view.bounds = CGRectMake(bounds.origin.x, bounds.origin.y, bounds.size.height, bounds.size.width);
				controller.view.transform = CGAffineTransformMakeRotation((3 * M_PI) / 2);
				break;

			case UIInterfaceOrientationLandscapeRight:
				controller.view.bounds = CGRectMake(bounds.origin.x, bounds.origin.y, bounds.size.height, bounds.size.width);
				controller.view.transform = CGAffineTransformMakeRotation(M_PI / 2);
				break;

			case UIInterfaceOrientationPortraitUpsideDown:
				controller.view.transform = CGAffineTransformMakeRotation(M_PI);
				break;
		}
	}

	[UIView beginAnimations: nil context: nil];
	controller.view.alpha = 1.0f;
	[UIView commitAnimations];

	[controller.textField becomeFirstResponder];

	//while (!controller.finished) {
		//[[NSRunLoop currentRunLoop] runMode: NSDefaultRunLoopMode beforeDate: [NSDate distantFuture]];
	//}

	[controller.view removeFromSuperview];

	bool userCanceled = controller.userCanceled;
	if (!userCanceled) {
		text.set(reinterpret_cast<const UTF16*>([controller.textField.text cStringUsingEncoding: NSUTF16LittleEndianStringEncoding]));
	}

	[controller release];
	return !userCanceled;
	 
	 */
}

//Luma: add console keyboard support
ConsoleFunction(getiOSKeyboardInput, const char *, 2, 2, "getiOSKeyboardInput(textToUse) Returns a string of text from the iOS OS keyboard")
{
    argc; argv;	 
	
	StringBuffer mTextBuffer = argv[1];
	bool cancelled = !iOSTextEntry::getUserText(mTextBuffer);
	
	const char *retrn = argv[1];
	
	if(cancelled)
	{
		//If they cancelled, return what they passed in as the "textToUse"
		Con::setVariable("$iOS::keyboardWasCancelled", "1" );
		
	}
	else 
	{
		//If they didnt cancel, return the results of their entry
		char *ret = Con::getReturnBuffer(mTextBuffer.length());
		dStrcpy(ret, mTextBuffer.getPtr8());
		
		Con::setVariable("$iOS::keyboardWasCancelled", "0" );
		
		return ret;
	}
	
	//Return the input text
    return retrn;	
}
