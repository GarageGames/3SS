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
function Wt_WorldConfirmDeleteGui::display(%this,%worldName, %caller, %callback, %data)
{
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    %this.worldName = %worldName;
    Canvas.pushDialog(%this);
}

function Wt_WorldConfirmDeleteGui::onDialogPush(%this)
{
    %arg = getWord(%this.data, 0);
    %this.index = %arg;

    Wt_WorldConfirmDeleteObjCtrl.visible = true;
    Wt_WorldConfirmDeleteObjCtrl.setText(%this.worldName);
    Wt_ConfirmDeleteListContainer.clear();
 
    %i = 1;
    while ( %arg !$= "" )
    {
        %arg = getWord(%this.data, %i);
        %depItem = Wt_ConfirmDeleteListContainer.createDependencyItem(%arg);
        Wt_ConfirmDeleteListContainer.add(%depItem);
        %i++;
    }
}

function Wt_ConfirmDeleteListContainer::add(%this, %item)
{
    %count = %this.getCount();
    %itemHeight = %item.Extent.y;
    %item.Position = "0" SPC (%count * %itemHeight);
    %this.addGuiControl(%item);
    %this.resize(0, 0, 266, (%count + 1) * %itemHeight);
}

function Wt_ConfirmDeleteListContainer::clear(%this)
{
    %count = %this.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %this.getObject(0);
        %obj.delete();
    }
}

function Wt_ConfirmDeleteListContainer::createDependencyItem(%this, %name, %type)
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
    };
    %control.addGuiControl(%itemType);
    return %control;
}

function Wt_WorldDeleteOKBtn::onClick(%this)
{
    %object = Wt_WorldConfirmDeleteGui.object;
    if (%object.isMethod(Wt_WorldConfirmDeleteGui.handler))
    {
        %object.call(Wt_WorldConfirmDeleteGui.handler, Wt_WorldConfirmDeleteGui.index);
    }
    Wt_WorldConfirmDeleteGui.object = "";
    Wt_WorldConfirmDeleteGui.handler = "";
    Wt_WorldConfirmDeleteGui.data = "";
    Canvas.popDialog(Wt_WorldConfirmDeleteGui);
}

function Wt_WorldDeleteCancelBtn::onClick(%this)
{
    Canvas.popDialog(Wt_WorldConfirmDeleteGui);
}