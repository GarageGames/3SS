//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function WorldButtonPreviewGui::displayImage(%this, %image, %container)
{
    Wbp_ImagePreview.setImage(%image);
    Wbp_ContainerGroup.setVisible(It_WorldToggleCheckbox.getValue());
    
    Wbp_ScoreContainer1.setImage(%container);
    Wbp_ScoreContainer2.setImage(%container);
    Wbp_ScoreContainer3.setImage(%container);

    PreviewLevelLabel.setText(InterfaceTool.levelsText);
    PreviewLevelLabel.setProfile(GuiGameTextCenteredSmProfile);

    PreviewWorldSelectLevelCount.setText(InterfaceTool.scoreCount);
    PreviewWorldSelectLevelCount.setProfile(GuiGameTextCenteredSmProfile);

    PreviewWorldScoreLabel.setText(InterfaceTool.scoreTExt);
    PreviewWorldScoreLabel.setProfile(GuiGameTextCenteredSmProfile);

    PreviewWorldSelectScore.setText(InterfaceTool.totalScore);
    PreviewWorldSelectScore.setProfile(GuiGameTextCenteredSmProfile);

    PreviewRewardCount.setText(InterfaceTool.scoreCount);
    PreviewRewardCount.setProfile(GuiGameTextCenteredSmProfile);
}

function Wbp_DoneBtn::onClick(%this)
{
    Canvas.popDialog(WorldButtonPreviewGui);
}