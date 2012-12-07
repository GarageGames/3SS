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
    EditorShellGui.addView(AnnouncementsGui, "");
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

/// <summary>
/// Publishes the current project
/// </summary>
function Hs_StoreButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);

    WebWindowGui.display("http://3stepstudio.com/shop");
}

/// <summary>
/// Exits the Editor
/// </summary>
function Hs_HelpButton::onClick(%this)
{
    ResetCommonToolButtons();
    %this.setStateOn(true);

    gotoWebPage("http://docs.3stepstudio.com/");
}

function ResetCommonToolButtons()
{
    Hs_HomeButton.setStateOn(false);
    Hs_MyGamesButton.setStateOn(false);
    Hs_StoreButton.setStateOn(false);
    Hs_HelpButton.setStateOn(false);
}