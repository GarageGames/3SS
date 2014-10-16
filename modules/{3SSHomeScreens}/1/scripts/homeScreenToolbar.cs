//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// Loads the Asset Library
/// </summary>
function Hs_HomeButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);
    
    // Clear all the views from the shell
    EditorShellGui.clearViews();
    
    // Push the template selector view
    EditorShellGui.addView(TemplateListGui, "");
}

/// <summary>
/// Tests the current project
/// </summary>
function Hs_MyGamesButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);
    
    showAllGames();
}

function ResetCommonToolButtons()
{
    Hs_HomeButton.setStateOn(false);
    Hs_MyGamesButton.setStateOn(false);
}