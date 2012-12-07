//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//
// iOSTextEntryController.mm
//  TGE
//
//  Created by Sven Bergstrom.
//  Copyright 2009 Luma Arcade. All rights reserved.
//

#import "iOSTextEntryController.h"
#include "platformiOS/iOSTextEntry.h"
 
@implementation iOSTextEntryController

@synthesize textField;
@synthesize finished; 
@synthesize userCanceled;

- (void)dealloc
{
	[textField release];
    [super dealloc];
}

- (void)onFinished: (id)sender
{
	[self.view removeFromSuperview];
		
	const char* str = [textField.text cString];
	iOSTextEntry::onUserTextFinished(userCanceled, str);
	
	[self release];
}

- (IBAction)onCancelClicked: (id)sender
{
	userCanceled = true;
	finished = true;
	[[NSNotificationCenter defaultCenter]
	 postNotificationName:@"didFinishNotification" object:self];
}

- (IBAction)onCommitClicked: (id)sender
{
	finished = true;
	[[NSNotificationCenter defaultCenter]
	 postNotificationName:@"didFinishNotification" object:self];
}


#define LEGAL @"1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz "
- (IBAction)onEditingChanged: (id)sender
{
	if (textField.text.length > 15) 
	{ 
		textField.text = [textField.text substringToIndex:15]; 
	}
}


- (BOOL)textField:(UITextField *)textField shouldChangeCharactersInRange:(NSRange)range replacementString:(NSString *)string
{
	unichar lastChar = [string characterAtIndex:string.length - 1];
	if(lastChar == '\n') return YES;
	
    NSCharacterSet *cs = [[NSCharacterSet characterSetWithCharactersInString:LEGAL] invertedSet];
	NSString *filtered = [[string componentsSeparatedByCharactersInSet:cs] componentsJoinedByString:@""];
	return [string isEqualToString:filtered];
}

@end
