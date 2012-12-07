//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#import <UIKit/UIKit.h>


@interface iOSTextEntryController : UIViewController
{
	UITextField* textField;
	bool finished;
	bool userCanceled;
}

@property(nonatomic, readonly) IBOutlet UITextField* textField;
@property(nonatomic, readonly) bool finished;
@property(nonatomic, readonly) bool userCanceled;

- (IBAction)onCancelClicked: (id)sender;
- (IBAction)onCommitClicked: (id)sender;
- (IBAction)onEditingChanged: (id)sender;

- (void)onFinished: (id)sender;

@end
