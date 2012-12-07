//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
/*
Copy and paste this set of controls into your tool to assure a standardized look.

Remember to change the name, tool, adjust location and size of the inner GuiControl.
*/
//-----------------------------------------------------------------------------
/*
<GuiControl
    canSaveDynamicFields="0"
    isContainer="1"
    Profile="GuiTextEditProfile"
    HorizSizing="right"
    VertSizing="bottom"
    Position="3 30"
    Extent="216 115"
    MinExtent="8 2"
    canSave="1"
    Visible="1"
    Active="0"
    tooltipprofile="GuiDefaultProfile"
    hovertime="1000">
    <GuiScrollCtrl
        canSaveDynamicFields="0"
        isContainer="1"
        Profile="GuiScrollProfile"
        HorizSizing="right"
        VertSizing="bottom"
        Position="3 3"
        Extent="211 109"
        MinExtent="8 2"
        canSave="1"
        Visible="1"
        Active="0"
        tooltipprofile="GuiDefaultProfile"
        hovertime="1000"
        willFirstRespond="1"
        hScrollBar="alwaysOff"
        vScrollBar="alwaysOn"
        constantThumbHeight="0"
        childMargin="0 2">
        <GuiControl
            Name="ABTagContainer"
            class="TagListContainer"
            canSaveDynamicFields="0"
            isContainer="1"
            Profile="GuiTransparentProfile"
            HorizSizing="left"
            VertSizing="top"
            Position="0 0"
            Extent="211 109"
            MinExtent="8 2"
            canSave="1"
            Visible="1"
            Active="0"
            tooltipprofile="GuiDefaultProfile"
            hovertime="1000"
            tool="AnimationBuilderGui" />
    </GuiScrollCtrl>
</GuiControl>
*/
function TagListContainer::onAdd(%this)
{
    // create our list of tags for the current object
    if (!isObject(%this.tagItemList))
        %this.tagItemList = new SimSet();

    %this.clearTagItemList();
    
}

function TagListContainer::clearTagItemList(%this)
{
    if ( !isObject(%this.tagItemList) )
        return;

    while (%this.tagItemList.getCount())
        %this.tagItemList.getObject(0).delete();
}

function TagListContainer::populateTagList(%this, %assetID)
{
    %this.clearTagItemList();
    // populate the tag list container with the current asset's tags
    %assetTags = AssetDatabase.getAssetTags();

    %assetTagCount = %assetTags.getAssetTagCount(%assetID);
    %tagButton = %this.createTagButton(-1, "temp");
    %posX = %this.Position.x;
    %posY = %this.Position.y;
    %extentX = %this.Extent.x;
    %extentY = %assetTagCount * %tagButton.Extent.y;
    %this.resize(%posX, %posY, %extentX, %extentY);
    %tagButton.delete();
    if (%this.getCount() > 0)
    {
        %found = false;
        for (%j = 0; %j < %assetTagCount; %j++)
        {
            %taglistcount = %this.tagItemList.getCount();
            for (%l = 0; %l < %taglistcount; %l++)
            {
                if (%this.tagItemList.getObject(%l).tagText $= %assetTags.getAssetTag(%j))
                    %found = true;
            }
        }
        if (!%found)
        {
            %c = %this.tagItemList.getCount();
            for (%k = 0; %k < %c; %k++)
            {
                %button = %this.tagItemList.getObject(%k);
                %this.tagItemList.remove(%button);
                %button.delete();
            }
        }
        %obj = %this.getObject(0);
        while (isObject(%obj))
        {
            %this.remove(%obj);
            %obj = %this.getObject(0);
        }
    }
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTags.getAssetTag(%assetID, %i);
        %this.addTagItem(%tag);
        %button = %this.tagItemList.getObject(%i);
        %button.Position = "2" SPC (%i * %button.Extent.y);
        %this.addGuiControl(%button);
    }
}

function TagListContainer::removeTagItem(%this, %tagName)
{
    if (%tagName $= "")
        return;

    %count = %this.tagItemList.getCount();
    for (%i = 0; %i < %count; %i++)
    {
        %obj = %this.tagItemList.getObject(%i);
        if (%obj.tagText $= %tagName)
        {
            %this.tagItemList.remove(%obj);
            break;
        }
    }
}

function TagListContainer::addTagItem(%this, %tagName)
{
    if (%tagName $= "")
        return;
    
    // create a gui element and add it to the container
    %buttonCount = %this.tagItemList.getCount();
    %tagButton = %this.createTagButton(%buttonCount, %tagName);
    %this.tagItemList.add(%tagButton);
}

function TagListContainer::getTagCount(%this)
{
    return %this.tagItemList.getCount();
}

function TagListContainer::getTagName(%this, %index)
{
    if (%index >= 0 && %index < %this.tagList.getCount())
        return %this.tagItemList.getObject(%index).tagText;
}

function TagListContainer::createTagButton(%this, %index, %tagName)
{
    if (%tagName $= "")
        return;

    %tool = %this.tool;
    %control = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="relative";
        VertSizing="relative";
        Position="0 0";
        Extent="180 29";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        index=%this.getCount();
        tool=%tool;
    };
    
    %removeBtn = new GuiImageButtonCtrl()
    {
        canSaveDynamicFields="0";
        class="DeleteTagButton";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 2";
        Extent="25 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Remove this tag from the asset.";
        wrap="0";
        buttonType="PushButton";
        NormalImage="{EditorAssets}:redCloseImageMap";
        HoverImage="{EditorAssets}:redClose_hImageMap";
        DepressedImage="{EditorAssets}:redClose_dImageMap";
        InactiveImage="{EditorAssets}:redClose_iImageMap";
    };
    %control.addGuiControl(%removeBtn);

    %label = new GuiTextCtrl(){
        canSaveDynamicFields="0";
        internalName = "TagContainerTextControl";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="31 2";
        Extent="140 25";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
        text=%tagName;
        maxLength="1024";
    };
    %control.addGuiControl(%label);
    %control.tagText = %tagName;

    return %control;
}

function DeleteTagButton::onClick(%this)
{
    %parent = %this.getParent();
    %parentTool = %parent.tool;
    %tag = %parent.findObjectByInternalName("TagContainerTextControl").text;
    %parentTool.removeTag(%tag);
}