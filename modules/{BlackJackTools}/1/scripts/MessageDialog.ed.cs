//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
//
// How to use:
//
//    // Example 1: Create a dialog with two buttons active (middle button invisible).
//    MessageDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Remove Enemy?", 
//      "Delete", "DeleteSelectedEnemy();", "", "", "Cancel", "");
//
//    // Example 2: Create a dialog with three buttons.
//   MessageDialog.setupAndShow(%this.getGlobalCenter(), "Save strategy before exiting?", 
//      "Save", "AiEditorWindow.saveAndExit();", "Don't Save", "Canvas.popDialog(AiEditorGui);", "Cancel", "");   


/// <summary>
/// Sets Up and Shows the Dialog
/// </summary>
function MessageDialog::setupAndShow(%this, %center, %text, %buttonOneText, %buttonOneCommand,
                                     %buttonTwoText, %buttonTwoCommand, %buttonThreeText, %buttonThreeCommand)
{
   MessageDialogWindow.setPositionGlobal(%center.x - (MessageDialogWindow.extent.x / 2), 
      %center.y - (MessageDialogWindow.extent.y / 2));
      
   MessageDialogText.text = %text;
   
   MessageDialogOneButton.text = %buttonOneText;
   %this.oneCommand = %buttonOneCommand;
   
   MessageDialogTwoButton.text = %buttonTwoText;
   %this.twoCommand = %buttonTwoCommand;
   
   MessageDialogThreeButton.text = %buttonThreeText;
   %this.threeCommand = %buttonThreeCommand;
   
   Canvas.pushDialog(%this);
}

/// <summary>
/// Show/Hide active buttons.
/// </summary>
function MessageDialog::onDialogPush(%this)
{
   MessageDialogOneButton.Visible = (MessageDialogOneButton.text $= "") ? 0 : 1;
   MessageDialogTwoButton.Visible = (MessageDialogTwoButton.text $= "") ? 0 : 1;
   MessageDialogThreeButton.Visible = (MessageDialogThreeButton.text $= "") ? 0 : 1;
}

/// <summary>
/// Reset the dialog.
/// </summary>
function MessageDialog::onDialogPop(%this)
{
   MessageDialogText.text = "Are you sure?";
   MessageDialogOneButton.text = "";
   MessageDialogTwoButton.text = "";
   MessageDialogThreeButton.text = "";
   MessageDialog.oneCommand = "";
   MessageDialog.twoCommand = "";
   MessageDialog.threeCommand = "";
}

/// <summary>
/// Preform the "One" action assigned.
/// </summary>
function MessageDialogOneButton::onClick(%this)
{
   eval(MessageDialog.oneCommand);
   Canvas.popDialog(MessageDialog);
}

/// <summary>
/// Preform the "Two" action assigned.
/// </summary>
function MessageDialogTwoButton::onClick(%this)
{
   eval(MessageDialog.twoCommand);
   Canvas.popDialog(MessageDialog);
}

/// <summary>
/// Preform the "Three" action assigned.
/// </summary>
function MessageDialogThreeButton::onClick(%this)
{
   eval(MessageDialog.threeCommand);
   Canvas.popDialog(MessageDialog);
}
