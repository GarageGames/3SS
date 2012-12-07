//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function displays a full-screen confirmation dialog with custom message and 
/// callback functionality.
/// </summary>
/// <param name="message">The message to display to the user.</param>
/// <param name="object">The object that contains a method to be called.</param>
/// <param name="handler">The method on %object to call to handle the OK button click.</param>
/// <param name="data">Additional information that needs to be passed on to %handler.  Takes up to 8 space-separated items.</param>
function Wt_ConfirmLevelDeleteGui::display(%this, %message, %caller, %callback, %data)
{
    %this.message = %message;
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Canvas.pushDialog(%this);
}

function Wt_ConfirmLevelDeleteGui::onDialogPush(%this)
{
    Wt_LevelDeleteMessageText.setText(%this.message);
}

function Wt_LevelDeleteOKBtn::onClick(%this)
{
    %object = Wt_ConfirmLevelDeleteGui.object;
    if (%object.isMethod(Wt_ConfirmLevelDeleteGui.handler))
    {
        %object.call(Wt_ConfirmLevelDeleteGui.handler, Wt_ConfirmLevelDeleteGui.data);
    }
    Wt_ConfirmLevelDeleteGui.object = "";
    Wt_ConfirmLevelDeleteGui.handler = "";
    Wt_ConfirmLevelDeleteGui.data = "";
    Canvas.popDialog(Wt_ConfirmLevelDeleteGui);
}

function Wt_LevelDeleteCancelBtn::onClick(%this)
{
    Wt_ConfirmLevelDeleteGui.object = "";
    Wt_ConfirmLevelDeleteGui.handler = "";
    Wt_ConfirmLevelDeleteGui.data = "";
    Canvas.popDialog(Wt_ConfirmLevelDeleteGui);
}