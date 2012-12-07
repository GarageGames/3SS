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
function TSSConfirmDeleteProjectGui::display(%this, %message, %caller, %callback, %data)
{
    %this.message = %message;
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Canvas.pushDialog(%this);
}

function TSSConfirmDeleteProjectGui::onDialogPush(%this)
{
    TSSDeleteProjectMsg.setText(%this.message);
}

function TSSProjectDeleteOKBtn::onClick(%this)
{
    %object = TSSConfirmDeleteProjectGui.object;
    if (%object.isMethod(TSSConfirmDeleteProjectGui.handler))
    {
        %object.call(TSSConfirmDeleteProjectGui.handler, TSSConfirmDeleteProjectGui.data);
    }
    TSSConfirmDeleteProjectGui.object = "";
    TSSConfirmDeleteProjectGui.handler = "";
    TSSConfirmDeleteProjectGui.data = "";
    Canvas.popDialog(TSSConfirmDeleteProjectGui);
}

function TSSProjectDeleteCancelBtn::onClick(%this)
{
    TSSConfirmDeleteProjectGui.object = "";
    TSSConfirmDeleteProjectGui.handler = "";
    TSSConfirmDeleteProjectGui.data = "";
    Canvas.popDialog(TSSConfirmDeleteProjectGui);
}