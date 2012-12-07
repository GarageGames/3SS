//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function LevelListPane::selectPane(%this, %selectedPane)
{
    if ( %this.getCount() < 2 )
    {
        if ( isObject(%this.selectedPane) )
        {
            %this.selectedPane.selected = false;
            %this.selectedPane.setProfile(%this.selectedPane.normalProfile);
        }
        
        %this.selectedPane = %selectedPane;
        %this.selectedPane.selected = true;
        %this.selectedPane.setProfile(%this.selectedPane.highlightProfile);
    }
    else
    {
        if ( isObject(%this.selectedPane) )
        {
            %this.selectedPane.selected = false;
            %this.selectedPane.setProfile(%this.selectedPane.normalProfile);
        }
        
        %this.selectedPane = %selectedPane;
        %this.selectedPane.selected = true;
        %this.SetSelectedLevelPane(%this.selectedPane);
    }
}

function LevelListPane::clearSelectedPane(%this)
{
    if ( isObject( %this.selectedLevelPane ) )
        %this.selectedLevelPane.delete();
}

function LevelListPane::createFirstLevelHighlightBtn(%this, %sourcePane)
{
    if ( isObject( %this.selectedLevelPane ) )
        %this.selectedLevelPane.delete();

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="224 208";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %pane = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLevelPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="1 1";
        Extent="204 208";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
    };
    %pane.normalProfile = %pane.Profile @ "Highlight";
    %pane.highlightProfile = %pane.Profile @ "Highlight";
    %base.addGuiControl(%pane);

    %levelNameLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 8";
        Extent="176 18";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%this.selectedPane.page;
        maxLength="1024";
    };
    %pane.addGuiControl(%levelNameLabel);

    %previewImage = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 30";
        Extent="186 138";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
    };
    %pane.addGuiControl(%previewImage);
    %paneCtrl = duplicateControl(%this.selectedPane.previewCtrl, "Pane");
    %previewImage.addGuiControl(%paneCtrl);

    %previewSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelPanePreviewBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="29 29";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Zoom to display the selected image at 100%.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:magnifyingGlassBackingImageMap";
        HoverImage="{EditorAssets}:magnifyingGlassBacking_hImageMap";
        DownImage="{EditorAssets}:magnifyingGlassBacking_dImageMap";
        InactiveImage="{EditorAssets}:magnifyingGlassBacking_iImageMap";
            index = %this.selectedPane.index;
            preview = %previewImage;
    };
    %pane.addGuiControl(%previewSelectBtn);

    %selectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %pane.addGuiControl(%selectEdit);
    %selectEdit.setText(%this.selectedPane.imageName);
    %base.backgroundEdit = %selectEdit;

    %selectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelSelectPaneIconSelectBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 172";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit=%selectEdit;
            preview=LevelSelectBackground @ %this.selectedPane.index;
            selectedPreview=LevelSelectBackground @ %this.selectedPane.index @ "Pane";
            index=%this.selectedPane.index;
    };
    %pane.addGuiControl(%selectBtn);

    %iconMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
            button=%selectBtn;
    };
    %pane.addGuiControl(%iconMouseEvent);

    %moveRightBtn = new GuiImageButtonCtrl()
    {
        Class="ListPaneMoveBackgroundRightBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="20 37";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Swap page background image with the page to the right.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:rightArrow_normalImage";
        HoverImage="{EditorAssets}:rightArrow_hoverImage";
        DownImage="{EditorAssets}:rightArrow_downImage";
        InactiveImage="{EditorAssets}:rightArrow_inactiveImage";
            index = %this.selectedPane.index;
    };
    %base.addGuiControl(%moveRightBtn);
    %moveBtnPosX = %base.Extent.x - %moveRightBtn.Extent.x - 4;
    %moveBtnPosY = (%base.Extent.y / 2) - (%moveRightBtn.Extent.y / 2);
    %moveRightBtn.setPosition(%moveBtnPosX, %moveBtnPosY);
    %base.pushToBack(%moveRightBtn);

    %this.selectedLevelPane = %base;
    %offset = "4 4";
    %panePosX = %sourcePane.Position.x + %offset.x;
    %panePosY = %sourcePane.Position.y + %offset.y;
    %base.setPosition(%panePosX, %panePosY);
    %this.levelSelectPaneCtrl.addGuiControl(%base);
}

function LevelListPane::createMidLevelHighlightBtn(%this, %sourcePane)
{
    if ( isObject( %this.selectedLevelPane ) )
        %this.selectedLevelPane.delete();

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="244 208";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %pane = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLevelPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="20 1";
        Extent="204 208";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
    };
    %pane.normalProfile = %pane.Profile @ "Highlight";
    %pane.highlightProfile = %pane.Profile @ "Highlight";
    %base.addGuiControl(%pane);

    %levelNameLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 8";
        Extent="176 18";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%this.selectedPane.page;
        maxLength="1024";
    };
    %pane.addGuiControl(%levelNameLabel);

    %previewImage = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 30";
        Extent="186 138";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
    };
    %pane.addGuiControl(%previewImage);
    %paneCtrl = duplicateControl(%this.selectedPane.previewCtrl, "Pane");
    %previewImage.addGuiControl(%paneCtrl);

    %previewSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelPanePreviewBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="29 29";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Zoom to display the selected image at 100%.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:magnifyingGlassBackingImageMap";
        HoverImage="{EditorAssets}:magnifyingGlassBacking_hImageMap";
        DownImage="{EditorAssets}:magnifyingGlassBacking_dImageMap";
        InactiveImage="{EditorAssets}:magnifyingGlassBacking_iImageMap";
            index = %this.selectedPane.index;
            preview = %previewImage;
    };
    %pane.addGuiControl(%previewSelectBtn);

    %selectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %pane.addGuiControl(%selectEdit);
    %selectEdit.setText(%this.selectedPane.imageName);
    %base.backgroundEdit = %selectEdit;

    %selectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelSelectPaneIconSelectBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 172";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit=%selectEdit;
            preview=LevelSelectBackground @ %this.selectedPane.index;
            selectedPreview=LevelSelectBackground @ %this.selectedPane.index @ "Pane";
            index=%this.selectedPane.index;
    };
    %pane.addGuiControl(%selectBtn);

    %iconMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
            button=%selectBtn;
    };
    %pane.addGuiControl(%iconMouseEvent);

    %moveRightBtn = new GuiImageButtonCtrl()
    {
        Class="ListPaneMoveBackgroundRightBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="20 37";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Swap page background image with the page to the right.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:rightArrow_normalImage";
        HoverImage="{EditorAssets}:rightArrow_hoverImage";
        DownImage="{EditorAssets}:rightArrow_downImage";
        InactiveImage="{EditorAssets}:rightArrow_inactiveImage";
            index = %this.selectedPane.index;
    };
    %base.addGuiControl(%moveRightBtn);
    %moveBtnPosX = %base.Extent.x - %moveRightBtn.Extent.x - 5;
    %moveBtnPosY = (%base.Extent.y / 2) - (%moveRightBtn.Extent.y / 2);
    %moveRightBtn.setPosition(%moveBtnPosX, %moveBtnPosY);
    %base.pushToBack(%moveRightBtn);

    %moveLeftBtn = new GuiImageButtonCtrl()
    {
        Class="ListPaneMoveBackgroundLeftBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="20 37";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Swap page background image with the page to the left.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:leftArrow_normalImage";
        HoverImage="{EditorAssets}:leftArrow_hoverImage";
        DownImage="{EditorAssets}:leftArrow_downImage";
        InactiveImage="{EditorAssets}:leftArrow_inactiveImage";
            index = %this.selectedPane.index;
    };
    %base.addGuiControl(%moveLeftBtn);
    %moveBtnPosX = 5;
    %moveBtnPosY = (%base.Extent.y / 2) - (%moveLeftBtn.Extent.y / 2);
    %moveLeftBtn.setPosition(%moveBtnPosX, %moveBtnPosY);
    %base.pushToBack(%moveLeftBtn);

    %this.selectedLevelPane = %base;
    %offset = "4 4";
    %panePosX = %sourcePane.Position.x + %offset.x - 19;
    %panePosY = %sourcePane.Position.y + %offset.y;
    %base.setPosition(%panePosX, %panePosY);
    %this.levelSelectPaneCtrl.addGuiControl(%base);
}

function LevelListPane::createLastLevelHighlightBtn(%this, %sourcePane)
{
    if ( isObject( %this.selectedLevelPane ) )
        %this.selectedLevelPane.delete();

    %base = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="224 208";
        MinExtent="8 2";
        canSave="1";
        Visible="0";
        Active="0";
        hovertime="1000";
    };

    %pane = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiLevelPanelContainerHighlight";
        HorizSizing="right";
        VertSizing="bottom";
        Position="20 1";
        Extent="204 208";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
    };
    %pane.normalProfile = %pane.Profile @ "Highlight";
    %pane.highlightProfile = %pane.Profile @ "Highlight";
    %base.addGuiControl(%pane);

    %levelNameLabel = new GuiTextCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 8";
        Extent="176 18";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        text=%this.selectedPane.page;
        maxLength="1024";
    };
    %pane.addGuiControl(%levelNameLabel);

    %previewImage = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 30";
        Extent="186 138";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        wrap="0";
        useSourceRect="0";
        sourceRect="0 0 0 0";
    };
    %pane.addGuiControl(%previewImage);
    %paneCtrl = duplicateControl(%this.selectedPane.previewCtrl, "Pane");
    %previewImage.addGuiControl(%paneCtrl);

    %previewSelectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelPanePreviewBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="29 29";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Zoom to display the selected image at 100%.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:magnifyingGlassBackingImageMap";
        HoverImage="{EditorAssets}:magnifyingGlassBacking_hImageMap";
        DownImage="{EditorAssets}:magnifyingGlassBacking_dImageMap";
        InactiveImage="{EditorAssets}:magnifyingGlassBacking_iImageMap";
            index = %this.selectedPane.index;
            preview = %previewImage;
    };
    %pane.addGuiControl(%previewSelectBtn);

    %selectEdit = new GuiTextEditCtrl()
    {
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiFileEditBoxProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        maxLength="1024";
        historySize="0";
        password="0";
        tabComplete="0";
        sinkAllKeyEvents="0";
        passwordMask="*";
        truncate="1";
    };
    %pane.addGuiControl(%selectEdit);
    %selectEdit.setText(%this.selectedPane.imageName);
    %base.backgroundEdit = %selectEdit;

    %selectBtn = new GuiImageButtonCtrl()
    {
        Class="It_LevelSelectPaneIconSelectBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 172";
        Extent="22 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:assetImageMap";
        HoverImage="{EditorAssets}:asset_hImageMap";
        DownImage="{EditorAssets}:asset_dImageMap";
        InactiveImage="{EditorAssets}:asset_iImageMap";
            edit=%selectEdit;
            preview=LevelSelectBackground @ %this.selectedPane.index;
            selectedPreview=LevelSelectBackground @ %this.selectedPane.index @ "Pane";
            index=%this.selectedPane.index;
    };
    %pane.addGuiControl(%selectBtn);

    %iconMouseEvent = new GuiMouseEventCtrl()
    {
        class="It_AssetBrowseEditClick";
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTransparentProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="8 172";
        Extent="157 25";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select the image file from the Asset Picker for the background image for the selected page of levels.";
        groupNum="-1";
            button=%selectBtn;
    };
    %pane.addGuiControl(%iconMouseEvent);

    %moveLeftBtn = new GuiImageButtonCtrl()
    {
        Class="ListPaneMoveBackgroundLeftBtn";
        canSaveDynamicFields="0";
        isContainer="0";
        Profile="GuiDefaultProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="164 138";
        Extent="20 37";
        MinExtent="8 2";
        canSave="0";
        Visible="1";
        Active="1";
        hovertime="1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Swap page background image with the page to the left.";
        groupNum="-1";
        buttonType="PushButton";
        useMouseEvents="0";
        isLegacyVersion="0";
        NormalImage="{EditorAssets}:leftArrow_normalImage";
        HoverImage="{EditorAssets}:leftArrow_hoverImage";
        DownImage="{EditorAssets}:leftArrow_downImage";
        InactiveImage="{EditorAssets}:leftArrow_inactiveImage";
            index = %this.selectedPane.index;
    };
    %base.addGuiControl(%moveLeftBtn);
    %moveBtnPosX = 5;
    %moveBtnPosY = (%base.Extent.y / 2) - (%moveLeftBtn.Extent.y / 2);
    %moveLeftBtn.setPosition(%moveBtnPosX, %moveBtnPosY);
    %base.pushToBack(%moveLeftBtn);

    %this.selectedLevelPane = %base;
    %offset = "4 4";
    %panePosX = %sourcePane.Position.x + %offset.x - 19;
    %panePosY = %sourcePane.Position.y + %offset.y;
    %base.setPosition(%panePosX, %panePosY);
    %this.levelSelectPaneCtrl.addGuiControl(%base);
}

function LevelListPane::SetSelectedLevelPane(%this)
{
    if ( isObject ( %this.levelSelectPaneCtrl ) )
    {
        if (%this.isMember(%this.levelSelectPaneCtrl))
            %this.remove(%this.levelSelectPaneCtrl);
        %this.levelSelectPaneCtrl.delete();
    }
    %this.levelSelectPaneCtrl = new GuiControl()
    {
        canSaveDynamicFields="0";
        isContainer="1";
        Profile="GuiTransparentProfileModeless";
        HorizSizing="right";
        VertSizing="bottom";
        Position="0 0";
        Extent="122 500";
        MinExtent="8 2";
        canSave="1";
        Visible="1";
        Active="0";
        hovertime="1000";
    };
    %scrollCtrl = %this.getParent();
    %size = %this.Extent;
    %posY = %scrollCtrl.getScrollPositionY();
    %scrollCtrl.add(%this.levelSelectPaneCtrl);
    %this.levelSelectPaneCtrl.setExtent(%size.x, %size.y);
    %this.levelSelectPaneCtrl.setPosition(0, 0 - %posY);

    %listCount = %this.getCount();
    if ( %this.selectedPane.index == 0 )
        %this.createFirstLevelHighlightBtn(%this.selectedPane);
    else if ( %this.selectedPane.index == ( %listCount - 1 ) )
        %this.createLastLevelHighlightBtn(%this.selectedPane);
    else
        %this.createMidLevelHighlightBtn(%this.selectedPane);

    %this.selectedLevelPane.SetVisible(true);
}

function ListPaneMoveBackgroundLeftBtn::onClick(%this)
{
    // swap level select page background with the page to the left
    %world = InterfaceTool.worldData.getObject(It_LevelListWorldSelectDropdown.getSelected());
    %selectedIndex = It_LevelListContentPane.selectedPane.index;
    %tempAssetID = %world.WorldBackground[%selectedIndex - 1];
    %prevEdit = It_LevelListContentPane.getObject(%selectedIndex - 1).backgroundEdit;
    %selectedEdit = %this.getParent().backgroundEdit;
    %currentEdit = It_LevelListContentPane.getObject(%selectedIndex).backgroundEdit;
    %tempName = %prevEdit.getText();
    %prevEdit.setText(%currentEdit.getText());
    %selectedEdit.setText(%tempName);
    %currentEdit.setText(%tempName);
    %world.WorldBackground[%selectedIndex - 1] = %world.WorldBackground[%selectedIndex];
    %world.WorldBackground[%selectedIndex] = %tempAssetID;
    %prevPane = LevelSelectBackground @ %selectedIndex - 1;
    %prevPane.setImage(%world.WorldBackground[%selectedIndex - 1]);
    %currentPane = LevelSelectBackground @ %selectedIndex;
    %currentPane.setImage(%world.WorldBackground[%selectedIndex]);
    %selectedPane = LevelSelectBackground @ %selectedIndex @ "Pane";
    %selectedPane.setImage(%world.WorldBackground[%selectedIndex]);

    InterfaceTool.saveData();
}

function ListPaneMoveBackgroundRightBtn::onClick(%this)
{
    // swap level select page background with the page to the right
    %world = InterfaceTool.worldData.getObject(It_LevelListWorldSelectDropdown.getSelected());
    %selectedIndex = It_LevelListContentPane.selectedPane.index;
    %tempAssetID = %world.WorldBackground[%selectedIndex + 1];
    %nextEdit = It_LevelListContentPane.getObject(%selectedIndex + 1).backgroundEdit;
    %selectedEdit = %this.getParent().backgroundEdit;
    %currentEdit = It_LevelListContentPane.getObject(%selectedIndex).backgroundEdit;
    %tempName = %nextEdit.getText();
    %nextEdit.setText(%currentEdit.getText());
    %selectedEdit.setText(%tempName);
    %currentEdit.setText(%tempName);
    %world.WorldBackground[%selectedIndex + 1] = %world.WorldBackground[%selectedIndex];
    %world.WorldBackground[%selectedIndex] = %tempAssetID;
    %nextPane = LevelSelectBackground @ %selectedIndex + 1;
    %nextPane.setImage(%world.WorldBackground[%selectedIndex + 1]);
    %currentPane = LevelSelectBackground @ %selectedIndex;
    %currentPane.setImage(%world.WorldBackground[%selectedIndex]);
    %selectedPane = LevelSelectBackground @ %selectedIndex @ "Pane";
    %selectedPane.setImage(%world.WorldBackground[%selectedIndex]);

    InterfaceTool.saveData();
}