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
/// <param name="data">Additional information that needs to be passed on to %handler. Takes tab or new-line delimited values.</param>
function ConfirmOverwriteGui::display(%this, %message, %caller, %callback, %data)
{
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    ConfirmOverwriteMsgCtrl.setText(%message);
    Canvas.pushDialog(%this);
}

function OverwriteOKBtn::onClick(%this)
{
    %object = ConfirmOverwriteGui.object;
    if (%object !$= "" && %object.isMethod(ConfirmOverwriteGui.handler))
    {
        %argCount = getFieldCount(ConfirmOverwriteGui.data);
        for (%i = 0; %i < %argCount; %i++)
        {
            if (%i == 0)
                %argList = getField(ConfirmOverwriteGui.data, %i);
            else
                %argList = %argList @ ", " @ getField(ConfirmOverwriteGui.data, %i);
        }
        
        %handler = ConfirmOverwriteGui.handler;
        eval(%object @ "." @ %handler @ "(" @ %argList @ ");");
    }
    else if (isFunction(ConfirmOverwriteGui.handler))
    {
        %argCount = getFieldCount(ConfirmOverwriteGui.data);
        for (%i = 0; %i < %argCount; %i++)
        {
            if (%i == 0)
                %argList = getField(ConfirmOverwriteGui.data, %i);
            else
                %argList = %argList @ ", " @ getField(ConfirmOverwriteGui.data, %i);
        }
        %handler = ConfirmOverwriteGui.handler;
        eval(%handler @ "(" @ %argList @ ");");
    }
    ConfirmOverwriteGui.object = "";
    ConfirmOverwriteGui.handler = "";
    ConfirmOverwriteGui.data = "";
    Canvas.popDialog(ConfirmOverwriteGui);
}

function OverwriteCancelBtn::onClick(%this)
{
    Canvas.popDialog(ConfirmOverwriteGui);
}