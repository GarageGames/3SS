//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
//
// How to use:
//
//    // Example 1: Create a dialog with two buttons active (middle button invisible).
//    AssetLibraryDialog.setupAndShow(EnemyToolWindow.getGlobalCenter(), "Warning!", "Remove Enemy?", 
//      "Delete", "DeleteSelectedEnemy();", "", "", "Cancel", "");
//
//    // Example 2: Create a dialog with three buttons.
//   AssetLibraryDialog.setupAndShow(%this.getGlobalCenter(), "Notice", "Save strategy before exiting?", 
//      "Save", "AiEditorWindow.saveAndExit();", "Don't Save", "Canvas.popDialog(AiEditorGui);", "Cancel", "");   


/// <summary>
/// Sets Up and Shows the Dialog
/// </summary>
function AssetLibraryDialog::setupAndShow(%this, %center, %title, %text, %buttonOneText, %buttonOneCommand,
                                     %buttonTwoText, %buttonTwoCommand, %buttonThreeText, %buttonThreeCommand)
{
   AssetLibraryDialogWindow.text = %title;
   
   AssetLibraryDialogWindow.setPositionGlobal(%center.x - (AssetLibraryDialogWindow.extent.x / 2), 
      %center.y - (AssetLibraryDialogWindow.extent.y / 2));
      
   AssetLibraryDialogText.setText(%text);
   
   AssetLibraryDialogOneButton.text = %buttonOneText;
   %this.oneCommand = %buttonOneCommand;
   
   AssetLibraryDialogTwoButton.text = %buttonTwoText;
   %this.twoCommand = %buttonTwoCommand;
   
   AssetLibraryDialogThreeButton.text = %buttonThreeText;
   %this.threeCommand = %buttonThreeCommand;
   
   Canvas.pushDialog(%this);
}

/// <summary>
/// Show/Hide active buttons.
/// </summary>
function AssetLibraryDialog::onDialogPush(%this)
{
   AssetLibraryDialogOneButton.Visible = (AssetLibraryDialogOneButton.text $= "") ? 0 : 1;
   AssetLibraryDialogTwoButton.Visible = (AssetLibraryDialogTwoButton.text $= "") ? 0 : 1;
   AssetLibraryDialogThreeButton.Visible = (AssetLibraryDialogThreeButton.text $= "") ? 0 : 1;
}

/// <summary>
/// Reset the dialog.
/// </summary>
function AssetLibraryDialog::onDialogPop(%this)
{
   AssetLibraryDialogText.text = "Are you sure?";
   AssetLibraryDialogOneButton.text = "";
   AssetLibraryDialogTwoButton.text = "";
   AssetLibraryDialogThreeButton.text = "";
   AssetLibraryDialog.oneCommand = "";
   AssetLibraryDialog.twoCommand = "";
   AssetLibraryDialog.threeCommand = "";
}

/// <summary>
/// Preform the "One" action assigned.
/// </summary>
function AssetLibraryDialogOneButton::onClick(%this)
{
   eval(AssetLibraryDialog.oneCommand);
   Canvas.popDialog(AssetLibraryDialog);
}

/// <summary>
/// Preform the "Two" action assigned.
/// </summary>
function AssetLibraryDialogTwoButton::onClick(%this)
{
   eval(AssetLibraryDialog.twoCommand);
   Canvas.popDialog(AssetLibraryDialog);
}

/// <summary>
/// Preform the "Three" action assigned.
/// </summary>
function AssetLibraryDialogThreeButton::onClick(%this)
{
   eval(AssetLibraryDialog.threeCommand);
   Canvas.popDialog(AssetLibraryDialog);
}
