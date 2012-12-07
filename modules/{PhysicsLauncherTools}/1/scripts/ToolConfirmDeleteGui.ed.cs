//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function displays a full-screen confirmation dialog with custom message and 
/// callback functionality.
/// </summary>
/// <param name="message">The message to display to the user.</param>
/// <param name="object">The object that contains a method to be called</param>
/// <param name="handler">The method on %object to call to handle the OK button click</param>
/// <param name="data">Additional information that needs to be passed on to %handler</param>
function Plt_ConfirmDeleteGui::display(%this, %message, %caller, %callback, %data)
{
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Plt_ConfirmDeleteMsgCtrl.setText(%message);
    Canvas.pushDialog(%this);
}

function Plt_ConfirmDeleteGui::onDialogPush(%this)
{
    %obj = %this.data;
    Plt_ConfirmDeleteMsgCtrl.text = "Are you sure you want to delete";
    Plt_ConfirmDeleteMsgCtrl.visible = true;
    Plt_ConfirmDeleteObjCtrl.setText(%obj.getInternalName());
    %arg = getWord(%this.data, 0);
    if (%arg !$= "")
    {
        Plt_DeletePromptDependencyList.clear();

        %depCount = 0; // need to use a TAML visitor to find occurences of this object in level files.
        if (%depCount > 0)
        {
            for (%i = 0; %i < %depCount; %i++)
            {
                %depItem = Plt_DeletePromptDependencyList.createDependencyItem(%name, %type);
                Plt_DeletePromptDependencyList.add(%depItem);
            }
        }
    }
    else
    {
        Plt_ConfirmDeleteMsgCtrl.text = %this.message;
        Plt_ConfirmDeleteMsgCtrl.visible = false;
    }
}

function Plt_DeleteOKBtn::onClick(%this)
{
    %object = Plt_ConfirmDeleteGui.object;
    if (%object.isMethod(Plt_ConfirmDeleteGui.handler))
        %object.call(Plt_ConfirmDeleteGui.handler, Plt_ConfirmDeleteGui.data);
    Plt_ConfirmDeleteGui.object = "";
    Plt_ConfirmDeleteGui.handler = "";
    Plt_ConfirmDeleteGui.data = "";
    Canvas.popDialog(Plt_ConfirmDeleteGui);
}

function Plt_DeleteCancelBtn::onClick(%this)
{
    Canvas.popDialog(Plt_ConfirmDeleteGui);
}