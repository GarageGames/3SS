//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This handles displaying the level name and sets up the world list dropdown
/// </summary>
function Wt_AssignLevelToWorldGui::display(%this, %levelName)
{
    Wt_AssignLevelWorldList.clear();
    for (%i = 0; %i < WorldTool.worldCount; %i++)
    {
        %worldName = WorldTool.currentWorlds[%i].WorldName;
        if (%i != WorldTool.selectedWorld)
            Wt_AssignLevelWorldList.add(%worldName, %i);
    }
    Wt_AssignLevelWorldList.setFirstSelected();
    Wt_MoveLevelNameEdit.setText(%levelName);
    Canvas.pushDialog(%this);
}

/// <summary>
/// This handles the OK button, telling the tool to move the current level to 
/// the selected world list.
/// </summary>
function Wt_LevelAssignOKBtn::onClick(%this)
{
    WorldTool.assignLevelToWorld(Wt_AssignLevelWorldList.getSelected());
    Canvas.popDialog(Wt_AssignLevelToWorldGui);
}

/// <summary>
/// This cancels the level reassignment.
/// </summary>
function Wt_LevelAssignCancelBtn::onClick(%this)
{
    Canvas.popDialog(Wt_AssignLevelToWorldGui);
}