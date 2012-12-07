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
function Pt_ConfirmDeleteGui::display(%this, %message, %caller, %callback, %data)
{
    %this.message = %message;
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Canvas.pushDialog(%this);
}

function Pt_ConfirmDeleteGui::onDialogPush(%this)
{
    %this.index = getWord(%this.data, 0);
    Pt_ConfirmDeleteObjCtrl.setText(getWord(%this.data, 1).getInternalName());
    Pt_DeletePromptDependencyList.clear();
    %count = getWordCount(%this.data);
    for (%i = 2; %i < %count; %i++)
    {
        %levelName = getWord(%this.data, %i);
        %item = Pt_DeletePromptDependencyList.createDependencyItem(%levelName, "Level");
        Pt_DeletePromptDependencyList.add(%item);
    }
}

function Pt_DependencyListContainer::add(%this, %item)
{
    %count = %this.getCount();
    %itemHeight = %item.Extent.y;
    %item.Position = "0" SPC (%count * %itemHeight);
    %this.addGuiControl(%item);
    %this.resize(0, 0, 266, (%count + 1) * %itemHeight);
}

function Pt_DependencyListContainer::clear(%this)
{
    %count = %this.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %this.getObject(0);
        %obj.delete();
    }
}

function Pt_DependencyListContainer::createDependencyItem(%this, %name, %type)
{
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="266 22";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };

    %itemName = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="3 3";
        Extent="100 16";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%name;
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%itemName);

    %itemType = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="120 3";
        Extent="126 16";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%type;
        maxLength="1024";
        truncate="1";
    };
    %control.addGuiControl(%itemType);
    return %control;
}

function Pt_DeleteOKBtn::onClick(%this)
{
    %object = Pt_ConfirmDeleteGui.object;
    if (%object.isMethod(Pt_ConfirmDeleteGui.handler))
    {
        %object.call(Pt_ConfirmDeleteGui.handler, Pt_ConfirmDeleteGui.index);
    }
    Pt_ConfirmDeleteGui.object = "";
    Pt_ConfirmDeleteGui.handler = "";
    Pt_ConfirmDeleteGui.data = "";
    Pt_ConfirmDeleteGui.index = "";
    Canvas.popDialog(Pt_ConfirmDeleteGui);
}

function Pt_DeleteCancelBtn::onClick(%this)
{
    Canvas.popDialog(Pt_ConfirmDeleteGui);
}