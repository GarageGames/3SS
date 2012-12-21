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
function ConfirmABCGui::display(%this, %message, %caller, %replaceCallback, %copyCallback, %replaceData, %copyData)
{
    %this.object = %caller;
    %this.replaceHandler = %replaceCallback;
    %this.copyHandler = %copyCallback;
    %this.replaceData = %copyData;
    %this.copyData = %data;
    ConfirmABCMsgCtrl.setText(%message);
    Canvas.pushDialog(%this);
}

function ReplaceAssetBtn::onClick(%this)
{
    %object = ConfirmABCGui.object;
    if (%object !$= "" && %object.isMethod(ConfirmABCGui.replaceHandler))
    {
        %argCount = getFieldCount(ConfirmABCGui.replaceData);
        for (%i = 0; %i < %argCount; %i++)
        {
            if (%i == 0)
                %argList = "\"" @ getField(ConfirmABCGui.replaceData, %i) @ "\"";
            else
                %argList = %argList @ ", " @ "\"" @ getField(ConfirmABCGui.replaceData, %i) @ "\"";
        }
        %handler = ConfirmABCGui.replaceHandler;
        eval(%object @ "." @ %handler @ "(" @ %argList @ ");");
    }
    else if (isFunction(ConfirmABCGui.replaceHandler))
    {
        %argCount = getFieldCount(ConfirmABCGui.replaceData);
        for (%i = 0; %i < %argCount; %i++)
        {
            if (%i == 0)
                %argList = "\"" @ getField(ConfirmABCGui.replaceData, %i) @ "\"";
            else
                %argList = %argList @ ", " @ "\"" @ getField(ConfirmABCGui.replaceData, %i) @ "\"";
        }
        %handler = ConfirmABCGui.replaceHandler;
        eval(%handler @ "(" @ %argList @ ");");
    }
    ConfirmABCGui.object = "";
    ConfirmABCGui.replaceHandler = "";
    ConfirmABCGui.copyHandler = "";
    ConfirmABCGui.replaceData = "";
    ConfirmABCGui.copyData = "";
    Canvas.popDialog(ConfirmABCGui);
}

function CopyAssetBtn::onClick(%this)
{
    %object = ConfirmABCGui.object;
    if (%object !$= "" && %object.isMethod(ConfirmABCGui.copyHandler))
    {
        %argCount = getFieldCount(ConfirmABCGui.copyData);
        for (%i = 0; %i < %argCount; %i++)
        {
            if (%i == 0)
                %argList = "\"" @ getField(ConfirmABCGui.copyData, %i) @ "\"";
            else
                %argList = %argList @ ", " @ "\"" @ getField(ConfirmABCGui.copyData, %i) @ "\"";
        }
        %handler = ConfirmABCGui.copyHandler;
        eval(%object @ "." @ %handler @ "(" @ %argList @ ");");
    }
    else if (isFunction(ConfirmABCGui.copyHandler))
    {
        %argCount = getFieldCount(ConfirmABCGui.copyData);
        for (%i = 0; %i < %argCount; %i++)
        {
            if (%i == 0)
                %argList = "\"" @ getField(ConfirmABCGui.copyData, %i) @ "\"";
            else
                %argList = %argList @ ", " @ "\"" @ getField(ConfirmABCGui.copyData, %i) @ "\"";
        }
        %handler = ConfirmABCGui.copyHandler;
        eval(%handler @ "(" @ %argList @ ");");
    }
    ConfirmABCGui.object = "";
    ConfirmABCGui.replaceHandler = "";
    ConfirmABCGui.copyHandler = "";
    ConfirmABCGui.replaceData = "";
    ConfirmABCGui.copyData = "";
    Canvas.popDialog(ConfirmABCGui);
}

function GuiImportCancelBtn::onClick(%this)
{
    ConfirmABCGui.object = "";
    ConfirmABCGui.replaceHandler = "";
    ConfirmABCGui.copyHandler = "";
    ConfirmABCGui.replaceData = "";
    ConfirmABCGui.copyData = "";
    Canvas.popDialog(ConfirmABCGui);
}