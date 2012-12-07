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
function ConfirmActionGui::display(%this, %message, %caller, %callback, %data)
{
    %this.message = %message;
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Canvas.pushDialog(%this);
}

function ConfirmActionGui::onDialogPush(%this)
{
    ConfirmActionMsgCtrl.setText(%this.message);
    ConfirmActionObjCtrl.visible = false;
}

function ActionOKBtn::onClick(%this)
{
    %object = ConfirmActionGui.object;
    if (%object.isMethod(ConfirmActionGui.handler))
    {
        %object.call(ConfirmActionGui.handler, ConfirmActionGui.data);
    }
    ConfirmActionGui.object = "";
    ConfirmActionGui.handler = "";
    ConfirmActionGui.data = "";
    Canvas.popDialog(ConfirmActionGui);
}

function ActionCancelBtn::onClick(%this)
{
    Canvas.popDialog(ConfirmActionGui);
}