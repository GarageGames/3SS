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
function Ie_ConfirmChangeGui::display(%this, %message, %caller, %callback, %data)
{
    %this.message = %message;
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Canvas.pushDialog(%this);
}

function Ie_ConfirmChangeGui::onDialogPush(%this)
{
    %arg = getWord(%this.data, 0);
    %declaredAsset = AssetDatabase.isDeclaredAsset(%arg);
    if (%declaredAsset)
    {
        %tempAsset = AssetDatabase.acquireAsset(%arg);
        Ie_ConfirmChangeMsgCtrl.text = "Are you sure you want to change";
        Ie_ConfirmChangeObjCtrl.visible = true;
        Ie_ConfirmChangeObjCtrl.setText(%tempAsset.AssetName);
        Ie_ChangePromptDependencyList.clear();

        %query = new AssetQuery();
        %depCount = AssetDatabase.findAssetIsDependedOn(%query, %arg);
        if (%depCount > 0)
        {
            for (%i = 0; %i < %depCount; %i++)
            {
                %asset = %query.getAsset(%i);
                %assetObj = AssetDatabase.acquireAsset(%asset);
                %assetName = %assetObj.AssetName;
                %assetType = %assetObj.AssetCategory;
                if (%assetType $= "")
                    %assetType = %assetObj.getClassName();
                %depItem = Ie_ChangePromptDependencyList.createDependencyItem(%assetName, %assetType);
                AssetDatabase.releaseAsset(%asset);
                Ie_ChangePromptDependencyList.add(%depItem);
                %this.dependentList = %this.dependentList SPC %asset;
            }
            %this.dependentList = trim(%this.dependentList);
        }
        AssetDatabase.releaseAsset(%arg);
    }
    else
    {
        Ie_ConfirmChangeMsgCtrl.text = %this.message;
        Ie_ConfirmChangeObjCtrl.visible = false;
    }
}

function Ie_ChangePromptDependencyList::add(%this, %item)
{
    %count = %this.getCount();
    %itemHeight = %item.Extent.y;
    %item.Position = "0" SPC (%count * %itemHeight);
    %this.addGuiControl(%item);
    %this.resize(0, 0, 266, (%count + 1) * %itemHeight);
}

function Ie_ChangePromptDependencyList::clear(%this)
{
    %count = %this.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %this.getObject(0);
        %obj.delete();
    }
}

function Ie_ChangePromptDependencyList::createDependencyItem(%this, %name, %type)
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

function Ie_ChangeOKBtn::onClick(%this)
{
    %object = Ie_ConfirmChangeGui.object;
    if (%object.isMethod(Ie_ConfirmChangeGui.handler))
        %object.call(Ie_ConfirmChangeGui.handler, Ie_ConfirmChangeGui.dependentList);

    Ie_ConfirmChangeGui.object = "";
    Ie_ConfirmChangeGui.handler = "";
    Ie_ConfirmChangeGui.data = "";
    Ie_ConfirmChangeGui.dependentList = "";
    Canvas.popDialog(Ie_ConfirmChangeGui);
}

function Ie_ChangeCancelBtn::onClick(%this)
{
    Canvas.popDialog(Ie_ConfirmChangeGui);
}