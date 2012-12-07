//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
//
// How to use:
//
//    // Example 1: Create a dialog with two buttons active (middle button invisible).
//    WarningDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Warning!", "Remove Enemy?", 
//      "Delete", "DeleteSelectedEnemy();", "", "", "Cancel", "");
//
//    // Example 2: Create a dialog with three buttons.
//   WarningDialog.setupAndShow(%this.getGlobalCenter(), "Notice", "Save strategy before exiting?", 
//      "Save", "AiEditorWindow.saveAndExit();", "Don't Save", "Canvas.popDialog(AiEditorGui);", "Cancel", "");   


/// <summary>
/// Sets Up and Shows the Dialog
/// </summary>
function WarningDialog::setupAndShow(%this, %center, %title, %text, %buttonOneText, %buttonOneCommand,
                                     %buttonTwoText, %buttonTwoCommand, %buttonThreeText, %buttonThreeCommand)
{
   WarningDialogWindow.text = %title;
   
   WarningDialogWindow.setPositionGlobal(%center.x - (WarningDialogWindow.extent.x / 2), 
      %center.y - (WarningDialogWindow.extent.y / 2));
      
   WarningDialogText.setText(%text);
   
   WarningDialogOneButton.text = %buttonOneText;
   %this.oneCommand = %buttonOneCommand;
   
   WarningDialogTwoButton.text = %buttonTwoText;
   %this.twoCommand = %buttonTwoCommand;
   
   WarningDialogThreeButton.text = %buttonThreeText;
   %this.threeCommand = %buttonThreeCommand;
   
   Canvas.pushDialog(%this);
}

/// <summary>
/// Show/Hide active buttons.
/// </summary>
function WarningDialog::onDialogPush(%this)
{
   WarningDialogOneButton.Visible = (WarningDialogOneButton.text $= "") ? 0 : 1;
   WarningDialogTwoButton.Visible = (WarningDialogTwoButton.text $= "") ? 0 : 1;
   WarningDialogThreeButton.Visible = (WarningDialogThreeButton.text $= "") ? 0 : 1;
}

/// <summary>
/// Reset the dialog.
/// </summary>
function WarningDialog::onDialogPop(%this)
{
   WarningDialogText.text = "Are you sure?";
   WarningDialogOneButton.text = "";
   WarningDialogTwoButton.text = "";
   WarningDialogThreeButton.text = "";
   WarningDialog.oneCommand = "";
   WarningDialog.twoCommand = "";
   WarningDialog.threeCommand = "";
}

/// <summary>
/// Preform the "One" action assigned.
/// </summary>
function WarningDialogOneButton::onClick(%this)
{
   eval(WarningDialog.oneCommand);
   Canvas.popDialog(WarningDialog);
}

/// <summary>
/// Preform the "Two" action assigned.
/// </summary>
function WarningDialogTwoButton::onClick(%this)
{
   eval(WarningDialog.twoCommand);
   Canvas.popDialog(WarningDialog);
}

/// <summary>
/// Preform the "Three" action assigned.
/// </summary>
function WarningDialogThreeButton::onClick(%this)
{
   eval(WarningDialog.threeCommand);
   Canvas.popDialog(WarningDialog);
}
