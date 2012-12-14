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
function ConfirmDeleteGui::display(%this, %message, %caller, %callback, %data)
{
    %this.message = %message;
    %this.object = %caller;
    %this.handler = %callback;
    %this.data = %data;
    Canvas.pushDialog(%this);
}

function ConfirmDeleteGui::onDialogPush(%this)
{
    %arg = getWord(%this.data, 0);
    %declaredAsset = AssetDatabase.isDeclaredAsset(%arg);
    if (%declaredAsset)
    {
        %tempAsset = AssetDatabase.acquireAsset(%arg);
        ConfirmDeleteMsgCtrl.text = "Are you sure you want to delete";
        ConfirmDeleteObjCtrl.visible = true;
        ConfirmDeleteObjCtrl.setText(%tempAsset.AssetName);
        DeletePromptDependencyList.clear();

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
                %depItem = DeletePromptDependencyList.createDependencyItem(%assetName, %assetType);
                AssetDatabase.releaseAsset(%asset);
                DeletePromptDependencyList.add(%depItem);
            }
        }
        AssetDatabase.releaseAsset(%arg);
    }
    else
    {
        ConfirmDeleteMsgCtrl.text = %this.message;
        ConfirmDeleteObjCtrl.visible = false;
    }
}

function DependencyListContainer::add(%this, %item)
{
    %count = %this.getCount();
    %itemHeight = %item.Extent.y;
    %item.Position = "0" SPC (%count * %itemHeight);
    %this.addGuiControl(%item);
    %this.resize(0, 0, 266, (%count + 1) * %itemHeight);
}

function DependencyListContainer::clear(%this)
{
    %count = %this.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %this.getObject(0);
        %obj.delete();
    }
}

function DependencyListContainer::createDependencyItem(%this, %name, %type)
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
        Extent="166 16";
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
        Position="166 3";
        Extent="100 16";
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

function DeleteOKBtn::onClick(%this)
{
    %object = ConfirmDeleteGui.object;
    if (%object.isMethod(ConfirmDeleteGui.handler))
    {
        %argCount = getWordCount(ConfirmDeleteGui.data);
        for (%i = 0; %i < %argCount; %i++)
        {
            %argList[%i] = getWord(ConfirmDeleteGui.data, %i);
        }
        switch(%argCount)
        {
            case 0:
                %object.call(ConfirmDeleteGui.handler);
            case 1:
                %object.call(ConfirmDeleteGui.handler, %argList[0]);
            case 2:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1]);
            case 3:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1], %argList[2]);
            case 4:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1], %argList[2], %argList[3]);
            case 5:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1], %argList[2], %argList[3], %argList[4]);
            case 6:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1], %argList[2], %argList[3], %argList[4], %argList[5]);
            case 7:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1], %argList[2], %argList[3], %argList[4], %argList[5], %argList[6]);
            case 8:
                %object.call(ConfirmDeleteGui.handler, %argList[0], %argList[1], %argList[2], %argList[3], %argList[4], %argList[5], %argList[6], %argList[7]);
        }
    }
    ConfirmDeleteGui.object = "";
    ConfirmDeleteGui.handler = "";
    ConfirmDeleteGui.data = "";
    Canvas.popDialog(ConfirmDeleteGui);
}

function DeleteCancelBtn::onClick(%this)
{
    Canvas.popDialog(ConfirmDeleteGui);
}