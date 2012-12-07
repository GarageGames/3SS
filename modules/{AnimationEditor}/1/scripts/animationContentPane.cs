//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function AnimationContentPane::createFrameControl(%this, %frame)
{
    %preview = new GuiSpriteCtrl()
    {
        superClass = ImageMapPreviewSprite;
        class = "ABImageMapPreviewSprite";
        Profile = "GuiDragAndDropProfile";
        position = "0 0";
        extent = "110 110";
        MinExtent = "2 2";
        HorizSizing = "relative";
        VertSizing = "relative";
        Image = %this.image;
        Frame = %frame;
    };

    %preview.sceneObject = %preview;
    %mouseEvent = new GuiMouseEventCtrl() {
        canSaveDynamicFields = "0";
        Profile = "GuiDefaultProfile";
        class = "Ab_FrameControl";
        HorizSizing = "relative";
        VertSizing = "relative";
        Position = "0 0";
        Extent = "110 110";
        MinExtent = "2 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
        tooltipprofile="GuiToolTipProfile";
        ToolTip="Each frame is displayed in order from left to right starting in the top left corner. You can drag and drop individual frames to the Animation Timeline.";
    };
    %preview.add(%mouseEvent);

    return %preview;
}

function AnimationContentPane::display(%this, %image)
{
    %this.image = %image;
    %this.imageAsset = AssetDatabase.acquireAsset(%image);
    %frameCount = %this.imageAsset.getFrameCount();
    for (%i = 0; %i < %frameCount; %i++)
    {
        %this.add(%this.createFrameControl(%i));
    }
    AssetDatabase.releaseAsset(%image);
    AnimBuilderEventManager.postEvent("_AnimContentPaneUpdateComplete", "Ab_ImageMapPreviewWindow " @ %frameCount);
}

function AnimationContentPane::update(%this, %frameCount)
{
    %rowCount = mCeil(%frameCount/4);
    %rowHeight = %this.rowSize + %this.rowSpacing;
    %height = %rowCount * %rowHeight;
    %width = 4 * (%this.colSize + %this.colSpacing);
    %this.resize(%this.Position.x, %this.Position.y, %width, %height);
}