//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
//
// How to use:
//
//    // Example 1: Create a dialog with two buttons active (middle button invisible).
//    ConfirmDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Remove Enemy?", 
//      "Delete", "DeleteSelectedEnemy();", "", "", "Cancel", "");
//
//    // Example 2: Create a dialog with three buttons.
//   ConfirmDialog.setupAndShow(%this.getGlobalCenter(), "Save strategy before exiting?", 
//      "Save", "AiEditorWindow.saveAndExit();", "Don't Save", "Canvas.popDialog(AiEditorGui);", "Cancel", "");   


/// <summary>
/// Sets Up and Shows the Dialog
/// </summary>
function ConfirmDialog::setupAndShow(%this, %center, %text, %buttonOneText, %buttonOneCommand,
                                     %buttonTwoText, %buttonTwoCommand, %buttonThreeText, %buttonThreeCommand)
{
   ConfirmDialogWindow.setPositionGlobal(%center.x - (ConfirmDialogWindow.extent.x / 2), 
      %center.y - (ConfirmDialogWindow.extent.y / 2));
      
   ConfirmDialogText.text = %text;
   
   ConfirmDialogOneButton.text = %buttonOneText;
   %this.oneCommand = %buttonOneCommand;
   
   ConfirmDialogTwoButton.text = %buttonTwoText;
   %this.twoCommand = %buttonTwoCommand;
   
   ConfirmDialogThreeButton.text = %buttonThreeText;
   %this.threeCommand = %buttonThreeCommand;
   
   Canvas.pushDialog(%this);
}

/// <summary>
/// Show/Hide active buttons.
/// </summary>
function ConfirmDialog::onDialogPush(%this)
{
   ConfirmDialogOneButton.Visible = (ConfirmDialogOneButton.text $= "") ? 0 : 1;
   ConfirmDialogTwoButton.Visible = (ConfirmDialogTwoButton.text $= "") ? 0 : 1;
   ConfirmDialogThreeButton.Visible = (ConfirmDialogThreeButton.text $= "") ? 0 : 1;
}

/// <summary>
/// Reset the dialog.
/// </summary>
function ConfirmDialog::onDialogPop(%this)
{
   ConfirmDialogText.text = "Are you sure?";
   ConfirmDialogOneButton.text = "";
   ConfirmDialogTwoButton.text = "";
   ConfirmDialogThreeButton.text = "";
   ConfirmDialog.oneCommand = "";
   ConfirmDialog.twoCommand = "";
   ConfirmDialog.threeCommand = "";
}

/// <summary>
/// Preform the "One" action assigned.
/// </summary>
function ConfirmDialogOneButton::onClick(%this)
{
   eval(ConfirmDialog.oneCommand);
   Canvas.popDialog(ConfirmDialog);
}

/// <summary>
/// Preform the "Two" action assigned.
/// </summary>
function ConfirmDialogTwoButton::onClick(%this)
{
   eval(ConfirmDialog.twoCommand);
   Canvas.popDialog(ConfirmDialog);
}

/// <summary>
/// Preform the "Three" action assigned.
/// </summary>
function ConfirmDialogThreeButton::onClick(%this)
{
   eval(ConfirmDialog.threeCommand);
   Canvas.popDialog(ConfirmDialog);
}
